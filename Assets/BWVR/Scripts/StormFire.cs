using System;
using System.Linq;
using UnityEngine;
namespace BWVR {
public enum StormFireOutcome {None,PlayerRain,VillagerWater,Weather,FuelSpent,TargetLost,Stopped,NoIgnition,Unresolved}
[Serializable] public sealed class StormFireState {
 public bool attempted,ignited;public string target="";public StormFireOutcome outcome;public int firefighter=-1,playerPulses,waterPulses;public float waterDrawn,damageBefore,damageAfter,resolvedAt;public Vector3 approach,waterPoint;
 public bool Resolved=>outcome!=StormFireOutcome.None;
 public bool Valid()=>Enum.IsDefined(typeof(StormFireOutcome),outcome)&&target!=null&&target.Length<=160&&(!attempted||target.Length>0)&&(!ignited||attempted)&&(!Resolved||attempted)&&firefighter>=-1&&firefighter<VillagePopulation.Adults&&playerPulses>=0&&playerPulses<=1000000&&waterPulses>=0&&waterPulses<=1000000&&HybridState.Number(waterDrawn,0,1000000)&&HybridState.Number(damageBefore,0,1)&&HybridState.Number(damageAfter,damageBefore,1)&&HybridState.Number(resolvedAt,0,100000000)&&Point(approach)&&Point(waterPoint)&&(!Resolved||outcome!=StormFireOutcome.VillagerWater||firefighter>=0&&waterPulses>0)&&(!Resolved||outcome!=StormFireOutcome.PlayerRain||playerPulses>0);
 static bool Point(Vector3 p)=>HybridState.Number(p.x,-10000,10000)&&HybridState.Number(p.y,-10000,10000)&&HybridState.Number(p.z,-10000,10000);
}
public sealed partial class SocialMemory {public int firesHelped;bool FiresValid()=>Count(firesHelped);}
public sealed partial class IslandWeather {
 public event Action<string,bool> RainBrushed;
 public bool DouseFire(string id,float seconds){if(season().paused||!HybridState.Number(seconds,0,.5f)||!byId.TryGetValue(id,out var target))return false;var m=State.Find(id);if(m==null||!m.burning||target.GetComponent<CarryItem>()?.held==true)return false;m.RainDose(seconds,WeatherMaterialProperties.For(target.kind));return true;}
}
public sealed partial class StormChapter {
 PlayImpactTarget fireTarget;VillagerExploration fireWorker;float fireSearchAt;
 public StormFireState Fire=>State.fire;
 public bool FirePending=>State.Active&&Fire?.ignited==true&&!Fire.Resolved&&fireTarget!=null&&fireTarget.gameObject.activeInHierarchy&&weather.State.Find(Fire.target)?.burning==true;
 public Vector3 FireWork=>nav.ToWorld(Fire.approach);public Vector3 FireWater=>nav.ToWorld(Fire.waterPoint);public Vector3 FirePoint=>fireTarget!=null?fireTarget.transform.position:FireWork;
 void BindFire(){CancelFireWorker();fireSearchAt=0;fireTarget=Fire?.attempted==true?weather.Targets.FirstOrDefault(t=>t!=null&&t.id==Fire.target):null;}
 bool TryStartFire(){
  if(State.phase!=StormPhase.Warning||State.elapsed<5||Fire?.attempted==true||season().paused)return false;
  var well=god.Find("cistern").transform.position;
  foreach(var target in weather.Targets.Where(t=>t!=null&&t.kind=="tree"&&t.gameObject.activeInHierarchy&&t.GetComponent<CarryItem>()?.held!=true&&t.GetComponent<ThrowableTree>()?.Harvested!=true&&season().WorldPlay.Damage(t.id)<.35f&&Vector3.Distance(t.transform.position,well)<nav.Scale*300).OrderBy(t=>(t.transform.position-well).sqrMagnitude).Take(512)){
   var bounds=target.Bounds(nav.Scale);bool isolated=!weather.Targets.Any(t=>t!=null&&t!=target&&(t.kind=="tree"||t.kind=="building")&&Vector3.Distance(t.Bounds(nav.Scale).ClosestPoint(bounds.center),bounds.center)<nav.Scale*16);if(!isolated)continue;
   var away=well-target.transform.position;away.y=0;if(away.sqrMagnitude<1e-10f)away=Vector3.forward;away.Normalize();
   if(!nav.Sample(target.transform.position+away*nav.Scale*16,nav.villager,4,out var approach)||!nav.Sample(well-away*nav.Scale*12,nav.villager,5,out var water)||!discoveries.Reachable(water,approach,nav.villager))continue;
   if(!weather.Strike(target.id))continue;fireTarget=target;float damage=season().WorldPlay.Damage(target.id);State.fire=new StormFireState{attempted=true,ignited=weather.State.Find(target.id)?.burning==true,target=target.id,approach=nav.ToNav(nav.ProjectFoot(approach,nav.villager)),waterPoint=nav.ToNav(nav.ProjectFoot(water,nav.villager)),damageBefore=damage,damageAfter=damage};
   if(!Fire.ignited)ResolveFire(StormFireOutcome.NoIgnition);god.Journal.Add(god.State.time,"storm lightning",CreatureAction.IdleOrObserve,target.id,Fire.ignited?"An isolated tree caught fire":"Lightning did not ignite the wet tree");return true;
  }return false;
 }
 void AdvanceFire(){
  if(State.phase==StormPhase.Warning&&State.elapsed>=5&&Fire?.attempted!=true&&god.State.time>=fireSearchAt){fireSearchAt=god.State.time+2;TryStartFire();}
  if(Fire?.attempted!=true||Fire.Resolved)return;Fire.damageAfter=Mathf.Max(Fire.damageBefore,season().WorldPlay.Damage(Fire.target));
  if(fireTarget==null||!fireTarget.gameObject.activeInHierarchy||fireTarget.GetComponent<CarryItem>()?.held==true||fireTarget.GetComponent<ThrowableTree>()?.Harvested==true){ResolveFire(StormFireOutcome.TargetLost);return;}
  var material=weather.State.Find(Fire.target);if(Fire.ignited&&material!=null&&!material.burning)ResolveFire(material.fuel<=0?StormFireOutcome.FuelSpent:StormFireOutcome.Weather);if(State.phase==StormPhase.Complete&&!Fire.Resolved){ResolveFire(StormFireOutcome.Unresolved);CancelFireWorker();}
 }
 void PlayerFireRain(string id,bool extinguished){if(season().paused||!State.Active||Fire?.ignited!=true||Fire.Resolved||Fire.target!=id)return;Fire.playerPulses=Math.Min(1000000,Fire.playerPulses+1);if(extinguished)ResolveFire(StormFireOutcome.PlayerRain);}
 void ResolveFire(StormFireOutcome outcome){
  if(Fire==null||Fire.Resolved)return;Fire.outcome=outcome;Fire.resolvedAt=god.State.time;Fire.damageAfter=Mathf.Max(Fire.damageBefore,season().WorldPlay.Damage(Fire.target));
  if(outcome==StormFireOutcome.VillagerWater&&Fire.firefighter>=0){var person=jobs[Fire.firefighter].GetComponent<VillagerSocial>();person.Memory.firesHelped=Math.Min(1000000,person.Memory.firesHelped+1);person.Memory.boldness=Mathf.Clamp01(person.Memory.boldness+.02f);person.GetComponent<VillagerAffect>()?.Reassured();}
  god.Journal.Add(god.State.time,"storm fire outcome",CreatureAction.IdleOrObserve,Fire.target,outcome+"; damage retained "+Fire.damageAfter.ToString("F2"));
 }
 public bool ClaimFireResponse(VillagerExploration actor){if(season().paused||!FirePending||fireWorker!=null||season().water<2||actor==null||actor.GetComponent<CarryItem>().held)return false;int index=Array.FindIndex(jobs,j=>j.GetComponent<VillagerExploration>()==actor);if(index<0||Pending&&Person==actor.GetComponent<VillagerSocial>())return false;fireWorker=actor;Fire.firefighter=index;return true;}
 public bool DrawFireWater(VillagerExploration actor){if(season().paused||!FirePending||fireWorker!=actor||season().water<2||Vector3.Distance(actor.transform.position,FireWater)>nav.Scale*4)return false;season().water-=2;Fire.waterDrawn=Mathf.Min(1000000,Fire.waterDrawn+2);return true;}
 public bool PourFireWater(VillagerExploration actor,float dose){
  if(!FirePending||fireWorker!=actor||season().paused||Vector3.Distance(actor.transform.position,FireWork)>nav.Scale*4||!weather.DouseFire(Fire.target,dose))return false;Fire.waterPulses=Math.Min(1000000,Fire.waterPulses+1);if(weather.State.Find(Fire.target)?.burning==false)ResolveFire(StormFireOutcome.VillagerWater);return true;
 }
 public void ReleaseFireResponse(VillagerExploration actor){if(fireWorker==actor)fireWorker=null;}
 void CancelFireWorker(){var old=fireWorker;fireWorker=null;if(old!=null&&old.FireResponding)old.ResetTask();}
 void StopFire(){if(Fire?.attempted==true&&!Fire.Resolved)ResolveFire(StormFireOutcome.Stopped);CancelFireWorker();}
 bool TickFireVillager(VillagerRoutine job,float dt){var actor=job.GetComponent<VillagerExploration>();if(fireWorker==actor&&actor.FireResponding){actor.Tick(dt,false);return true;}if(FirePending&&fireWorker==null&&actor.TryFireResponse(this)){actor.Tick(dt,false);return true;}return false;}
}
}
