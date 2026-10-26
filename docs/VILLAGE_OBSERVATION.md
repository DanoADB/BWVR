# Temporary village observation focus

`QuestSlice.observeWithoutFoodWork` defaults on and is effective only alongside `developmentFocus`. These are development settings, not saved season preferences. Disable Observe Without Food Work in the QuestSlice Inspector to resume the food economy; disabling development focus also restores it.

While active, automatic food reserve jobs, harvesting choices, sowing and farm expansion are deprioritized by removing their automatic selection. Existing food work cancels safely through the existing cargo recovery and reservation cleanup. Food urgency no longer blocks toolmaking or displaces social/play activity. Meal consumption is paused so observing does not create an accumulating hunger emergency. Spoilage, crops, weather, wood/resin, storage, households, play, exploration and creature autonomy still run. Supplies are not refilled, and explicit direct gathering APIs remain available for isolated checks. Existing water development focus remains unchanged.

Children retain inherited traits and age. Their scale is now 68–88% of the parent's readable adult scale, rather than roughly half. They use the existing villager prefab collider plus a registered CarryItem, so ordinary player pickup/drop applies. Held children stop autonomous navigation and scale updates; released children resume their household routine. This does not give children adult jobs or social controllers. Family IDs and save schema are unchanged.

Editor validation: Verify food observation and child pickup (Play mode). Physical headset targeting/readability remains a user playtest.

Validation on 2026-10-26: the complete check body passed against the running Editor after correcting the birth fixture to use its actual nextBirth deadline. It verified live food-action cleanup, suppression/resumption, child scale, collider ray targeting, player grab, held immobility, release/reselection and valid family serialization. A later live snapshot at frame 7529 showed observation active, zero food jobs, one child and valid season data. Original save restored after confirmed Play stop.
