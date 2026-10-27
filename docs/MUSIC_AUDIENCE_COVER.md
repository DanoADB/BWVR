# Spectators use nearby cover during rain

S4 increment, 2026-10-27. A villager joining a music audience during heavy rain can make a short walk into an available supported shelter while watching the existing creature/villager duet. No new command, job system or permanent HUD.

## Playable behavior

At audience entry, a listener who is not already covered checks the existing bounded construction sites for a useful resting point within six terrain units. The point must remain within the existing 24-unit listening range, have a clear construction-space approach, be body-clear and reachable by the existing villager navigator. The normal visibility and participation gates still apply. Dry-weather listeners keep their existing behavior.

A temporary claim belongs to the existing three-listener music session. Claims respect participant radii and spacing, so a second listener cannot claim the same spot while the first approaches. Sites already owned by a visitor, requester or repairer are excluded. Construction geometry and site ownership are rechecked at 4 Hz. No actor is teleported or given a second movement controller.

The adjustment lasts at most three simulation seconds. A blocked route, occupied arrival, lost roof or acquired site lease releases the movement claim and lets the villager continue listening from their actual position. There is no retry loop or forced crowd queue. A listener without available cover can still watch from outside. Claim cleanup follows audience completion, cancellation and pickup. Pause suspends the existing audience motor and timers; claims are transient and never restored from a save.

## Independent outcomes

The existing eight-beat performance, 1.5 seconds of witnessed music and completion visibility remain required. Cover and rain are evaluated at each listener's actual completion position. Sheltered and exposed villagers can therefore form different music-interest and place memories from the same performance. This is a useful placement consequence, not a new personality/learning framework or a simulation of personal wetness.

No additional reward, friendship, teaching or player-credit event is granted for moving into cover. Existing outcome cooldowns and save validation remain authoritative. Food observation, water priority and player controls are unchanged. New data is limited to at most three transient spot claims and per-listener movement state; there is no save schema change.

## Verification

Unity 6000.6.7f1, FirstQuestSlice, temporary DX11 batch Editor. Final compilation passed; the three new cases passed again in a fresh Play session.

MusicCoverChecks exercises an actual stationary chime offer, utility-selected creature approach, native listener registration and movement, eight reciprocal beats and existing completion receipts. A staged nearby listener approaches a supported canopy; the claim rejects another audience member, while a separately visible exposed listener keeps an independent rainy outcome. Checks confirm positive versus negative interest/place experience, null-listener claim rejection, pause without reward, cover-loss claim cleanup, listener pickup and valid saved state. The tests stage construction/needs/positions and use native actions; they do not establish unattended frequency or crowd balance.

Initial test positions blocked the exposed listener's view with scene objects/another actor. The fixture now chooses a real clear sightline after the performer reaches the instrument, as the older music fixture does. No runtime visibility rule was weakened. The existing place-music harness now ticks registered audiences and exposes setup/completion callbacks for this composed scenario.

SharedActivitiesChecks, MusicGatheringChecks model/runtime and SharedShelterChecks passed. FeedbackChecks, DevelopmentFocusChecks and CreatureCarryChecks passed. Editor frames advanced during verification. A focused scene capture was inspected as a placement/rendering smoke check; it does not verify natural animation cadence or physical headset readability.

The user's original season save was restored with matching SHA256, Standalone/Android XR startup flags restored, and the temporary Editor closed. No APK or Quest result is claimed. The complete integrated S4 scenario and headset gates remain open.
