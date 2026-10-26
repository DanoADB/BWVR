using System;
using UnityEngine;
namespace BWVR {
[Serializable] public sealed class ReligionState {
 public float pressure,cooldownUntil;public bool fanatic;public int episodes,recoveries;
 public bool Valid()=>HybridState.Number(pressure,0,60)&&HybridState.Number(cooldownUntil,0,100000120)&&episodes>=0&&episodes<=1000000&&recoveries>=0&&recoveries<=episodes;
 // Sustained consensus, not one believer or one frame, creates an obsession.
 public void Advance(float dt,float now,float ferventFraction,bool diversion,bool hardship){if(!HybridState.Number(dt,0,1)||!HybridState.Number(now,0,100000000)||!HybridState.Number(ferventFraction,0,1)||dt<=0)return;
  float rate=ferventFraction>=.75f&&now>=cooldownUntil?1:-2;if(diversion)rate-=3;if(hardship&&fanatic)rate-=2;
  pressure=Mathf.Clamp(pressure+dt*rate,0,60);
  if(!fanatic&&pressure>=30&&now>=cooldownUntil){fanatic=true;episodes=Math.Min(1000000,episodes+1);}
  else if(fanatic&&pressure<=12){fanatic=false;recoveries=Math.Min(episodes,recoveries+1);cooldownUntil=now+60;}
 }
}
public sealed partial class SocialMemory {
 public float nextWorshipAt,worshipSeconds;public int rituals;
 bool ReligionValid()=>HybridState.Number(nextWorshipAt,0,100000300)&&HybridState.Number(worshipSeconds,0,100000000)&&Count(rituals);
}
// A scheduler-owned activity: no independent Update, scene scans or competing motor controller.
public sealed class VillageWorship {
 readonly VillagerRoutine[] jobs;readonly VillageResources world;readonly CreatureHybrid god;readonly VillageFamilies families;readonly Vector3 altar;
 readonly bool[] active;readonly float[] elapsed,praying,retry;readonly Vector3[] destinations;SliceState known;float perception;bool diversion;
 public int Worshippers{get{int count=0;foreach(bool value in active)if(value)count++;return count;}}
 public string Description=>world.Season.Religion.fanatic?"Fanaticism · work abandoned":world.Season.Religion.pressure>=10||world.VillageFaith>=.65f?"Zeal · worship growing":"Balanced devotion";
 public VillageWorship(VillagerRoutine[] people,VillageResources resources,CreatureHybrid creature,VillageFamilies homes,Vector3 center){jobs=people;world=resources;god=creature;families=homes;altar=center;active=new bool[jobs.Length];elapsed=new float[jobs.Length];praying=new float[jobs.Length];retry=new float[jobs.Length];destinations=new Vector3[jobs.Length];}
 public bool Active(VillagerRoutine job){int i=Array.IndexOf(jobs,job);return i>=0&&active[i];}
 public void Advance(float dt){if(world.Season.paused||dt<=0)return;EnsureSeason();perception-=dt;if(perception<=0){perception=.5f;var intent=god.State.intention;bool opportunity=god.Owner!=CreatureHybrid.ActionOwner.None&&intent.active&&(intent.action==CreatureAction.PlayWithToy||intent.action==CreatureAction.ConstructionPlay||intent.action==CreatureAction.SocialInteraction||intent.action==CreatureAction.WaterDelivery);int seen=0;if(opportunity)foreach(var job in jobs)if(world.ProvisionVisible(job.GetComponent<VillagerSocial>(),god.transform.position,god.transform))seen++;diversion=seen>=Mathf.CeilToInt(jobs.Length/3f);}
  int fervent=0;foreach(var job in jobs)if(job.GetComponent<VillagerSocial>().Memory.faith>=.85f)fervent++;var model=world.Season.Religion;bool previous=model.fanatic;bool hardship=(!world.Season.observeWithoutFoodWork&&world.LowFood)||(!world.Season.developmentFocus&&world.Season.water<15);model.Advance(dt,world.Season.Hybrid.time,fervent/(float)jobs.Length,diversion,hardship);
  if(previous!=model.fanatic)god.Journal.Add(world.Season.Hybrid.time,"village religion",CreatureAction.IdleOrObserve,"village",model.fanatic?"Widespread obsession interrupts work":"Worship subsides; work can resume");
 }
 public void ResetWorld(){known=null;EnsureSeason();}
 void EnsureSeason(){if(known==world.Season)return;for(int i=0;i<jobs.Length;i++){if(active[i]){active[i]=false;jobs[i].GetComponent<HumanoidMotion>().Pray(false);jobs[i].ResetTask();}retry[i]=0;}known=world.Season;perception=0;diversion=false;}
 public bool Tick(VillagerRoutine job,float dt){if(world.Season.paused||!HybridState.Number(dt,0,1)||dt<=0)return false;EnsureSeason();int i=Array.IndexOf(jobs,job);if(i<0)return false;var social=job.GetComponent<VillagerSocial>();var memory=social.Memory;bool fanatic=world.Season.Religion.fanatic;
  if(job.GetComponent<CarryItem>().held||memory.injury>.05f||social.RecentFright){End(i,false);return false;}
  if(active[i]&&!fanatic&&world.Season.Hybrid.time<world.Season.Religion.cooldownUntil){End(i,true);return false;}
  if(!active[i]){if(world.Season.Hybrid.time<retry[i])return false;if(!fanatic){if(memory.faith<.45f)return false;if(families!=null&&families.Owns(job))return false;if(memory.nextWorshipAt==0){memory.nextWorshipAt=world.Season.Hybrid.time+30+i*3;return false;}if(world.Season.Hybrid.time<memory.nextWorshipAt||social.Active||job.Signalling||job.GetComponent<VillagerResourceGathering>().Active||job.GetComponent<VillagerExploration>().Active||job.GetComponent<VillagerBlockPlay>().Active)return false;}
   float angle=i*Mathf.PI*2/jobs.Length;var wanted=altar+new Vector3(Mathf.Cos(angle),0,Mathf.Sin(angle))*(14+(i%2)*5)*world.Navigation.Scale;
   if(!world.Navigation.Sample(wanted,world.Navigation.villager,4,out var point)){retry[i]=world.Season.Hybrid.time+20;return false;}destinations[i]=world.Navigation.ProjectFoot(point,world.Navigation.villager);families?.Release(job);job.GetComponent<VillagerResourceGathering>().Cancel();social.ResetTask();job.GetComponent<VillagerExploration>().ResetTask();job.GetComponent<VillagerBlockPlay>().ResetTask();job.BeginWorship();active[i]=true;elapsed[i]=praying[i]=0;
  }
  elapsed[i]+=dt;var nav=job.GetComponent<IslandNavigator>();var motion=job.GetComponent<HumanoidMotion>();if(nav.MoveTo(destinations[i],job.transform.lossyScale.y*1.6f,dt)){praying[i]+=dt;memory.worshipSeconds=Mathf.Min(100000000,memory.worshipSeconds+dt);var toward=god.transform.position-job.transform.position;toward.y=0;if(toward.sqrMagnitude>1e-10f)job.transform.rotation=Quaternion.LookRotation(toward);motion.LookAt(god.transform);motion.Activity(0);motion.Pray(true);job.GetComponent<VillagerAffect>().ProvisionSeen();if(!fanatic&&praying[i]>=(memory.faith>=.65f?8:4)){End(i,true);return false;}}
  else{motion.Pray(false);if(nav.Blocked&&elapsed[i]>8||elapsed[i]>45&&praying[i]==0){End(i,false);retry[i]=world.Season.Hybrid.time+20;return false;}}
  return true;
 }
 void End(int i,bool completed){if(!active[i])return;active[i]=false;var job=jobs[i];job.GetComponent<HumanoidMotion>().Pray(false);job.ResetTask();var memory=job.GetComponent<VillagerSocial>().Memory;memory.nextWorshipAt=world.Season.Hybrid.time+(memory.faith>=.65f?35:70)+i;if(completed&&praying[i]>0)memory.rituals=Math.Min(1000000,memory.rituals+1);}
}
}
