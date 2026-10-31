using System;
using Meta.XR;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;
namespace BWVR.Editor {
public sealed class FoveationSetup:IPreprocessBuildWithReport {
 [InitializeOnLoadMethod]static void RetainLinkDefaults(){EditorApplication.delayCall+=()=>{if(!EditorApplication.isPlaying&&!BuildPipeline.isBuildingPlayer)KeepQuestLinkDefaults();};}
 public static void KeepQuestLinkDefaults(){var xr=OpenXRSettings.GetSettingsForBuildTargetGroup(BuildTargetGroup.Standalone);if(xr==null)return;foreach(var feature in xr.GetFeatures<OpenXRFeature>())if(feature is MetaXRFoveationFeature||feature is MetaXREyeTrackedFoveationFeature){SetEnabled(feature,false);AssetDatabase.SaveAssetIfDirty(feature);}}
 public int callbackOrder=>-10000;
 public void OnPreprocessBuild(BuildReport report){if(report.summary.platform==BuildTarget.Android)Configure();}
 // Run before OpenXR/Meta build validation, including builds outside our APK menu.
 [MenuItem("BWVR/Rendering/Configure standalone foveation")]
 public static void Configure(){
  if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop Play mode before configuring OpenXR");
  var xr=OpenXRSettings.GetSettingsForBuildTargetGroup(BuildTargetGroup.Android);if(xr==null)throw new BuildFailedException("Android OpenXR settings unavailable");
  if(GraphicsSettings.defaultRenderPipeline!=null)throw new BuildFailedException("BWVR foveation currently targets Built-in + Meta Legacy; review the integration after a render-pipeline migration");
  var apis=PlayerSettings.GetGraphicsAPIs(BuildTarget.Android);if(PlayerSettings.GetUseDefaultGraphicsAPIs(BuildTarget.Android)||apis.Length!=1||apis[0]!=GraphicsDeviceType.Vulkan)throw new BuildFailedException("Standalone eye-tracked foveation requires explicit Vulkan-only Android graphics; run BWVR setup");
  xr.renderMode=OpenXRSettings.RenderMode.SinglePassInstanced;xr.foveatedRenderingApi=OpenXRSettings.BackendFovationApi.Legacy;
  var fixedFeature=xr.GetFeature<MetaXRFoveationFeature>();var eyeFeature=xr.GetFeature<MetaXREyeTrackedFoveationFeature>();if(fixedFeature==null||eyeFeature==null)throw new BuildFailedException("Meta foveation features missing; resolve Meta XR Core and refresh OpenXR settings");
  // The SDK's Eye feature enabled setter targets the active build group, which can
  // be Standalone here. Serialize only the intended Android feature instead.
  SetEnabled(fixedFeature,true);SetEnabled(eyeFeature,true);var srp=xr.GetFeature<FoveatedRenderingFeature>();if(srp!=null)SetEnabled(srp,false);
  var config=OVRProjectConfig.CachedProjectConfig;if(config==null)throw new BuildFailedException("Meta project config missing; run BWVR/Rendering/Configure standalone foveation before building");config.eyeTrackingSupport=OVRProjectConfig.FeatureSupport.Supported;OVRProjectConfig.CommitProjectConfig(config);AssetDatabase.SaveAssetIfDirty(config);EditorUtility.SetDirty(xr);AssetDatabase.SaveAssetIfDirty(xr);AssetDatabase.SaveAssetIfDirty(fixedFeature);AssetDatabase.SaveAssetIfDirty(eyeFeature);if(srp!=null)AssetDatabase.SaveAssetIfDirty(srp);
  Debug.Log("BWVR_FOVEATION_SETTINGS_READY: Android Legacy + SPI/Vulkan; optional eye tracking; desktop settings untouched");
 }
 static void SetEnabled(OpenXRFeature feature,bool enabled){var serialized=new SerializedObject(feature);var state=serialized.FindProperty("m_enabled");var required=serialized.FindProperty("required");if(state.boolValue==enabled&&!required.boolValue)return;state.boolValue=enabled;required.boolValue=false;serialized.ApplyModifiedPropertiesWithoutUndo();EditorUtility.SetDirty(feature);}
}
}
