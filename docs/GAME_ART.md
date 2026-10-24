# Local original game artwork

The first slice can use the user's converted Black & White artwork: cow, three houses, man and woman villagers, beech, oak, pine and grass. The cow uses five original standing poses and fourteen original walking poses baked from the local CBN animation cache. Walking advances with travelled distance; pausing freezes the pose.

## Import on this machine

1. Keep the original disks/game files locally. Shade's existing extraction/export workflow prepares `.local/cow.json` with its `village` section and the companion `.local/cow-animation-raw.json`.
2. From the BWVR repository, run `python tools/prepare-game-art.py ../Shade/.local/cow.json`.
3. In Unity, stop Play Mode and select **BWVR > Import local game artwork**.
4. Play `FirstQuestSlice`. The console reports `BWVR_ART_ACTIVE` when the catalog is loaded. If it is missing, the prototype geometry remains available.

The importer creates meshes, materials, textures, colliders and normalized prefabs under `Assets/LocalGameAssets/Resources/BWVRArt`. These derived assets, their metadata, the intermediate JSON and local screenshots are ignored by Git. The public repository contains the importer, shader and gameplay integration, not the copied game artwork. A local Quest build includes the imported artwork through the Resources catalog.

## Conversion details

Vertices and normals reflect Z into the Unity coordinate system and triangle winding reverses with them. Each model normalizes to unit height with its feet/base at zero, preserving its proportions. Textures keep original UVs and use a lightweight cutout shader with double-sided foliage and stereo instancing macros. The cow's original row-axis Euler channels compose with its bind orientations and hierarchy; baked poses become blend shapes. A rigid single-bone skin supplies Unity's required vertex layout for those blend shapes.

## Verification and limits

Local checks cover nine prefab references, colliders, shader compilation, nineteen cow poses, complete skin weights, live frame advancement, water delivery and unsafe villager return. Desktop rendering was inspected. Headset appearance, GPU cost and controller ergonomics need Quest testing. This import does not yet provide original villager animation, creature carrying/drinking/casting clips, LODs or a full terrain mesh. Art placement is still the prototype layout.
