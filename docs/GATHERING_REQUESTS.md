# Villagers ask to improve gathering places

S4 increment, 2026-10-26. Uses the existing leisure scheduler, navigator, site requester lease, gestures, supported-cover test and beachball controller. No new buttons, floating signs, command menu, trust bonus or creature orders.

## Requests and responses

A villager needs an actual recorded visit before asking to improve a place. They approach the existing request position and briefly demonstrate a need:

- **Cover:** a tired villager with a remembered uncomfortable visit can shield their forehead and indicate the place during heavy rain, when no reachable supported shelter exists there. The two-hand gesture alternates with indicating the site.
- **Beachball:** a willing villager who likes the place and has completed ball play can beckon and point between a familiar, currently visible beachball and the site. The ball must be free, outside the place and seen through the existing distance/facing/occlusion checks. They do not request an unknown or already-present available toy.

Approach and signaling own the existing exploration motor, so routine work, worship, social carrying and construction cannot take competing control. One requester reserves a site. Routine equipment is put away so a water bucket cannot occupy the signaling hands, and normal routine cleanup restores it afterward.

Requests last up to 16 seconds after arrival, with a 90-second overall approach limit and 180-second saved per-person/place cooldown after signaling begins. Checks for improvements run at 4 Hz. The player and creature can ignore a request; a timeout releases it without a punishment or invented answer. Rain ending cancels a cover request. Pickup, injury, fright, site removal/lock, ownership loss and pause have safe cleanup or suspension behavior. No active gesture or reservation is restored on load.

A cover request is accepted only if the actor can start the existing shelter visit using actual stable geometry, headroom, clearance and reachable entry/rest positions. A ball request is accepted only after the delivered ball is released and the native play controller accepts its reservation/navigation prerequisites. Held, reserved or flying balls are unavailable. Normal shelter/play completion then produces its existing comfort and learning outcomes; request acceptance itself does not grant those rewards.

Each existing bounded place record saves request count, accepted-answer count and next request time. Answers cannot exceed requests. An answer records usable opportunity and action handoff, not a completed activity or evidence that the player was the contributor. Older saves start with zero request history. Existing bench-building requests remain available separately.

## Verification

Unity 6000.6.7f1, FirstQuestSlice in a temporary DX11 batch Editor. Compilation passed without errors/warnings. PlaceRequestChecks model/runtime passed: legacy JSON, bounded counts and finite times, native leisure selection and approach, prior-experience/familiarity gates, request ownership, saved cooldown, pause/pickup/weather/timeout cleanup, hands-free signaling, actual supported canopy-to-shelter use, held-ball rejection, released-ball-to-play handoff and saved history.

Gathering-place, shelter, creature/peer ball play, development-focus, feedback and creature pickup regression checks passed. Tests stage needs/experience and physical objects and advance existing action controllers; they do not demonstrate the frequency of requests in an unattended full session. Food and water development priorities are preserved.

Rendered pose inspection caught a real presentation issue: the idle water gatherer's bucket occupied both hands. The hands-free entry point fixes that request handoff. Final pose inspection uses an isolated copy of the existing villager prefab with the live request flags, avoiding paused animation state and same-frame IK pose carryover in the synchronous harness. It shows the forehead-shield and pointing poses; natural cadence and meaning at tabletop scale still need headset playtesting. A failed attempt to rebind the live test actor interfered with that fixture's subsequent rest, so the final passing checks use a fresh Play session and do not rebind gameplay actors.

The user's original season save and XR startup settings are restored after verification. No APK or headset result is claimed.

## Remaining S4 work

The read-only Book shell and island evidence presentation are now implemented; see [CREATION_BOOK.md](CREATION_BOOK.md). Place-specific contributor receipts remain open. The full integrated gate still needs three distinct demonstrated useful player contributions, a declined creature suggestion, autonomous progress, mixed social outcomes and physical Quest readability/comfort checks. These authored affordance requests are not newly discovered behaviors.
