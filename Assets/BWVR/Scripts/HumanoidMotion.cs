using UnityEngine;
namespace BWVR {
// Movement stays with gameplay; the shared CC0 library supplies pose animation only.
public sealed class HumanoidMotion:MonoBehaviour {
 public bool paused; Animator animator; HumanoidHands hands; Vector3 previous; int activity;
 void Awake(){animator=GetComponentInChildren<Animator>();previous=transform.position;if(animator!=null){hands=animator.GetComponent<HumanoidHands>();if(hands!=null)hands.actor=transform;}}
 void LateUpdate(){if(animator==null)return;var delta=transform.position-previous;previous=transform.position;
  var item=GetComponentInParent<CarryItem>();bool carried=item!=null&&item.held;
  delta.y=0;bool moving=delta.magnitude/Mathf.Max(Time.deltaTime,.000001f)>transform.lossyScale.y*.05f;
  if(item!=null&&item.kind!=Offering.Creature&&moving&&!carried&&!paused&&delta.sqrMagnitude>1e-12f)transform.rotation=Quaternion.LookRotation(delta);
  bool walking=!paused&&!carried&&moving;
  // A held load owns the hands, not the legs. Movement supersedes stale work poses.
  animator.speed=paused?0:1;if(hands!=null)hands.paused=paused;animator.SetFloat("Speed",walking?1:0);animator.SetInteger("Activity",carried||walking?0:activity);
 }
 public void Activity(int value){activity=value;}
 public void Touch(Transform target){if(hands!=null)hands.touchTarget=target;}
 public void Carry(Transform resource,bool drink=false){if(hands!=null){hands.resource=resource;hands.drinking=drink;}}
 public void Playful(float clock){if(hands!=null)hands.displayTilt=Mathf.Sin(clock*3)*28;}
 public void PlaceAt(Vector3 point,Quaternion rotation,float blend){if(hands!=null){hands.placementPoint=point;hands.placementRotation=rotation;hands.placementBlend=Mathf.Clamp01(blend);}}
 public void LookAt(Transform target){if(hands==null)return;var other=target!=null?target.GetComponentInChildren<Animator>():null;hands.lookTarget=other!=null&&other.isHuman?other.GetBoneTransform(HumanBodyBones.Head):target;}
 public void RespondTo(Transform target,bool nod,float progress){if(hands==null)return;hands.attentionTarget=target;float wave=progress>0&&progress<1?Mathf.Sin(progress*Mathf.PI*2)*Mathf.Sin(progress*Mathf.PI):0;hands.attentionPose=nod?new Vector2(0,wave*.55f):new Vector2(wave*.55f,0);}
 public void Signal(bool beckon,bool point=false,Vector3 target=default,float tilt=0){if(hands==null)return;hands.beckon=beckon;hands.indicating=point;hands.indicatedPoint=target;hands.displayTilt=tilt;}
 public void React(){if(animator!=null&&!paused&&activity==0)animator.SetTrigger("Warmth");}
 public void Feedback(bool positive){
  if(animator==null||paused||activity!=0||(hands!=null&&hands.resource!=null))return;
  if(positive){foreach(var parameter in animator.parameters)if(parameter.name=="Unease")animator.ResetTrigger("Unease");React();}
  else foreach(var parameter in animator.parameters)if(parameter.name=="Unease"&&parameter.type==AnimatorControllerParameterType.Trigger){animator.ResetTrigger("Warmth");animator.SetTrigger("Unease");break;}
 }
}
}
