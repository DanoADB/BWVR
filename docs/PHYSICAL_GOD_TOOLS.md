# Physical god tools — S2

The palette adds a **Rain brush** beside the existing guidance, cistern Rain and Healing tools. Select it and hold the normal tool trigger over a supported patch of dry island ground (desktop F). Release to stop. The circular footprint changes to a crossed shape when invalid, as well as changing color. Tracking loss, pause, carrying/zoom/menu conflicts and dismissal stop the channel. These gestures keep existing pickup, zoom and feedback bindings.

## Rain increment

Each accepted half-second pulse costs 2 energy, up to 16 pulses / 8 seconds per hold. Insufficient energy requires trigger release before resuming; no catch-up burst. Payment follows actual wetting/cooling or planted growth. Preview is read-only. At most eight registered tree/building materials are affected, prioritizing hot objects, within a twelve-unit footprint. Held objects are excluded. The 192-material save cap remains enforced; full rain and saturated/irrelevant areas are rejected without charge.

A direct rain dose uses the weather model's existing wetting/cooling coefficients and material heat capacities, without advancing fuel burn/simulation time twice. Water can extinguish cooled fire. Natural weather continues its normal step afterward, so temporal sampling is not an exact deterministic equivalence to a continuous ambient rain field. The brush adds only the precipitation amount above current natural rain. Natural rain now gives growing crops a small common growth bonus; brushing supplies its missing local fraction. Neither form creates seeds or immediately harvested food. Food-work observation mode stays enabled as before.

Material wetness/heat/fire and crop growth persist in their existing bounded save records. Gestures, cooldowns and particles are transient; loading does not replay a stroke. New-season state resets transient brush timing. No extra ground-moisture simulation, global water/crop grant or creature training is introduced. The existing cistern rain remains a separate 35-energy effect with its original explicit resource accounting.

Rain presentation is capped at 64 droplets with short pooled pour cues. Unsupported empty terrain has a preview rejection rather than pretending it received a simulated benefit. Editor check: **Verify physical rain brush (Play mode)**. Rain runtime check passed in the Editor. The combined run passed rain plus twelve regression checks; the existing palette check initially assumed a lifetime use count of zero. Its corrected full body passed when tested relative to the starting use count, preserving exact cistern rain cost/outcome. Native headset gesture/readability remains unverified. Original save restored after confirmed Play stop.

## Wind increment

Next: directional fan for supported loose, unreserved physics objects, with capped velocity and local leaf/air presentation. It must preserve held objects, snapped construction and action ownership.
