using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace BWVR.Editor {
public static class WeatherChecks {
 static void Require(bool condition,string reason){if(!condition)throw new Exception(reason);}
 [MenuItem("BWVR/Verify weather material model")]
 public static void VerifyModel(){
  var dry=new WeatherMaterial{id="dry",heat=1.5f,burning=true};for(int i=0;i<10;i++)dry.Step(0,1,1);Require(dry.burning&&dry.fuel<1,"Dry fuel did not burn");
  var wet=new WeatherMaterial{id="wet",heat=1.5f,burning=true};for(int i=0;i<10;i++)wet.Step(1,1,1);Require(!wet.burning&&wet.wetness>.9f&&wet.fuel>dry.fuel,"Rain did not suppress combustion");
  Require(WeatherMaterial.HeatReceived(4,1,1,0,1)>WeatherMaterial.HeatReceived(4,0,1,0,1)&&WeatherMaterial.HeatReceived(4,1,1,1,1)==0,"Wind direction/wet fuel had no thermal effect");
  var foliage=new WeatherMaterial{id="leaf",heat=1.1f};var timber=new WeatherMaterial{id="wood",heat=1.1f};foliage.Step(0,0,1,WeatherMaterialProperties.For("tree"));timber.Step(0,0,1,WeatherMaterialProperties.For("building"));Require(foliage.burning&&!timber.burning,"Material ignition properties had no effect");
  var a=new WeatherState();var b=new WeatherState();for(int i=0;i<900;i++){a.Advance(1,true);b.Advance(1,true);}Require(JsonUtility.ToJson(a)==JsonUtility.ToJson(b),"Seeded weather sequence diverged");
  var legacy=new SliceState();Require(legacy.Valid(),"Legacy save rejected");legacy.Weather.Touch("tree").burning=true;var saved=JsonUtility.FromJson<SliceState>(JsonUtility.ToJson(legacy));Require(saved.Valid()&&saved.Weather.Find("tree").burning,"Weather save roundtrip failed");saved.Weather.materials[0].heat=float.NaN;Require(!saved.Valid(),"Invalid heat accepted");
  var bounded=new WeatherState();for(int i=0;i<250;i++)bounded.Touch("tree-"+i);Require(bounded.materials.Length==WeatherState.MaterialLimit,"Material budget exceeded");for(int i=0;i<=WeatherState.FireLimit;i++)bounded.materials[i].burning=true;Require(!bounded.Valid(),"Over-budget fire save accepted");
  Debug.Log("BWVR_WEATHER_MODEL_PASS: dry combustion, rain cooling, fuel consumption, seeded climate, legacy/save validation, material/fire budgets");
 }
 [MenuItem("BWVR/Verify weather interactions (Play mode)")]
 public static void VerifyRuntime(){
  var world=UnityEngine.Object.FindAnyObjectByType<IslandWeather>();Require(world!=null,"Enter Play mode first");var slice=world.GetComponentInParent<QuestSlice>();var field=typeof(QuestSlice).GetField("state",BindingFlags.NonPublic|BindingFlags.Instance);var original=(SliceState)field.GetValue(slice);bool auto=world.automatic;var test=new SliceState{developmentFocus=true};field.SetValue(slice,test);world.automatic=false;
  try{
   world.ResetWorld();var target=world.Targets.First(t=>t.kind=="tree"&&t.GetComponent<CarryItem>()?.held!=true);var residents=world.Targets.Where(t=>t.villager!=null).ToArray();var trusts=residents.Select(t=>t.villager.Memory.trust).ToArray();
   Require(world.Strike(target.id)&&world.State.Find(target.id).burning,"Lightning did not ignite dry tree");for(int i=0;i<8;i++)world.Advance(1,false);Require(test.WorldPlay.Damage(target.id)>0,"Fire caused no structural damage");
   var house=world.Targets.First(t=>t.kind=="building");Require(world.Strike(house.id)&&world.State.Find(house.id).burning,"Dry timber building did not ignite");
   float clock=world.State.time,heat=world.State.Find(target.id).heat;world.Advance(5,true);Require(world.State.time==clock&&world.State.Find(target.id).heat==heat,"Paused weather progressed");
   world.SetWeather(IslandWeatherKind.Rain);for(int i=0;i<25;i++)world.Advance(1,false);Require(!world.State.Find(target.id).burning,"Rain did not extinguish tree");Require(test.water>52,"Natural rain did not replenish water");
   for(int i=0;i<residents.Length;i++)Require(residents[i].villager.Memory.trust==trusts[i],"Weather blamed on creature");
   test.weather=new WeatherState();var trees=world.Targets.Where(t=>t.kind=="tree").ToArray();PlayImpactTarget source=null,neighbor=null;foreach(var t in trees){var from=t.Bounds(world.GetComponentInParent<QuestSlice>().GetComponent<IslandNavigation>().Scale).center;neighbor=trees.FirstOrDefault(n=>n!=t&&Vector3.Distance(from,n.Bounds(world.GetComponentInParent<QuestSlice>().GetComponent<IslandNavigation>().Scale).ClosestPoint(from))<world.GetComponentInParent<QuestSlice>().GetComponent<IslandNavigation>().Scale*6);if(neighbor!=null){source=t;break;}}Require(source!=null,"No nearby fuel fixture");world.Strike(source.id);for(int i=0;i<45;i++)world.Advance(1,false);Require(world.State.Find(neighbor.id)!=null&&world.State.Find(neighbor.id).fuel<1,"Heat did not spread to neighboring fuel");
   world.State.materials=Array.Empty<WeatherMaterial>();world.State.rain=world.State.wetness=0;world.SetWeather(IslandWeatherKind.Clear);foreach(var t in world.Targets.Where(t=>t.kind=="tree").Take(40))world.Strike(t.id);Require(world.BurningCount<=WeatherState.FireLimit,"Lightning exceeded active fire budget");
   for(int i=0;i<15;i++)world.Advance(1,false);Require(world.State.Valid()&&world.BurningCount<=WeatherState.FireLimit,"Fire spread exceeded budget");
   world.SetWeather(IslandWeatherKind.TidalWave,24);var nav=slice.GetComponent<IslandNavigation>();float sea=nav.Terrain.SeaLevel;var coastal=world.Targets.Where(t=>t.kind=="tree"&&t.Bounds(nav.Scale).min.y<sea+nav.Scale*7&&t.Bounds(nav.Scale).max.y>sea).FirstOrDefault();Require(coastal!=null,"No coastal fixture");
   var island=nav.Terrain.mesh.bounds;world.State.phaseTime=24*Mathf.InverseLerp(island.min.x-nav.Scale*40,island.max.x+nav.Scale*40,coastal.Bounds(nav.Scale).center.x);Require(world.WaveHeight(coastal.Bounds(nav.Scale).center)>nav.Scale*7,"Wave crest and coastal target misaligned");float damage=test.WorldPlay.Damage(coastal.id);world.Advance(.01f,false);typeof(IslandWeather).GetMethod("MaterialsStep",BindingFlags.NonPublic|BindingFlags.Instance).Invoke(world,null);Require(test.WorldPlay.Damage(coastal.id)>damage&&world.State.Find(coastal.id).wetness==1,"Coastal surge had no physical consequence");damage=test.WorldPlay.Damage(coastal.id);typeof(IslandWeather).GetMethod("MaterialsStep",BindingFlags.NonPublic|BindingFlags.Instance).Invoke(world,null);Require(test.WorldPlay.Damage(coastal.id)==damage,"Wave damaged target twice");
   var roundtrip=JsonUtility.FromJson<SliceState>(JsonUtility.ToJson(test));Require(roundtrip.Valid(),"Runtime weather save invalid");
   Debug.Log("BWVR_WEATHER_RUNTIME_PASS: lightning ignition; structural fire damage; pause; rain extinguishing/resources; no creature blame; bounded spread; visible/logical coastal crest; one hit per wave; saved material state");
  }finally{field.SetValue(slice,original);world.automatic=auto;foreach(var target in world.Targets)target.Apply(original.WorldPlay.Damage(target.id));world.ResetWorld();}
 }
}
}
