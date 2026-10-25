# Households, children and storage

## Storage

The covered village store has separate timber and grain fill posts, physical stacks/sacks, and an uncovered overflow pile. Posts fill relative to current capacity, so their meaning stays consistent after upgrades. Up to three visibly constructed shelving expansions fit within its existing navigation footprint. Each costs six delivered timber and twenty seconds of on-site villager work, adding eighteen timber and twenty-four grain capacity. Only one worker may build at a time. Committed work continues even if consumption relieves pressure. Work survives interruption and saving; timber is paid exactly once on completion.

Villagers seek storage expansion at 85% capacity or whenever grain is exposed. Timber gathering aims for 95% of capacity. Wheat gathering can exceed cover by twelve units, so storage pressure and spoilage can arise naturally when construction falls behind; reservations allow at most twenty-four overflow grain. Overflow loses 0.025 grain per simulation second. Available sheltered space receives overflow first, including after consumption or an expansion. Sheltered grain is protected from this spoilage rule. If timber is lacking, wood gathering continues; construction takes priority over choosing another resource trip once timber is available. This is shelving expansion at one store, not placement of additional freestanding buildings. Fire/flood damage to the store is not yet modeled.

## Relationships and households

Existing friendship, trust, fear and peer memories remain authoritative. Adults occasionally arrange a visit to a reachable house. One can wait at home while the other finishes its current activity; only actors who join surrender their previous movement ownership. Visits time out after three minutes. Repeated safe time together strengthens their household bond and mutual friendship. After at least sixty shared seconds and a bond of 0.65, the pair becomes partners and shares the same saved house. A small heart marks their home. Existing work, exploration and creature interactions continue between visits. Carried or frightened adults abandon a visit safely. Storage shortages take precedence over initiating visits. Low food raises gathering priority before choosing leisure (a partnered household keeps twelve food plus three per child in reserve), while existing finite activities finish safely. Critical drought work supersedes a home visit outside development focus.

A well-bonded household (0.75) with at least eight sheltered food can have a child after five simulation minutes; subsequent births require ten minutes. A birth uses two food. Children consume pantry food alongside adults. Their stable identities, parentage, home, age and traits persist. The prototype caps offspring at three (six inhabitants total), with one partnered pair among the original three adults.

Children use existing villager artwork at child scale. Boldness, sociability, diligence and stature inherit the parental mean with bounded seeded variation. Those traits affect exploration radius, preference for following parents, walking pace, activity cadence and visible size. Children navigate near home and parents and gradually grow a little; they do not yet become adult workers or form subsequent generations. Adults retain the existing three-worker architecture. This is the family foundation, not a full population lifecycle.

All new timers use simulation time and respect pause/suspend. Older saves initialize optional state. Runtime reconstruction restores children and shared home assignments without duplicating births. No additional controller bindings or permanent text UI are introduced.

## Verification

`HouseholdStorageChecks.VerifyModel` covers accounting, exposed food spoilage, construction cost/progress, capacity bounds, birth conditions/cooldown, deterministic inheritance, population bounds and save validation. `VerifyRuntime` checks native construction, visual shelving, shared home visits, partnership, visible offspring, food cost, pause and reload. Existing resource and social/navigation regressions remain applicable.


### Verified 2026-10-25

Both new model/native runtime checks and the existing resource model/runtime checks passed. Fourteen existing checks passed: hybrid model/action ownership, development focus, shared rests, shared activities, construction yard, village building, repairs, bridge, navigation, tree play, weather, creature carrying and feedback.

An unattended 1,800-second simulation used the normal scheduler with development focus, clear weather and initial stocks of 12 timber/43 grain. Without forced build, partnership or birth commands it completed one storage expansion, formed a household, produced two children and made eighteen deliveries. End stocks were 27 timber and approximately 0.76 grain; food remained tight, so this is an autonomy demonstration rather than a claim of balanced survival. The resulting state validated. Live frames advanced from 2 to over 8,600, and the final console interval contained no warnings or errors. Staged close-up renders confirmed capacity posts, overflow, shelving, shared-home marker and child scale. These are Editor checks, not standalone Quest performance or headset readability validation.
