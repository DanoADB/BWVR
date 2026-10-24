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