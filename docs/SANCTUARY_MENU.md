# Sanctuary menu

The wrist sanctuary now shares the God Tools' existing CC0 parchment scroll, warm ink, pale captions and raised stone-disk appearance. Home shows four large controls: God tools, Book of Creation, Pause/Resume and Save. Settings contains Sound, Help/details, spirit visibility and Begin again. Beginning again requires a separate confirmation; cancel leaves the season untouched.

## Controls and behavior

- Press left X (desktop Tab) to open/close. Opening always returns to Home. The sanctuary stays hidden by default and refuses to open while an object is held.
- Point with the right controller and press the **front trigger** once to select. Desktop uses F. The side grip remains zoom. Held or untracked trigger input cannot activate a menu control repeatedly.
- God tools closes the sanctuary and transfers to the existing tool palette. Opening the sanctuary cancels any selected tool. The underside-wrist shortcut remains available.
- Sound returns to Settings; Book returns to Home. Pause is explicit; opening a menu does not stop the simulation. Save gives a brief acknowledgment only after a successful manual save. Autosave remains unchanged.
- When open, the sanctuary consumes both hands' pointer input, blocks world pickup/training and selects only active menu colliders. Scenery cannot intercept visible menu targets. Closing consumes that frame's trigger edge, preventing click-through.

## Implementation

`QuestSliceSanctuary` adapts the existing runtime uGUI/spatial UI instead of adding a second event/input system. Physical trigger colliders and `SpatialTarget` callbacks remain the activation path; an EventSystem/GraphicRaycaster is intentionally not added. Active private menu colliders are cached once and queried only while open. New page labels use one shared TextMeshPro font atlas/material. Existing Book/audio text remains compatible with its current checks.

The stone disk UI shader uses the existing God Tools' stone colors, grain, bevel lighting and inlay style without adding decorative physics or mesh objects per button. Parchment, stone controls and font render as private overlays with stereo shader support. A saved font-material reference retains the mobile overlay shader in builds; it does not depend on runtime Shader.Find. Imported official TMP Essentials include their bundled font licenses. The scroll uses the already recorded God Tools artwork/license; no additional downloaded artwork.

## Verification

`SanctuaryChecks.Verify()` exercises hidden startup, page routing, native scoped ray targeting through a staged world occluder, tracked press-edge activation, held-input suppression, pause/resume, off-scroll world isolation, pickup/training blocking, canceling restart, save acknowledgment, tool handoff, held-object protection and new caption bounds. `CreationBookChecks.VerifyModel/VerifyRuntime` cover existing read-only pages and navigation. Actual Unity renders of Home, Settings, Sound, Help and Book are inspected for layering and readability. Compilation and applicable pickup/fetch regression checks are run for this increment.

These are Editor checks with staged pointer/controller state; physical Quest readability, wrist comfort, stereo appearance and performance remain to be tested. No APK is built. The original user season is backed up before testing and restored afterward; XR startup settings are restored too.

Next: headset playtest the menu and bowling slice, then choose the next playful activity from demonstrated tricks, tug/chase or social chaos. Benches/shelters remain deferred.

The fetch regression fixture now explicitly aims away from bowling: a reproduced lane-aligned throw correctly entered bowling instead of reserving fetch. This fixture correction leaves runtime throw routing unchanged; the full menu/Book/pickup/fetch check sequence passes.

## Rounded rectangular controls (2026-10-29)

Per user direction, Book/Sanctuary buttons now use straight-edged rounded rectangles instead of stretched circular disks. The shared stone shader derives physical aspect from UV/world derivatives so corner radii remain consistent on wide buttons and tilted VR panels. Stone texture, pointer targets, hover, tracking gates and menu ownership are retained. God Tool choices retain their circular raised-stone icons, as clarified by the user. The palette's Clear cue/Close buttons and all Book/Sanctuary buttons use rounded rectangles. World-placed emblems retain the existing circular presentation. Book/Sanctuary/God Tools views were rendered and reviewed; menu/tool interaction checks pass in desktop Play mode. Headset readability remains unverified.

## Uniform menu sizing (2026-10-29)

All 33 non-God-Tool menu buttons now share the same 240-by-60 UI-unit rectangle and matching physical hit target, including Sanctuary, settings, sound, Book navigation, sections and outing actions. Text uses shared padding and bounded fitting for longer captions. The Book uses a Sections chooser instead of five undersized tabs; each reading page has standard-sized actions, previous/next and sections/back controls. God Tool icon sizes and circular styling are unchanged. Desktop checks verified all button/hit sizes, tracked section selection, Book/menu/outing input and page layout; Sanctuary, Book and chooser renders were inspected. Quest comfort/readability remains unverified.
