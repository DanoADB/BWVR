using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace BWVR.Editor {
public static class PlaceContributionChecks {
 static void Require(bool ok,string why){if(!ok)throw new Exception(why);}
 public static void VerifyModel(){
  var log=new PlaceContributionLog();Require(log.Add(0,0,"player",false,0)&&log.Add(0,1,"creature",false,1)&&log.Add(0,2,"villager-2",true,2),"Typed placement sources rejected");Require(!log.Add(0,2,"villager-2",true,2)&&log.records.Length==3,"Duplicate placement receipt");
  Require(!log.Add(3,0,"player",false,3)&&!log.Add(0,48,"player",false,3)&&!log.Add(0,0,"villager-12",false,3)&&!log.Add(0,0,"villager-02",false,3)&&!log.Add(0,0,"player",true,3)&&!log.Add(0,0,"player",false,float.NaN),"Invalid actor/site/block/time accepted");
  var state=new SliceState{placeContributions=log};state.Hybrid.time=2;var page=CreationBookText.Read(state,CreationBookSection.Places,1,new[]{"Alden","Mira","Rowan"});Require(page.text.Contains("Player: 1 snaps")&&page.text.Contains("Creature: 1 snaps")&&page.text.Contains("Rowan: 1 snaps · 1 repairs"),"Book mixed actor contributions");
  string before=JsonUtility.ToJson(state);for(int i=0;i<10;i++)CreationBookText.Read(state,CreationBookSection.Places,i);Require(before==JsonUtility.ToJson(state),"Book created or changed contribution data");Require(JsonUtility.FromJson<SliceState>(before).Valid()&&new SliceState().Valid(),"Contribution/legacy save invalid");
  for(int i=3;i<100;i++)Require(log.Add(i%3,i%48,"player",false,i),"Bounded receipt append failed");state.Hybrid.time=99;Require(log.records.Length==48&&log.Valid(99),"Contribution history unbounded/invalid");log.records[0].actor="anonymous";Require(!state.Valid(),"Invalid attribution accepted");Debug.Log("BWVR_PLACE_CONTRIBUTION_MODEL_PASS");
 }
 public static void VerifyRuntime()=>VerifyWithCapture(null);
 public static void VerifyWithCapture(Action<QuestSlice> capture){
  Require(Application.isPlaying,"Enter Play mode");var slice=UnityEngine.Object.FindAnyObjectByType<QuestSlice>();var god=slice.GetComponentInChildren<CreatureHybrid>();var site=god.GetComponent<CreatureConstructionLearning>().sites[0];var flags=BindingFlags.Instance|BindingFlags.NonPublic;var field=typeof(QuestSlice).GetField("state",flags);var original=field.GetValue(slice);var records=site.Capture();var recipe=site.Recipe;bool paused=site.Paused;var held=(CarryItem[])typeof(QuestSlice).GetField("held",flags).GetValue(slice);var previousHeld=(CarryItem[])held.Clone();var test=new SliceState{developmentFocus=true,observeWithoutFoodWork=true};test.Hybrid.time=100;god.Interrupt("Contributions QA");foreach(var person in slice.GetComponentsInChildren<VillagerExploration>()){person.ResetTask();person.GetComponent<VillagerSocial>().ResetTask();person.GetComponent<VillagerResourceGathering>().Cancel();person.GetComponent<VillagerBlockPlay>().ResetTask();}field.SetValue(slice,test);var palette=slice.GetComponentInChildren<GodToolPalette>();palette.Cancel();palette.Process(new Ray(Camera.main.transform.position,Camera.main.transform.forward),false,true,false,.1f,false);
  try{
   site.Paused=false;site.ResetConstruction();site.Recipe=BuildRecipe.Bench;var plan=ConstructionPatterns.Plan(BuildRecipe.Bench);var playerBlock=site.blocks.First(b=>b.Shape==ConstructionShape.Brick);Require((bool)typeof(QuestSlice).GetMethod("TryGrab",flags).Invoke(slice,new object[]{1,playerBlock.Item}),"Native player grab refused");Physics.SyncTransforms();var ray=new Ray(site.World(new Vector3(plan[0].position.x,12,plan[0].position.z)),Vector3.down);typeof(QuestSlice).GetMethod("Release",flags).Invoke(slice,new object[]{1,ray});Require(playerBlock.Attached&&test.placeContributions?.records.Length==1&&test.placeContributions.records[0].actor=="player"&&test.placeContributions.records[0].site==0,"Native player release did not create site-specific receipt");
   Require(site.Drop(playerBlock,playerBlock.transform.position,playerBlock.transform.rotation,false,"player"),"Repeat-snap staging failed");Require(test.placeContributions.records.Length==1,"Stationary repeated Drop farmed credit");
   var creatureBlock=site.blocks.First(b=>b.Shape==ConstructionShape.Brick&&!b.Attached);Require(site.Drop(creatureBlock,site.World(plan[1].position),site.transform.rotation,true),"Creature boundary snap failed");Require(test.placeContributions.records.Last().actor=="creature","Creature placement labeled as player");
   var beam=site.blocks.First(b=>b.Shape==ConstructionShape.Beam&&!b.Attached);Require(site.Drop(beam,site.World(plan[2].position),site.transform.rotation),"Anonymous staging snap failed");Require(test.placeContributions.records.Length==2,"Anonymous/fixture placement was inferred as player");
   string ledger=JsonUtility.ToJson(test.placeContributions);var geometrySnapshot=site.Capture();site.Restore(geometrySnapshot);Require(JsonUtility.ToJson(test.placeContributions)==ledger,"Restore fabricated placement history");
   site.ReturnLoose(beam);test.paused=true;Require(site.Drop(beam,site.World(plan[2].position),site.transform.rotation,true),"Paused geometry fixture failed");test.paused=false;Require(test.placeContributions.records.Length==2,"Paused placement recorded");
   site.ReturnLoose(beam);Require(!site.Drop(beam,site.World(new Vector3(500,0,0)),site.transform.rotation,false,"player"),"Out-of-bounds drop unexpectedly snapped");Require(test.placeContributions.records.Length==2,"Failed placement rewarded");
   var saved=JsonUtility.FromJson<SliceState>(JsonUtility.ToJson(test));Require(saved.Valid()&&saved.placeContributions.records.Length==2,"Runtime contribution save invalid");capture?.Invoke(slice);Debug.Log("BWVR_PLACE_CONTRIBUTION_RUNTIME_PASS: native player grab/release/raycast snap, creature boundary, anonymous exclusion, repeated-drop guard, pause/failure/restore safety, valid saved attribution");
  }finally{for(int i=0;i<held.Length;i++)held[i]=previousHeld[i];foreach(var b in site.blocks)b.Item.held=false;site.Recipe=recipe;site.Restore(records);site.Paused=paused;field.SetValue(slice,original);god.Interrupt("Contributions QA complete");}
 }
}
}
