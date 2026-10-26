# Renewable wheat

The existing eight registered wheat patches now support another harvest after villager sowing and visible growth. This extends the resource scheduler and shared stores; no permanent farmer assignment or player command menu is added. The spirit/agent work is deferred while this earlier resource milestone is developed. Water remains secondary in development focus.

## Playable loop

- Villagers harvest a ripe patch in finite three-unit loads and deliver them through the existing carry/deposit system. Growing crops cannot be harvested.
- A depleted patch becomes eligible for sowing only after all its previous harvest cargo has been delivered or recovered. A worker reserves the plot and one unit of covered grain, carries a seed pouch, walks to the existing approach and spends twelve simulation seconds sowing.
- Seed is charged exactly once on successful completion, recorded in `seedUsed`. Cancellation or timeout before completion releases the plot and seed reservation without spending grain or creating a crop. Normal food consumption cannot eat seed reserved for a current sowing trip. The pouch is a work prop, not a separate grabbable resource object.
- Wheat grows over 180 unpaused simulation seconds. Green shoots increase in height; grain heads appear during the last 35% of growth and turn gold as they ripen. At maturity the patch supplies twelve new harvestable units, rather than crediting the store directly. Partial harvesting lowers the visible crop to stubble.
- At most three registered patches may be reserved for sowing or growing at once. Other villagers retain their work, family, play and social opportunities. Low food favors collecting available ripe wheat before taking on sowing; storage expansion keeps its existing priority.

Sowing requires one actual covered grain unit. There is no free seed creation or rescue harvest when all stores and fields are empty. Canceling the last reserved sowing trip makes that grain available to residents again. An unattended village therefore still needs a functioning harvest/storage pipeline.

## Persistence and accounting

The source's existing `growing`, `growth`, `plantings` and `regrown` fields now support wheat as well as wood. Wheat remains distinct from a felled tree; resin cannot acquire crop growth state. Validation checks the typed states, counters, pending cargo and seed-spending total. Old saves with existing finite wheat fields remain valid and start with their prior remaining yield. They are not reset or awarded new grain on load.

Growth and the seed-use total persist in season saves. Loading a completed planting resumes its stage without offline growth or another charge. An unfinished sowing trip restarts from a safe idle boundary; because payment occurs at completion, it does not need a seed refund. Pending harvested grain follows the existing recover-to-source rule before a plot can be replanted. Extra growth time after maturity never produces multiple harvests without another paid sowing job.

## Rendering and limits

Reuse the existing two batched meshes per patch, shared materials and cached renderers. A shared material-property block sets growth/ripeness colors; plants have no colliders or Rigidbodies and add no navigation obstacles. At startup, decorative collider-free undergrowth overlapping the farm footprints is hidden to expose the shoots and cultivated soil; trees, buildings and actor targets retain their existing state. The registry stays at eight crop sites and forty resource records including timber and linked resin. New farms, other crops, milling/baking, weather damage to crops, seasons and a dedicated seed granary remain later work. Crop growth is currently a fixed simulation-time rate, not a rainfall/soil model.

## Validation

`FarmingChecks.VerifyModel()` covers finite harvests, seed spending and reservations, pending cargo, immature harvest rejection, saved growth, exactly-once maturity, legacy/corrupt state and grain accounting. `FarmingChecks.VerifyRuntime()` exercises a native villager's plot/seed reservation, pause, interruption/retry, sowing animation, growth visuals, saved stages and a renewed harvest delivered to the store. Existing resource, forestry, resin, household/storage and gameplay regressions remain relevant. Editor verification and unattended results are recorded after running them; no Quest APK is requested for this milestone.

## Verified 2026-10-25

Five model checks and five native runtime checks passed across farming, resources, forestry, resin and household storage. Fourteen broader gameplay regression checks also passed. Farming and navigation were rechecked after the decorative clearing change. An initial Unity lifecycle error from constructing a material-property block in a component field initializer was corrected before the passing runs.

A one-hour unattended simulation, starting with normal fields and without planting commands, completed seven paid sowings, seven crop maturations and 67 resource deliveries. Remaining covered wheat was approximately 0.97 units: this demonstrates renewable farming, but does not establish a balanced or guaranteed self-sufficient food economy. Reserve tuning and additional farm sites remain future work.

Staged green and ripe render captures were inspected. Young shoots remain small among surrounding vegetation, while ripe grain has visible golden heads. The original player save was restored after testing and its hash matched the pre-test backup. No Quest APK or headset performance validation was performed for this milestone.

Food-reserve priorities and a population-aware readiness marker now extend this loop; see FOOD_RESERVES.md. Crop variety, new farm plots and weather preparation remain separate upcoming work.
