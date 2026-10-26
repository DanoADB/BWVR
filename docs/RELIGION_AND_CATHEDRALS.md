# Religion, worship and fanaticism

Faith is an individual saved value, independent of affection/trust. Religion now has a bounded, saved village pressure model. This increment implements worship behavior and recovery; the five cathedral building models and their construction/upgrades are still pending.

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

## Persistence and future cathedral

Save pressure, fanatic status, episode/recovery counts, cooldown and each adult's ritual schedule/time. Transient paths and hand poses restart safely; no physics replay. All timers use simulation time and explicit pause/application suspension. Historical saves without religion fields remain valid.

Planned cathedral progression: shrine, chapel, village church, Gothic church, grand cathedral. Visible construction should require village materials and labor. A cathedral's completed level should not shrink when fanaticism subsides. Candidate CC0 sources are listed below; they have not been imported or performance-validated:

1. Kay Lousberg shrine — https://poly.pizza/m/tFxdxO5clk
2. CreativeTrio church — https://poly.pizza/m/GHzPfvoyzX
3. Daniel Andersson medieval church — https://opengameart.org/content/medieval-church
4. loafbrr Gothic-Like Structures (three assembled churches) — https://loafbrr.itch.io/gothic-like-structures
5. Zuendholz cathedral (old/unfinished; compatibility work needed) — https://blendswap.com/blend/1863

Preferred consistent upper-stage source: SigilsVault Modular Cathedral Kit, CC0, 40+ low-poly pieces — https://sigilsvault.itch.io/modular-cathedral-kit . Building staged prefabs from this kit is future work, not five ready imported upgrades.

## Verification

Editor model and runtime checks cover sustained consensus, hysteresis/recovery, cooldown, finite values, legacy saves, cleanup, player pickup, prayer poses, pause and serialized state. Live results are recorded after the final run. Headset presentation and long-running balance remain playtests.

2026-10-26 Editor results: nine checks passed (religion model/runtime, provisions, hybrid model/runtime, development focus, creature carry, feedback and god tools). After the body sight-point and prayer-height corrections, religion model/runtime, provisions and creature carrying passed again. A separate full-scheduler fixture stopped work for eleven adults and demonstrated recovery with four actual line-of-sight witnesses to an accepted creature toy activity. The corrected hands-together pose was rendered from the live rig at frame 8332, after staging at frame 7266; staging was restored at frame 12703. The sight test aims at the observed actor collider center and ignores only that actor/self colliders, retaining other occlusion. This is Editor/staged validation, not a headset or long-running balance claim.
