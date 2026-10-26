# BWVR build sequence

Current implementation order, 2026-10-26. Implements [ROADMAP.md](ROADMAP.md). This is a staged plan, not a claim that the features below exist or a calendar/token-budget commitment. Each stage ends in a playable, reviewable increment; split larger stages into small validated commits and push as work proceeds.

## Working rules

- Reuse QuestSlice/SliceState orchestration, CreatureHybrid/action coordination, existing learning/affect, villager routines/social memory, affordances, construction, IslandNavigator, material/weather and resource systems. Inspect current call sites before choosing file edits.
- Preserve old saves and the user's current save. New state is bounded, uses stable identities, respects pause/suspend and starts safely empty for historical saves. Exactly-once effects require completion receipts, not polling or held input.
- One locomotion/carry owner per actor; release reservations on failure, interruption, target loss and reload. Never take player-held objects.
- No new controller binding without auditing pickup, right side-grip zoom, locomotion, A/B feedback and palette ownership. No forced camera movement or permanent HUD.
- Perception/decisions use bounded registries, spatial queries and scheduled updates. No scene-wide per-frame searches, unbounded logs or a Rigidbody for every plant.
- Commit/push each coherent verified addition. Synchronize feature branches only when fast-forward safe; report divergence instead of overwriting work. Do not include unrelated XR/project settings or licensed art accidentally.
- Keep optional explanations honest. Record test setup and observed results; distinguish staged checks, unattended simulation, Editor rendering and physical headset validation. Do not claim an APK/device result without running it. Build APKs when requested or agreed for a release gate.

## S0 — Establish a trustworthy checkpoint

**Checkpoint:** targeted Editor baseline recorded in [BASELINE_CHECKPOINT.md](BASELINE_CHECKPOINT.md). The resource-state fixture defect is corrected; both previously failing checks passed in the final run. The historical intermittent hybrid failure's cause remains unproven. Hardware/full-suite validation is not claimed. Next planned feature is S1.

**Deliver:** confirm current source and saves, document available event receipts and tool/actor ownership, and make a short repeatable baseline scenario. Resolve or isolate the older HybridChecks water-receipt failure and DevelopmentFocus state-swap fixture failure before expanding affected systems; report any unresolved failure explicitly.

**Gate:** current project compiles; baseline pickup/drop, pause/resume, saving, autonomy and tool release/rearm work. Classify failures as product defects or demonstrated test-fixture defects with evidence. Preserve a known-good save and record starting population/weather/seed. No priority changes to water merely to make a test pass.

**Depends on:** existing implementation. **S1 Editor implementation:** delivered; see [CREATURE_RESPONSES.md](CREATURE_RESPONSES.md). Headset readability remains a release check. S1A ambience/context foundation is delivered for Editor testing; the background-melody increment passed Editor behavior/output checks. Next stage: S2 physical rain brush.

## S1 — Make attention and consequences readable

**Deliver:** when an appropriate player-placed object or cue is perceived, show a brief glance/affect response. Distinguish noticed, chosen, occupied/unavailable and completed without a text command list. Preserve the creature's right to ignore guidance. Add a small bounded, saved evidence foundation for new meaningful outcomes; do not pretend to reconstruct history before instrumentation.

**Integration:** existing perception/guidance, affect, action completion and journal; extend confirmed receipts with actor/target/outcome/source and simulation time. Keep event observation separate from training and action execution.

**Gate:** noticed cues can be declined; occluded/unperceived cues receive no false acknowledgment. Held/canceled/expired requests do not generate duplicate completions or teaching. Save/reload preserves recorded outcomes without replaying them. Input and save regressions pass. Inspect actual nonverbal presentation in Play mode; leave headset comfort unverified until tested.

**Depends on:** S0. **Research:** R4, R11.

## S1A — Establish the island's soundscape

**Status:** environmental/contextual first pass delivered and Editor signal/event checks passed. See [ISLAND_AUDIO.md](ISLAND_AUDIO.md). A streamed original background-melody increment is now implemented; see [ISLAND_MUSIC.md](ISLAND_MUSIC.md). Subjective mix audition and Quest profiling remain pending.

**Deliver in three increments:** environmental ambience first, contextual sounds second, then a soft, lilting instrumental background melody. Begin with ocean/surf, breeze and leaves, sparse birds, and weather-appropriate rain. Add nearby footsteps, pickup/drop, block contacts/snaps, ball taps/bounces, water, harvesting and restrained creature/villager reactions driven by actual events. Music should feel warm, curious and spacious, with gentle acoustic/plucked or woodwind colors, long quiet gaps and smooth transitions. No lyrics or imitation of Black & White's score. See [AUDIO_DIRECTION.md](AUDIO_DIRECTION.md).

**Integration:** audit existing AudioSources/clips/listener first. Use a small central playback/mix service over confirmed interaction/animation/material events, bounded pooled emitters and separate ambience/effects/music controls. These are presentation cues, not a new AI hearing system. Keep important sounds paired with visual cues; music yields to interactions and weather warnings.

