using System;
using System.Linq;
using UnityEngine;
namespace BWVR {
[Serializable] public sealed class GatheringPlaceMemory {
 public int slot,visits,uncomfortable;public float appeal=.5f,nextVisitAt;
 public bool Valid()=>slot>=0&&slot<3&&visits>=0&&visits<=1000000&&uncomfortable>=0&&uncomfortable<=visits&&HybridState.Number(appeal,0,1)&&HybridState.Number(nextVisitAt,0,100000300);
}
public sealed partial class SocialMemory {
 public GatheringPlaceMemory[] gatheringPlaces;
 bool PlacesValid()=>gatheringPlaces==null||(gatheringPlaces.Length<=3&&gatheringPlaces.All(p=>p!=null&&p.Valid())&&gatheringPlaces.Select(p=>p.slot).Distinct().Count()==gatheringPlaces.Length);
 public GatheringPlaceMemory Place(int slot)=>gatheringPlaces?.FirstOrDefault(p=>p.slot==slot);
 // A shared rest and its host's later bench completion are one visit, not two rewards.
 public bool RememberPlace(int slot,bool comfortable,float now){
  if(slot<0||slot>=3||!HybridState.Number(now,0,100000000))return false;
  var place=Place(slot);if(place!=null&&now<place.nextVisitAt)return false;
  if(place==null){place=new GatheringPlaceMemory{slot=slot};gatheringPlaces=(gatheringPlaces??Array.Empty<GatheringPlaceMemory>()).Concat(new[]{place}).ToArray();}
  place.visits=Math.Min(1000000,place.visits+1);if(!comfortable)place.uncomfortable=Math.Min(place.visits,place.uncomfortable+1);
  place.appeal=Mathf.Clamp01(place.appeal+(comfortable?.1f:-.2f));place.nextVisitAt=now+90;return true;
 }
 public bool WantsPlace(int slot,float now,bool comfortable){var place=Place(slot);if(!comfortable||place!=null&&now<place.nextVisitAt)return false;return restNeed>(place!=null&&place.appeal>=.6f?.35f:place!=null&&place.appeal<.4f?.9f:.65f);}
}
public sealed partial class VillagerExploration {
 bool CoveredAt(ConstructionSite site,Vector3 at)=>site!=null&&site.TryShelterSpot(nav.profile.radius+.08f,nav.profile.height,out var rest,out _)&&Vector3.Distance(rest,at)<nav.world.Scale*2;
 bool ComfortableAt(ConstructionSite site,Vector3 at)=>construction.GetComponent<CreatureHybrid>().Season.Weather.rain<.5f||CoveredAt(site,at);
 void RememberPlaceAt(ConstructionSite site,bool comfortable){var god=construction.GetComponent<CreatureHybrid>();if(social.Memory.RememberPlace(SiteSlot(site),comfortable,god.State.time))god.Journal.Add(god.State.time,"gathering place",CreatureAction.IdleOrObserve,GetComponent<HybridTarget>().id,comfortable?"Comfortable visit; more willing to return":"Exposed visit; less willing to return");}
 void RememberNearbyPlace(bool comfortable){var site=construction.sites.Where(s=>!s.IsBridge&&Vector3.Distance(s.transform.position,transform.position)<nav.world.Scale*18).OrderBy(s=>(s.transform.position-transform.position).sqrMagnitude).FirstOrDefault();if(site!=null)RememberPlaceAt(site,comfortable);}
 bool WantsBench(ConstructionSite site){var god=construction.GetComponent<CreatureHybrid>();return social.Memory.WantsPlace(SiteSlot(site),god.State.time,ComfortableAt(site,site.World(new Vector3(0,0,-4))));}
}
}
