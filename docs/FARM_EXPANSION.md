# Village farm expansion

Implemented 2026-10-25 after food reserves.

The existing eight wheat fields remain. Up to four extra village farm locations are selected on reachable, gently sloped terrain, away from buildings, trees, registered resources, the store footprint and construction yards. Potential locations are hidden and yield nothing. Positions are saved in original navigation units, so established farms are restored where they were made rather than relocated on load. The resource registry remains capped at 64 records; a complete set of new sites brings this island to at most 44.

An idle villager can carry a timber bundle to a new site and prepare it with the existing working animation. Preparation takes 18 simulation seconds at the site and costs two actual timber once, at completion. One future seed and the timber budget are reserved while preparing. Pause freezes the work; interruption releases ownership/reservations and spends nothing. Unfinished preparation safely restarts after loading. Prepared soil persists even if a later sowing trip is canceled. Storage construction cannot spend timber reserved for a farm.

Prepared fields still need the existing paid sowing trip: one grain, 12 seconds of sowing, then 180 seconds of visible growth and a finite 12-grain harvest carried to storage. Preparation never grants grain. Saved field preparation and timber spending are validated, along with crop and seed state. Old saves have no established expansion plots or additional grain.

The production chooser looks ahead at remaining and growing harvests. When that pipeline is below twice the population's food reserve target, it can prepare/sow before an emergency, even if covered stocks are currently adequate. It initially allows two additional plots for three people, rising to four as the population grows. Existing empty expansion plots are resown before opening more. Growth concurrency stays capped at three. Emergency collection remains ahead of new preparation; an available mature crop is still useful while future crops grow. Existing action ownership, free-play and water's development-focus priority remain intact.

The new sites reuse batched wheat geometry, shared materials and collider-free soil patches. Decorative undergrowth is cleared only for prepared farms, without removing trees or changing navigation. This adds bounded village agriculture, not unrestricted terrain editing, a second crop, seasons, crop damage or weather forecasts.

## Verification

`FarmExpansionChecks.Verify` passed model and native tests for hidden/empty future sites, autonomous expansion selection, exclusive claims, timber/seed reservations, pause/interruption/retry, exactly-once preparation costs, persistent positions, paid sowing, growth and a real delivery from the new crop. Food-reserve and farming runtime checks, resource/forestry/resin/storage models, native gathering and actor movement passed. All fourteen broader gameplay regressions passed.

In the unattended one-hour clear-weather run from 18 grain/18 timber, villagers independently prepared four farms (eight timber spent), completed 77 deliveries and matured 15 crops from 17 sowings. State remained valid. With six people at the end, stores held 25.81 grain and about 372 seconds of meal-ready food. After the first five minutes, meal-ready stock never reached zero; the minimum was 8.38 grain. This is one observed clear-weather run, not a guarantee across weather, seeds or populations.

Visual QA identified that a horizontal soil cube could intersect sloping terrain. The soil now uses a lightweight 25-vertex terrain-following mesh with no collider; crop geometry, decisions and accounting are unchanged. No APK or physical headset validation was requested.

After the soil rendering change, the native expansion check passed again and the ripe/growing preview was inspected. The final live scene advanced to frame 950 with zero current console errors and two warnings. The original player save was restored and hash-verified.
