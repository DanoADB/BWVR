# Book of Creation — read-only shell

S4 increment, 2026-10-26. Open the sanctuary with left-controller X (Tab on desktop), then select **Book of Creation**. Use the existing right front-trigger pointer (F on desktop) for tabs, Previous/Next and Back. No new controller binding. The sanctuary and Book remain hidden by default.

## Pages

- **Creature:** active plan, hunger, curiosity/boredom when recorded, strongest experienced object preference, known construction forms, confirmed physical placement count and the two most-sampled learned preferences. These preference scores are not guarantees of the next action. Object labels come from the existing target registry.
- **People:** one adult profile at a time, with actual trust, affection, faith, rest need, injury, ball/music/rest/construction experience and recorded tool practice/equipment. Existing child profiles show parentage and inherited traits; they do not invent adult social memories for children.
- **Provisions:** stored food/wood/resin against actual upgraded capacity, exposed/spoiled food, deliveries, storage upgrades, growing fields/regrowing trees and recorded player gifts.
- **Places:** recorded person-visits, comfortable/exposed visits, mean remembered appeal and request/accepted-answer counts for each recorded stable site. Confirmed requested cover use and four-reception shared ball play now have separate counts; see [GATHERING_REQUESTS.md](GATHERING_REQUESTS.md). Each site now has a second page for confirmed block-placement contributors and repairs; see [PLACE_CONTRIBUTIONS.md](PLACE_CONTRIBUTIONS.md). A separate page shows the two latest retained island completion, miracle or provision records, with actor and source distinguished.

No visit counts are presented as unique gatherings. A shared visit counts each participant. Accepted requests describe handoffs, not completed activities or proven player contributions. New confirmed block snaps identify the player, creature or individual villager at a stable site. Earlier placements and other kinds of contribution remain **unknown**; place memories and general island evidence do not establish their authorship. The Book does not backfill old history, call authored recipes discoveries, or claim that a player-guided creature action was performed by the player. Missing records remain unknown. Island evidence retains the existing latest-96-entry bound; its two-row view is not a complete lifetime log.

## Interaction and implementation

The existing wrist Canvas gains one menu entry and an optional parchment-colored four-tab page. Text and graphics use a resource-retained stereo-capable UI material so clouds, world geometry and the spirit cannot obscure the page content. This changes only the Book's rendering; its world-space pose follows the existing left-hand menu and introduces no camera movement. Other sanctuary actions remain available through Back. Returning from Sound does not accidentally open the Book.

The Book owns UI interaction while visible. Its pointer accepts tracked right-hand press edges on Book controls and blocks pickup, teaching and unrelated SpatialTargets through the page. Button colliders are triggers, not solid obstacles; existing actor visibility queries ignore triggers. Opening is declined while the player holds an object. Menu/tool shortcuts retain their existing contexts. No world action or navigation destination is issued by a Book button.

Page projections read raw saved fields, avoiding state properties that lazily create or migrate data. They produce detached strings and never initialize history, spend resources, reinforce behavior, reserve objects or change pause. The running simulation continues; an already-paused season remains paused. Visible pages refresh at 2 Hz and hidden pages do no periodic work. No save schema change is needed.

## Verification

Unity 6000.6.7f1, FirstQuestSlice, temporary DX11 batch Editor. Final C# compilation passed without errors/warnings; the Book shader reported supported with no shader messages.

CreationBookChecks model/runtime passed:

- Historical and populated states remain byte-for-byte equivalent under repeated JSON comparison before/after read projections and UI navigation.
- Exact stock/profile/place values, honest missing history, player gift attribution, bounded evidence and save round trip.
- Hidden default, adult/child pagination, every live page within body bounds, long retained-event fields, renderer inspection of tabs/content and no cloud/spirit occlusion after the UI material fix.
- Physical raycast to a navigation trigger; tracked press-edge navigation, no repeated held-trigger page turns, untracked rejection and unrelated-world-target isolation.
- Held-item protection, Sound/Back coexistence, simulation-time advancement with Book open and unchanged paused state while reading.

GodToolsChecks, FeedbackChecks, DevelopmentFocusChecks and CreatureCarryChecks passed. Editor frame progression was observed during verification. The user's original season save and XR startup settings are restored after testing. Existing unrelated project settings are excluded from the commit. No APK built and no physical Quest result claimed. Font size, wrist reach, depth comfort and Android shader/performance validation remain device checks.

## S4 status

The Book shell is delivered for Editor testing. Confirmed block-placement and repair receipts are now implemented. The integrated three-useful-contribution/declined-suggestion/autonomous-progress/social-outcome scenario remains open, along with hardware validation. This increment does not mark the whole shared gathering-place milestone complete.
