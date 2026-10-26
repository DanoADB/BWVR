using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace BWVR.Editor {
public static class WeatherFoodChecks {
 static void Require(bool ok,string why){if(!ok)throw new Exception(why);}
 [MenuItem("BWVR/Verify rain and food protection (Play mode)")]
 public static void Verify(){
  var dry=new ResourceState{wheat=48,exposedWheat=12};var wet=JsonUtility.FromJson<ResourceState>(JsonUtility.ToJson(dry));dry.Consume(10,0);wet.Consume(10,0,0,1);Require(Mathf.Abs(dry.spoiled-.25f)<.0001f&&Mathf.Abs(wet.spoiled-1)<.0001f&&wet.wheat==48,"Rain loss or covered protection invalid");float before=wet.exposedWheat;wet.Consume(0,0,0,1);wet.Consume(float.NaN,0,0,1);wet.Consume(10,0,0,float.NaN);Require(wet.exposedWheat==before,"Invalid time/rain altered stocks");wet.Consume(10000,0,0,1);Require(wet.exposedWheat==0&&wet.spoiled==12&&wet.Valid(),"Rain spoilage exceeded actual grain");
  Require(Application.isPlaying,"Enter Play mode");var world=UnityEngine.Object.FindAnyObjectByType<VillageResources>();var slice=world.GetComponentInParent<QuestSlice>();var f=typeof(QuestSlice).GetField("state",BindingFlags.Instance|BindingFlags.NonPublic);var original=(SliceState)f.GetValue(slice);var actors=world.Gatherers;var positions=actors.Select(a=>a.transform.position).ToArray();var test=new SliceState{developmentFocus=true};test.Resources.wood=12;test.Resources.wheat=48;test.Resources.exposedWheat=6;
  slice.GetComponentInChildren<CreatureHybrid>().Interrupt("Food weather QA");foreach(var a in actors){a.Cancel();a.GetComponent<VillagerSocial>().ResetTask();a.GetComponent<VillagerExploration>().ResetTask();a.GetComponent<VillagerBlockPlay>().ResetTask();}f.SetValue(slice,test);world.ResetWorld();
  try{var actor=actors[0];actor.transform.position=world.StoreApproach;actor.GetComponent<IslandNavigator>().ResetRoute();test.Weather.kind=IslandWeatherKind.Thunderstorm;Require(!actor.TryWeatherPreparation(),"Read future weather instead of visible rain");test.Weather.rain=1;test.paused=true;Require(!actor.TryWeatherPreparation(),"Paused worker prepared");test.paused=false;actor.GetComponent<CarryItem>().held=true;Require(!actor.TryWeatherPreparation(),"Held worker prepared");actor.GetComponent<CarryItem>().held=false;test.Resources.wheat=1;Require(!actor.TryWeatherPreparation(),"Storage preempted food emergency");test.Resources.wheat=48;Require(actor.TryWeatherPreparation()&&!actors[1].TryWeatherPreparation(),"Rain protection missing or double-owned");actor.Tick(30,true);Require(test.Resources.storageWork==0,"Pause advanced protection");actor.Cancel();Require(test.Resources.wood==12&&test.Resources.storageUpgrades==0,"Cancel spent materials");Require(actor.TryWeatherPreparation(),"Cannot retry rain protection");for(int i=0;i<1000&&actor.Active;i++)actor.Tick(.05f,false);Require(test.Resources.storageUpgrades==1&&test.Resources.wood==6&&test.Resources.exposedWheat==0&&test.Resources.wheat==54,"Paid rain protection failed to shelter real grain");Require(JsonUtility.FromJson<SliceState>(JsonUtility.ToJson(test)).Valid(),"Rain protection save invalid");
   var sack=world.CreateLoad(VillageResource.Wheat);try{Require(sack.name=="Carried wheat sack"&&sack.GetComponentsInChildren<MeshRenderer>().Length>=2&&sack.GetComponentsInChildren<Collider>().Length==0,"Food sack missing or blocks actors");}finally{UnityEngine.Object.DestroyImmediate(sack);}
   Debug.Log("BWVR_WEATHER_FOOD_PASS: rain loss, covered protection, bounds, observable rain, pause/held/emergency/cancel, exclusive builder, native paid shelter, save, food sack");
  }finally{foreach(var a in actors){a.GetComponent<CarryItem>().held=false;a.Cancel();}f.SetValue(slice,original);world.ResetWorld();for(int i=0;i<actors.Length;i++){actors[i].transform.position=positions[i];actors[i].GetComponent<IslandNavigator>().ResetRoute();}}
 }
}
}
