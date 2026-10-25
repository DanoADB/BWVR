using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace BWVR.Editor {
public static class VillagerRepairChecks {
 [MenuItem("BWVR/Verify autonomous villager repairs (Play mode)")]
 public static void Verify(){
  if(!EditorApplication.isPlaying)throw new Exception("Enter Play mode first");
  var slice=UnityEngine.Object.FindAnyObjectByType<QuestSlice>();var god=UnityEngine.Object.FindAnyObjectByType<CreatureHybrid>();var learning=god.GetComponent<CreatureConstructionLearning>();var site=learning.sites[0];
  var field=typeof(QuestSlice).GetField("state",BindingFlags.Instance|BindingFlags.NonPublic);var original=(SliceState)field.GetValue(slice);var saved=site.Capture();var recipe=site.Recipe;
  var actors=slice.GetComponentsInChildren<VillagerExploration>();var actor=actors[0];var position=actor.transform.position;var nav=actor.GetComponent<IslandNavigator>();
  god.Interrupt("Repair QA");foreach(var a in actors){a.ResetTask();a.GetComponent<VillagerSocial>().ResetTask();a.GetComponent<VillagerBlockPlay>().ResetTask();}
  var test=new SliceState{water=100,crops=100,developmentFocus=true};field.SetValue(slice,test);var memory=actor.GetComponent<VillagerSocial>().Memory;
  void Require(bool ok,string why){if(!ok)throw new Exception(why);}
  void Approach(){Require(nav.world.Sample(site.World(new Vector3(0,0,-12)),nav.profile,12,out var at),"No repair approach");actor.transform.position=nav.world.ProjectFoot(at,nav.profile);nav.ResetRoute();}
  void Age(){for(int i=0;i<1100;i++)site.Advance(.02f);}
  void Run(int count=4000){for(int i=0;i<count&&actor.Active;i++){actor.Tick(.05f,false);site.Advance(.02f);}}
  try{
   site.Paused=false;site.ResetConstruction();site.Recipe=BuildRecipe.Bench;
   foreach(var p in ConstructionPatterns.Plan(site.Recipe)){var b=site.blocks.First(b=>!b.Attached&&(b.size.x>3)==p.beam);Require(site.Drop(b,site.World(p.position),site.transform.rotation,true),"Bench fixture did not snap");for(int i=0;i<5;i++)site.Advance(.02f);}
   Approach();Require(actor.TryStructure(site),"Could not use bench");Run();Require(memory.usefulStructures!=0&&memory.constructionUses==1,"Successful use did not remember this site/form");actor.ResetTask();
   var beam=site.blocks.First(b=>b.Shape==ConstructionShape.Beam&&b.Attached);site.ReturnLoose(beam);Age();Approach();
   int known=memory.usefulStructures;memory.usefulStructures=0;Require(!actor.TryRepair(site),"Unknown structure repaired");memory.usefulStructures=known;
   beam.Item.held=true;Require(!actor.TryRepair(site),"Player-held block stolen");beam.Item.held=false;
   float trustBefore=memory.trust;actor.Tick(36,false);Require(actor.Current==VillagerExploration.Phase.RepairFetch,"Idle villager did not autonomously choose known damaged bench");Require(!actors[1].TryRepair(site)&&site.Repairer==actor&&!beam.CanBuild,"Repair lease failed");
   var phase=actor.Current;actor.Tick(10,true);Require(actor.Current==phase&&!beam.Item.held,"Pause advanced repair");
   Run();Require(memory.repairs==1&&site.RecipeComplete&&site.Repairer==null&&!beam.Item.held&&!beam.creatureBuilt,"Native fetch/carry/snap/cleanup or villager attribution failed");Require(memory.trust==trustBefore,"Repair changed trust unexpectedly");actor.ResetTask();
   // Remembered experience survives reload; a different form at the same site is unknown.
   var restored=JsonUtility.FromJson<SliceState>(JsonUtility.ToJson(test));Require(restored.Valid()&&restored.Social(0).repairs==1&&restored.Social(0).usefulStructures==known,"Repair memory did not round-trip");
   var corrupt=JsonUtility.FromJson<SliceState>(JsonUtility.ToJson(test));corrupt.Social(0).usefulStructures=65536;Require(!corrupt.Valid(),"Unbounded structure memory accepted");
   site.Recipe=BuildRecipe.Arch;memory.nextRepairAt=0;Require(!actor.TryRepair(site),"Remembered bench generalized to unknown arch");site.Recipe=BuildRecipe.Bench;
   site.ReturnLoose(beam);Age();Approach();Require(actor.TryRepair(site),"Cancellation setup rejected");
   for(int i=0;i<2000&&actor.Current==VillagerExploration.Phase.RepairFetch;i++){actor.Tick(.05f,false);site.Advance(.02f);}
   Require(beam.Item.held,"Repair did not reach native carry");actor.ResetTask();Require(site.Repairer==null&&!beam.Item.held&&beam.GetComponent<Collider>().enabled&&!beam.Body.isKinematic,"Cancelled repair stranded a carried block");
   // Player grab during approach cancels without moving/releasing the player's object.
   site.ReturnLoose(beam);Age();memory.nextRepairAt=0;Approach();Require(actor.TryRepair(site),"Player interruption setup rejected");beam.Item.held=true;var heldAt=beam.transform.position;actor.Tick(.05f,false);Require(!actor.Active&&site.Repairer==null&&beam.Item.held&&beam.transform.position==heldAt,"Player grab cancellation changed player ownership");beam.Item.held=false;
   site.ResetConstruction();memory.nextRepairAt=0;Require(!actor.TryRepair(site),"Repair started construction from an empty tray");
   Debug.Log("BWVR_VILLAGER_REPAIR_PASS: native structure use teaches site/form; damaged bench fetch/carry/physical snap; pause; exclusive site lease; player-held and empty/unknown-form guards; villager attribution; save validation; cancellation releases carry; player takeover preserved");
  }finally{actor.ResetTask();site.Recipe=recipe;site.Restore(saved);actor.transform.position=position;nav.ResetRoute();field.SetValue(slice,original);god.Interrupt("Repair QA complete");}
 }
}
}
