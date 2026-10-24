"""Extract local placement records from an owned Land1.txt; never executes script code."""
import argparse
import csv
import hashlib
import json
import math
import re
from collections import Counter
from pathlib import Path


def read_placements(text):
    records, skipped = [], Counter()
    for line, source in enumerate(text.splitlines(), 1):
        match = re.fullmatch(r"\s*(\w+)\((.*)\)\s*", source)
        if not match:
            continue
        command, arguments = match.groups()
        args = next(csv.reader([arguments], skipinitialspace=True, strict=True))
        record = dict(line=line, command=command, kind="", type="", town=-1,
                      x=0, z=0, yaw=0, size=1, variant=0, vertical=0, pitch=0, roll=0)
        position = None
        try:
            if command in ("CREATE_ABODE", "CREATE_TOWN_CENTRE"):
                record.update(kind="building", town=int(args[0]), type=args[2],
                              yaw=-float(args[3]) * .001 * 180 / math.pi, size=float(args[4]) * .001)
                position = args[1]
            elif command in ("CREATE_NEW_TREE", "CREATE_TREE"):
                record.update(kind="tree", variant=int(args[2]), type=args[2])
                position = args[1]
                if command == "CREATE_NEW_TREE":
                    record.update(yaw=-float(args[4]) * 180 / math.pi, size=float(args[5]))
                else:
                    record.update(yaw=-float(args[3]) * .001 * 180 / math.pi, size=float(args[4]) * .001)
            elif command == "CREATE_DEAD_TREE":
                position = args[0]
                record.update(kind="tree", variant=22, type="22", size=float(args[3]), yaw=-float(args[5]) * 180 / math.pi)
            elif command == "CREATE_NEW_TOWN_FIELD":
                position = args[1]
                record.update(kind="field", town=int(args[0]), type=args[2], yaw=-float(args[3]) * 180 / math.pi)
            elif command == "CREATE_NEW_FEATURE":
                position = args[0]
                record.update(kind="feature", type=args[1], yaw=-float(args[2]) * .001 * 180 / math.pi, size=float(args[3]) * .001)
            elif command == "CREATE_MOBILE_STATIC" and (int(args[1]) == 2 or 16 <= int(args[1]) <= 47):
                position = args[0]
                # Mobile statics use radians, not the legacy milli-radians convention.
                record.update(kind="rock", variant=int(args[1]), type=args[1], yaw=-float(args[4]) * 180 / math.pi, size=float(args[6]), vertical=float(args[2]), pitch=-float(args[3])*180/math.pi, roll=-float(args[5])*180/math.pi)
            elif command == "CREATE_MOBILEOBJECT" and int(args[1]) in (29, 30):
                position = args[0]
                record.update(kind="well", type=args[1], yaw=-float(args[2]) * .001 * 180 / math.pi, size=float(args[3]) * .001)
            elif command == "CREATE_DRINK_WAYPOINT":
                position = args[0]
                record.update(kind="drink", type="drinking waypoint")
            elif command == "CREATE_VILLAGER_POS":
                position = args[1]
                record.update(kind="villager", type=args[2])
            else:
                if command.startswith("CREATE_"):
                    skipped[command] += 1
                continue
            record["x"], record["z"] = map(float, position.split(","))
            if not all(math.isfinite(record[k]) for k in ("x", "z", "yaw", "size")) or not 0 < record["size"] <= 100:
                raise ValueError("Invalid placement numbers")
        except (ValueError, IndexError) as error:
            raise ValueError(f"Invalid {command} at line {line}: {error}") from error
        records.append(record)
    return records, dict(skipped)


if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("source", type=Path)
    parser.add_argument("--terrain", type=Path, default=Path(".local/land1.json"))
    args = parser.parse_args()
    records, skipped = read_placements(args.source.read_text())
    terrain = json.loads(args.terrain.read_text())
    terrain.update(placements=records, placementHash=hashlib.sha256(args.source.read_bytes()).hexdigest())
    args.terrain.write_text(json.dumps(terrain, separators=(",", ":")))
    print(json.dumps(dict(mapped=dict(Counter(r["kind"] for r in records)), unsupported=skipped), indent=2))
