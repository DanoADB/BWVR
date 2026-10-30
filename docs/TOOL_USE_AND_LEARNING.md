# Delivered tools: demonstrated use and useful outcomes

S5 continuation, 2026-10-30. This adds evidence and a small voluntary work opportunity to the workshop handoff, using existing gathering, equipment, observation, stock and animation systems.

## What changes in play

A newly delivered axe can encourage its recipient to choose one normal standing-tree gathering trip when wood is needed. Normal actor availability, cooldown, reachable/safe sources, capacity, food emergencies and urgent storage still constrain it. Existing social/family/scheduler priorities remain. The preference ends after an actual use or by 120 seconds after the offering's original delivery deadline. Old handoffs do not retroactively acquire this preference.

Food work stays deprioritized: a delivered hoe does not start farming while observation/development focus suppresses food gathering. When normal farming is enabled, actual equipped hoe harvests can produce the same evidence.

Successful equipped harvests now have separate bounded axe/hoe use counters. Nearby free villagers can learn the design while facing the worker, within 18 island units and with a clear physical sightline. Equipped-work observation uses collider visibility instead of a navigation-segment proxy. Seeing work teaches knowledge, not free equipment, motor practice, resources or global belief. Observation is sampled at the successful harvest event; it is not a full-duration classroom simulation.

## Evidence follows real outcomes

The latest handoff has an optional ToolUseProof: actual use, resource source, simulation time, newly taught villagers, and whether produce reached storage. Book → Creature → Workshop handoffs shows this under the original delivery attribution.

Using an axe and stocking wood are separate events. A canceled carry can retain evidence of real felling/learning, but earns no stored-produce result. A later delivery from that same source by the recipient can complete it within 180 simulation seconds of the recorded use. No stock is added by the evidence system: it records the existing successful Deposit amount, at most one load of three units. Full stores, abandoned cargo, duplicate callbacks, pause and unrelated sources cannot fabricate success.

Individual use counters persist. The detailed proof belongs to the latest handoff and may be replaced by a later offer; this is not an unlimited work history. Empty optional proof records remain valid for historical saves, and Book reads do not initialize history. Work is attributed to the villager; player/creature delivery attribution remains unchanged. No additional creature lesson, trust bonus or faith award is minted for the villager's work.

## Validation

`ToolUseChecks.VerifyModel/VerifyRuntime` passed in Unity 6000.6.7f1. The native fixture performs a player pickup/drop handoff, lets the recipient's ordinary gathering selector choose needed standing wood, and follows actual navigation/harvest/carry/deposit phases. One staged observer sees the work; another is physically occluded. Only the visible observer learns, without receiving equipment. Canceling cargo leaves storage uncredited; a later real salvage delivery stocks three wood, preserving the original tool payment and player source. Pause, duplicate receipts, native Save/Load and exact Book values are checked.

Existing tool technology, workshop handoff, resource, planting and Book model/runtime checks passed. The populated proof page was rendered and visually inspected within the existing panel. Test traffic and observer positions are staged for repeatability; this does not establish a natural full-population participation rate. No new artwork or controller binding is added.

The original user save and XR startup flags are restored after testing. Physical Quest testing remains deferred at the user's request; no APK is built or device performance claimed.
