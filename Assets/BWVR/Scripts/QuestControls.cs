using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;
namespace BWVR {
// OpenXR exposes separate grip and aim poses. Keep carried objects at the grip;
// use the aim pose for selection rays instead of adding a guessed angle.
public static class QuestControls {
 public struct Hand {
  public bool tracked,grip,trigger,menu,recenter,zoom;
  public Vector2 stick;
  public Vector3 position,aimPosition;
  public Quaternion rotation,aimRotation;
 }
 public static Hand Read(int side){
  var controller=side==0?XRController.leftHand:XRController.rightHand;
  if(controller!=null){
   var h=new Hand();
   h.tracked=controller.isTracked.isPressed;
   if(!h.tracked)return h;
   h.position=controller.devicePosition.ReadValue();h.rotation=controller.deviceRotation.ReadValue();
   h.aimPosition=controller.TryGetChildControl<Vector3Control>("pointerPosition")?.ReadValue()??h.position;
   h.aimRotation=controller.TryGetChildControl<QuaternionControl>("pointerRotation")?.ReadValue()??h.rotation;
   h.stick=controller.TryGetChildControl<Vector2Control>("thumbstick")?.ReadValue()??Vector2.zero;
   h.grip=controller.TryGetChildControl<ButtonControl>("gripPressed")?.isPressed??false;
   h.trigger=controller.TryGetChildControl<ButtonControl>("triggerPressed")?.isPressed??false;
   h.menu=side==0&&((controller.TryGetChildControl<ButtonControl>("primaryButton")?.isPressed??false)||(controller.TryGetChildControl<ButtonControl>("menu")?.isPressed??false));
   h.zoom=side==1&&h.grip;
   h.recenter=side==0&&(controller.TryGetChildControl<ButtonControl>("secondaryButton")?.isPressed??false);
   return h;
  }
  return new Hand();
 }
 // Scale the tracked origin about a fixed world focus. Head rotation and FOV stay physical.
 // A smaller virtual player makes the island larger; the focus stays in the same view direction.
 public static void Zoom(Transform origin,Vector3 focus,float amount){
  if(float.IsNaN(amount)||float.IsInfinity(amount))return;
  float before=origin.localScale.x;
  float after=Mathf.Clamp(before*Mathf.Exp(-Mathf.Clamp(amount,-1,1)),.125f,2f);
  if(Mathf.Approximately(before,after))return;
  origin.position=focus+(origin.position-focus)*(after/before);
  origin.localScale=Vector3.one*after;
 }
 public static Vector2 DeadZone(Vector2 stick){
  float length=stick.magnitude;if(length<=.2f)return Vector2.zero;
  return stick.normalized*Mathf.Clamp01((length-.2f)/.8f);
 }
 public static void Move(Transform origin,Transform head,Vector2 move,Vector2 turn,float delta,ref bool turnHeld){
  var forward=Vector3.ProjectOnPlane(head.forward,Vector3.up).normalized;
  if(forward.sqrMagnitude<.01f)forward=origin.forward;
  var right=Vector3.Cross(Vector3.up,forward);
  var input=DeadZone(move);
  origin.position+=(forward*input.y+right*input.x)*(.65f*origin.localScale.x*Mathf.Min(delta,.05f));
  if(Mathf.Abs(turn.x)<.25f)turnHeld=false;
  if(Mathf.Abs(turn.x)>.7f&&!turnHeld){origin.RotateAround(head.position,Vector3.up,Mathf.Sign(turn.x)*30);turnHeld=true;}
  // Vertical travel lets the unseen player inspect the tabletop at different heights.
  float vertical=Mathf.Abs(turn.y)>.3f?Mathf.Sign(turn.y)*(Mathf.Abs(turn.y)-.3f)/.7f:0;
  origin.position+=Vector3.up*(vertical*.45f*origin.localScale.x*Mathf.Min(delta,.05f));
 }
}
}
