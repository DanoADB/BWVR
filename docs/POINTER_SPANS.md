# Build supported spans with the player pointer

S4 input-to-use fix, 2026-10-27. Slabs and beams can now bridge supports through the normal player pickup, pointer preview and release path.

## Controls

Hold a Slab or Beam with the right front trigger. Aim at the inner top rim of an upright support, toward the gap. When a valid braced placement exists, the existing green outline spans the supports. Release to place at that same position. Aiming at the centre of the support's top keeps ordinary stacking available. No new binding or permanent interface is introduced.

For the existing example shelter, use Columns at local (-2, 2.4, -12) and (2, 2.4, -12), then aim a Slab at the inner top rim of either column. The assisted roof position is (0, 5.4, -12). The resulting cover must still have clear headroom and access before villagers use it.

## Cause and implementation

The earlier shelter fixtures placed geometry through ConstructionSite.Drop directly. The player's RayPoint instead derived the piece centre from a surface contact. A gap-spanning roof centre has no surface beneath it; aiming near the column's inner edge could fall outside the ordinary 0.85-unit snap tolerance. A native pre-fix probe at local (1.1, 5.4, -12) returned surface contact but no valid snap, and DropRay failed. PlacementPoint's surface-contact boolean alone was not proof of a valid preview/drop.

A top-rim contact now considers a small bounded set of grid-aligned spans at the contacted height, only for Slab/Beam pieces in non-bridge yards. Candidates must contain the aimed contact within their footprint, fit the existing yard bounds, be empty and supported, and have grounded stable bracing on opposing sides. It reuses the shelter bracing rule rather than adding a named canopy recipe. At most nine slab candidates or three beam candidates are considered; there is no scene-wide search or recursive planner.

The same resolved target feeds PlacementPoint, the existing green PreviewRay and DropRay. Preview changes neither geometry nor saved evidence. Release uses the existing snap physics, audio and confirmed player-placement record. Ordinary centre stacking and other piece types retain their existing path. This does not make every snap a useful shelter or add general rotation controls, automatic bridge construction, new pieces or rewards.

## Verification

Unity 6000.6.7f1, FirstQuestSlice, temporary DX11 batch Editor. Final compilation passed.

PointerSpanChecks passed actual QuestSlice pickup and release with a physical downward ray at the support rim. It verifies matching preview/release positions, preview purity, native slab and beam placement, one player record per placement, useful recipe-independent roof geometry and an actual villager shelter approach/rest/place memory using that player-built roof. Centre stacking, held/missing opposing support and occupied roof space remain covered by negative checks. Save validation passes.

ShelterChecks runtime, SharedShelterChecks, PlaceContributionChecks runtime, FeedbackChecks, DevelopmentFocusChecks and CreatureCarryChecks passed. The older shelter fixture ran after releasing pre-existing resource jobs. Editor frames advanced during verification; the green spanning outline was captured and visually inspected. These are staged native input-boundary/controller checks, not physical Quest trigger tracking or a full unattended village trial.

The original user save was restored with matching SHA256, Standalone/Android XR startup flags restored, and the temporary Editor closed. No APK or headset result is claimed. The full integrated S4 scenario, unattended scheduler behavior and device gates remain open; this increment fixes a demonstrated obstacle in the actual player placement path first.
