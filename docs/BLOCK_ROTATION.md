# Rotate player-held construction blocks

While holding a block in the **right hand**, tap **A** to turn clockwise or **B** to turn counterclockwise. Desktop uses **R / Q**. Each new press turns ninety degrees around the construction site's upright axis. Holding a button does not repeat. The green snap outline and final placement share the selected orientation.

These are contextual building controls: ordinary free-hand A/B feedback and desktop 1/2 teaching keep their current behavior. Left-hand carrying, non-block objects, simultaneous turn buttons, pause, menu/tool/zoom ownership, lost tracking and a locked site do not rotate a block. Tracking recovery, blocked input and suspension consume stale presses; release and press again to turn. Existing authored bridge pieces retain their fixed placement orientation.

## Geometry and outcomes

Pointer spans use the rotated beam/slab footprint when finding a braced gap, checking space and support, drawing the preview and dropping the piece. Beams can span north–south as well as east–west. Rotation does not bypass grounded bracing, yard bounds, occupancy or normal snap physics.

Square recipe bricks accept physically equivalent upright quarter turns. Recipe beams accept upright end-for-end turns while retaining the required span axis. Turning a held block does not place it, teach a shape, grant practice or create contribution history. Only a confirmed drop uses the existing player-placement receipt. A placed rotation persists in the existing construction record; an in-hand orientation remains transient under the normal save/carry rules.

No new art, components per block, UI panels, physics scenes or save schema are added. Existing meshes, actors, ownership and material rules remain authoritative.

## Verification

Unity Editor compilation passed without errors or warnings. Native checks passed pickup and turn press edges, reverse/simultaneous/tracking/blocked/pause guards, unchanged ownership and progress, matching north–south beam preview/drop, rotated slab cover and actual villager shelter use, confirmed player provenance, equivalent recipe orientations and native Save/Load. The rotated joint remained attached and aligned through two seconds of private PhysX simulation. Existing construction-yard, feedback and shared-build attention checks passed; the older pointer-span navigation fixture passed with other actors staged unavailable to exclude crowd interference. A staged preview render was inspected and Editor frames advanced. Android player-script compilation passed with 43 assemblies. The original save was restored byte-for-byte, desktop/Android XR startup restored, and the Editor left out of Play mode. Fixtures stage clear supports and control competing actors; physical Quest button feel, outline readability, reach and performance remain playtest gates. No APK is included.
