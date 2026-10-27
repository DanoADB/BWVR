# BWVR roadmap

Updated 2026-10-26 following research into immersive sims, emergent AI, god games and emergent city builders. This is the current priority and status index. Older dated entries in GAMEPLAY_DIRECTION.md record history; they are not the current delivery order. Follow [BUILD_SEQUENCE.md](BUILD_SEQUENCE.md) for implementation order and acceptance gates.

## Direction

The unseen player creates opportunities through physical tools, toys, objects and occasional miracles. The creature chooses how to respond, learns through experience and feedback, and is the god villagers perceive. Villagers pursue needs, relationships and projects independently. Their choices leave a visible, remembered history.

Prioritize enjoyable player interaction and understandable consequences. Keep ordinary play largely nonverbal, water secondary in development focus, and the wrist interface hidden until requested. Preserve movement, zoom, grabbing, teaching, save/resume and Quest comfort. Extend existing action ownership, utility decisions, affordances, social memory and simulation time; do not introduce a competing AI framework.

Research supplies design hypotheses, not proof that copying a feature will reproduce another game's success. Commercial success is context, not evidence of a particular mechanic's causal effect. Features below remain proposals until implemented and validated.

## Current baseline

The S0 checkpoint is recorded in [BASELINE_CHECKPOINT.md](BASELINE_CHECKPOINT.md). S1 attention/evidence, S1A audio/music and S2 physical rain/fan tools have Editor-tested foundations. S3 now adds geometry-based useful shelters and finite player crop planting. The tool palette also adds open icon art, direct held world actions and Food/Wood gifts with witnessed religious memory; see [provisions](PLAYER_PROVISIONS.md). Religion/worship now has a scheduler-integrated foundation; see [religion and cathedrals](RELIGION_AND_CATHEDRALS.md). Five CC0 cathedral assemblies now have faith authorization, paid timber transport, bounded builders and persistent upgrades. S4 has a first musical-gathering increment: nearby spectators remember comfort and weather during shared chime play; see [musical gatherings](MUSICAL_GATHERINGS.md). Individual place memories now connect bench, shelter and nearby chime experiences to return choices and weather-sensitive comfort; see [remembered gathering places](GATHERING_PLACES.md). Completed ball exchanges now also shape place memories and autonomous play interest. Experience-based cover and beachball improvement requests are implemented; see [gathering requests](GATHERING_REQUESTS.md). The read-only Creature/People/Provisions/Places Book shell is implemented; see [Book of Creation](CREATION_BOOK.md). Confirmed block-placement and repair contributors are now recorded by place; see [placement records](PLACE_CONTRIBUTIONS.md). Requested cover and shared ball play now record confirmed use separately from acceptance. Expanded shelters can now support a native two-villager gathering with separate covered spots; see [shared shelters](SHARED_SHELTERS.md). A chime placed at a familiar gathering site can now host an in-place creature/villager duet; see [place music](PLACE_MUSIC.md). Music listeners can now use spare nearby cover in rain, with separate spot claims and individual outcomes; see [audience cover](MUSIC_AUDIENCE_COVER.md). The integrated three-useful-contribution slice gate remains next. Hardware feel/performance remains pending.

“Foundation” means a bounded implementation exists, not feature completeness or verified standalone Quest performance. Verification results and limits are recorded in the feature documents.

| Area | Current status | Next extension |
| --- | --- | --- |
| Player tools | Foundation: wrist palette, paid cistern rain, targeted healing, one saved player-issued guidance sign, local rain brush, directional wind fan and finite seed tool; old intention props hidden | Shared gathering-place slice (S4) |
| Creature autonomy | Foundation: utility selection, exclusive action owner, contextual training, needs/favorites/practice | Perceived opportunities, short reusable plans, richer learning |
| Shared activities | Foundation: ball, fetch, chime with remembered musical audiences, bench visits and cooperative construction | A gathering place with several uses and changing social meaning |
| Religion | Foundation: witnessed gifts increase saved faith; bounded rituals, collective fanaticism/recovery and five-stage paid cathedral projects | Tune cathedral/faith economy and richer worship presentation |
| Village agency | Foundation: twelve adults, individual memories, gathering, farming, storage, equipment and limited households | Need-driven projects, broader cooperation and settlement growth |
| Construction | Foundation: shared 48-piece yard, bounded authored forms, bridge use/repair and geometry-based useful shelters | Shared gathering-place uses and new projects |
| Weather/materials | Foundation: rain, fog, wind, storms, lightning/fire and coastal waves | Autonomous responses, meaningful protection and recovery |
| Technology | Foundation: personally learned axe/hoe designs, paid crafting and nearby observation | Tool experimentation, shared techniques and local traditions |
| History/explanation | Foundation: optional intent details, responses and a saved 96-event outcome ledger | Broader village evidence, discovery recognition and physical Book of Creation |
| Audio | Editor foundation: surf/wind/rain, sparse birds, bounded contextual effects and device-local mix controls | S1A includes an original intermittent instrumental sketch; headset audition/performance pending |
| Release | Editor checks and previous APK workflow exist | Current Quest Pro/3 comfort, performance and release validation |

