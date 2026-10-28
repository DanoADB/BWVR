# Giant bowling — watch me, then try it

Current playable priority: hands-on creature play. The bench/shelter milestone is deferred following the user's 2026-10-28 direction. Its unfinished cooperative-project implementation is shelved outside Assets and is not included here.

## Play

Find the six wooden pins beside the beachball's play circle. The small turquoise pebbles point down the lane.

1. Hold the right front trigger to pick up the ball. Move your hand toward the pins and release. A toss from the start area aimed down the lane becomes a ground-level bowling roll, with bounded power and the player's actual horizontal aim. Other directions retain ordinary throwing/fetch behavior.
2. Let the creature watch. It must perceive the rack or ball during a real pin hit. A witnessed successful demonstration creates a brief curiosity response and a temporary opportunity bias; it does not issue a command.
3. Reset with the red orb beside the starting area: point and use the existing interact press (right front trigger; desktop F). The rack also resets after a short result interval when the ball is free and the creature is not handling it. A held ball or active shot cannot be reset by that control.
4. Leave the ball nearby. A creature that has witnessed success can choose to pick it up, aim and roll at the pins through its existing utility/action system. Other needs and occupied lanes can win. Praise/discourage with the existing A/B controls (desktop 1/2) after its completed attempt.

Desktop tossing remains Space + release E. No controller bindings or menu panels were added.

Pins physically collide with the ball, one another, terrain and existing contact proxies. A pin counts when it actually tips or drops below its standing height during an attempt with confirmed ball contact. Fallen-pin lights and an outcome sound communicate the result. A strike resolves promptly; partial attempts wait for settling or a bounded timeout. Idle nearby villagers who can see the result may react. Existing injury, harm and fear rules still apply if the creature hits someone or damages the world; pin success does not erase harmful consequences.

## Implementation and boundaries

- Six dynamic pin bodies share the ball's existing private PhysX scene; no new world physics scene or actor motor. Original low-poly pin meshes are generated in code and shared by the six renderers. No new third-party art or downloads.
- Lane placement checks a bounded set of flat, reachable headings near the ball and avoids existing static footprints. If none is feasible, bowling reports a warning and ordinary ball play remains available. The current island produced a valid lane.
- Only a witnessed player success unlocks imitation. An unseen shot can score for the player without teaching the creature. A fresh demonstration briefly increases attention; learned preference affects later choices, and successful creature attempts improve bounded aiming practice.
- In-flight bowling excludes autonomous pickup. Cancellation, pickup, reset and reload do not fabricate completion or teaching. Physical reset uses the existing spatial interaction target.
- Player/creature rolls, fallen pins, best attempts, observations, practice and preference use optional bounded WorldPlayMemory state. Old saves remain accepted. A typed bowling receipt keeps praise/discouragement distinct from fetch, rally and chime games; reinforcement is once-only within the existing feedback window.
- Active physics rounds restart safely on load; pin poses/velocities and unfinished throws are not replayed or rewarded. The practice rack resets while learned history persists.
- No permanent score HUD. Bowling history is saved but has no dedicated menu page yet. Menu readability and organization are the next task.

## Verification — 2026-10-28

Compilation reported zero errors/warnings. BowlingChecks.VerifyModel and Verify passed in the Editor. The final physical run used the native measured pickup/release path: the player knocked down six pins; the observing creature then selected the ball through normal utility ranking and knocked down five. No fallen-pin counters, positive observations or action completions were injected. The fixture stages a nearby watching creature and favorable starting needs, and ticks the existing creature/ball controllers; it does not establish an unattended village occurrence rate or prove fun in the headset.

Checks also cover actual pointer access to reset, physical reset through SpatialTarget, pause, exclusion during an active roll, no duplicate scoring, distant unwitnessed play, canceled/picked-up shots, one-shot contextual praise, and save roundtrip. Existing native beachball placement and player toss/fetch checks passed. A rendered local preview was visually inspected after the pin-mesh update.

Quest controls, comfort and frame cost still need physical headset validation. Six additional bodies are a bounded starting budget, not a measured standalone performance claim. The user's original save and XR startup settings are restored after diagnostics; unrelated project settings and licensed art are excluded from commits.
