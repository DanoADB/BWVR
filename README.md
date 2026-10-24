# BWVR

A standalone Quest god-game prototype: the player is unseen, the creature is worshipped, and its learned choices shape a village's survival.

## Open and play

1. In Unity Hub, add this repository folder and open with **Unity 6000.6.7f1**.
2. Allow the pinned packages to resolve. Open `Assets/BWVR/Scenes/FirstQuestSlice.unity`.
3. Press Play. Without a headset, hold right mouse to look, use WASD to move, E to grab/release, F to select or encourage the creature, and Tab to toggle the sanctuary panel.

The generated scene contains the BWVR bootstrap. It builds the island and spatial UI at runtime. To regenerate the scene and Quest settings, select **BWVR > Set up first Quest slice**. This replaces the prototype scene, so save custom scene work elsewhere first.

## Quest controls

- Point a controller at a resource or villager. Hold grip to carry; release while pointing onto the island to place.
- Release the blue water offering over the blue village cistern to deliver water.
- Point at the creature and squeeze trigger to encourage care. Food placed near the creature also encourages it.
- Press the left controller menu button to show/hide the wrist sanctuary. Point and trigger to pause, save, resume or begin again.
- The player stays outside the island. No forced camera movement or smooth locomotion in VR.

Controllers are the first input target. Tracked-hand gestures are not implemented yet.

## Quest build

In Unity Hub, add **Android Build Support**, including Android SDK/NDK and OpenJDK, to this editor version. Then select **BWVR > Build Quest APK**. Output: `Builds/BWVR-Quest.apk`. The initial setup sets ARM64 / IL2CPP, Vulkan and linear color, and assigns the Android OpenXR loader. Single-pass rendering, Touch controller and Meta Quest features are enabled when Android Build Support is available; setup logs `BWVR_ANDROID_PENDING` otherwise. Rerun setup after installing the module. Android OpenXR starts automatically; desktop preview does not require a PC XR runtime.

The local setup machine currently lacks Android Build Support. A compiled editor project does not establish headset compatibility. Install and test on Quest Pro and Quest 3 before treating this as a verified Quest build.

## First slice

A three-minute dry season, tabletop village, three moving villagers, a placeholder creature, carryable offerings, encouragement, water delivery, a wrist sanctuary and local season save/resume. Saves preserve season resources and learned care; object placement and in-flight creature travel are not yet persistent. Reload restores paused. The prototype uses original primitive art; copied Black & White assets are not included.

## Development

Use `tools/unity.ps1 -Action Setup`, `-Action Validate`, or `-Action BuildQuest` from PowerShell. Logs go to ignored `.local/`. Commit `Assets` (including `.meta` files), `Packages` and `ProjectSettings`. Never commit Library, local game files, signing keys or build outputs. Gameplay and UI direction: [VR slice design](docs/VR_SLICE.md).
