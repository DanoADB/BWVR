# The Gathering Storm — vertical-slice backbone

The larger target is a 15–20 minute Quest experience: play builds a relationship, weather tests it, and the village remembers the outcome. This first increment connects a complete chapter sequence to one reusable, physical assistance loop. It is a roughly seven-minute prototype backbone, not the finished storm/fire/recovery release.

## Play

Open the Sanctuary, choose Settings, then Gathering Storm. Starting retains your island, learned preferences, population and stores. Historical saves remain in free exploration until you opt in. Choosing the entry again during a chapter returns to play without restarting it. End chapter returns to free play without erasing learning; Begin again remains the separate destructive new-season action.

- **Calm — 180 seconds:** existing ball games, chimes, tricks and the cooperative gate remain available for play and teaching.
- **Warning — 35 seconds:** the existing weather system brings wind, with a contextual gust cue.
- **Storm — 150 seconds:** rain begins. One healthy adult at their actual position becomes frightened and seeks a clear gathering point. They beckon and indicate its green arrival ring. A suitable point must be reachable for both actor sizes; no participant is teleported into danger.
- **Recovery — 45 seconds:** the weather clears. An unresolved participant can still reach safety. Confirmed help remains recorded.
- **Complete:** normal weather autonomy and free play resume. The Sanctuary summary retains the chapter phase and the source of any assistance.

Pick up the villager normally and release them inside the ring to help directly. Alternatively, offer the existing Help guidance to the villager and let the creature decide whether to assist. Prior safe social training, individual affinity and current needs influence that choice. The creature approaches, lifts, carries and gently places the villager using its existing movement, hand IK and animation system. It must actually reach safety before a social-action receipt becomes eligible for praise or discouragement.

After twenty simulation seconds without a creature/player carrier, the villager attempts their own evacuation through the existing exploration motor. The village can address this problem without the player. The chapter records Player, Creature or Independent arrival exactly once; direct player and independent arrivals do not create a creature-training receipt. Player placement while paused is remembered and confirmed on resume with its original attribution. Assistance reassures the villager; only confirmed creature assistance earns the creature-specific trust/bond change. It does not heal unrelated injuries or blame the creature for the storm.

## Ownership, persistence and scope

`StormChapterState` persists opt-in phase, phase time, episode, stable adult index, gathering point, last grounded actor position, pending player placement and assistance source. Existing `SliceState` remains the save format; missing chapter data means Dormant. Active carry ownership and animation are transient. A save during a carry retains the last safe grounded position; restore reacquires the participant after navigation is ready, clears temporary ownership and allows a fresh feasible action. Existing learning and village data remain intact. Native Save/Load captures the chapter alongside the other systems. Pause/suspend do not advance it, and no offline time is simulated.

`StormChapter` owns chapter/weather transitions. `StormRescue` extends the existing curiosity and exploration intentions, and CreatureHybrid remains the only creature action coordinator. One participant's ordinary jobs, family, worship and leisure reservations yield for the assistance opportunity. Other villagers remain autonomous. Navigation, pickup/drop, held-object protection, interruption, feedback and cleanup use the established systems. Matching Help guidance becomes obsolete if somebody else solves the problem. During a pending rescue it offers the actual assistance action, rather than a generic visit or a water job inferred from the participant's occupation. Ending the chapter safely releases any creature-held villager and restores weather autonomy without a success award.

This first storm uses wind and rain. Existing island weather/material systems still exist, but a new scenario-specific lightning/fire challenge, object retrieval objective, autonomous multi-person emergency response, recovery celebration and new Book chapter pages are later milestones. Water and food work remain deprioritized in development focus; the existing unlimited-energy test setting remains unchanged. There is no new energy economy, possession, AI service, compulsory HUD or camera movement.

## Build sequence

1. **V1: chapter and one complete assistance loop** — this increment: opt-in/end controls, phase/weather orchestration, player/creature/independent arrival, contextual training, grounded save/resume and outcome attribution.
2. **V2: play skills become useful** — displaced-object retrieval and safe delivery, reusing fetch/carry and learned preferences; readable requests and visible practical outcomes.
3. **V3: bounded environmental pressure** — one localized lightning/fire incident, direct rain/healing intervention where relevant, multiple feasible responses and autonomous villager reactions.
4. **V4: recovery and lasting consequences** — help/harm memories, social acknowledgment, return-to-play invitations and concise saved chapter evidence in the Book.
5. **V5: Quest release gate** — nonverbal onboarding, art/audio polish, comfort, physical controller playthrough, frame-time/memory profiling and a requested APK build. Tune toward the complete 15–20 minute experience using playtest evidence.

## Verification

Unity 6000.6.7f1 Editor validation covers old-save opt-in behavior, bounded phase transitions, pause, source/outcome validation, weather ownership, Sanctuary chapter entry, utility-selected creature navigation/carry/place, exactly-once social feedback, independent evacuation, native player pickup/drop, deferred paused placement, native pending-rescue Save/Load, safe ownership recovery and ending during a paused carry. The fixture stages a healthy adult near the creature, temporarily disables competing construction ideas, and moves other residents aside for reproducibility. Phase waits are shortened by advancing the saved phase clock near each boundary; actual assistance uses ordinary simulation ticks and utility selection. It does not establish a natural acceptance rate or guarantee the creature will always help.

Existing gameplay/menu/weather regressions and staged rendering are checked separately. Headset readability, controller feel, natural full-population behavior and device performance remain release gates. No APK is built for this increment. The original user save and XR startup settings are restored after testing.
