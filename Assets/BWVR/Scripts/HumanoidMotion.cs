using UnityEngine;
namespace BWVR {
// Movement stays with gameplay; the shared CC0 library supplies pose animation only.
public sealed class HumanoidMotion:MonoBehaviour {
 public bool paused; Animator animator; Vector3 previous;
 void Awake(){animator=GetComponentInChildren<Animator>();previous=transform.position;}
 void LateUpdate(){if(animator==null)return;var delta=transform.position-previous;previous=transform.position;
  var item=GetComponent<CarryItem>();bool carried=item!=null&&item.held;
  delta.y=0;bool moving=delta.magnitude/Mathf.Max(Time.deltaTime,.000001f)>.005f;
  if(item!=null&&moving&&!carried&&!paused)transform.rotation=Quaternion.LookRotation(delta);
  animator.speed=paused?0:1;animator.SetFloat("Speed",!paused&&!carried&&moving?1:0);
 }
 public void React(){if(animator!=null&&!paused)animator.SetTrigger("Warmth");}
}
}
