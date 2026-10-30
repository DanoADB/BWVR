# Beachball knockdown play

Stack or arrange blocks in the construction yard, then roll or throw the beachball at them using the existing pickup/toss controls. Gentle collisions can move loose pieces; a contact at 9 game units/second or more can detach a snapped piece and its direct connections. Rebuild and try another arrangement. No new menu mode, input binding, currency reward or imported art is needed. The Book's Places section ends with **Knockdown play**; **Build with me** remains the preceding page.

If a damaged recipe becomes incomplete while a villager is visiting, the existing visit controller releases its reservation and records disappointment. Knocking loose toy blocks is not itself a new global morality event or an automatic creature-blame penalty. Existing ball impacts with houses, trees and villagers retain their separate damage and source-attribution rules. The creature can hit blocks through normal throwing/kicking, but this increment does not add deliberate demolition planning or a new learned knockdown challenge.

## Physics boundary

The beachball and construction already simulate in separate game-unit PhysX scenes. A 16-object pool now mirrors nearby registered construction pieces into the ball scene. Up to 144 startup-registered pieces are considered, with nearest selection every quarter second and active transforms synchronized before ball physics substeps. Current main-yard inventory is 48 pieces. There are no per-frame scene searches or new visible primitives. Box compounds preserve arch openings; convex mesh colliders retain ramps/cylinders.

Ball-scene contacts transfer the opposite collision impulse into the construction body's local coordinates. Gentle impulses are capped at 0.35 and hard impulses at 1.5, keeping toy motion bounded. High-speed contacts explicitly detach joints before applying force. The ball collides against kinematic mirrors, so this is a controlled two-scene approximation, not a fully unified momentum-conserving rigid-body simulation. Mirror selection is bounded; very dense arrangements may have more nearby pieces than the 16-contact budget.

Player-/actor-held pieces, villager-owned pieces, kinematic carried bodies, paused/locked yards and bridges are excluded. Holding the ball disables all construction contacts, preventing hand-driven bulldozing. Normal pause/suspend stop simulation. Returning/throwing/placing the ball refreshes contact selection. Existing construction capture/restore retains the resulting detached geometry; no new save schema is introduced. Physics proxies belong to the ball scene and are unloaded with it.

## Verification

Unity 6000.6.7f1 compilation and `KnockdownChecks.Verify()` pass. Isolated native ball-scene impacts move a loose cube, leave gentle-hit snapped geometry intact and detach/move a hard-hit cube. Checks also cover the 16-proxy bound, held block/ball protection, pause, captured/restored detached state, actual compound arch/convex ramp shapes, and a physical bench knockdown interrupting a native villager visit. A first low-speed/low-cap trial did not produce sufficient displacement; the final gentle test uses speed 5 and the bounded 0.35 impulse cap.

Goal, bowling, cooperative gate, construction-use and Book model/runtime regressions pass; the Book page was rendered and inspected. The older general beachball fixture initially failed `Tree ignored ball` both with and without the new block contacts. Its Hit helper stopped on any collision while checking one specific target. Journals showed a neighboring placement hit; a separate target-specific native tree trial passed. The helper now waits for the intended target's per-throw contact receipt and tries up to eight approach directions. The corrected full beachball physics/damage/learning check passes.

These are staged desktop Editor checks. Natural dense-yard play, Quest performance, throwing comfort and contact tuning remain hardware playtest gates. User save and XR startup flags are restored after QA. No APK built.

Next: headset playtest the build → knock down → rebuild loop before adding goal scoring or deliberate creature demolition behavior.
