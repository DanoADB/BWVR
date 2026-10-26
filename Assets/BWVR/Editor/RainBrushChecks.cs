using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace BWVR.Editor {
public static class RainBrushChecks {
 static void Require(bool ok,string why){if(!ok)throw new Exception(why);}
 [MenuItem("BWVR/Verify physical rain brush (Play mode)")]
 public static void Verify(){
  Require(EditorApplication.isPlaying,"Enter Play mode");var palette=UnityEngine.Object.FindAnyObjectByType<GodToolPalette>();var slice=palette.GetComponentInParent<QuestSlice>();var world=slice.GetComponentInChildren<VillageResources>();var weather=slice.GetComponentInChildren<IslandWeather>();var brush=palette.Brush;var flags=BindingFlags.Instance|BindingFlags.NonPublic;var field=typeof(QuestSlice).GetField("state",flags);var original=(SliceState)field.GetValue(slice);var test=new SliceState{developmentFocus=true};field.SetValue(slice,test);world.ResetWorld();weather.ResetWorld();
  var energyTuning=UnityEngine.Object.FindAnyObjectByType<DivineIntervention>().tuning;bool unlimited=energyTuning.unlimitedEnergy;energyTuning.unlimitedEnergy=false;
  try{
   test.Hybrid.energy=100;var target=weather.Targets.First(t=>t.kind=="tree"&&t.gameObject.activeInHierarchy&&t.GetComponent<CarryItem>()?.held!=true&&t.GetComponent<ThrowableTree>()?.Harvested!=true);var nav=slice.GetComponent<IslandNavigation>();var point=nav.Terrain.Project(target.transform.position);var m=test.Weather.Touch(target.id);m.wetness=0;m.heat=.4f;m.burning=true;
   string before=JsonUtility.ToJson(test);Require(brush.CanRain(point,out _)&&JsonUtility.ToJson(test)==before,"Preview mutated simulation");float energy=test.Hybrid.energy,water=test.water,crops=test.crops;Require(brush.RainPulse(point)&&test.Hybrid.energy==energy-WorldBrush.RainCost&&m.wetness>0&&m.heat<.4f&&!m.burning,"Paid rain did not wet/cool/extinguish");Require(test.water==water&&test.crops==crops&&test.Hybrid.Evidence.playerMiracles==1&&test.Hybrid.Evidence.completedCreatureActions==0,"Brush fabricated village/creature credit");
   energy=test.Hybrid.energy;Require(!brush.RainPulse(point)&&test.Hybrid.energy==energy,"Held-time duplicate charged");test.Hybrid.time+=1;test.paused=true;Require(!brush.RainPulse(point)&&test.Hybrid.energy==energy,"Paused brush spent");test.paused=false;Require(!brush.CanRain(new Vector3(float.NaN,0,0),out _)&&!brush.CanRain(point+Vector3.up,out _),"Invalid/air target accepted");
   var fieldNode=world.sources.First(s=>s.kind==VillageResource.Wheat);var record=test.Resources.Find(fieldNode.id);record.remaining=0;record.growing=true;record.plantings=1;record.growth=.2f;float growth=record.growth;Require(brush.RainPulse(fieldNode.at)&&record.growth>growth&&test.Resources.wheat==VillagePopulation.Adults*2,"Local rain created food instead of tending growth");
   test.Weather.rain=1;energy=test.Hybrid.energy;Require(!brush.CanRain(fieldNode.at,out _)&&!brush.RainPulse(fieldNode.at)&&test.Hybrid.energy==energy,"Natural full rain stacked/charged");test.Weather.rain=0;
   test.Hybrid.time+=1;test.Hybrid.energy=1;Require(!brush.ProcessRain(fieldNode.at,true,.05f)&&!brush.Active,"Empty energy kept stream active");test.Hybrid.energy=100;Require(!brush.ProcessRain(fieldNode.at,true,.05f),"Held trigger auto-restarted after exhaustion");brush.ProcessRain(fieldNode.at,false,.05f);int pulses=brush.Pulses;energy=test.Hybrid.energy;
   for(int i=0;i<180;i++){test.Hybrid.time+=.05f;brush.ProcessRain(fieldNode.at,true,.05f);}Require(brush.Pulses-pulses<=16&&energy-test.Hybrid.energy<=32.001f&&!brush.Active,"Long stroke exceeded budget or continued");brush.Stop();
   test.Hybrid.time+=1;var ray=new Ray(fieldNode.at+Vector3.up*.2f,Vector3.down);palette.Select(SpiritualTool.RainBrush);palette.Process(ray,false,true,false,.05f,false);pulses=brush.Pulses;palette.Process(ray,true,true,false,.05f,false);Require(brush.Pulses==pulses+1&&brush.Active,"Palette brush input did not paint");energy=test.Hybrid.energy;palette.Process(ray,true,false,false,.05f,true);Require(!brush.Active&&!palette.Selected.HasValue&&test.Hybrid.energy==energy,"Tracking loss retained brush/spend");
   var copy=JsonUtility.FromJson<SliceState>(JsonUtility.ToJson(test));Require(copy.Valid()&&copy.Weather.Find(target.id).wetness==m.wetness&&copy.Resources.Find(fieldNode.id).growth==record.growth,"Rain consequences did not persist");var dose=new WeatherMaterial{wetness=.2f,heat=1};dose.RainDose(float.NaN,WeatherMaterialProperties.For("tree"));Require(dose.wetness==.2f&&dose.heat==1,"Nonfinite dose corrupted material");
   energyTuning.unlimitedEnergy=true;test.Hybrid.energy=0;test.Hybrid.time+=1;m.wetness=0;m.heat=.4f;m.burning=true;Require(brush.RainPulse(point)&&test.Hybrid.energy==0&&!m.burning,"Unlimited rain brush failed at zero balance");test.Hybrid.time+=1;test.paused=true;Require(!brush.RainPulse(point),"Unlimited rain bypassed pause");test.paused=false;
   Debug.Log("BWVR_RAIN_BRUSH_PASS: read-only preview, paid local wet/cool/extinguish, no global resource/creature credit, cooldown/pause/finite ground gates, local planted growth only, no stacking full natural rain, energy exhaustion requires release, bounded stroke, palette channel/tracking stop, valid saved consequences");
  }finally{energyTuning.unlimitedEnergy=unlimited;palette.Cancel();brush.Stop();field.SetValue(slice,original);world.ResetWorld();weather.ResetWorld();}
 }
}
}
