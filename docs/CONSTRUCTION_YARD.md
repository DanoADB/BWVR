# Shared construction yard

One 60 × 48 game-unit yard replaces the three separate 18 × 18 toy trays. The creek bridge remains a separate functional construction site because its physical supports open a real crossing. The six exploration clearings still hold their tools and treats; they no longer spawn toy platforms.

The shared yard has 48 physical pieces in eight shapes: 21 bricks, three beams, and four each of cubes, slabs, columns, ramps, open arches and cylinders. Loose supplies line the sides, leaving the middle for building. A single private physics scene owns the toy pieces; no global physics settings change.

Pieces snap on the existing two-unit grid with quarter-turn alignment. Tall pieces use their actual height when snapping and stacking. Ramps have convex sloped collision and do not offer a pretend flat top attachment. Arches have compound physical legs/lintel and an open pickup mesh, so their openings remain empty. Boundary checks include the rotated piece footprint; the snap search checks nearby grid coordinates instead of scanning the entire expanded foundation.

The central learned arch/bench/tower patterns remain available. The creature can also carry and place the new shapes at a small set of supported construction positions elsewhere on the same yard. These are bounded stacking opportunities, not unrestricted engineering or new general-purpose visual recognition. Villager building and repairs use the known central recipe rather than accidentally working on unrelated free building. Existing construction use, disassembly, feedback and ownership rules remain.

## Saves and migration

The main construction save now accepts 48 records, while old eight-piece saves still load. Previously attached builds from the two remote trays migrate into reserved groups of eight on the shared platform, retaining relative positions, rotations, support references and creature attribution. Loose cache pieces return to their organized supply positions. The first eight piece identities remain the old main set, followed by the two old cache sets; the final 24 supply the new shapes. Existing beachball/chime/discovery identities remain bound so saved interests do not shift to another object after expansion.

The next save captures the full yard and clears the retired cache arrays. The bridge still requires eight records. Invalid support graphs and unsupported record counts are rejected. Held pieces still resume at safe supply positions rather than in an actor's hand.

## Verification

`ConstructionYardChecks.Verify()` checks inventory and consolidation, all shape snaps, mixed-height stacking, real arch/ramp collisions, stability, save validation, old-main/cache migration, and creature pickup/carry/placement of every shape. Existing construction, building learning, discovery, villager construction/use/repair, bridge, interaction and navigation checks cover the reused systems. Headset performance and a fresh APK require separate device/build validation.

Verified in Unity 6000.6.7f1 on 2026-10-25: the shared-yard check and all 12 selected regression checks passed (construction physics, building learning, construction use, villager repairs, village building, gatherings, discoveries, bridge construction/repair, navigation, creature carry and feedback). Live frames advanced from 3,596 to 14,636. A staged render was inspected for platform placement, piece variety and scale. The final console window contained no gameplay errors and one existing XR audio fallback warning. Play was stopped and the original save restored byte-for-byte; no APK was rebuilt.
