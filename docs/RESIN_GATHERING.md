# Renewable tree resin

The sixteen registered wood trees now also offer resin while living, rooted and safe. Each linked resin source holds four units and replenishes one unit per ninety simulation seconds. A gatherer carries at most two units per jar, walks to the existing store and deposits into a separate saved resin inventory. Tree collection cups show available resin; the store has amber jars and its own physical fullness post.

Tapping preserves the tree and its timber yield. Collectors and loggers reserve the same physical tree throughout approach, work and delivery. They cannot compete for it simultaneously, and the creature cannot take a reserved tree. Resin in transit also reserves replenishment capacity, so interruption or load recovery can return it without creating or losing material. Cutting timber clears that tree's uncollected resin. Stumps and growing saplings yield none; a replanted tree resumes replenishment after maturity. Burning, uprooted, carried and destroyed trees are unavailable, and their resin does not replenish while unsafe.

Autonomous gathering maintains a small six-unit resin reserve. Food shortages and needed construction timber retain priority. When timber is adequate, rich living resin trees receive a modest preservation preference, while felled timber remains useful salvage. This is a renewable-resource foundation, not a new permanent worker assignment or a command menu.

Base resin storage is twelve units, increasing by six with each existing store expansion. Resin never enters wheat stock, feeds villagers or spoils as exposed grain. Full-capacity returns and canceled jobs use the same conservation/recovery rules as other cargo. Existing enum values and old saves remain compatible; linked source records are added lazily. Sources stay bounded at forty (sixteen wood, eight wheat, sixteen resin), below the existing sixty-four-record budget. No new physics bodies or navigation obstacles are added for jars.

Resin sealing, repair and crafting recipes remain future work. This milestone gathers and stores a reserve; it does not invent a consumption sink merely to keep villagers tapping.

Validation: ResinChecks.VerifyModel/VerifyRuntime; existing resource, forestry and household/storage checks; an unattended scheduler run; visual checks of cups and stored jars. Pause, save/load, source linkage, exactly-once deposits, cross-resource stock isolation and shared-tree ownership are required gates.


## Verified 2026-10-25

Resin, resource, forestry and household/storage model and native runtime checks passed. The existing player save validates with the new resource kind and source links. Fourteen existing gameplay checks also passed, covering hybrid decisions/ownership, social/shared activities, construction/repairs, native navigation, tree play, weather, carrying and feedback.

A 1,200-second unattended normal-scheduler run in clear weather/development focus started with eighteen timber and thirty grain. It gathered four resin, completed thirteen resource deliveries, and ended with all sixteen resin sources living, eighteen timber and approximately 15.76 grain. Food consumption was 45.23. No tapping commands were issued; resulting state validation passed. Staged store renders showed amber jars and the third fullness post; collection-cup rendering was also captured, though nearby tools can obscure it from a particular viewpoint. Live frames advanced from 3,206 to over 27,900. The checked console interval had no warnings or errors. Quest headset readability and standalone performance remain unmeasured.
