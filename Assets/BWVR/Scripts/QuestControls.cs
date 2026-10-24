using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;
namespace BWVR {
// OpenXR exposes separate grip and aim poses. Keep carried objects at the grip;
// use the aim pose for selection rays instead of adding a guessed angle.
public static class QuestControls {
 public struct Hand {
  public bool tracked,grip,trigger,menu,recenter;
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
   h.recenter=side==0&&(controller.TryGetChildControl<ButtonControl>("secondaryButton")?.isPressed??false);
   return h;
  }
  return new Hand();
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
  origin.position+=(forward*input.y+right*input.x)*(.65f*Mathf.Min(delta,.05f));
  if(Mathf.Abs(turn.x)<.25f)turnHeld=false;
  if(Mathf.Abs(turn.x)>.7f&&!turnHeld){origin.RotateAround(head.position,Vector3.up,Mathf.Sign(turn.x)*30);turnHeld=true;}
  // Vertical travel lets the unseen player inspect the tabletop at different heights.
  float vertical=Mathf.Abs(turn.y)>.3f?Mathf.Sign(turn.y)*(Mathf.Abs(turn.y)-.3f)/.7f:0;
  origin.position+=Vector3.up*(vertical*.45f*Mathf.Min(delta,.05f));
 }
}
}
