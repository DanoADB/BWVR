# Island audio: S1A ambience and contextual foundation

2026-10-26. Extends the existing slice. No AI hearing, resource effects, new input bindings or music in this increment.

## Playing and adjusting

Open the existing wrist sanctuary (desktop Tab), then **Sound**. Master, Ambience and Effects cycle through quieter levels to mute and back to full. Back returns to the normal sanctuary. Preferences are device-local PlayerPrefs, separate from the season save. There is no permanent sound HUD. All important information retains its existing visual cue.

## Coverage

- Three quiet loop beds: synthesized shore wash, breeze/leaf-like rustle and weather-linked rain. Wind/rain follow WeatherState, and accepted rain miracles contribute their temporary local-rain state. Gains fade rather than jump. Sparse directional bird sketches occur only in calmer conditions.
- Grounded distance-cadence footsteps for registered villagers, children and creature. No steps while held, paused, stationary, airborne or teleporting. This is locomotion-distance timing, not a claim of per-foot animation contacts.
- Confirmed player pickup and valid placement; real block physics contacts above a speed threshold and successful snaps; real beachball collision entries; completed harvesting and water delivery; creature acknowledgment/completion, villager recoil/shared enjoyment; accepted miracles and real lightning strikes.
- Existing discovery and chime events use the central effects pool when available, with the old mono source as fallback if the service is absent. No historical ledger replay. Preview, failed grabs, invalid drops, canceled harvesting and rejected miracles do not announce success.

## Budget and lifecycle

One existing tracked-head listener. Three 2D ambience sources and eight pooled mono 3D effects sources. Existing fallback source components remain allocated, but their normal playback routes through the pool. Source reuse replaces only lower-priority sounds; there is no deferred queue. Per-kind and fixed-size emitter cooldowns, impact thresholds, distance rejection and lower footstep priority keep crowds quiet. Active-voice headroom and temporary ambience ducking reserve space for useful contacts. No per-tree or per-wave source, audio effects stack or new mixer graph.

Spatial effects use linear falloff; near/far ranges follow the existing player rig zoom scale with island-unit floors. Doppler is off. This provides ordinary listener-relative positioning, not acoustic occlusion, HRTF simulation or AI hearing. Headset audition across zoom extremes remains necessary.

Explicit pause/application suspension stops one-shots and pauses loops in place. Resume continues loop position, never queued effects. Audio cadence uses its own bounded presentation delta, and does not depend on saved event history or grant simulation outcomes. Foot trackers reset across carry/teleport/pause. Scene teardown stops voices and releases the singleton; clips are imported resources shared by emitters.

## Sources and imports

All 18 mono sound sketches are original procedural noise/oscillator designs, with no third-party recordings. WAVs: PCM16 / 22050 Hz, about 2.19 MB source and 4.38 MB decoded PCM before Unity object overhead. Small files use preloaded PCM DecompressOnLoad; only this new owned asset folder receives those importer settings. The generator, waveform report and CC0 audio dedication live beside the assets/in tools. This is a first-pass sound palette, not photoreal recordings or an imitated score.

## Validation

Run **BWVR / Verify island audio (Play mode)**. Checks cover clip signal data, listener/pool bounds, mute/preferences, pause/suspend/resume, repetition/flood/distance gates, grounded versus carried/hovering footsteps, weather gain changes, zoom ranges and hidden wrist controls. The corrected complete audio check body passed through the live Editor evaluator after isolating a synthetic moving walker from its paused animation state. The compiled first run passed through signal/budget/mute/lifecycle tests but correctly rejected that paused walker as a footstep source. Native listener capture sampled 245,760 values over 120 advancing frames: RMS 0.01516, peak 0.07290, no clipping in that sample. This is signal verification, not a subjective listening review. Actual pickup, repeated-grab rejection, valid drop, native block snap and a physically simulated beachball bounce passed event-hook checks. Gameplay regression checks passed for responses/evidence, hybrid model/runtime, development focus, creature skills, tools, miracles, creature carry, feedback, population and food-observation/child pickup. The wrist menu render was inspected for readable, contained controls. Initial compile/API errors and test-fixture issues were corrected; the final console reported no current compile failures/errors and one warning. The original season file was restored and its hash verified after Play stopped. Physical Quest listening, spatial comfort, CPU/native memory and speaker/headphone routing remain release checks. Music is the next S1A increment.
