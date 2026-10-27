using System;
using System.Linq;
using UnityEngine;
namespace BWVR {
[Serializable] public sealed class GatheringPlaceMemory {
 public int slot,visits,uncomfortable,requests,answers;public float appeal=.5f,nextVisitAt,nextRequestAt;
 public bool Ask(float now){if(!HybridState.Number(now,0,100000000)||now<nextRequestAt||visits==0)return false;requests=Math.Min(1000000,requests+1);nextRequestAt=now+180;return true;}
 public bool Valid()=>slot>=0&&slot<3&&visits>=0&&visits<=1000000&&uncomfortable>=0&&uncomfortable<=visits&&requests>=0&&requests<=1000000&&answers>=0&&answers<=requests&&HybridState.Number(nextRequestAt,0,100000300)&&HybridState.Number(appeal,0,1)&&HybridState.Number(nextVisitAt,0,100000300);
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
 public float PlacePlayBias(int slot,bool comfortable)=>Mathf.Clamp(((Place(slot)?.appeal??.5f)-.5f)*.3f+(comfortable?0:-.15f),-.15f,.15f);
 public bool WantsPlace(int slot,float now,bool comfortable){var place=Place(slot);if(!comfortable||place!=null&&now<place.nextVisitAt)return false;return restNeed>(place!=null&&place.appeal>=.6f?.35f:place!=null&&place.appeal<.4f?.9f:.65f);}
}
public sealed partial class VillagerExploration {
 bool CoveredAt(ConstructionSite site,Vector3 at)=>site!=null&&site.TryShelterSpot(nav.profile.radius+.08f,nav.profile.height,out var rest,out _)&&Vector3.Distance(rest,at)<nav.world.Scale*2;
 bool ComfortableAt(ConstructionSite site,Vector3 at)=>construction.GetComponent<CreatureHybrid>().Season.Weather.rain<.5f||CoveredAt(site,at);
 bool RememberPlaceAt(ConstructionSite site,bool comfortable){var god=construction.GetComponent<CreatureHybrid>();bool remembered=social.Memory.RememberPlace(SiteSlot(site),comfortable,god.State.time);if(remembered)god.Journal.Add(god.State.time,"gathering place",CreatureAction.IdleOrObserve,GetComponent<HybridTarget>().id,comfortable?"Comfortable visit; more willing to return":"Exposed visit; less willing to return");return remembered;}
 ConstructionSite NearbyPlace(Vector3 at)=>construction?.sites.Where(s=>!s.IsBridge&&Vector3.Distance(s.transform.position,at)<nav.world.Scale*18).OrderBy(s=>(s.transform.position-at).sqrMagnitude).FirstOrDefault();
 bool RememberNearbyPlace(bool comfortable){var site=NearbyPlace(transform.position);return site!=null&&RememberPlaceAt(site,comfortable);}
 float BallPlaceBias(BeachBall ball){var foot=nav.world.ProjectFoot(ball.transform.position,nav.profile);var site=NearbyPlace(foot);if(site==null)return 0;return social.Memory.PlacePlayBias(SiteSlot(site),ComfortableAt(site,foot));}
 BeachBall rememberedPlayBall;int rememberedPlaySession=-1;
 // Called only at the existing four-reception boundary; no credit for invitations or solo kicks.
 public bool WitnessBallPlay(BeachBall ball){
  if(Current!=Phase.Beachball||ball==null||playingBall!=ball||!ball.OwnsPlayer(this)||ball.RallyReceptions!=4||ball.Harmful||actor.held||ball.GetComponent<CarryItem>().held||ball.Playmate.Season.paused||rememberedPlayBall==ball&&rememberedPlaySession==ball.ObservationSession)return false;
  rememberedPlayBall=ball;rememberedPlaySession=ball.ObservationSession;
  var site=NearbyPlace(transform.position);return site!=null&&RememberPlaceAt(site,ComfortableAt(site,transform.position));
 }
 bool WantsBench(ConstructionSite site){var god=construction.GetComponent<CreatureHybrid>();return social.Memory.WantsPlace(SiteSlot(site),god.State.time,ComfortableAt(site,site.World(new Vector3(0,0,-4))));}
}
}
