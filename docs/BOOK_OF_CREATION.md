# Book of Creation

Design requirement recorded 2026-10-25. This document specifies a planned interface; it does not claim a working book or discovery detector.

## Purpose and presentation

An optional physical book lets the unseen player inspect the island's history without turning normal play into a management dashboard. Open it from the existing sanctuary interaction, closed by default. Use illustrated tabs, portraits, object silhouettes and short captions, with detailed numbers available on demand. Keep the world visible, use comfortable adjustable book placement, and never move the player's head or repurpose occupied pickup/teaching controls. Opening the book does not silently pause the simulation; retain an explicit pause action.

The book observes and explains. It does not issue villager orders or overwrite creature learning.

## Chapters

| Chapter | Contents |
| --- | --- |
| Creature | Current mood, needs, practiced skills, learned contextual preferences, favorite toys/tools/activities, individual relationships and recent changes. Distinguish a practiced capability from willingness to use it. |
| People | A portrait/profile for every resident, including children: stable identity, household/parents, relationships, temperament, injury, current activity and genuinely measured skills. Show unavailable skills as unknown, not fabricated ratings. |
| Creations | Existing structures and inventions, builders/contributors, materials used, purpose observed, use/repair history and condition. Separate completed creations from plans, failed attempts and dismantled work. |
| Provisions | Food, wood and resin; covered capacity, exposed food, spoilage, harvest/delivery/consumption and material spending. Distinguish current stock from lifetime totals, reservations and in-transit loads. Future fuel/housing costs appear only when implemented. |
| Sentiment | Creature feelings toward individual villagers/player interactions, villagers' individual trust/fear/affection toward the creature, and a clearly labeled village summary. Keep fear and affection separate; show mixed opinions rather than collapsing them into one morality score. |
| Discoveries | Evidence-backed patterns, interests, creative uses and social habits that arose during this particular playthrough, plus how those patterns changed or spread. |

Use the existing simulation's actual state and confirmed events. The book must not invent biographies, skill values, emotions, creation counts or historical totals to fill empty pages. Missing historical evidence starts accumulating after instrumentation is introduced.

## What a discovery means

All behavior still runs on implemented mechanics. The book can identify unplanned combinations and learned patterns within those mechanics; it cannot prove that an event was never anticipated by a developer or that an agent invented executable behavior.

Examples of eligible observations include a persistent preference for a particular toy, an independently repeated social ritual, a novel useful arrangement of known construction pieces, or a learned activity spreading between individuals. A one-off random action, authored recipe completion or visiting a predefined discovery clearing is not by itself emergent behavior.

Use three evidence levels:

- **Observation:** a confirmed event with participants, target, simulation time and outcome; no claim of an established habit.
- **Developing pattern:** repeated comparable observations across separate opportunities, with competing choices available and no duplicate rewards from one action.
- **Established discovery:** sustained evidence of preference, useful creation or social transmission; retain examples and the rule/version that recognized it. This label expresses evidence, not certainty about intent or causation.

Detection should compare actual choices with available opportunities, distinguish player-directed encounters from autonomous ones, and account for interruptions, infeasible alternatives and authored routines. Do not infer a favorite solely from forced exposure. Record who originated, witnessed and later independently repeated a behavior before claiming imitation or a tradition. Repeated success can support a useful construction; geometry alone cannot prove purpose.

Show concise descriptions such as 'Often chooses the ball after a shared rest' with an optional evidence page. Avoid 'invented friendship' or claims of uncoded intelligence. Allow patterns to weaken, change or be superseded without deleting their history. A discovery grants no automatic reinforcement or material reward; observation must not alter the phenomenon being measured.

## Integration and persistence

Reuse the existing sanctuary, stable actor/object/site identities, SliceState, creature learning/affect, social memories, construction provenance and resource accounting. First audit available fields and confirmed completion events; map each displayed value to its source. Keep UI read-only over simulation state and refresh visible pages at a bounded cadence rather than scene-scanning each frame.

The existing transient journal cannot supply a permanent history. Add a bounded saved discovery ledger and aggregate counters fed by confirmed events, retaining only a few evidence examples per pattern. Use stable event identities/receipts to avoid counting reloads, repeated polling, canceled actions or held inputs as new accomplishments. Preserve old saves with empty optional book history. Respect pause, simulation time and new-season resets; never infer offline activity.

Limit profiles, creation records, patterns and evidence to explicit configured caps. Keep discovery recognition separate from action execution and training, without adding another AI framework or an external language-model dependency. Any optional narrative captions must be grounded in recorded evidence.

## Delivery sequence and validation

1. Build the physical book shell and read-only Creature, People and Provisions pages from existing state, including child profiles and missing-data states. Audit controller bindings before adding input.
2. Add attributed creation records and the separate individual/village sentiment views. Validate counts against the simulation and define what each summary includes.
3. Add a bounded saved evidence ledger, then detectors for toy preferences, shared social habits and useful construction outcomes. Keep predefined discoveries visually distinct from observed patterns.
4. Add recognition of independent imitation and changing traditions only when event evidence supports it.

Validate live updates, old/new save round-trips, pause, new-season reset, exactly-once counts, missing/deleted targets, record caps and mixed sentiment. Test discovery false positives using forced repetition, canceled actions, duplicate events and insufficient opportunities, as well as positive independent-choice cases. Verify Quest readability, comfortable placement, hand interaction, closing/reopening and frame cost on hardware before calling the VR interface complete.

## Planned summonable-tool entry

The underside-left-wrist palette is a proposed additional entry to summon the
physical book, alongside the sanctuary. The Book remains read-only and closed by
default; opening it does not implicitly pause. Coordinate selection, placement,
closing and controller ownership with [summonable tools](SUMMONABLE_TOOLS.md).
Neither the palette integration nor the Book shell is implemented by this plan.
