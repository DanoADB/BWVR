using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace BWVR.Editor {
public static class PlayerFetchChecks {
 [MenuItem("BWVR/Verify player throwing and fetch (Play mode)")]
 public static void Verify(){
  if(!EditorApplication.isPlaying)throw new Exception("Enter Play mode first");
  void Require(bool ok,string why){if(!ok)throw new Exception(why);}
  Require(JsonUtility.FromJson<WorldPlayMemory>("{}").Valid(),"Legacy play memory rejected");var history=new ThrowMotion();history.Sample(Vector3.zero,0,1);history.Sample(Vector3.right*.01f,.02f,1);history.Sample(Vector3.right*.02f,.04f,1);Require(history.Velocity.x>.2f,"Hand motion not measured");history.Sample(Vector3.right,1,1);Require(history.Velocity==Vector3.zero,"Stale/jumped tracking became throw");
  var slice=UnityEngine.Object.FindAnyObjectByType<QuestSlice>();var god=UnityEngine.Object.FindAnyObjectByType<CreatureHybrid>();var mind=god.GetComponent<CreatureCuriosity>();var nav=god.GetComponent<IslandNavigator>();var ball=slice.GetComponentsInChildren<BeachBall>()[0];var item=ball.GetComponent<CarryItem>();var prop=ball.GetComponent<CreatureAffordance>();
  var field=typeof(QuestSlice).GetField("state",BindingFlags.NonPublic|BindingFlags.Instance);var original=(SliceState)field.GetValue(slice);var position=god.transform.position;god.Interrupt("Player fetch QA");slice.SendMessage("OnApplicationPause",false);
  foreach(var resident in slice.GetComponentsInChildren<VillagerExploration>()){resident.ResetTask();resident.GetComponent<VillagerSocial>().ResetTask();resident.GetComponent<VillagerBlockPlay>().ResetTask();}
  var test=new SliceState{water=100,crops=100,hunger=0,playPreference=1,socialPreference=0,developmentFocus=true};field.SetValue(slice,test);
  // The saved season may be older than the fixture clock. Reset decision deadlines after switching states.
  god.NewSeason();
  try{
   test.Building.observed=15;test.WorldPlay.caution=1;ball.ResetWorld();Require(nav.world.Sample(ball.FetchHome+Vector3.back*nav.world.Scale*12,nav.profile,12,out var at),"No play approach");god.transform.position=nav.world.ProjectFoot(at,nav.profile);nav.ResetRoute();
   test.paused=true;Require(!ball.PlayerToss(Vector3.right*.5f),"Paused toss accepted");test.paused=false;
   // Drive the real release handler with measured hand samples, rather than bypassing input dispatch.
   var held=(CarryItem[])typeof(QuestSlice).GetField("held",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(slice);var motion=(ThrowMotion[])typeof(QuestSlice).GetField("throwMotion",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(slice);held[1]=item;item.held=true;item.GetComponent<Collider>().enabled=false;
   var rig=(Transform)typeof(QuestSlice).GetField("rig",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(slice);var direction=ball.Direction(ball.transform);motion[1].Reset();for(int i=0;i<4;i++)motion[1].Sample(direction*i*.01f*rig.localScale.x,i*.02f,rig.localScale.x);
   typeof(QuestSlice).GetMethod("Release",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(slice,new object[]{1,new Ray(ball.transform.position,Vector3.forward)});
   Require(held[1]==null&&!item.held&&ball.InFlight&&ball.FetchReserved,"Carry release did not launch player fetch toss");Require(test.WorldPlay.throws==0,"Player toss credited creature");int ticket=ball.FetchTicket;
   var before=ball.Body.position;ball.Advance(1,true);Require(ball.Body.position==before,"Paused toss moved");
   bool carried=false;for(int i=0;i<2400&&test.WorldPlay.fetchReturns==0;i++){ball.Advance(.05f,false);god.Tick(.05f);carried|=mind.Current==CreatureCuriosity.Phase.FetchCarry;}
   Require(carried&&test.WorldPlay.fetchReturns==1&&!item.held&&!ball.FetchReserved&&Vector3.Distance(ball.transform.position,item.home)<nav.world.Scale,"Autonomous native fetch/return failed: "+mind.Current+" @ "+test.Hybrid.time+" harm "+ball.Harmful);
   Require(test.Hybrid.receipt.eligible&&test.Hybrid.receipt.action==CreatureAction.PlayWithToy&&test.Hybrid.receipt.target==ball.StableId,"No completed retrieval lesson");Require(test.Hybrid.receipt.fetched,"Retrieval was not identified in receipt");float fun=test.WorldPlay.fetchFun;Require(god.Feedback(true)&&!god.Feedback(true)&&test.WorldPlay.fetchFun>fun,"Retrieval feedback was missing or repeatable");
   var saved=JsonUtility.FromJson<SliceState>(JsonUtility.ToJson(test));Require(saved.Valid()&&saved.WorldPlay.fetchReturns==1&&saved.WorldPlay.fetchFun==test.WorldPlay.fetchFun&&saved.Hybrid.receipt.fetched,"Fetch experience did not save");Require(!ball.CompleteFetch(ticket),"Old completion duplicated reward");
   god.Interrupt("Fetch cancellation QA");ball.ReturnHome();Require(ball.PlayerToss(direction*.3f),"Second toss rejected");
   for(int i=0;i<600&&!ball.FetchReady;i++){ball.Advance(.05f,false);test.Hybrid.time+=.05f;}Require(ball.FetchReady,"Cancellation toss did not settle");Require(nav.world.Sample(ball.transform.position,nav.profile,12,out var fetchAt),"No cancellation approach");god.transform.position=nav.world.ProjectFoot(fetchAt,nav.profile);nav.ResetRoute();Require(mind.TryBegin(prop),"Cancellation action rejected");for(int i=0;i<600&&mind.Current!=CreatureCuriosity.Phase.FetchCarry;i++){mind.Tick(.05f,test,false);ball.Advance(.05f,false);}
   Require(mind.Current==CreatureCuriosity.Phase.FetchCarry&&item.held,"Cancellation did not reach native fetch carry");god.Interrupt("Player canceled fetch");Require(!item.held&&item.GetComponent<Collider>().enabled&&!ball.FetchReserved&&test.WorldPlay.fetchReturns==1,"Interrupted carry stranded ball or granted return");
   ball.ReturnHome();Require(ball.PlayerToss(direction*.3f),"Expiry toss rejected");ball.CancelFetch(ticket);Require(ball.FetchReserved,"Stale cancellation erased a newer invitation");test.Hybrid.time+=91;ball.Advance(.02f,false);Require(!ball.FetchReserved&&!ball.ClaimFetch(ticket),"Expired/stale invitation claimed");
   ball.ReturnHome();held[1]=item;item.held=true;motion[1].Reset();typeof(QuestSlice).GetMethod("Release",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(slice,new object[]{1,new Ray(item.home+Vector3.up*.5f,Vector3.down)});Require(!item.held&&!ball.InFlight&&!ball.FetchReserved,"Slow release became a toss");
   ball.ReturnHome();Require(ball.PlayerToss(direction*.3f,.5f),"Scaled toss rejected");var velocity=ball.Body.linearVelocity;ball.ReturnHome();Require(ball.PlayerToss(direction*.6f,1)&&Vector3.Distance(velocity,ball.Body.linearVelocity)<.001f,"Zoom changed equivalent throw strength");
   Debug.Log("BWVR_PLAYER_FETCH_PASS: bounded motion history; actual carry-release handler launches native player physics without creature blame; pause; autonomous selection/reach/lift/carry/return; physical play circle; typed one-shot feedback; saved return count; duplicate/stale/expired completion guards");
  }finally{god.Interrupt("Player fetch QA complete");ball.ReturnHome();god.transform.position=position;nav.ResetRoute();field.SetValue(slice,original);ball.ResetWorld();}
 }
}
}
