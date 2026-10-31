# Villager phrases and creature replies

Villager-led chime duets now use four distinct short two-tone motifs. A villager
offers the instrument through the existing fetch/carry/present behavior; the
creature can choose to join. Two notes from the villager are answered by the
creature, then a second two-note call receives its reply. Existing hands, chime
artwork, audio and eight-beat choreography are reused. No gathering job, new
controls, permanent HUD or second AI controller is added.

A stable identity and simulation seed select an initial authored motif. After a
fully heard and completed duet, the villager remembers that phrase across saves.
This is a small authored vocabulary with learned familiarity and preference, not
procedural composition or an uncoded emergent discovery.

## Hearing, practice and preference

- The creature records the four calls only within twelve game units and without
  an obstructing collider between its hearing point and the instrument. A ray
  check at each villager call ignores the two performers themselves. It is a
  bounded approximation, not an acoustic propagation simulation.
- All four heard calls and four matching replies must complete before phrase
  familiarity records. Partial, interrupted or obstructed performances do not
  create phrase memory. Ordinary shared-play outcomes retain their existing rules.
- The host retains a motif and bounded successful-echo count. The creature retains
  four bounded echo counts and four independent preferences. These fields persist;
  active performances and transient hearing masks do not resume on load.
- Actual completion adds a typed motif to the existing shared-play feedback
  receipt, with its real inviting villager as partner. Existing Quest A/B and
  desktop 1/2 feedback can then praise or discourage that phrase once, through
  the existing feedback window and consumption guards.
- Phrase preference contributes a bounded ±0.2 interest term when that phrase is
  offered again. It does not guarantee acceptance or override other needs,
  reservations, ownership, cooldowns or learned social relationships. Unknown
  motifs do not acquire preference from feedback.
- Player-led rhythm demonstrations and echoes keep their separate memory and
  source attribution. Rain rituals, audiences and place music reuse their current
  paths. Showing or starting an invitation does not count as learning a phrase.

## Verification and limits

`VillageMelodyChecks.VerifyModel` covers legacy saves, the four authored motifs,
unknown/positive/negative preference behavior, bounded serialization, malformed
arrays and typed-lesson provenance.

The native runtime check extends the existing shared-activity fixture: a villager
physically fetches, carries and presents the chime; the normal utility model
selects the duet; eight actual tap sources and notes match the two calls/replies;
host and creature record the completed motif; native feedback reinforces it once;
and native Save/Load preserves the motif, count and preference. The existing
fixture also exercises pause, cancellation, duplicate outcomes and actor pickup.
A second run introduces a technical blocking collider after the first reply and
verifies that partial hearing grants no motif memory or motif-specific lesson.

The fixture constrains competing activities and stages a reachable invitation.
It does not establish natural participation frequency, actual headset audio
quality or Quest performance. These remain physical playtest gates. No APK is
built for this increment.

## Local transmission and Book visibility (2026-10-31)

A reserved audience member can now learn a phrase after two comfortable,
completed performances in which it saw the performer and heard all four call
notes. The existing bounded audience, visibility checks, pause, pickup and
activity ownership remain authoritative. A partial, blocked, canceled or
late-arrival performance cannot teach a phrase. Completion can count only once.
Hearing is checked at each actual call, within 24 game units, with a physical
sightline to the instrument; this does not depend on whether the creature heard
its own side of the duet.

Each villager keeps four bounded hearing counts, one adopted phrase and its
teacher's stable identity. Existing performers retain their own phrase. Learning
by listening does not award a played game or fabricated practice. When the
learner later offers the chime through the normal social routine, its learned
phrase is used; a completed duet then creates its own performance memory.
No new motor, global broadcasts or guaranteed command is introduced.

Book -> Creature -> Village phrases shows the creature's real echo counts and
preferences, including unknown phrases. People profiles distinguish a learned
phrase from one the villager has actually performed. These are read-only saved
projections. The four patterns are authored; their transmission and learned
preferences must not be labeled uncoded discoveries.

`MelodyTransmissionChecks.VerifyModel` checks old saves, the learning threshold,
knowledge versus practice, malformed history and pure Book reads. Its native
runtime check uses real stationary offers and utility-selected duets: late and
occluded listeners are excluded, paused notes do not count, two real complete
gatherings teach the phrase, duplicate completion has no effect, the same
listener becomes the next actual host, and native Save/Load retains the result.
Existing melody, audience and Book runtime/layout regressions also pass in the
Editor. Fixtures stage visible reachable actors and suppress competing activities;
this does not establish natural frequency, sound quality or Quest performance.

Next candidate: make shared music easier for the player to invite nonverbally;
headset readability and natural participation tuning remain pending playtesting.
