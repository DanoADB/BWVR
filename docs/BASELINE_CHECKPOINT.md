# Editor baseline checkpoint — 2026-10-26

S0 of [BUILD_SEQUENCE.md](BUILD_SEQUENCE.md). This records a targeted Editor baseline, not complete regression, balance, APK or hardware certification.

## Ownership and event integration points

| Concern | Current boundary |
| --- | --- |
| Time / pause | QuestSlice.Simulate advances Hybrid.time and schedules actors/resources/weather. CreatureHybrid.Tick can advance its own clock for isolated checks; do not advance both for the same step. Exploration focus keeps autonomy/time running while suppressing drought pressure. |
| Creature actions | CreatureHybrid owns Water, Curiosity or Observe. Complete emits the attributable receipt. Interrupt resets navigation/carry/controllers and invalidates teaching. Suspend retains only a safely resumable logical intention. |
| Learning | HybridState.Reinforce consumes an eligible completed receipt once within 12 seconds; CreatureHybrid.Feedback integrates category/context/partner learning. Miracles do not become creature achievements. |
| Placement / guidance | QuestSlice.Release notifies placement attention. GodToolPalette.Apply validates and commits. Preview is read-only. Cue noticed, action selected and action completed are distinct events. |
| Carry / reservations | QuestSlice owns player hands; resource sources, blocks and social/play controllers hold their existing claims. Never steal a player-held object or replace a claimed movement/carry controller without cleanup. |
| Persistence | QuestSlice.Save captures construction/discoveries/building into SliceState. Load validates logical state; systems restore physical state and recover interrupted jobs. Palette previews and held inputs do not resume. |
| History | WorldEventJournal is a transient 48-entry ring. Future permanent evidence needs bounded saved records and deduplicated confirmed events. |

## Earlier failures

DevelopmentFocusChecks reproduced a null record in HarvestSeconds when replacing its first synthetic season with a second fresh state while a live harvesting job retained its source. The fresh state had no corresponding resource-source record. Production's new-season path already calls resource ResetWorld; this fixture did not.

The check now cancels resource/social/exploration/block/routine jobs before state replacement, registers sources through ResetWorld afterward, and restores/cleans up in finally. The corrected check passed. No gameplay water priority or production resource behavior was changed to satisfy it.

HybridChecks.VerifyRuntime passed before the fixture change and again in the final combined run. The historical intermittent water-receipt failure was not reproduced in those two calls; its cause is not established. This is not a proven production fix. Capture actor position, route, deadline, owner and journal if it recurs; an isolated actor loop does not exercise all village traffic conditions.

## Verification

Unity 6000.6.7f1, native Editor Play mode. All nine targeted entries passed:

- HybridChecks.VerifyRuntime and VerifyModel.
- DevelopmentFocusChecks.Verify.
- QuestControlsChecks.Verify: analog precision, travel at minimum/intermediate/maximum view scales, zoom focus/limits, snap-turn rearm and right side-grip binding.
- PopulationChecks.Verify and ResourceChecks.VerifyModel.
- GodToolsChecks.Verify: inactive original templates, all five nonempty summoned symbols, pointer selection, press/release, exact costs, saved guidance placement/removal and tracking/input cleanup.
- MiracleChecks.Verify and CreatureCarryChecks.Verify.

Results were written by the running Editor to ignored `.local/baseline/verified-tests.json`. Live frames advanced from 811 to 5048. Capture-stage state was valid, with twelve adults, creature seed 1347, clear weather, rain 0 and wind 0.12. Individual checks use their own synthetic states; these snapshot values are not one shared deterministic weather fixture.

An extra paused Simulate call did not advance time. A real device-file Save was read back and validated against current logical time, creature seed and wheat stock. This verifies writing/deserialization, not a complete Editor restart/load migration. Existing targeted checks separately cover logical round-trips and guidance restoration.

An actual 1440×900 camera render was inspected: the palette retained all five symbols after hiding the old props. Active original intention-template count was zero. Gameplay checks ended with no current compilation failures/errors and two XR audio-output fallback warnings. An earlier XR package-list timeout and a later tooling-only error from an obsolete console command were observed; the command was corrected to console get. Do not claim an entirely clean console.

After confirmed Play stop, the original user season file was restored and SHA-256 matched its backup. Unrelated XR/project settings were preserved. No APK was produced; physical Quest comfort, audio routing, control speed and standalone performance remain unverified.

## Repeatable setup / next feature

Back up the device season, start FirstQuestSlice, confirm live frames, then run the targeted menu checks above from a fresh Play session. GodToolsChecks expects a fresh palette use count; cancel/release tool input before ordinary pickup checks. These tests replace live logical state: stop and restore the backup afterward. Use fixed model seeds for decision comparisons; record positions/weather separately for world/traffic checks.

Proceed to S1 attention/response and bounded evidence work. Keep exploration focus and water secondary. The immediate control/sign adjustments do not mark S1 or audio S1A complete.

## S1 verification follow-up

The old isolated HybridChecks water phase sometimes stuck against villagers that the test never ticked. A diagnostic run confirmed blocked navigation and zero deliveries over 200 simulated seconds; the comparison with frozen NPC bodies excluded completed delivery. Only that isolated phase now temporarily excludes those bodies and restores them before the independent village-progress check and in cleanup. Live crowd navigation is not proven by this fixture. The feedback fixture also uses a temporary ordinary toy rather than borrowing a beachball that live villagers may have reserved. These changes do not raise gameplay water priority or bypass real cue occlusion.

S1 response/evidence model and runtime checks passed. A native-frame gorilla render verified aligned eye/head animation; headset comfort remains untested. See CREATURE_RESPONSES.md for scope.

Final S1 regression run passed responses/evidence, hybrid model/runtime, development focus, creature skills, god tools, miracles, creature carrying, feedback and population. The new village observation check initially stopped at a birth-fixture cooldown; its corrected complete body then passed through the live Editor evaluator. The final scene advanced from frame 5511 to 7529, observation active with zero food work and one child. The console reported no current compilation failures or gameplay errors at inspection; pre-existing obsolete API warnings and earlier tooling/test failures remain in historical output. These are Editor checks, not a Quest device test.
