# Remembered gathering places

S4 increment, 2026-10-26. Extends the existing exploration motor, bench invitations, geometry-based shelter visits and chime audiences. No new controller binding, permanent HUD or action controller.

## Playable behavior

A completed bench rest, shared bench visit, shelter rest, nearby witnessed chime performance or completed four-reception ball exchange can become a place experience. Each villager keeps independent, bounded memories for the existing three non-bridge site identities. Historical saves start without invented visits.

Comfortable visits raise appeal by 0.1. Exposed visits in rain of at least 0.5 lower it by 0.2. Appeal remains within 0–1. A 90-second simulation-time cooldown prevents a shared rest and its host's later bench completion from crediting the same visit twice. Nearby music is associated only with the nearest existing non-bridge yard within 18 terrain units; music elsewhere still has its existing personal enjoyment memory.

For known benches, a pleasant remembered place can attract a return at rest need above 0.35 instead of 0.65. An unpleasant place requires need above 0.9. Returns still compete in the existing leisure selection order, require navigation and an available visitor lease, and cannot take ownership from work, worship, construction, social activity or player pickup. Remembered appeal ranks available complete structures before distance. First-time inspection remains available, so this is not a hard prohibition on exploring an unfamiliar place.

Heavy rain suppresses ordinary exposed bench returns. Cover is checked using actual supported, stable construction geometry near the actor/rest destination, not a named recipe or the mere existence of a roof somewhere in the yard. Existing shelter-seeking remains available to tired villagers and can create a better experience at a previously disappointing place.

Shared bench visits assess each participant's actual position separately. Comfortable participants gain rest relief; exposed participants gain a small discomfort increase. Friendship grows only when both are comfortable. Bad weather does not blame the neighbor or reduce trust in the creature. Solo exposed bench use no longer grants the normal rest relief or first-use creature praise. Existing happy/uneasy gestures present the outcome, and optional journal entries state the place experience.

## Ball-play extension

Creature–villager and villager–villager rallies now connect to the same place memory at the existing fourth confirmed physical reception. Each participating villager assesses their own location and actual cover. Only a non-bridge site within 18 terrain units receives a visit. Ordinary solo kicks, invitations, spectators, incomplete rallies, pause and pickup cancellation do not fabricate participant visits. A transient ball/session receipt prevents re-crediting the same completed game even after the saved place cooldown expires. Active rallies are not restored across save/load, so this receipt needs no new persistent schema.

Autonomous creature invitations and peer-play choices now receive a separate place bias capped at ±0.15. Pleasant memories encourage play there; unpleasant memories and exposed rain reduce its likelihood. The bias leaves relationships, observational confidence, learned creature control and their existing seeded decisions intact. Explicit invitations still use existing capability and ownership gates. The ground beneath the ball is used for prospective cover; completion assesses each participant's feet. A fun exchange can therefore improve social ties while leaving a negative memory of standing in rain.

No new score is awarded for a harmful throw, and no extra creature training receipt is emitted. Existing impacts, blame, injury and witnessed caution keep their existing handling. This extension does not introduce a separate saved danger score for places.

### Ball-play verification

Unity 6000.6.7f1 DX11 batch Editor, FirstQuestSlice. Final compilation passed with zero errors/warnings. BallPlaceChecks passed its seeded preference model, dry and rainy native peer rallies, creature rally and distant-place checks. Every runtime exchange uses existing navigation, turn taking and private-scene ball physics; no reception counter is injected. For attribution isolation the fixture temporarily positions the main yard around a known reachable rally lane, then restores it. This validates the event/location connection, not a naturally selected unattended game at the unmodified yard.

Checks confirm both participants' saved memories, no credit before the fourth reception, no pause/cancellation credit, no repeat credit after 100 further simulation seconds and no credit to a distant yard. The native creature-rally check also retains its original training, control-practice, solo-kick and ownership checks. Existing peer friendship, visible observational learning, bench/place, chime audience, development focus, feedback and creature pickup checks passed. Editor frames advanced during verification. The user's original save and XR startup settings are restored after testing. No headset or APK validation is claimed.

## Earlier place-memory verification

Unity 6000.6.7f1, running FirstQuestSlice in a temporary DX11 batch Editor. Compilation passed without errors or warnings. Targeted checks passed:

- GatheringPlaceChecks model/runtime: legacy JSON, bounded/unique IDs and finite values, save round trip, native bench completion, return preference, cooldown, pause, player-held interruption, rain avoidance and exposed completion.
- GatheringChecks dry and rain variants: native host/guest invitation and approach, reciprocal comfort-dependent friendship, per-person place memories, no second visit credit at host completion, guest lease, cancellation, injury and destroyed bench.
- ShelterChecks model/runtime: supported arrangements and real covered rest, saved place experience, missing/held/blocked roof checks.
- MusicGatheringChecks model/runtime and SharedActivitiesChecks: existing audience and performer completion, learning receipts, pause and pickup cleanup.
- DevelopmentFocusChecks, FeedbackChecks and CreatureCarryChecks: exploration priorities, training and pickup regression checks.

The older gathering fixture initially rejected its host because a resource job from the live season retained ownership. Its setup now releases resource jobs before staging and resets family ownership against the test season. SharedActivitiesChecks has the same prior-job assumption; it passed after resource jobs were explicitly canceled in the verification invocation. No runtime ownership gate was weakened to make these checks pass.

Editor frame progression was verified (1330 to 14146) and a current yard/village view rendered successfully. This is a rendering smoke check, not validation of the comfort gestures or readability in a headset. The original user save is restored after testing; XR startup settings are restored and unrelated project settings excluded from the commit. No APK built.

Nonverbal cover and beachball requests are now implemented; see [GATHERING_REQUESTS.md](GATHERING_REQUESTS.md).

The read-only Book shell is now implemented; see [CREATION_BOOK.md](CREATION_BOOK.md).

## Remaining S4 work

This increment connects remembered utility to existing places. It does not complete the integrated slice: three distinct demonstrated useful player contributions and physical Quest playtesting remain open. Confirmed block-placement and repair attribution is now implemented separately; see [PLACE_CONTRIBUTIONS.md](PLACE_CONTRIBUTIONS.md). Placement alone does not prove utility. These memories describe actual instrumented visits, not proof of a new emergent cultural discovery.
