# Shelters become small social places

S4 increment, 2026-10-26. A villager who reaches a useful shelter can now invite one willing nearby neighbor, using the existing gathering gesture, leisure motor, relationships and visitor lease. There are no new buttons, signs or permanent UI.

## Build and observe

A canopy with just one qualifying resting position remains a solo shelter. Extend the construction with another separately supported roof nearby, leaving headroom and clear access. The second position must be within 10 terrain units of the host and separated by both villagers' navigation radii plus a small margin. It cannot route the guest through the host's occupied position.

A tested example in the main yard uses four Columns at local x = -2 / 2, y = 2.4, z = -12 / -18. Place flat Slabs at (0, 5.4, -12) and (0, 5.4, -18). Other arrangements can qualify through the same geometry checks; a named completed recipe is not required. The resting host checks existing neighbor willingness and availability before inviting. Busy, frightened, injured, held or unavailable neighbors keep their ownership protections. No guest is guaranteed.

The guest approaches a clear entrance, then their own covered resting spot. Both look toward each other during the encounter. After three seconds together, the existing shared-rest outcome assesses each participant's comfort, records per-person place experience and improves friendship only if both are comfortable. Comfort now searches all qualifying roofs near each participant, rather than only the first roof in the yard. The guest does not receive a fabricated solo-shelter completion, construction credit or creature teaching receipt.

The host retains the site's visitor lease. Only one guest can join. The host continues the ordinary six-second shelter rest, waiting at most 24 seconds while an invited guest approaches. A missing guest cannot reserve the space indefinitely. Existing shared-rest/place cooldowns and subsequent solo completion avoid duplicate person-visits. Confirmed requested-cover use still occurs only at the host's completed shelter rest.

## Interruption, cost and persistence

Existing supported/braced geometry, held-piece exclusions, headroom and clear-lane checks remain in use. Guest cover and host validity are rechecked at 4 Hz. Losing the guest's cover cancels the social encounter while leaving a valid host free to finish resting alone. Losing the host's cover cancels both participants. Pickup and normal action cleanup release the guest link; pause suspends progress. Guest arrival must actually reach its separate spot, including when navigation stops near an occupied target.

Uses existing bounded shared-rest, friendship and place memories; there is no new save schema or reconstructed history. Active invitations are transient and do not resume or award outcomes on reload. No resource grants, water/food priority changes or new economy are introduced. Construction still uses the existing 48-piece sandbox and private physics.

## Verification

Unity 6000.6.7f1, FirstQuestSlice, temporary DX11 batch Editor. Final compilation passed with no errors or warnings.

SharedShelterChecks passed staged native host and guest navigation, one-roof capacity rejection, two-roof invitation without a recipe, individual rainy comfort, friendship/rest outcomes, one guest, pause, save round trip, one person-visit per participant, guest/host roof loss, pickup cleanup, continued solo host completion and bounded waiting for an unresponsive guest. Needs, construction and starting positions are staged; the invitation and movement use existing controllers without writing completion counters.

Existing GatheringChecks dry/rain, PlaceRequestChecks, RequestedPlayChecks, ShelterChecks model/runtime, GatheringPlaceChecks, FeedbackChecks, DevelopmentFocusChecks and CreatureCarryChecks passed. The older shelter fixture was run after canceling pre-existing resource jobs, preserving its normal ownership gates. Editor frame progression was observed; a separate focused camera captured the constructed shelters. That render is a placement smoke check, not proof of natural gesture cadence or headset readability.

The user's original save was restored with matching SHA256 after verification. XR startup flags were restored for Standalone and Android, and the temporary Editor closed. No APK or headset validation is claimed. Unattended encounter frequency and physical Quest readability/comfort remain playtest work. This increment does not close the complete integrated S4 gate.
