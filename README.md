# BWVR

A standalone Quest god-game prototype: the player is unseen, the creature is worshipped, and its learned choices shape a village's survival.

Future design: [gameplay direction](docs/GAMEPLAY_DIRECTION.md) — tools, toys and feedback, rare costly possession, and villagers who build, form relationships and develop technology independently.

## Open and play

1. In Unity Hub, add this repository folder and open with **Unity 6000.6.7f1**.
2. Allow the pinned packages to resolve. Open `Assets/BWVR/Scenes/FirstQuestSlice.unity`.
3. Press Play. Without a headset, hold right mouse to look, use WASD to move, E to grab/release, F to select or encourage the creature, and Tab to toggle the sanctuary panel.

The generated scene contains the BWVR bootstrap. It builds the island and spatial UI at runtime. To regenerate the scene and Quest settings, select **BWVR > Set up first Quest slice**. This replaces the prototype scene, so save custom scene work elsewhere first.

## Quest Link: press Play in the headset

Install Meta Horizon Link on Windows and set it as the active OpenXR runtime in its Settings > General. Connect your Quest over USB or Air Link, then launch Quest Link inside the headset before pressing Play in Unity.

BWVR configures the Standalone OpenXR loader, single-pass instanced rendering, and Touch / Touch Plus / Touch Pro controller profiles. Unity uses the Standalone XR settings for Editor Play mode even with Android as the active build target. Open the first slice scene and press Play; head and controller tracking use the same gameplay path as the Quest APK. The Android Meta Quest configuration remains separate.

**BWVR > Quest Link > Enable headset Play mode** reapplies these desktop settings without rebuilding the scene. **BWVR > Quest Link > Use desktop preview without headset** disables desktop XR initialization and keeps keyboard/mouse preview available; re-enable headset mode before the next Link session. Stop Play mode before changing modes, and restart Play after connecting Link.

If Unity shows only the desktop view, confirm Quest Link is actually running in the headset, Meta is the active OpenXR runtime, and desktop initialization is enabled. USB debugging alone connects Android deployment, not Link. Link runs the game on the PC; use the APK to evaluate standalone Quest performance.

Reference: [Meta Link development workflow](https://developers.meta.com/vr/documentation/unity/unity-link/).

## Quest controls

- Point a controller at a resource or villager. Hold grip to carry; release while pointing onto the island to place.
- Release the blue water offering over the blue village cistern to deliver water.
- Point at the creature and squeeze trigger to encourage care. Food placed near the creature also encourages it.
- Press the left controller menu button to show/hide the wrist sanctuary. Point and trigger to pause, save, resume or begin again.
- The player stays outside the island. No forced camera movement or smooth locomotion in VR.

Routine labels and meters are hidden by default. Open the wrist sanctuary (left menu / desktop Tab) and choose **Help / details** for controls, resource numbers and the optional creature intent caption. The world communicates drought through water levels, bucket demonstrations, gestures and ground/crop color. A restored paused season opens the sanctuary for resume.

Controllers are the first input target. Tracked-hand gestures are not implemented yet.

## Quest build

In Unity Hub, add **Android Build Support**, including Android SDK/NDK and OpenJDK, to this editor version. Then select **BWVR > Build Quest APK**. Output: `Builds/BWVR-Quest.apk`. The initial setup sets ARM64 / IL2CPP, Vulkan and linear color, and assigns the Android OpenXR loader. Single-pass rendering, Touch controller and Meta Quest features are enabled when Android Build Support is available; setup logs `BWVR_ANDROID_PENDING` otherwise. Rerun setup after installing the module. Android OpenXR starts automatically. Desktop Play mode also initializes OpenXR for Quest Link; use the desktop preview menu below when testing without a headset.

Android Build Support, SDK and NDK are available on the setup machine. Unity-provided OpenJDK 17 was downloaded into ignored `.local/jdk` after the initial build reported a missing Java installation. The build helper uses that local JDK when present; other machines should install OpenJDK through Unity Hub. A compiled editor project does not establish headset compatibility. Install and test on Quest Pro and Quest 3 before treating this as a verified Quest build.

## First slice

A three-minute dry season, tabletop village, three moving villagers, a placeholder creature, carryable offerings, encouragement, water delivery, a wrist sanctuary and local season save/resume. Saves preserve season resources and learned care; object placement and in-flight creature travel are not yet persistent. Reload restores paused. The prototype includes a local original-game-art importer for the cow, village, villagers, trees and grass. See [local artwork setup](docs/GAME_ART.md). Without a locally generated catalog it falls back to primitive art; copied artwork is excluded from Git.

## Development

Use `tools/unity.ps1 -Action Setup`, `-Action Validate`, `-Action Smoke`, or `-Action BuildQuest` from PowerShell. Logs go to ignored `.local/`. Commit `Assets` (including `.meta` files), `Packages` and `ProjectSettings`. Never commit Library, local game files, signing keys or build outputs. Gameplay and UI direction: [VR slice design](docs/VR_SLICE.md).

## Verified locally

Unity setup and rules validation passed. Play Mode created and rendered the scene. An ARM64 development APK was built successfully at `Builds/BWVR-Quest.apk` (build output stays local). OpenXR/Meta Quest Android settings are enabled. No USB headset was attached; controller operation, comfort and performance on Quest remain untested. Unity emitted a search-index startup exception during automated editor startup; the smoke runner records that known editor-only failure separately and still fails on game/runtime errors.
