# Creature learning from unsuccessful attempts

S5 first increment, 2026-10-30. This extends the existing utility selector, action owner and physical skill systems. It does not add another planner or general-purpose AI.

## Player-visible behavior

When an activity cannot reach its target, the creature temporarily excludes that same activity/target and considers other opportunities. Repeated failures at the same location produce 15, 30, 60 and then at most 120 simulation seconds of backoff. Ordinary play, chasing, rhythm echo and spinning are distinct modes, so one failed mode does not disable every use of a ball or chime. An unchanged player cue does not bypass an actual route failure.

Moving the target at least eight island units from the recorded failure point allows reconsideration before the remembered delay expires. The next failed attempt at that new location starts a new short streak; history is retained. This is a fresh opportunity, not an order, and ordinary object cooldown, ownership, safety, feasibility and utility still apply. Successful physical completion clears the streak and records one completion. It does not turn a harmful throw into a beneficial outcome or create an extra teaching receipt.

Only failed route checks, explicitly reported blocked approaches and coordinator timeouts add failure memory. Generic interruption/unavailability does not, since it may mean the player took an object, a partner left or ownership changed. Pause, player interruption, reset and application suspension do not teach incompetence. Existing cleanup still releases route/cargo ownership before another activity starts. The backoff follows simulation time and persists through save/resume.

## Knowledge, skill and willingness

Book → Creature now has three pages: the existing profile, physical practice versus willingness, and recent action attempts. All are read-only projections of actual saved data. Unknown historical practice/attempts stay unknown; reading never creates them.

Known construction forms remain the existing knowledge representation. Confirmed placements already improve BuildingPractice control, motion time and alignment; confirmed gentle-ball practice already improves aim. Praise/discouragement changes contextual preferences and willingness, independently of those practiced controls. This increment exposes and verifies that distinction rather than replacing the working skill systems. Attempt completions report execution, not virtue or social success.

## Bounds and integration

HybridState holds an optional 32-entry array keyed by action, stable target and activity mode. Entries contain bounded failure/completion counts, a maximum four-step streak, last target position in island coordinates, retry time, update time and concise reason. Once full, the oldest entry is deterministically replaced. Old saves with no array remain valid. Duplicate keys, invalid modes, nonfinite coordinates/times and oversized records are rejected.

Perception marks remembered failures infeasible before utility ranking. The actual route precheck records a failed attempt; typed existing approach failures and coordinator timeouts use the same adapter. Native confirmed completion records success. There is no per-frame scene scan, global preference retune, new actor controller or permanent HUD.

This is the first S5 increment. It does not implement arbitrary tool invention, novel construction blueprints or a generic reusable-plan language. Knowledge, motor practice and willingness existed already; richer plans and context-sensitive experiments remain later development.

## Verification

Unity 6000.6.7f1 Editor model/runtime checks passed for bounded deterministic waits, activity-mode isolation, moved-target reconsideration, failure deduplication, success reset, save validation and old-save purity. The runtime fixture finds a genuinely unreachable location within perception range, observes actual selector failure and subsequent exclusion, uses native Save/Load, moves the same chime to reachable ground before expiry, and completes the native approach/inspect/lift/play/place motor. It checks pause, ownership release, unchanged failure counts on player interruption and preference-only teaching.

Existing hybrid model/runtime, construction learning, physical skills, invented knockdown game, storm recovery and Book checks pass. Following the hybrid test's asynchronous navigation rebuild, dependent checks wait for navigation readiness before running. The populated three-page Creature view was rendered with bounded maximum counters; the learning pages were visually reviewed. Test actors and alternative offers are staged for reproducibility. These results do not establish natural full-population participation or perceived fun.

The user's save and original XR startup flags are restored after tests. Physical headset testing is explicitly deferred at the user's request. No Quest performance/comfort claim or APK build is made.
