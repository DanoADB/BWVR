# Standalone Quest foveated rendering

BWVR uses Unity 6000.6.7f1, OpenXR 1.18.0 and the Built-in Render Pipeline. Unity's [OpenXR foveation documentation](https://docs.unity3d.com/Packages/com.unity.xr.openxr@1.18/manual/features/foveatedrendering.html) distinguishes its SRP implementation from the Meta Legacy path that supports Built-in. This increment uses **Meta XR Core 207.0.1**, resolved through Meta's official scoped registry, plus Unity's required AssetBundle module. It does not migrate rendering pipelines or install a second camera rig.

## Behavior

Settings → **Foveation: Auto** cycles Auto → Fixed → Off. Preference is saved locally in PlayerPrefs, independently of the season save.

| Preference / capability | Requested configuration |
| --- | --- |
| Auto, supported eye tracking, permission granted and enable readback succeeds | Eye-tracked; dynamic level capped at Medium |
| Auto, no eye support, permission denied/revoked, or eye enable fails | Fixed; dynamic level capped at Low |
| Fixed | Eye tracking disabled; dynamic fixed foveation capped at Low |
| Off | Eye tracking disabled; foveation level Off; dynamic adjustment disabled |
| Desktop Editor / Quest Link, non-Vulkan or unsupported runtime | No foveation configuration from this controller |

The intended hardware split is Quest Pro eye-tracked rendering and a conservative fixed fallback on Quest 3. Runtime capabilities, not a headset-name whitelist, decide. [Meta's ETFR guide](https://developers.meta.com/vr/documentation/unity/unity-eye-tracked-foveated-rendering/) documents Vulkan/multiview requirements and the effect of permission or system eye-tracking settings. An eye tracker alone does not guarantee that a runtime supports this Meta-specific path.

`QuestFoveation` installs once on the existing slice root. It waits for a running OpenXR display and checks capabilities at one-second intervals. It does not create an OVRManager, OVRCameraRig, alternate input system or gameplay camera. The supported SDK static OpenXR APIs configure foveation. No raw gaze samples are read, stored or sent anywhere by BWVR.

Auto requests Android eye permission only when the runtime reports support, at most once per app session. Denial leaves the game usable with fixed foveation. A later permission grant or revoke is detected; resume/focus rechecks the runtime. SDK eye-enable readback determines whether BWVR reports EyeTracked. Unsupported configurations do not call the native foveation setters. A native API exception stops retries until focus/resume or a preference change and produces one warning rather than breaking gameplay. Shutdown cleanup is guarded against an already-destroyed XR session.

## Build integration

- Android stays Vulkan-only with single-pass instanced/multiview rendering and the Legacy foveation backend. Unity's SRP foveation feature is disabled; Meta fixed and eye-tracked foveation features are enabled as optional features.
- `FoveationSetup` runs from the project's APK setup and an early Android pre-build callback. It validates the render-pipeline/API contract, so a later migration cannot silently use the wrong backend.
- The SDK eye-feature setter can target the currently selected build group. The setup instead serializes only the intended Android feature. Newly imported Meta foveation features are disabled for Standalone without changing its graphics API or stereo mode.
- Meta's project config marks eye tracking **Supported**, never Required. `FoveationManifest` enforces the final exported Android contract: `com.oculus.permission.EYE_TRACKING` and `oculus.software.eye_tracking` with `android:required="false"`. Non-eye-tracked headsets are not excluded.
- SDK-generated per-machine developer/runtime tooling settings are ignored by Git. The Meta package and its dependencies resolve from the committed package manifest/lock; vendor package files remain in Unity's cache under their original license.

## Validation and limits

`FoveationChecks.VerifyPolicy()` uses a fake device to cover desktop isolation, missing displays, unsupported hardware, fixed fallback, one permission request, denial/grant/revoke, enable failure/readback, all preferences, resume and redundant-write suppression. `VerifySettingsAndManifest()` checks Android configuration, all desktop feature snapshots, and an idempotently patched manifest fixture. Play-mode validation checks the existing single camera/no competing manager and that desktop mode stays Inactive. Android player-script compilation passed (43 assemblies), without producing an APK.

Console state transitions use `BWVR_FOVEATION`; a configured/read-back mode does **not** prove GPU savings. Physical Quest validation remains required: permission granted/denied; system eye tracking on/off; gaze-centered quality; dock/Book readability, especially in fixed fallback; sleep/resume; and GPU timing comparing Off with Auto in equivalent island views and workloads. This affects GPU shading work, not village AI, navigation bakes or terrain-publication CPU stalls. No performance percentage or standalone comfort result is claimed from Editor checks.
