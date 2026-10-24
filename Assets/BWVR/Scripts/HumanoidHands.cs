using UnityEngine;
namespace BWVR {
// The free UAL has no carry/drink clip: preserve locomotion and solve the hands around the visible resource.
public sealed class HumanoidHands:MonoBehaviour {
 public Transform actor,resource; public bool drinking; Animator animator;
 void Awake(){animator=GetComponent<Animator>();}
 Vector3 Goal(){float height=actor.lossyScale.y;var head=animator.GetBoneTransform(HumanBodyBones.Head);
  return drinking?head.position+actor.forward*height*.18f-actor.up*height*.035f:actor.position+actor.up*height*.57f+actor.forward*height*.23f;
 }
 void OnAnimatorIK(int layer){if(resource==null||actor==null)return;var goal=Goal();float spread=actor.lossyScale.y*.09f;
  animator.SetIKPositionWeight(AvatarIKGoal.LeftHand,1);animator.SetIKPositionWeight(AvatarIKGoal.RightHand,1);
  animator.SetIKPosition(AvatarIKGoal.LeftHand,goal-actor.right*spread);animator.SetIKPosition(AvatarIKGoal.RightHand,goal+actor.right*spread);
 }
 void LateUpdate(){if(resource==null||actor==null)return;resource.position=Goal();resource.rotation=actor.rotation*Quaternion.Euler(drinking?-45:0,0,0);}
}
}
