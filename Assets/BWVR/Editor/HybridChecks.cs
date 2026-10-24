using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace BWVR.Editor {
public static class HybridChecks {
 [MenuItem("BWVR/Verify hybrid decision model")]
 public static void VerifyModel(){
  var memory=new HybridState();var candidates=new List<CreatureCandidate>{new CreatureCandidate{action=CreatureAction.WaterDelivery,situation=CreatureSituation.WaterShortage,target="cistern",need=.5f,learned=.5f},new CreatureCandidate{action=CreatureAction.PlayWithToy,situation=CreatureSituation.PlayOpportunity,target="toy",need=.5f,learned=.5f}};
  int baseline=Count(candidates,.5f),praised=Count(candidates,.98f),discouraged=Count(candidates,.02f);
  if(praised<baseline+130||discouraged>baseline-130)throw new Exception("Context preference has no material choice effect");
  for(int i=0;i<100;i++)if(CreatureDecisionModel.Select(candidates,34,i).action!=CreatureDecisionModel.Select(candidates,34,i).action)throw new Exception("Seeded selection differs");
  candidates[0].feasible=false;candidates[0].urgency=100;if(CreatureDecisionModel.Select(candidates,1,1).action==CreatureAction.WaterDelivery)throw new Exception("Infeasible action selected");
  memory.receipt=new ActionReceipt{eligible=true,action=CreatureAction.WaterDelivery,situation=CreatureSituation.WaterShortage,target="cistern"};if(!memory.Reinforce(true)||memory.Reinforce(false)||memory.Preference(CreatureSituation.WaterShortage,CreatureAction.WaterDelivery)<=.5f)throw new Exception("Context reinforcement attribution failed");
  memory.receipt=new ActionReceipt{eligible=true,action=CreatureAction.EatFood,situation=CreatureSituation.Hungry};memory.time=12.01f;if(memory.Reinforce(true))throw new Exception("Expired context reinforced");
  var saved=JsonUtility.FromJson<HybridState>(JsonUtility.ToJson(memory));if(!saved.Valid()||saved.associations.Length!=1||saved.associations[0].samples!=1)throw new Exception("Context memory round trip failed");saved.energy=float.NaN;if(saved.Valid())throw new Exception("Nonfinite energy accepted");
  var journal=new WorldEventJournal();for(int i=0;i<100;i++)journal.Add(i,"complete",CreatureAction.PlayWithToy,"toy","");if(journal.Count!=48||journal.Capture()[0].time!=52||journal.Latest.time!=99)throw new Exception("Journal is not a bounded ordered ring");
  Debug.Log($"BWVR_HYBRID_MODEL_PASS: fixed seed; feasible gates; reinforcement baseline={baseline}/500, praised={praised}/500, discouraged={discouraged}/500; one receipt; expiry; save validation; 48-event journal");
 }
 static int Count(List<CreatureCandidate> candidates,float preference){candidates[0].learned=preference;int count=0;for(int i=0;i<500;i++)if(CreatureDecisionModel.Select(candidates,1347,i).action==CreatureAction.WaterDelivery)count++;return count;}
 [MenuItem("BWVR/Verify hybrid action coordination (Play mode)")]
 public static void VerifyRuntime(){
  if(!EditorApplication.isPlaying)throw new Exception("Enter Play mode first");
  var slice=UnityEngine.Object.FindAnyObjectByType<QuestSlice>();var hybrid=UnityEngine.Object.FindAnyObjectByType<CreatureHybrid>();var mind=hybrid.GetComponent<CreatureCuriosity>();var water=hybrid.GetComponent<CreatureRoutine>();var field=typeof(QuestSlice).GetField("state",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance);var original=(SliceState)field.GetValue(slice);
  bool focus=slice.developmentFocus;slice.developmentFocus=false;hybrid.Interrupt("QA setup");var test=new SliceState{water=14,hunger=.1f};field.SetValue(slice,test);
  try{
   hybrid.NewSeason();int before=water.deliveries;hybrid.Tick(.5f);if(hybrid.Owner!=CreatureHybrid.ActionOwner.Water||mind.Current!=CreatureCuriosity.Phase.Rest)throw new Exception("Critical shortage did not grant exclusive water ownership");
   if(hybrid.Guide(GuidanceIntent.Investigate,hybrid.Targets.Find(t=>t.affordance!=null&&t.affordance.kind==CreatureAffordance.Kind.Roll))||test.Hybrid.guidance.status!=GuidanceStatus.Rejected)throw new Exception("Urgent ownership did not explain rejected distraction");
   for(int i=0;i<2000&&water.deliveries==before;i++)hybrid.Tick(.1f);
   if(water.deliveries!=before+1||test.gifts!=1||test.Hybrid.receipt.action!=CreatureAction.WaterDelivery||test.Hybrid.receipt.situation!=CreatureSituation.WaterShortage)throw new Exception("Confirmed water receipt or outcome missing");
   test=JsonUtility.FromJson<SliceState>(JsonUtility.ToJson(test));field.SetValue(slice,test);mind.InvalidateFeedback();float care=test.care;if(!hybrid.Feedback(true)||test.care<=care||hybrid.Feedback(false)||test.Hybrid.Preference(CreatureSituation.WaterShortage,CreatureAction.WaterDelivery)<=.5f)throw new Exception("Restored context receipt and existing care did not learn exactly once");
   hybrid.Interrupt("Observe check");test.water=90;test.hunger=.05f;var toy=hybrid.Targets.Find(t=>t.affordance!=null&&t.affordance.kind==CreatureAffordance.Kind.Roll);
   if(!hybrid.Guide(GuidanceIntent.Investigate,toy))throw new Exception("Feasible investigation rejected");float time=test.Hybrid.time;test.paused=true;hybrid.Tick(40);if(test.Hybrid.time!=time||test.Hybrid.guidance.status!=GuidanceStatus.Active)throw new Exception("Pause advanced simulation/guidance");test.paused=false;
   int events=hybrid.Journal.Count;for(int i=0;i<1000&&test.Hybrid.guidance.status==GuidanceStatus.Active;i++)hybrid.Tick(.1f);
   if(test.Hybrid.guidance.status!=GuidanceStatus.Completed&&test.Hybrid.guidance.status!=GuidanceStatus.Expired)throw new Exception("Guidance neither completed nor expired");
   hybrid.Interrupt("Carry cleanup check");test.seconds=0;test.hunger=1;test.playPreference=test.socialPreference=0;foreach(var target in hybrid.Targets)if(target.affordance!=null)target.affordance.cooldown=0;
   for(int i=0;i<1400&&mind.Holding==null;i++)hybrid.Tick(.1f);var held=mind.Holding;if(held==null)throw new Exception("No curiosity ownership to test");
   hybrid.Suspend(true);if(held.held||!held.GetComponent<Collider>().enabled||hybrid.Owner!=CreatureHybrid.ActionOwner.None||test.Hybrid.receipt.eligible)throw new Exception("Suspend stranded ownership or eligible failed lesson");
   var saved=JsonUtility.FromJson<SliceState>(JsonUtility.ToJson(test));if(!saved.Valid()||!saved.Hybrid.intention.active)throw new Exception("Saved intention missing after safe suspension");time=test.Hybrid.time;hybrid.Tick(20);if(test.Hybrid.time!=time)throw new Exception("Suspended simulation advanced");hybrid.Suspend(false);
   hybrid.Suspend(true);test.seconds=180;test.water=15;test.crops=60;var gatherer=System.Array.Find(slice.GetComponentsInChildren<VillagerRoutine>(),v=>v.role==VillagerRoutine.Role.WaterGatherer);gatherer.ResetTask();int jobs=gatherer.completedJobs;bool improved=false;
   for(int i=0;i<1600&&gatherer.completedJobs==jobs;i++){float previous=test.water;slice.Simulate(.1f);if(test.water>previous)improved=true;}
   if(gatherer.completedJobs!=jobs+1||!improved)throw new Exception("Village made no independent resource progress while creature unavailable");hybrid.Suspend(false);
   Debug.Log("BWVR_HYBRID_RUNTIME_PASS: unified urgent water; exclusive owner; confirmed outcome/context; one reinforcement and legacy care; guidance rejection/expiry; pause; carried-object cleanup; saved resumable intention; no suspend advancement");
  }finally{hybrid.Suspend(false);hybrid.Interrupt("QA complete");slice.developmentFocus=focus;field.SetValue(slice,original);hybrid.NewSeason();}
 }
}
}
