using System;
using System.Reflection;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace BWVR.Editor {
public static class MiracleChecks {
 [MenuItem("BWVR/Verify guidance tools and miracles (Play mode)")]
 public static void Verify(){
  if(!EditorApplication.isPlaying)throw new Exception("Enter Play mode first");var slice=UnityEngine.Object.FindAnyObjectByType<QuestSlice>();var god=UnityEngine.Object.FindAnyObjectByType<CreatureHybrid>();var system=UnityEngine.Object.FindAnyObjectByType<DivineIntervention>();var stateField=typeof(QuestSlice).GetField("state",BindingFlags.Instance|BindingFlags.NonPublic);var original=(SliceState)stateField.GetValue(slice);god.Interrupt("Miracle QA");var test=new SliceState{water=20,crops=40};stateField.SetValue(slice,test);
  try{
   system.ResetWorld();if(system.tools.Count!=5||system.tools.Any(t=>t.GetComponent<CreatureAffordance>()!=null))throw new Exception("Physical cue inventory invalid");
   var target=god.Find("cistern");float energy=test.Hybrid.energy;test.Hybrid.receipt=new ActionReceipt{eligible=true,action=CreatureAction.WaterDelivery,situation=CreatureSituation.WaterShortage,target="water"};
   if(!system.TryUse(SpiritualTool.Rain,target)||Mathf.Abs(test.Hybrid.energy-(energy-35))>.001f||test.water!=42||test.crops!=46||test.Hybrid.rainRemaining!=6||test.gifts!=0)throw new Exception("Rain cost or deterministic outcome incorrect");
   if(system.TryUse(SpiritualTool.Rain,target)||test.water!=42||test.Hybrid.energy!=25)throw new Exception("Repeated gesture duplicated miracle");
   test.paused=true;system.Tick(10,true);if(test.Hybrid.rainRemaining!=6||test.Hybrid.energy!=25||system.TryUse(SpiritualTool.Heal,god.Find("villager-2")))throw new Exception("Paused miracle changed simulation");test.paused=false;test.Hybrid.time+=.6f;
   var patient=god.Find("villager-2");patient.villager.Memory.injury=.35f;
   if(!system.TryUse(SpiritualTool.Heal,patient)||patient.villager.Memory.injury!=0||test.Hybrid.energy!=0||test.water!=42||test.crops!=46)throw new Exception("Healing changed unrelated resources or failed cost");
   if(god.Feedback(true)||test.Hybrid.associations.Length!=0)throw new Exception("Miracle incorrectly became creature learning");test.Hybrid.time+=.6f;
   if(system.TryUse(SpiritualTool.Rain,target)||test.Hybrid.energy!=0||test.water!=42)throw new Exception("Unaffordable miracle applied");
   system.Suspended=true;system.Tick(20,false);if(test.Hybrid.energy!=0||system.TryUse(SpiritualTool.Help,target))throw new Exception("Suspension advanced effects or accepted tool");system.Suspended=false;
   system.Tick(10,false);if(Mathf.Abs(test.Hybrid.energy-1)>.001f||test.Hybrid.rainRemaining!=0)throw new Exception("Bounded recovery/effect expiry failed");
   if(system.TryUse(SpiritualTool.Protect,patient)||test.Hybrid.guidance.status!=GuidanceStatus.Rejected)throw new Exception("Protection invented a threat");
   if(!system.TryUse(SpiritualTool.Help,target)||test.Hybrid.guidance.target!="cistern")throw new Exception("Physical help cue missing");
   var restored=JsonUtility.FromJson<SliceState>(JsonUtility.ToJson(test));if(!restored.Valid()||restored.Hybrid.energy!=test.Hybrid.energy||restored.Social(2).injury!=0||restored.Hybrid.guidance.status!=GuidanceStatus.Active)throw new Exception("Energy/guidance/health did not save");
   var ended=new SliceState{seconds=0,water=0,crops=0};ended.RememberOutcome();ended.water=ended.crops=100;if(ended.Won)throw new Exception("Free-play resource change rewrote season outcome");
   Debug.Log("BWVR_MIRACLE_PASS: five physical tools; rain exact delta/cost; duplicate and affordability gates; pause/suspend; healing only injury; no miracle training; recovery/effect expiry; credible protection; help cue; saved energy/health/guidance; historical outcome retained");
  }finally{system.Suspended=false;god.Interrupt("Miracle QA complete");stateField.SetValue(slice,original);system.Tick(0,false);}
 }
}
}
