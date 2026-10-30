# Build with me

Snap a block into a construction yard while the creature watches. The existing curious glance/affect and notice sound acknowledge a perceived placement. For 45 simulation seconds, feasible loose blocks in that same yard receive a bounded, declining attention contribution to normal utility selection. The creature may pick one, carry it and continue a known construction form using the existing construction executor. Praise/discourage the completed placement with the existing controls. Read **Book → Places → Build with me** (the last page) for instructions.

The invitation does not reserve pieces, teach an unknown recipe, teleport anyone, interrupt a current action or guarantee completion. Critical needs and feasibility gates still apply. Player-held and villager-owned pieces remain unavailable. Taking back the invitation block withdraws it, even if it is subsequently dropped unwitnessed. A later witnessed player snap can refresh or move the single invitation to another yard. An already complete recipe receives no boost. Bridges are excluded from this first increment.

## Integration

`ConstructionSite.PlayerPlaced` is emitted only after a new valid player placement receipt enters the existing bounded contribution log. Anonymous, creature, repair, failed, paused, locked and unchanged repeat placements do not emit a fresh invitation. The normal detach path withdraws an anchored invitation. `CreatureConstructionLearning` owns event subscriptions and cleanup. `CreatureSharedBuilding` supplies attention to existing candidates; no second action controller, scene scan, new input binding, artwork or physics settings are introduced. A chosen action is attributed as player placement influence while the construction itself remains a creature action.

Pause/suspend prevent decisions and simulation time does not consume the invitation. New-season reset and save/state replacement invalidate it. The transient request is intentionally unsaved; construction geometry, contextual learning and separate player/creature placement receipts retain their existing persistence. No new save fields or inferred historical successes are added.

## Verification

Unity 6000.6.7f1 compilation passes. `SharedBuildingChecks.Verify()` stages a visible reachable yard, moves villagers out of the test route and cools unrelated affordances. It performs native player grab/raycast release, checks receipt-driven attention, then lets the regular coordinator choose and physically carry/place a further block. It verifies same-yard scope, rear/unseen placement, anonymous/paused placement, repeat-drop no refresh, carry withdrawal, pause/suspend, expiry, attributed contributions, serialized geometry/evidence and no transient invitation after state replacement. Test-fixture fixes distinguish a canceled duplicate Drop from a refreshed request, use distinct simulation timestamps and call public perception directly.

This staged test verifies the execution and integration, not natural selection frequency among all island activities. The Book page is included in layout/runtime checks. Existing contribution, Book, first-steps and creature response checks are rerun. Physical Quest interaction, response readability and subjective participation frequency remain playtest gates. No APK is built. Original save and XR startup flags are restored after QA.

Next: playtest player-led construction alongside villagers, then add consequences based on actual use of the shared structure. This increment adds a physical invitation, not a new challenge checklist or arbitrary building reward.
