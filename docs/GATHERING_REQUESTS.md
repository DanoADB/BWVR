# Villagers ask to improve gathering places

S4 increment, 2026-10-26. Uses the existing leisure scheduler, navigator, site requester lease, gestures, supported-cover test and beachball controller. No new buttons, floating signs, command menu, trust bonus or creature orders.

## Requests and responses

A villager needs an actual recorded visit before asking to improve a place. They approach the existing request position and briefly demonstrate a need:

- **Cover:** a tired villager with a remembered uncomfortable visit can shield their forehead and indicate the place during heavy rain, when no reachable supported shelter exists there. The two-hand gesture alternates with indicating the site.
- **Beachball:** a willing villager who likes the place and has completed ball play can beckon and point between a familiar, currently visible beachball and the site. The ball must be free, outside the place and seen through the existing distance/facing/occlusion checks. They do not request an unknown or already-present available toy.

Approach and signaling own the existing exploration motor, so routine work, worship, social carrying and construction cannot take competing control. One requester reserves a site. Routine equipment is put away so a water bucket cannot occupy the signaling hands, and normal routine cleanup restores it afterward.

Requests last up to 16 seconds after arrival, with a 90-second overall approach limit and 180-second saved per-person/place cooldown after signaling begins. Checks for improvements run at 4 Hz. The player and creature can ignore a request; a timeout releases it without a punishment or invented answer. Rain ending cancels a cover request. Pickup, injury, fright, site removal/lock, ownership loss and pause have safe cleanup or suspension behavior. No active gesture or reservation is restored on load.

A cover request is accepted only if the actor can start the existing shelter visit using actual stable geometry, headroom, clearance and reachable entry/rest positions. A ball request is accepted only after the delivered ball is released and the native play controller accepts its reservation/navigation prerequisites. Held, reserved or flying balls are unavailable. Normal shelter/play completion then produces its existing comfort and learning outcomes; request acceptance itself does not grant those rewards. Confirmed requested use is now tracked separately, as described below.

Each existing bounded place record saves request count, accepted-answer count and next request time. Answers cannot exceed requests. An answer records usable opportunity and action handoff, not a completed activity or evidence that the player was the contributor. Older saves start with zero request history. Existing bench-building requests remain available separately.

## Confirmed use after an accepted request

The Book's Places page now separates accepted requests from **Used after request: cover / ball**. Cover records one completed six-second sheltered rest by the requester. Ball records one completed four-reception shared game by that requester with the requested ball at the same gathering site. Solo nudges and unfinished games do not meet this narrower shared-play measure.

Acceptance stores only a transient link to the current season, site, requested activity and specific ball. The activity's existing physical completion boundary confirms the use once. Pause does not advance it. Pickup, cancellation, lost cover, timeout or ordinary activity cleanup discards the link. A later unrelated rest/game cannot complete it. Save/load retains confirmed counts but does not resume a pending link or invent earlier use. A changed season cannot consume an old link.

These bounded per-person/site counts cannot exceed accepted requests. They do not add rewards, teaching, trust, player authorship or commands. They establish that a requested opportunity was used, not who supplied it. Existing place comfort, relationships and training remain separate. Historical zero means no recorded confirmed uses; earlier uninstrumented outcomes remain unknown.

### Confirmed-use verification, 2026-10-26

Unity 6000.6.7f1, fresh FirstQuestSlice Play session in the temporary DX11 Editor; final compilation had no errors/warnings.

- Expanded PlaceRequestChecks model/runtime passed: bounded/legacy save data, actual cover request and approach, no credit at handoff or pause, removed-roof cancellation, unrelated later rest excluded, second genuine request completed once, canceled ball handoff excluded and confirmed counts retained on save round trip.
- RequestedPlayChecks passed a staged experienced/familiar request through native approach, held/released ball handoff and four actual creature/villager physical receptions. No reception counter was injected. It retains BallRallyChecks training, control practice, cancellation and independent villager-nudge checks. The fixture locates the gathering site around the existing reachable rally lane to isolate the event/location connection; it does not prove unattended encounter frequency at the unchanged yard.
- CreationBookChecks model/runtime passed read-only projections, maximum aggregate counter layout, pointer/held protections and pause/run behavior. A Book capture from the completed requested game was visually inspected.
- PlaceContributionChecks model/runtime, FeedbackChecks, DevelopmentFocusChecks and CreatureCarryChecks passed. Editor frames advanced during checks (826 to 6712). No water/food priority or action-selection tuning changed.

Two test assumptions were corrected while composing the scenario: the older rally fixture now expects one additional game relative to its actual starting memory, and the completion check no longer jumps the simulation clock beyond the existing teaching window before that fixture verifies feedback. A fresh Play session was used after script recompilation to reinitialize runtime registries. No product ownership or teaching gate was weakened.

The original user save and XR startup settings are restored after testing; no APK or physical Quest result is claimed. These two staged request/use paths do not close the integrated three-useful-contribution, declined-suggestion, autonomous-progress and mixed-outcome gate.

## Verification

Unity 6000.6.7f1, FirstQuestSlice in a temporary DX11 batch Editor. Compilation passed without errors/warnings. PlaceRequestChecks model/runtime passed: legacy JSON, bounded counts and finite times, native leisure selection and approach, prior-experience/familiarity gates, request ownership, saved cooldown, pause/pickup/weather/timeout cleanup, hands-free signaling, actual supported canopy-to-shelter use, held-ball rejection, released-ball-to-play handoff and saved history.

Gathering-place, shelter, creature/peer ball play, development-focus, feedback and creature pickup regression checks passed. Tests stage needs/experience and physical objects and advance existing action controllers; they do not demonstrate the frequency of requests in an unattended full session. Food and water development priorities are preserved.

Rendered pose inspection caught a real presentation issue: the idle water gatherer's bucket occupied both hands. The hands-free entry point fixes that request handoff. Final pose inspection uses an isolated copy of the existing villager prefab with the live request flags, avoiding paused animation state and same-frame IK pose carryover in the synchronous harness. It shows the forehead-shield and pointing poses; natural cadence and meaning at tabletop scale still need headset playtesting. A failed attempt to rebind the live test actor interfered with that fixture's subsequent rest, so the final passing checks use a fresh Play session and do not rebind gameplay actors.

The user's original season save and XR startup settings are restored after verification. No APK or headset result is claimed.

## Remaining S4 work

The read-only Book shell and island evidence presentation are now implemented; see [CREATION_BOOK.md](CREATION_BOOK.md). Confirmed site-specific block-placement and repair receipts are now implemented; see [PLACE_CONTRIBUTIONS.md](PLACE_CONTRIBUTIONS.md). These do not attribute every kind of assistance or prove useful construction. The full integrated gate still needs three distinct demonstrated useful player contributions, a declined creature suggestion, autonomous progress, mixed social outcomes and physical Quest readability/comfort checks. These authored affordance requests are not newly discovered behaviors.
