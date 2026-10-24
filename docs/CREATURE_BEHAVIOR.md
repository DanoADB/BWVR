# Creature play and social foundations

The creature now discovers a ball, turning blocks, a sound-making chime, food and villagers through typed affordances. It scores interest using novelty, proximity, hunger and learned play/social preferences. Repeated discoveries become less novel. This is a bounded, rule-based prototype, not a general-purpose AI or a completed BDI simulation.

The loop is approach, look/inspect, reach, hold/play, and put down. The ball can be moved a short distance; blocks are turned in its hands; the chime sounds when explored. Food reduces hunger. Villagers can be gently lifted, carried briefly, and released on the villager navigation surface. Their normal activity resumes afterward. There is no throwing, damage or forced relocation into buildings/water. A carried object is exclusively reserved; the creature cannot take an object held by the player. Timeout, reset and disable release its object safely.

Players can carry toys using existing controls and place them near the creature. Left-trigger interaction or desktop F on an affordance invites investigation; the creature accepts only while available. Warmth directed at the creature within twelve seconds after an interaction reinforces its last play or social preference. Hunger and both preferences are included in existing save/resume data; individual object discovery counts and in-flight actions currently reset on reload. Delivering food directly to the creature also relieves hunger.

## Nonverbal affect

One camera-facing world-space icon appears above the creature. It uses a shared tiny atlas and no text, interaction raycaster or additional lights. Changes have a short minimum display time to avoid flicker.

| Symbol | Meaning |
| --- | --- |
| Question mark | Curious; approaching or inspecting |
| Heart | Happy; friendly carrying, successful contact, or warmth |
| Tearful face | Sad/frustrated; failed route, interrupted interest or negative feedback |
| Apple | Hungry; high hunger or eating |
| Sleepy face | Bored; no recent play |
| Spark | Excited; lifting or exploring a toy |

Calm has no icon. Hunger can override a recent happy idle state. Curiosity pauses with the season and does not start autonomous distractions when village water is critically low. Existing water work is retained as a regression check; this addition does not expand villager jobs.

Validation: Play-mode checks complete all three toy interactions, eating, villager carrying/safe placement and resumption, prevent stealing a player-held toy, preserve state while paused, release an interrupted carry, expose hunger affect, and round-trip need data through serialization. Creature delivery, existing actor/terrain checks and navigation also passed. Icons were rendered and reviewed in the Editor. Quest performance and headset legibility remain to be checked; no APK was rebuilt.

## Open artwork candidates

Verified on 2026-10-24:

- Kenney Minigolf Kit: https://kenney.nl/assets/minigolf-kit — CC0; downloaded archive contains ball-blue/red/green FBX and GLB models. A good starting point for a rolling toy.
- Kenney Brick Kit: https://kenney.nl/assets/brick-kit — CC0; 290 interlocking pieces. A small curated assembly can replace the prototype turning blocks.
- Quaternius Bell: https://poly.pizza/m/cILuaO115Y — author-uploaded CC0 FBX/glTF model. Fits the village setting for sound exploration.

The Kenney archives and bundled license files have been inspected locally. These artwork candidates are not yet imported into runtime scenery; the ball and chime still use procedural prototype geometry; the construction toy now imports the selected Kenney brick meshes. Final mesh/material budgets should be checked during import.
## Physical construction toy

Eight separate Kenney CC0 bricks replace the decorative turning-block cluster: seven 2x2 pieces and one 2x6 lintel. Their authored studs and beveled shapes are preserved, with earth-tone play colors. Source FBX files and license/provenance live in Assets/ThirdParty/Kenney/ConstructionBlocks. BWVR > Import CC0 construction blocks regenerates the two Resources-backed models and snap-preview material.

Loose bricks have gravity, friction and box collisions. A private 3D physics scene simulates metre-scale bodies and maps their poses to island units, avoiding millimetre-sized PhysX contacts without changing global gravity or collision settings. The eight bodies use continuous collision detection and twelve solver iterations; the visible island colliders support ray picking. A raised wooden tray has visible retaining rims and limits this first construction sandbox. This is a toy assembly, not villager building construction or a new navigation obstacle system.

