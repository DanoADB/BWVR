using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.XR.Management;
using UnityEditor.XR.Management.Metadata;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.Management;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features.Interactions;
using UnityEngine.XR.OpenXR.Features.MetaQuestSupport;
namespace BWVR.Editor {
public static class ProjectSetup {
 const string Scene="Assets/BWVR/Scenes/FirstQuestSlice.unity";
 [MenuItem("BWVR/Set up first Quest slice")]
 public static void Setup(){
  PlayerSettings.companyName="DanoADB";PlayerSettings.productName="BWVR";PlayerSettings.bundleVersion="0.1.0";
  PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android,"com.danoadb.bwvr");PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android,ScriptingImplementation.IL2CPP);PlayerSettings.Android.targetArchitectures=AndroidArchitecture.ARM64;PlayerSettings.Android.minSdkVersion=AndroidSdkVersions.AndroidApiLevel32;
  PlayerSettings.colorSpace=ColorSpace.Linear;PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android,false);PlayerSettings.SetGraphicsAPIs(BuildTarget.Android,new[]{GraphicsDeviceType.Vulkan});
  PlayerSettings.defaultInterfaceOrientation=UIOrientation.LandscapeLeft;PlayerSettings.runInBackground=false;
  var settings=AssetDatabase.LoadAssetAtPath<XRGeneralSettingsPerBuildTarget>("Assets/BWVR/QuestXRSettings.asset");
  if(settings==null){settings=ScriptableObject.CreateInstance<XRGeneralSettingsPerBuildTarget>();AssetDatabase.CreateAsset(settings,"Assets/BWVR/QuestXRSettings.asset");}
  EditorBuildSettings.AddConfigObject(XRGeneralSettings.k_SettingsKey,settings,true);
  if(!settings.HasManagerSettingsForBuildTarget(BuildTargetGroup.Android))settings.CreateDefaultManagerSettingsForBuildTarget(BuildTargetGroup.Android);
  var android=settings.SettingsForBuildTarget(BuildTargetGroup.Android);android.InitManagerOnStart=true;
  if(!XRPackageMetadataStore.AssignLoader(android.Manager,"UnityEngine.XR.OpenXR.OpenXRLoader",BuildTargetGroup.Android))throw new Exception("OpenXR loader assignment failed");
  ConfigureOpenXR();EditorUtility.SetDirty(settings);
  // Desktop preview deliberately starts without an XR loader; the Android player initializes OpenXR.
  var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);new GameObject("BWVR - First Quest Slice").AddComponent<QuestSlice>();EditorSceneManager.SaveScene(scene,Scene);EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(Scene,true)};
  AssetDatabase.SaveAssets();Validate();Debug.Log("BWVR_SETUP_OK");
 }
 static bool ConfigureOpenXR(){
  var xr=OpenXRSettings.GetSettingsForBuildTargetGroup(BuildTargetGroup.Android);
  if(xr==null){Debug.LogWarning("BWVR_ANDROID_PENDING: Android Build Support is missing. Install it and rerun BWVR setup to enable Quest features.");return false;}
  xr.renderMode=OpenXRSettings.RenderMode.SinglePassInstanced;
  var touch=xr.GetFeature<OculusTouchControllerProfile>();if(touch==null)throw new Exception("Touch controller profile missing");touch.enabled=true;
  var quest=xr.GetFeature<MetaQuestFeature>();if(quest==null)throw new Exception("Meta Quest feature missing");quest.enabled=true;EditorUtility.SetDirty(xr);AssetDatabase.SaveAssets();return true;
 }
 [MenuItem("BWVR/Validate slice")]
 public static void Validate(){var s=new SliceState();s.Step(30);if(Mathf.Abs(s.seconds-150)>.001f)throw new Exception("Season timer failed");s.paused=true;float water=s.water;s.Step(10);if(s.water!=water)throw new Exception("Paused water changed");s.paused=false;s.Deliver();if(s.gifts!=1||s.water<=water)throw new Exception("Water offering failed");s.Praise();var copy=JsonUtility.FromJson<SliceState>(JsonUtility.ToJson(s));if(!copy.Valid()||copy.gifts!=s.gifts||copy.care!=s.care)throw new Exception("Save round trip failed");copy.water=float.NaN;if(copy.Valid())throw new Exception("Invalid save accepted");Debug.Log("BWVR_RULES_OK: timer, pause, offering, teaching and save validation");}
 public static void Smoke(){
  EditorSceneManager.OpenScene(Scene);EditorSettings.enterPlayModeOptionsEnabled=true;EditorSettings.enterPlayModeOptions=EnterPlayModeOptions.DisableDomainReload;
  int frames=0;bool done=false;
  Application.logMessageReceived+=(message,stack,type)=>{if(stack.Contains("UnityEditor.Search.SearchDatabase")){Debug.LogWarning("Known Unity editor search-index startup error; gameplay smoke checks continue.");return;}if(!done&&(type==LogType.Exception||type==LogType.Error)){done=true;Debug.Log("BWVR_SMOKE_FAILED: "+message+"\n"+stack);EditorApplication.Exit(1);}};
  EditorApplication.update+=()=>{
   if(done||!EditorApplication.isPlaying)return;if(++frames<120)return;done=true;
   try{
    var slice=UnityEngine.Object.FindAnyObjectByType<QuestSlice>();if(slice==null||Camera.main==null)throw new Exception("Scene bootstrap or camera missing");
    if(UnityEngine.Object.FindObjectsByType<CarryItem>().Length!=6)throw new Exception("Expected three workers and three offerings");
    Validate();ValidateRelease(slice);
    var camera=Camera.main;camera.transform.localRotation=Quaternion.Euler(25,0,0);
    var rt=new RenderTexture(1440,1000,24);camera.targetTexture=rt;camera.Render();RenderTexture.active=rt;
    var texture=new Texture2D(1440,1000,TextureFormat.RGB24,false);texture.ReadPixels(new Rect(0,0,1440,1000),0,0);texture.Apply();Directory.CreateDirectory(".local");File.WriteAllBytes(".local/first-slice.png",texture.EncodeToPNG());camera.targetTexture=null;RenderTexture.active=null;
    Debug.Log("BWVR_PLAYMODE_OK: runtime scene, offerings, camera, rules and rendered preview");EditorApplication.Exit(0);
   }catch(Exception error){Debug.LogException(error);EditorApplication.Exit(1);}
  };EditorApplication.isPlaying=true;
 }
 static void ValidateRelease(QuestSlice slice){
  const System.Reflection.BindingFlags flags=System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic;
  var stateField=typeof(QuestSlice).GetField("state",flags);var state=(SliceState)stateField.GetValue(slice);var backup=JsonUtility.ToJson(state);
  var hands=(CarryItem[])typeof(QuestSlice).GetField("held",flags).GetValue(slice);var release=typeof(QuestSlice).GetMethod("Release",flags);
  var offerings=UnityEngine.Object.FindObjectsByType<CarryItem>();var water=Array.Find(offerings,item=>item.kind==Offering.Water);var villager=Array.Find(offerings,item=>item.kind==Offering.Villager);
  state.seconds=180;int gifts=state.gifts;hands[1]=water;water.held=true;water.GetComponent<Collider>().enabled=false;
  release.Invoke(slice,new object[]{1,new Ray((Vector3)typeof(QuestSlice).GetField("cistern",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).GetValue(slice)+Vector3.up,Vector3.down)});
  if(state.gifts!=gifts+1||hands[1]!=null||water.held||!water.GetComponent<Collider>().enabled)throw new Exception("Water delivery/release failed");
  hands[1]=villager;villager.held=true;villager.GetComponent<Collider>().enabled=false;
  release.Invoke(slice,new object[]{1,new Ray(new Vector3(4,2,4),Vector3.down)});
  if(villager.transform.position!=villager.home||villager.held||!villager.GetComponent<Collider>().enabled)throw new Exception("Unsafe placement did not return villager safely");
  stateField.SetValue(slice,JsonUtility.FromJson<SliceState>(backup));Debug.Log("BWVR_RELEASE_OK: actual water delivery and unsafe villager return");
 }
 [MenuItem("BWVR/Build Quest APK")]
 public static void BuildQuest(){
 if(Directory.Exists(".local/jdk/bin"))UnityEditor.Android.AndroidExternalToolsSettings.jdkRootPath=Path.GetFullPath(".local/jdk");
if(!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Android,BuildTarget.Android))throw new Exception("Install Android Build Support, SDK/NDK and OpenJDK for Unity 6000.6.7f1 in Unity Hub.");if(!ConfigureOpenXR())throw new Exception("Quest OpenXR settings unavailable");Directory.CreateDirectory("Builds");var result=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{Scene},locationPathName="Builds/BWVR-Quest.apk",target=BuildTarget.Android,options=BuildOptions.Development});if(result.summary.result!=BuildResult.Succeeded)throw new Exception("Quest build failed: "+result.summary.result);}
}
}
