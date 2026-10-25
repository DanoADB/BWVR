# Gameplay direction

Recorded from the user's design notes on 2026-10-23. These are future gameplay requirements, not a description of implemented systems. They guide later changes to the first slice.

## Core direction

- Minimize text. Communicate needs, intentions and relationships through natural behavior, gaze, gesture, objects, environmental change and eventually sound. Textual help and accessibility cues remain optional; operational errors and pause/resume controls must stay understandable.
- The player remains unseen. The creature is the god that villagers perceive and worship.
- Normal interaction with the creature happens through **tools, toys and positive/negative feedback**. Teach and influence its choices rather than issue direct action commands.
- **Possession is an exceptional form of control:** the player can possess the creature for a short period, at great cost. It must remain rare enough that learning and indirect influence matter.
- Both villagers and the creature develop **emergent behavior** rather than only repeat scripted job loops.
- Villagers choose to construct their own buildings and establish or grow villages.
- Villagers develop relationships with one another that affect their behavior.
- Villagers solve problems and develop technology, allowing their society to evolve **with or without assistance from the creature or player**. An unattended village must be capable of meaningful progress, not merely wait for intervention.

Possession is the deliberate exception to indirect creature interaction. It should not become the routine way to solve every village problem.

## Candidate simulation approach

**Beliefs, desires and intentions (BDI)** are a proposed approach, not yet a committed architecture:

- **Beliefs:** what an individual thinks is true about resources, other individuals, danger, the creature and past events. Knowledge may be incomplete or mistaken.
- **Desires:** competing needs and aspirations, such as survival, safety, companionship or improving the settlement.
- **Intentions:** the goals an individual commits to pursuing, supported by plans and reconsidered when circumstances change.

Creature learning, villager relationships and social knowledge should influence these decisions. Construction and technology should follow from perceived needs, available resources and cooperation, rather than depend entirely on player orders. Social learning and discovery through practical problem solving are candidate mechanisms to explore.

The first implementation can use a small, inspectable set of needs, memories and plans. We do not need a large simulation to establish whether the resulting decisions are compelling.

## Decisions still open

- Possession duration, entry/exit behavior, cost, recovery and consequences. No particular currency or numerical balance has been selected.
- Which tools and toys support the first teaching loop, and how feedback identifies the action being reinforced.
- The initial relationship model and how individuals share knowledge or coordinate work.
- Construction prerequisites, site selection and settlement expansion rules.
- The first technologies, how discoveries spread and what counts as societal evolution.
- Which simulation state must persist, including memories, relationships, intentions, construction and discoveries.

## Smallest useful proving scenario

Extend the drought chapter so villagers recognize a water shortage, choose a response, cooperate on a useful construction project and retain what they learned. The creature can learn to help through tools, toys and feedback, but villagers can also make progress independently. Add costly possession after the autonomous teaching loop works, so it supplements that loop rather than substitutes for it.

This scenario is a proposed development test of the direction above. It is not a claim that autonomous construction, relationships, technology or possession are already implemented.

## Emergence priorities (2026-10-24)

Prioritize opportunities for villagers and the creature to interact and change through experience. More job types are not the immediate goal. Keep the broader ideas as a staged backlog:

1. **Social foundation:** individual temperament, remembered encounters, villager-initiated attention, creature preferences and learning from nearby safe encounters. First implementation described in CREATURE_BEHAVIOR.md.
2. **Shared play:** invite the creature to a ball, chime or construction site; reciprocal turns, encouragement and consent. Teach object-specific interests through repeated experiences.
3. **Meaning from use:** recognize that the same blocks can become an arch, seat, shelter or obstacle. Villagers investigate and develop different uses; remember useful places rather than require a prescribed solution.
4. **Relationships and disagreements:** friendship, attachments and shared ownership; watching, helping, protecting or dismantling follows each actor's experience. Expand creature-villager memory into villager-villager relationships.
5. **Observation and habits:** imitation of successful actions, social transmission and persistent local traditions. Current nearby trust learning is only a small first step, not skill imitation.
6. **Competing desires and capabilities:** balance safety, hunger, play, companionship and curiosity; playful practice develops skills. Interrupt and reconsider plans when circumstances change.
7. **Environmental opportunities:** weather, scarce resources and terrain create problems that existing objects and learned skills can solve in different ways.

Use the creature-built arch as the first proving ground for stages 2–4: investigate, find a use, alter it, and react differently based on remembered encounters. Avoid a single scripted sequence presented as emergence. Autonomous settlement construction, technology, possession, general-purpose object recognition and the later stages above remain future work.

