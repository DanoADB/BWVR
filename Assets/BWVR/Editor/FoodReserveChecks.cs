using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace BWVR.Editor {
public static class FoodReserveChecks {
 static void Require(bool ok,string why){if(!ok)throw new Exception(why);}
 [MenuItem("BWVR/Verify food reserves (Play mode)")]
 public static void Verify(){
  Require(Application.isPlaying,"Enter Play mode");Require(VillageResources.ReserveTarget(3,48)==15&&VillageResources.ReserveTarget(6,48)==27&&VillageResources.ReserveTarget(6,20)==20,"Population/capacity reserve targets invalid");
  var world=UnityEngine.Object.FindAnyObjectByType<VillageResources>();var slice=world.GetComponentInParent<QuestSlice>();var field=typeof(QuestSlice).GetField("state",BindingFlags.Instance|BindingFlags.NonPublic);var original=(SliceState)field.GetValue(slice);var actors=world.Gatherers;var positions=actors.Select(a=>a.transform.position).ToArray();var test=new SliceState{developmentFocus=true};test.Resources.wheat=VillagePopulation.Adults*2;test.Resources.wood=18;
  slice.GetComponentInChildren<CreatureHybrid>().Interrupt("Food reserve QA");foreach(var a in actors){a.Cancel();a.GetComponent<VillagerSocial>().ResetTask();a.GetComponent<VillagerExploration>().ResetTask();a.GetComponent<VillagerBlockPlay>().ResetTask();}
  field.SetValue(slice,test);world.ResetWorld();
  try{
   Require(world.NeedsFoodReserve&&!world.FoodEmergency&&world.FoodSeconds==180,"Early shortage not recognized");
   Require(actors[0].TryFoodReserve(),"Former water gatherer could not help food reserve");Require(!actors[1].TryFoodReserve(),"Routine priority consumed a second idle adult");actors[0].Cancel();
   test.Resources.wheat=1;Require(actors[0].TryFoodReserve()&&actors[1].TryFoodReserve()&&!actors[2].TryFoodReserve(),"Emergency reserve slots invalid");Require(actors[0].Source!=actors[1].Source,"Workers reserved the same plot");foreach(var a in actors)a.Cancel();
   test.paused=true;Require(!actors[0].TryFoodReserve(),"Paused village started food work");test.paused=false;
   test.Resources.wheat=world.FoodReserveTarget;Require(!actors[0].TryFoodReserve(),"Healthy reserve still preempted leisure");
   world.Refresh();Require(world.transform.Find("Meal reserve crate")!=null&&world.VisibleReserveMeals==8,"Full meal reserve crate missing or not filled");
   test.Resources.wheat=1;world.Refresh();Require(world.VisibleReserveMeals==1,"Low food reserve crate did not visibly empty");
   // An existing trip is not duplicated to fill the last small deficit.
   test.Resources.wheat=world.FoodReserveTarget-1;Require(actors[0].TryFoodReserve()&&!actors[1].TryFoodReserve(),"Incoming food was ignored");actors[0].Cancel();
   // With no ripe grain, preserve actual seed and use the existing sowing motor.
   foreach(var p in world.sources.Where(s=>s.kind==VillageResource.Wheat))test.Resources.Find(p.id).remaining=0;
   test.Resources.wheat=2;Require(actors[0].TryFoodReserve()&&actors[0].Sowing&&world.ReservedSeed==1&&world.MealGrain==1,"Sowing fallback did not reserve seed");actors[0].Cancel();Require(world.ReservedSeed==0&&test.Resources.wheat==2,"Canceled reserve job lost seed");
   // One full native collection/delivery, not an inventory grant.
   var plot=world.sources.First(s=>s.kind==VillageResource.Wheat);test.Resources.Find(plot.id).remaining=12;test.Resources.wheat=3;actors[0].transform.position=plot.approach;actors[0].GetComponent<IslandNavigator>().ResetRoute();Require(actors[0].TryFoodReserve(),"Cannot begin native reserve trip");
   for(int i=0;i<4000&&actors[0].Active;i++)actors[0].Tick(.05f,false);
   Require(test.Resources.wheat==6&&test.Resources.Find(plot.id).remaining==9&&test.Resources.deliveries==1&&test.Valid(),"Native reserve harvest/delivery accounting failed");
   var saved=JsonUtility.FromJson<SliceState>(JsonUtility.ToJson(test));Require(saved.Valid()&&saved.Resources.wheat==6,"Food reserve save invalid");
   Debug.Log("BWVR_FOOD_RESERVES_PASS: population/capacity target, early action, limited priority slots, emergency help, exclusive plots, pause, seed fallback/cancel, visible marker, native delivery and saved stock");
  }finally{foreach(var a in actors)a.Cancel();field.SetValue(slice,original);world.ResetWorld();for(int i=0;i<actors.Length;i++){actors[i].transform.position=positions[i];actors[i].GetComponent<IslandNavigator>().ResetRoute();}}
 }
}
}
