"""Synthetic placement tests; contains no original island data."""
import importlib.util
import math
from pathlib import Path
import unittest

spec = importlib.util.spec_from_file_location("placements", Path(__file__).with_name("prepare-placements.py"))
placements = importlib.util.module_from_spec(spec)
spec.loader.exec_module(placements)


class PlacementTests(unittest.TestCase):
    def test_legacy_and_float_rotation_units(self):
        records, _ = placements.read_placements('CREATE_ABODE(0, "100,200", "HOME", 1571, 800, 0, 0)\nCREATE_NEW_TREE(-1, "110,220", 4, 1, 1.571, .6, 1)')
        self.assertAlmostEqual(records[0]["yaw"], -1.571 * 180 / math.pi)
        self.assertEqual(records[0]["yaw"], records[1]["yaw"])
        self.assertEqual(records[1]["size"], .6)  # Current size, not eventual maximum.
        self.assertEqual((records[0]["x"], records[0]["z"]), (100, 200))
        self.assertEqual(records[1]["line"], 2)

    def test_furniture_is_not_mistaken_for_a_well(self):
        records, skipped = placements.read_placements('CREATE_MOBILEOBJECT("100,200", 18, 0, 1000)\nCREATE_MOBILEOBJECT("110,220", 29, 0, 1000)')
        self.assertEqual(len(records), 1)
        self.assertEqual(records[0]["kind"], "well")
        self.assertEqual(skipped["CREATE_MOBILEOBJECT"], 1)

    def test_rock_offset_and_rotation(self):
        records, _ = placements.read_placements('CREATE_MOBILE_STATIC("100,200", 2, .4, .1, .2, .3, .7)')
        self.assertEqual(records[0]["vertical"], .4)
        self.assertEqual(records[0]["size"], .7)
        self.assertAlmostEqual(records[0]["yaw"], -.2 * 180 / math.pi)

    def test_invalid_coordinates_fail_with_line(self):
        with self.assertRaisesRegex(ValueError, "line 1"):
            placements.read_placements('CREATE_DRINK_WAYPOINT("nan,200")')


if __name__ == "__main__":
    unittest.main()
