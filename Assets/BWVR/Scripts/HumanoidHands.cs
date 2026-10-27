using UnityEngine;
namespace BWVR {
// The free UAL has no carry/drink clip: preserve locomotion and solve the hands around the visible resource.
public sealed class HumanoidHands:MonoBehaviour {
 public Transform actor,resource,touchTarget; public bool drinking,paused,beckon,indicating,praying,sheltering;public Transform lookTarget;public Vector3 indicatedPoint;public float displayTilt; Animator animator;float sip,gestureClock;
 public Vector3 placementPoint;public Quaternion placementRotation;public float placementBlend;
 public Transform attentionTarget;public Vector2 attentionPose;
 void Awake(){animator=GetComponent<Animator>();}
 void Update(){AdvancePose(Time.deltaTime);}
 public void AdvancePose(float dt){if(!paused){sip=Mathf.MoveTowards(sip,drinking&&resource!=null?1:0,Mathf.Max(0,dt)*3);gestureClock+=Mathf.Max(0,dt);}}
 Vector3 Goal(){float height=actor.lossyScale.y;var head=animator.GetBoneTransform(HumanBodyBones.Head);
  var item=resource!=null?resource.GetComponent<CarryItem>():null;bool friend=item!=null&&item.kind==Offering.Villager;
  var carrying=actor.position+actor.up*height*(friend?.60f:.57f)+actor.forward*height*(friend?.36f:.23f);var mouth=head.position+actor.forward*height*.3f+actor.up*height*.1f;return Vector3.Lerp(Vector3.Lerp(carrying,mouth,sip),placementPoint,placementBlend);
 }
 void OnAnimatorIK(int layer){
  var gaze=attentionTarget!=null?attentionTarget:lookTarget;
  animator.SetLookAtWeight(gaze!=null?.7f:0,.15f,.85f,.5f,.55f);
  if(gaze!=null){var at=gaze.position;if(actor!=null&&attentionPose.sqrMagnitude>0){var head=animator.GetBoneTransform(HumanBodyBones.Head);var from=head!=null?head.position:actor.position;float gazeHeight=actor.lossyScale.y;at=from+(at-from).normalized*gazeHeight*2+(actor.right*attentionPose.x+actor.up*attentionPose.y)*gazeHeight;}animator.SetLookAtPosition(at);}
  bool carry=resource!=null&&actor!=null;animator.SetIKPositionWeight(AvatarIKGoal.LeftHand,carry||praying||sheltering?1:0);animator.SetIKPositionWeight(AvatarIKGoal.RightHand,carry||praying||sheltering||beckon||indicating||touchTarget!=null?1:0);
  if(actor==null)return;float height=actor.lossyScale.y;
  if(carry){var goal=Goal();float spread=height*.09f;animator.SetIKPosition(AvatarIKGoal.LeftHand,goal-actor.right*spread);animator.SetIKPosition(AvatarIKGoal.RightHand,goal+actor.right*spread);}
  if(!carry&&sheltering){var head=animator.GetBoneTransform(HumanBodyBones.Head);var crown=head!=null?head.position:actor.position+actor.up*height;var goal=crown+actor.up*height*.03f+actor.forward*height*.12f;animator.SetIKPosition(AvatarIKGoal.LeftHand,goal-actor.right*height*.05f);animator.SetIKPosition(AvatarIKGoal.RightHand,goal+actor.right*height*.05f);}
  else if(!carry&&praying){var head=animator.GetBoneTransform(HumanBodyBones.Head);float body=head!=null?Mathf.Max(height,Vector3.Dot(head.position-actor.position,actor.up)):height;var goal=actor.position+actor.up*body*(.78f+Mathf.Sin(gestureClock*1.5f)*.01f)+actor.forward*body*.18f;animator.SetIKPosition(AvatarIKGoal.LeftHand,goal-actor.right*body*.025f);animator.SetIKPosition(AvatarIKGoal.RightHand,goal+actor.right*body*.025f);}
  else if(!carry&&touchTarget!=null){animator.SetIKPosition(AvatarIKGoal.RightHand,touchTarget.position+touchTarget.up*touchTarget.lossyScale.y*.5f);}
  else if(beckon){float wave=Mathf.Sin(gestureClock*7);animator.SetIKPosition(AvatarIKGoal.RightHand,actor.position+actor.up*height*(.8f+wave*.08f)+actor.right*height*.2f+actor.forward*height*(.2f+wave*.12f));}
  else if(indicating){var direction=indicatedPoint-actor.position;direction.y=0;if(direction.sqrMagnitude>1e-10f)animator.SetIKPosition(AvatarIKGoal.RightHand,actor.position+actor.up*height*.72f+direction.normalized*height*.5f);}
 }
 void LateUpdate(){if(resource==null||actor==null)return;var goal=Goal();
  var item=resource.GetComponent<CarryItem>();
  if(item!=null&&(item.kind==Offering.Villager||item.kind==Offering.Toy)){
   // Retarget the short creature arms after animation as well as native Humanoid IK.
   // Different authored shoulder proportions otherwise leave tiny friends inside the chest.
   float spread=actor.lossyScale.y*(item.kind==Offering.Villager?.045f:.07f);
   Reach(HumanBodyBones.LeftUpperArm,HumanBodyBones.LeftLowerArm,HumanBodyBones.LeftHand,goal-actor.right*spread);
   Reach(HumanBodyBones.RightUpperArm,HumanBodyBones.RightLowerArm,HumanBodyBones.RightHand,goal+actor.right*spread);
  }
  resource.rotation=Quaternion.Slerp(actor.rotation*Quaternion.Euler(-45*sip+displayTilt,0,0),placementRotation,placementBlend);resource.position=goal;if(resource.GetComponent<ThrowableTree>() is ThrowableTree tree)resource.position-=resource.up*tree.Height*tree.World.Unit*.35f;if(resource.GetComponent<WaterVessel>()!=null)resource.position-=resource.up*resource.lossyScale.y*.85f;
 }
 void Reach(HumanBodyBones upper,HumanBodyBones lower,HumanBodyBones hand,Vector3 goal){
  var shoulder=animator.GetBoneTransform(upper);var elbow=animator.GetBoneTransform(lower);var palm=animator.GetBoneTransform(hand);if(shoulder==null||elbow==null||palm==null)return;
  for(int i=0;i<4;i++){elbow.rotation=Quaternion.FromToRotation(palm.position-elbow.position,goal-elbow.position)*elbow.rotation;shoulder.rotation=Quaternion.FromToRotation(palm.position-shoulder.position,goal-shoulder.position)*shoulder.rotation;}
 }
}
}
