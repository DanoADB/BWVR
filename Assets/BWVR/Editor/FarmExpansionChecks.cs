using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace BWVR.Editor {
public static class FarmExpansionChecks {
 static void Require(bool ok,string why){if(!ok)throw new Exception(why);}
 [MenuItem("BWVR/Verify new farm expansion (Play mode)")]
 public static void Verify(){
  Require(Application.isPlaying,"Enter Play mode");var model=new ResourceState{wood=4,wheat=3};var source=model.EnsureFarm("farm-0",new Vector3(20,2,20));Require(model.Valid()&&!model.Sow(source.id)&&model.Harvest(0,source.id,3)==0,"Unprepared farm supplied food");Require(model.PrepareFarm(source.id)&&!model.PrepareFarm(source.id)&&model.wood==2&&model.farmWoodUsed==2&&model.Sow(source.id),"Farm costs not exactly once");model.Grow(source.id,180);Require(model.Valid()&&source.remaining==12,"Prepared crop growth invalid");var bad=JsonUtility.FromJson<ResourceState>(JsonUtility.ToJson(model));bad.farmWoodUsed=0;Require(!bad.Valid(),"Invalid farm cost ledger accepted");
  var world=UnityEngine.Object.FindAnyObjectByType<VillageResources>();var slice=world.GetComponentInParent<QuestSlice>();var field=typeof(QuestSlice).GetField("state",BindingFlags.Instance|BindingFlags.NonPublic);var original=(SliceState)field.GetValue(slice);var positions=world.Gatherers.Select(a=>a.transform.position).ToArray();slice.GetComponentInChildren<CreatureHybrid>().Interrupt("Expansion QA");foreach(var a in world.Gatherers){a.Cancel();a.GetComponent<VillagerSocial>().ResetTask();a.GetComponent<VillagerExploration>().ResetTask();a.GetComponent<VillagerBlockPlay>().ResetTask();}
  var test=new SliceState{developmentFocus=true};test.Resources.wood=4;test.Resources.wheat=18;field.SetValue(slice,test);world.ResetWorld();
  try{
   var plots=world.sources.Where(s=>s.expansion).ToArray();Require(plots.Length>0&&plots.Length<=4&&world.sources.Count<=44,"No bounded suitable farm locations");var node=plots[0];var actor=world.Gatherers[0];var record=test.Resources.Find(node.id);
   Require(!node.visual.gameObject.activeSelf&&!node.sapling.gameObject.activeSelf&&!world.Available(node),"Potential plot looks like a free farm");
   foreach(var p in world.sources.Where(s=>s.kind==VillageResource.Wheat))test.Resources.Find(p.id).remaining=0;
   actor.transform.position=node.approach;actor.GetComponent<IslandNavigator>().ResetRoute();Require(actor.TryFoodReserve()&&actor.PreparingFarm,"Idle villager did not autonomously choose farm expansion");Require(!world.Gatherers[1].TryPrepareFarm(actor.Source)&&world.ReservedFarmWood==2&&world.ReservedSeed==1,"Farm reservations overlap");
   actor.Tick(40,true);Require(!record.farmPrepared&&test.Resources.wood==4,"Pause spent farm material");actor.GetComponent<CarryItem>().held=true;actor.Tick(.1f,false);actor.GetComponent<CarryItem>().held=false;Require(!actor.Active&&world.ReservedFarmWood==0&&test.Resources.wood==4&&!record.farmPrepared,"Canceled preparation spent resources");
   Require(actor.TryPrepareFarm(node),"Cannot retry preparation");for(int i=0;i<2000&&actor.Active;i++)actor.Tick(.05f,false);Require(record.farmPrepared&&test.Resources.wood==2&&test.Resources.farmWoodUsed==2&&record.remaining==0&&node.sapling.gameObject.activeSelf,"Native preparation did not charge/show soil exactly once");
   var saved=JsonUtility.FromJson<SliceState>(JsonUtility.ToJson(test));Require(saved.Valid()&&saved.Resources.Find(node.id).farmPosition==record.farmPosition,"Saved farm lost position/cost");field.SetValue(slice,saved);world.ResetWorld();record=saved.Resources.Find(node.id);
   Require(actor.TrySow(node),"New farm cannot be sown");for(int i=0;i<2000&&actor.Active;i++)actor.Tick(.05f,false);Require(record.growing&&saved.Resources.wheat==17&&saved.Resources.seedUsed==1,"New farm seed payment failed");saved.Resources.Grow(node.id,180);world.Refresh();Require(record.remaining==12&&world.Available(node),"New harvest unavailable");
   Require(actor.TryBegin(node),"New crop not harvestable");for(int i=0;i<4000&&actor.Active;i++)actor.Tick(.05f,false);Require(saved.Resources.wheat==20&&record.remaining==9&&saved.Valid(),"New farm harvest delivery invalid");
   Debug.Log("BWVR_FARM_EXPANSION_PASS: bounded reachable sites, autonomous preparation, timber/seed reservation, pause/cancel, exactly-once costs, persistent location, sowing/growth and native harvest delivery");
  }finally{foreach(var a in world.Gatherers)a.Cancel();field.SetValue(slice,original);world.ResetWorld();for(int i=0;i<positions.Length;i++){world.Gatherers[i].transform.position=positions[i];world.Gatherers[i].GetComponent<IslandNavigator>().ResetRoute();}}
 }
}
}
