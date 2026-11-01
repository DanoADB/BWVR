using UnityEngine;
using UnityEngine.Rendering;
namespace BWVR {
public sealed partial class WorldBrush {
 public const float RegrowCost=3,RegrowSeconds=20;
 ParticleSystem growthMotes;SliceState regrowthSeason;bool growthNeedsRelease;
 void BuildRegrowth(){
  var go=new GameObject("Tree growth brush — 24 green motes");go.transform.SetParent(transform,false);growthMotes=go.AddComponent<ParticleSystem>();growthMotes.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
  var main=growthMotes.main;main.playOnAwake=false;main.loop=true;main.startLifetime=.8f;main.startSpeed=nav.Scale*.5f;main.startSize=nav.Scale*.3f;main.startColor=new Color(.55f,1,.45f,.7f);main.maxParticles=24;main.simulationSpace=ParticleSystemSimulationSpace.World;
  var emission=growthMotes.emission;emission.rateOverTime=22;var shape=growthMotes.shape;shape.shapeType=ParticleSystemShapeType.Circle;shape.radius=nav.Scale*2;shape.rotation=new Vector3(90,0,0);var velocity=growthMotes.velocityOverLifetime;velocity.enabled=true;velocity.space=ParticleSystemSimulationSpace.World;velocity.y=nav.Scale*4;var renderer=growthMotes.GetComponent<ParticleSystemRenderer>();renderer.sharedMaterial=material;renderer.shadowCastingMode=ShadowCastingMode.Off;
 }
 public bool CanRegrow(Vector3 point,out string reason){
  EnsureSeason();reason="";if(Season.paused||miracles.Suspended){reason="Paused";return false;}if(!GroundPoint(point)){reason="Aim at dry island ground";return false;}if(!miracles.CanAfford(RegrowCost)){reason="Not enough spiritual energy";return false;}return resources.CanPlayerRegrow(point,out reason);
 }
 public bool RegrowPulse(Vector3 point){
  EnsureSeason();if(!PulseReady){Reason="Brush is settling";return false;}if(!CanRegrow(point,out var why)){Reason=why;return false;}
  var node=resources.PlayerTreeAt(point);bool planting=!resources.State.Find(node.id).growing;if(!resources.TryPlayerRegrow(point,out why)){Reason=why;return false;}
  miracles.SpendEnergy(RegrowCost);nextPulse=Season.Hybrid.time+PulseSeconds;Pulses++;Point=node.at;Reason="";creature.InvalidateLesson("Player forest regrowth is not creature work");
  Season.Hybrid.Evidence.Add(Season.Hybrid.time,EvidenceKind.Miracle,EvidenceSource.PlayerMiracle,CreatureAction.IdleOrObserve,node.id,(planting?"Sapling planted; ":"Tree nurtured; ")+RegrowSeconds+" seconds of natural growth; cost "+miracles.EnergyCost(RegrowCost));IslandAudio.Cue(IslandSound.Harvest,node.at,GetEntityId().GetHashCode(),.35f);return true;
 }
 public bool ProcessRegrow(Vector3 point,bool held,float dt,bool aimValid=true){
  EnsureSeason();if(regrowthSeason!=Season){regrowthSeason=Season;growthNeedsRelease=held;}if(!held)growthNeedsRelease=false;StopRain();StopWind();StopWhirlwind();CancelLightning();if(growthNeedsRelease){StopRegrowth();Reason="Release the trigger after restoring a season";return false;}if(!held||Season.paused||miracles.Suspended||!HybridState.Number(dt,0,1)){Stop();return false;}
  heldFor+=Mathf.Min(dt,.05f);if(!miracles.CanAfford(RegrowCost)){restRequired=true;StopRegrowth();Reason="Release the trigger: energy exhausted";return false;}
  if(restRequired||heldFor>StrokeSeconds||strokePulses>=16){StopRegrowth();Reason="Release the trigger to rest the brush";return false;}
  string why="Aim at a supported tree site";if(!aimValid||!CanRegrow(point,out why)){StopRegrowth();Reason=aimValid?why:"Aim at a supported tree site";return false;}
  bool applied=PulseReady&&RegrowPulse(point);if(applied)strokePulses++;Active=true;Point=resources.PlayerTreeAt(point).at;growthMotes.transform.position=Point+Vector3.up*nav.Scale*.5f;if(!growthMotes.isPlaying)growthMotes.Play();return applied;
 }
 void StopRegrowth(){Active=false;if(growthMotes!=null)growthMotes.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);}
}
}