**Gate:** ambience loops without obvious seams; weather transitions crossfade; canceled actions/reloads/held input do not replay one-shots. Footsteps follow grounded movement, contacts are thresholded/cooldown-limited, distant crowds cannot overwhelm the mix, and sounds do not multiply with every object. Verify listener-relative direction and comfortable distance behavior throughout zoom; pause/suspend/resume causes no burst or stale loops. Music can be independently reduced/muted and never obscures useful cues. Record asset licenses and measure active voices/memory on Quest before declaring device readiness.

**Depends on:** S0 and S1 event audit; implement before S2 and carry the soundscape into S4. Missing audio must not block simulation. **Research:** supports R4, R10, R11.

## S2 — Turn tools into world interactions

**Deliver in two increments:** first a bounded ground-target rain brush, then a lightweight directional wind fan. Rain uses the same wetness/cooling rules as weather and affects only implemented local crop/material properties. Wind applies capped force to supported loose objects and visible foliage/particles. Preserve existing cistern rain's explicit accounting; do not grant global water/crop bonuses for every brush patch.

**Integration:** god palette, DivineIntervention, weather/material registry, physical toys and resource/crop state. Decide energy cost/rate, stroke limits and recoverability before enabling continuous use. Revalidate at application and charge only accepted effects. Preview must not mutate the world.

**Gate:** the same object responds consistently to natural and player-made rain; wet fuel is harder to ignite according to implemented material rules. Wind cannot steal held/reserved objects or generate uncontrolled impulses. Pause, insufficient energy, tracking loss and release stop effects/spending. Verify clear budgets, finite resource deltas and save behavior. Add local rain presentation and accessible shape cues as well as color.

**Depends on:** S1, with S1A audio foundation delivered first. **Research:** R1, R2.

## S3 — Give placement and construction practical meaning

**Deliver:** one lightweight shade/shelter construction option using existing pieces, plus a physical planting interaction at bounded eligible sites. Objects must provide a real measured benefit such as cover, comfortable rest or safer storage; validate that benefit independently of a named recipe. Different supported arrangements may provide it. Planting spends a real seed/plant resource and uses saved growth; it does not create free unlimited forests.

**Integration:** construction snapping/use, geometry/material cover checks, existing farming/reforestation and navigation. Prototype clear safe placement and bracing before adding piece catalogs.

**Gate:** at least two supported arrangements can supply the same useful outcome; obstructed or unstable work is rejected or fails understandably. Walkways remain accessible. Removing a roof removes its benefit. Player-held pieces are excluded, reservations clean up and growth/build state survives reload without duplicate payment.

**Depends on:** S2. **Research:** R1–R3.

## S4 — Ship the shared gathering-place slice

**Deliver:** connect rest, bench use, ball/chime play, shade/shelter and simple villager requests at one place. The player can improve it several ways. Creature choices compete with its other needs; villagers choose whether to return based on actual utility and experiences. Add the read-only Book shell with Creature, People and Provisions pages, plus a concise gathering-place/contributor record from S1 evidence.

**Gate:** demonstrate three different useful player contributions, a declined creature suggestion, a villager-initiated encounter and progress without player intervention. Weather changes the value of a placement. A shared activity can have mixed social outcomes. Save/resume retains relationships and constructions. Book values match state, missing history is shown as unknown, and its opening neither commands actors nor silently pauses.

**Depends on:** S1–S3. **Research:** R3, R6, R9, R11.

**Milestone:** first integrated playable delivery. Evaluate whether users can identify a need, choose an intervention, see a response and understand the result before adding breadth.

## S5 — Deepen learning and reusable problem solving

**Deliver:** separate knowledge, practiced capability and willingness for a small set of construction/tool/social actions. Add short bounded plans using existing executors, explicit preconditions, costs and timeouts. Curiosity can prefer an achievable improvement opportunity; repeated mastery reduces its novelty, while failed impossible tasks stop attracting retries. Do not globally retune behavior without comparative evidence.

**Gate:** a knowledgeable unskilled actor differs from a practiced unwilling one. Practice improves measured execution; feedback changes later comparable choices without guaranteeing obedience. Fixed seed/state/events reproduce logical decisions. Plans safely fail on changed conditions, cannot monopolize the creature indefinitely and clean up owned movement/cargo.

**Depends on:** S4. **Research:** R5; supports R3/R9.

## S6 — Add environmental pressure and autonomous recovery

**Deliver:** observable storm warnings, sensible sheltering and supply protection, then one bounded fire-response capability. Villagers can seek safety, protect stores or contribute to recovery; the creature can help through existing capabilities. Use real heat/wetness/fuel and resources. Allow calm experimentation between pressures.

**Gate:** a warned storm has multiple viable preparations, including one autonomous village response. Actors cannot read future random rolls. Preparation measurably changes losses; failure remains recoverable. Interrupted jobs release claims, endangered villagers avoid hazards, and finite Quest budgets bound fire/effect work. The gathering place changes visibly through a full play/pressure/recovery cycle.

**Depends on:** S4; use S5 plans where helpful. **Research:** R1, R3, R10.

