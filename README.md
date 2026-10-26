# BWVR

A standalone Quest god-game prototype: the player is unseen, the creature is worshipped, and its learned choices shape a village's survival.

Future design: [gameplay direction](docs/GAMEPLAY_DIRECTION.md) — tools, toys and feedback, rare costly possession, and villagers who build, form relationships and develop technology independently.

Current priorities and implementation status: [roadmap](docs/ROADMAP.md). Follow the [build sequence](docs/BUILD_SEQUENCE.md) for staged delivery and validation, starting with readable creature responses and richer physical tools, then a shared gathering-place slice.

## Open and play

1. In Unity Hub, add this repository folder and open with **Unity 6000.6.7f1**.
2. Allow the pinned packages to resolve. Open `Assets/BWVR/Scenes/FirstQuestSlice.unity`.
3. Press Play. Without a headset, hold right mouse to look, use WASD to move, E to grab/release, F to select, 1/2 to praise/discourage while aiming at the creature, and Tab to toggle the sanctuary panel.

**Current development default: exploration focus.** Drought drain and the season countdown are paused, empty-bucket requests are quiet, and the creature does not automatically choose water delivery. Villagers can still work in the background; an explicit Help cue can make water assistance available. Play, social contact, building, hunger, discovery, energy recovery and the simulation clock continue. To test the full drought again, turn off **Development Focus** on the scene's QuestSlice component. This development switch does not erase or reset the saved season, and does not add headset buttons or a permanent HUD.

The generated scene contains the BWVR bootstrap. It builds the island and spatial UI at runtime. To regenerate the scene and Quest settings, select **BWVR > Set up first Quest slice**. This replaces the prototype scene, so save custom scene work elsewhere first.

**Creature responses:** a visible nearby placement or suggestion can get a curious glance; choosing a suggested activity gets a brief nod, while continuing other work gets a brief sideways response. Completion and failure have separate affect. These cues preserve autonomy and player teaching. A bounded saved outcome history supplies the future Book of Creation; see [responses and evidence](docs/CREATURE_RESPONSES.md).

## Quest Link: press Play in the headset

Install Meta Horizon Link on Windows and set it as the active OpenXR runtime in its Settings > General. Connect your Quest over USB or Air Link, then launch Quest Link inside the headset before pressing Play in Unity.

BWVR configures the Standalone OpenXR loader, DirectX 11 / multi-pass desktop rendering, and Touch / Touch Plus / Touch Pro controller profiles. Unity uses the Standalone XR settings for Editor Play mode even with Android as the active build target. Open the first slice scene and press Play; head and controller tracking use the same gameplay path as the Quest APK. The Android Meta Quest configuration remains separate (Vulkan / single-pass instanced). The desktop compatibility path avoids the sky-only output reproduced with the previous configuration. Changing desktop graphics API requires an Editor restart.

**BWVR > Quest Link > Enable headset Play mode** reapplies these desktop settings without rebuilding the scene. **BWVR > Quest Link > Use desktop preview without headset** disables desktop XR initialization and keeps keyboard/mouse preview available; re-enable headset mode before the next Link session. Stop Play mode before changing modes, and restart Play after connecting Link.

The island is placed in front of your first tracked headset pose, so your room position and starting direction do not leave the village behind you. Placement runs once; moving or turning afterward is unrestricted. To reset the view during Play mode, use **BWVR > Quest Link > Recenter island in front of headset**. This rotates/translates the virtual origin without overriding head pitch or roll.

If Unity shows only the desktop view, confirm Quest Link is actually running in the headset, Meta is the active OpenXR runtime, and desktop initialization is enabled. USB debugging alone connects Android deployment, not Link. Link runs the game on the PC; use the APK to evaluate standalone Quest performance.

