"""Regression checks for LND cell orientation and malformed inputs, using synthetic data."""
import importlib.util
import struct
import tempfile
import unittest
from pathlib import Path

spec = importlib.util.spec_from_file_location("island", Path(__file__).with_name("prepare-island.py"))
island = importlib.util.module_from_spec(spec)
spec.loader.exec_module(island)


class IslandReaderTests(unittest.TestCase):
    def setUp(self):
        self.folder = tempfile.TemporaryDirectory(dir=".local")
        self.path = Path(self.folder.name) / "synthetic.lnd"

    def tearDown(self):
        self.folder.cleanup()

    def data(self):
        data = bytearray(1052 + 2520)
        struct.pack_into("<I", data, 0, 2)
        struct.pack_into("<6I", data, 1028, 0, 0, 2520, 0, 0, 0)
        for x in range(17):
            for z in range(17):
                data[1052 + (x * 17 + z) * 8 + 4] = x * 10 + z
                data[1052 + (x * 17 + z) * 8 + 6] = 128
        return data

    def test_x_major_cells_and_split_flags(self):
        self.path.write_bytes(self.data())
        result = island.read_island(self.path)
        self.assertEqual(result["heights"][3 * 513 + 7], 73)
        self.assertEqual(result["flags"][3 * 513 + 7], 128)
        self.assertEqual(result["heights"][16 * 513 + 7], -1)
        self.assertEqual(result["heightUnit"], .67)

    def test_truncated_header(self):
        self.path.write_bytes(bytes(100))
        with self.assertRaisesRegex(ValueError, "header"):
            island.read_island(self.path)

    def test_truncated_blocks(self):
        self.path.write_bytes(self.data()[:-1])
        with self.assertRaisesRegex(ValueError, "blocks"):
            island.read_island(self.path)

    def test_inconsistent_block_coordinates(self):
        data = self.data()
        struct.pack_into("<f", data, 1052 + 2316, 160)
        self.path.write_bytes(data)
        with self.assertRaisesRegex(ValueError, "coordinates"):
            island.read_island(self.path)


if __name__ == "__main__":
    unittest.main()