Connections align to the tray grid and horizontal brick connectors in quarter turns. A nearby compatible socket gives a green outline; unsupported, occupied, steeply tilted and distant placements are rejected. Snapped pieces remain dynamic and connect through joints with all six motions locked. Grabbing a piece unlocks and removes its own connection and connections directly attached to it; remaining loose pieces fall under physics. Unsnapped pieces remain loose, and pieces lost outside the sandbox return to staging. No global layer/settings changes are required.

Controls: use the existing right front trigger (desktop E) to carry a brick, point at the tray or a supporting brick, then release over the green outline. Left-trigger interaction (desktop F) on a loose brick invites the creature. It reaches, carries, previews, lowers and attaches individual pieces. After discovering the toy it favors continuing construction. Its first bounded plan is two three-brick pillars, a long lintel and a cap; this supplies an end-to-end construction intention, not arbitrary architectural planning. Players can use the grid for their own arrangements and detach/rebuild them. Warmth still reinforces play interest.

The creature and physical toys remain active after the drought timer ends, while the scenario result remains intact. Explicit pause freezes construction. Saves include each brick pose and connection; held pieces restore safely to staging. Empty/legacy construction data is accepted, and malformed poses or cyclic connection graphs are rejected. Begin again resets the tray.

Validation in Play mode: loose-body fall/settling; unsupported, tilted and occupied snap rejection; creature placement of all eight pieces; five-second assembly stability; controller-ray preview/drop; pause; detachment; save/restore stability and cyclic-data rejection. A separate autonomous check completed all eight pieces from one invitation in 80.8 simulated seconds with an already-finished season. Existing curiosity, action/terrain and navigation checks also run against this integration. Quest performance remains unmeasured and no APK was rebuilt.

## Additional artwork sources

Sketchfab alternatives checked on 2026-10-24:

- Maggatron / MaggaModels, giant Beach Ball: https://sketchfab.com/3d-models/beach-ball-25e1816c0e22444bb62816d3999d1b0b — downloadable, 3.6k triangles, CC Attribution. A more expressive ball candidate; not imported yet. Credit the creator if adopted.
- mithgarden.studio, Building Blocks: https://sketchfab.com/3d-models/building-blocks-c47b9847ad7042cc9cb3c8cb200a8a39 — three toy shapes, 1.1k triangles, CC Attribution. A candidate for non-interlocking wooden shape play.
- Free3D Ball: https://free3d.com/3d-model/ball-12524.html — listed as personal-use only, so excluded from the project.

Model scale is normalized to the creature during import; visual character and texture quality matter more than the download's original dimensions.
## Villager curiosity and disassembly

Villagers occasionally investigate the creature's construction, look at a reachable piece, make a short prying gesture, loosen its connection and physically push it toward the tray edge. This is independent toy play alongside their existing routine, not a new job. Normal activity resumes afterward, and the behavior remains available in post-scenario free play. Visible feet follow the raised tray top without a dynamic navigation rebake.

Only attached bricks explicitly marked as creature-built are candidates. Player-built pieces, held pieces, large lintels and pieces above villager reach are excluded as direct targets. Removing a support can destabilize connected pieces under the existing physics. Build provenance is saved with each piece; older saves without provenance remain unmarked. Rebuilding a piece with the creature marks it again.

Each villager has a staggered initial delay and a 100–160 second cooldown after an attempt. The tray additionally permits only one attempt per 90 seconds, with a 20-second quiet period after the creature attaches a piece. A target reservation prevents conflicting grabs or duplicate actors during the interaction. Lifting or disabling the villager interrupts the attempt and frees the reservation. Resetting the construction also cancels any ongoing interaction. Actual disassembly prompts a brief sad/frustrated affect in the creature; an already-active task can take display priority.

