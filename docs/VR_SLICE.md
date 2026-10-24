# First Quest slice

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
