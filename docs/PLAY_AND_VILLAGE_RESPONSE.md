# Play, preferences and village response

Water remains deprioritized through the existing Exploration Focus default. This increment reduces general construction need/novelty scores and removes the automatic partial-building utility bonus. A specific villager building request can still supply a temporary bias. There is no permanent building command or mandatory water routine.

## Favorite objects and behaviors

Existing contextual behavior learning now works alongside up to 128 stable object preferences. Confirmed toy, tool, eating and construction experiences give a small bounded preference change; praise/discouragement uses the same eligible completion receipt and changes both contextual behavior and that specific target. Recently used objects receive a temporary repetition penalty. Learned appeal changes utility, not feasibility or guaranteed commands. Individual preferences, sample counts and last-use simulation times save with the creature.

## Large beachball

The seven-game-unit striped ball is approximately half the creature's height and too large for villagers' personal gift routine. It replaces the small blue rolling sphere. Its striped texture is generated locally; no external artwork is downloaded. The creature uses existing reach/carry poses, winds up, releases a thrown ball and watches the result.

A dedicated game-unit PhysX scene provides gravity, rotation and rubber-like bouncing without changing global gravity or tiny tabletop collision tolerances. The ground collider follows the island mesh. A pool of 64 nearby contact proxies represents houses, tree trunks and villagers; target discovery is cached and contact selection refreshes four times per simulation second. This is a bounded toy physics system, not a second general island simulation. Pause freezes it. Water/out-of-bounds recovery returns the ball safely to its home. In-flight poses are transient and restart at home on reload.

Strong contacts (at least nine game units/sec) damage each target at most once per throw. Repeated strong hits can visibly collapse a house or topple a tree; villagers struck by the ball acquire the existing injury condition. These are prototype deformations and injury, not mesh fracture, ragdolls or villager death. Damage uses stable placement IDs and persists. Existing navigation footprints stay conservative around rubble. Nearby witnesses recoil, point at the ball and lose trust/affinity toward the creature only when its throw caused the harm. Player throws do not receive creature blame; distant villagers do not automatically witness incidents.

Safe throws increase remembered throwing enjoyment and object appeal. Harmful throws increase caution and reduce appeal; higher caution makes the creature compare throw directions for nearby collision risks. A late harmful contact within the throw's bounded window can revise an initially safe outcome. This is local risk estimation and outcome memory, not arbitrary prediction of the full physics simulation.

## Construction use and visible needs

Villagers can reserve a completed non-bridge structure and approach it through existing navigation. They pass through arches, gather/rest beside bench-like builds, and inspect towers. The bench behavior does not claim a seated animation; the current toy bricks make a tall gathering landmark. Completed bridges retain their existing actual navigation crossing behavior.

Visits reduce a small saved rest/gathering need. The first use of each form by each villager can improve that villager's relationship with the creature and the creature's form preference, only when the attached structure was creature-built. Repeat visits cannot farm that trust reward. Player-built structures remain usable. Taking away a support cancels an active visit and prompts a pointing reaction; a visitor lease prevents autonomous block disassembly during use, while player manipulation remains available.

When the need is high and no suitable gathering place is available, a villager can approach an empty tray or unfinished bench, point between a real bench example and the site, and beckon toward the creature. This raises observation/construction utility and can guide an empty tray toward the learned bench form. It does not take over the creature's movement or replace player guidance. Requests expire, have a saved backoff, and release site ownership on interruption. No water requests are added.

## Communal tributes

A communal fruit offering is separate from personal toy/berry gifts. A strict majority of residents must individually have extreme sentiment: trust at least 0.8 for affection, or at most 0.2 for fear. With three villagers, at least two must qualify in the same direction. An extreme minority, neutral majority or village-wide average alone cannot trigger it.

The village spends eight crops to prepare a large seven-fruit offering, provided at least twelve crops are available. An available villager fetches and carries it to the offering place through the existing exploration controller. The creature can choose to eat it once delivered. Affection and fear produce distinct presentation gestures/tints and separate saved counters; eating one does not count as an individual's personal gift or automatically make fear become love. One pending/delivered offering and a 180-second simulation cooldown prevent mood-based duplication. Pending and uneaten offerings restore safely, without restoring an unsafe carried pose. There is no new command panel, controller binding or worship toward the invisible player.

## Verification

Editor checks cover favorite-object choice effects, native ball contacts and creature throw execution, damage/injury/blame, strict-majority tribute gating, native communal delivery/consumption, construction use and need gestures, interruption, pause, deduplication and save validation. Focused controller fixtures may stage initial actor/prop positions; movement and collision assertions use the native systems. Quest frame time, haptics, readability and comfort still require the deferred headset playtest. No APK is built for this increment.

Creature contact with the beachball now produces physical kicks. Controlled taps earn a small enjoyment reward with a 1.2 simulation-second cooldown; powerful kicks earn a larger reward with a seven-second cooldown. Rewards follow the physical outcome; destructive contact retains the existing caution and village reactions. Held balls, intentional pickup sequences, pause, stationary overlap and teleport-like movement do not earn kick rewards. Kick cooldown and counts persist with the season. Kicks share the existing physical-play outcome counters with throws.

## Gentle ball rallies

Villagers independently choose the beachball during leisure, approach it on the island navigation mesh, and nudge it along a clear lane. They beckon toward the creature while waiting. One villager reserves the ball at a time; normal urgent work, fear, injury, player carrying, and existing social intentions retain their arbitration.

A villager's invitation makes a gentle rally an available creature decision. It competes with other needs and opportunities rather than issuing a command. If the creature chooses it, both actors use their existing navigators to approach behind the ball and alternate short, physical rolling passes. A rally ends after four receptions or a bounded timeout. Normal throwing and incidental running kicks remain available outside rallies.

An alternating return counts only after the ball physically travels at least four game units and reaches the other participant slowly. Completed back-and-forth exchanges increase the creature's saved enjoyment of gentle rallies, its preference for that behavior and toy, and the participant's bounded social affinity. Solo nudges and stationary contact cannot produce shared-play rewards. These encounters are separate from gifts and communal tributes.

Player grabs, unavailable or injured participants, blocked routes, destructive impacts, and session timeouts release ownership. Pause freezes progress. Saves preserve learned preferences and counts; a reload safely starts a new physical encounter instead of restoring a half-completed pass. Existing fast-impact damage and source attribution remain active.

Validation for gentle rallies: fixed-step Play-mode checks cover native approaches and four physical receptions, independent solo nudges, pause, exclusive ball reservations, player-grab cancellation, bounded social/favorite learning, and save validation. A separate 180-second hands-off simulation from the initial village produced one complete four-reception rally, three learned returns, two creature kicks and three villager nudges, with no damage. Existing beachball, construction-use, tribute, development-focus and controller checks also passed. The development-focus check required an explicit app-resume event because this Editor session began suspended. Headset visuals and performance remain unverified.
