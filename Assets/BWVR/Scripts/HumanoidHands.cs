using UnityEngine;
namespace BWVR {
// The free UAL has no carry/drink clip: preserve locomotion and solve the hands around the visible resource.
public sealed class HumanoidHands:MonoBehaviour {
 public Transform actor,resource; public bool drinking,paused,beckon,indicating;public Transform lookTarget;public Vector3 indicatedPoint;public float displayTilt; Animator animator;float sip,gestureClock;
 void Awake(){animator=GetComponent<Animator>();}
 void Update(){AdvancePose(Time.deltaTime);}
 public void AdvancePose(float dt){if(!paused){sip=Mathf.MoveTowards(sip,drinking&&resource!=null?1:0,Mathf.Max(0,dt)*3);gestureClock+=Mathf.Max(0,dt);}}
 Vector3 Goal(){float height=actor.lossyScale.y;var head=animator.GetBoneTransform(HumanBodyBones.Head);
  var carrying=actor.position+actor.up*height*.57f+actor.forward*height*.23f;var mouth=head.position+actor.forward*height*.3f+actor.up*height*.1f;return Vector3.Lerp(carrying,mouth,sip);
 }
 void OnAnimatorIK(int layer){
  animator.SetLookAtWeight(lookTarget!=null?.7f:0,.15f,.85f,.5f,.55f);if(lookTarget!=null)animator.SetLookAtPosition(lookTarget.position);
  bool carry=resource!=null&&actor!=null;animator.SetIKPositionWeight(AvatarIKGoal.LeftHand,carry?1:0);animator.SetIKPositionWeight(AvatarIKGoal.RightHand,carry||beckon||indicating?1:0);
  if(actor==null)return;float height=actor.lossyScale.y;
  if(carry){var goal=Goal();float spread=height*.09f;animator.SetIKPosition(AvatarIKGoal.LeftHand,goal-actor.right*spread);animator.SetIKPosition(AvatarIKGoal.RightHand,goal+actor.right*spread);}
  if(beckon){float wave=Mathf.Sin(gestureClock*7);animator.SetIKPosition(AvatarIKGoal.RightHand,actor.position+actor.up*height*(.8f+wave*.08f)+actor.right*height*.2f+actor.forward*height*(.2f+wave*.12f));}
  else if(indicating){var direction=indicatedPoint-actor.position;direction.y=0;if(direction.sqrMagnitude>1e-10f)animator.SetIKPosition(AvatarIKGoal.RightHand,actor.position+actor.up*height*.72f+direction.normalized*height*.5f);}
 }
 void LateUpdate(){if(resource==null||actor==null)return;resource.rotation=actor.rotation*Quaternion.Euler(-45*sip+displayTilt,0,0);resource.position=Goal();if(resource.GetComponent<WaterVessel>()!=null)resource.position-=resource.up*resource.lossyScale.y*.85f;}
}
}
