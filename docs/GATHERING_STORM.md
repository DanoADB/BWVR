# The Gathering Storm — vertical-slice backbone

The larger target is a 15–20 minute Quest experience: play builds a relationship, weather tests it, and the village remembers the outcome. The first two increments connect a complete chapter sequence to physical villager assistance and useful timber retrieval. It is a roughly seven-minute prototype backbone, not the finished storm/fire/recovery release.

## Play

Open the Sanctuary, choose Settings, then Gathering Storm. Starting retains your island, learned preferences, population and stores. Historical saves remain in free exploration until you opt in. Choosing the entry again during a chapter returns to play without restarting it. End chapter returns to free play without erasing learning; Begin again remains the separate destructive new-season action.

- **Calm — 180 seconds:** existing ball games, chimes, tricks and the cooperative gate remain available for play and teaching.
- **Warning — 35 seconds:** the existing weather system brings wind, with a contextual gust cue.
- **Storm — 150 seconds:** rain begins. One healthy adult at their actual position becomes frightened and seeks a clear gathering point. They beckon and indicate its green arrival ring. A suitable point must be reachable for both actor sizes; no participant is teleported into danger.
- **Recovery — 45 seconds:** the weather clears. An unresolved participant can still reach safety. Confirmed help remains recorded.
- **Complete:** normal weather autonomy and free play resume. The Sanctuary summary retains the chapter phase and the source of any assistance.

Pick up the villager normally and release them inside the ring to help directly. Alternatively, offer the existing Help guidance to the villager and let the creature decide whether to assist. Prior safe social training, individual affinity and current needs influence that choice. The creature approaches, lifts, carries and gently places the villager using its existing movement, hand IK and animation system. It must actually reach safety before a social-action receipt becomes eligible for praise or discouragement.

After twenty simulation seconds without a creature/player carrier, the villager attempts their own evacuation through the existing exploration motor. The village can address this problem without the player. The chapter records Player, Creature or Independent arrival exactly once; direct player and independent arrivals do not create a creature-training receipt. Player placement while paused is remembered and confirmed on resume with its original attribution. Assistance reassures the villager; only confirmed creature assistance earns the creature-specific trust/bond change. It does not heal unrelated injuries or blame the creature for the storm.

## V2 — useful timber retrieval

During Warning, the chapter reveals one stranded timber bundle on reachable dry ground near the existing stores. It uses the imported Kenney Survival Kit `resource-wood` mesh, rather than primitive placeholder art. Its destination is the amber ring at the covered-store approach. The green ring still belongs to the separate frightened-villager opportunity.

Carry the bundle into the amber ring yourself, or place the existing Help cue on it and let the creature decide. The creature can also notice it without a cue. Existing fetch enjoyment contributes to desirability alongside contextual HelpRequested/RespondToGuidance learning; this is a new useful outcome of the established approach, lift, carry and gentle-place motor. It is not a guaranteed order. The matching action is a real retrieval, never a generic two-second inspection. Praise/discouragement reinforces the completed useful response, without incrementing beachball fetch statistics.

A confirmed delivery adds exactly six wood and one delivery to the existing ResourceState and immediately updates the visible timber pile. These six units are a chapter scenario supply, not deducted from an existing store; a restarted chapter may offer another bundle. No resources appear before physical delivery. The stores must have six units of free capacity; otherwise the bundle remains available, with no partial award, overflow or receipt. A player delivery is attributed to Player and cannot train the creature. Creature delivery is attributed to Creature. Nearby villagers acknowledge the arrival, but this increment does not add a separate independent villager courier.

Pending and delivered state, source, destination and last grounded bundle position save with the chapter. Saving in a carry restores a grounded, unclaimed pickup on load. Paused player delivery is deferred to resume. Stop, expiry, interruption and player-held protection release the existing action ownership without a success award. The completed supply cannot be picked up or credited again. An invalid/unreachable drop follows native placement recovery. There is no new binding, HUD or routine food/water assignment.

## Ownership, persistence and scope