See the feature documents for exact limits: [tools](GOD_TOOLS.md), [creature](CREATURE_BEHAVIOR.md), [play](PLAY_AND_VILLAGE_RESPONSE.md), [construction](CONSTRUCTION_YARD.md), [materials](WEATHER_AND_MATERIALS.md), [technology](TOOLS_AND_TECHNOLOGY.md), [population](EXPANDED_POPULATION.md) and [Book](BOOK_OF_CREATION.md).

S1 attention and factual outcome evidence are implemented for Editor testing; see [CREATURE_RESPONSES.md](CREATURE_RESPONSES.md). S1A environmental/contextual audio now has an Editor-tested foundation; see [ISLAND_AUDIO.md](ISLAND_AUDIO.md). The soft background melody is now implemented; see [ISLAND_MUSIC.md](ISLAND_MUSIC.md). S2 rain brush and wind fan are implemented; see [PHYSICAL_GOD_TOOLS.md](PHYSICAL_GOD_TOOLS.md). S3 useful shelter and planting now have Editor-tested foundations. Next implementation: S4 shared gathering place. Food work can temporarily be paused independently in exploration focus to observe other behaviors; see [VILLAGE_OBSERVATION.md](VILLAGE_OBSERVATION.md).

## Research attributes to incorporate

All eleven research recommendations are included below. These extend existing systems rather than replace them.

| ID | Attribute | BWVR delivery and intended player experience | Status / sequence |
| --- | --- | --- | --- |
| R1 | Consistent interacting rules | Wetness, heat, shelter, force and resources have shared meanings across tools, weather, objects and actors. Experimenting teaches transferable rules. | Partial material foundation; S2, S6 |
| R2 | Skillful physical verbs | Paint bounded rain, aim a wind fan, plant, assemble and brace. Precision and placement affect outcomes; tools are more than menu buttons. | Basic tools exist; S2, S3, S8 |
| R3 | Several solutions to a problem | Help a gathering place through shade, shelter, planting, relocation, supplies or creature cooperation. Keep feasible alternatives and account for tradeoffs. | Planned integrated scenario; S4–S6 |
| R4 | Legible input and autonomous response | Distinguish target validity, cue noticed, intention chosen, progress and outcome through gaze, gesture, affect and subtle haptics. Acknowledgment does not promise obedience. | S1 Editor foundation implemented; headset readability pending |
| R5 | Knowledge, skill and willingness are separate | Knowing a technique does not grant equipment or mastery. Practice changes capability; reinforcement changes preference. Curiosity can favor achievable learning progress over endless novelty. | Partial knowledge/practice/preference; S5 |
| R6 | Social actions have several consequences | A playful kick can create enjoyment, fear, injury, trust or repair needs according to witnesses and outcome. Avoid one global good/evil meter. | Partial ball/social foundation; S4, S7 |
| R7 | Local knowledge, imitation and traditions | Actors learn from observed successful actions, try them independently, and transmit changing practices through relationships and places. No instant village-wide unlock. | Local observation exists; S7 |
| R8 | Witness-based interpretations of godhood | Villagers interpret the creature through witnessed help/harm and credible social transmission. Track affection, fear, confidence and dependence separately; permit disagreement and different tributes. | Partial individual sentiment/tributes; S7, S10 |
| R9 | Needs and relationships drive settlement projects | Villagers identify a need, propose a site, contribute real materials, cooperate, build, use and maintain it. Households and preferences shape the settlement. | Bench/storage/farm foundation; S9, S10 |
| R10 | Play, pressure, intervention and recovery | Leave room for experimentation, then introduce observable manageable pressures. Recovery produces learning and visible change; avoid relentless chores or secretly predetermined outcomes. | Weather foundation; S6, S11 |
| R11 | Visible persistent history | Keep bounded creation/contributor/use/repair records, changing places, memorable relationships and evidence-backed discoveries in a summonable Book. | Book/ledger planned; instrumentation S1, UI S4, S7 |

## Next playable milestone: a shared gathering place

Villagers and creature can already share a bench and toys. Extend this into a place worth changing: villagers visit to rest or play; the player can arrange objects, improve shade/shelter and offer tools; the creature can investigate, play, build, help or decline. Weather reveals which arrangements work. Villagers use, improve, avoid or repair the result according to experience, and their return visits reflect relationships and memories.

Prove more than one workable intervention and an unattended path to progress. The player must have something enjoyable to do immediately, not wait for a job loop or read a command panel. Deliver S1–S4, including early audio stage S1A, as the first integrated milestone, then deepen it through S5–S7. A dry spell can remain a test condition without restoring water-job dominance.

## Early audio priority

After the baseline and event/attention audit, build environmental ambience, then context sounds, then a soft, lilting background melody, before expanding the god tools. Ocean, wind, foliage, sparse wildlife and changing weather establish place; nearby actions and restrained actor reactions explain what is happening. Gentle instrumental music should leave room for silence and never mask meaningful cues. Keep music, ambience and effects independently adjustable. Audio reinforces visual communication rather than becoming mandatory for play. See [audio direction](AUDIO_DIRECTION.md) for scope, licensing and Quest constraints; none of this entry claims implemented audio.

