using System;
using UnityEngine;
namespace BWVR {
public sealed partial class BeachBall {
 public VillagerExploration ProspectivePlayer{get;private set;}
 public string ConfirmedPartner{get;private set;}="";
 public void RefreshPlaymate(){
  ProspectivePlayer=null;
  if(creature==null||Player!=null||item.held||InFlight||creature.Season.paused||creature.Season.WorldPlay.rallyFun<=0||creature.State.time<creature.Season.WorldPlay.nextCreatureInvite)return;
  float best=float.NegativeInfinity;
  foreach(var target in creature.Targets){var resident=target.villager;if(resident==null)continue;var visitor=resident.GetComponent<VillagerExploration>();if(visitor==null||!visitor.CanReceivePlayInvite(this))continue;
   var bond=creature.State.Bond(target.id);if(bond!=null&&creature.State.time<bond.nextInviteAt)continue;
   float distance=Vector3.Distance(resident.transform.position,transform.position)/nav.Scale;if(distance>120)continue;
   float score=creature.State.PlaymateAffinity(target.id)*.7f+resident.Attraction*.2f+resident.Memory.boldness*.05f-distance*.0025f;
   if(score>best){best=score;ProspectivePlayer=visitor;}
  }
 }
 public bool TryInvitePlaymate(){
  RefreshPlaymate();var chosen=ProspectivePlayer;if(chosen==null)return false;
  var id=chosen.GetComponent<HybridTarget>().id;var bond=creature.State.Bond(id,true);if(bond==null)return false;
  bond.nextInviteAt=creature.State.time+90;creature.Season.WorldPlay.nextCreatureInvite=creature.State.time+70;
  bool accepted=chosen.TryCreaturePlayInvite(this);if(accepted)bond.acceptedInvitations=Math.Min(1000000,bond.acceptedInvitations+1);else bond.declinedInvitations=Math.Min(1000000,bond.declinedInvitations+1);
  creature.Journal.Add(creature.State.time,accepted?"creature invites playmate":"play invitation declined",CreatureAction.PlayWithToy,id,accepted?"Gaze and gesture toward the ball":"Unavailable or unwilling; no relationship penalty");
  if(accepted)creature.GetComponent<CreatureCuriosity>().NoticeGreeting(chosen.transform);return accepted;
 }
}
}
