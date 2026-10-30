# Optional first steps

Sanctuary → **Start here** introduces the unseen-player fiction, movement/zoom, carrying and contextual A/B teaching in three short pages. Desktop controls appear when XR is inactive. It uses the existing scroll and uniform rounded stone buttons; God Tools stay circular.

**Explore freely** closes the menu. **Find the chime** places the existing private 45-simulation-second location beacon without starting an outing or commanding the creature. Pause, suspension and held objects block placement. **Open Play Day** opens the Book's Outings overview; beginning or replaying remains a separate player choice there. Back returns to Sanctuary. No page opens automatically, changes the camera, resets learning or marks activities complete. No save schema changes.

## Verification

Unity 6000.6.7f1 desktop Editor compilation passed. `FirstStepsChecks.Verify()` covers hidden startup, exclusive page routing, physical ray hit targets, tracked right-hand press edges, page boundaries, rendered text geometry bounds, pause/suspend/carry protection, actual beacon placement and unchanged serialized season state when reading/marking/opening Play Day. Existing Sanctuary, Creation Book and Play Day runtime checks passed after the change.

Captured and visually inspected Sanctuary plus all three guide pages. The first render exposed disabled text wrapping; explicit normal wrapping fixed it, and text geometry bounds are now checked as well as TMP overflow flags. Original user save and XR startup flags are restored after testing.

This is an optional orientation aid, not a completed first-time gameplay tutorial or proof that strangers can finish a 15–20 minute demo. Physical Quest readability, natural discovery/creature acceptance and first-time player comprehension still need playtesting. Next demo work should test this path with new players and improve in-world invitations based on where they get stuck, rather than expanding compulsory instructions.
