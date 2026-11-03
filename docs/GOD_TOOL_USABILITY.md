# God tool usability and visible fire — 2026-11-03

## Findings

Native dock, gift, planting and lightning checks could execute their effects. The live saved village had full food storage (120 units), no injured villagers, and only 6 usable Terraform points in 84 sampled dry patches. Healing healthy villagers and offering food into full storage correctly did not alter the simulation. The tool dock hid subtool names until hover, making category selection easy to confuse with equipping a tool. Continuous tools could reject a press without the discrete tools' haptic/notice feedback.

A rendered lightning test revealed the fire visibility problem: the emitter used conservative legacy collision bounds and was hidden within replacement foliage. Native material ignition/fuel/damage worked even when its flames were not visible.

## Changes

- Category rows show all available tool names immediately. Selection still consumes the selecting press; release before using the equipped tool. Existing circular tool disks and menu routes remain intact.
- Rejected continuous-tool presses report their reason through the existing notice/haptic callback once per press, rather than once per held frame. Discrete healing/guidance keep their own feedback path. Full storage takes precedence over secondary gift targeting errors.
- Terraform uses a 12-unit radius rather than 30, a matching smaller preview mesh and a more forgiving charge movement allowance. It changes dry eligible cells rather than rejecting a whole patch because a neighbouring cell is shoreline/steep. Lowering never takes shoreline cells below their natural level or the protected inland minimum. Proposals preserve surrounding walkable slope classification, so raising does not strand the patch. Smoothing uses the local dry height average, not just the centre height. Buildings, trees, actors, plots, toys, construction, pause, costs, cooldowns, bounded saves and prepared navigation publication retain their protections. The same coarse audit now accepted 51 of 84 dry sample points; this is a staged Editor measurement, not an island-wide guarantee.
- Fire placement uses cached mesh renderer bounds above the canopy/roof, leaving physics/navigation/impact bounds unchanged. Sixteen reusable fire pools retain their limit; each allows twenty particles. The fire size is more visible. Exhausted fuel cannot accept another charged player strike. Lightning reports `Timber ignited` or `Timber heated · too wet to ignite` for three simulation seconds. Wetness, fuel, rain extinguishing and damage remain authoritative; a bolt does not guarantee ignition of wet material.

## Controls and valid targets

Choose a category, then a named tool. Release the selection press. Use the **right front trigger** over the world: hold for rain, wind, gifts, seeds or regrowth. Terraform charges for half a simulation second and applies on release; lightning charges for 1.2 seconds and strikes on release. Keep clear of the toolbar while using the world tool. Red reticles and the selected-tool hint explain rejected targets.

Healing requires an injured villager. Seeds use the empty outlined field plots and available seed/timber resources. Regrowth targets harvested stumps or saplings. Gifts need safe ground near villagers/stores and unreserved storage space. These conditions were preserved rather than silently bypassed or granting resources/faith for rejected attempts. Unlimited spiritual energy remains enabled for testing.

## Verification

Editor compilation and Android player-script compilation passed (43 assemblies; no APK). Native dock selection/release and immediate label visibility/layout, gift accounting/faith/hold cadence/full-store safeguards, planting/field preparation, charged lightning/thermal damage/dousing/wet resistance/save-load, and Terraform raise/lower/smooth/collider/navigation/cost/persistence checks passed. The terrain full-cycle fixture selects a patch with at least three editable grid samples, since a newly supported single-cell shoreline patch can legitimately have nothing to smooth.

A separate real palette-pointer check verified one rejection per held press, full-store notices, missing healing targets, wet-strike outcome messaging and exhausted-fuel rejection. Actual running fire particles were required above the rendered tree crown. Dock and tree ignition renders were inspected; Editor frames advanced. Original season data was restored byte-for-byte, both XR startup switches restored, and Unity left open in Edit mode. Headset interaction feel/readability and the user's exact reported failure remain physical playtest checks.

## Follow-up: red target rings

The user's red-ring-everywhere report remains under investigation; the selected tool and its exact rejection reason are needed to attribute that session. Fractional-grid ray tests against the current terrain collider found no height mismatch above two game units (maximum observed approximately 0.00014), with 74 accepted sculpt targets out of 222 mixed land/water mesh hits. These measurements use the current build and do not establish that the user's headset targets were valid.

Invalid world targets now show the actual rejection beside the ring in a small, distance-scaled, headset-facing label. It only appears while an equipped tool has a visible invalid preview. The view has no colliders, raycast targets or simulation data; it clears immediately on valid targets, tracking loss, menu/busy ownership and cancellation. Existing docking, controls and validation remain unchanged.

`GodToolTargetChecks.Verify` passed real full-store ray rejection, exact reason/layout, read-only state, collider-free input, facing/angular scaling and cleanup. A staged pointer test also transitioned to a valid green gift target with the label hidden after making storage available, without granting any gift during preview. The label render was inspected. Existing dock, provisions, planting and rejection-feedback checks passed. Editor and Android script compilation passed (43 assemblies). Original save restored byte-for-byte and XR startup restored for both targets. Physical headset confirmation and the user's exact tool/reason remain open.
