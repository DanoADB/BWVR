# Religion, worship and fanaticism

Faith is an individual saved value, independent of affection/trust. Religion now has a bounded, saved village pressure model. Worship behavior, recovery and a five-stage CC0 cathedral construction loop are implemented. Quest presentation and long-running balance remain playtests.

## Behavior

- Faith below 0.45 adds no religious interruption.
- At 0.45, an adult may take a four-second ritual, with a 70-second recovery interval plus a small identity offset.
- At 0.65, rituals last eight seconds with a 35-second interval. Ordinary rituals wait for other activities rather than stealing a reserved resource or household visit.
- When at least 75% of the twelve adults have faith of 0.85 or above, obsession pressure builds at one point per simulation second. Thirty points triggers collective fanaticism. A single enthusiastic villager cannot cause it.
- Fanatic adults abandon productive work and gather around the existing tribute altar, facing their creature god with hands together. Injury, fright, player carrying and unreachable destinations have safe exits. Children retain their separate existing routines.

The existing scheduler owns worship. Entering cancels gathering, social, exploration, construction-play and household reservations through their existing cleanup paths. Rituals do not create resources, fake creature completions, learning rewards or free religious energy. No new command menu is added. The optional Help view reports the village religion state; ordinary play uses congregations, prayer poses and existing heart affect.

## Recovery and intervention

Worshippers can observe a nearby creature playing, building, socializing or delivering water. If at least one third of the adults can see such an active activity, pressure falls; detection uses bounded cached actors and refreshes twice per second. Player toys, placed opportunities and relocating the creature can help produce that situation, but the creature still chooses its own actions.

Insufficient majority also lowers pressure. Food shortage (when food observation mode is off) or critical water shortage (outside development focus) can challenge a fanatical village. The current testing preferences keep food/water pressure deprioritized. Gifts can strengthen individual faith and thereby worsen obsession; there is no artificial decay applied to faith itself.

Fanaticism ends at twelve pressure points. A saved sixty-second cooldown prevents immediate relapse, then ordinary ritual schedules resume. Repeated fanaticism remains possible if high belief continues without a balancing activity. Faith is not forcibly erased. This is initial tuning, not a balanced religion economy.

## Cathedral construction and persistence

Twenty sustained simulation seconds of mean village faith at 0.25 / 0.45 / 0.55 / 0.65 / 0.75 authorize the next stage: shrine, chapel, village church, Gothic church, grand cathedral. Lost qualifying faith reduces the unfinished qualification timer. An authorized project and completed levels remain when faith falls; fanaticism suspends productive labor rather than demolishing buildings.

Each stage costs 6 / 12 / 18 / 24 / 32 unreserved timber and 120 / 180 / 240 / 360 / 480 worker seconds. At most two available adults build. They walk to existing storage, collect up to three paid timber, carry it to the forecourt and work with the existing animation. Delivered timber is visible in piles. Reserved farm/tool materials cannot be spent on the cathedral. No free resource or learning receipt is generated. The player's Wood gifts can support the supply chain.

Timber in transit is saved by stable adult identity, alongside on-site stock, labor, stage, authorization, qualification and plot. Interrupted builders retain that paid cargo and resume carrying it later; reload clears transient paths and visual carry ownership. Ordinary worship waits for a busy builder; fanaticism cancels building before taking ownership. Player pickup, injury and fright stop work. All progress respects simulation pause and suspension. Historical saves without cathedral data remain valid.

A bounded startup search selects clear reachable land, rejecting existing navigation exclusions, steep ground, the construction yard, resource/crop patches and occupied settlement props. The visible foundation fits the current authorized/completed stage. The final footprint is reserved once construction starts, with navigation carving and safe route recovery, so later upgrades cannot engulf actors. Buildings have physical box colliders and a fixed exterior forecourt; interiors are not traversable gameplay spaces. Worship gathers outside that forecourt, still facing the creature god. The optional Help view reports building progress; no normal-play construction menu is added.

## Open artwork

All five buildings are BWVR assemblies of **Kevin Barany / SigilsVault's Modular Cathedral Kit v1.0**, CC0: https://sigilsvault.itch.io/modular-cathedral-kit . The downloaded kit includes an explicit CC0 license; its original license/readme and the used GLBs with SHA-256 hashes are preserved under `ArtSources/Cathedrals`. Earlier candidate lists were research, not imported assets.

