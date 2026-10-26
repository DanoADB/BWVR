using System;
using UnityEngine;
namespace BWVR {
public enum IslandWeatherKind { Clear, Wind, Fog, Rain, Thunderstorm, TidalWave }
public readonly struct WeatherMaterialProperties {
 public readonly float heatCapacity,burnRate,ignition;
 public WeatherMaterialProperties(float capacity,float fuelRate,float ignitionHeat){heatCapacity=capacity;burnRate=fuelRate;ignition=ignitionHeat;}
 public static WeatherMaterialProperties For(string kind)=>kind=="building"?new WeatherMaterialProperties(1.6f,.01f,1.2f):new WeatherMaterialProperties(.8f,.018f,1);
}
// Sparse changed materials, not a Rigidbody or a saved record for every tree.
[Serializable] public sealed partial class WeatherMaterial {
 public string id;public float wetness,heat,fuel=1;public int wave=-1;public bool burning;
 public static float HeatReceived(float distance,float downwind,float wind,float wetness,float capacity)=>(.12f+wind*Mathf.Clamp01(downwind)*.2f)*Mathf.Clamp01(1-distance/12)*(1-wetness)/capacity;
 public bool Valid()=>!string.IsNullOrEmpty(id)&&id.Length<=160&&HybridState.Number(wetness,0,1)&&HybridState.Number(heat,0,2)&&HybridState.Number(fuel,0,1)&&wave>=-1;
 public void Step(float rain,float wind,float dt)=>Step(rain,wind,dt,WeatherMaterialProperties.For("tree"));
 public void Step(float rain,float wind,float dt,WeatherMaterialProperties properties){
  wetness=Mathf.Clamp01(wetness+dt*(rain*RainWetting-(1-rain)*(.007f+wind*.012f)));
  if(burning){heat=Mathf.Clamp(heat+dt*(.12f*(1-wetness)-rain*RainCooling-wetness*.16f)/properties.heatCapacity,0,2);fuel=Mathf.Max(0,fuel-dt*properties.burnRate*(1-wetness));if(heat<.25f||fuel<=0)burning=false;}
  else {heat=Mathf.Max(0,heat-dt*(.035f+wetness*.2f)/properties.heatCapacity);if(heat>=properties.ignition&&wetness<.65f&&fuel>0)burning=true;}
 }
}
[Serializable] public sealed class WeatherState {
 public const int MaterialLimit=192,FireLimit=16;
 public IslandWeatherKind kind;public float time,remaining=90,phaseTime,stepRemainder,rain,wind=.12f,fog=.1f,wetness,lightningIn=10;
 public int seed=71239,wave;public WeatherMaterial[] materials=Array.Empty<WeatherMaterial>();
 public float Random(){seed=(int)(((long)seed*48271)%2147483647);return seed/2147483647f;}
 public WeatherMaterial Find(string id){foreach(var m in materials)if(m.id==id)return m;return null;}
 public WeatherMaterial Touch(string id){var found=Find(id);if(found!=null)return found;if(materials.Length>=MaterialLimit)return null;var added=new WeatherMaterial{id=id,wetness=wetness};Array.Resize(ref materials,materials.Length+1);materials[materials.Length-1]=added;return added;}
 public void Set(IslandWeatherKind next,float duration=60){kind=next;remaining=Mathf.Clamp(duration,1,600);phaseTime=0;lightningIn=8;if(next==IslandWeatherKind.TidalWave)wave=wave==int.MaxValue?0:wave+1;}
 public void Advance(float dt,bool automatic){time=Mathf.Min(100000000,time+dt);phaseTime=Mathf.Min(600,phaseTime+dt);remaining=Mathf.Max(0,remaining-dt);
  if(automatic&&remaining<=0){var next=kind==IslandWeatherKind.Thunderstorm?IslandWeatherKind.TidalWave:(IslandWeatherKind)Mathf.FloorToInt(Random()*5);Set(next,next==IslandWeatherKind.TidalWave?24:60+Random()*60);}
  float r=kind==IslandWeatherKind.Rain?.75f:kind==IslandWeatherKind.Thunderstorm?1:0;
  float w=kind==IslandWeatherKind.Wind?.7f:kind==IslandWeatherKind.Thunderstorm?1:kind==IslandWeatherKind.TidalWave?.8f:.12f;
  float f=kind==IslandWeatherKind.Fog?1:kind==IslandWeatherKind.Rain?.35f:kind==IslandWeatherKind.Thunderstorm?.5f:.1f;
  rain=Mathf.MoveTowards(rain,r,dt*.08f);wind=Mathf.MoveTowards(wind,w,dt*.06f);fog=Mathf.MoveTowards(fog,f,dt*.05f);wetness=Mathf.Clamp01(wetness+dt*(rain*.12f-(1-rain)*(.005f+wind*.008f)));
 }
 public bool Valid(){if(!Enum.IsDefined(typeof(IslandWeatherKind),kind)||!HybridState.Number(time,0,100000000)||!HybridState.Number(remaining,0,600)||!HybridState.Number(phaseTime,0,600)||!HybridState.Number(stepRemainder,0,1)||!HybridState.Number(lightningIn,-1,100)||seed<=0||seed>=2147483647||wave<0||!HybridState.Number(rain,0,1)||!HybridState.Number(wind,0,1)||!HybridState.Number(fog,0,1)||!HybridState.Number(wetness,0,1)||materials==null||materials.Length>MaterialLimit)return false;var ids=new System.Collections.Generic.HashSet<string>();int fires=0;foreach(var m in materials){if(m==null||!m.Valid()||!ids.Add(m.id))return false;if(m.burning)fires++;}return fires<=FireLimit;}
}
}
