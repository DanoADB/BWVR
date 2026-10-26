# Six adult villagers

2026-10-26. Starting population is now six adults: three men and three women.
The original villager-0/1/2 identities and their male/female/male models remain;
villager-3/4/5 use female/male/female models. The additional three have distinct
clothing tints using property blocks, not extra material instances. Names are
stable roster labels: Alden, Mira, Rowan, Tessa, Bram and Elin. Appearance does not
change skills, role eligibility, social preferences or household rules.

All six have the existing movement, carrying, gathering, tool learning, social,
play, injury and affect-icon components. There is still one background water
worker; the additional familiar routines favor farming/forestry. Need-based jobs
are not permanent assignments. All household pair candidates include the expanded
roster, but the existing one-household/three-child foundation is not replaced by
a multi-household or generational system in this update. Children remain capped
at three, for a bounded maximum of nine residents.

Existing saves with three social/cargo/tool/trait records are validated before
loading, then extended to six with empty/new records for the newcomers. Original
records, IDs, stock, pending cargo, tools, traits, parent lineage and relationships
are preserved. Six-entry saves also validate; other array lengths are rejected.
The existing source/cargo recovery contract still runs on resume. There is no
free newcomer equipment or inventory. New seasons start with 12 grain rather than
six to retain the same initial per-adult provisioning; saved stock is unchanged.
Consumption and reserve targets count up to nine residents instead of silently
capping demand at six. Resource capacity, worker slot limits and crop-site limits
remain bounded as before, so economy tuning is still needed.

Actor placement checks sampled ground and body clearance rather than stacking
new villagers on existing spawns. First-island placements and existing humanoid
prefabs/animations are reused. The resource food/wood target IDs are kept stable
when extra actors are added ahead of them in the scene hierarchy. No new paid
assets, packages, controller bindings or APK are required.

Discovery visitor masks also grow from four bits (creature plus three adults) to
seven bits (creature plus six adults), preserving old visit history. The newcomers
can independently explore the same discovery sites without inheriting a previous
resident's progress. Their familiar roles do not change sex-dependent capability;
all adults retain the same available activities.

## Validation

PopulationChecks passed legacy/new save validation, identity-preserving expansion,
pending cargo/tool preservation, sixth-actor crafting, new parent lineage,
nine-person food demand, all six humanoid rigs, three men/three women, stable
food/water/villager targets, state icons and newcomer discovery visit bits.
FoodReserveChecks, ToolTechnologyChecks, ResourceChecks.VerifyRuntime,
TurnipChecks and HouseholdStorageChecks.VerifyRuntime passed with six actors.
SpiritChecks.VerifyModel passed without mutating observed state. Six-person
tribute consensus was checked: three supporters is not a majority; four is.

All fourteen broader regressions passed on the final combined rerun. The initial
combined pass had a DevelopmentFocusChecks null reference; that check then passed
in isolation and in the full rerun, without a production change for that failure.
A staged six-character lineup was visually inspected with fog temporarily disabled
for the diagnostic camera. Rigs, meshes and clothing variations reuse actual game
prefabs. The final live run reached frame 13767 with zero current console errors
and two warnings. Physical headset performance/readability remain unverified.

In a 20-minute unattended clear-weather run from 12 grain/18 timber, the six adults
completed 2, 8, 6, 7, 8 and 8 deliveries respectively (39 total). Five crafted hoes;
one new farm was prepared, turnips were grown/harvested, and a child was born.
State remained valid. Ready meals never reached zero after the initial five-minute
warm-up in this run; final food was 12.27 grain for seven residents. This is one
observed run, not a balanced-economy guarantee for other conditions or all nine
residents. Original user save was restored and hash-verified. No APK was built.

## Doubled starting population — 2026-10-26

The current starting roster is twelve adults: six men and six women. The original
six keep their IDs, names, appearances, familiar roles and personal progress.
Corin, Nora, Darin, Lena, Ewan and Faye add six more independent actors, with
clothing tint variation and existing male/female humanoid prefabs. The new familiar
farming/forestry roles cross the previous appearance pattern; all adults still
choose needs-based jobs and have the same available capabilities. There remains
one background water worker and the existing three-child/one-household foundation,
for a bounded maximum of fifteen residents.

This section supersedes the six-adult counts above. Save validation accepts the
historical three- and six-entry rosters and the new twelve-entry roster. Expansion
appends only missing social, trait, cargo and tool records, preserving every prior
entry, pending cargo and parent identity. The twelve-adult save format retains
version 1 with validated optional/larger arrays. New seasons start with 24 grain
(two units per adult); loaded inventories are never increased. Food consumption
and reserve targets count all fifteen possible residents, within existing covered
capacity. Discovery visit bits cover the creature plus all twelve adults; earlier
visit bits keep their meaning. Peer-memory limits already fit the enlarged roster.

Spawn clearance, live actor avoidance, icons and personal routines apply to every
new adult. This does not add multi-household simulation, a larger child cap or an
unlimited population. Quest device performance and food balance still need hardware
and longer mixed-weather validation; Editor simulation results are not a headset
frame-rate guarantee.

Validation for the twelve-adult update: PopulationChecks (model and runtime),
FoodReserveChecks, ToolTechnologyChecks, ResourceChecks, TurnipChecks,
HouseholdStorageChecks and SpiritChecks model passed. Actual prefab rigs and
clothing variation were inspected in a staged twelve-character lineup.

In a 1,200-second unattended clear-weather simulation from 24 grain/18 timber,
all twelve adults delivered resources: 1, 7, 9, 8, 6, 8, 7, 7, 7, 7, 6 and 10
completed deliveries (83 total). Twelve learned the hoe design; nine crafted it.
Three farms were prepared, twelve crops were sown and eleven matured. One child
was born. The season remained valid. Ready food did not reach zero after the
five-minute warm-up, but ended at about 82 seconds of meals for thirteen people;
this is an observed run, not a balanced-economy guarantee. The loop took about
23 seconds of Editor CPU time, which does not measure Quest rendering performance.

Twelve of fourteen broader regression checks passed: hybrid decision model,
gathering, shared activities, construction yard, village building, villager repair,
bridges, navigation, tree play, weather, creature carrying and teaching feedback.
Two failures remain unresolved: HybridChecks.VerifyRuntime did not confirm a
water-delivery outcome in its isolated creature loop; DevelopmentFocusChecks
encountered a null resource record in HarvestSeconds after replacing live season
state. A separate water diagnostic did complete one delivery, but the full hybrid
check still failed on retest. Do not interpret this update as passing every
regression or resolving those failures. No production workaround was added to
suppress them. Original user save restored and hash-verified after stopping Play
mode. No APK built; physical headset performance remains unverified.