`tools/prepare-cathedrals.py` assembles the source pieces with Blender; **BWVR → Import cathedral stages** creates combined Unity meshes, shared materials and runtime prefabs. A common scale preserves door proportions across upgrades. Stone/roof, stained windows and timber doors use three shared 512px atlases. Each active building has at most three renderers. Triangle counts are 798 / 1,973 / 5,581 / 8,999 / 14,943. Outside construction only the completed level is visible; construction uses the labor-driven facade described below. No interior lights, real-time reflection probes or per-window particles are added. These are content budgets, not a measured headset frame rate.

## Verification

Editor model and runtime checks cover sustained consensus, hysteresis/recovery, cooldown, finite values, legacy saves, cleanup, player pickup, prayer poses, pause and serialized state. Live results are recorded after the final run. Headset presentation and long-running balance remain playtests.

2026-10-26 Editor results: nine checks passed (religion model/runtime, provisions, hybrid model/runtime, development focus, creature carry, feedback and god tools). After the body sight-point and prayer-height corrections, religion model/runtime, provisions and creature carrying passed again. A separate full-scheduler fixture stopped work for eleven adults and demonstrated recovery with four actual line-of-sight witnesses to an accepted creature toy activity. The corrected hands-together pose was rendered from the live rig at frame 8332, after staging at frame 7266; staging was restored at frame 12703. The sight test aims at the observed actor collider center and ignores only that actor/self colliders, retaining other occlusion. This is Editor/staged validation, not a headset or long-running balance claim.

2026-10-26 cathedral results: model progression and runtime construction checks passed, as did religion, provisions, creature carrying, feedback and development focus (seven relevant checks across the run). The combined pickup check initially encountered the preceding provision fixture's deliberate input release gate; it passed after an explicit released-input frame, along with feedback/focus. Cathedral model/runtime and religion passed again after the final foundation/site changes. Rendered all five prefabs and the staged in-world shrine. A live, unteleported builder run advanced from frame 5228 to 17883 and completed the paid shrine at 122.5 simulation seconds, including a delayed builder returning from a household activity. The final presentation fixture verified clearance from registered resource patches and was restored at frame 4281. Original season JSON was restored with matching SHA-256, and standalone/Android XR startup settings restored to true. No APK or headset performance test was performed.

## Easier first shrine and visible construction (2026-10-31)

The first threshold is now 25%, so the saved village's 31% mean faith qualifies
after the existing twenty-second sustained-belief period. Higher thresholds and
timber costs are unchanged. Work now requires 120 / 180 / 240 / 360 / 480 worker-
seconds. Two continuously working villagers can finish the first shrine's labor
in a minute; qualification, fetching, walking and interruptions add to that time.
Existing partial saves retain their paid timber, absolute work and cargo.

The site shows delivered timber and temporary sourced wooden scaffolding. Actual
labor reveals the next building from its base upward; roof geometry appears only
as the build reaches it. Shader clipping applies equally to the visible mesh and
its shadow, so an absent roof does not cast a completed-building shadow. Side
platforms rise in four tiers, leaving the center open to the tabletop view. The
previous completed facade remains during delivery and is replaced by the rising
upgrade once construction work starts. Temporary scaffolding disappears at
completion; the ordinary completed building and colliders resume.

Presentation is derived entirely from existing saved work and delivered stock.
Reload restores the same visible height. Pause freezes progress. In-progress
facades have no active colliders or interaction ownership; existing final-plot
navigation reservation remains authoritative. Kenney's existing CC0 plank and
floor prefabs supply supports, platforms and the foundation, with the existing
SigilsVault cathedral meshes supplying the actual building. No extra physical
stock is created by decorative scaffolding.

`CathedralProgressChecks` verifies the lowered threshold at 31%, the full labor
requirement, natural authorization, supplied scaffolding, all five labor-driven
facades, pause, native Save/Load and completion cleanup. Existing cathedral model
and worker/cargo/runtime checks pass. Rendered progress stages are reviewed in
the Editor. Quest shader/comfort/performance validation remains a headset gate.

A separate live Editor run cloned the user's saved village (31% mean faith,
86 wood, cathedral level/authorization zero). It naturally authorized, physically
collected/delivered timber and completed the first shrine, with no assigned
progress or builder teleporting. Simulation was accelerated sixfold and automatic
weather changes held fixed for this check; other village/creature routines ran.
The completed level and empty transit cargo were confirmed before restoring the
original state, actor poses, weather setting and time scale. This demonstrates
the saved-village path, not a measured real-time headset completion duration.
