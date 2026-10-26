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

Each icon sits on a raised gray stone disk with a beveled edge, restrained grain, a colored inset ring and a soft contact shadow. The shared shallow cylinder has actual thickness; static vertex shading keeps its shape readable without another light. The same artwork appears in the wrist palette, as a floating selected tool near the right hand, and in placed Help/Protect/Investigate cues. Distinct silhouettes carry meaning alongside the colors.

The placed cue has no wooden signpost, stick, collider, physical ownership or actor affordance. It floats above the target/ground point and faces the player from elevated tabletop angles. Its small, slow hover uses simulation time, so pause freezes the motion. It follows an entity target when appropriate; saved ground cues retain their stable ground point. Expiration, replacement, clear-cue, guidance rejection, save/resume and the creature's decision autonomy keep the existing logic. This is a private player visualization, not a villager-visible order or acknowledgment that the creature obeyed.

Rain, Wind and Plant act at the aimed location while held. Food and Wood deliver supplies with transient sack/bundle displays. Healing remains a targeted press. These actions do not place cues. See [resource gifts](PLAYER_PROVISIONS.md). Ordinary input, aiming, pickup, zoom, planting costs, unlimited testing energy and training rules are unchanged. The old five inactive legacy interaction templates remain private for compatibility; none supplies the visible tool art now.

## Source, attribution and budget

- Original SVGs, source URLs, SHA256 hashes, pinned upstream revision and upstream license notice: `ArtSources/GodToolIcons/`.
- Runtime credits and license link ship in `Resources/GodToolIcons/CREDITS.txt`; palette footer credits Lorc, Delapouite and Game-icons.net under CC BY 3.0.
- `tools/render-god-tool-icons.cjs` converts the checked-in SVGs using Node.js + sharp. It removes the solid background and rasterizes a padded atlas; it never downloads or executes upstream code. Original vectors remain unchanged.
- One 1280×512 glyph atlas (ten entries, including the hidden legacy local-rain alias), one 1024×1024 scroll sprite, two unlit stereo-aware emblem materials, four shared meshes and four mesh renderers per emblem. The stone disk has 48 sides and six profile intervals; all icons share its mesh. No new packages, lights, particle systems, physics bodies or post-processing. The shader is a Resources asset so player builds retain it.
- The selected tool and only one active guidance icon can appear in the world at once. Inactive alternatives do not render. Resource/GPU counts need Quest profiling before device performance is claimed.

## Verification

Current Editor results are recorded after checking the rendered palette and floating cue, atlas/import settings, ownership/clear/save behavior and existing tool regressions. Headset readability and standalone performance remain unverified until tested on device.

2026-10-26 final Editor run: all eleven checks passed (provision model/runtime, planting model/runtime, palette/guidance, rain, wind, miracles, hybrid model/runtime and feedback). Rendered and inspected the actual nine-tool palette at frame 2071 and resource deliveries/floating cue at frame 2118; live frames advanced beyond startup. Confirmed 1280×512 non-readable atlas, supported shader with no compiler messages, and pinned hashes for all ten vectors. No APK or headset validation in this increment.

## CC0 scroll and stone presentation

The palette backdrop is the blank open scroll from **Papers** by **Pavel Kutejnikov**, published under CC0: https://opengameart.org/content/papers . The archive also includes the author's explicit CC0 notice. The original sheet, notice, source URL and SHA256 are retained in `ArtSources/GodToolScroll/`; runtime provenance ships in `Resources/GodToolScroll/CREDITS.txt`. Existing Lorc/Delapouite glyph licenses are still CC BY 3.0, separately credited.

`tools/render-god-tool-scroll.cjs` crops the blank scroll and resizes three painted bands so the curls stay outside the controls. It requires Node.js + sharp and fetches no external content. The original sheet is unchanged. The palette uses warm ink labels, transparent tool cards and a small hover lift of the emblem. Its original button colliders, arrangement and input ownership are preserved. Clear/Close remain explicit controls. Selected and placed emblems share the stone appearance; decorative art has no colliders or gameplay components.

Stone uses original mesh/shader code with a fixed directional gradient and two bounded noise scales, not a new external texture or lighting system. No lights, physics or post-processing were added. Standalone Quest cost and headset readability still require device checks.

Scroll/stone verification: scripts compiled without reported errors; the existing GodToolsChecks passed and physics pointers selected all nine buttons. Inspected front and angled runtime renders at frame 2557, then darkened the stone for glyph contrast and inspected matching renders at frame 6709. The scroll was loaded as a 1024×1024 sprite and the stone shader was supported with zero shader messages. No headset or APK testing was performed for this visual change.
