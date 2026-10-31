# A creature turn at your course

Build & Bowl can now continue from a returned ball into a creature turn. Rebuild
a piece where the creature can see it. That actual player placement can offer a
roll at the rebuilt piece. The creature chooses whether to join through its normal
utility model. You can instead take the ball and make your own next shot.

The creature uses its existing approach, inspect, lift, carry, gesture, roll and
watch behavior. One bounded carry phase takes it to a sampled clear approach
before the roll. No new object artwork, command menu, resource job or energy cost
is introduced. Course shape remains player-arranged using the existing snap rules.

## Rules and attribution

- A physical return during an active challenge arms a 75-simulation-second
  opportunity. A fresh, witnessed native player snap in the main yard must follow
  that return's placement cursor. Old work, creature placements and loading a save
  cannot offer a turn. A completed challenge offers no further creature turn.
- The approach search checks at most eight points, about 24 game units from the
  target. Existing navigation, held-object, occupied-yard and villager-corridor
  checks apply. Claiming the opportunity does not interrupt another action.
- The known roll executor can prefer this target without requiring three earlier
  invented-game discoveries. It still requires feasible aim; it cannot bypass
  physical safety. The separate action choice remains voluntary. A carry that
  loses its opportunity cancels instead of falling back to an unrelated throw.
- Only a direct hard PhysX contact that detaches that rebuilt piece is a hit.
  A safe miss records a creature attempt. A provisional shot reserves the ball and
  yard for the existing 12-second harm window. Pause freezes it. Harm, pickup,
  reset, wind, End, a changed attempt or replaced save invalidates the result and
  releases the reservation.
- Creature attempts/hits have separate saved counters. They never advance the
  player's two contributor-hit objectives, player-shot total or return count.
  At most one safe creature result records per returned player shot. Starting a
  new challenge clears that attempt's counters. Pending opportunities and flights
  are transient and do not resume after load.
- Actual creature play uses the existing completion, experience, invention and
  feedback systems, including their existing late-harm retraction. The project
  counter does not issue another action-completion receipt or training reward.
  Authored turn offers do not themselves count as discoveries.
- Book → Projects distinguishes your shots, physical returns and creature hits
  over creature attempts. You can rebuild a different stack, arch or short course
  between turns. This is one optional turn per returned shot, not a compulsory
  multi-round match or an automatic return after the creature's roll.

## Verification and limits

`BowlCreatureTurnChecks.VerifyModel` covers old saves, physical-return prerequisite,
duplicate result guards, separate player objectives, serialization and replay.
The native fixture chains a real safe player shot, utility-selected physical ball
return, native player pickup/snap, autonomous creature selection, lift/carry to the
approach, real roll/contact/detach, pause, 12-second result confirmation and native
completed Save/Load. A second run injects late harm through the native villager
impact API and withholds project credit. A cancellation run resets the course
during actual carry and checks physical release without a fallback roll or reward.

As in the preceding fixtures, unrelated loose pieces are isolated, villagers are
parked, competing affordances are constrained and a visible player-snap viewing
position is staged. The ball is held still after the genuine target collision to
isolate result confirmation. The harm is injected, not a naturally observed
full-population accident. Normal invented-game, fetch, shared-building and Book
regressions cover reused behavior. Natural participation, aim tuning, full-island
performance and physical Quest testing remain open gates. No APK is built here.

Next candidate: a clearer in-world handoff between player and creature turns,
followed by course variation and difficulty tuning from actual Quest playtests.
