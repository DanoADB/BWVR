using System;
using System.Linq;
using UnityEngine;
namespace BWVR {
public sealed partial class SocialMemory {
 public int sharedRests;public float nextGatheringAt;
 bool GatheringsValid()=>Count(sharedRests)&&HybridState.Number(nextGatheringAt,0,100000300);
}
// A guest uses the existing leisure motor; the host retains the site's visitor lease.
public sealed partial class VillagerExploration {
 VillagerExploration gatheringHost,gatheringGuest;float togetherClock,gatheringCoverCheck;bool gatheringShelter,gatheringAtEntrance;Vector3 gatheringRest,gatheringEntrance;
 public bool Gathering=>gatheringHost!=null||gatheringGuest!=null;
 bool GatheringReady=>isActiveAndEnabled&&CanStartStructure&&social.Memory.injury<=0&&!social.RecentFright&&construction!=null&&
  !construction.GetComponent<CreatureHybrid>().Season.paused&&construction.GetComponent<CreatureHybrid>().State.time>=social.Memory.nextGatheringAt&&
  (construction.GetComponent<CreatureHybrid>().Season.developmentFocus||construction.GetComponent<CreatureHybrid>().Season.Finished||
  (construction.GetComponent<CreatureHybrid>().Season.water>=45&&construction.GetComponent<CreatureHybrid>().Season.crops>=45));
 public bool TryJoinGathering(VillagerExploration host){
  if(!GatheringReady||host==null||host==this||!host.isActiveAndEnabled||host.Current!=Phase.UseStructure||host.structure==null||!host.GatheringStructureUseful||host.structure.Visitor!=host||host.gatheringGuest!=null||host.actor.held||host.social.Memory.injury>0||host.social.RecentFright)return false;
  var god=construction.GetComponent<CreatureHybrid>();string own=GetComponent<HybridTarget>().id,other=host.GetComponent<HybridTarget>().id;
  if(god.State.time<host.social.Memory.nextGatheringAt||!social.Memory.PeerWilling(other,god.State.time)||!host.social.Memory.PeerWilling(own,god.State.time)||
   Vector3.Distance(transform.position,host.transform.position)>nav.world.Scale*80)return false;
  bool sheltered=host.shelterVisit;var goal=host.structure.World(new Vector3(4,0,-4));var entrance=goal;
  if(sheltered&&!host.structure.TryCompanionShelterSpot(nav.profile.radius+.08f,nav.profile.height,host.shelterRest,(nav.profile.radius+host.nav.profile.radius+.2f)*nav.world.Scale,out goal,out entrance))return false;
  if(!world.Reachable(transform.position,entrance,nav.profile)||sheltered&&!world.Reachable(entrance,goal,nav.profile))return false;
  gatheringShelter=sheltered;gatheringAtEntrance=!sheltered;gatheringRest=goal;gatheringEntrance=entrance;gatheringCoverCheck=0;
  gatheringHost=host;host.gatheringGuest=this;togetherClock=0;host.togetherClock=0;structureGoal=entrance;
  routine.ResetTask();nav.ResetRoute();clock=elapsed=0;Current=Phase.GatheringGuest;motion?.LookAt(host.transform);
  host.motion?.Signal(true,true,host.structure.transform.position);return true;
 }
 bool GatheringStructureUseful=>structure!=null&&!structure.Paused&&!structure.Locked&&(shelterVisit?ShelterStillUseful():structure.Recipe==BuildRecipe.Bench&&structure.RecipeComplete);
 void InviteGathering(){
  if(!GatheringStructureUseful||gatheringGuest!=null)return;
  foreach(var peer in neighbors.Where(p=>p!=null&&p!=this).OrderByDescending(p=>social.Memory.PeerAppeal(p.GetComponent<HybridTarget>().id)))
   if(peer.TryJoinGathering(this))break;
 }
 bool TickGathering(float dt){
  clock+=dt;elapsed+=dt;var host=gatheringHost;
  if(host==null||host.gatheringGuest!=this||host.Current!=Phase.UseStructure||host.actor.held||host.social.Memory.injury>0||social.Memory.injury>0||
   social.RecentFright||host.social.RecentFright||host.structure==null||host.structure.Visitor!=host||elapsed>25){ResetTask();return false;}
  gatheringCoverCheck-=dt;if(gatheringCoverCheck<=0){gatheringCoverCheck=.25f;
   if(!host.GatheringStructureUseful||gatheringShelter&&(!host.structure.TryCompanionShelterSpot(nav.profile.radius+.08f,nav.profile.height,host.shelterRest,(nav.profile.radius+host.nav.profile.radius+.2f)*nav.world.Scale,out var rest,out var entrance)||(rest-gatheringRest).sqrMagnitude>host.structure.unit*host.structure.unit*.01f||(entrance-gatheringEntrance).sqrMagnitude>host.structure.unit*host.structure.unit*.01f)){ResetTask();return false;}
  }
  if(nav.MoveTo(structureGoal,transform.lossyScale.y*1.6f,dt)){
   if(gatheringShelter&&!gatheringAtEntrance){gatheringAtEntrance=true;structureGoal=gatheringRest;nav.ResetRoute();return true;}
   if(gatheringShelter&&Vector3.Distance(transform.position,gatheringRest)>host.structure.unit*.35f){togetherClock=0;if(elapsed>5){ResetTask();return false;}return true;}
   motion?.LookAt(host.transform);host.motion?.LookAt(transform);host.motion?.Signal(false);
   var facing=host.transform.position-transform.position;facing.y=0;if(facing.sqrMagnitude>1e-10f)transform.rotation=Quaternion.LookRotation(facing);
   if(Vector3.Distance(transform.position,host.transform.position)<nav.world.Scale*10)togetherClock+=dt;else togetherClock=0;
   if(togetherClock>=3){
    var god=construction.GetComponent<CreatureHybrid>();float time=god.State.time;
    bool comfortable=ComfortableAt(host.structure,transform.position),hostComfortable=host.ComfortableAt(host.structure,host.transform.position);if(comfortable&&hostComfortable){social.Memory.RememberPeer(host.GetComponent<HybridTarget>().id,true,time);host.social.Memory.RememberPeer(GetComponent<HybridTarget>().id,true,time);}
    foreach(var member in new[]{this,host}){member.social.Memory.sharedRests=Math.Min(1000000,member.social.Memory.sharedRests+1);bool pleasant=member==this?comfortable:hostComfortable;member.RememberPlaceAt(host.structure,pleasant);member.social.Memory.restNeed=Mathf.Clamp01(member.social.Memory.restNeed+(pleasant?-.25f:.05f));member.social.Memory.nextGatheringAt=time+90;if(pleasant)member.motion?.React();else member.motion?.Signal(false,true,host.structure.transform.position);}
    god.Journal.Add(time,"shared gathering",CreatureAction.SocialInteraction,GetComponent<HybridTarget>().id,host.GetComponent<HybridTarget>().id);
    ResetTask();Current=Phase.Celebrate;return true;
   }
  }else{togetherClock=0;if(nav.Blocked&&elapsed>5){ResetTask();return false;}}
  return Active;
 }
 void ReleaseGathering(){
  var god=construction!=null?construction.GetComponent<CreatureHybrid>():null;
  if(Gathering){motion?.Signal(false);if(god!=null)social.Memory.nextGatheringAt=Mathf.Max(social.Memory.nextGatheringAt,god.State.time+30);}
  if(gatheringHost!=null){if(gatheringHost.gatheringGuest==this){gatheringHost.gatheringGuest=null;gatheringHost.motion?.Signal(false);}gatheringHost=null;}
  if(gatheringGuest!=null){var guest=gatheringGuest;gatheringGuest=null;guest.gatheringHost=null;guest.ResetTask();}
  togetherClock=0;gatheringShelter=gatheringAtEntrance=false;gatheringCoverCheck=0;
 }
}
}
