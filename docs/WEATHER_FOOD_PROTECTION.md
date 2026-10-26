# Food sacks and visible-rain preparation

2026-10-26. Quaternius Bag Open is used for carried wheat and covered grain
stocks, replacing yellow spheres. Source: https://poly.pizza/m/rJuZexcuhU
The source explicitly lists CC0 1.0. Original GLB, derived FBX and source/license
record are in Assets/ThirdParty/QuaterniusFoodSack. It has 280 source triangles.
FoodSackImporter builds a collider-free normalized prefab using three shared
GameSurface materials and a small visible grain fill. Runtime carry animation,
load limits and resource ownership are unchanged.

Rain accelerates spoilage of exposed grain from .025 units/second in dry weather
to .1 at full rain. Covered grain is protected from this spoilage. Only current
rain intensity is consulted; no worker reads future random weather rolls. Rain
intensity is clamped; invalid deltas/rain and paused resource updates do nothing.
Losses cannot exceed exposed stocks and join the existing saved spoiled ledger.
No save migration is needed.

When visible rain exceeds .15 and grain is exposed, an available adult can
prioritize existing storage construction over optional food-reserve work or
crafting, unless food is already at emergency levels. Held/busy actors and active
actions retain ownership. The same 20-second job costs six actual timber on
completion, adds existing shelves/capacity and shelters overflow. A canceled job
retains the preexisting storage-progress semantics and never grants an upgrade
or spends timber early. Storage remains capped at three upgrades. Water stays
secondary in development focus.

This is a first observable weather response, not forecasting, new granaries,
storm sheltering, tide damage to stores, crop storm damage, seasons or a different
weather economy. Rain protection cannot solve overflow if timber or upgrades
are exhausted. Further weather-readiness work remains on the roadmap.

Validation: native WeatherFoodChecks passed for dry/wet loss, covered protection,
invalid inputs, bounded losses, observable-rain decisions, pause/held/emergency
exclusions, cancellation, exclusive storage work, paid shelter and save validity.
HouseholdStorageChecks.VerifyModel, ToolTechnologyChecks, FoodReserveChecks and
ResourceChecks.VerifyRuntime also passed. After correcting the carry scale/pivot,
WeatherFoodChecks passed again and the staged carrying pose was visually checked.
The final live frame counter advanced from 2 to 1533, with zero current console
errors or warnings. Physical headset validation and an APK build were not done.
