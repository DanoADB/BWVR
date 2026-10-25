using System.Linq;
using UnityEngine;
namespace BWVR {
// Runs after animation/hand IK. Pose blending owns only the artist's facial bones.
[DefaultExecutionOrder(100)]
public sealed class GorillaMoodFace:MonoBehaviour {
 public GorillaFaceProfile profile;
 public CreatureAffect.Mood CurrentMood{get;private set;}
 Transform[] bones;Vector3[] positions,scales;Quaternion[] rotations;Animator animator;HumanoidMotion motion;CreatureCuriosity mind;float blinkTime=1;bool authoredHold;
 bool Ready(){
  if(bones!=null)return true;if(profile==null||!profile.Valid())return false;
  var all=GetComponentsInChildren<Transform>();bones=profile.bones.Select(name=>all.FirstOrDefault(t=>t.name==name)).ToArray();
  if(bones.Any(b=>b==null)){bones=null;return false;}
  positions=bones.Select(b=>b.localPosition).ToArray();rotations=bones.Select(b=>b.localRotation).ToArray();scales=bones.Select(b=>b.localScale).ToArray();
  animator=GetComponent<Animator>();motion=GetComponentInParent<HumanoidMotion>();return true;
 }
 void LateUpdate(){
  if(!Ready())return;if(mind==null)mind=GetComponentInParent<CreatureCuriosity>();if(mind==null)return;
  if(motion!=null&&motion.paused){WritePose();if(!authoredHold)Blink(0);return;}
  bool authored=false;if(animator!=null){var state=animator.GetCurrentAnimatorStateInfo(0);authored=state.IsName("Warmth reaction")||state.IsName("Unease reaction");if(animator.IsInTransition(0)){state=animator.GetNextAnimatorStateInfo(0);authored|=state.IsName("Warmth reaction")||state.IsName("Unease reaction");}}
  ApplyMood(mind.Affect,Time.deltaTime,authored);if(!authored)Blink(Time.deltaTime);
 }
 public void ApplyMood(CreatureAffect.Mood mood,float dt,bool authoredReaction=false){
  if(!Ready()||dt<0||float.IsNaN(dt)||float.IsInfinity(dt)||(int)mood<0||(int)mood>=profile.moods.Length)return;
  authoredHold=authoredReaction;
  // Preserve the complete authored reaction and blend out from its final face.
  if(authoredReaction){for(int i=0;i<bones.Length;i++){positions[i]=bones[i].localPosition;rotations[i]=bones[i].localRotation;scales[i]=bones[i].localScale;}return;}
  if(dt>0){CurrentMood=mood;var target=profile.moods[(int)mood];float blend=1-Mathf.Exp(-dt*6);
   for(int i=0;i<bones.Length;i++){positions[i]=Vector3.Lerp(positions[i],target.positions[i],blend);rotations[i]=Quaternion.Slerp(rotations[i],target.rotations[i],blend);scales[i]=Vector3.Lerp(scales[i],target.scales[i],blend);}}
  WritePose();
 }
 void WritePose(){for(int i=0;i<bones.Length;i++){if(bones[i]==null)continue;bones[i].SetLocalPositionAndRotation(positions[i],rotations[i]);bones[i].localScale=scales[i];}}
 void Blink(float dt){blinkTime=Mathf.Repeat(blinkTime+Mathf.Max(0,dt),5.2f);float amount=blinkTime<.24f?Mathf.Sin(blinkTime/.24f*Mathf.PI):0;if(amount<=0)return;
  for(int i=0;i<bones.Length;i++)if(bones[i]!=null&&profile.bones[i].Contains("Eyelid")){bones[i].localRotation=Quaternion.Slerp(rotations[i],profile.blink.rotations[i],amount);bones[i].localPosition=Vector3.Lerp(positions[i],profile.blink.positions[i],amount);bones[i].localScale=Vector3.Lerp(scales[i],profile.blink.scales[i],amount);}}
}
}
