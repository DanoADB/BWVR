# Play Day — player-led outing and field guide

Delivered 2026-10-29 for desktop Editor testing. This increment gives the player a reason to try BWVR's existing physical play systems, instructions for finding/using them, and short readings earned through actual shared outcomes. It does not introduce another creature controller or guarantee cooperation.

## How to play

Open the Sanctuary and choose Book of Creation. It initially opens to Outings; use Sections to switch categories. The Book uses rounded rectangular controls and initially opens to Outings; its other sections remain available. Choose Begin Play Day. Complete these experiments in any order, without a deadline:

1. Play a witnessed four-note phrase on the chime and let the creature complete an echo.
2. Use a goal's jewel to invite shared turns. Complete three rounds containing both turns and physical returns. Misses count as shared play; incomplete turns do not. Rounds can span invitations.
3. Dock the Together gate's key, let the creature choose the paw pad, and send the physical beachball through the open gate.

Turn pages for instructions and field notes. Mark activity closes the menu and places a quiet teal, collider-free beacon at that page's activity for 45 simulation seconds. From the overview, it marks the first unfinished activity. It does not move the camera, teleport anyone or issue a creature command. The marker follows a moved goal and ends on expiry, target loss, save reload, ending the outing or starting another outing. Pause/suspend freeze its timer. No new controller bindings are introduced: use the existing tracked, press-edge Book input and normal world interactions.

Each confirmed experiment unlocks a short authored field note about shared play. Three successes finish the outing with a gentle sound and saved completion count. End outing stops recording, clears the marker and retains learned behavior and unlocked notes; it does not stop an ongoing physical game. Play again starts new baselines and keeps past notes readable. The Sanctuary privately summarizes active progress. There is no permanent HUD, mandatory quest, currency grant or penalty for leaving.

## Progress and persistence

PlayOutingState is an optional bounded addition to SliceState version 1. Historical saves remain dormant (Unity may deserialize an empty inline object). Reading pages does not initialize or change simulation history. Begin records the existing echo-games, shared-rounds and puzzle-solved counters; earlier achievements never auto-complete a new outing. Progress uses only the confirmed counters from the existing player-led activities. Ordinary creature play, feedback, touching a marker or reading the Book do not award progress.

QuestSlice observes those counters after ordinary simulation work. Pause/suspend prevent recording. Finished experiments and outing completion are idempotent; partial rounds, baselines, completed outings and note unlocks survive native Save/Load. The ephemeral beacon is intentionally not saved. A new season resets outings with the rest of that season; ending/replaying an outing does not reset the world.

The field notes are curated interpretations of these specific play outcomes, not claims of general intelligence or previously unrecorded emergent discoveries. Optional text complements existing gaze, affect, sounds and physical play. Water/food priorities, test energy, weather, creature utility and player pickup controls remain unchanged.

## Verification

Unity 6000.6.7f1 compilation and PlayOutingChecks model/runtime pass: old-save/pure reading, start baselines, pause, partial progress, once-only completion, replay/end, retained notes, malformed state, tracked-edge UI, held-object protection, marker lifecycle and native Save/Load. CreationBookChecks and SanctuaryChecks pass, including all section pages fitting and input isolation.

PlayOutingChecks.VerifyActivities separately stages the real native activity fixtures: witnessed four-note input/utility echo, three physical shared goal rounds with carries/returns, and native key/pad/ball puzzle completion. Their confirmed counters earn all three outing experiments. These are staged source-event checks; they do not establish an uninterrupted natural full-population playthrough or a guaranteed acceptance rate. Runtime orchestration is additionally tested using published fixture counters, explicitly identified in the check source.

Seven guide pages and an unlocked note were rendered and inspected. User save and XR initialization settings are restored after QA. Headset readability, beacon visibility across the full island, comfort and subjective enjoyment remain playtest gates. No APK built.

## Next engagement increments

Use playtesting to tune discoverability and participation before adding more checklists. Candidate follow-ups: an exploration outing with meaningful finds, a shared construction challenge with several valid solutions, and storm recovery pages grounded in saved outcomes. Existing storm/fire milestones remain on the broader roadmap.
