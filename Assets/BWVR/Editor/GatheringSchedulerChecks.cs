using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace BWVR.Editor {
// Unlike the focused controller fixtures, only QuestSlice chooses/ticks activities here.
public static class GatheringSchedulerChecks {
 [MenuItem("BWVR/Verify gathering through village scheduler (Play mode)")]
 public static void Verify(){
  void Require(bool ok,string why){if(!ok)throw new Exception(why);}
  Require(EditorApplication.isPlaying,"Enter Play mode first");
  var slice=UnityEngine.Object.FindAnyObjectByType<QuestSlice>();
  var god=slice.GetComponentInChildren<CreatureHybrid>();
  var actors=slice.GetComponentsInChildren<VillagerExploration>();
  var sites=slice.GetComponentsInChildren<ConstructionSite>();
  var props=slice.GetComponentsInChildren<CarryItem>();
  Require(actors.Length==VillagePopulation.Adults&&!props.Any(p=>p.held),"Expected the full village with no held objects");
  var field=typeof(QuestSlice).GetField("state",BindingFlags.Instance|BindingFlags.NonPublic);
  var original=(SliceState)field.GetValue(slice);
  var positions=props.Select(p=>p.transform.position).ToArray();var rotations=props.Select(p=>p.transform.rotation).ToArray();
  var geometry=sites.Select(s=>s.Capture()).ToArray();var recipes=sites.Select(s=>s.Recipe).ToArray();var paused=sites.Select(s=>s.Paused).ToArray();
  var weather=slice.GetComponentInChildren<IslandWeather>();bool automatic=weather.automatic,focus=slice.developmentFocus,food=slice.observeWithoutFoodWork;
  var test=new SliceState{developmentFocus=true,observeWithoutFoodWork=true,water=100,crops=100};test.Hybrid.seed=1347;
  void Cancel(){god.Interrupt("Scheduler QA cleanup");foreach(var a in actors){a.GetComponent<VillagerSocial>().ResetTask();a.ResetTask();a.GetComponent<VillagerResourceGathering>().Cancel();a.GetComponent<VillagerBlockPlay>().ResetTask();}}
  void ResetWorld(){slice.GetComponentInChildren<VillageFamilies>().ResetWorld();slice.GetComponentInChildren<VillageResources>().ResetWorld();slice.GetComponentInChildren<TreePlayWorld>().ResetWorld();slice.GetComponentInChildren<VillageTributes>().ResetWorld();slice.GetComponentInChildren<VillageCathedral>().ResetWorld();foreach(var b in slice.GetComponentsInChildren<BeachBall>())b.ResetWorld();weather.ResetWorld();}
  try{
   Cancel();field.SetValue(slice,test);god.NewSeason();ResetWorld();weather.automatic=false;slice.developmentFocus=slice.observeWithoutFoodWork=true;
   // Initial conditions only: a useful bench and tired neighbors near it. No action
   // starts, private cooldown edits, target isolation or fabricated completion receipts.
   var site=god.GetComponent<CreatureConstructionLearning>().sites[0];site.Paused=false;site.ResetConstruction();site.Recipe=BuildRecipe.Bench;
   foreach(var p in ConstructionPatterns.Plan(BuildRecipe.Bench)){var block=site.blocks.First(b=>!b.Attached&&(b.size.x>3)==p.beam);Require(site.Drop(block,site.World(p.position),site.transform.rotation),"Could not stage useful bench");for(int i=0;i<5;i++)site.Advance(.02f);}
   for(int i=0;i<actors.Length;i++){var nav=actors[i].GetComponent<IslandNavigator>();Require(nav.world.Sample(site.World(new Vector3((i%4-1.5f)*5,0,-6-i/4*6)),nav.profile,8,out var at),"No village gathering approach");actors[i].transform.position=nav.world.ProjectFoot(at,nav.profile);actors[i].GetComponent<VillagerSocial>().Memory.restNeed=.9f;nav.ResetRoute();}
   var actions=new System.Collections.Generic.HashSet<CreatureAction>();int completions=0;float lastComplete=-1;
   for(int step=0;step<6000;step++){
    slice.Simulate(.05f);foreach(var yard in sites)yard.Advance(.05f);
    var receipt=test.Hybrid.receipt;if(receipt.eligible&&receipt.completedAt>lastComplete){lastComplete=receipt.completedAt;actions.Add(receipt.action);completions++;}
   }
   int uses=actors.Sum(a=>a.GetComponent<VillagerSocial>().Memory.constructionUses),rests=actors.Sum(a=>a.GetComponent<VillagerSocial>().Memory.sharedRests),visitors=actors.Count(a=>a.GetComponent<VillagerSocial>().Memory.Place(0)?.visits>0);
   Require(uses>=2&&visitors>=2,"Normal scheduler did not produce independent bench visits: uses="+uses+", visitors="+visitors);
   // Shared invitations are contingent choices, not a required fixed sequence.
   Require(completions>=2&&actions.Count>=2,"Creature did not complete varied autonomous actions");
   float time=test.Hybrid.time;int visits=uses;test.paused=true;
   for(int i=0;i<100;i++){slice.Simulate(.05f);foreach(var yard in sites)yard.Advance(.05f);}
   Require(test.Hybrid.time==time&&actors.Sum(a=>a.GetComponent<VillagerSocial>().Memory.constructionUses)==visits,"Paused scheduler advanced logical progress");test.paused=false;
   test.construction=site.Capture();slice.GetComponentInChildren<IslandDiscoveries>().Capture();god.GetComponent<CreatureConstructionLearning>().Capture();
   var saved=JsonUtility.FromJson<SliceState>(JsonUtility.ToJson(test));Require(saved.Valid(),"Integrated scheduler produced invalid save state");
   Require(saved.relationships.Sum(m=>m.sharedRests)==rests&&saved.construction.Length==site.blocks.Count,"Integrated save lost shared rests or construction");
   Debug.Log("BWVR_GATHERING_SCHEDULER_PASS: 300s, "+actors.Length+" villagers, "+uses+" uses, "+visitors+" visitors, "+rests+" shared-rest participants, "+completions+" creature completions across "+actions.Count+" categories; pause and save roundtrip passed");
  }finally{
   Cancel();field.SetValue(slice,original);god.NewSeason();ResetWorld();
   for(int i=0;i<sites.Length;i++){sites[i].Recipe=recipes[i];sites[i].Restore(geometry[i]);sites[i].Paused=paused[i];}
   for(int i=0;i<props.Length;i++)if(props[i]!=null)props[i].transform.SetPositionAndRotation(positions[i],rotations[i]);
   foreach(var a in actors)a.GetComponent<IslandNavigator>().ResetRoute();god.GetComponent<IslandNavigator>().ResetRoute();
   weather.automatic=automatic;slice.developmentFocus=focus;slice.observeWithoutFoodWork=food;Physics.SyncTransforms();
  }
 }
}
}
