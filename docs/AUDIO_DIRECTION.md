# Audio direction

Roadmap direction, 2026-10-26. Environmental/contextual first-pass implementation now exists; see [ISLAND_AUDIO.md](ISLAND_AUDIO.md) for coverage and verification limits. An original intermittent background-music sketch is now implemented too; see [ISLAND_MUSIC.md](ISLAND_MUSIC.md). Hardware audition remains pending. The user requested environmental and contextual sounds first, with a soft, lilting background melody. Deliver this as S1A in [BUILD_SEQUENCE.md](BUILD_SEQUENCE.md), before the expanded god tools. Audit existing clips, sources and listeners before introducing new playback machinery.

## 1. Environmental ambience

- Ocean and shore wash establish the island's surroundings; breeze and leaf rustle establish land. Add sparse birds or insects where appropriate, with variation and quiet intervals rather than constant calls.
- Blend calm, wind and rain layers from current weather. Later add restrained storm/thunder and fire layers tied to actual conditions. Fog need not make a sound. Avoid a full-volume emitter on every tree or wave.
- Use a few regional beds and nearby details. Crossfade changes and avoid obvious short loops. Keep the village calm enough to hear a block being placed or a creature reacting.

## 2. Contextual interaction sounds

Start with the actions players and actors already perform:

| Context | Audible response |
| --- | --- |
| Pickup, carry release and placement | Subtle grasp/release plus material contact when impact actually occurs; no repeated grab tone while holding |
| Construction | Wood/stone contact, deliberate snap acknowledgment and restrained completed-work cue |
| Beachball and shared play | Light taps, hollow bounces and splashes; intensity follows supported impact strength |
| Creature and villagers | Sparse nonverbal curiosity, effort, enjoyment, distress and acknowledgment; avoid a voice on every decision |
| Walking and working | Grounded, movement/animation-timed footsteps and harvest/tool contacts; no footsteps while hovering, stationary or being carried |
| Water and miracles | Local pour/splash/rain/healing cues tied to accepted effects; preview does not sound like a completed cast |
| Guidance and tools | Quiet selection/validity/acknowledgment cues that distinguish player selection from the creature noticing or choosing |

Source sounds from actual animation contacts, physical impacts or confirmed gameplay events as appropriate. Threshold and rate-limit repeated contacts, pool voices and prioritize nearby important events. Presentation must not grant rewards, teach behavior or fabricate completions. Do not silently introduce AI hearing or acoustic perception; that is a separate simulation capability.

## 3. Background music

Aim for a soft, lilting, original instrumental theme: warm, lightly playful and a little wondrous. Try gentle plucked/acoustic textures and airy woodwind-like phrases over a sparse accompaniment. Leave long breathing spaces; avoid an insistent beat, dense orchestration, vocals or a permanently repeating short tune. These are creative starting points to audition, not a selected track or fixed instrumentation requirement.

Begin with one understated calm theme and clean fades. Let silence and ambience carry stretches of exploration. Later, modest variations may reflect weather or recovery without a dramatic sting for every action. Reduce music beneath useful interactions and warnings. Do not copy the original game's melodies, recordings or recognizable score arrangement.

## Mixing, comfort and accessibility

- Separate master, ambience, effects and music controls in the existing optional settings/sanctuary. Allow independent music mute and persist preferences separately from season progress. No permanent audio HUD.
- A single correctly tracked listener serves the player's head. Nearby effects should locate objects naturally; broad ambience/music should remain comfortable while moving or turning. Test distance falloff across the island's zoom range rather than making all island sounds inaudible at one scale and overwhelming at another.
- Important information retains visible equivalents: actor gestures/icons, contact motion, weather changes and tool previews. Sound must not be the sole way to detect a need, target validity or a completed interaction.
- Bound simultaneous voices, actor chatter and weather layers. Avoid harsh surprise peaks. Keep loops and one-shots clean through explicit pause, tracking/application suspension, scene teardown and resume. Do not replay saved historical events as fresh audio.
- Profile real voice counts, memory, loading and CPU on Quest Pro/3. Choose import/load/compression settings by clip purpose after auditing the project. Editor listening alone cannot establish standalone quality or cost.

## Asset sourcing and completion

Prefer original, CC0 or appropriately licensed free audio. Record source URL, author, exact license, modification and required attribution for each imported asset. Confirm redistribution terms for the actual files; “free download” alone is insufficient. Keep purchased/restricted source files out of the public repository unless their license explicitly permits that distribution. The current implementation uses 18 original procedural sound sketches with a CC0 audio dedication; no third-party sound files were downloaded.

Deliver ambience, context effects and music as separate auditionable increments. Validate real event timing, loops/crossfades, mute/mix controls, zoom, pause/resume, cancellation and bounded voices. Listen in the rendered scene and report what was actually heard; a screenshot or compilation check cannot verify sound. Extend coverage with later roadmap features, and close hardware comfort/performance checks at S12.
