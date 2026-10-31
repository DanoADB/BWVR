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

Next candidate: optional Book visibility for remembered phrases, followed by
bounded local villager-to-villager transmission using witnessed performances.
