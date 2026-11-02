using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
namespace BWVR.Editor {
public static class HeldBallChecks {
 static readonly BindingFlags F=BindingFlags.Instance|BindingFlags.NonPublic;
 static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
 public static void Verify(Action<CreatureHybrid,BeachBall> capture=null){
  bool single=false,handoff=false;string saved=null;
  BallChaseChecks.Verify((god,ball)=>{Check(god.FollowingHeldBall&&god.Owner==CreatureHybrid.ActionOwner.Curiosity&&ball.Chase.Active,"Follow has a competing owner");capture?.Invoke(god,ball);},(slice,god,ball)=>{
   var nav=god.GetComponent<IslandNavigator>();float u=nav.world.Scale;var forward=god.transform.forward;
   ball.transform.position=god.transform.position+(forward*18+Vector3.up*10)*u;
   var rig=(Transform)typeof(QuestSlice).GetField("rig",F).GetValue(slice);var history=((ThrowMotion[])typeof(QuestSlice).GetField("throwMotion",F).GetValue(slice))[1];history.Reset();
   for(int i=0;i<7;i++)history.Sample(ball.transform.position+forward*((6-i)*.02f*8/18*rig.localScale.x),1+i*.02f,rig.localScale.x);
   Check(Vector3.Dot(history.Velocity,-forward)>rig.localScale.x*.35f,"Measured release is not a gentle throw");
   typeof(QuestSlice).GetMethod("Release",F).Invoke(slice,new object[]{1,new Ray(ball.transform.position,Vector3.down)});
   Check(!ball.Chase.Active&&god.Season.WorldPlay.Chase.games==1&&god.State.receipt.chased&&!god.State.receipt.catchBall,"Release lost the completed chase or confused its lesson");
   typeof(CreatureHybrid).GetMethod("FinishHeldBallRelease",F).Invoke(god,new object[]{ball});Check(god.Season.WorldPlay.Chase.games==1,"Release counted twice");
  },(god,ball)=>{
   god.Perceive();var candidates=(System.Collections.Generic.List<CreatureCandidate>)typeof(CreatureHybrid).GetField("candidates",F).GetValue(god);var choices=candidates.Where(c=>c.target==ball.StableId).ToArray();
   Check(choices.Length==1&&choices[0].chase&&choices[0].action==CreatureAction.PlayWithToy,"Duplicate held-ball decision paths remain");single=true;
  },(god,ball)=>{Check(!ball.Chase.Active&&god.Owner==CreatureHybrid.ActionOwner.Curiosity&&god.State.intention.catchBall&&god.GetComponent<CreatureCuriosity>().Current==CreatureCuriosity.Phase.CatchBall,"Native toss did not hand off immediately to catch selection");handoff=true;},(god,ball)=>saved=JsonUtility.ToJson(god.Season));
  Check(single&&handoff&&saved!=null,"Missing native unified-play evidence");
  var slice=UnityEngine.Object.FindAnyObjectByType<QuestSlice>();var field=typeof(QuestSlice).GetField("state",F);var original=field.GetValue(slice);var creature=slice.GetComponentInChildren<CreatureHybrid>();var toy=slice.GetComponentInChildren<BeachBall>();
  try{creature.Interrupt("Chase persistence QA");field.SetValue(slice,JsonUtility.FromJson<SliceState>(saved));typeof(QuestSlice).GetMethod("Save",F).Invoke(slice,null);typeof(QuestSlice).GetMethod("Load",F).Invoke(slice,null);var loaded=(SliceState)field.GetValue(slice);creature.NewSeason();toy.ResetWorld();Check(loaded.Valid()&&loaded.WorldPlay.Chase.games==1&&loaded.WorldPlay.Chase.preference>.5f&&!creature.FollowingHeldBall&&!toy.Chase.Active,"Native reload lost training or revived a held chase");}
  finally{field.SetValue(slice,original);creature.NewSeason();toy.ResetWorld();}
  Debug.Log("BWVR_UNIFIED_HELD_BALL_PASS: single feasible candidate and curiosity owner; native measured release; one real chase outcome and one-shot training; immediate catch selection; legacy interruption/stationary guards; native Save/Load");
 }
}
}
