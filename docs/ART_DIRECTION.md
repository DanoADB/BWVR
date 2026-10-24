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
