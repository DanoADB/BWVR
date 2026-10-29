# Island Finds — exploration through shared discoveries

Delivered 2026-10-29 for desktop Editor testing. The existing six discovery clearings now give the player an exploration-and-gifting loop: find something, bring it back, offer it, and see whether the creature chooses to finish interacting. Confirmed outcomes unlock concise Book field notes. This extends ordinary carrying, creature curiosity and sourced discovery objects; it adds no competing AI controller or guaranteed command.

## Play

Open Book of Creation > Sections > Finds. The overview shows clearings found and finds accepted; six clearing pages give hints, status and earned notes. Mark clearing uses the existing optional 45-second teal beacon. From the overview it prefers an undiscovered clearing, then an unshared one. A selected clearing page can mark that location again. The marker closes the menu without moving the camera.

Pick up a discovery at its original clearing with normal carry controls. A first pickup records the player find and plays a discovery sound. Bring its hammer, shaker or strawberry back and release it close to the creature. It may choose to play/eat; completion records one shared discovery for that clearing, gives a brief positive acknowledgment and unlocks a field note distinguishing food from a tool. Existing object preferences and contextual teaching remain authoritative. The player remains unseen; villagers continue their own activities.

There is no deadline for completing the trail. Offers have a 60-simulation-second freshness window; if the creature is busy, pick the find up and offer it again later. Multiple objects may wait together: there are twelve bounded runtime slots, one per existing discovery object. Accepting either object from a clearing completes that clearing once. Taking an object back withdraws that offer without erasing others. A villager reservation, expiry or lost object clears its offer. No repeated pickup or acceptance farms new trail rewards.

## Attribution and saves

A clearing is found only on an unpaused native player pickup of an available discovery within six game units of its home. Ordinary gifts/resources and remote objects moved by other actors do not invent new clearing visits. An offer requires a found clearing, normal native placement within fourteen game units of the creature, and an available unreserved object. It does not begin a motor action. Credit comes only from the existing curiosity controller's successful Completed event for that exact offered item, while not player-held. Looking, dropping, unrelated actions and interruptions do not count as accepted gifts.

TreasureTrailState adds three validated six-bit masks to SliceState version 1: found, shared and food. Shared is a subset of found; food is a subset of shared. Old saves begin with unknown player discovery history and pure Book reading creates no progress. Native Save/Load preserves earned masks and field notes. Runtime offers are not serialized; after a reload, re-present an unaccepted find. A new season resets this season's trail. Pause/suspend prevent recording and freeze simulation-based offer/marker expiry.

The beacon is shared presentation infrastructure with Play Day and clears when the season instance changes. Book controls retain the 240-by-60 rounded rectangles, icons and tracked press-edge input. The new Finds section occupies the sixth section-chooser slot. No new physical artwork, controller binding, economy, food/water priority or AI service is introduced.

## Validation and limits

TreasureTrailChecks model/runtime pass in Unity 6000.6.7f1: pure old-history reads, mask bounds/validation, native pickups/drops, pause and distance gates, hand ownership/re-presentation, unrelated and duplicate outcomes, interruption, simultaneous offers, expiry, native Save/Load, and utility-selected food and toy acceptance. Other affordances are cooled down and frozen villagers staged clear in the acceptance fixture, while normal perception, utility selection, navigation, carrying and completion remain active. These checks prove the controlled path; they do not establish a natural full-population acceptance rate.

Existing independent DiscoveryChecks, Book model/layout/input, Play Day runtime and Sanctuary regressions pass. Finds overview/clearing pages were rendered and inspected; the menu audit found 34 consistently sized controls with matching hit areas. User save and XR startup settings are restored after QA. Quest readability, broad-island beacon visibility and subjective enjoyment remain playtest gates. No APK built.
