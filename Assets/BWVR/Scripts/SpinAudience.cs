using System;
using System.Linq;
using UnityEngine;
namespace BWVR {
public sealed partial class SocialMemory {
 public int tricksEnjoyed;public float trickInterest=.5f,nextTrickAt;
 bool TricksValid()=>Count(tricksEnjoyed)&&Unit(trickInterest)&&HybridState.Number(nextTrickAt,0,100000300);
 public bool RememberTrick(float now){if(!HybridState.Number(now,0,100000000)||now<nextTrickAt)return false;tricksEnjoyed=Math.Min(1000000,tricksEnjoyed+1);trickInterest=Mathf.Clamp01(trickInterest+.06f);trust=Mathf.Clamp01(trust+.015f);affinity=Mathf.Clamp01(affinity+.01f);nextTrickAt=now+90;return true;}
}
// A bounded audience observes the existing action; it never controls the creature or the toy.
public sealed partial class BallSpin {
 readonly VillagerExploration[] audience=new VillagerExploration[2];VillagerExploration[] neighbors;
 public CreatureHybrid Performer=>god;public int AudienceCount=>audience.Count(p=>p!=null);
 void InviteAudience(){if(neighbors==null)neighbors=god.GetComponentInParent<QuestSlice>().GetComponentsInChildren<VillagerExploration>();foreach(var p in neighbors.Where(p=>p!=null).OrderBy(p=>(p.transform.position-anchor).sqrMagnitude)){if(AudienceCount>=2)break;p.TryWatchSpin(this);}}
 public bool ReserveSpectator(VillagerExploration person){int slot=Array.IndexOf(audience,null);if(!Active||person==null||slot<0||audience.Contains(person))return false;audience[slot]=person;return true;}
 public void ReleaseSpectator(VillagerExploration person){for(int i=0;i<audience.Length;i++)if(audience[i]==person)audience[i]=null;}
 void FinishAudience(bool success){foreach(var person in audience.ToArray()){if(person==null)continue;if(!success||!person.WitnessSpin(this))person.ResetTask();ReleaseSpectator(person);}}
}
public sealed partial class VillagerExploration {
 BallSpin watchedSpin;float spinSeen,spinSight,spinElapsed,applause;bool spinVisible;
 public bool WatchingSpin=>Current==Phase.SpinAudience&&watchedSpin!=null;
 public bool ApplaudingSpin=>Current==Phase.SpinAudience&&applause>0;
 public bool CanSeeSpin(BallSpin spin){if(spin==null||actor.held)return false;var god=spin.Performer;float distance=Vector3.Distance(transform.position,god.transform.position)/nav.world.Scale;if(distance>32||distance<nav.world.creature.radius+nav.profile.radius+1)return false;var resources=musicWorld!=null?musicWorld:(musicWorld=nav.world.GetComponentInChildren<VillageResources>());return resources!=null&&resources.ProvisionVisible(social,god.transform.position,god.transform);}
 public bool TryWatchSpin(BallSpin spin){if(!isActiveAndEnabled||!CanStartStructure||spin==null||!spin.Active||spin.Performer.Season.paused||social.Memory.injury>0||social.RecentFright||social.Memory.nextTrickAt>spin.Performer.State.time||social.Memory.trickInterest+social.Memory.boldness*.2f<.4f||(!spin.Performer.Season.developmentFocus&&!spin.Performer.Season.Finished&&spin.Performer.Season.water<45)||!CanSeeSpin(spin)||!spin.ReserveSpectator(this))return false;watchedSpin=spin;spinSeen=spinSight=spinElapsed=applause=0;spinVisible=true;routine.BeginHandsFreeActivity();nav.ResetRoute();Current=Phase.SpinAudience;motion?.Activity(0);motion?.LookAt(spin.Performer.transform);return true;}
 bool TickSpinAudience(float dt){if(social.Memory.injury>0||social.RecentFright||routine.BuildingCathedral||routine.Worshipping){ResetTask();return false;}if(applause>0){applause=Mathf.Max(0,applause-dt);motion?.Clap(Mathf.Abs(Mathf.Sin(applause*Mathf.PI*3)));if(applause<=0)ResetTask();return Active;}if(watchedSpin==null||!watchedSpin.Active||spinElapsed>5){ResetTask();return false;}spinElapsed+=dt;var at=watchedSpin.Performer.transform;var toward=at.position-transform.position;toward.y=0;if(toward.sqrMagnitude>1e-10f)transform.rotation=Quaternion.RotateTowards(transform.rotation,Quaternion.LookRotation(toward),dt*150);spinSight-=dt;if(spinSight<=0){spinSight=.25f;spinVisible=CanSeeSpin(watchedSpin);}if(spinVisible)spinSeen+=dt;motion?.LookAt(at);return true;}
 public bool WitnessSpin(BallSpin spin){if(!WatchingSpin||watchedSpin!=spin||!spin.JustCompleted||!spin.Active||spinSeen<1.5f||actor.held||social.Memory.injury>0||social.RecentFright||spin.Performer.Season.paused||!CanSeeSpin(spin))return false;var god=spin.Performer;if(!social.Memory.RememberTrick(god.State.time))return false;god.State.RememberBond(GetComponent<HybridTarget>().id,true);god.Journal.Add(god.State.time,"trick audience",CreatureAction.IdleOrObserve,GetComponent<HybridTarget>().id,"Enjoyed a witnessed creature spin");ReleaseSpinAudience();applause=1.8f;motion?.React();GetComponent<VillagerAffect>()?.EnjoyedTrick();IslandAudio.Cue(IslandSound.Content,transform.position,GetEntityId().GetHashCode(),.5f,1.15f);return true;}
 void ReleaseSpinAudience(){var old=watchedSpin;watchedSpin=null;old?.ReleaseSpectator(this);spinSeen=spinElapsed=spinSight=applause=0;spinVisible=false;motion?.Clap(0);motion?.LookAt(null);}
}
}
