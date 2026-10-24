# BWVR visual upgrade shortlist

Direction: a warm, painterly miniature world. Preserve the recognizable creature silhouette, village scale and gentle god-game atmosphere while replacing blurry legacy surfaces and sparse scenery. Recommendations are based on publisher descriptions, not a completed Quest benchmark.

## Recommended environment candidates

- [Quaternius Stylized Nature MegaKit](https://quaternius.com/packs/stylizednaturemegakit.html): textured, Ghibli-inspired trees, plants, rocks and foliage. The complete kit lists 116 models; Standard is a free subset, while Pro/Source add content and engine-ready setups. The publisher lists CC0. Start by testing one tree, one rock and one grass cluster together.
- [Quaternius Medieval Village MegaKit](https://quaternius.com/packs/medievalvillagemegakit.html): more than 300 modular building pieces across the complete kit. Standard is a free subset; Source includes a Unity URP setup and custom shaders. The publisher lists CC0. Adapt the palette and silhouettes toward rustic, rounded thatched dwellings rather than a generic fortified fantasy town.
- [Quaternius Ultimate Stylized Nature Pack](https://quaternius.com/packs/ultimatestylizednature.html): 63 models, including seamless textures and normal maps. A useful texture reference and alternative to the newer MegaKit; assess its style alongside village assets before mixing them.

Free editions are sufficient for an initial comparison. Their Unity-ready Source packages use URP; BWVR currently uses the Built-in pipeline and a custom stereo-compatible game-art shader. Raw FBX/OBJ imports therefore need material setup, or a separately planned URP migration. No package purchase or replacement has been made as part of this shortlist.

## Creature

Keep the existing original cow and animations while developing a new hero model with a friendly face, expressive eyes, readable hands, a clean biped rig and complete stand/walk/carry/drink/reaction clips. Generic farm-animal packs are not a direct substitute for the upright god creature. Prioritize the creature's close-up appearance because it is the central interaction target in VR.

## Order of work

1. Compare a small nature/village sample against the current scene under the same lighting.
2. Replace the flat island with shaped terrain, paths, banks, water edges and clustered planting.
3. Establish a consistent ground/roof/wood/stone palette, then add lighting and contact shadows within a Quest budget.
4. Replace the hero creature and retarget interactions/animation.
5. Profile on Quest 3 and Quest Pro before increasing foliage density, material count or shader complexity.

Texture resolution alone will not fix the flat terrain, weak composition, placeholder water and inconsistent scale. Those are part of the same visual pass. Prefer efficient silhouettes, shared materials, controlled transparency and LODs to unbounded scene detail.
