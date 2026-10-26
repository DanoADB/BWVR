using UnityEngine;
using UnityEngine.Rendering;
namespace BWVR {
// Bounded player channels. Input ownership remains with GodToolPalette.
public sealed partial class WorldBrush:MonoBehaviour {
 public const float RainCost=2,PulseSeconds=.5f,StrokeSeconds=8;
 public bool PulseReady=>Season.Hybrid.time>=nextPulse;
 public int Pulses{get;private set;}public bool Active{get;private set;}public string Reason{get;private set;}="";
 public Vector3 Point{get;private set;}
 CreatureHybrid creature;DivineIntervention miracles;IslandNavigation nav;IslandWeather weather;VillageResources resources;ParticleSystem rain;Material material;
 SliceState knownSeason;
 void EnsureSeason(){if(knownSeason==Season)return;Stop();nextPulse=0;knownSeason=Season;}
 float nextPulse,heldFor;int strokePulses;bool restRequired;SliceState Season=>creature.Season;
 public static WorldBrush Build(Transform parent,CreatureHybrid god,DivineIntervention miracles,IslandNavigation nav){var go=new GameObject("Physical god brush tools");go.transform.SetParent(parent,false);var b=go.AddComponent<WorldBrush>();b.creature=god;b.miracles=miracles;b.nav=nav;b.weather=nav.GetComponentInChildren<IslandWeather>();b.resources=nav.GetComponentInChildren<VillageResources>();b.BuildRain();return b;}
 void BuildRain(){material=new Material(Resources.Load<Material>("ConstructionPreview"));var go=new GameObject("Player rain brush — 64 droplets");go.transform.SetParent(transform,false);rain=go.AddComponent<ParticleSystem>();rain.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);var main=rain.main;main.playOnAwake=false;main.loop=true;main.startSpeed=0;main.startLifetime=.7f;main.startSize=nav.Scale*.18f;main.startColor=new Color(.45f,.8f,1,.75f);main.maxParticles=64;main.simulationSpace=ParticleSystemSimulationSpace.World;var emission=rain.emission;emission.rateOverTime=70;var shape=rain.shape;shape.shapeType=ParticleSystemShapeType.Circle;shape.radius=nav.Scale*12;shape.rotation=new Vector3(90,0,0);var velocity=rain.velocityOverLifetime;velocity.enabled=true;velocity.space=ParticleSystemSimulationSpace.World;velocity.y=-nav.Scale*30;var renderer=rain.GetComponent<ParticleSystemRenderer>();renderer.sharedMaterial=material;renderer.renderMode=ParticleSystemRenderMode.Stretch;renderer.lengthScale=3;renderer.shadowCastingMode=ShadowCastingMode.Off;}
 public bool CanRain(Vector3 point,out string reason){EnsureSeason();reason="";
  if(Season.paused||miracles.Suspended){reason="Paused";return false;}
  if(!GroundPoint(point)){reason="Aim at dry island ground";return false;}
  if(Season.Hybrid.energy<RainCost){reason="Not enough spiritual energy";return false;}
  if(weather.RainBrush(point,PulseSeconds,false)+resources.RainBrush(point,PulseSeconds,false)==0){reason="Aim near dry timber, fire or growing crops";return false;}
  return true;
 }
 bool GroundPoint(Vector3 point){if(!HybridState.Number(point.x,-10000,10000)||!HybridState.Number(point.y,-10000,10000)||!HybridState.Number(point.z,-10000,10000))return false;return nav.Terrain!=null?nav.Terrain.Dry(point)&&Mathf.Abs(nav.Terrain.Project(point).y-point.y)<nav.Scale*2:Mathf.Abs(point.y-.82f)<nav.Scale*2;}
 public bool RainPulse(Vector3 point){EnsureSeason();if(Season.Hybrid.time<nextPulse){Reason="Brush is settling";return false;}if(!CanRain(point,out var reason)){Reason=reason;return false;}
  int affected=weather.RainBrush(point,PulseSeconds,true)+resources.RainBrush(point,PulseSeconds,true);if(affected==0){Reason="Nothing needs rain here";return false;}
  Season.Hybrid.energy-=RainCost;nextPulse=Season.Hybrid.time+PulseSeconds;Pulses++;Point=point;Reason="";creature.InvalidateLesson("Player rain brush is not creature work");
  Season.Hybrid.Evidence.Add(Season.Hybrid.time,EvidenceKind.Miracle,EvidenceSource.PlayerMiracle,CreatureAction.IdleOrObserve,"ground-rain-brush","Local rain dose; affected "+affected+"; cost "+RainCost);IslandAudio.Cue(IslandSound.Pour,point,GetEntityId().GetHashCode(),.5f);return true;
 }
 public bool ProcessRain(Vector3 point,bool held,float dt,bool aimValid=true){EnsureSeason();if(!held||Season.paused||miracles.Suspended||!HybridState.Number(dt,0,1)){Stop();return false;}heldFor+=Mathf.Min(dt,.05f);if(Season.Hybrid.energy<RainCost){restRequired=true;StopRain();Reason="Release the trigger: energy exhausted";return false;}if(restRequired||heldFor>StrokeSeconds||strokePulses>=16){StopRain();Reason="Release the trigger to rest the brush";return false;}
  if(!aimValid){StopRain();Reason="Aim at a supported patch of island ground";return false;}if(!CanRain(point,out var reason)){StopRain();Reason=reason;return false;}bool applied=false;if(Season.Hybrid.time>=nextPulse){applied=RainPulse(point);if(applied)strokePulses++;}
  Active=true;Point=point;rain.transform.position=point+Vector3.up*nav.Scale*20;if(!rain.isPlaying)rain.Play();return applied;
 }
 void StopRain(){Active=false;if(rain!=null)rain.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);}
 public void Stop(){StopRain();heldFor=0;strokePulses=0;restRequired=false;}
 void OnDisable(){Stop();}void OnDestroy(){if(material!=null)Destroy(material);}
}
}
