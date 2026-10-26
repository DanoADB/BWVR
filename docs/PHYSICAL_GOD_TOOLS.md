# Physical god tools — S2

The palette adds a **Rain brush** beside the existing guidance, cistern Rain and Healing tools. Select it and hold the normal tool trigger over a supported patch of dry island ground (desktop F). Release to stop. The circular footprint changes to a crossed shape when invalid, as well as changing color. Tracking loss, pause, carrying/zoom/menu conflicts and dismissal stop the channel. These gestures keep existing pickup, zoom and feedback bindings.

## Rain increment

Each accepted half-second pulse costs 2 energy, up to 16 pulses / 8 seconds per hold. Insufficient energy requires trigger release before resuming; no catch-up burst. Payment follows actual wetting/cooling or planted growth. Preview is read-only. At most eight registered tree/building materials are affected, prioritizing hot objects, within a twelve-unit footprint. Held objects are excluded. The 192-material save cap remains enforced; full rain and saturated/irrelevant areas are rejected without charge.

A direct rain dose uses the weather model's existing wetting/cooling coefficients and material heat capacities, without advancing fuel burn/simulation time twice. Water can extinguish cooled fire. Natural weather continues its normal step afterward, so temporal sampling is not an exact deterministic equivalence to a continuous ambient rain field. The brush adds only the precipitation amount above current natural rain. Natural rain now gives growing crops a small common growth bonus; brushing supplies its missing local fraction. Neither form creates seeds or immediately harvested food. Food-work observation mode stays enabled as before.

Material wetness/heat/fire and crop growth persist in their existing bounded save records. Gestures, cooldowns and particles are transient; loading does not replay a stroke. New-season state resets transient brush timing. No extra ground-moisture simulation, global water/crop grant or creature training is introduced. The existing cistern rain remains a separate 35-energy effect with its original explicit resource accounting.

Rain presentation is capped at 64 droplets with short pooled pour cues. Unsupported empty terrain has a preview rejection rather than pretending it received a simulated benefit. Editor check: **Verify physical rain brush (Play mode)**. Rain runtime check passed in the Editor. The combined run passed rain plus twelve regression checks; the existing palette check initially assumed a lifetime use count of zero. Its corrected full body passed when tested relative to the starting use count, preserving exact cistern rain cost/outcome. Native headset gesture/readability remains unverified. Original save restored after confirmed Play stop.

## Wind increment

Select **Wind fan**, point at island ground near a free beachball or loose construction pieces, and hold the normal tool trigger (desktop F). The arrow shows horizontal direction; a nearly vertical controller ray uses the current view heading. Empty or unsupported patches show a crossed preview and cost nothing. The palette now has seven tools, with a centered third row.

Each accepted quarter-second pulse costs 1.5 energy, up to 24 pulses / six seconds per hold. At most eight bodies are nudged. Every horizontal velocity change is capped at 0.85 game-units/s; wind-generated horizontal speed is capped at 8. Existing faster momentum changes gradually rather than being erased in one step, and vertical velocity/gravity is preserved. Loose block rotation is damped to a bounded spin. Effects use each private physics scene's coordinate frame.

The fan skips held/kinematic objects, snapped pieces, villager-owned construction, sites under repair, the creature's current target, discovery/gift reservations and balls reserved for fetch or social play. Rooted/uprooted trees, buildings and villagers are not fan-driven bodies in this first increment. Natural weather keeps its own existing behavior. A free ball nudged by the player uses its existing environmental-impulse path, so the creature does not receive throwing credit for that intervention.

Leaf-like motes are local presentation, capped at 32 particles, and a short original gust uses the existing pooled audio service. Together with the rain brush the particle budget is 96; no extra audio source is introduced. This is not a new global wind field, foliage-bending system or AI hearing feature. Release, tracking loss, pause, dismissal, exhaustion and input conflicts stop the channel. Energy exhaustion requires release before rearming. New-season/reload state does not replay a held channel.

Energy and player-attributed outcome receipts persist. Block poses use existing construction saving; ball momentum remains transient, matching the existing ball system. No creature training or free village resource delta is generated.

## Final Editor verification

The combined run passed all seventeen checks: wind, rain, weather model/runtime, music/audio, responses/evidence, hybrid model/runtime, development focus, creature skills, god tools, miracles, creature carrying, feedback, population and observation/child pickup. Wind tests isolate known registry members for ownership/velocity cases and restore the registry afterward. A separate native ball-physics run moved the ball 0.463 game units horizontally after one 1.5-energy pulse. Live frames advanced into the tens of thousands. The expanded palette was rendered and inspected, then the rain-brush glyph was raised to clear its caption and the matching live adjustment was rendered again.

The final console inspection reported no current compilation/gameplay errors and one warning. The original season file was restored after confirmed Play stop and its hash matched the backup. No APK was built. Quest controller comfort, spatial readability, subjective sound and standalone cost remain hardware checks. Next implementation stage: S3 useful shelter construction and bounded planting.
