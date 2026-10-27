using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace BWVR.Editor {
public static class GatheringPlaceChecks {
 static void Require(bool ok,string why){if(!ok)throw new Exception(why);}
 public static void VerifyModel(){
  var m=new SocialMemory{restNeed=.5f};Require(!m.WantsPlace(0,0,true),"Unknown place replaced ordinary rest need");
  Require(m.RememberPlace(0,true,0)&&!m.RememberPlace(0,true,1)&&m.Place(0).visits==1,"Visit credited twice");
  Require(!m.WantsPlace(0,89,true)&&m.WantsPlace(0,90,true)&&!m.WantsPlace(0,90,false),"Return ignores learning, cooldown or weather");
  Require(m.RememberPlace(0,false,90)&&!m.WantsPlace(0,180,true),"Uncomfortable experience did not reduce willingness");
  Require(!m.RememberPlace(3,true,180)&&!m.RememberPlace(1,true,float.NaN),"Invalid place/time accepted");
  var saved=JsonUtility.FromJson<SocialMemory>(JsonUtility.ToJson(m));Require(saved.Valid()&&saved.Place(0).visits==2&&saved.Place(0).uncomfortable==1,"Place save round trip failed");
  saved.gatheringPlaces=new[]{new GatheringPlaceMemory{slot=0},new GatheringPlaceMemory{slot=0}};Require(!saved.Valid(),"Duplicate place IDs accepted");
  saved.gatheringPlaces=new[]{new GatheringPlaceMemory{appeal=float.NaN}};Require(!saved.Valid(),"Invalid appeal accepted");
  Require(JsonUtility.FromJson<SocialMemory>("{}").Valid()&&new SliceState().Valid(),"Historical save rejected");
  Debug.Log("BWVR_GATHERING_PLACE_MODEL_PASS");
 }
 public static void VerifyRuntime(){
  Require(Application.isPlaying,"Enter Play mode");var slice=UnityEngine.Object.FindAnyObjectByType<QuestSlice>();var god=slice.GetComponentInChildren<CreatureHybrid>();var learning=god.GetComponent<CreatureConstructionLearning>();var site=learning.sites[0];var actors=slice.GetComponentsInChildren<VillagerExploration>();var actor=actors[0];var nav=actor.GetComponent<IslandNavigator>();var flags=BindingFlags.Instance|BindingFlags.NonPublic;var field=typeof(QuestSlice).GetField("state",flags);var original=field.GetValue(slice);var records=site.Capture();var recipe=site.Recipe;bool paused=site.Paused;var position=actor.transform.position;var rotation=actor.transform.rotation;var test=new SliceState{developmentFocus=true,observeWithoutFoodWork=true,water=100,crops=100};
  god.Interrupt("Place QA");foreach(var a in actors){a.ResetTask();a.GetComponent<VillagerSocial>().ResetTask();a.GetComponent<VillagerResourceGathering>().Cancel();a.GetComponent<VillagerBlockPlay>().ResetTask();}field.SetValue(slice,test);slice.GetComponentInChildren<VillageFamilies>().ResetWorld();actor.ConfigurePeers(Array.Empty<VillagerExploration>());
  void Stage(){actor.ResetTask();Require(nav.world.Sample(site.World(new Vector3(0,0,-6)),nav.profile,8,out var at),"No bench approach");actor.transform.position=nav.world.ProjectFoot(at,nav.profile);nav.ResetRoute();}
  void Step(){test.Hybrid.time+=.05f;actor.Tick(.05f,false);site.Advance(.02f);}
  try{
   site.Paused=false;site.ResetConstruction();site.Recipe=BuildRecipe.Bench;foreach(var p in ConstructionPatterns.Plan(BuildRecipe.Bench)){var b=site.blocks.First(b=>!b.Attached&&(b.size.x>3)==p.beam);Require(site.Drop(b,site.World(p.position),site.transform.rotation),"Bench failed to snap");for(int i=0;i<5;i++)site.Advance(.02f);}
   var m=test.Social(0);m.restNeed=.8f;test.Weather.rain=0;Stage();Require(actor.TryStructure(site),"Bench visit rejected");
   actor.Tick(10,true);Require(m.Place(0)==null,"Pause credited visit");
   for(int i=0;i<1500&&m.Place(0)==null;i++)Step();Require(m.Place(0)?.visits==1&&m.Place(0).appeal>=.6f&&m.restNeed<.5f,"Native comfortable visit missing");
   Stage();m.usedForms=15;m.restNeed=.5f;test.Hybrid.time=m.Place(0).nextVisitAt;var choose=typeof(VillagerExploration).GetMethod("ChooseStructure",flags);
   Require((bool)choose.Invoke(actor,null)&&site.Visitor==actor,"Remembered place did not attract autonomous return");actor.GetComponent<CarryItem>().held=true;actor.Tick(.05f,false);actor.GetComponent<CarryItem>().held=false;Require(site.Visitor==null&&m.Place(0).visits==1,"Pickup retained lease or credited visit");
   Stage();test.Weather.rain=1;Require(!(bool)choose.Invoke(actor,null),"Exposed bench attracted rainy return");
   Require(actor.TryStructure(site),"Rain experience staging rejected");for(int i=0;i<1500&&m.Place(0).visits==1;i++)Step();Require(m.Place(0).visits==2&&m.Place(0).uncomfortable==1&&m.Place(0).appeal<.5f,"Exposed completion has no negative place memory");
   var saved=JsonUtility.FromJson<SliceState>(JsonUtility.ToJson(test));Require(saved.Valid()&&saved.Social(0).Place(0).uncomfortable==1,"Runtime place save invalid");
   Debug.Log("BWVR_GATHERING_PLACE_RUNTIME_PASS: actual bench visit, pause, learned return, held interruption, rain avoidance, negative experience, persisted memory");
  }finally{actor.GetComponent<CarryItem>().held=false;actor.ResetTask();actor.ConfigurePeers(actors);site.Recipe=recipe;site.Restore(records);site.Paused=paused;actor.transform.SetPositionAndRotation(position,rotation);nav.ResetRoute();field.SetValue(slice,original);god.Interrupt("Place QA complete");}
 }
}
}
