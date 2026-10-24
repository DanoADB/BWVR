# BWVR visual upgrade shortlist

Direction: a warm, painterly miniature world. Preserve the recognizable creature silhouette, village scale and gentle god-game atmosphere while replacing blurry legacy surfaces and sparse scenery. Recommendations are based on publisher descriptions, not a completed Quest benchmark.

## Recommended environment candidates

- [Quaternius Stylized Nature MegaKit](https://quaternius.com/packs/stylizednaturemegakit.html): textured, Ghibli-inspired trees, plants, rocks and foliage. The complete kit lists 116 models; Standard is a free subset, while Pro/Source add content and engine-ready setups. The publisher lists CC0. Start by testing one tree, one rock and one grass cluster together.
- [Quaternius Medieval Village MegaKit](https://quaternius.com/packs/medievalvillagemegakit.html): more than 300 modular building pieces across the complete kit. Standard is a free subset; Source includes a Unity URP setup and custom shaders. The publisher lists CC0. Adapt the palette and silhouettes toward rustic, rounded thatched dwellings rather than a generic fortified fantasy town.
- [Quaternius Ultimate Stylized Nature Pack](https://quaternius.com/packs/ultimatestylizednature.html): 63 models, including seamless textures and normal maps. A useful texture reference and alternative to the newer MegaKit; assess its style alongside village assets before mixing them.

Free editions are sufficient for an initial comparison. Their Unity-ready Source packages use URP; BWVR currently uses the Built-in pipeline and a custom stereo-compatible game-art shader. Raw FBX/OBJ imports therefore need material setup, or a separately planned URP migration. The free Nature and Village editions are now integrated, with shared Built-in materials and normalized modular prefabs. No purchase was needed.

## Implemented art pass (2026-10-23)

- 17 selected Nature/Village FBX models: four modular houses, mixed trees, bushes, grass, ferns, stone paths and rocks. Ground texture and simple horizon clouds are original procedural BWVR artwork.
- Three villagers use Universal Base Characters heads with Fantasy peasant/ranger outfits. The hidden base body is removed to avoid clipping. The free base edition contains superhero-proportioned male/female bases; heads are attached to the clothing's humanoid head bone.
- Universal Animation Library Standard contributes 43 imported clips. Idle/walk and an interaction reaction are connected to gameplay through a shared humanoid controller. Root motion is disabled. Other imported actions, including pickup/spell clips, are not yet connected to physical carrying or miracles.
- Puglin is the first local Bestiary creature trial. Its weapon is removed. QAL files and generated creature prefab stay under ignored Assets/LocalLicensedArt; public clones retain the existing cow/primitive fallback until the user imports their own Bestiary download. The source importer contains no Bestiary geometry or textures.
- Universal Base Characters, Fantasy Outfits, Nature Standard, Village Standard and Animation Standard archives include CC0 license files, preserved with the checked-in assets.
- The older Medieval Village pack lists 44 CC0 buildings/props. Keep the newer MegaKit as the primary building style; consider older props only after a visual comparison. https://quaternius.com/packs/medievalvillage.html
- Ultimate Stylized Nature lists 63 CC0 models and seamless textures. Its Google Drive download returned a quota-exceeded page on 2026-10-23; no files from that pack are integrated. https://quaternius.com/packs/ultimatestylizednature.html

Rebuild environment assets with **BWVR > Import free environment artwork**. Rebuild villagers and the optional local creature with **BWVR > Import Quaternius villagers and creature**. For the optional creature, copy the Unity Puglin.fbx to Assets/LocalLicensedArt/Models and T_Puglin texture files to Assets/LocalLicensedArt/Textures before importing. The included license is kept locally. Source purchase does not grant raw-asset redistribution rights under the published QAL terms: https://quaternius.com/license.html

Verified in Editor: compilation, humanoid avatars for all four actors, advancing animation time, water delivery and safe villager return, visual capture, no errors in the cleared runtime console. Quest hardware performance and controller ergonomics still need a headset check.

## Order of work

1. Compare a small nature/village sample against the current scene under the same lighting.
2. Replace the flat island with shaped terrain, paths, banks, water edges and clustered planting.
3. Establish a consistent ground/roof/wood/stone palette, then add lighting and contact shadows within a Quest budget.
4. Replace the hero creature and retarget interactions/animation.
5. Profile on Quest 3 and Quest Pro before increasing foliage density, material count or shader complexity.

Texture resolution alone will not fix the flat terrain, weak composition, placeholder water and inconsistent scale. Those are part of the same visual pass. Prefer efficient silhouettes, shared materials, controlled transparency and LODs to unbounded scene detail.

Quest APK build completed successfully (52,440,823 bytes). Unity also logged internal BuildPipeline assertions during packaging; the log still ended with Build Finished, Result: Success. These assertions remain unexplained, and the APK has not been tested on a headset.


Animation correction: the initial controller accidentally referenced Unity's __preview__ FBX subassets. The importer now filters those out and repairs existing controllers to reference the real humanoid clips. Verification measured 0.0675m of natural creature travel with a 66-degree knee swing over 0.75 seconds; idle selection and frozen paused poses also passed for all four actors. Advancing controller time alone was insufficient evidence in the initial pass.


Creature actions now use an arrival-driven pickup/drink/carry/delivery routine, with visible carried water, humanoid hand IK, and UAL pickup/interaction/locomotion clips. Drinking and carrying are procedural hand poses over UAL, not dedicated clips in the free pack. Villagers travel to work, animate, carry supplies home, and deliver; water gathering and farming improve the village counters. Verified all five creature phases, one gift per completed delivery, pause, and completed jobs for all three workers in the running scene.


## Land 1 terrain pass

The first island now uses the owned Land1.lnd height field (100 source blocks), not a generated imitation. Coordinates use 10-unit cells and 0.67-unit altitude steps; X-major cell storage and original split flags are preserved. The dry-land extent is presented at 4.8m wide in VR, with the starting village near the original first town. Geometry and actors now share the original game-unit conversion (4.8 / 2190, about 0.002192 Unity metres per game unit). Measured source mesh heights: villager 1.661 units, representative house 3.66 units, tree 12.723 units, and cow 46.412 units multiplied by OpenBlack’s 0.3 creature spawn scale (13.924 units). Quaternius replacements use these heights: 3.64mm villagers, 8.02mm houses, 27.89mm trees and a 30.52mm creature on the miniature island. The old exaggerated sizes were approximately 23×, 24×, 11× and 11× larger respectively. Village spacing, work sites, resources, walking speeds, arrival distances and carry loads shrink with the actors; controller hands and wrist UI retain their physical VR sizes. Whole-island views therefore show very small villagers; moving closer is necessary for detail. Representative mesh sizes establish consistent proportions, not exact reconstruction of every original entity or its growth/variation. This pass does not recreate every original town or mission placement.

The geometry, height data and source file remain local under ignored Assets/LocalTerrain and .local. Only importer code, Quaternius ground material, shader and tests are pushed. The Quest build bundles the local terrain. Public clones without it retain the round-island fallback.

To reproduce locally, run `python tools/prepare-island.py <extracted-game>/Data/Landscape/Land1.lnd`, then **BWVR > Import original first island terrain** in Edit mode. The terrain uses Quaternius Grass palette, Rocks_Diffuse and Rocks_Desert_Diffuse textures; no original terrain textures or original scenery models are imported. Original terrain format references were inspected in Shade/components/lnd/include/LNDFile.h and src/3D/Implementations/LandIsland.cpp. Source SHA-256: 82df97c1e8eefd69b076407e2c503ab794a7c1521ddc508b7f6064571b237094.

The imported mesh has 44,400 vertices and 44,664 triangles. Scene placement samples the same split triangles as the collider; 48 dry-land probe comparisons differed by at most 0.000002m. Buildings and work sites choose nearby dry, gentle terrain. Object release uses the terrain collider and rejects water/steep ground. Pickup rays reach 8m across the expanded tabletop. The current actors still follow direct routes within the starter village; this is not whole-island obstacle-aware navigation.

Validation: `python tools/test-island-export.py` checks X-major cell orientation, flags and malformed/truncated files using synthetic input. **BWVR > Verify actor actions and terrain (Play mode)** checks the real runtime creature/worker cycles, carried-resource hand reach, drinking reach, pause, exactly-once gift credit and terrain/collider agreement. Carry/drink hand poses are original procedural IK over UAL clips; no dedicated carry/drink clips are claimed to exist in the free library. Headset performance and comfort need hardware testing.


## Original island placements

The optional owned `Land1.txt` now supplies the layout as well as the LND terrain. After exporting the height field, run:

```powershell
python tools/prepare-placements.py "..\Shade\.local\native-game-data\Scripts\Land1.txt"
```

Then use **BWVR → Import original first island terrain** in Unity. Re-run the placement export after re-exporting terrain: the latter replaces `.local/land1.json`. The script extracts data only; it does not execute original commands. Both raw records and generated Unity assets remain ignored, alongside the owned height field. Public clones retain the procedural fallback.

Current local source: 38 completed buildings/town centres, 1,398 vegetation records (including three dead trees), 132 rocks, 19 fields and 18 landmarks = 1,605 scene placements. Original X/Z, yaw and size multipliers are retained; Y comes from the imported terrain, plus explicit rock vertical offsets. Source line numbers identify each scene object. Planned buildings are not completed structures and are not spawned. Conversion tests cover radians versus legacy milli-radians, current versus maximum tree size, mobile puzzle versus well type recognition and invalid coordinates. Runtime checks compare every scene replacement against its source record.

Quaternius modular buildings replace the original abodes. Original storage-pit sites use stone rings and graveyards use small stone markers. Tree variants select broadleaf, pine or bush replacements. The current catalog lacks palms and dead-tree meshes, so those use approximate silhouettes; statues and geological pillars also use approximate stone replacements. Fields use grass patches, not original crop artwork. Decorative streams, fish farms, animals, furniture, lanterns, weather and big-forest commands are reported as unsupported rather than presented as recreated. The detailed Quaternius vegetation switches to a shared 18-triangle silhouette at overview distance; distant material instances are shared. This preserves positions with a lower distant geometry cost, but is not a headset performance certification.

The existing three active workers begin at three distinct original villager positions; the 55 source villager records are not all simulated. Their work targets use original field, tree, drinking and storage locations. There is **no explicit well record** in this Land1 script. The slice's modern well is assembled from Quaternius stone/wall/roof pieces at the drinking waypoint nearest the first town's storage pit. This is a gameplay substitution, not evidence of an original well at that coordinate. The delivery destination is the actual original storage-pit coordinate. The modern drought rules and direct-route actor movement remain the slice's gameplay, not a recreation of every original mission.
