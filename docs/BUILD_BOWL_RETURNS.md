# Build & Bowl: bring it back

After a safe shot settles, the creature can choose to fetch the beachball and
bring it back near the player's shooting spot. This extends the existing
utility-selected approach → inspect → lift → carry → place motor. It is a physical
return turn, not another thrown shot or a guaranteed command.

The familiar return circle marks a clear drop spot. The player can rebuild the
course while waiting, pick up the ball themselves, or leave the invitation alone.
Book → Projects records actual returns separately from safe shots and completions.
No new buttons, models, resource requirements or energy costs are introduced.

## Boundaries

- Only a confirmed safe Build & Bowl shot offers this return. Both active and
  just-completed challenges are eligible. A miss can still invite retrieval.
- A bounded 17-point search samples near the shot origin, within 12 game units
  plus navigation adjustment. It checks navigable ground, snapped/carried blocks
  and villagers, and respects the yard's raised floor. If no clear spot exists,
  the shot remains recorded without a return invitation.
- The invitation uses the existing 90-simulation-second fetch ticket and normal
  utility scoring. It neither interrupts another action nor guarantees selection.
  Claiming checks that the destination is still clear and reachable. Placement
  rechecks clearance, physical carrying, the native placement phase and arrival.
- Picking up or rethrowing the ball, wind/reset, ending the project, a new attempt,
  replaced save state, expiry or motor interruption invalidates the return through
  the existing fetch cleanup. Pause freezes the clock and prevents claiming or
  completing the custom return. A stale ticket cannot complete a newer fetch.
- Only physical placement records one return for that shot and increments the
  existing fetch-return total. Existing fetched-action feedback remains available.
  The player's shot and the return offer do not produce a creature lesson.
- Confirmed return counts and the last returned shot persist. Pending destination,
  ticket and ownership remain transient and are discarded on load. Starting a new
  challenge resets that attempt's return count while preserving completion history.
- Ordinary fetch still returns to its play circle; shared goal returns retain
  their existing destination and rules.

## Verification

`BuildBowlReturnChecks.VerifyModel` checks old/default saves, actual-shot and
duplicate guards, final-shot returns, saved bounds, End and replay.
`BuildBowlReturnChecks.VerifyRuntime` extends the native Build & Bowl fixture:
an actual safe player collision offers the return, the existing chooser selects
it, and the real lift/carry/place motor returns the ball to the local destination.
It checks pause, stale season ownership, absence of free completion, one fetched
lesson, one-shot feedback, serialized state and native completed Save/Load.

The fixture parks villagers, isolates competing affordances, stages a reachable
creature approach and settles the ball after its genuine shot collision. The
stale-season guard is tested by temporarily replacing the state reference. These
are controlled Editor checks, not natural full-population or Quest playtests.
Normal player-fetch and Book layout/input regressions cover the reused systems.
Headset comfort, spontaneous participation and performance remain deferred. No
APK is built for this increment.

Next candidate: a visible request to rebuild a course after a return, reusing
existing shared-building invitations and preserving the creature's choice.
