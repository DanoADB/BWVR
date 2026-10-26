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
