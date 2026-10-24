using UnityEngine;
namespace BWVR {
// The free UAL has no carry/drink clip: preserve locomotion and solve the hands around the visible resource.
public sealed class HumanoidHands:MonoBehaviour {
 public Transform actor,resource; public bool drinking,paused; Animator animator;float sip;
 void Awake(){animator=GetComponent<Animator>();}
 void Update(){AdvancePose(Time.deltaTime);}
 public void AdvancePose(float dt){if(!paused)sip=Mathf.MoveTowards(sip,drinking&&resource!=null?1:0,Mathf.Max(0,dt)*3);}
 Vector3 Goal(){float height=actor.lossyScale.y;var head=animator.GetBoneTransform(HumanBodyBones.Head);
  var carrying=actor.position+actor.up*height*.57f+actor.forward*height*.23f;var mouth=head.position+actor.forward*height*.3f+actor.up*height*.1f;return Vector3.Lerp(carrying,mouth,sip);
 }
 void OnAnimatorIK(int layer){if(resource==null||actor==null){animator.SetIKPositionWeight(AvatarIKGoal.LeftHand,0);animator.SetIKPositionWeight(AvatarIKGoal.RightHand,0);return;}var goal=Goal();float spread=actor.lossyScale.y*.09f;
  animator.SetIKPositionWeight(AvatarIKGoal.LeftHand,1);animator.SetIKPositionWeight(AvatarIKGoal.RightHand,1);
  animator.SetIKPosition(AvatarIKGoal.LeftHand,goal-actor.right*spread);animator.SetIKPosition(AvatarIKGoal.RightHand,goal+actor.right*spread);
 }
 void LateUpdate(){if(resource==null||actor==null)return;resource.position=Goal();resource.rotation=actor.rotation*Quaternion.Euler(-45*sip,0,0);}
}
}
