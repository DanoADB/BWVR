# Workshop tool handoffs

S5 multi-step capability increment, 2026-10-30. This connects the existing workshop designs, finite resources, creature utility selection, carrying motor, villager equipment and teaching receipts. It does not introduce another AI framework.

## Play

A known village axe/hoe design and available materials can produce one workshop offering for a nearby forester/farmer who lacks that equipment. The visible offering uses the imported Kenney upgraded axe/hoe model, with a technical pickup collider and a horizontal carry orientation. There is no new primitive artwork or controller binding.

Book → Creature → Workshop handoffs names the intended recipient and reports the latest offer/delivery. Carry the physical tool to that villager yourself, or offer Help at the tool and let the creature choose. A free recipient briefly looks toward the workshop using the existing celebration/gaze system; the learning icon also signals the opportunity. No villager is ordered to stand still or teleported for a handoff.

The creature's bounded plan is approach → lift → carry/follow → lower/place → confirm equipment. The existing action coordinator owns its movement and object. The handoff approach samples at most eight nearby positions, respects both body sizes, and retains a stable, briefly cached approach unless the recipient moves or the spot becomes blocked. At most three alternative approaches are attempted before reporting a route failure. Blocked routes, injury/fright, player pickup, timeout, changed conditions and interruption clean up ownership. Existing failure-aware attempt memory applies to reported route failures. The creature can choose another activity or decline Help.

## Inventory, knowledge and attribution

The offering reserves two wood and the existing resin cost (axe: two, hoe: one). Reservation is deducted from FreeWood/FreeResin, protecting other crafting/field/cathedral spenders. Nothing is debited and no equipment/knowledge is granted merely because an offering appeared or a carry started. On confirmed arrival, the existing Craft operation spends the materials exactly once and equips the intended villager. A novice can learn the known design from this delivered artifact. The recorded maker is the source of design knowledge, not proof of a new fabrication animation.

A creature handoff creates one contextual HelpRequested/RespondToGuidance receipt and a modest recipient relationship change; praise/discouragement uses the existing twelve-second teaching window. It is not a beachball fetch or another toy completion. A player handoff records Player, grants the same paid equipment and clears creature lesson eligibility, without giving the creature credit. A player placement during pause defers confirmation until resume with its source preserved.

Unclaimed offerings expire after twelve simulation seconds, leaving villagers free to craft for themselves. New unsolicited offers are at least 120 seconds apart. A claimed/player-held delivery has a 150-second absolute deadline. On expiry resources are released; a tool still held by the player remains visible until released, cannot equip anyone from an expired offer, and cannot be replaced with a new offer in their hand. Food/water development priorities are unchanged.

## Persistence and limits

One optional ToolDeliveryState stores design source, recipient, type, source/status, opening/deadline, last safe grounded position and pending paused player arrival. Empty deserialized optional records use an explicit Dormant state, preserving old saves. No live carry or route ownership is saved. Restoring an in-transit tool places it on safe ground and allows a fresh twelve-second reclaim window within its original deadline; there is no payment or equipment duplication.

The delivery uses current known village designs and real stock. This does not add generalized tool invention, dynamic crafting recipes, physical material fabrication or unlimited inventory. Recipients keep their own agency, and the usual equipped harvest/sowing benefits remain owned by the existing resource system. Natural acceptance rate, noticeability of the short offer window and inherited carry animations need playtesting.

## Verification

Model/native Editor checks passed for existing design knowledge, no early grant/spend, distinct axe/hoe costs, exact source attribution, utility-selected physical carrying and placement, moving recipients, pause, pending native Save/Load beyond the initial offer window, player pickup/drop, paused player attribution, duplicate feedback, interrupted carry, held-tool expiry and depleted-stock cleanup. Existing technology/resource/planting/attempt-learning/storm-supply/Book checks also passed. Dependent navigation checks run after asynchronous tree/navigation updates settle. Test actors and competing activities are staged for reproducibility; these checks do not establish a full-population success rate.

The sourced tool and Book page are reviewed in staged Editor renders. The inherited creature carry pose still needs polish; physical Quest readability/performance remain unverified. The user deferred headset testing; no APK is built. User save/XR flags are restored after validation.
