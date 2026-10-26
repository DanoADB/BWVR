# God tool emblems

The nine visible tools now use open artwork from [Game-icons.net](https://game-icons.net/about.html), drawn by Lorc and Delapouite under [CC BY 3.0](https://creativecommons.org/licenses/by/3.0/). We compared Kenney Game Icons (CC0) and Game-icons.net; the latter supplied one coherent fantasy silhouette family for the tool family. These are imported artist-made icons, not newly generated replacement shapes.

| Tool | Imported artwork | Artist |
| --- | --- | --- |
| Help | Shaking hands | Delapouite |
| Protect | Heart shield | Delapouite |
| Investigate | Magnifying glass | Lorc |
| Rain | Raining cloud | Lorc |
| Heal | Healing figure | Delapouite |
| Wind | Windy stripes | Lorc |
| Plant seeds | Seedling | Delapouite |
| Food | Meal | Delapouite |
| Wood | Wood pile | Delapouite |

## Presentation

Each icon sits on dark enamel with a narrow colored rim and a restrained translucent outer ring. The same artwork appears in the wrist palette, as a floating selected tool near the right hand, and in placed Help/Protect/Investigate cues. Distinct silhouettes carry meaning alongside the colors.

The placed cue has no wooden signpost, stick, collider, physical ownership or actor affordance. It floats above the target/ground point and faces the player from elevated tabletop angles. Its small, slow hover uses simulation time, so pause freezes the motion. It follows an entity target when appropriate; saved ground cues retain their stable ground point. Expiration, replacement, clear-cue, guidance rejection, save/resume and the creature's decision autonomy keep the existing logic. This is a private player visualization, not a villager-visible order or acknowledgment that the creature obeyed.

Rain, Wind and Plant act at the aimed location while held. Food and Wood deliver supplies with transient sack/bundle displays. Healing remains a targeted press. These actions do not place cues. See [resource gifts](PLAYER_PROVISIONS.md). Ordinary input, aiming, pickup, zoom, planting costs, unlimited testing energy and training rules are unchanged. The old five inactive legacy interaction templates remain private for compatibility; none supplies the visible tool art now.

## Source, attribution and budget

- Original SVGs, source URLs, SHA256 hashes, pinned upstream revision and upstream license notice: `ArtSources/GodToolIcons/`.
- Runtime credits and license link ship in `Resources/GodToolIcons/CREDITS.txt`; palette footer credits Lorc, Delapouite and Game-icons.net under CC BY 3.0.
- `tools/render-god-tool-icons.cjs` converts the checked-in SVGs using Node.js + sharp. It removes the solid background and rasterizes a padded atlas; it never downloads or executes upstream code. Original vectors remain unchanged.
- One 1280×512 atlas (ten entries, including the hidden legacy local-rain alias) with mipmaps, one unlit stereo-aware material, three shared meshes and four simple mesh renderers per emblem. No new packages, lights, particle systems, physics bodies or post-processing. The shader is a Resources asset so player builds retain it.
- The selected tool and only one active guidance icon can appear in the world at once. Inactive alternatives do not render. Resource/GPU counts need Quest profiling before device performance is claimed.

## Verification

Current Editor results are recorded after checking the rendered palette and floating cue, atlas/import settings, ownership/clear/save behavior and existing tool regressions. Headset readability and standalone performance remain unverified until tested on device.

2026-10-26 final Editor run: all eleven checks passed (provision model/runtime, planting model/runtime, palette/guidance, rain, wind, miracles, hybrid model/runtime and feedback). Rendered and inspected the actual nine-tool palette at frame 2071 and resource deliveries/floating cue at frame 2118; live frames advanced beyond startup. Confirmed 1280×512 non-readable atlas, supported shader with no compiler messages, and pinned hashes for all ten vectors. No APK or headset validation in this increment.
