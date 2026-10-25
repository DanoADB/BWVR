# First Quest slice

Future gameplay requirements: [indirect creature teaching, costly possession and evolving autonomous villages](GAMEPLAY_DIRECTION.md). Those notes describe intended systems; the sections below describe the original slice plan.

## Experience

The village lives on an island roughly 1.8 metres across, at a comfortable tabletop height. The player sits or stands beside it. The creature is the villagers' god. The player teaches through encouragement and offerings; villagers do not address a visible player avatar.

## Spatial interface

Replace the browser sidebar with three layers:

- **World:** resource piles, a blue cistern, a spring and the creature's short intent label. Objects and destinations provide context directly.
- **Hands:** controller presence and a restrained aiming ray; grip carries, trigger encourages or selects. Wrist sanctuary opens from the left menu button.
- **Sanctuary:** a compact dark teal panel with warm ivory type, season resources and large separate pause/save/restart controls. It remains attached to the left hand instead of following the head.

There are no screen-edge meters, browser controls or forced viewpoint changes. This first implementation needs on-headset text-size, reach, menu-button and handedness testing. A physical sanctuary stone, haptics, contextual highlights and accessibility scaling are later improvements.

## Milestones

1. Foundation: reproducible Unity project, OpenXR Android settings, tabletop scene, controller grabbing, wrist UI and editor validation.
2. Device proof: Android module installation, development APK, Quest Pro/3 installation, tracked pose and controller binding tests, comfortable scale and frame-time measurement.
3. Gameplay: action-based creature learning, meaningful praise timing, autonomous villager needs, drought balancing and repeatable win/loss runs.
4. Persistence: moved villagers, resources, creature route and action state; robust resume around application suspension.
5. Art and animation: original or appropriately licensed creature, village and landscape assets, carry poses, footsteps, water delivery and reactions.
6. Interaction polish: tracked hands, two-handed island scaling/repositioning, haptics, seated/reach settings and spatial audio.

## Limits of this milestone

The art and creature gait are placeholders. Controller rays use Unity XR device input with OpenXR rather than a full interaction toolkit rig. Hand tracking, physics throwing, complete OpenBlack behavior and asset parity are not implemented. Saves cover the logical season, not the whole physical world. Android packaging and headset performance remain unverified until a real device build is tested.


## First nonverbal drought loop

Routine world labels and detailed meters are now off by default. The wrist menu keeps short save/pause/restart controls; **Help / details** opts into the existing meters, control instructions and creature intent caption. The wrist menu starts closed; restored seasons resume automatically. X / left menu opens it on demand. Save/load errors and the season outcome still get a short visible message.

When water is low, the water gatherer can interrupt an outbound work trip to check the cistern, show an empty open bucket, look toward the creature, beckon and point toward the water source. The demonstration uses a nearby walkable spot across the cistern from the creature. If it is seeking water and nearby, the creature briefly acknowledges the gatherer with its gaze, then resumes its own routine. Busy creatures are not forced out of their current action. The gatherer resumes work regardless of whether it receives help. Requests have a cooldown and cannot start while paused or carried.

The bucket visibly fills during collection and empties during delivery. The cistern shows the actual village water level, and terrain field colors respond to crop health and water stress. A successful water gift prompts a short villager gesture toward the creature. These reactions do not grant extra resources, trust or learning rewards. Storage footprints now stop actors beside the cistern rather than letting them stand inside the new container.

This is a deterministic visual communication pass, not the future BDI, social relationships, tool/toy learning or possession systems. Speech, creature vocalizations, custom emotional animations and headset readability testing remain future work. Temporary gesture/request state is not added to the season save format.

Validation menu: **BWVR → Verify nonverbal drought cues (Play mode)**. It checks optional text, visible water levels, automatic low-water requests, paused behavior, empty-bucket display, gaze, beckoning, pointing, creature acknowledgement, independent worker completion, bucket fill/delivery and celebration without extra rewards. Existing navigation, terrain, actor action and release checks also pass. No Quest APK was built for this pass.

## Quest controller navigation

