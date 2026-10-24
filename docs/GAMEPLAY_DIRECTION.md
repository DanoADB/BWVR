# Gameplay direction

Recorded from the user's design notes on 2026-10-23. These are future gameplay requirements, not a description of implemented systems. They guide later changes to the first slice.

## Core direction

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
