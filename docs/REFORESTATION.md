# Renewable forestry foundation

Villagers now replant depleted registered tree sites. A source must have been cut, have no collectable wood remaining and no timber cargo in transit, and be safe and reachable. One worker reserves it, carries a visible seedling, walks to the existing approach and spends ten simulation seconds planting. Low food and storage construction retain priority. Planting is an occasional resource action, not a permanent assigned profession; family visits and existing play/social activities continue.

The island’s existing Quaternius tree art supplies growing saplings, matching the site’s species when its model is available (CommonTree_1 is the default). Growth takes five simulation minutes, increasing visible height from 12% to 80% of the original tree. On maturity the existing authored tree returns, offering one new finite twelve-unit timber yield. The original rooted navigation footprint is conservatively reserved during growth so actors avoid the planting site. A small procedural fallback keeps projects without imported environment art functional. No new Rigidbody or additional unbounded forest is created.

Tree planting reuses naturally available local seed rather than adding a tree-seed inventory. Reforestation is limited to the existing sixteen registered wood sources; new forest patches remain later work. Living-tree resin and renewable wheat are now implemented separately; see RESIN_GATHERING.md and FARMING.md. Wheat sowing uses counted stored grain. Unsafe, uprooted, burning or destroyed trees are excluded. This does not repair fire damage or restore creature-moved trees automatically.

Saved source records retain growth, planting and maturation counts. Pause/suspend freezes growth; loading resumes its saved stage without offline growth or another timber credit. Canceling a worker before planting finishes releases ownership and creates no sapling. Pending timber must be delivered or recovered before replanting can begin. Sub-milliunit floating-point residue is normalized on planting.

Checks: ForestryChecks.VerifyModel and ForestryChecks.VerifyRuntime, plus existing resource, household/storage, native navigation and tree-play regression checks. A normal-scheduler unattended run should demonstrate planting and maturation without forced planting commands.


## Verified 2026-10-25

Forestry, resource and household/storage model and native runtime checks passed; the existing player save also validates with the added fields. Fourteen existing gameplay checks passed, covering hybrid decisions/ownership, development focus, shared rests/activities, construction, repairs, bridge/navigation, tree play, weather, creature carrying and feedback.

A 1,200-second unattended normal-scheduler run started with no wood and thirty grain in clear weather/development focus. Without planting commands or pre-depleted sites, villagers planted two saplings, one matured, and seventeen resource deliveries completed. End stocks were twenty-four wood and approximately 9.98 grain, with 45.02 food consumed. State validation passed. A staged half-grown sapling render confirmed its height (3.377 game units) matched the intended scale. Live frames advanced from 2 to over 20,000; the checked console interval had no warnings or errors. These are Editor checks; standalone Quest performance and headset readability remain unmeasured.
