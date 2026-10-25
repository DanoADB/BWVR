using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace BWVR.Editor {
public static class GentlePlayChecks {
 [MenuItem("BWVR/Verify gentle skill and pickup hands (Play mode)")]
 public static void Verify(){
  if(!Application.isPlaying)throw new Exception("Enter Play mode");
  var skill=new GentleBallSkill();float spread=skill.AimSpread;
  if(!skill.Practice(0)||skill.Practice(0)||skill.Practice(11))throw new Exception("Practice cooldown failed");
  for(int i=1;i<=30;i++)skill.Practice(i*12);
  if(skill.control<.9f||skill.AimSpread>=spread*.4f||skill.Aim(Vector3.forward,1347,10,"ball")!=skill.Aim(Vector3.forward,1347,10,"ball"))throw new Exception("Practice did not improve deterministic control");
  var legacy=JsonUtility.FromJson<WorldPlayMemory>("{}");if(!legacy.Valid()||!legacy.GentleBall.Valid())throw new Exception("Legacy skill default invalid");
  var slice=UnityEngine.Object.FindAnyObjectByType<QuestSlice>();var field=typeof(QuestSlice).GetField("state",BindingFlags.Instance|BindingFlags.NonPublic);var original=(SliceState)field.GetValue(slice);var god=slice.GetComponentInChildren<CreatureHybrid>();
  god.Interrupt("Gentle skill QA");var test=new SliceState{developmentFocus=true};field.SetValue(slice,test);
  try{
   var ball=slice.GetComponentInChildren<BeachBall>();test.Hybrid.time=20;test.Hybrid.receipt=new ActionReceipt{eligible=true,gentleRally=true,action=CreatureAction.PlayWithToy,situation=CreatureSituation.SafeContact,target=ball.StableId,completedAt=20};
   float before=test.WorldPlay.GentleBall.preference;
   test.paused=true;if(god.Feedback(true)||test.WorldPlay.GentleBall.preference!=before)throw new Exception("Paused teaching changed skill");test.paused=false;
   if(!god.Feedback(true)||test.WorldPlay.GentleBall.preference<=before)throw new Exception("Completed rally praise did not teach");float praised=test.WorldPlay.GentleBall.preference;
   if(god.Feedback(true)||test.WorldPlay.GentleBall.preference!=praised)throw new Exception("Duplicate feedback taught twice");
   test.Hybrid.receipt=new ActionReceipt{eligible=true,gentleRally=true,action=CreatureAction.PlayWithToy,target=ball.StableId,completedAt=0};if(god.Feedback(false)||test.WorldPlay.GentleBall.preference!=praised)throw new Exception("Expired feedback taught skill");
   test.Hybrid.receipt=new ActionReceipt{eligible=true,gentleRally=true,action=CreatureAction.PlayWithToy,target=ball.StableId,completedAt=20};if(!god.Feedback(false)||test.WorldPlay.GentleBall.preference>=praised)throw new Exception("Discouragement did not lower preference");
   test.WorldPlay.gentleBall=skill;var save=JsonUtility.FromJson<SliceState>(JsonUtility.ToJson(test));if(!save.Valid()||save.WorldPlay.GentleBall.practices!=31||save.WorldPlay.GentleBall.control!=skill.control)throw new Exception("Skill did not persist");save.WorldPlay.GentleBall.control=float.NaN;if(save.Valid())throw new Exception("Invalid skill accepted");
   CheckHand(slice);
   Debug.Log("BWVR_GENTLE_PLAY_PASS: deterministic practiced accuracy, bounded cooldown, pause/duplicate/expired feedback, discouragement, legacy/save validation; pickup and drop hand follows live ray, closes, scales with zoom and hides on tracking loss");
  }finally{field.SetValue(slice,original);}
 }
 static void CheckHand(QuestSlice slice){
  var flags=BindingFlags.Instance|BindingFlags.NonPublic;var hands=(PickupHand[])typeof(QuestSlice).GetField("pickupHands",flags).GetValue(slice);var held=(CarryItem[])typeof(QuestSlice).GetField("held",flags).GetValue(slice);var update=typeof(QuestSlice).GetMethod("UpdatePickupHand",flags);var previous=held[1];var villager=slice.GetComponentsInChildren<CarryItem>().First(c=>c.kind==Offering.Villager);var originalPosition=villager.transform.position;
  try{
   var ray=new Ray(villager.transform.position+Vector3.up*.5f,Vector3.down);if(!Physics.Raycast(ray,out var hit,1))throw new Exception("No native pickup ray hit");
   held[1]=null;update.Invoke(slice,new object[]{1,ray,hit,true});var hand=hands[1];if(!hand.gameObject.activeSelf||Vector3.Distance(hand.transform.position,hit.point)>.1f||hand.GetComponentsInChildren<Collider>().Any(c=>c.enabled))throw new Exception("Pickup hand missing or blocks ray");
   held[1]=villager;var dropRay=new Ray(villager.home+Vector3.up*.5f,Vector3.down);update.Invoke(slice,new object[]{1,dropRay,hit,true});var before=hand.transform.position;
   var shifted=new Ray(dropRay.origin+Vector3.right*.015f,Vector3.down);update.Invoke(slice,new object[]{1,shifted,hit,true});if(hand.transform.position==before)throw new Exception("Held hand did not follow drop ray");
   if(hand.transform.Find("Finger 1").localEulerAngles.x<30)throw new Exception("Carry hand did not close");
   hand.Show(true,before,Vector3.up,Vector3.forward,2,true,true,false);if(Mathf.Abs(hand.transform.localScale.x-.07f)>.0001f)throw new Exception("Hand zoom scale incorrect");
   update.Invoke(slice,new object[]{1,shifted,hit,false});if(hand.gameObject.activeSelf)throw new Exception("Tracking loss left hand visible");
  }finally{held[1]=previous;villager.transform.position=originalPosition;hands[1].gameObject.SetActive(false);}
 }
}
}
