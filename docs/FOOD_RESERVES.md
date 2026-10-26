# Food reserves

Implemented 2026-10-25 as the next food-security step after renewable wheat.

The village derives a target from its actual adult/child population: six minutes of consumption at the existing one-grain-per-person-per-90-seconds rate, plus three future sowings. The target is capped by covered capacity (15 grain for three people, 27 for six at normal capacity). Seed reserved by an active sowing trip is excluded from meal availability. These are derived values; inventories, cargo, seeds and consumption keep their existing save format.

Before starting discretionary social/play or forestry work, an idle adult may choose a reserve job. One early-priority slot is available normally, two below 135 seconds of meal supply. Existing owned actions, family activities and non-development urgent water work retain their ownership. Ordinary autonomous routines can still choose food; these caps apply to early-priority slots, not permanent professions. The former water gatherer can help reserves too. Once covered grain plus promised deliveries reaches the target, the early-priority bias stops. With no ripe harvest, a worker may use the existing paid sowing loop; empty stores never create free seed or food.

A small physical post beside the store fills as meal-ready stock approaches the target. It is red in an emergency, amber below target and green when ready. Height communicates fullness as well as color. It shares materials, adds no blocking collider or Rigidbody, and refreshes with the existing half-second resource visuals. Existing capacity/overflow markers remain distinct.

This is preparation through reserves, not weather forecasting, seasonal simulation or expanded farmland. Food variety, additional farm plots and weather-driven harvest protection remain later milestones. Water stays secondary in development focus. A reserve target does not guarantee a self-sufficient economy if no harvest/seed is available or other needs occupy the village.

## Verification

Native `FoodReserveChecks.Verify` passed population/capacity targets, early and emergency work selection, exclusive plot claims, pause, reserved seed cancellation, physical marker height, a real three-grain delivery and save validity. Farming/resource runtime checks and all fourteen broader gameplay regressions also passed.

A one-hour unattended clear-weather run from 18 grain/18 timber finished with valid state, 77 deliveries, ten paid sowings and eight mature crops. The household grew to six people (target 27 grain). After a five-minute warm-up, meal-ready stock was empty for 145.2 seconds; the final one grain was reserved for planting. This is a functioning reserve-priority mechanic, not proof of a balanced economy. Farm expansion and planting/harvest cadence remain necessary follow-ups; no new terrain plots or weather forecast were added here.

The live scene advanced past frame 8300 with zero current console errors (two warnings). The original player save is preserved through a pre-test backup. No APK or physical headset validation was requested.

Close-up empty/ready renders were visually inspected: the post shows a short red fill versus a full green fill. The first preview camera was obscured by terrain and was raised for verification. The original player save was restored and hash-verified after the final preview.
