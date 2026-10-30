# Build & Bowl

An optional player-led construction challenge, using the existing sourced blocks,
shared-building invitations and beachball physics. No wood, food or water gathering
is required. The creature still chooses whether to join a build.

## Playing

1. Open Book of Creation → Projects. Turn past Village gateway and its guide to
   **Build & Bowl** (page 3 of 4), then choose **Begin project**.
2. Snap a piece in the main yard where the creature can see you. It can add to a
   known form through the normal utility chooser. Tap your recorded placed block
   with the right front trigger to renew its invitation; hold to pick up. Desktop
   F invites, E picks up. Only fresh placements after Begin count.
3. Arrange a shared stack or course. Toss the beachball at the snapped pieces.
   A sufficiently hard direct collision must actually detach a player's piece and
   a creature's piece. These may be separate shots: rebuild between attempts.
4. Leave the ball alone for its 12-simulation-second safety window. Book progress
   records safe shots and the two confirmed source hits. Discovery audio marks
   successful hits; completion also gives a brief happy creature response.

Begin, End and Build again preserve the world layout. Gateway and Build & Bowl
cannot be active together in the same yard. End the current project before
switching. The optional Mark yard button reuses the private activity beacon.

## Evidence and ownership

- A bounded pair of 48-bit masks records new player/creature placement sources in
  the main yard. Detached/held pieces lose current construction credit. Villager
  replacements do not become player/creature work. Reading the Book never creates
  history. Existing placement receipts and native save geometry are reused.
- Only `PlayerToss` can begin a scoring shot. It snapshots credited snapped pieces
  in a free yard. `ConstructionContact` supplies evidence only from an actual hard
  ball contact that detaches that piece. Manually taking a build apart, creature or
  villager throws, ordinary environmental pushes and chain-detached supports do
  not score.
- Pending shots briefly reserve the ball and yard against new autonomous ball
  play, construction, block disassembly, repair and structure visits. Existing
  owners are never interrupted to start a shot. Player pickup remains available
  and cancels the pending result.
- A harmful impact anywhere in the existing 12-second throw window discards the
  shot. Pickup, another throw, wind impulse, return/reset, changed yard ownership,
  End or replacement of the saved season also discards it. Pause freezes physics
  and the window. Confirmed progress persists; in-flight shots are discarded on
  reload. Each safe shot settles once; each attempt completes at most once.
- Player shots create no creature action-completion receipt, taught preference,
  experience reward or creature throw statistic. The optional journal identifies
  the player outcome. This challenge is a designed activity, not an emergent-game
  discovery or a new physical motor.

## Verification and limits

`BuildBowlChecks.VerifyModel` checks legacy saves, pure Book reads, four-page
navigation, fresh source baselines, disjoint/detached credit, pause/unsafe/nonfinite
guards, two-shot completion and serialization bounds.

`BuildBowlChecks.VerifyRuntime` stages native Book Begin/Mark, a native player
pickup/snap and a utility-selected creature placement. Actual `PlayerToss` PhysX
contacts detach the two builders' pieces. It exercises provisional results,
pause, an injected late native harm event, pickup/wind/second-throw/ownership/reload
cancellation, two safe hits, absence of creature teaching from player shots,
partial/completed native Save/Load, and non-destructive replay/end.

The fixture isolates unrelated loose pieces, parks actors away after their native
contributions, and holds the ball still after the real collision to isolate the
safety window. The harm event is injected through the existing impact API; it is
not evidence of a naturally occurring impact during a full-island playtest.
Existing knockdown, shared-building attention, gateway and Book regressions cover
the reused motors and UI layout. Quest input feel, natural full-population
participation and performance remain deferred to the user's headset playtest.
No APK is built for this increment.

Next candidate: a creature return turn after a player shot, preserving native
fetch, safe aim, source attribution and the creature's choice to participate.
