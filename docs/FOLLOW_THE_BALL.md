# Follow the ball

A new player–creature game uses the existing beachball and carry controls. Pick up the ball with the right front trigger (desktop E), hold it near the creature's head/ground level in front of it, and move it slowly away or from side to side. A brief curious response acknowledges a visible invitation. The creature can choose to follow and show excitement; it never takes the ball from your hand. Release near it after several seconds of moving together to finish the bout. Existing A/B feedback (desktop 1/2) teaches its preference for this game.

The creature retains autonomy. Hunger, critical water pressure outside exploration focus, other opportunities and existing tasks can take precedence. Water and food development priorities are unchanged. This adds a physical play opportunity, not a guaranteed follow command.

## Rules and boundaries

- The held ball must belong to the player, be visible/in front within 45 game units, and be within 28 game units of ground. About three quarters of a second of visible presentation creates an opportunity.
- The existing utility selector chooses the opportunity. The existing curiosity executor owns locomotion, using IslandNavigator's pathfinding, ground projection and actor avoidance. It keeps a stand-off distance from the held ball and never owns the player's carry item.
- A successful bout requires at least three seconds, eight game units of actual creature travel and six units of ball movement. Release must remain nearby and recently visible. Merely holding still, canceled/unreachable attempts, rapid tracking-size jumps, timeouts and distant releases grant no completion. A ten-second cooldown limits repetition.
- Sampling/visibility and target updates run at approximately 4 Hz; per-frame navigation uses the cached goal. Existing pause/suspend and controller ownership apply. Tracking loss or menu ownership blocks the game. A bout lasts at most 25 seconds.
- Slow release still places the ball; a real toss still uses the existing fetch/bowling routing. A subsequent activity has a separate receipt. Chase completions carry a typed flag so teaching cannot silently become bowling/fetch/rhythm teaching.
- Completed count, enjoyment, preference and best distance are bounded and saved. Old saves are accepted. Active invitations and movement samples are transient. Saved chase intentions require a currently feasible chase opportunity to resume; an unheld ball after reload cannot masquerade as the same chase.

`BallChase` is a small service beside the ball's existing bowling/fetch services. It has no Update loop, physics scene or independent motor. Existing animations, affect icons, notice audio and coordinator ownership are reused. No new artwork, UI, jobs or controller bindings.

## Verification

`BallChaseChecks.VerifyModel/Verify` passed in the Editor: historical saves, bounded/invalid memory, exclusive typed receipts, native pickup, unseen exclusion, visible invitation, utility-selected grounded following, player carry protection, pause, native slow release, exactly-once completion/feedback, save round trip, interruption and stationary-bait rejection. The staged creature traveled **18.84 game units**. Other affordances were cooled down and starting needs/position were staged to isolate this activity; this is not a claim about unattended choice frequency or subjective fun.

The final sequence also passed existing native ball placement, player fetch, bowling model/physical imitation, and sanctuary checks. Unity compilation has no errors/warnings. One initial model fixture incorrectly assumed Unity would leave an absent nested serializable class null; Unity initializes its defaults, so the historical-save assertion now checks valid empty history instead.

Physical Quest input feel, wrist-scale reach, animation appearance and performance remain unverified. No APK was built. The user's original saved season and XR startup settings are restored after testing.
