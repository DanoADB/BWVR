# Together gate: a cooperative toy puzzle

The player provides a blue key toy; the creature can choose to hold a paw pad. Both inputs raise a shutter so the player can roll or toss the beachball through. The shared result earns a discovery sound, green jewel, remembered enjoyment and an ordinary contextual teaching opportunity. No new buttons, menu, energy cost or guaranteed creature order are introduced.

## Play

1. Find the stone gate with a gold shutter, blue socket and gold paw pad near the beachball play area. The builder searches for a clear, level site outside the bowling lane and portable goals.
2. Pick up the blue sphere using the normal carry control. Release it onto the matching blue socket; a nearby grounded release snaps into place. A held key does not power the puzzle. The inset blue/gold tracks show which inputs connect to the shutter.
3. Let the creature notice the pad and choose to help. It walks there using its normal navigator, keeps its own hands free and holds the shutter open by staying on the pad. If it is looking away or busy, it can ignore the opportunity.
4. Roll or toss the beachball through from the socket/pad side. The whole sphere must fit through the opening. The existing hand-motion throw works in VR; desktop throwing retains the existing Space modifier. The backstop catches the ball behind the gate.
5. A safe passage followed by one second without harm completes the puzzle. Praise or discourage the creature using the existing feedback controls within 12 seconds. Remove the key, wait for the short cooldown and replace it to offer another round. Repeated completions gradually increase remembered enjoyment; player feedback also changes the established context/target preferences.

The creature can wait up to 70 simulation seconds, then leave. Key removal, creature pickup, suspension/tracking/input loss, a competing shared-goal invitation, harmful play or interruption ends assistance safely. The key stays with its player carrier, and the shutter closes. Picking up or placing the beachball clears passage tracking without canceling the helper. Carrying a ball through, teleporting it across, an unpowered gate and repeated notifications cannot earn a completion.

## Integration

`CooperativePuzzle` shares the beachball's game-unit PhysX scene, substeps and pause clock. Four fixed collision bodies form piers, a sill and backstop; a fifth kinematic body follows the visible shutter. Primitive geometry and four shared materials supply this first toy-art pass without new asset dependencies. The frame and backstop join IslandNavigation's existing obstacle-source/update pipeline; actors route around the gate even while its shutter is raised for the ball. If no safe site exists, the puzzle stays unavailable and reports a warning.

The fixed `PuzzlePad` is a registered play affordance. It exposes a utility candidate only when its key is grounded, visible, the site is available and no conflicting ball activity owns the toy. `CreaturePuzzle` adapts the existing curiosity action and navigator; no second motor, action owner or carry controller is added. Active assistance excludes chase/spin, villager ball reservations and shared goal games. Completion uses the existing PlayWithToy receipt with target `together-gate-pad`, never a fetch, spin or goal receipt. Feedback retains the existing once-only, pause and carry protections.

`WorldPlayMemory.Puzzle` persists bounded attempts, solved count and enjoyment. Historical saves have no invented completions. The fixed puzzle rebuilds at its deterministic site; key position, partial passage, current attempt and shutter state are transient and reset on load/new season. A loaded partial intention must meet the normal opportunity checks again. Successful history remains, while the next play session begins with the key beside the socket.

## Verification — 2026-10-28

Unity 6000.6.7f1 compiles without errors/warnings. The repeatable Editor fixture stages the creature near the puzzle, temporarily disables competing construction demonstrations and moves residents aside. It uses native key pickup/release and beachball placement, actual player-toss physics, ordinary utility selection, navigation and the normal completion/feedback path. It verifies closed-shutter collision, pad approach/hold, pause, a safe shared passage, exactly-once reward/teaching, save validity, carried/teleported ball exclusion, conflicting ball-game rejection, key-removal closure and interruption cleanup. It also checks that navigation blocks the frame while the helper can reach the pad. Pure model checks cover historical saves and invalid/bounded memory values.

Existing spin/training, trick audience, chime audience, goal-turn, pickup/drop, chase and sanctuary checks also pass. Normal Editor frames advance and a staged render was visually reviewed. This establishes a reproducible playable interaction, not a measured natural acceptance rate. Physical Quest throws, comfort, discoverability, art readability and frame-time performance still need headset playtesting. No APK was requested or built. The original user save and XR startup settings are restored after verification.

## Nonverbal cues — 2026-10-28

The empty socket slowly pulses blue. Docking the key highlights the matching paw pad; small colored beads travel along each powered input track. Once the creature is on the pad and the shutter is fully raised, a launch circle and forward chevron appear, one ready chime plays, and the creature points toward the launch spot while looking at the ball. The cue does not promise acceptance before the creature has chosen and arrived. The throw markers disappear after completion or cancellation. The jewel retains the confirmed solved state until rearming.

`PuzzleCues` reads the existing authoritative state and simulation clock. It adds two short line renderers and two small visual beads, reuses existing materials, adds no colliders or lights and cannot grant resources, learning or completion. Sound is transition-driven with a cooldown; reset/load does not play a ready sound. Pause freezes cue motion and the action. The creature's existing visual component owns the pointing gesture; normal action cleanup releases it.

The Editor fixture now also checks all logical cue stages, readiness/gesture/launch-marker agreement, one ready chime, pause, solved cleanup and key-removal reset. A ready-state render is inspected separately from the physical gameplay assertions. Headset readability and a new player's ability to infer the puzzle without instructions remain unverified.

Next playtest focus: can a new player infer both inputs and complete a round without reading instructions? Tune the pad's acknowledgment, wait time and ball approach cues from that evidence before adding more puzzle types.
