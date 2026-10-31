using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;
using UnityEngine.XR.Management;
using UnityEngine.XR.OpenXR;
namespace BWVR {
public enum FoveationPreference {Automatic,Fixed,Off}
public enum FoveationMode {Inactive,Waiting,Unsupported,Fixed,EyeTracked,Off,Suspended,Failed}
public interface IFoveationDevice {
 bool Eligible{get;}bool Ready{get;}bool Supported{get;}bool EyeSupported{get;}bool EyePermission{get;}bool EyeEnabled{get;}
 void RequestEyePermission();void SetEye(bool value);void SetLevel(int level,bool dynamic);
}
// Bounded capability/permission policy. No raw gaze samples or actor simulation state.
public sealed class FoveationPolicy {
 public FoveationPreference Preference=FoveationPreference.Automatic;
 public FoveationMode Mode{get;private set;}=FoveationMode.Inactive;
 public int RequestedLevel{get;private set;}=-1;
 bool requestedPermission,wasReady;bool? requestedDynamic;
 public void Reset(){wasReady=false;RequestedLevel=-1;requestedDynamic=null;}
 public void Suspend(){Reset();Mode=FoveationMode.Suspended;}
 public void Fail(){Reset();Mode=FoveationMode.Failed;}
 public void Tick(IFoveationDevice device){
  if(!device.Eligible){Mode=FoveationMode.Inactive;return;}
  if(!device.Ready){Reset();Mode=FoveationMode.Waiting;return;}
  if(!wasReady){RequestedLevel=-1;requestedDynamic=null;wasReady=true;}
  if(!device.Supported){Mode=FoveationMode.Unsupported;return;}
  bool eyeSupport=device.EyeSupported;
  if(Preference==FoveationPreference.Automatic&&eyeSupport&&!device.EyePermission&&!requestedPermission){requestedPermission=true;device.RequestEyePermission();}
  bool eyeWanted=Preference==FoveationPreference.Automatic&&eyeSupport&&device.EyePermission;
  if(eyeSupport&&device.EyeEnabled!=eyeWanted)device.SetEye(eyeWanted);
  bool eyeActive=eyeSupport&&eyeWanted&&device.EyeEnabled;
  // Conservative fixed fallback keeps the low dock/Book more legible on Quest 3.
  int level=Preference==FoveationPreference.Off?0:eyeActive?2:1;bool dynamic=level>0;
  if(level!=RequestedLevel||dynamic!=requestedDynamic){device.SetLevel(level,dynamic);RequestedLevel=level;requestedDynamic=dynamic;}
  Mode=level==0?FoveationMode.Off:eyeActive?FoveationMode.EyeTracked:FoveationMode.Fixed;
 }
}
public sealed class QuestFoveation:MonoBehaviour {
 public const string PreferenceKey="BWVR.Rendering.Foveation.v1";
 public FoveationPolicy Policy{get;private set;}public FoveationPreference Preference=>Policy?.Preference??FoveationPreference.Automatic;
 public FoveationMode Mode=>Policy?.Mode??FoveationMode.Inactive;
 public string PreferenceLabel=>Preference==FoveationPreference.Automatic?"Auto":Preference.ToString();
 IFoveationDevice device;float nextPoll;bool suspended,failed;
 public static QuestFoveation Install(GameObject owner)=>owner.GetComponent<QuestFoveation>()??owner.AddComponent<QuestFoveation>();
 void Awake(){int saved=PlayerPrefs.GetInt(PreferenceKey,0);Policy=new FoveationPolicy{Preference=Enum.IsDefined(typeof(FoveationPreference),saved)?(FoveationPreference)saved:FoveationPreference.Automatic};device=new MetaFoveationDevice();}
 public void SetPreference(FoveationPreference value){if(!Enum.IsDefined(typeof(FoveationPreference),value))return;Policy.Preference=value;Policy.Reset();failed=false;nextPoll=0;PlayerPrefs.SetInt(PreferenceKey,(int)value);PlayerPrefs.Save();}
 public void CyclePreference()=>SetPreference((FoveationPreference)(((int)Preference+1)%3));
 void Update(){if(suspended||failed||Time.realtimeSinceStartup<nextPoll)return;nextPoll=Time.realtimeSinceStartup+1;var before=Mode;
  try{Policy.Tick(device);if(before!=Mode)Debug.Log("BWVR_FOVEATION: "+Mode+"; configured level "+Policy.RequestedLevel+"; preference "+PreferenceLabel);}
  catch(Exception e){failed=true;Policy.Fail();Debug.LogWarning("BWVR_FOVEATION_UNAVAILABLE: "+e.GetType().Name+"; rendering continues without a claimed foveation result");}
 }
 void OnEnable(){failed=false;nextPoll=0;Policy?.Reset();}
 void OnApplicationPause(bool pause){suspended=pause;if(pause)Policy?.Suspend();else{Policy?.Reset();failed=false;nextPoll=0;}}
 void OnApplicationFocus(bool focus){if(focus){Policy?.Reset();failed=false;nextPoll=0;}}
 void OnDisable(){try{if(device?.Eligible==true&&device.Ready&&device.Supported){if(device.EyeSupported)device.SetEye(false);device.SetLevel(0,false);}}catch(Exception){/* XR may already be shutting down. */}Policy?.Suspend();}
}
// Uses the existing OpenXR loader; no OVRManager component or replacement camera rig.
sealed class MetaFoveationDevice:IFoveationDevice {
 readonly List<XRDisplaySubsystem> displays=new List<XRDisplaySubsystem>();
 public bool Eligible {
  get {
   #if UNITY_ANDROID && !UNITY_EDITOR
   return SystemInfo.graphicsDeviceType==GraphicsDeviceType.Vulkan;
   #else
   return false;
   #endif
  }
 }
 public bool Ready{get{if(!Eligible||XRGeneralSettings.Instance?.Manager?.activeLoader is not OpenXRLoader)return false;displays.Clear();SubsystemManager.GetSubsystems(displays);foreach(var display in displays)if(display.running)return true;return false;}}
 public bool Supported=>Ready&&OpenXRSettings.Instance.GetFeature<Meta.XR.MetaXRFoveationFeature>()?.enabled==true&&OVRManager.fixedFoveatedRenderingSupported;
 public bool EyeSupported=>OpenXRSettings.Instance.GetFeature<Meta.XR.MetaXREyeTrackedFoveationFeature>()?.enabled==true&&OVRManager.eyeTrackedFoveatedRenderingSupported;
 public bool EyeEnabled=>OVRManager.eyeTrackedFoveatedRenderingEnabled;
 public bool EyePermission {
  get {
   #if UNITY_ANDROID && !UNITY_EDITOR
   return UnityEngine.Android.Permission.HasUserAuthorizedPermission("com.oculus.permission.EYE_TRACKING");
   #else
   return false;
   #endif
  }
 }
 public void RequestEyePermission(){
  #if UNITY_ANDROID && !UNITY_EDITOR
  UnityEngine.Android.Permission.RequestUserPermission("com.oculus.permission.EYE_TRACKING");
  #endif
 }
 public void SetEye(bool value){OVRManager.eyeTrackedFoveatedRenderingEnabled=value;}
 public void SetLevel(int level,bool dynamic){OVRManager.foveatedRenderingLevel=(OVRManager.FoveatedRenderingLevel)level;OVRManager.useDynamicFoveatedRendering=dynamic;}
}
}
