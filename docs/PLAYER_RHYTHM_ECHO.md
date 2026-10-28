# Player rhythm echoes

The existing village chime is now playable. Point at either colored bar and press/release the **right front trigger** (desktop **F**). Blue is the lower note; amber is higher. The four beads show a short pattern. Play four notes while the creature watches; it may choose to approach and echo their order and timing with the existing touch animation, sound and excited affect. Praise/discourage the finished echo with A/B (desktop 1/2). Aim at the wooden body rather than the bars to pick up the instrument with the usual carry controls.

No new controller buttons, text command list or permanent HUD. Notes sound immediately even when the creature cannot see them, but unwitnessed notes do not teach or invite an echo. Nearby notes still require line of sight; the close-range check permits seeing the instrument at the creature's feet.

## Behavior and ownership

A pattern has exactly four low/high notes and three intervals clamped to 0.2–1.2 seconds. A gap longer than 2.5 seconds starts a new phrase. A complete witnessed pattern offers a 25-second opportunity. The normal utility selector competes with other needs; it does not guarantee a response. A new phrase supersedes an unclaimed old phrase. A claimed performance is protected from further taps.

The existing curiosity executor owns the approach and animated playback. The chime stays on the ground, with no creature carry reservation. Player recording and playback reserve the instrument against simultaneous villager sharing. Villager-led duets keep their existing controller and memory; their completion is separate from a player demonstration.

Holding a trigger does not repeat notes. Untracked input, menus, tools, carrying and pause prevent player notes. Pause freezes playback. Pickup, interruption, expiration or target loss cancels the attempt without completion credit. State transitions release the reservation and touch pose. New seasons reset input deadlines. Saved echo intentions cannot resume as ordinary chime play after reload.

Completed echoes have an exclusive typed receipt. Existing once-per-event feedback changes the bounded echo preference used in future utility scoring. Saves contain bounded demonstration/completion counts, preference and the last successfully echoed four-note pattern/timing. In-progress recording/playback is transient; historical saves remain accepted. These are explicit, coded musical behaviors, not evidence of general intelligence or discoveries of uncoded actions.

## Implementation and validation

`PlayerRhythm` reuses the chime's existing meshes, clip, shared audio system, animations and creature coordinator. Two note triggers and four small beads supply the affordance. Per-frame presentation uses a cached instrument array and shared materials/property blocks; there is no scene search or new physics scene. The camera, menus and existing bindings remain intact.

`PlayerRhythmChecks.VerifyModel/Verify` passed in a fresh Editor Play session: native physical key hits, tracked press edges, held-input/menu/pause protection, unseen-performance rejection, four witnessed notes, utility-selected approach, exact note order and timing within 0.11 seconds in the fixture, paused playback, no instrument carry ownership, distinct one-shot learning/save, duplicate protection and pickup cancellation. Other opportunities and starting positions/needs are staged; this does not measure normal unattended choice frequency. A cancellation fixture explicitly restores the creature's attention before demonstrating a second pattern.

Existing villager duet/play-invitation, ball chase, sanctuary, native beachball pickup/drop, fetch and bowling-model checks also passed. The actual chime render was inspected for visible bars and four beads. The initial material-property-block setup error was fixed by initializing it in Awake, and verification restarted from a fresh session. Compilation reports zero errors/warnings.

Physical Quest aiming, musical feel, animation alignment, audio audition and performance remain unverified. No APK was built. The user's original season and XR startup settings are restored after testing.
