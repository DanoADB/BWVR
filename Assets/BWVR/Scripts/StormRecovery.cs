using System;
using UnityEngine;
namespace BWVR {
[Serializable] public sealed class StormRecoveryState {
 public bool recorded,thanked,invited,closed;public float until,nextAttempt;
 public bool Valid()=>HybridState.Number(until,0,100000300)&&HybridState.Number(nextAttempt,0,100000300)&&(!thanked&&!invited&&!closed||recorded)&&(!recorded||until>0)&&(!invited||closed);
}
public sealed partial class SocialMemory {
 public int stormsSurvived,stormCreatureRescues,stormThanks,stormPlayInvitations;
 bool StormRecoveryValid()=>Count(stormsSurvived)&&Count(stormCreatureRescues)&&stormCreatureRescues<=stormsSurvived&&Count(stormThanks)&&stormThanks<=stormCreatureRescues&&Count(stormPlayInvitations)&&stormPlayInvitations<=stormsSurvived;
}
public sealed partial class StormChapter {
 public StormRecoveryState Recovery=>State.recovery;
 public bool RecoveryReady=>!season().paused&&(State.phase==StormPhase.Recovery||State.phase==StormPhase.Complete)&&Recovery?.recorded==true&&!Recovery.closed&&god.State.time<Recovery.until&&weather.BurningCount==0&&weather.State.rain<.25f;
 public bool NeedsStormThanks=>RecoveryReady&&State.source==RescueSource.Creature&&!Recovery.thanked;
 public bool WantsRecoveryPlay=>RecoveryReady&&!Recovery.invited&&(State.source!=RescueSource.Creature||Recovery.thanked);
 void BindRecovery(){foreach(var job in jobs)job.GetComponent<VillagerExploration>().CancelStormRecovery(this);}
 void AdvanceRecovery(){
  var r=Recovery;if(r==null||season().paused||State.phase!=StormPhase.Recovery&&State.phase!=StormPhase.Complete)return;
  if(!r.recorded&&State.resolved&&Person!=null){r.recorded=true;r.until=god.State.time+180;r.nextAttempt=god.State.time+3;var m=Person.Memory;m.stormsSurvived=Math.Min(1000000,m.stormsSurvived+1);if(State.source==RescueSource.Creature)m.stormCreatureRescues=Math.Min(m.stormsSurvived,m.stormCreatureRescues+1);Person.GetComponent<VillagerAffect>()?.Reassured();god.Journal.Add(god.State.time,"storm recovery",CreatureAction.IdleOrObserve,Person.GetComponent<HybridTarget>().id,State.source+" arrival remembered; damage retained");}
  if(r.recorded&&!r.closed&&god.State.time>=r.until){r.closed=true;BindRecovery();}
 }
 bool TickRecoveryVillager(VillagerRoutine job,float dt){
  if(job.GetComponent<VillagerSocial>()!=Person)return false;var actor=job.GetComponent<VillagerExploration>();
  if(actor.StormThanking){actor.Tick(dt,false);return true;}
  if(!RecoveryReady||god.State.time<Recovery.nextAttempt)return false;Recovery.nextAttempt=god.State.time+5;
  if(NeedsStormThanks&&actor.TryStormThanks(this)){actor.Tick(dt,false);return true;}
  if(WantsRecoveryPlay&&actor.TryStormReturnPlay(this))return true;return false;
 }
 public bool ConfirmStormThanks(VillagerExploration actor){
  if(!NeedsStormThanks||actor==null||actor.GetComponent<VillagerSocial>()!=Person||!actor.StormThanking||actor.GetComponent<CarryItem>().held||god.GetComponent<CarryItem>().held||Vector3.Distance(actor.transform.position,god.transform.position)>nav.Scale*8)return false;
  Recovery.thanked=true;Recovery.nextAttempt=god.State.time+5;Person.Memory.stormThanks=Math.Min(Person.Memory.stormCreatureRescues,Person.Memory.stormThanks+1);Person.GetComponent<VillagerAffect>()?.Reassured();god.Journal.Add(god.State.time,"storm thanks",CreatureAction.SocialInteraction,Person.GetComponent<HybridTarget>().id,"Survivor thanked the creature for its actual rescue");if(god.Owner==CreatureHybrid.ActionOwner.None)god.GetComponent<CreatureCuriosity>().NoticeGreeting(Person.transform);return true;
 }
 public bool ConfirmStormInvitation(VillagerExploration actor,BeachBall ball){
  if(!WantsRecoveryPlay||actor==null||actor.GetComponent<VillagerSocial>()!=Person||actor.GetComponent<CarryItem>().held||ball==null||!actor.StormInviteOffered(this,ball)||!ball.OwnsPlayer(actor))return false;
  Recovery.invited=true;Recovery.closed=true;Person.Memory.stormPlayInvitations=Math.Min(Person.Memory.stormsSurvived,Person.Memory.stormPlayInvitations+1);god.Journal.Add(god.State.time,"storm return to play",CreatureAction.PlayWithToy,Person.GetComponent<HybridTarget>().id,"Survivor offered a physical ball invitation; acceptance remains autonomous");return true;
 }
 void StopRecovery(){if(Recovery?.recorded==true)Recovery.closed=true;BindRecovery();}
}
}
