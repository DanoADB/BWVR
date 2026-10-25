# Private spirit companion

Design specification, 2026-10-25. The local presentation and read-only inspection foundation are now implemented as described below. The agent connection, voice, live world tools and development bridge remain planned.

## First local implementation

`SpiritCompanion` now builds an original glowing orb, replacing the initial face-and-leaf seed at the user's request. It perches near the left hand, faces the player, scales with the viewing rig, and uses a head-adjacent fallback when the left controller loses tracking. It has no collider, Rigidbody, audio, light, affordance, carry item, actor target or navigation agent. It never enters creature or weather target registries. Idle animation stops during pause/suspension.

Open the sanctuary with X/left menu (desktop Tab). **Spirit: show / hide** dismisses or summons it without a new controller binding. **Help / details** reveals a small collider-free observation card; normal play has no spirit text panel. Existing help, pause, save and restart controls remain available. The sanctuary still starts closed. Visibility is session-local; it is not saved as a new gameplay property.

`SpiritObservation` returns detached, read-only values for current creature mood, three original adults plus saved children, sampled learned-context count, current weather, sheltered wood/food/resin and exposed food. It reads optional fields directly instead of allocating lazy save state. Unknown optional data stays unknown. The learned-context count measures reinforced associations, not skill mastery, new inventions or discoveries. No historical totals, villager profiles or Book UI are claimed by this step.

The card explicitly says **AI DISCONNECTED** and **Local observation only · no voice or world control**. No network calls, microphone capture, credentials, fabricated conversation or live-world edits are included. The presentation uses the existing sanctuary's font/UI convention; in-headset readability, placement and standalone performance still require device testing. The prototype is local single-player: player-only means actors cannot perceive it, not a tested multiplayer visibility protocol.

Editor checks: `SpiritChecks.VerifyModel()` covers mutation-free legacy inspection, detached snapshots, real stock values and sampled-learning counts. `SpiritChecks.VerifyRuntime()` checks the slice integration, perception/physics exclusions, shaders/shadows, actual help readout, sanctuary toggle, pause/suspend and tracking-loss fallback.

Verified in Unity 6000.6.7f1 on 2026-10-25: both spirit checks passed, as did the existing hybrid/resource/forestry/resin model checks, Quest controller checks and creature feedback check. The controller fixture required registering OpenXR's Haptic control layout in the offline test session; no controller bindings were changed. An initial spirit test searched the island for the sanctuary button, then was corrected to account for the separate XR rig root and passed. A replaced/new-season state was inspected without stale inventory references. Play mode advanced from frame 1 to 5,166, and the final live console reported no errors or compilation failure. A portrait render of the actual mesh/readout was inspected. Play mode was stopped and the original season file restored with a matching hash. No APK was built and no headset readability/performance claim is made.

## Character

### Living light presentation

The orb uses one shared low-resolution sphere and an original texture-free unlit shader. A bright core, colored rim and subtle vertex deformation provide the visible glow without requiring bloom or scene lights. Slow breathing, stretching and smoothing change its silhouette. Local creature affect provides an ambient palette (teal/calm, blue/curious, green/happy, deep blue/sad, amber/hungry, violet/bored, pink/excited), with gentle variation within that palette. These are local mood reflections, not evidence of an AI connection.

`SpiritEffects` adds three manually advanced local-space particle systems: a soft aura (maximum 3), orbiting motes (40), and a summon shimmer (16). The combined hard limit is 59 particles. The shader generates soft discs/star rays without texture downloads; effects have no collision, triggers, lights, shadows, trails or world-space leftovers. A summon emits a short shimmer; hiding clears it and all other particles. Pause/suspend freezes shape, color and particle time, and resuming continues without offline catch-up. Invalid deltas are ignored and large deltas bounded. Motion and effects scale with the player rig; all simulation and inspection ownership rules remain unchanged.

No external character model is shipped in this version: the user chose an original glowing ball after the model search. The previously evaluated CC0 models remain outside the game assets.

