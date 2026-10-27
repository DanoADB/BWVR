using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace BWVR.Editor {
public static class GatheringChecks {
 [MenuItem("BWVR/Verify shared bench gatherings (Play mode)")]
 public static void Verify()=>Run(0);
 public static void VerifyRain()=>Run(1);
 static void Run(float rain){
  if(!EditorApplication.isPlaying)throw new Exception("Enter Play mode first");
  void Require(bool ok,string why){if(!ok)throw new Exception(why);}
  var slice=UnityEngine.Object.FindAnyObjectByType<QuestSlice>();var god=UnityEngine.Object.FindAnyObjectByType<CreatureHybrid>();
  var actors=slice.GetComponentsInChildren<VillagerExploration>();var host=actors[0];var guest=actors[1];var site=god.GetComponent<CreatureConstructionLearning>().sites[0];
  var field=typeof(QuestSlice).GetField("state",BindingFlags.Instance|BindingFlags.NonPublic);var original=(SliceState)field.GetValue(slice);
  var positions=actors.Select(a=>a.transform.position).ToArray();var records=site.Capture();var recipe=site.Recipe;
  god.Interrupt("Gathering QA");foreach(var a in actors){a.ResetTask();a.GetComponent<VillagerSocial>().ResetTask();a.GetComponent<VillagerResourceGathering>().Cancel();a.GetComponent<VillagerBlockPlay>().ResetTask();}
  var test=new SliceState{developmentFocus=true,water=100,crops=100};field.SetValue(slice,test);test.Weather.rain=rain;slice.GetComponentInChildren<VillageFamilies>().ResetWorld();
  void Step(){test.Hybrid.time+=.05f;host.Tick(.05f,false);guest.Tick(.05f,false);site.Advance(.02f);}
  void Stage(){foreach(var a in actors){a.ResetTask();var nav=a.GetComponent<IslandNavigator>();Require(nav.world.Sample(site.World(new Vector3(a==host?0:4,0,-6)),nav.profile,8,out var at),"No gathering approach");a.transform.position=nav.world.ProjectFoot(at,nav.profile);nav.ResetRoute();}host.ConfigurePeers(new[]{guest});}
  try{
   site.Paused=false;site.ResetConstruction();site.Recipe=BuildRecipe.Bench;
   foreach(var p in ConstructionPatterns.Plan(BuildRecipe.Bench)){var block=site.blocks.First(b=>!b.Attached&&(b.size.x>3)==p.beam);Require(site.Drop(block,site.World(p.position),site.transform.rotation),"Bench staging failed");for(int i=0;i<5;i++)site.Advance(.02f);}
   Stage();Require(host.TryStructure(site),"Host rejected bench");
   for(int i=0;i<1200&&!guest.Gathering;i++){test.Hybrid.time+=.05f;host.Tick(.05f,false);}
   Require(guest.Current==VillagerExploration.Phase.GatheringGuest&&host.Gathering,"Host did not invite nearby neighbor");
   Require(!actors[2].TryJoinGathering(host),"Second guest stole invitation");var phase=guest.Current;guest.Tick(10,true);Require(guest.Current==phase&&test.Social(1).sharedRests==0,"Paused gathering progressed");
   for(int i=0;i<600&&test.Social(1).sharedRests==0;i++)Step();
   Require(test.Social(0).sharedRests==1&&test.Social(1).sharedRests==1,"Native approach/shared rest did not complete");
   for(int i=0;i<600&&host.Current==VillagerExploration.Phase.UseStructure;i++)Step();
   Require(test.Social(0).Place(0).visits==1,"Host bench completion credited the same visit again");
   Require(!guest.TryJoinGathering(host),"Gathering cooldown allowed immediate repeat");
   Require(test.Social(0).Place(0)?.visits==1&&test.Social(1).Place(0)?.visits==1&&test.Social(1).Place(0).uncomfortable==(rain==0?0:1),"Shared place memory missing or weather ignored");
   string aId=host.GetComponent<HybridTarget>().id,bId=guest.GetComponent<HybridTarget>().id;
   Require(rain==0?(test.Social(0).Peer(bId)?.good==1&&test.Social(1).Peer(aId)?.good==1):(test.Social(0).Peer(bId)==null&&test.Social(1).Peer(aId)==null),"Reciprocal friendship missing");
   Require(rain==0?(test.Social(0).restNeed<.5f&&test.Social(1).restNeed<.5f):(test.Social(0).restNeed>.5f&&test.Social(1).restNeed>.5f),"Shared rest did not satisfy participants");
   var saved=JsonUtility.FromJson<SliceState>(JsonUtility.ToJson(test));Require(saved.Valid()&&saved.Social(1).sharedRests==1&&saved.Social(1).nextGatheringAt>test.Hybrid.time,"Gathering memory failed save");
   saved.Social(1).sharedRests=-1;Require(!saved.Valid(),"Invalid gathering memory accepted");
   Stage();test.Social(0).nextGatheringAt=test.Social(1).nextGatheringAt=0;Require(host.TryStructure(site),"Host restart failed");
   for(int i=0;i<1200&&!guest.Gathering;i++){test.Hybrid.time+=.05f;host.Tick(.05f,false);}
   Require(guest.Gathering,"Second invitation failed");guest.GetComponent<CarryItem>().held=true;guest.Tick(.05f,false);guest.GetComponent<CarryItem>().held=false;
   Require(!host.Gathering&&!guest.Gathering&&test.Social(1).sharedRests==1,"Player pickup did not cancel without reward");
   Stage();test.Social(0).nextGatheringAt=test.Social(1).nextGatheringAt=0;guest.GetComponent<VillagerSocial>().Memory.injury=.2f;
   Require(host.TryStructure(site),"Injury host staging failed");for(int i=0;i<1200&&host.Current!=VillagerExploration.Phase.UseStructure;i++){test.Hybrid.time+=.05f;host.Tick(.05f,false);}
   Require(!guest.Gathering&&!guest.TryJoinGathering(host),"Injured guest joined gathering");guest.GetComponent<VillagerSocial>().Memory.injury=0;
   Require(guest.TryJoinGathering(host),"Healthy guest could not join");site.ResetConstruction();guest.Tick(.05f,false);Require(!guest.Gathering&&!host.Gathering&&test.Social(1).sharedRests==1,"Destroyed bench retained gathering/reward");
   Debug.Log("BWVR_GATHERING_PASS: native invitation/approach, exclusive guest, pause, reciprocal friendship/rest, bounded saved place memory, weather-sensitive outcomes, cooldown, pickup cancellation, injury gate, destroyed-bench cleanup");
  }finally{foreach(var a in actors){a.GetComponent<CarryItem>().held=false;a.ResetTask();a.ConfigurePeers(actors);}site.Recipe=recipe;site.Restore(records);for(int i=0;i<actors.Length;i++){actors[i].transform.position=positions[i];actors[i].GetComponent<IslandNavigator>().ResetRoute();}field.SetValue(slice,original);}
 }
}
}
