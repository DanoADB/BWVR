# God Tools category dock

The God Tools scroll is replaced by a compact, private world-space dock near the bottom of the player's view. It is collapsed and unarmed at startup, with a faint idle rail. Opening a category reveals only its tools in a second row. Selecting a tool collapses that row and highlights its category. No gameplay, energy tuning or saved preferences are changed by the layout.

| Category | Tools |
| --- | --- |
| Terraform | Raise ground, Lower ground, Smooth ground |
| Weather | Rain, Wind fan, Lightning, Whirlwind |
| Life | Plant seeds, Heal |
| Offerings | Food, Wood |
| Guidance | Help, Protect, Investigate, plus Clear cue |

The old `RainBrush` API enum remains a Rain alias, not a duplicate button. All 14 current tool choices are represented once. The dock also provides Empty Hand, Book of Creation and Sanctuary shortcuts. God-tool and category controls are circular stone disks; utility controls remain rounded rectangles. Glyphs reuse the existing attributed Game-icons.net and Lucide atlases, including the previously sourced terrain and lightning icons. Licenses remain in `Resources/GodToolIcons` and `Resources/BookIcons`. No new external artwork or model download is required.

## Controls

- Quest: point with the right controller and click its front/index trigger to expand a category or equip a tool. Release the selection press before aiming into the world and using it. The right side grip remains zoom.
- Desktop: point with the mouse and left-click, or press F over the dock. Right-mouse look and world-tool F use remain unchanged. T opens/collapses the current category; the existing Sanctuary and deliberate underside-wrist shortcut remain available.
- Tools retain their current world interactions: held rain/wind/gifts/planting, charged lightning and terrain strokes, targeted healing and placed guidance. Hovering the dock stops world channels and cancels charges/pending sculpt requests. Crossing the dock while holding a press requires release before world use resumes.
- Empty Hand clears the selected tool and returns normal pickup/interaction. Escape also clears it. Clear cue appears only in Guidance and removes the existing player-placed cue.
- Book/menu shortcuts use the real existing pages and consume the initiating trigger edge. Those pages retain their existing controls and pause/save behavior.

Category captions remain visible; individual tool names appear on hover or for the selected tool. A short use hint occupies the dock's lower edge while equipped. The rail brightens on interaction and fades to 28% after two idle seconds. The expanded row stays steady while choosing. Idle following has a small angular delay/damping, and the dock follows head translation and rig scale. Only UI transforms move; the camera is never moved by this feature.

## Input and simulation isolation

`GodToolDock` extends `GodToolPalette`, reusing its selection/release ownership, target previews, costs and tool executors. Private collider ray tests select only active category/tool/utility targets even if scenery overlaps the overlay. UI colliders are triggers on Ignore Raycast, excluded from ordinary world picking/physics. The right pointer endpoint uses the actual private UI hit while hovering. Tool selection cannot also pick up an item, activate a world object or train the creature.

Carrying, zoom, pause, menus, suspension and lost tracking dismiss/hide the dock and cancel active channels. Restoring tracking while a trigger is held cannot replay a selection. Merely looking at the dock does not expand a category. Returning the wrist to rest does not close a row the player is selecting from. The toolbar is transient presentation, not save data.

## Validation

`GodToolDockChecks.Verify()` covers category counts and sourced glyphs; ray expansion/equip; release gating; Empty Hand and ordinary input; idle fading; stable expanded pose; actual Book/menu callbacks; physics layers; blocked/tracking behavior; and no simulation mutation. `GodToolsChecks` retains existing healing, energy, guidance/save, input-ownership and wrist checks with the new category presentation. Editor checks pass for the dock, legacy god-tool effects and ownership, Sanctuary, Book, rain, wind, lightning and terrain/save behavior. Additional staged checks confirm matching apparent size and UI targeting at 0.5x, 1x and 8x rig scales, plus canceling a real pending terrain stroke when aiming back into the dock. Idle, expanded categories and equipped renders were inspected. No new shader/material warnings remain after adding the required UI texture property.

Editor validation and captured renders do not establish Quest comfort or readability. The fixed 0.85-head-unit distance, low placement, target size and idle opacity need physical Quest tuning, including supported zoom extremes and head/controller tracking. No APK or headset result is claimed for this increment.
