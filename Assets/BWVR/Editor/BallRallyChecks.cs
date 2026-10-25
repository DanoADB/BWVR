using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;
namespace BWVR.Editor {
public static class BallRallyChecks {
 [MenuItem("BWVR/Verify villager beachball rallies (Play mode)")]
 public static void Verify(){
  if(!EditorApplication.isPlaying)throw new Exception("Enter Play mode");
  var ball=UnityEngine.Object.FindAnyObjectByType<BeachBall>();var god=ball.Playmate;var mind=god.GetComponent<CreatureCuriosity>();var slice=UnityEngine.Object.FindAnyObjectByType<QuestSlice>();var field=typeof(QuestSlice).GetField("state",BindingFlags.Instance|BindingFlags.NonPublic);var original=(SliceState)field.GetValue(slice);
  god.Interrupt("Rally QA");var test=new SliceState{water=100,developmentFocus=true};field.SetValue(slice,test);
  var residents=slice.GetComponentsInChildren<VillagerExploration>();var positions=residents.Select(a=>a.transform.position).ToArray();var godPosition=god.transform.position;var actor=residents[0];var nav=actor.GetComponent<IslandNavigator>();float scale=nav.world.Scale;var initialHome=ball.GetComponent<CarryItem>().home;
  void Step(){test.Hybrid.time+=.02f;mind.Tick(.02f,test,false);actor.Tick(.02f,false);ball.Advance(.02f,false);}
  void Reset(){god.Interrupt("Rally fixture");foreach(var a in residents){a.ResetTask();a.GetComponent<VillagerSocial>().ResetTask();a.GetComponent<VillagerBlockPlay>().ResetTask();}ball.ReturnHome();ball.GetComponent<CreatureAffordance>().cooldown=0;actor.GetComponent<VillagerSocial>().Memory.trust=.65f;actor.GetComponent<VillagerSocial>().Memory.injury=0;}
  bool Stage(){for(int i=0;i<16;i++){Reset();var d=Quaternion.Euler(0,i*22.5f,0)*Vector3.forward;var center=ball.transform.position;if(!nav.world.Sample(center+d*scale*24,nav.world.creature,2,out var g)||!nav.world.Sample(center-d*scale*6,nav.profile,2,out var v))continue;god.transform.position=nav.world.ProjectFoot(g,nav.world.creature);actor.transform.position=nav.world.ProjectFoot(v,nav.profile);god.GetComponent<IslandNavigator>().ResetRoute();nav.ResetRoute();if(Mathf.Abs(god.transform.position.y-(center.y-ball.Radius*scale))>scale*1.1f)continue;if(actor.TryBall(ball)&&(bool)typeof(BeachBall).GetMethod("ClearLane",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(ball,new object[]{god.transform.position}))return true;}return false;}
  try{
   if(!Stage())throw new Exception("No safe initial rally lane on native island");
   if(residents[1].TryBall(ball))throw new Exception("Two villagers reserved one ball");
   var pos=actor.transform.position;int kicks=test.WorldPlay.villagerKicks;actor.Tick(2,true);ball.Advance(2,true);if(actor.transform.position!=pos||test.WorldPlay.villagerKicks!=kicks)throw new Exception("Paused play advanced");
   god.Perceive();var candidates=(System.Collections.Generic.List<CreatureCandidate>)typeof(CreatureHybrid).GetField("candidates",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(god);var offer=candidates.Single(c=>c.target==ball.StableId);if(offer.situation!=CreatureSituation.SafeContact||offer.affinity<.3f)throw new Exception("Invitation missing from utility model");
   typeof(CreatureHybrid).GetMethod("BeginAction",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(god,new object[]{offer,god.Find(ball.StableId)});if(god.Owner!=CreatureHybrid.ActionOwner.Curiosity)throw new Exception("Creature refused native ball action");
   for(int i=0;i<4000&&(mind.Current!=CreatureCuriosity.Phase.Rest||i<2);i++)Step();
   if(ball.RallyReceptions<4||test.WorldPlay.rallyReturns<3||test.WorldPlay.villagerKicks<2||test.WorldPlay.kicks<2)throw new Exception("No native alternating rally: receptions="+ball.RallyReceptions+", villager kicks="+test.WorldPlay.villagerKicks+", creature kicks="+test.WorldPlay.kicks+", phase="+mind.Current+", player="+actor.Current+", ball="+ball.transform.position+", god="+god.transform.position+", villager="+actor.transform.position);
   if(test.Hybrid.Interest(BeachBall.RallyInterest)?.appeal<=.5f||actor.GetComponent<VillagerSocial>().Memory.ballGames!=1)throw new Exception("Shared returns did not teach a favorite/social experience");
   if(test.WorldPlay.GentleBall.practices<1||test.WorldPlay.GentleBall.control<=.2f)throw new Exception("Confirmed creature return did not practice control");
   if(!test.Hybrid.receipt.gentleRally||!test.Hybrid.receipt.eligible)throw new Exception("Native completed rally did not create a gentle teaching receipt");
   float preference=test.WorldPlay.GentleBall.preference;if(!god.Feedback(true)||test.WorldPlay.GentleBall.preference<=preference||god.Feedback(true))throw new Exception("Native rally teaching attribution failed");
   int returns=test.WorldPlay.rallyReturns;for(int i=0;i<100;i++)ball.Advance(.02f,false);if(test.WorldPlay.rallyReturns!=returns)throw new Exception("Stationary reward farming");
   if(!Stage())throw new Exception("Could not stage cancellation");ball.GetComponent<CarryItem>().held=true;ball.Advance(.02f,false);actor.Tick(.02f,false);if(ball.Player!=null||actor.Active)throw new Exception("Player grab did not release rally ownership");ball.GetComponent<CarryItem>().held=false;
   if(!Stage())throw new Exception("Could not stage solo play");god.transform.position+=Vector3.right*scale*80;int solo=test.WorldPlay.villagerKicks;for(int i=0;i<2000&&test.WorldPlay.villagerKicks==solo;i++){test.Hybrid.time+=.02f;actor.Tick(.02f,false);ball.Advance(.02f,false);}if(test.WorldPlay.villagerKicks<=solo||test.WorldPlay.rallyReturns!=returns)throw new Exception("Independent villager nudge failed or fabricated shared reward");
   var save=JsonUtility.FromJson<SliceState>(JsonUtility.ToJson(test));if(!save.Valid()||save.WorldPlay.rallyReturns!=returns||save.Hybrid.Interest(BeachBall.RallyInterest)==null)throw new Exception("Rally learning did not save");save.WorldPlay.rallyFun=float.NaN;if(save.Valid())throw new Exception("Invalid rally learning accepted");
   Debug.Log("BWVR_RALLY_PASS: native villager approach/nudge, creature decision opportunity, four alternating physical receptions, bounded social learning, no stationary/solo social farming, exclusive reservation, pause/grab cancellation and save validation");
  }finally{god.Interrupt("Rally QA complete");foreach(var a in residents)a.ResetTask();field.SetValue(slice,original);for(int i=0;i<residents.Length;i++){residents[i].transform.position=positions[i];residents[i].GetComponent<IslandNavigator>().ResetRoute();}god.transform.position=godPosition;god.GetComponent<IslandNavigator>().ResetRoute();ball.GetComponent<CarryItem>().home=initialHome;ball.ResetWorld();}
 }
}
}