## S7 — Let local practices and beliefs develop

**Deliver:** observation → attempted imitation → independently repeated use for two existing activities. Relationships and places influence transmission. Extend witnessed interpretations of creature behavior without merging fear and affection. Add Book sentiment/creation pages and evidence-backed developing/established patterns. Track confidence/dependence only when their concrete behavioral effects are specified.

**Gate:** an unwitnessed event does not instantly change every villager's belief. Forced repetition and authored recipes alone do not qualify as discoveries. A practice can spread locally, fail to spread, weaken or change. Preserve mixed opinions, separate gift/tribute logic and bounded evidence across saves. Do not equate an association with proven causation.

**Depends on:** S1, S4, S5. **Research:** R6–R8, R11.

**Milestone:** the shared gathering place now has a remembered local history, rather than only a scripted interaction sequence.

## S8 — Add bounded terrain shaping and further god powers

**Deliver:** clear-ground raise/lower/smooth brush with explicit energy, area and height limits. Start away from structures, rooted trees, crops and actors. Further miracles are separate small additions using the same target/cost/effect contract; choose them from playtest needs.

**Gate:** preview/cancel causes no mutation. Commit updates rendered surface, collision, navigation and actor grounding together; unsupported edits are rejected. Saves store bounded deltas and old saves load safely. Navigation routes remain valid or are revalidated before work resumes. No full fluid simulation is required or claimed.

**Depends on:** S2, S6 and verified terrain/navigation feasibility. **Research:** R1–R3.

## S9 — Turn resources into useful settlement projects

**Deliver in increments:** paid fuel/cooking or preservation; useful resin recipes; then need-driven covered store/shelter projects. Propose, reserve materials, select a reachable safe site, contribute, complete, use and maintain. Allow bounded creative combinations of known techniques, with purpose confirmed by use rather than a generated name.

**Gate:** alternative projects compete for the same actual stock, costs occur once, cancellations do not mint resources, and site claims cannot block essential access. Villagers start and finish one useful project unattended. Confirm attribution to real contributors and persistence of incomplete work at safe boundaries.

**Depends on:** S3, S5–S7; terrain editing is optional, not a prerequisite for safe fixed-terrain projects. **Research:** R3, R5, R9.

## S10 — Grow a society and settlement

**Deliver:** resource-funded housing expansion, multiple bounded households, preferences for shared space and negotiated project participation. Extend existing children/parent traits carefully; no unlimited population growth. Witness-based creature relationships shape attendance, aid and tributes without all villagers agreeing.

**Gate:** a housing need can lead to a new usable home without player orders. Relationships affect cohabitation/project choices, while food, access and safety still constrain growth. Save migration retains stable identities/parents and avoids exponential pair-memory growth. Larger population must fit measured simulation/render budgets.

**Depends on:** S7, S9. **Research:** R6–R9.

## S11 — Balance seasons, preparation and replayability

**Deliver:** seasons through existing crop/regrowth/weather systems, reserve targets and recovery. Tune pressures and opportunities so the same build is not always best. Introduce variety through supported conditions and choices, not mandatory randomized recipes or a second economy. Preserve an exploration-friendly mode.

**Gate:** multiple seeded unattended and assisted runs show observable progress, resource conservation and recovery opportunities. Compare avoidable losses, idle time, action variety, intervention frequency and time spent waiting. Ensure pressure does not restore water dominance or eliminate shared play. Document failures and seeds; do not infer fun from simulation survival alone.

**Depends on:** S6, S9, S10. **Research:** R3, R9, R10.

## S12 — Validate the slice on Quest and prepare release

**Deliver:** art/audio/readability polish, accessible cue alternatives, current Quest Pro/3 performance and comfort checks, save migration and a release checklist. Perform headset checks during earlier milestones when available; this stage closes the accumulated release gates rather than postponing all hardware feedback.

**Gate:** verify physical tool reveal, aim/reach at supported zoom, pickup/carry/drop, movement, teaching, weather visibility, book readability and suspend/resume. Profile representative worst-case population/weather/construction loads and set measured budgets. Resolve or disclose remaining regressions. Produce/install an APK when requested and report the exact tested build; Editor success alone does not establish standalone readiness.

**Depends on:** chosen release scope; S1–S7 may ship as an earlier slice without S8–S11 being falsely marked complete.

## How to choose the next task

1. Follow the earliest unfinished dependency for the active milestone; the S0 Editor checkpoint is recorded, so default now is S1 → S1A (ambience → contextual sounds → gentle music) → S2 → S3 → S4. Recheck the baseline after affected system changes.
2. Write the small user-visible outcome, source integration points and acceptance check before coding. Do not implement an entire stage in one uncontrolled change.
3. Verify affected behavior plus necessary regressions, inspect actual presentation, preserve the user's save, commit and push. Record limitations in the feature document and update this roadmap's status when a stage passes.
4. At S4 and S7, use playtests to choose depth versus breadth. Reorder later work explicitly based on results; do not silently treat research suggestions as completed features.

Rare possession and the AI/voice companion remain later work. Neither blocks this sequence or ordinary play.