BWVR > Verify villager block play (Play mode) checks player-built/held protection, target ownership, inspect/pry/push phases, physical brick displacement (4.25 game units in the checked cycle), pause, tray cooldown, saved provenance and interruption/disable cleanup. Construction, curiosity, action/terrain and navigation regression checks passed. Quest performance remains unmeasured; no APK was rebuilt.
# Individual social learning

VillagerSocial adds a bounded social intention alongside block play and existing routines. Three stable villager identities retain trust in the creature, the creature's affinity toward that individual, temperament (boldness), and greeting/ride/witness/dispute counters in the season save. Old saves with no relationship array start with neutral memories; invalid arrays or nonfinite values are rejected.

When the creature is idle, villagers periodically approach within five game units, look up, beckon and wait. A completed nearby greeting builds trust. Willing villagers then invite a gentle carry using the existing safe carry/place behavior. Lower-trust villagers can greet without consenting to a ride; the creature checks willingness again before lifting. Trust and boldness affect how often each villager seeks attention. Explicit pause freezes this intention; carrying a villager interrupts it; navigation failure and a bounded timeout release it.

A completed safe ride raises that villager's trust and the creature's individual affinity. Other unheld villagers within 22 game units gain a smaller trust increase. This is proximity-based social exposure, without line-of-sight or knowledge propagation. Disassembling the creature's construction reduces the creature's affinity toward the responsible individual. That affinity influences its autonomous social target scoring. Feedback during the existing twelve-second window after a completed social action reinforces both the general social preference and the preference for that individual; it does not lower the villager's trust as punishment.

No dialogue, quest text or new menu is added. Existing gaze, beckoning, carry/place animation and creature affect convey encounters. These are small rule-based memories and bounded plans, not general BDI reasoning or full social simulation. Shared toy turns, friendship networks, recognition of structures and skill imitation are next stages in GAMEPLAY_DIRECTION.md.

Verification: `BWVR/Verify social learning (Play mode)` covers greeting approach/completion, pause, consent, safe carry release, nearby observational trust, individual feedback/dispute effects, save round-trip, invalid memory rejection and legacy saves. Existing construction, curiosity, action/terrain and navigation checks are regression coverage. Desktop Editor verification does not establish Quest performance or headset comfort; no APK was rebuilt for this change.
# Positive and negative teaching

Point the right controller ray at the creature and tap **A to praise** or **B to discourage**. Desktop: aim at it and tap **1 / 2**. The existing front-trigger/select interaction on the creature still provides praise. Feedback does not require opening the wrist menu. Carrying an object blocks teaching so the controls do not accidentally reinforce an action during placement. Pause blocks feedback, button presses are edge-triggered, simultaneous A+B is ignored, and receipts have a half-second cooldown.

The creature immediately shows a heart or sad-face icon for two simulation seconds, including while another activity is running. The icon bypasses normal affect hysteresis for feedback. A short right-controller haptic pulse accompanies receipt on XR devices; hardware haptics still require a headset check.

Teaching targets the **last completed action**, within twelve simulation seconds, once per event. Play/construction modifies play preference; social carry modifies social preference and individual affinity; creature water delivery modifies care, which affects its water routine speed. Negative feedback decreases the same values; it does not injure the creature, cancel a task or punish villagers. Earlier events are superseded by the next completion. Outside the window the creature reacts, but learns no preference. Action windows are transient; learned values persist in saves. Feeding now satisfies hunger without silently reinforcing an unrelated recent action. Player-delivered water does not become a creature learning event.

Verification: `BWVR/Verify creature feedback (Play mode)` checks both signs, immediate icons, repeat protection, pause, expiry, toy/water credit, visible reaction persistence and saved learned values. `BWVR/Validate Quest controls` checks A/B mapping with a simulated Oculus Touch device alongside movement, zoom and existing bindings. This is Editor verification, not confirmation of physical button feel or haptics in the headset.
