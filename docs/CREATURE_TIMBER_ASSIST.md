# Creature assistance for unanswered timber offers

S5 continuation, 2026-10-30. Reuses the existing villager gathering motor, finite cargo, CreatureHybrid utility/coordinator, CreatureCuriosity executor, guidance, contextual feedback and Book.

## Play loop

Equipped cutters still offer a real harvested bundle to a free, willing neighboring villager first. An offer can also begin when the creature can visibly notice the cutter, even if no villager helper is free. After two simulation seconds without a helper, the cutter points toward storage. The creature can perceive a bounded HelpRequested / RespondToGuidance candidate and choose to assist. A player can suggest Help at that cutter through existing guidance tools. Guidance biases utility; it does not guarantee selection. The original six-second unclaimed offer still falls back to solo carrying if nobody accepts.

Creature candidates require physical visibility, an unheld/safe worker with actual finite wood, available storage, a reachable creature approach, and a reachable store route. Existing need, preference, novelty, affinity, guidance and failed-attempt gates score the opportunity. Choosing assistance reserves the offer exclusively; a villager helper cannot claim it simultaneously. Existing jobs are not canceled to manufacture a request, and a villager helper already approaching retains the task.

The existing creature coordinator grants its curiosity executor one action: approach, face the cutter, reach for 1.2 seconds, take the sourced timber bundle, carry to storage and place it. Approach sampling happens during perception/start, not every animation frame. The existing motion/carry layer handles the pose; no parallel locomotion owner, new primitive artwork, controller binding or UI command panel is introduced. The cutter waits with its source reservation until delivery or cleanup. This is one authored assistance capability, not unrestricted cooperative planning or a newly learned motor skill.

## Conservation, attribution and teaching

Wood remains in the cutter's bounded cargo slot while the creature carries its visible bundle. The creature does not use an extra invisible inventory or spawn a new resource. Only the existing positive native Deposit changes stock. Successful outcomes extend TimberRelayMemory with creature=true / carrier=-1, preserving the villager cutter, exact accepted amount and cumulative relay count. Existing villager-to-villager records stay valid. Book -> Provisions -> Next distinguishes the creature carrier from a villager carrier.

Real delivery can complete the latest matching delivered-axe storage proof, without another resource award. The cutter receives a small existing trust/affinity benefit and a safe creature-bond experience. The coordinator issues one normal HelpRequested receipt; praise/discouragement uses the existing input protections and 12-second training window. It is not a toy-fetch or felling receipt. No benefit or lesson is minted merely for taking the wood, for an interrupted action, or by another villager's work.

Explicit pause stops the plan. Before pickup, releasing a creature reservation returns the cutter to solo carrying. After pickup, interruption recovers cargo to its finite source and hides the carried load. Lifting the cutter, lifting/interruption of the creature, fire/unsafe source, vanished ownership or timeout causes cleanup through the existing controllers. The motor has a 110-second bound, coordinator 115 seconds and donor lease 120 seconds; ordinary failed-route handling remains in effect.

Completed attribution survives native Save/Load. In-flight plans follow existing gathering recovery: their cargo returns to the finite source on reconstruction rather than pretending an unfinished gesture completed. The latest relay detail remains a single bounded record, not an unlimited history.

## Verification and outstanding checks

CreatureTimberChecks model/runtime checks pass in Unity 6000.6.7f1. A staged native fixture performs actual felling, an unanswered visible cue, autonomous utility selection without guidance, exclusive approach/take, pause and interrupted cargo recovery, pending native Save/Load recovery, then a second guided native carry/deposit. It verifies exact stock, full-store/held-worker/occluded rejection, original player tool attribution, separate axe work, saved creature carrier, one contextual praise and duplicate protection.

The fixture suppresses competing toy opportunities and stages actor positions to isolate the motor; natural acceptance frequency is not established by this test. It exposed and fixed a handoff-facing issue at the second tree: a valid approach may end facing away from the donor. The creature now faces the cutter through the taking phase. Physical headset hand-contact polish, scale/readability and population-wide participation still require playtesting.

Villager timber relay, delivered-tool use, resource and Book regressions are checked alongside the increment. The populated Book page is rendered with a staged verified-value fixture. Original user save and XR startup flags are restored after checks. Food/water stay deprioritized, development energy remains unlimited, and no APK is built.
