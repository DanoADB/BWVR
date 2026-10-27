# Gathering-place scheduler validation

The S4 slice now has a composed Editor check: **BWVR → Verify gathering through village scheduler (Play mode)** (`GatheringSchedulerChecks.Verify`). Run it in a disposable FirstQuestSlice Play session after startup has finished, with no objects held. Back up the local season before automated runs; restore it after stopping Play because the normal scene autosaves and saves on quit.

The fixture supplies initial conditions: a complete bench, twelve tired neighbors nearby, clear weather, development focus, food-work observation mode and logical seed 1347. It does not start a leisure activity, isolate creature candidates, edit private cooldowns, manufacture memories, or call selected villagers' Tick methods. For 300 simulated seconds, `QuestSlice.Simulate(.05f)` owns the full scheduler. Construction yards also advance their private physics scenes, normally driven by FixedUpdate. The test runs synchronously, so rendering, audio and animation do not advance as 300 real seconds would.

Assertions cover independent useful visits from multiple villagers, completed creature activity in at least two categories, pause, valid serialized simulation state and retained construction/shared-rest counters. Shared rests are reported rather than mandated: opportunity and willingness can differ with competing routines. The focused GatheringChecks still requires the actual invitation, approach, completion and reciprocal outcomes.

On 2026-10-27, two completed harness runs reported:

| Run | Bench uses | Distinct visitors | Creature completions | Action categories | Shared-rest participants |
| --- | --- | --- | --- | --- | --- |
| 1 | 5 | 5 | 6 | 5 | 0 |
| 2 | 6 | 6 | 5 | 4 | 0 |

An earlier local exploratory setup produced one shared rest (two participants), but the committed fixture did not reproduce that encounter. This is evidence of autonomous progress, not a guarantee of social pacing or deterministic PhysX replay. The fixture keeps the existing global random/physics state and does not reset every controller's private runtime history. The focused shared-rest and full actor movement/carry checks also passed.

The run exposed repeated `Look rotation viewing vector is zero` messages from adult/child movement at tabletop scale. HumanoidMotion and VillageFamilies now use a shared horizontal facing helper: negligible motion retains heading, while visible movement is normalized before LookRotation. ActorMovementChecks.VerifyFacing covers zero, vertical, sub-epsilon and small diagonal movement. Compilation passed with no reported errors or warnings.

The fixture restores the season reference, construction records/recipes, carried-object transforms, actor routes and development/weather settings in finally. Active plans are cancelled, not replayed; use a fresh Play session for continued testing. It never invokes the game's disk Save method. The validation session's original disk save was restored separately.

## S4 remains open

This check does not prove three different useful player contributions in one coherent player session, an organically initiated chime performance with mixed audience outcomes, a declined player suggestion, or physical Quest comfort/controls. Those remain the integrated acceptance pass. In particular, repeatable natural shared-activity pacing still needs observation; do not raise its priority merely to make the test pass.
