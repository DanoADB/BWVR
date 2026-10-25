# Private spirit companion

Design specification, 2026-10-25. The character, agent connection and voice bridge are planned, not implemented by this document.

## Character

An original small floating spirit with an ivory face, teal seed-shaped body and leaf fins is visible and audible only to the player. Its restrained poses express idle, listening, thinking, speaking, suggesting and acting. It rests beside the Book of Creation or at a comfortable world-space perch; summon/dismiss must not move the player's head or block the island. Use a lightweight mesh or billboard without a real-time light or particle swarm.

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
