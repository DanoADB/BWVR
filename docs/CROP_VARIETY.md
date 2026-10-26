# Two crops and fresh-food reserves

Turnips extend the existing food-source pipeline, not a competing farm AI.
Wheat is the legacy/default crop: 180 seconds, 12 meal units. Turnips take 90
seconds and yield eight meal units. Both sowings cost one grain unit from actual
seed stock. Seed is an abstract planting budget in this prototype, not a botany
simulation. Existing crops and older saves default to wheat.

Available villagers choose turnips for a depleted plot when ready meals are below
six minutes or future harvest exceeds the reserve target, fresh stock is below 35%
of the reserve target, and no turnip plot is
already growing, waiting to be harvested or being sown. Otherwise they plant wheat. Existing three-growing-
plot, reachability, reservations, seed protection and emergency priorities remain.
Explicitly staged sowing may request either crop; normal players need no commands.

Turnips use a separate saved freshFood stock. Both foods share covered capacity;
fresh cargo that cannot fit returns to its source rather than turning into dry
exposed grain. Meals consume fresh food first, then unreserved grain. Remaining
fresh food decays exponentially with a 900-second time constant, even under cover;
loss joins the existing spoiled ledger. Grain retains its existing storage rules.
These are food-equivalent quantities, not weights or separate nutrition systems.
Growth, cargo, recovery, fresh stocks and crop identity persist. No offline growth
is granted. All timers advance only through the existing unpaused simulation.

Pink bulbs and broad green leaves distinguish turnip plots from wheat; a visible
crate/pile shows fresh reserves at the store. Harvests reuse the food sack and hoe
animations. Crop art is batched and collider-free. The current adults perform work;
children retain their existing behavior. Crop choice is an authored bounded
capability, not unrestricted invention or a new discovery claim.

Weather damage to standing crops, seasons, cooking, additional crops and autonomous
freestanding granaries remain future work. Water stays secondary; companion work
remains deferred.

## Validation and observed limits

TurnipChecks passed model/native checks for crop selection, reserved plots/seed,
pause/cancel, paid sowing, half-growth/save, finite harvest, delivery to fresh stock,
capacity overflow recovery, fresh-first meals, freshness losses and invalid saves.
Existing FarmingChecks (model/native), ResourceChecks (model/native),
HouseholdStorageChecks.VerifyModel, FarmExpansionChecks, FoodReserveChecks,
ToolTechnologyChecks and WeatherFoodChecks passed. After choice tuning,
TurnipChecks, FoodReserveChecks and FarmExpansionChecks passed again.

A 30-minute unattended clear-weather run from 18 grain/18 timber confirmed an
autonomously planted/matured turnip plot and a delivered fresh harvest. One turnip
plot remained at five units (three harvested), and 0.093 units of fresh food spoiled.
The saved state stayed valid; 24 deliveries completed, but meals were empty for
124.4 seconds after the initial five-minute warm-up and empty at the end. Food
priority/availability needs further tuning; this is not proof of a balanced economy.
No deterministic physics or cross-run balance guarantee is claimed.

A staged crop screenshot was visually inspected. The final scene advanced from
frame 3446 to 12827 with zero current console errors/warnings. Original user save
was restored and hash-verified after testing. No APK or headset validation.