## Implementation checkpoint — 2026-10-24

Use this checkpoint before rereading the longer implementation history. Current foundations include contextual feedback/guidance, limited miracles, saved individual and peer relationships, creature/villager ball games, player throws and teachable fetch, bounded observation/practice, construction use and requests, and native physical tray/bridge repairs.

The current independent-building step adds a need-driven cooperative three-piece gathering bench on existing construction sites. It is a first village crafting capability, not autonomous house placement or settlement growth. Villagers and the creature retain separate learning, ownership and contribution attribution.

Remaining roadmap work still includes broader structure uses and shared ownership; learned skills and competing capabilities; persistent traditions beyond ball play; environmental problems/weather; autonomous houses and settlement expansion; technology discovery; rare costly possession; and headset readability, comfort, device performance and release validation. Water remains secondary in development focus. Consult PLAY_AND_VILLAGE_RESPONSE.md for the precise tested boundaries. The full roadmap is open-ended and is not a one-week completion commitment.

The shared-use follow-up lets a bench visitor invite one available neighbor. Native approach, reciprocal gaze and a completed shared rest strengthen the existing peer relationship. Saved counts and simulation cooldowns bound the interaction; canceled visits grant no reward. This adds a social purpose to constructed gathering places without new menus or creature-model dependencies.

The purchased Animal Friends gorilla now provides mood-driven facial poses and preserves its authored feedback reactions. Villager social behavior also includes checking on an injured or recently frightened neighbor, using the existing leisure ownership and peer-memory systems. Reassurance strengthens the pair's relationship without healing injury or rewriting their feelings toward the creature. These are small extensions to nonverbal communication and social agency; water remains secondary in development focus.

The next skill milestone starts with practiced gentle beachball control: confirmed creature returns improve deterministic aim precision, while contextual feedback shapes willingness to join future cooperative play. The world-space pickup/drop hand also restores a visible interaction point without new controls or a permanent HUD. Broader skills, environmental opportunities and autonomous settlement growth remain future work.

Villager-initiated beachball invitations now communicate through gaze, beckoning and a deliberate rolling invitation. Shared chime call-and-response adds a second reciprocal creature/villager activity through existing gift ownership, utility selection, gestures and contextual feedback. Both activities preserve autonomy and saved social learning; missed invitations are not relationship penalties.

The next skill extension adds saved practiced block alignment and bounded boredom/curiosity/companionship, category satiation, urgent food reconsideration, and brief interest in visible player-placed objects. It reuses current motor/action ownership and training. Remaining skill work includes tool-specific capabilities and richer imitation; environmental challenges, traditions and independent settlement growth remain later milestones.

Tree play now extends physical-world curiosity: the creature can uproot, carry and throw existing tree models, remember outcomes and respond to player feedback. Villagers can fear uprooting and react to impact damage. This is a bounded object interaction, not a forestry economy or unrestricted destruction system; persistent trunks and navigation changes are included.

## Queued: autonomous resources, forestry and food security — 2026-10-25

Construction toys now share one larger yard with 48 pieces/eight shapes rather than three small trays. Old assemblies and remembered use migrate into the common space; the functional creek bridge stays separate. See [shared construction yard](CONSTRUCTION_YARD.md) for tested scope and migration details.

The weather/material foundation is now implemented; see [weather and material interactions](WEATHER_AND_MATERIALS.md). Next, extend villagers' independent decisions with a real resource loop. The wood/wheat gathering and storage foundation is now implemented; see [resource gathering](RESOURCE_GATHERING.md). Storage expansion and exposed-food spoilage are also implemented. Replanting depleted registered tree sites is now implemented; see REFORESTATION.md. Renewable living-tree resin gathering and storage are now implemented; see RESIN_GATHERING.md. New forest patches, resin recipes, growing/sowing crops, new farms, weather-driven food damage and seasons remain queued. Keep water secondary in development focus, preserve creature/villager play, and avoid replacing agency with permanent worker assignments or player command menus.

