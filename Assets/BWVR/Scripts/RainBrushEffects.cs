using UnityEngine;
namespace BWVR {
// A direct rain dose uses the same precipitation coefficients as ambient weather.
public sealed partial class WeatherMaterial {
 public const float RainWetting=.22f,RainCooling=.6f;
 public void RainDose(float seconds,WeatherMaterialProperties properties){
  if(!HybridState.Number(seconds,0,.5f))return;float amount=seconds;wetness=Mathf.Clamp01(wetness+amount*RainWetting);heat=Mathf.Max(0,heat-amount*RainCooling/properties.heatCapacity);if(heat<.25f||fuel<=0)burning=false;
 }
}
public sealed partial class IslandWeather {
 readonly PlayImpactTarget[] brushTargets=new PlayImpactTarget[8];
 // Registered materials only; at most eight affected objects per paid pulse.
 public int RainBrush(Vector3 point,float seconds,bool apply){
  if(!HybridState.Number(seconds,0,.5f)||!HybridState.Number(point.x,-10000,10000)||!HybridState.Number(point.y,-10000,10000)||!HybridState.Number(point.z,-10000,10000))return 0;int count=0,newMaterials=0;float radius=nav.Scale*12;float dose=Mathf.Clamp(seconds,0,.5f)*(1-State.rain);if(dose<=.0001f)return 0;
  for(int pass=0;pass<2;pass++)foreach(var target in targets){if(count>=8)break;if(!Combustible(target)||target.GetComponent<CarryItem>()?.held==true||!target.gameObject.activeInHierarchy)continue;
   var bounds=target.Bounds(nav.Scale);var close=bounds.ClosestPoint(new Vector3(point.x,bounds.center.y,point.z));if(new Vector2(close.x-point.x,close.z-point.z).sqrMagnitude>radius*radius)continue;
   var material=State.Find(target.id);bool hot=material!=null&&(material.burning||material.heat>.25f);if(hot!=(pass==0))continue;if(material==null&&State.materials.Length+newMaterials>=WeatherState.MaterialLimit)continue;
   if((material?.wetness??State.wetness)>=.999f&&(material?.heat??0)<=.001f)continue;
   brushTargets[count++]=target;if(material==null)newMaterials++;
  }if(apply)for(int i=0;i<count;i++){var target=brushTargets[i];var material=State.Find(target.id)??State.Touch(target.id);material?.RainDose(dose,WeatherMaterialProperties.For(target.kind));}return count;
 }
}
public sealed partial class VillageResources {
 public int RainBrush(Vector3 point,float seconds,bool apply){if(!HybridState.Number(seconds,0,.5f)||!HybridState.Number(point.x,-10000,10000)||!HybridState.Number(point.y,-10000,10000)||!HybridState.Number(point.z,-10000,10000))return 0;int count=0;float radius=Navigation.Scale*12;float bonus=Mathf.Clamp(seconds,0,.5f)*.25f*(1-Season.Weather.rain);if(bonus<=.0001f)return 0;
  foreach(var node in sources){var record=State.Find(node.id);if(node.kind!=VillageResource.Wheat||record==null||!record.growing||!Safe(node)||(node.at-point).sqrMagnitude>radius*radius)continue;if(apply)State.Grow(node.id,bonus);count++;}return count;
 }
}
}