Left stick moves relative to the horizontal head direction at 0.65 m/s with a drift deadzone. Right stick left/right snap-turns 30 degrees around the head, once per stick deflection; up/down changes viewing height at up to 0.45 m/s. Ground clearance limits downward travel. This is floating-player travel, not villager pathfinding.

Controller poses and inputs read OpenXR Input System layouts. Aim pose drives the pointing ray; grip pose drives the carried object and wrist attachment. Grip holds/releases, trigger interacts, X / left menu toggles the sanctuary, Y recenters. Tracking loss returns a held resource safely home. No tracked controller means no ray or action from its stale pose.

BWVR > Validate Quest controls checks simulated OpenXR inputs, distinct grip/aim poses, head-relative movement, deadzone, snap-turn pivot/latch/rearm, vertical travel and tracking loss. Physical Quest Pro controller comfort and direction still need headset confirmation.

Zoom: hold the right side trigger (grip) and move the right stick up/down for 0.5x–8x world magnification. A fixed gaze/terrain focus is chosen when the right side trigger is pressed. Scale and position change together around that focus, keeping the tracked head rotation and FOV untouched. Y recenters at 1x. Movement speed, selection range, carry offset, near clip and ray thickness follow player scale. Zoom is suppressed while carrying; the mouse wheel supports desktop inspection. Physical headset comfort remains to be confirmed.

The creature can also be picked up: aim at it, hold the right front/index trigger to carry, then release over dry, clear walking ground. Its current activity is interrupted without a training reward; autonomy resumes after placement. Invalid drops or lost tracking return it to its pickup position. It cannot be thrown, and being carried does not kick the beachball. Desktop E provides the same pickup/release for testing.

The right side trigger is reserved for zoom. The right front/index trigger picks up and carries resources while held, drops on release, or interacts with non-carryable targets on press. Left side grip still carries; left front trigger interacts. Zoom does not start a pickup.

## Environment art pass — October 25, 2026

The island uses warm morning sunlight, cooler sky/ground ambient light and pale distance haze. The existing shared surface shader now has an alpha-tested, instancing/stereo-aware shadow-caster pass, so its meshes can participate in the single directional shadow map. Roof tints use restrained terracotta, slate and timber accents while retaining the five complete building shapes and authored material detail.

Ground colors blend meadow green, dry gold, forest shade, soil, sand and rock. The biome mask follows the actual navigable settlement lane network instead of straight source-placement spokes. The existing Quaternius rock, sand and ground-detail textures remain in use; palette-atlas grass is not repeated across the terrain. Village herb beds and terracotta planters join the existing timber, benches and fences. Decorations are combined by material, with no new colliders, dynamic lights, post-processing or gameplay ownership. Soft terrain-conforming contact patches beneath homes and up to 64 nearby trees improve grounding at tabletop scale. These are decorative ambient-contact cues, not an additional real-time shadow system.

The existing 15% villager/building enlargement and current creature, well and object sizes remain intact. Herb beds and planters are visual dressing rather than new food resources or villager tasks. Standalone Quest frame time and in-headset color/readability require device testing; no APK is part of this pass.

Validation: before/after Unity renders were captured and visually inspected; a repeating palette-atlas stripe was removed. Build-time footprint calculations now use transform matrices rather than unsynchronized collider bounds, fixing oversized decorative offsets and ground-contact patches. Final NavigationChecks, SliceRuntimeChecks, GentlePlayChecks and QuestControlsChecks passed: 55/55 settlement sites connected, 34 detail groups, preserved actor/building scale, and functioning pickup/drop controls. The two decorative meshes add 4,008 vertices; the bounded contact mesh adds 1,372 vertices, for three additional settlement renderers. Maximum contact radius measured 0.0321 world units. Live frames advanced from 36 to 1,823. The three edited shaders compiled without messages; final console compilation failures, errors and warnings were zero.

BeachBallChecks passed its building tint-restoration and building-impact steps, then failed with “Tree ignored ball”; the full suite is not recorded as passing. A separate staged native PhysX check confirmed damage 0.65 to tree placement-1123 from a 45-degree approach. No production ball-physics code was changed. Shader/visual checks are on the desktop Editor, not Android GPU or headset performance certification. The existing season save was backed up and restored after QA.
