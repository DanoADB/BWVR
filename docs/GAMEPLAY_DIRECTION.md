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