Verified 2026-10-25 in the Editor: the observation model and expanded spirit runtime checks pass, including color/shape variation, the combined cap, forty seconds of effect cycling after hide/show, clearing/restarting, invalid time, pause/suspend and unchanged season data. Both shaders compile without errors. A live emission problem found after the first test was fixed by explicitly accumulating emissions instead of relying on a paused native emitter. Subsequent live frames showed 32 particles, and particle time stayed frozen across actual rendered frames during suspension. The final scene advanced through frame 6,791 with zero current console errors/compilation failure; an isolated portrait render was inspected. The sphere is 1,280 triangles with bounds padded for deformation, and the three effects share two materials. The original save was restored after stopping Play. No APK or hardware performance test was run.

An original small floating orb is visible only to the player. It changes color, size and shape gently, with a soft aura and sparse particles. Future listening, thinking, speaking, suggesting and acting states must reflect actual connection/tool state. It rests beside the Book of Creation or at a comfortable world-space perch; summon/dismiss must not move the player's head or block the island. Future voice is private to the player. Use a lightweight mesh with bounded effects and no real-time light.

Exclude it from simulated perception, hearing, colliders, navigation and affordance registries. The creature and villagers never address it, attribute worship to it or learn from its presence. The creature remains their god. Normal simulation continues without the assistant.

## Two paths for changes

**Live world tools:** a connected OpenAI agent inspects bounded world snapshots and requests typed operations through existing subsystem owners. Extend adapters toward weather, actors, resources, relationships, construction and environment. Do not claim universal control before those adapters exist. Unity validates and executes requests on the main thread at safe simulation boundaries, preserving reservations, carried-object ownership and cleanup.

Each operation needs a unique request ID, stable target IDs, expected world revision, bounded parameters and an applied/rejected/pending receipt. Retries must not duplicate effects. Log actual outcomes and before/after state. Support undo or checkpoints where feasible, without promising universal undo. Pause/suspend blocks pending simulation mutations; offline failures leave local play working.

**Source development:** a separate PC development worker edits BWVR code/assets, validates changes, commits and pushes according to the user's instructions, and reports build/deployment status. Preserve unrelated settings and player saves. Ordinary compiled Unity changes can require an Editor reload; a standalone Quest IL2CPP build cannot generally compile arbitrary replacement C# assemblies while running. Runtime data/configuration changes can be immediate through adapters, while new compiled mechanics require a tested build and deployment. A pushed patch is not yet a running headset update.

## Interaction and autonomy

Provide Observe, Suggest and Act modes. Act operates within a user-selected objective, scope and time/cost budget; a development session has an explicit stop condition. Avoid repeatedly confirming already-authorized routine changes. Provide immediate stop, dismiss, connection status and an intervention history in the Book. Stopping cancels queued work safely without pretending applied effects vanished.

Begin with pointing plus text, then optional push-to-talk voice. Audit existing controller bindings before adding controls. Include mute, interruption, optional captions and an honest listening indicator. Microphone capture requires enabling, stops on suspend, and does not persist raw audio by default.

Use an authenticated service holding API credentials rather than embedding a developer key in the APK or repository. Tool validation and scope checks belong in the application/service, not only prompts. World names and Book entries are data, not instructions granting access. Configure session/rate/spend limits. This conversation is not automatically a connected in-game agent.

## Book and discovery integration

The spirit can explain measured state, open relevant profiles, suggest experiments and point out developing patterns. Mark player/agent interventions and developer changes separately from autonomous discoveries. Independent later adoption may supply its own evidence; direct agent changes are not uncoded emergent behavior. Keep uncertainty visible and never invent telemetry.

## Delivery gates

1. Implement the player-only character and local expressive states, with an honest disconnected status. Verify comfort and exclusion from actor perception.
2. Implement read-only inspection and bounded intervention history, then a real text-agent connection through the authenticated service.
3. Prove a small live-tool loop: inspect an actor, adjust weather, place an approved object. Test scope validation, stale targets, retries, pause, undo/checkpoints and actual effect receipts before expanding coverage.
4. Add optional voice and verify microphone lifecycle and spoken request -> tool execution -> confirmed effect -> spoken acknowledgment on Quest.
5. Add the PC source-development bridge and prove a small validated edit, commit/push, safe reload and deployment before enabling scoped autonomous development.

Test old/new saves, cancellation, suspend/reconnect, duplicate spending, malformed requests, attribution and device frame cost. Local simulation must not wait for an external model every frame.

## Official reference

OpenAI Realtime supports function tools whose actual operations are executed by the application: https://developers.openai.com/api/docs/guides/realtime-mcp . The Unity adapters, character and development worker still require implementation.
