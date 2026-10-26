using System.Linq;
using UnityEngine;
namespace BWVR {
// Shelter is a leisure activity on the existing motor and visitor reservation.
public sealed partial class VillagerExploration {
 bool shelterVisit,shelterAtEntrance;float shelteredClock,shelterCheck;Vector3 shelterRest,shelterEntrance;
 bool ChooseShelter(){
  if(construction==null||social.Memory.restNeed<=.65f)return false;
  foreach(var site in construction.sites.OrderBy(s=>(s.transform.position-transform.position).sqrMagnitude))if(TryShelter(site))return true;return false;
 }
 public bool TryShelter(ConstructionSite site){
  if(!CanStartStructure||construction==null||site==null||site.Visitor!=null||site.Repairer!=null||site.Requester!=null||site.Paused||social.Memory.restNeed<=.65f)return false;
  if(!site.TryShelterSpot(nav.profile.radius+.08f,nav.profile.height,out var rest,out var entry)||!world.Reachable(transform.position,entry,nav.profile)||!world.Reachable(entry,rest,nav.profile))return false;
  structure=site;site.Visitor=this;shelterVisit=true;shelterAtEntrance=false;shelteredClock=0;shelterCheck=0;shelterRest=rest;shelterEntrance=entry;structureGoal=entry;
  routine.ResetTask();nav.ResetRoute();clock=elapsed=0;Current=Phase.ApproachStructure;motion?.LookAt(site.transform);return true;
 }
 bool ShelterStillUseful(){return structure!=null&&structure.Visitor==this&&structure.Repairer==null&&structure.TryShelterSpot(nav.profile.radius+.08f,nav.profile.height,out var rest,out var entry)&&(rest-shelterRest).sqrMagnitude<structure.unit*structure.unit*.01f&&(entry-shelterEntrance).sqrMagnitude<structure.unit*structure.unit*.01f;}
 bool TickShelter(float dt){
  shelterCheck-=dt;if(shelterCheck<=0&&!ShelterStillUseful()){social.Memory.buildDisappointments=System.Math.Min(1000000,social.Memory.buildDisappointments+1);ResetTask();return false;}if(shelterCheck<=0)shelterCheck=.25f;
  if(Current==Phase.ApproachStructure){if(nav.MoveTo(structureGoal,transform.lossyScale.y*1.6f,dt)){
    if(!shelterAtEntrance){shelterAtEntrance=true;structureGoal=shelterRest;nav.ResetRoute();}
    else {var delta=transform.position-shelterRest;delta.y=0;if(delta.magnitude>structure.unit*.35f){ResetTask();return false;}Current=Phase.UseStructure;clock=0;motion?.Activity(0);}
   }}
  else if(Current==Phase.UseStructure){var delta=transform.position-shelterRest;delta.y=0;if(delta.magnitude>structure.unit*.35f){ResetTask();return false;}motion?.LookAt(structure.transform);shelteredClock+=dt;
   if(shelteredClock>=6){if(!ShelterStillUseful()){ResetTask();return false;}var memory=social.Memory;memory.RememberShelter(shelteredClock,construction.GetComponent<CreatureHybrid>().Season.Weather.rain);memory.constructionUses=System.Math.Min(1000000,memory.constructionUses+1);
    var god=construction.GetComponent<CreatureHybrid>();god.Journal.Add(god.State.time,"shelter rest",CreatureAction.IdleOrObserve,GetComponent<HybridTarget>().id,"Supported cover; rest relief; rain "+god.Season.Weather.rain.ToString("0.00"));ResetTask();Current=Phase.Celebrate;motion?.React();return true;
   }
  }if(nav.Blocked&&elapsed>5)ResetTask();return Active;
 }
}
}
