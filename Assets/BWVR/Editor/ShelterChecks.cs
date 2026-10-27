using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace BWVR.Editor {
public static class ShelterChecks {
 static void Require(bool ok,string reason){if(!ok)throw new Exception(reason);}
 [MenuItem("BWVR/Verify useful shelter memory")]
 public static void VerifyModel(){
  var dry=new SocialMemory{restNeed=1};dry.RememberShelter(6,0);var wet=new SocialMemory{restNeed=1};wet.RememberShelter(6,1);
  Require(dry.shelterRests==1&&Mathf.Abs(dry.restNeed-.55f)<.001f&&Mathf.Abs(wet.restNeed-.35f)<.001f,"Cover does not provide measured rain-sensitive rest relief");
  var saved=JsonUtility.FromJson<SocialMemory>(JsonUtility.ToJson(wet));Require(saved.Valid()&&saved.shelterRests==1&&saved.shelteredSeconds==6,"Shelter experience did not save");
  Require(JsonUtility.FromJson<SocialMemory>("{}").Valid(),"Historical social memory rejected");saved.shelteredSeconds=-1;Require(!saved.Valid(),"Invalid shelter duration accepted");
  Debug.Log("BWVR_SHELTER_MODEL_PASS: bounded rest benefit, larger wet-weather relief, valid legacy memory and save round trip");
 }
 [MenuItem("BWVR/Verify useful shelter geometry and visits (Play mode)")]
 public static void VerifyRuntime(){
  Require(EditorApplication.isPlaying,"Enter Play mode");var slice=UnityEngine.Object.FindAnyObjectByType<QuestSlice>();var god=slice.GetComponentInChildren<CreatureHybrid>();var site=god.GetComponent<CreatureConstructionLearning>().sites.First(s=>!s.IsBridge);
  var actors=slice.GetComponentsInChildren<VillagerExploration>();var actor=actors[0];var nav=actor.GetComponent<IslandNavigator>();var flags=BindingFlags.Instance|BindingFlags.NonPublic;var field=typeof(QuestSlice).GetField("state",flags);var original=(SliceState)field.GetValue(slice);var records=site.Capture();var recipe=site.Recipe;bool paused=site.Paused;var position=actor.transform.position;god.Interrupt("Shelter QA");foreach(var a in actors){a.ResetTask();a.GetComponent<VillagerSocial>().ResetTask();a.GetComponent<VillagerBlockPlay>().ResetTask();}
  var test=new SliceState{developmentFocus=true};field.SetValue(slice,test);slice.GetComponentInChildren<VillageResources>().ResetWorld();var memory=actor.GetComponent<VillagerSocial>().Memory;
  ConstructionBlock Place(ConstructionShape shape,Vector3 at,bool rotate=false){var b=site.blocks.First(b=>b.Shape==shape&&!b.Attached);Require(site.Drop(b,site.World(at),site.transform.rotation*Quaternion.Euler(0,rotate?90:0,0)),"Fixture failed to snap "+shape+" at "+at);return b;}
  void Reset(){actor.ResetTask();site.ResetConstruction();site.Paused=false;site.Recipe=BuildRecipe.Tower;}
  try{
   Reset();Require(!site.TryShelterSpot(nav.profile.radius+.08f,nav.profile.height,out _,out _),"Loose yard pieces provided cover");
   Place(ConstructionShape.Column,new Vector3(-2,2.4f,-12));var right=Place(ConstructionShape.Column,new Vector3(2,2.4f,-12));var roof=Place(ConstructionShape.Slab,new Vector3(0,5.4f,-12));
   Require(!site.RecipeComplete&&site.TryShelterSpot(nav.profile.radius+.08f,nav.profile.height,out var rest,out var entry),"Unscripted column canopy was not usable");
   Require(!site.TryShelterSpot(nav.profile.radius+.08f,6,out _,out _),"Roof ignored headroom");
   var saved=site.Capture();roof.Grab();roof.Item.held=true;Require(!site.TryShelterSpot(nav.profile.radius+.08f,nav.profile.height,out _,out _),"Player-held roof provided cover");site.Restore(saved);
   right=site.blocks.First(b=>b.Shape==ConstructionShape.Column&&b.Attached&&b.Body.position.x>0);right.Grab();right.Item.held=true;Require(!site.TryShelterSpot(nav.profile.radius+.08f,nav.profile.height,out _,out _),"One-sided cantilever counted as braced cover");site.Restore(saved);
   var loose=site.blocks.First(b=>b.Shape==ConstructionShape.Cube&&!b.Attached);var old=loose.Body.position;loose.Body.position=new Vector3(0,1.2f,-12);Require(!site.TryShelterSpot(nav.profile.radius+.08f,nav.profile.height,out _,out _),"Blocked resting space was accepted");loose.Body.position=old;
   Require(site.TryShelterSpot(nav.profile.radius+.08f,nav.profile.height,out rest,out entry),"Cleared canopy did not recover");memory.restNeed=.95f;actors[1].GetComponent<VillagerSocial>().Memory.restNeed=.95f;actor.transform.position=entry;nav.ResetRoute();Require(actor.TryShelter(site),"Shelter visit could not start");Require(!actors[1].TryShelter(site),"Shelter visitor lease failed");
   actor.Tick(10,true);Require(memory.shelterRests==0&&site.Visitor==actor,"Paused visit changed state");test.Weather.rain=1;
   for(int i=0;i<3000&&memory.shelterRests==0;i++)actor.Tick(.05f,false);
   Require(memory.shelterRests==1&&memory.shelteredSeconds>=6&&memory.restNeed<.4f&&site.Visitor==null,"Native shelter visit did not complete with measured relief");
   Require(memory.Place(0)?.visits==1&&memory.Place(0).uncomfortable==0,"Covered rest failed to improve place memory");
   var save=JsonUtility.FromJson<SliceState>(JsonUtility.ToJson(test));Require(save.Valid()&&save.Social(0).shelterRests==1,"Visit memory invalid after load");
   actor.ResetTask();actor.transform.position=entry;nav.ResetRoute();memory.restNeed=.95f;Require(actor.TryShelter(site),"Removal visit did not start");roof=site.blocks.First(b=>b.Shape==ConstructionShape.Slab&&b.Attached);roof.Grab();roof.Item.held=true;actor.Tick(.3f,false);Require(site.Visitor==null&&memory.shelterRests==1,"Removed roof retained benefit/lease");
   Reset();foreach(int x in new[]{-2,2}){Place(ConstructionShape.Cube,new Vector3(x,1.2f,-12));Place(ConstructionShape.Cube,new Vector3(x,3.6f,-12));}Place(ConstructionShape.Slab,new Vector3(0,5.4f,-12),true);
   Require(site.TryShelterSpot(nav.profile.radius+.08f,nav.profile.height,out _,out _),"Second arrangement with stacked cubes/rotated roof was rejected");
   var restored=JsonUtility.FromJson<ConstructionRecordList>(JsonUtility.ToJson(new ConstructionRecordList{pieces=site.Capture()}));site.Restore(restored.pieces);Require(site.TryShelterSpot(nav.profile.radius+.08f,nav.profile.height,out _,out _),"Cover was lost on geometry restore");
   Debug.Log("BWVR_SHELTER_RUNTIME_PASS: two arrangements, no recipe flag, bracing/headroom/held/obstruction gates, restored geometry, native visit/reservation/pause, rain-sensitive relief, removed-roof cleanup, saved experience");
  }finally{actor.ResetTask();site.Recipe=recipe;site.Restore(records);site.Paused=paused;actor.transform.position=position;nav.ResetRoute();field.SetValue(slice,original);god.Interrupt("Shelter QA complete");}
 }
 [Serializable] sealed class ConstructionRecordList {public ConstructionRecord[] pieces;}
}
}
