# Creature attention, responses and outcome evidence

S1 implementation, 2026-10-26. Extends the existing creature coordinator, mood/face, humanoid gaze and contextual feedback. No new agent framework, controller bindings or permanent HUD.

## What the player should see

| Response | Meaning and presentation |
| --- | --- |
| Noticed | A brief glance and curious affect when a nearby frontal object or active suggestion is visible. This does not promise a new action. |
| Chosen | An excited response and short head nod when a visible player-influenced activity actually acquires action ownership. |
| Busy | After noticing a different cue, a brief sideways head response while the existing activity continues. It does not cancel a carried load or route. |
| Unavailable | Sad affect and, where a visible target is available, a sideways response for rejected/failed activity. Optional details give the actual cause. |
| Completed | A brief completion response following the executor's confirmed event. Harmful ball/tree play retains a negative response rather than celebrating harm. Completion is not player praise. |

The normal world remains largely nonverbal. Existing Help/details can show the short response reason alongside the current intention; it is hidden by default. New head movements affect the creature's gaze only, never the player's camera or the actor's navigation/hands.

Placement retains its existing bounded utility bias. Guidance remains a temporary suggestion. Neither a glance nor a nod installs an order or creates learning. Replacing a suggestion interrupts an observation only when that observation belonged to the superseded target; unrelated active work continues.

## Perception and interruption

Placement acknowledgment and guidance acknowledgment share a bounded frontal/range/line-of-sight check. Held, inactive, rear, distant or occluded targets do not receive a false notice. Accepted remote guidance can remain pending; it is acknowledged only if subsequently visible. The request's noticed flag is saved, avoiding a repeated notice simply because a save was restored.

Visibility uses the existing registered target, its collider center or a ground point, ignores the creature's own body and trigger volumes, and checks real occlusion. Active guidance is reconsidered for notice at most four times per simulation second. There is one transient response and one delayed busy cue, not an unlimited reaction queue.

Pause freezes response timing; carrying the creature stops its deliberation; interrupt/suspend/new season clears transient gaze. Praise/discouragement and harmful-play reactions protect their affect interval from these new acknowledgments. An expired response returns gaze ownership to the active executor. A failed/interrupted action is not a completed teaching receipt.

## Saved evidence foundation

HybridState optionally contains an OutcomeLedger. Old saves begin with no history; nothing is backfilled. Keep at most 96 recent structured records, plus bounded counters for creature completions and player miracles since instrumentation began in the current season.

Each record contains a monotonically increasing season-local ID, optional related event ID, simulation time, event kind, source, actor, action, situation, target, confirmed partner and concise outcome. A guided action can be traced from request to action start to completion/interruption. The event actor is the player for an offered request or miracle, the creature for its responses/actions, and the system for guidance lifecycle changes. Source distinguishes autonomous choice, player placement, player guidance and player miracle.

For a selected action, source describes the candidate's active cue bias, not proof
that the creature would have chosen differently without it. The current single
creature uses the stable actor ID `creature`. This first ledger does not yet record
every villager action or infer social transmission.

Action completion is recorded once while the coordinator owns an active intention; a second completion callback cannot increment history. Interrupted intentions close their evidence record without success. A safely resumed plan starts a new attempt through the existing logical intention workflow, without replaying a historical completion. Player miracles are recorded only after accepted effects and remain outside creature achievement/teaching counts. Read-only previews create no evidence.

Records and counters persist; transient response pose does not. Validate sizes, finite ordered times, enums, IDs, attribution, bounded strings and references when loading. Old linked records may have aged out of the ring, so later Book pages must treat missing detail as unavailable rather than invent it. ID exhaustion stops recording instead of wrapping.

This is recent factual evidence, not discovery recognition, proof of emergence, full village history or the Book interface. It does not change utility weights, grant resources, alter reinforcement or collect raw world snapshots. Future detectors must distinguish forced exposure, independent choices, useful outcomes and actual social transmission.

## Validation

Editor menu checks: **Verify outcome evidence model** and **Verify creature responses and evidence (Play mode)**. Runtime validation stages visibility, an existing busy action and a native observation completion; it does not claim the creature naturally chooses that staged plan in every playthrough. The response/evidence model and runtime checks passed in the Editor, including real native observation completion, duplicate suppression, pause, occlusion, interruption, reload and separate miracle attribution. A live-frame gorilla render was inspected with aligned eyes and skinned head; head-bone probes showed the nod and sideways overlay moving the real humanoid head. These staged checks do not verify headset readability or comfort.
