# Let's rebuild

After physically returning the beachball during an active Build & Bowl challenge,
an idle creature can point toward a usable loose recipe block. The existing light
guide marks that piece and a feasible snap position. Pick it up and place it to
resume the ordinary shared-building loop. There are no new controls or chores.

This is a nonverbal opportunity, not another command, construction motor, saved
goal or reward. It does not increase building utility or automatically place a
piece. Normal creature decisions continue and may end the invitation immediately.

- Only a confirmed physical return can arm it, once per returned shot. Completed
  challenges do not request more work.
- The transient offer lasts at most 30 simulation seconds. It checks the same
  project, attempt, shot and placement cursor; old saves do not revive it.
- A registered visible loose recipe piece must fit a known unfinished form in the
  main yard. The yard must be free of held pieces, repairs, visitors and requests.
  A piece's creature cooldown does not prevent a visual suggestion to the player;
  all normal action feasibility and cooldown checks remain in force.
- The creature gestures once for up to two seconds while idle. Feedback reactions,
  pause, carrying and action ownership take priority. Pause hides the cue and
  freezes its expiry clock. Starting another action, player pickup, new placement,
  new shot, interruption, End, reload or expiry dismisses it.
- The shared light renderer supplies two visible guides, no colliders, selectable
  items, resources or new models. The four gateway milestone lights stay hidden.
  Its existing bounded geometry is reused and disposed with the creature.
- Showing the invitation never creates placement receipts, action completions,
  training credit or project progress. Native pickup/drop creates real player
  evidence and the existing witnessed-snap invitation as before. The actual fetch
  lesson remains available for contextual feedback.

## Verification and limits

`BowlRebuildChecks.Verify` extends the native ball-return fixture. A real safe
player shot and utility-selected lift/carry/place arm the request. The check then
stages a visible loose-piece viewing position and exercises feedback/pause guards,
the two read-only guides, bounded gesture duration, stale-save ownership, native
player pickup/snap with a real placement receipt, preserved fetch lesson, expiry
and a new throw. The expiry/throw guard fixture reuses the same real return receipt
to rearm the transient request; it does not claim a second physical return.

Existing gateway cue and shared-building input regressions cover the factored
renderer and normal input. The fixture constrains other activities and parks
villagers, so spontaneous full-population participation, gesture readability and
Quest comfort remain open playtest gates. No APK is built in this increment.

Next candidate: player-driven course variations and a creature turn at a rebuilt
course, with physical outcomes and distinct attribution for each actor.