`StormChapterState` persists opt-in phase, phase time, episode, stable adult index, gathering point, last grounded actor position, pending player placement and assistance source. Existing `SliceState` remains the save format; missing chapter data means Dormant. Active carry ownership and animation are transient. A save during a carry retains the last safe grounded position; restore reacquires the participant after navigation is ready, clears temporary ownership and allows a fresh feasible action. Existing learning and village data remain intact. Native Save/Load captures the chapter alongside the other systems. Pause/suspend do not advance it, and no offline time is simulated.

`StormChapter` owns chapter/weather transitions. `StormRescue` extends the existing curiosity and exploration intentions, and CreatureHybrid remains the only creature action coordinator. One participant's ordinary jobs, family, worship and leisure reservations yield for the assistance opportunity. Other villagers remain autonomous. Navigation, pickup/drop, held-object protection, interruption, feedback and cleanup use the established systems. Matching Help guidance becomes obsolete if somebody else solves the problem. During a pending rescue it offers the actual assistance action, rather than a generic visit or a water job inferred from the participant's occupation. Ending the chapter safely releases any creature-held villager and restores weather autonomy without a success award.

This first storm uses wind and rain. Existing island weather/material systems still exist, but a new scenario-specific lightning/fire challenge, independent supply courier, autonomous multi-person emergency response, recovery celebration and new Book chapter pages are later milestones. Water and food work remain deprioritized in development focus; the existing unlimited-energy test setting remains unchanged. There is no new energy economy, possession, AI service, compulsory HUD or camera movement.

## Build sequence

1. **V1: chapter and one complete assistance loop** — this increment: opt-in/end controls, phase/weather orchestration, player/creature/independent arrival, contextual training, grounded save/resume and outcome attribution.
2. **V2: play skills become useful** — Editor-verified timber retrieval, physical player/creature delivery, visible six-wood stock increase, contextual teaching, capacity protection and grounded save/resume. Natural discovery/readability still needs Quest playtesting.
3. **V3: bounded environmental pressure** — one localized lightning/fire incident, direct rain/healing intervention where relevant, multiple feasible responses and autonomous villager reactions.
4. **V4: recovery and lasting consequences** — help/harm memories, social acknowledgment, return-to-play invitations and concise saved chapter evidence in the Book.
5. **V5: Quest release gate** — nonverbal onboarding, art/audio polish, comfort, physical controller playthrough, frame-time/memory profiling and a requested APK build. Tune toward the complete 15–20 minute experience using playtest evidence.

## Verification

Unity 6000.6.7f1 Editor validation covers old-save opt-in behavior, bounded phase transitions, pause, source/outcome validation, weather ownership, Sanctuary chapter entry, utility-selected creature navigation/carry/place, exactly-once social feedback, independent evacuation, native player pickup/drop, deferred paused placement, native pending-rescue Save/Load, safe ownership recovery and ending during a paused carry. The fixture stages a healthy adult near the creature, temporarily disables competing construction ideas, and moves other residents aside for reproducibility. Phase waits are shortened by advancing the saved phase clock near each boundary; actual assistance uses ordinary simulation ticks and utility selection. It does not establish a natural acceptance rate or guarantee the creature will always help.

Existing gameplay/menu/weather regressions and staged rendering are checked separately. Headset readability, controller feel, natural full-population behavior and device performance remain release gates. No APK is built for this increment. The original user save and XR startup settings are restored after testing.

V2 Editor checks additionally exercise warning-time spawning, real utility selection and carry/place, native player pickup/drop, full stores, exactly-once rewards, pause/source isolation, contextual feedback, native pending-carry Save/Load and stop cleanup. Existing chapter, hybrid, toy, menu and weather checks remain regression gates.

V2 staged rendering confirms the sourced bundle is visible in the existing ape carry attachment. The inherited carry pose still needs animation/art polish; this does not establish headset quality. Broad hybrid regression uses a reachable staged investigation toy, avoiding dependence on a ball moved by a preceding live session.