| Order | Capability | Playable outcome |
| --- | --- | --- |
| 1 | Shared inventories and physical stores | Wood/wheat stocks, carrying, reservations, visible covered stores and consumption are implemented. Resin inventory comes with its gathering milestone. Track actual wood, resin and food quantities, carrying capacity, reservations and consumption. Villagers carry visible bundles/baskets to reachable stockpiles. Start with wood and wheat rather than implementing every resource at once. |
| 2 | Forestry and reforestation | Villagers choose suitable mature trees, use cutting tools, produce finite wood and carry it home. They can also recover suitable fallen timber. Harvesting changes the existing tree/navigation state. Saplings grow into harvestable trees; villagers replant depleted areas and can establish new forest patches when wood is scarce. |
| 3 | Resin gathering | Suitable living trees offer a renewable, slowly replenished resin yield, distinct from cutting them down. Gatherers choose between preserving a resin source and harvesting timber according to need. Reserve resin uses such as sealing, repairs or later crafting for a subsequent recipe pass. |
| 4 | Farming and food variety | Wheat grows through planting, growth, ripening and harvesting rather than appearing from a completed work timer. Add a second food crop with a different growth/storage tradeoff after the wheat loop works. Villagers can prepare and plant new suitable farm plots when demand and available supplies justify expansion. |
| 5 | Food security and bad-weather preparation | Villagers move harvests into covered stores/granaries, eat from actual stock and build a reserve based on population needs. Exposed food spoils faster or is damaged by rain/flooding; covered storage reduces loss but is finite. Low reserves prompt more growing/gathering, while approaching bad weather prompts sheltering supplies and harvesting ripe crops. |
| 6 | Seasonal extension | Once storage and food consumption work, add seasons affecting crop growth, tree regeneration and reserve targets. Seasonal shortages should test preparations, rather than introduce a second disconnected economy. Full seasons remain future work. |

### First proving loop

A villager notices low wood or food, gathers a reachable resource, carries it to a store, and the village consumes or uses that stock. Another villager may replant a harvested tree or sow an eligible plot. When a storm approaches, villagers protect a harvest and later use stored food while outside work is unsafe. The creature can help move resources or protect a store, but the loop must also work without it or the player.

Villagers should act on needs, available knowledge, safety and opportunities. Storage availability, food reserves, wood shortages and planting opportunities compete with social/play desires; this is not a return to water-job dominance. Communicate needs through empty baskets, carried tools, gestures, visible crop stages and store fullness, keeping normal play largely text-free. Villagers should only prepare for forecastable weather or observable warning cues, not secretly read future random weather rolls.

### Integration and completion gates

- Reuse the existing action/carry ownership, stable tree identities, navigation updates, relationships, weather/material state and season persistence. Reserve trees, plots and store capacity so two actors cannot claim the same yield. Player-held objects remain protected.
- Save stocks, resource depletion/regrowth, planted plots, crop stages and bounded village needs. Pause and reload must not duplicate harvests or grant offline growth unexpectedly. Each harvested amount has one source and one destination; cancellation must release reservations without creating resources.
- Limit active forestry/farm sites and growth checks for Quest. Do not give every plant a Rigidbody or create an unlimited forest. New plots and saplings must respect reachable terrain, settlement access, existing structures and planting space.
- Test independent gathering, cutting/replanting, resin regeneration, sowing/harvesting, storage capacity, consumption/spoilage, weather damage, canceled jobs, simultaneous claims and save/resume. Include an unattended-village run showing progress and bounded resource use.
- Deliver in the order above, with small validated commits and pushes. The first implementation should prove wood/wheat collection and storage before expanding the crop catalog, resin recipes or seasons.


## Households and covered storage — 2026-10-25

Stores now expose physical capacity posts, uncovered overflow and up to three villager-built shelving expansions paid from timber. Exposed grain spoils; covered grain feeds adults and children. Existing peer relationships support shared-home visits, partnership and a bounded first family: up to three children inherit behavioral traits and stature, navigate near parents/home, and persist across reloads. Original three adult workers retain their existing roles. Full generational aging and additional freestanding storehouses remain later work. See HOUSEHOLDS_AND_STORAGE.md for tuning, boundaries and validation.


### Forestry renewal — 2026-10-25

Depleted wood sites can now be replanted by villagers and visibly grow back into harvestable trees, using saved source state and existing navigation/action ownership. This closes the first renewable timber loop within the sixteen registered sites. Renewable resin now follows this step; next in the queued resource sequence are planted/growing crops and weather preparation.


### Renewable resin — 2026-10-25

Villagers gather small resin reserves from living trees, preserving their timber while resin slowly replenishes. Shared tree reservations prevent cutting, tapping and creature pickup from competing for one tree. Amber cups/jars and a store fullness post communicate the resource without new menus. Resin recipes remain later work; planted and growing wheat is the next food-security milestone.
