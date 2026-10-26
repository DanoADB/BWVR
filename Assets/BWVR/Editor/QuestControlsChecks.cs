using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.XR.OpenXR.Features.Interactions;
namespace BWVR.Editor {
public static class QuestControlsChecks {
 static void Set<T>(InputControl<T> control,T value) where T:struct {
  using(UnityEngine.InputSystem.LowLevel.StateEvent.From(control.device,out var ev)){
   control.WriteValueIntoEvent(value,ev);InputSystem.QueueEvent(ev);
  }
  InputSystem.Update();
 }
 [MenuItem("BWVR/Validate Quest controls")]
 public static void Verify(){
  var origin=new GameObject("Temporary locomotion check");
  var head=new GameObject("Tracked head");head.transform.SetParent(origin.transform,false);
  UnityEngine.InputSystem.InputDevice device=null;
  try{
   head.transform.localPosition=new Vector3(.3f,1.5f,-.4f);
   head.transform.localRotation=Quaternion.Euler(40,90,0);
   bool latch=false;
   QuestControls.Move(origin.transform,head.transform,new Vector2(.1f,.1f),Vector2.zero,.05f,ref latch);
   if(origin.transform.position.sqrMagnitude>.000001f)throw new Exception("Stick drift moved player");
   QuestControls.Move(origin.transform,head.transform,Vector2.up,Vector2.zero,.05f,ref latch);
   if(origin.transform.position.x<.03f||Mathf.Abs(origin.transform.position.y)>.00001f)throw new Exception("Movement is not horizontal/head-relative");
   var before=head.transform.position;
   QuestControls.Move(origin.transform,head.transform,Vector2.zero,Vector2.right,.05f,ref latch);
   if(Vector3.Distance(before,head.transform.position)>.00001f)throw new Exception("Snap turn moved head");
   float yaw=origin.transform.eulerAngles.y;
   QuestControls.Move(origin.transform,head.transform,Vector2.zero,Vector2.right,.05f,ref latch);
   if(Mathf.Abs(Mathf.DeltaAngle(yaw,origin.transform.eulerAngles.y))>.001f)throw new Exception("Held stick repeated snap");
   QuestControls.Move(origin.transform,head.transform,Vector2.zero,Vector2.zero,.05f,ref latch);
   QuestControls.Move(origin.transform,head.transform,Vector2.zero,Vector2.left,.05f,ref latch);
   if(Mathf.Abs(Mathf.DeltaAngle(origin.transform.eulerAngles.y,0))>.001f)throw new Exception("Turn did not rearm");
   float y=origin.transform.position.y;
   QuestControls.Move(origin.transform,head.transform,Vector2.zero,Vector2.up,.05f,ref latch);
   if(origin.transform.position.y<=y)throw new Exception("Vertical stick travel failed");
   origin.transform.localScale=Vector3.one;
   var focus=head.transform.position+head.transform.forward*2;
   var originalPosition=origin.transform.position;var originalRotation=head.transform.rotation;
   var focusInHead=head.transform.InverseTransformPoint(focus);
   QuestControls.Zoom(origin.transform,focus,.5f);
   if(origin.transform.localScale.x>=1||Vector3.Distance(head.transform.InverseTransformPoint(focus),focusInHead)>.0001f)throw new Exception("Zoom moved gaze focus");
   if(Quaternion.Angle(head.transform.rotation,originalRotation)>.001f)throw new Exception("Zoom rotated tracked head");
   QuestControls.Zoom(origin.transform,focus,-.5f);
   if(Vector3.Distance(origin.transform.position,originalPosition)>.0001f||Mathf.Abs(origin.transform.localScale.x-1)>.0001f)throw new Exception("Zoom round trip drifted");
   for(int i=0;i<30;i++)QuestControls.Zoom(origin.transform,focus,1);
   if(Mathf.Abs(origin.transform.localScale.x-.025f)>.00001f)throw new Exception("Zoom-in limit failed");
   for(int i=0;i<30;i++)QuestControls.Zoom(origin.transform,focus,-1);
   if(Mathf.Abs(origin.transform.localScale.x-2)>.00001f)throw new Exception("Zoom-out limit failed");
   QuestSlice.PlaceView(origin.transform,head.transform,Vector3.zero);
   if(origin.transform.localScale!=Vector3.one)throw new Exception("Recenter did not reset zoom");
   InputSystem.RegisterLayout<OculusTouchControllerProfile.OculusTouchController>();
   device=InputSystem.AddDevice<OculusTouchControllerProfile.OculusTouchController>();
   InputSystem.SetDeviceUsage(device,UnityEngine.InputSystem.CommonUsages.LeftHand);
   Set(device.GetChildControl<ButtonControl>("isTracked"),1f);
   Set(device.GetChildControl<Vector3Control>("devicePosition"),new Vector3(1,2,3));
   Set(device.GetChildControl<QuaternionControl>("deviceRotation"),Quaternion.identity);
   Set(device.GetChildControl<Vector3Control>("pointerPosition"),new Vector3(4,5,6));
   Set(device.GetChildControl<QuaternionControl>("pointerRotation"),Quaternion.Euler(0,45,0));
   Set(device.GetChildControl<ButtonControl>("gripPressed"),1f);
   Set(device.GetChildControl<ButtonControl>("triggerPressed"),1f);
   Set(device.GetChildControl<ButtonControl>("primaryButton"),1f);
   Set(device.GetChildControl<Vector2Control>("thumbstick"),Vector2.up);
   InputSystem.Update();
   var input=QuestControls.Read(0);
   if(!input.tracked||!input.grip||!input.trigger||!input.menu||input.stick.y<.9f)throw new Exception("Controller button/stick binding failed");
   if(Vector3.Distance(input.position,new Vector3(1,2,3))>.001f||Vector3.Distance(input.aimPosition,new Vector3(4,5,6))>.001f||Quaternion.Angle(input.aimRotation,Quaternion.Euler(0,45,0))>.01f)throw new Exception("Grip/aim poses not separate");
   InputSystem.SetDeviceUsage(device,UnityEngine.InputSystem.CommonUsages.RightHand);
   Set(device.GetChildControl<ButtonControl>("secondaryButton"),0f);
   if(!QuestControls.Read(1).praise||QuestControls.Read(1).discourage||QuestControls.Read(1).menu)throw new Exception("Right A praise binding conflicts with menu");
   Set(device.GetChildControl<ButtonControl>("primaryButton"),0f);
   Set(device.GetChildControl<ButtonControl>("secondaryButton"),1f);
   if(QuestControls.Read(1).praise||!QuestControls.Read(1).discourage||QuestControls.Read(1).recenter)throw new Exception("Right B discourage binding conflicts with recenter");
   Set(device.GetChildControl<ButtonControl>("gripPressed"),0f);
   if(QuestControls.Read(1).zoom)throw new Exception("Front trigger or B enabled zoom");
   Set(device.GetChildControl<ButtonControl>("triggerPressed"),0f);
   Set(device.GetChildControl<ButtonControl>("gripPressed"),1f);
   if(!QuestControls.Read(1).zoom)throw new Exception("Right side grip zoom modifier binding failed");
   Set(device.GetChildControl<ButtonControl>("gripPressed"),0f);
   if(QuestControls.Read(1).zoom)throw new Exception("Released side grip left zoom enabled");
   InputSystem.SetDeviceUsage(device,UnityEngine.InputSystem.CommonUsages.LeftHand);
   Set(device.GetChildControl<ButtonControl>("isTracked"),0f);InputSystem.Update();
   if(QuestControls.Read(0).grip)throw new Exception("Tracking loss left grip pressed");
   Debug.Log("BWVR_CONTROLS_PASS: head-relative movement; drift deadzone; snap pivot/latch/rearm; elevation; grip/aim separation; carry/interact/menu/stick controls; tracking loss; right side grip zoom binding, focus stability, rotation, round trip, limits and reset");
  }finally{if(device!=null)InputSystem.RemoveDevice(device);UnityEngine.Object.DestroyImmediate(origin);}
 }
}
}
