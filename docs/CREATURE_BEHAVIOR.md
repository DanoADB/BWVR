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

The Kenney archives and bundled license files have been inspected locally. These artwork candidates are not yet imported into runtime scenery; current toy geometry remains the procedural prototype. Final mesh/material budgets should be checked during import.