"""Read height/split cells from an owned Land1.lnd; never exports original textures.

Layout reference: openblack components/lnd/include/LNDFile.h (1052-byte header,
2520-byte blocks, 8-byte cells). Output is local game data, not source artwork.
"""
import argparse
import hashlib
import json
import struct
from pathlib import Path


def read_island(source):
    data = source.read_bytes()
    if len(data) < 1052:
        raise ValueError("Truncated LND header")
    count = struct.unpack_from("<I", data)[0]
    _, _, block_size, _, _, low_count = struct.unpack_from("<6I", data, 1028)
    if block_size != 2520 or not 1 < count <= 1025 or low_count > 1024:
        raise ValueError("Unsupported LND block header")
    offset = 1052
    for _ in range(low_count):
        if offset + 20 > len(data):
            raise ValueError("Truncated low-resolution texture header")
        size = struct.unpack_from("<I", data, offset + 16)[0]
        if size < 4 or offset + 16 + size > len(data):
            raise ValueError("Invalid low-resolution texture size")
        offset += 16 + size
    if offset + (count - 1) * block_size > len(data):
        raise ValueError("Truncated terrain blocks")
    heights, flags, blocks = [-1] * (513 * 513), [0] * (513 * 513), []
    for index in range(count - 1):
        start = offset + index * block_size
        _, map_x, map_z, bx, bz = struct.unpack_from("<IffII", data, start + 2312)
        if bx >= 32 or bz >= 32 or abs(map_x - bx * 160) > .1 or abs(map_z - bz * 160) > .1:
            raise ValueError(f"Inconsistent block coordinates: {bx}, {bz}, {map_x}, {map_z}")
        blocks.append([bx, bz])
        # Ignore the duplicate outer row/column; neighbours provide authoritative cells.
        for z in range(16):
            for x in range(16):
                at = start + (x * 17 + z) * 8
                target = (bz * 16 + z) * 513 + bx * 16 + x
                heights[target], flags[target] = data[at + 4], data[at + 6]
    occupied = [(i % 513, i // 513) for i, h in enumerate(heights) if h > 2]
    if not occupied:
        raise ValueError("No land found")
    bounds = [min(p[0] for p in occupied), min(p[1] for p in occupied),
              max(p[0] for p in occupied), max(p[1] for p in occupied)]
    return dict(version=1, width=513, cellSize=10, heightUnit=.67,
                heights=heights, flags=flags, blocks=blocks, bounds=bounds,
                sha256=hashlib.sha256(data).hexdigest(), source=source.name)


if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("source", type=Path)
    parser.add_argument("--output", type=Path, default=Path(".local/land1.json"))
    args = parser.parse_args()
    result = read_island(args.source)
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(json.dumps(result, separators=(",", ":")))
    print(json.dumps({key: result[key] for key in ("source", "bounds", "sha256")}))
    print(f"Exported {len(result['blocks'])} blocks; original textures excluded")