## Broader backlog retained

| Capability | Order / boundary |
| --- | --- |
| Environmental, contextual and musical audio | S1A first pass; extend with tools in S2, shared activity in S4, weather in S6 and polish/device validation in S12 |
| Local weather, fire response and shelter | S6; real material/resource consequences, bounded effects and recovery |
| Terrain shaping and further miracles | S8; geometry, collision, navigation, grounding and persistence must agree |
| Wood fuel, cooking and resin recipes | S9; spend actual stock, use existing material rules, no disconnected economy |
| Autonomous houses, stores and village expansion | S9–S10; safe site selection, real costs, access and maintenance |
| Relationships, households and offspring | Extend current limited foundation in S10; stable identities, parent traits and explicit population caps |
| Food reserves, crop weather damage and seasons | S6/S11; population-aware balance and observable preparation, no future-weather omniscience |
| Creative construction and practical technology | S5/S9; bounded composable techniques and useful outcomes, not unrestricted invented code |
| Rare costly possession | After autonomous loop and S12 validation; explicit interruption/cleanup, comfort and economic design first |
| Private spirit companion, voice and optional AI | Later; local tools remain usable without a service. Runtime simulation stays authoritative. Autonomous source-code editing is not part of this sequence. |
| More creatures, aesthetic polish and accessibility | Incremental after interaction/animation compatibility checks; preserve purchased-asset licensing and Quest budgets |
| Multiplayer, expansive civilizations, hostile gods | Outside this slice; revisit after the core loop is proven |

## Evidence and research lineage

These sources support the attributes; specific BWVR mechanics and sequence are our design proposals. We do not claim to have reproduced these systems or verified their fun through research alone.

| Source | Relevant lesson |
| --- | --- |
| [Warren Spector, Deus Ex postmortem](https://media.gdcvault.com/GD_Mag_Archives/GDM_November_2000.pdf) and [Arkane developer interview](https://www.gamedeveloper.com/design/not-really-artists-the-creative-team-behind-i-dishonored-i-) | Player expression, multiple approaches and interacting rules; curate harmful or confusing surprises |
| [Smith/Worch, What Happened Here?](https://media.gdcvault.com/gdc10/slides/Smith_Harvey_WhatHappenedHereWeb_Notes.pdf) | Environmental evidence and the player's own persistent impact |
| [Orkin, F.E.A.R. planning](https://www.gamedevs.org/uploads/three-states-plan-ai-of-fear.pdf) | Reusable actions and understandable intentions; adapt short plans to existing executors |
| [Evans/Short, Versu](https://cs.uky.edu/~sgware/reading/papers/evans2014versu.pdf) and [Comme il Faut](https://ojs.aaai.org/index.php/AIIDE/article/view/12454) | Social opportunities, autonomous utility choices and interacting social consequences |
| [Oudeyer/Kaplan, intrinsic motivation](https://www.frontiersin.org/journals/neurorobotics/articles/10.3389/neuro.12.006.2007/full) and [Reynolds, Boids](https://www.red3d.com/cwr/boids/) | Learning progress and local-rule emergence; neither establishes a full social mind or balanced game |
| [Generative Agents](https://arxiv.org/html/2304.03442v2), [Voyager](https://arxiv.org/html/2305.16291v2) and [emergent tool use](https://openai.com/index/emergent-tool-use/) | Relevant memory, reusable skills, opportunities and feedback; inspiration does not require runtime LLMs or on-device reinforcement training |
| [Black & White developer postmortem](https://www.gamedeveloper.com/design/postmortem-lionhead-studios-i-black-white-i-) | Trainable autonomous creature as a central relationship |
| [Reus](https://abbeygames.com/game/reus/), [From Dust](https://www.ubisoft.com/en-us/company/about-us/our-brands/from-dust) and [WorldBox](https://www.superworldbox.com/) | Influence through environment, meaningful elemental tools and autonomous societies |
| [RimWorld](https://rimworldgame.com/) | Individual histories, relationships and paced pressures create stories |
| [Timberborn developer overview](https://mechanistry.com/press/timberborn-celebrates-its-second-anniversary-and-1-000-000-copies-sold-with-a-new-trailer) | Construction changes environmental opportunities; preparation has visible utility |
| [Against the Storm design pillars](https://eremitegames.com/devlog-1/) and [design iteration](https://eremitegames.com/devlog-6/) | Adaptation, belonging and choices that avoid one safe repeated build |
| [Cities: Skylines](https://www.paradoxinteractive.com/games/cities-skylines/about) | Services, access and interacting settlement systems rather than isolated buildings |

## Definition of emergence

Recognize repeated choices among feasible alternatives, useful combinations and independently transmitted practices. An authored recipe, a random animation or forced repetition is not sufficient evidence of invention. Record observations, developing patterns and established patterns separately, with provenance and bounded evidence. See BOOK_OF_CREATION.md. Simulation truth must precede narrative claims.