Reference: [Meta Link development workflow](https://developers.meta.com/vr/documentation/unity/unity-link/).

## Quest controls

- Hold **right SIDE trigger (grip) + right stick up/down** to zoom in/out around the island under your gaze (0.5×–8×). Release the side trigger for normal height adjustment. The front/index trigger does not zoom. **Y** resets zoom and recenters; desktop mouse wheel also zooms. Zoom pauses while carrying an object.
- Left stick: move forward/back/sideways relative to your head. Right stick left/right: 30-degree snap turn; center the stick before turning again. Right stick up/down: rise/lower above the tabletop.
- Island traversal uses faster defaults: horizontal/vertical stick travel is 4× the earlier rate, right-grip stick zoom is 2.5×, and desktop wheel zoom is 2×. Partial stick deflection still provides fine movement; zoom limits and snap turns stay the same. Desktop WASD uses the same horizontal speed as the sticks.
- Point a controller at a resource or villager. Hold the **right front/index trigger** or **left side grip** to carry; release while pointing onto the island to place. Rays use the OpenXR aim pose; held objects use the grip pose.
- Release the blue water offering over the blue village cistern to deliver water.
- Aim at the creature and tap right A/B to praise/discourage its last eligible completed action. The front-trigger interaction also praises. Food satisfies hunger; it does not teach an unrelated action.
- Summon Help, Protect, Investigate, Rain or Healing from the hidden God tools palette, then aim and press to use. Behavior/action cues are player-issued; the five old intention objects are no longer pre-placed beside the cistern. Guidance remains temporary influence, rain targets the water system, and healing targets an injured villager (ankle wrap).
- Press X (or the left controller menu button) to show/hide the wrist sanctuary. Press Y to recenter the island ahead. Point and trigger to pause, save, resume or begin again.
- Locomotion is user-controlled; no forced camera travel after the initial placement. The right stick changes viewing height, with ground clearance.

Routine labels and meters are hidden by default. Open the wrist sanctuary (left menu / desktop Tab) and choose **Help / details** for controls, resource numbers and the optional creature intent caption. The world communicates drought through water levels, bucket demonstrations, gestures and ground/crop color. The wrist sanctuary starts closed, including restored seasons, which resume automatically. Open it with X / left menu / desktop Tab.

**Summonable god tools:** turn the underside of the left wrist toward you while looking at it, or open X → God tools. Desktop: T. Select with the right front trigger (desktop F), release, then aim and press again to cast or place a guidance sign. Clear sign/dismiss are in the palette; desktop Escape cancels the selected tool. See [god tools](docs/GOD_TOOLS.md) for targets, costs and validation. The wrist gesture still needs hardware confirmation.

Controllers are the first input target. Tracked-hand gestures are not implemented yet.

## Quest build

In Unity Hub, add **Android Build Support**, including Android SDK/NDK and OpenJDK, to this editor version. Then select **BWVR > Build Quest APK**. Output: `Builds/BWVR-Quest.apk`. The initial setup sets ARM64 / IL2CPP, Vulkan and linear color, and assigns the Android OpenXR loader. Single-pass rendering, Touch controller and Meta Quest features are enabled when Android Build Support is available; setup logs `BWVR_ANDROID_PENDING` otherwise. Rerun setup after installing the module. Android OpenXR starts automatically. Desktop Play mode also initializes OpenXR for Quest Link; use the desktop preview menu below when testing without a headset.

Android Build Support, SDK and NDK are available on the setup machine. Unity-provided OpenJDK 17 was downloaded into ignored `.local/jdk` after the initial build reported a missing Java installation. The build helper uses that local JDK when present; other machines should install OpenJDK through Unity Hub. A compiled editor project does not establish headset compatibility. Install and test on Quest Pro and Quest 3 before treating this as a verified Quest build.

## First slice

The purchased Animal Friends gorilla is the preferred local creature when imported. See [gorilla setup](docs/ANIMAL_FRIENDS.md). Purchased artwork stays out of the public repository; the import tools and runtime integration are tracked.

A three-minute dry season with one autonomous creature, three independent villagers, physical toys and construction, contextual teaching, indirect guidance and limited rain/healing miracles. The player remains unseen. Quaternius art catalogs provide the settlement and villagers; imported local creature artwork supplies the god. The original terrain importer remains a local reference workflow. See [art setup](docs/GAME_ART.md).

The creature's water, food, toy, social, observation and construction actions now share one utility comparison and one movement/carry owner. It can inspect tagged benches and a cairn, then try bench-like spans and towers on empty trays. A tagged shoreline gap provides one useful physical bridge opportunity: completed supports and span open native villager routes; dismantling closes them. This is a small authored pattern vocabulary, not general visual recognition or unrestricted architecture.

Villagers can also fetch an available toy or berry, carry it near the creature, set it down and invite a shared experience. The creature weighs that invitation against its other needs. Completed play or eating improves that individual relationship; ignored invitations cause a longer wait, without a trust penalty. Teaching after a shared action can reinforce the creature's preference toward its actual inviting partner. Reservations and carry cleanup protect player-held objects and other villagers' discoveries.

The large striped beachball adds physical throwing, bouncing and consequences: houses/trees can be damaged and villagers can be injured, with nearby negative reactions and saved caution. Individual toys/tools develop saved appeal alongside behavior preferences, while repetition encourages variety. Construction has less automatic priority; villagers can use completed forms and gesture toward a need for a gathering place. Communal fruit tributes require a strongly affectionate or fearful majority and remain separate from personal gifts. See [play and village response](docs/PLAY_AND_VILLAGE_RESPONSE.md) for the exact rules and prototype limits.

Saves preserve resources, learned category/context preferences, relationships, injury, spiritual energy, guidance, a logical intention, discovery memories and physical construction. Reload resumes automatically and revalidates a saved intention from a safe action boundary; held objects and exact animation poses are not resumed. Loose tool positions and the transient 48-event journal are not saved. See [hybrid audit and validation](docs/HYBRID_IMPLEMENTATION_AUDIT.md).

## Development

Use `tools/unity.ps1 -Action Setup`, `-Action Validate`, `-Action Smoke`, or `-Action BuildQuest` from PowerShell. Logs go to ignored `.local/`. Commit `Assets` (including `.meta` files), `Packages` and `ProjectSettings`. Never commit Library, local game files, signing keys or build outputs. Gameplay and UI direction: [VR slice design](docs/VR_SLICE.md).

## Verified locally

Unity setup and rules validation passed. Play Mode created and rendered the scene. An ARM64 development APK was built successfully at `Builds/BWVR-Quest.apk` (build output stays local). OpenXR/Meta Quest Android settings are enabled. No USB headset was attached; controller operation, comfort and performance on Quest remain untested. Unity emitted a search-index startup exception during automated editor startup; the smoke runner records that known editor-only failure separately and still fails on game/runtime errors.

Creature–villager relationships now retain individual affection/confidence and recent frightening encounters. A creature that has enjoyed gentle rallies can invite an available playmate, favor familiar partners, and learn from correctly attributed partner feedback. See the remembered-playmates section in [play and village response](docs/PLAY_AND_VILLAGE_RESPONSE.md).

Villagers also develop saved peer familiarity through independent two-villager beachball games. Preferred friends compete with distance and availability; witnessed harmful peer play causes individual caution toward its source. These relationships are separate from creature trust and worship.

Nearby idle villagers can now watch visible ball exchanges, learn confidence from their outcomes, and become more inclined to try shared play. Practice is remembered separately from observation; a learner can later become another villager's example. Occlusion, facing, watching time, pause and carrying gate observation.

Villagers remember construction they have used and can autonomously carry loose pieces back to repair a partly intact tray structure. Repairs share the existing navigation, carry and physical snapping systems, respect player grabs and reservations, and save site-specific experience. Villagers also remember successful creek crossings and can repair a damaged span from the bank. House repairs remain future work; see docs/PLAY_AND_VILLAGE_RESPONSE.md for the tested scope.

Play fetch with the creature: hold the beachball with the right front trigger (or left grip), swing and release to toss. It can choose to carry the ball back to the temporary play circle; aim at the creature and use A/B praise and discouragement after a completed return teach its saved fetching preference. Slow releases still place objects, and right grip still zooms. Desktop: carry with E, hold Space and release E to toss.

Villagers can now turn a request for a gathering place into cooperative construction: they take turns carrying and snapping a three-piece bench together on an existing building green, retain partial progress across saves, and use the result to rest. The creature can contribute once it knows the form. This is the first village crafting capability; autonomous house expansion remains later work.

The island now has interacting rain, wind, fog, lightning, fire and coastal storm waves. Weather and changed material state save with the season; rain cools burning fuel and downwind heat can spread fires. See [weather and material interactions](docs/WEATHER_AND_MATERIALS.md) for budgets, development controls and prototype limits.

Construction toys now share one large yard with 48 pieces in eight shapes. Old tray assemblies migrate into it; the functional creek bridge remains separate. See [shared construction yard](docs/CONSTRUCTION_YARD.md).

Villagers now cut finite timber sources, harvest existing wheat, carry reserved loads into a visible shared store, and consume stored food. See [resource gathering](docs/RESOURCE_GATHERING.md) for persistence rules and the remaining forestry/farming roadmap.

Exploration observation can temporarily pause villager food work and meal consumption without changing stored supplies. Children are larger and use normal pickup/drop. See [village observation](docs/VILLAGE_OBSERVATION.md).
