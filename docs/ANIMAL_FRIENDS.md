# Animal Friends gorilla

BWVR prefers the purchased Animal Friends gorilla when its generated local prefab exists. The previous Puglin and original/prototype artwork remain fallback paths. Creature height retains the existing island-relative scale and 10% actor increase. Saves contain behavior and village state, not the visual model, so changing artwork does not reset progress.

Source: [Animal Friends: Wild Animals (7 Characters), Jose (Dogzerx) Díaz](https://assetstore.unity.com/packages/3d/characters/animal-friends-wild-animals-7-characters-123115), purchased separately under the Unity Asset Store EULA. Source meshes, textures, authored animations and generated prefabs/controllers remain under ignored `Assets/LocalLicensedArt/`. The original package import folder is also ignored. Do not commit purchased artwork to the public repository.

## Prepare on a licensed development machine

1. Download the purchased package through Unity Package Manager's My Assets.
2. Run `python tools/prepare-animal-friends.py "FULL_PATH_TO_PURCHASED.unitypackage"` from the repository. This extracts only the Gorilla subtree into the ignored local source folder and preserves its Unity metadata. It does not install package scripts, sample scenes or the other six characters.
3. Stop Play mode and select **BWVR > Import purchased Animal Friends gorilla**.
4. Run **BWVR > Verify purchased gorilla artwork**, then enter `FirstQuestSlice` Play mode.

The importer builds a separate gorilla controller from BWVR's shared humanoid controller. Authored gorilla idle and in-place walk replace the locomotion poses; authored Success and Failure animate feedback, including the package's facial bones. Praise/discouragement do not override an active carry or activity. Existing pickup/drink/activity clips and hand IK remain in use, with root motion disabled so navigation owns movement. The material conversion uses BWVR's existing Quest surface shader and source color textures.

This package's face is bone-animated; the imported mesh has no facial blend shapes. Its authored reactions are available, but the importer does not claim a complete procedural emotional face or a dedicated authored carry/drink/throw clip. Headset appearance, hand alignment and standalone cost require device testing.

## Validation on the development machine

GorillaChecks passed the humanoid avatar, feet/height normalization, in-place idle/walk selection, facial animation curves, pickup/drink/IK wiring and material checks. A rendered preview caught and corrected the source color-alpha/gloss versus cutout distinction: body and eye color are opaque, while the eye highlight retains cutout. The final Unity preview was visually inspected.

SliceRuntimeChecks passed pickup, drinking, carrying, delivery, terrain/actor scale and hand reach using the actual gorilla. PlayerFetchChecks and FeedbackChecks passed. CuriosityChecks passed after isolating a fresh season with willing residents; the current saved residents were ineligible for its carrying fixture. Explicit Animator advancement confirmed moving walk poses, authored success/failure transitions, eyelid movement and suppression of feedback animation while carrying. The normal Editor frame counter also advanced between observations. These checks do not verify headset comfort, hand feel or standalone Quest performance. No APK was built.
