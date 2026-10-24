using UnityEngine;
namespace BWVR {
// Movement stays with gameplay; the shared CC0 library supplies pose animation only.
public sealed class HumanoidMotion:MonoBehaviour {
 public bool paused; Animator animator; HumanoidHands hands; Vector3 previous; int activity;
 void Awake(){animator=GetComponentInChildren<Animator>();previous=transform.position;if(animator!=null){hands=animator.GetComponent<HumanoidHands>();if(hands!=null)hands.actor=transform;}}
 void LateUpdate(){if(animator==null)return;var delta=transform.position-previous;previous=transform.position;
  var item=GetComponent<CarryItem>();bool carried=item!=null&&item.held;
  delta.y=0;bool moving=delta.magnitude/Mathf.Max(Time.deltaTime,.000001f)>transform.lossyScale.y*.05f;
  if(item!=null&&moving&&!carried&&!paused)transform.rotation=Quaternion.LookRotation(delta);
  animator.speed=paused?0:1;if(hands!=null)hands.paused=paused;animator.SetFloat("Speed",!paused&&!carried&&moving?1:0);animator.SetInteger("Activity",carried?0:activity);
 }
 public void Activity(int value){activity=value;}
 public void Carry(Transform resource,bool drink=false){if(hands!=null){hands.resource=resource;hands.drinking=drink;}}
 public void Playful(float clock){if(hands!=null)hands.displayTilt=Mathf.Sin(clock*3)*28;}
 public void LookAt(Transform target){if(hands==null)return;var other=target!=null?target.GetComponentInChildren<Animator>():null;hands.lookTarget=other!=null&&other.isHuman?other.GetBoneTransform(HumanBodyBones.Head):target;}
 public void Signal(bool beckon,bool point=false,Vector3 target=default,float tilt=0){if(hands==null)return;hands.beckon=beckon;hands.indicating=point;hands.indicatedPoint=target;hands.displayTilt=tilt;}
 public void React(){if(animator!=null&&!paused&&activity==0)animator.SetTrigger("Warmth");}
}
}
