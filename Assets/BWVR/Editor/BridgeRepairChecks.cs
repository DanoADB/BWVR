using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace BWVR.Editor {
public static class BridgeRepairChecks {
 [MenuItem("BWVR/Verify villager bridge maintenance (Play mode)")]
 public static void Verify(){
  if(!EditorApplication.isPlaying)throw new Exception("Enter Play mode first");
  var slice=UnityEngine.Object.FindAnyObjectByType<QuestSlice>();var god=UnityEngine.Object.FindAnyObjectByType<CreatureHybrid>();var learning=god.GetComponent<CreatureConstructionLearning>();var bridge=learning.Bridge;if(bridge==null)throw new Exception("No creek crossing");var site=bridge.Site;
  var field=typeof(QuestSlice).GetField("state",BindingFlags.Instance|BindingFlags.NonPublic);var original=(SliceState)field.GetValue(slice);var records=site.Capture();bool locked=site.Locked;
  var actor=slice.GetComponentsInChildren<VillagerExploration>()[0];var position=actor.transform.position;var nav=actor.GetComponent<IslandNavigator>();
  god.Interrupt("Bridge maintenance QA");foreach(var a in slice.GetComponentsInChildren<VillagerExploration>()){a.ResetTask();a.GetComponent<VillagerSocial>().ResetTask();a.GetComponent<VillagerBlockPlay>().ResetTask();}
  var test=new SliceState{water=100,crops=100,developmentFocus=true};field.SetValue(slice,test);var memory=actor.GetComponent<VillagerSocial>().Memory;
  void Require(bool ok,string message){if(!ok)throw new Exception(message);}
  void Bank(){actor.ResetTask();actor.transform.position=bridge.BankA;nav.ResetRoute();}
  void Run(){for(int i=0;i<4800&&actor.Active;i++){actor.Tick(.05f,false);site.Advance(.02f);if(bridge.OverWater(actor.transform.position))Require(bridge.Walkable,"Repairer walked into broken crossing");}}
  try{
   site.Paused=false;site.Locked=false;site.ResetConstruction();
   foreach(var p in ConstructionPatterns.Plan(BuildRecipe.Bridge,site.LayerHeight)){var block=site.blocks.First(b=>!b.Attached&&(b.size.x>3)==p.beam);Require(site.Drop(block,site.World(p.position),site.transform.rotation,true),"Bridge fixture failed");for(int i=0;i<5;i++)site.Advance(.02f);}
   Require(bridge.Walkable,"Complete bridge not walkable");Bank();Require(actor.TryBridge(bridge),"Initial crossing rejected");Run();Require(memory.bridgeCrossings==1&&(memory.usefulStructures&(1<<15))!=0,"Crossing did not establish site knowledge");
   var beam=site.blocks.Single(b=>b.size.x>3);site.ReturnLoose(beam);Require(!bridge.Walkable,"Broken bridge remained open");for(int i=0;i<1100;i++)site.Advance(.02f);Bank();
   int known=memory.usefulStructures;memory.usefulStructures=0;Require(!actor.TryRepair(site),"Unfamiliar bridge repaired");memory.usefulStructures=known;
   beam.Item.held=true;Require(!actor.TryRepair(site),"Player-held span stolen");beam.Item.held=false;
   actor.Tick(36,false);Require(actor.Current==VillagerExploration.Phase.RepairFetch,"Idle villager did not choose bridge maintenance");var phase=actor.Current;actor.Tick(10,true);Require(actor.Current==phase&&!bridge.Walkable,"Paused repair advanced/opened bridge");
   Run();Require(memory.repairs==1&&site.RecipeComplete&&bridge.Walkable&&site.Repairer==null&&!beam.Item.held&&!beam.creatureBuilt,"Repair did not restore physical span/navigation or attribution");
   Bank();Require(actor.TryBridge(bridge),"Repaired crossing rejected");Run();Require(memory.bridgeCrossings==2,"Native crossing after repair did not finish");
   learning.Capture();var saved=JsonUtility.FromJson<SliceState>(JsonUtility.ToJson(test));Require(saved.Valid()&&saved.Social(0).repairs==1&&saved.Building.bridge.Count(r=>r.support>=-1)==3,"Repaired bridge/experience did not save");
   bridge.Restore(saved.Building.bridge,true);Require(bridge.Walkable,"Reloaded repair did not restore navigation");
   Debug.Log("BWVR_BRIDGE_REPAIR_PASS: native crossing teaches useful site; idle maintenance choice; no unknown/player-held repair; pause; bank-side carry/place without entering broken crossing; physical span and native routing restored; second native crossing; saved pieces and villager experience");
  }finally{actor.ResetTask();bridge.Restore(records,!locked);actor.transform.position=position;nav.ResetRoute();field.SetValue(slice,original);god.Interrupt("Bridge maintenance QA complete");}
 }
}
}
