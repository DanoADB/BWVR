using System;
using System.Linq;
using UnityEngine;
namespace BWVR {
public sealed partial class SocialMemory {
 public int musicGatherings,musicDisappointments;public float musicInterest=.5f,nextMusicAt;
 bool MusicValid()=>Count(musicGatherings)&&Count(musicDisappointments)&&Unit(musicInterest)&&HybridState.Number(nextMusicAt,0,100000300);
 public bool RememberMusic(bool sheltered,float rain,float now){if(!HybridState.Number(rain,0,1)||!HybridState.Number(now,0,100000000)||now<nextMusicAt)return false;bool comfortable=rain<.5f||sheltered;musicGatherings=Math.Min(1000000,musicGatherings+1);if(!comfortable)musicDisappointments=Math.Min(1000000,musicDisappointments+1);musicInterest=Mathf.Clamp01(musicInterest+(comfortable?.08f:-.12f));restNeed=Mathf.Clamp01(restNeed+(comfortable?-.15f:.05f));nextMusicAt=now+90;return true;}
}
// Uses the established exploration lease and animation; never takes a second motor.
public sealed partial class VillagerExploration {
 VillageResources musicWorld;SharedRhythm music;float musicVisible,musicElapsed,musicSight;bool musicSeen;int musicSession;
 public bool MusicAudience=>Current==Phase.MusicAudience;
 public bool CanSeeMusic(SharedRhythm instrument){var person=instrument!=null?instrument.Performer:null;if(person==null||Vector3.Distance(transform.position,instrument.transform.position)>nav.world.Scale*24)return false;var resources=musicWorld!=null?musicWorld:(musicWorld=nav.world.GetComponentInChildren<VillageResources>());return resources!=null&&resources.ProvisionVisible(social,person.transform.position,person.transform);}
 public bool TryWatchMusic(SharedRhythm instrument){if(!CanStartStructure||instrument==null||!instrument.Active||instrument.Performer==social||construction==null||construction.GetComponent<CreatureHybrid>().Season.paused||(!construction.GetComponent<CreatureHybrid>().Season.developmentFocus&&!construction.GetComponent<CreatureHybrid>().Season.Finished&&construction.GetComponent<CreatureHybrid>().Season.water<45)||social.Memory.injury>0||social.RecentFright||social.Memory.nextMusicAt>construction.GetComponent<CreatureHybrid>().State.time||social.Memory.musicInterest+social.Memory.boldness*.2f<.4f||!CanSeeMusic(instrument)||!instrument.ReserveListener(this))return false;
  music=instrument;musicSession=instrument.Session;musicVisible=musicElapsed=musicSight=0;musicSeen=true;routine.ResetTask();nav.ResetRoute();Current=Phase.MusicAudience;motion?.LookAt(instrument.transform);motion?.Activity(6);return true;
 }
 bool TickMusic(float dt){if(music==null||!music.Active||music.Session!=musicSession||social.Memory.injury>0||social.RecentFright||routine.BuildingCathedral||routine.Worshipping||musicElapsed>16){ResetTask();return false;}musicElapsed+=dt;musicSight-=dt;var toward=music.transform.position-transform.position;toward.y=0;if(toward.sqrMagnitude>1e-10f)transform.rotation=Quaternion.RotateTowards(transform.rotation,Quaternion.LookRotation(toward),dt*100);motion?.LookAt(music.transform);if(musicSight<=0){musicSight=.25f;musicSeen=CanSeeMusic(music);}if(musicSeen)musicVisible+=dt;return true;}
 public bool WitnessMusic(SharedRhythm instrument){if(!MusicAudience||music!=instrument||musicSession!=instrument.Session||!instrument.Completed||musicVisible<1.5f||!CanSeeMusic(instrument)||actor.held)return false;var god=construction.GetComponent<CreatureHybrid>();if(god.Season.paused)return false;
  bool cover=false;foreach(var site in construction.sites)if(site.TryShelterSpot(nav.profile.radius+.08f,nav.profile.height,out var rest,out _)&&Vector3.Distance(rest,transform.position)<nav.world.Scale*2){cover=true;break;}
  bool comfortable=god.Season.Weather.rain<.5f||cover;var performer=instrument.Performer;if(!social.Memory.RememberMusic(cover,god.Season.Weather.rain,god.State.time))return false;RememberNearbyPlace(comfortable);if(comfortable&&performer!=null){social.Memory.RememberPeer(performer.GetComponent<HybridTarget>().id,true,god.State.time);social.Memory.trust=Mathf.Clamp01(social.Memory.trust+.01f);}god.Journal.Add(god.State.time,"music gathering",CreatureAction.SocialInteraction,GetComponent<HybridTarget>().id,comfortable?"Enjoyed a shared performance":"Exposed rain spoiled the gathering");ResetTask();Current=Phase.Celebrate;if(comfortable)motion?.React();else motion?.Signal(false,true,instrument.transform.position);return true;
 }
 void ReleaseMusic(){var old=music;music=null;old?.ReleaseListener(this);musicVisible=musicElapsed=0;motion?.LookAt(null);}
}
public sealed partial class SharedRhythm {
 public VillagerSocial Performer=>host;
 public int Session{get;private set;}
 readonly VillagerExploration[] audience=new VillagerExploration[3];VillagerExploration[] nearby;
 public int AudienceCount=>audience.Count(a=>a!=null);
 public bool ReserveListener(VillagerExploration listener){if(!Active||listener==null||audience.Contains(listener))return false;for(int i=0;i<audience.Length;i++)if(audience[i]==null){audience[i]=listener;return true;}return false;}
 public void ReleaseListener(VillagerExploration listener){for(int i=0;i<audience.Length;i++)if(audience[i]==listener)audience[i]=null;}
 void InviteAudience(){Session=Session==int.MaxValue?1:Session+1;if(nearby==null)nearby=creature.GetComponentInParent<QuestSlice>().GetComponentsInChildren<VillagerExploration>();foreach(var person in nearby.OrderBy(p=>(p.transform.position-transform.position).sqrMagnitude)){if(AudienceCount>=3)break;person.TryWatchMusic(this);}}
 void FinishAudience(bool success){for(int i=0;i<audience.Length;i++){var listener=audience[i];if(listener==null)continue;if(!success||!listener.WitnessMusic(this))listener.ResetTask();audience[i]=null;}}
}
}
