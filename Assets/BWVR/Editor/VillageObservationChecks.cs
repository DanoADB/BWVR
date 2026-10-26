using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace BWVR.Editor {
public static class VillageObservationChecks {
 static void Require(bool ok,string why){if(!ok)throw new Exception(why);}
 [MenuItem("BWVR/Verify food observation and child pickup (Play mode)")]
 public static void Verify(){
  Require(EditorApplication.isPlaying,"Enter Play mode");
  var slice=UnityEngine.Object.FindAnyObjectByType<QuestSlice>();var world=slice.GetComponentInChildren<VillageResources>();var family=slice.GetComponentInChildren<VillageFamilies>();var god=slice.GetComponentInChildren<CreatureHybrid>();
  var stateField=typeof(QuestSlice).GetField("state",BindingFlags.NonPublic|BindingFlags.Instance);var original=(SliceState)stateField.GetValue(slice);bool focus=slice.developmentFocus,food=slice.observeWithoutFoodWork;
  var test=new SliceState{developmentFocus=true};god.Interrupt("Food observation QA");stateField.SetValue(slice,test);world.ResetWorld();family.ResetWorld();
  try{
   foreach(var actor in world.Gatherers){actor.GetComponent<VillagerSocial>().ResetTask();actor.GetComponent<VillagerExploration>().ResetTask();actor.GetComponent<VillagerBlockPlay>().ResetTask();}
   var farmer=world.Gatherers[1];var field=world.sources.First(s=>s.kind==VillageResource.Wheat&&world.Available(s));Require(farmer.TryBegin(field),"Cannot stage food gathering");test.observeWithoutFoodWork=true;Require(!farmer.Tick(.1f,false)&&!farmer.Active&&field.owner==null,"Observation retained food ownership");
   Require(!farmer.TryFoodReserve()&&!farmer.TrySow()&&!farmer.TryFoodProduction(),"Automatic food work still preempts observation");test.Resources.wheat=0;Require(!world.LowFood&&!world.FoodEmergency&&!world.NeedsFoodReserve&&world.GatheringNeed(VillageResource.Wheat)==0,"Food urgency still blocks other activity");
   test.Resources.wheat=20;float eaten=test.Resources.consumed;world.Advance(30,false);Require(test.Resources.consumed==eaten,"Observation consumed meals");test.observeWithoutFoodWork=false;world.Advance(1,false);Require(test.Resources.consumed>eaten,"Food economy did not resume");test.Resources.wheat=0;Require(world.LowFood&&world.FoodEmergency,"Normal food urgency lost");
   slice.developmentFocus=true;slice.observeWithoutFoodWork=true;slice.Simulate(.1f);Require(test.observeWithoutFoodWork,"Inspector observation toggle not propagated");slice.developmentFocus=false;slice.Simulate(.1f);Require(!test.observeWithoutFoodWork,"Food suppression escaped development focus");slice.developmentFocus=true;
   var homes=(System.Collections.Generic.Dictionary<int,Vector3>)typeof(VillageFamilies).GetField("homes",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(family);Require(homes.Count>0,"No child home");Require(test.Families.FormHousehold(0,1,homes.Keys.First()),"Cannot stage household");test.Families.bond=.9f;test.Families.clock=test.Families.nextBirth;Require(test.Families.TryBirth(20,17),"Cannot stage child");family.ResetWorld();
   var children=(System.Collections.Generic.List<GameObject>)typeof(VillageFamilies).GetField("children",BindingFlags.NonPublic|BindingFlags.Instance).GetValue(family);var child=children.Single();var item=child.GetComponent<CarryItem>();var collider=child.GetComponent<Collider>();Require(item!=null&&item.kind==Offering.Villager&&item.CanGrab&&collider!=null&&collider.enabled,"Child is not selectable");float ratio=child.transform.localScale.x/world.Gatherers[0].transform.localScale.x;Require(ratio>=.68f&&ratio<=.88f,"Child readability scale incorrect");
   Physics.SyncTransforms();Require(collider.Raycast(new Ray(collider.bounds.center+Vector3.up,Vector3.down),out _,2),"Child collider cannot be aimed at");
   Require((bool)typeof(QuestSlice).GetMethod("TryGrab",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(slice,new object[]{1,item}),"Player cannot grab child");var heldAt=child.transform.position+Vector3.up*.2f;child.transform.position=heldAt;family.Advance(2,false);Require(child.transform.position==heldAt&&item.held&&!collider.enabled,"Child walks out of player hand");
   typeof(QuestSlice).GetMethod("Release",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(slice,new object[]{1,new Ray(item.home+Vector3.up,Vector3.down)});Require(!item.held&&collider.enabled&&item.CanGrab,"Child cannot be released/reselected");Require(JsonUtility.FromJson<SliceState>(JsonUtility.ToJson(test)).Valid(),"Family save invalid");
   Debug.Log("BWVR_VILLAGE_OBSERVATION_PASS: food work releases ownership; food pressure/meal consumption paused only in development observation; normal economy restored; child readable, ray-selectable, held without autonomous movement, released/reselectable; save valid");
  }finally{foreach(var actor in world.Gatherers)actor.Cancel();slice.developmentFocus=focus;slice.observeWithoutFoodWork=food;stateField.SetValue(slice,original);world.ResetWorld();family.ResetWorld();god.NewSeason();}
 }
}
}
