# Whose turn?

Build & Bowl now has private world-space light guides for its existing handoffs:

- **Gold single ring around the ball:** available for your next shot or rebuild.
- **Blue ball, approach and target guides:** a creature turn is invited. The small
  arrow shows the planned roll direction at the approach, not a navigation path.
  While the creature carries the ball, the ball ring hides and the approach/target
  guides remain. The creature can still choose another activity.
- **Amber double ring:** a player or creature shot is inside its safety window.
  The ring does not claim a hit or completion.
- **Gold again after a safe creature turn settles:** you can retrieve the ball
  and make your next shot. There is no automatic extra fetch or forced match.

The existing fetch destination circle and returned-ball rebuilding cues remain
available. Book → Projects includes the short color legend. No controller binding,
tool, model, permanent HUD, new action, score rule or learning bonus is added.

## Implementation boundaries

The view reads existing project, invitation, carry and flight state. It never calls
the aim planner, claims an object, changes time, emits simulation events or records
progress. It introduces no saved state. A current invitation is shown as an
opportunity; execution still performs all existing safety and feasibility checks.

Each ball lazily owns four fixed light renderers with 83 positions total and one
copy of the existing construction-preview material. There are no colliders,
CarryItems, SpatialTargets or registered AI targets. Ground-aligned rings account
for the raised construction yard. Visible positions follow the ball each rendered
simulation update without scene scans, candidate ranking or animated flashing.
Renderers hide when paused, inactive, ended, outside this activity, or when the
player holds the ball. Destruction disposes the material and light root.

## Verification and limits

`BowlHandoffChecks.Verify` observes the native creature-turn fixture: real safe
player shot, physical return, player rebuild, autonomous creature lift/carry/roll,
physical target contact and the existing result confirmation. It checks offered,
carrying, settling and player phases; exact visible-line counts; fixed geometry;
absence of physical/AI affordances; pause and player-held exclusion; and unchanged
serialized state and yard reservations after refreshing the view.

The fixture constrains competing activities, parks villagers and stages a visible
placement as documented in [BOWL_CREATURE_TURNS.md](BOWL_CREATURE_TURNS.md). Native
phase captures were inspected in the Editor. The Book and existing ball-game
regressions cover the updated legend and reused gameplay. This does not validate
natural full-population pacing, color readability or comfort inside Quest; those
remain physical playtest gates. No APK is built for this increment.

Next: tune course spacing, aim and turn participation from a Quest playtest before
expanding the activity. Other roadmap capabilities can proceed independently.
