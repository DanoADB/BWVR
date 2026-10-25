using System;
using System.Linq;
using UnityEngine;
namespace BWVR {
public sealed partial class SocialMemory {
 public int comfortGiven,comfortReceived;public float nextComfortAt;
 bool ComfortValid()=>Count(comfortGiven)&&Count(comfortReceived)&&HybridState.Number(nextComfortAt,0,100000300);
}
// Neighbors can offer company after distress; this neither heals nor changes creature trust.
public sealed partial class VillagerExploration {
 VillagerExploration comfortFriend;bool comfortHelper;Vector3 comfortGoal;
 public bool Comforting=>comfortFriend!=null;
 bool ComfortAvailable=>isActiveAndEnabled&&CanStartStructure&&construction!=null&&
  !construction.GetComponent<CreatureHybrid>().Season.paused&&construction.GetComponent<CreatureHybrid>().State.time>=social.Memory.nextComfortAt&&
  (construction.GetComponent<CreatureHybrid>().Season.developmentFocus||construction.GetComponent<CreatureHybrid>().Season.Finished||
  (construction.GetComponent<CreatureHybrid>().Season.water>=45&&construction.GetComponent<CreatureHybrid>().Season.crops>=45));
 bool Distressed=>social.Memory.injury>0||social.RecentFright;
 bool ChooseComfort(){if(!ComfortAvailable||Distressed)return false;foreach(var peer in neighbors.Where(p=>p!=null&&p!=this&&p.Distressed).OrderByDescending(p=>social.Memory.PeerAppeal(p.GetComponent<HybridTarget>().id)))if(TryComfort(peer))return true;return false;}
 public bool TryComfort(VillagerExploration friend){
  if(friend==null||friend==this||!ComfortAvailable||Distressed||!friend.ComfortAvailable||!friend.Distressed||Vector3.Distance(transform.position,friend.transform.position)>nav.world.Scale*80)return false;
  var god=construction.GetComponent<CreatureHybrid>();if(!social.Memory.PeerWilling(friend.GetComponent<HybridTarget>().id,god.State.time)||!friend.social.Memory.PeerWilling(GetComponent<HybridTarget>().id,god.State.time))return false;
  var side=transform.position-friend.transform.position;side.y=0;if(side.sqrMagnitude<1e-10f)side=friend.transform.forward;
  if(!nav.world.Sample(friend.transform.position+side.normalized*nav.world.Scale*3,nav.profile,5,out comfortGoal)||!world.Reachable(transform.position,comfortGoal,nav.profile))return false;
  comfortFriend=friend;comfortHelper=true;friend.comfortFriend=this;friend.comfortHelper=false;
  foreach(var member in new[]{this,friend}){member.routine.ResetTask();member.nav.ResetRoute();member.clock=member.elapsed=0;member.Current=Phase.ComfortNeighbor;member.motion?.LookAt(member.comfortFriend.transform);}
  return true;
 }
 bool TickComfort(float dt){
  var friend=comfortFriend;elapsed+=dt;
  if(friend==null||friend.comfortFriend!=this||!friend.isActiveAndEnabled||friend.actor.held||actor.held||elapsed>25||(comfortHelper?Distressed:friend.Distressed)){ResetTask();return false;}
  var recipient=comfortHelper?friend:this;if(!recipient.Distressed){ResetTask();return false;}
  motion?.LookAt(friend.transform);
  if(!comfortHelper)return true;
  if(nav.MoveTo(comfortGoal,transform.lossyScale.y*1.6f,dt)&&Vector3.Distance(transform.position,friend.transform.position)<nav.world.Scale*8){
   clock+=dt;motion?.Signal(true);var facing=friend.transform.position-transform.position;facing.y=0;if(facing.sqrMagnitude>1e-10f)transform.rotation=Quaternion.LookRotation(facing);
   if(clock>=3){var god=construction.GetComponent<CreatureHybrid>();float time=god.State.time;
    social.Memory.RememberPeer(friend.GetComponent<HybridTarget>().id,true,time);friend.social.Memory.RememberPeer(GetComponent<HybridTarget>().id,true,time);
    social.Memory.comfortGiven=Math.Min(1000000,social.Memory.comfortGiven+1);friend.social.Memory.comfortReceived=Math.Min(1000000,friend.social.Memory.comfortReceived+1);
    foreach(var member in new[]{this,friend}){member.social.Memory.nextComfortAt=time+90;member.social.Memory.restNeed=Mathf.Max(0,member.social.Memory.restNeed-.1f);}
    god.Journal.Add(time,"neighbor reassurance",CreatureAction.SocialInteraction,GetComponent<HybridTarget>().id,friend.GetComponent<HybridTarget>().id);
    ResetTask();Current=Phase.Celebrate;friend.Current=Phase.Celebrate;motion?.React();friend.motion?.React();return true;
   }
  }else{clock=0;if(nav.Blocked&&elapsed>5){ResetTask();return false;}}
  return Active;
 }
 void ReleaseComfort(){if(comfortFriend==null)return;var friend=comfortFriend;comfortFriend=null;var god=construction!=null?construction.GetComponent<CreatureHybrid>():null;
  if(god!=null)social.Memory.nextComfortAt=Mathf.Max(social.Memory.nextComfortAt,god.State.time+30);
  motion?.Signal(false);if(friend.comfortFriend==this){friend.comfortFriend=null;if(god!=null)friend.social.Memory.nextComfortAt=Mathf.Max(friend.social.Memory.nextComfortAt,god.State.time+30);friend.ResetTask();}}
}
}
