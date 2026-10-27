using System;
using System.Linq;
using UnityEngine;
namespace BWVR {
public enum PlaceImprovement {None,Cover,BeachBall}
// A short request on the existing leisure motor. The player and creature remain free to ignore it.
public sealed partial class VillagerExploration {
 ConstructionSite acceptedPlace;PlaceImprovement acceptedImprovement;BeachBall acceptedBall;SliceState acceptedSeason;
 PlaceImprovement placeRequest;BeachBall requestedBall;bool requestShown;float requestCheck;
 public PlaceImprovement GatheringRequest=>Current==Phase.PlaceRequest&&requestShown?placeRequest:PlaceImprovement.None;
 bool UsefulRequestCover(ConstructionSite site)=>site.TryShelterSpot(nav.profile.radius+.08f,nav.profile.height,out var rest,out var entry)&&world.Reachable(transform.position,entry,nav.profile)&&world.Reachable(entry,rest,nav.profile);
 bool FreeRequestBall(BeachBall ball)=>ball!=null&&ball.Player==null&&!ball.FetchReserved&&!ball.InFlight&&!ball.GetComponent<CarryItem>().held&&ball.GetComponent<CreatureAffordance>().Available;
 bool BallAtPlace(BeachBall ball,ConstructionSite site)=>FreeRequestBall(ball)&&Vector3.Distance(ball.transform.position,site.transform.position)<nav.world.Scale*18;
 BeachBall FamiliarVisibleBall(){
  if(social.Memory.ballGames+social.Memory.peerGames==0)return null;
  var resources=musicWorld!=null?musicWorld:(musicWorld=nav.world.GetComponentInChildren<VillageResources>());
  return beachballs.Where(b=>FreeRequestBall(b)&&resources!=null&&resources.ProvisionVisible(social,b.transform.position,b.transform)).OrderBy(b=>(b.transform.position-transform.position).sqrMagnitude).FirstOrDefault();
 }
 bool ChoosePlaceImprovement(){if(construction==null)return false;foreach(var site in construction.sites.Where(s=>!s.IsBridge).OrderByDescending(s=>social.Memory.Place(SiteSlot(s))?.visits??0))if(TryPlaceImprovement(site))return true;return false;}
 public bool TryPlaceImprovement(ConstructionSite site){
  if(!CanStartStructure||construction==null||site==null||site.IsBridge||site.Paused||site.Locked||site.Requester!=null||site.Visitor!=null||site.Repairer!=null||social.Memory.injury>0||social.RecentFright)return false;
  var god=construction.GetComponent<CreatureHybrid>();var memory=social.Memory.Place(SiteSlot(site));
  if(god.Season.paused||(!god.Season.developmentFocus&&!god.Season.Finished&&(god.Season.water<45||god.Season.crops<45))||memory==null||memory.visits==0||god.State.time<memory.nextRequestAt||Vector3.Distance(transform.position,site.transform.position)>nav.world.Scale*80)return false;
  PlaceImprovement need=PlaceImprovement.None;BeachBall toy=null;
  if(memory.uncomfortable>0&&social.Memory.restNeed>.65f&&god.Season.Weather.rain>=.5f&&!UsefulRequestCover(site))need=PlaceImprovement.Cover;
  else if(social.Willing&&memory.appeal>=.6f&&social.Memory.restNeed>.35f&&god.Season.Weather.rain<.5f&&!beachballs.Any(b=>BallAtPlace(b,site))){toy=FamiliarVisibleBall();if(toy!=null&&!BallAtPlace(toy,site))need=PlaceImprovement.BeachBall;}
  var destination=site.World(new Vector3(0,0,-11));if(need==PlaceImprovement.None||!world.Reachable(transform.position,destination,nav.profile))return false;
  structure=site;site.Requester=this;placeRequest=need;requestedBall=toy;requestShown=false;requestCheck=0;structureGoal=destination;clock=elapsed=0;routine.BeginHandsFreeActivity();nav.ResetRoute();Current=Phase.PlaceRequest;return true;
 }
 bool TickPlaceRequest(float dt){
  var site=structure;var god=construction.GetComponent<CreatureHybrid>();if(god.Season.paused)return true;elapsed+=dt;requestCheck-=dt;
  if(site==null||site.Requester!=this||site.Paused||site.Locked||site.Repairer!=null||routine.Worshipping||routine.BuildingCathedral||social.Memory.injury>0||social.RecentFright||elapsed>90){ResetTask();return false;}
  if(placeRequest==PlaceImprovement.Cover&&god.Season.Weather.rain<.5f){ResetTask();return false;}
  if(!requestShown){if(nav.MoveTo(structureGoal,transform.lossyScale.y*1.6f,dt)){var memory=social.Memory.Place(SiteSlot(site));if(memory==null||!memory.Ask(god.State.time)){ResetTask();return false;}requestShown=true;clock=0;motion?.Activity(0);god.Journal.Add(god.State.time,"place improvement request",CreatureAction.IdleOrObserve,GetComponent<HybridTarget>().id,placeRequest.ToString());}else if(nav.Blocked&&elapsed>5){ResetTask();return false;}return true;}
  clock+=dt;
  if(requestCheck<=0){requestCheck=.25f;bool answered=placeRequest==PlaceImprovement.Cover?UsefulRequestCover(site):BallAtPlace(requestedBall,site);
   if(answered){var memory=social.Memory.Place(SiteSlot(site));var need=placeRequest;var ball=requestedBall;ResetTask();bool accepted=need==PlaceImprovement.Cover?TryShelter(site):TryBall(ball);if(accepted){memory.answers=Math.Min(memory.requests,memory.answers+1);acceptedPlace=site;acceptedImprovement=need;acceptedBall=ball;acceptedSeason=god.Season;god.Journal.Add(god.State.time,"place improvement accepted",CreatureAction.IdleOrObserve,GetComponent<HybridTarget>().id,need.ToString());motion?.React();}return Active;}
  }
  if(clock>16){ResetTask();return false;}
  if(placeRequest==PlaceImprovement.Cover){bool overhead=clock%6<3;motion?.LookAt(god.transform);motion?.Shelter(overhead);motion?.Signal(false,!overhead,site.transform.position);}
  else {if(requestedBall==null){ResetTask();return false;}float cycle=clock%8;motion?.LookAt(cycle<5?requestedBall.transform:site.transform);motion?.Signal(cycle<2,cycle>=2,cycle<5?requestedBall.transform.position:site.transform.position);}
  return true;
 }
 // A handoff is not a completed benefit. Keep only this live activity's pending receipt.
 void CompleteRequestedPlaceUse(ConstructionSite site,PlaceImprovement improvement,BeachBall ball=null){
  var god=construction.GetComponent<CreatureHybrid>();if(acceptedPlace!=site||acceptedImprovement!=improvement||!ReferenceEquals(acceptedSeason,god.Season)||god.Season.paused||actor.held||improvement==PlaceImprovement.BeachBall&&acceptedBall!=ball)return;
  var memory=social.Memory.Place(SiteSlot(site));if(memory!=null&&memory.ConfirmRequestedUse(improvement))god.Journal.Add(god.State.time,"requested improvement used",CreatureAction.IdleOrObserve,GetComponent<HybridTarget>().id,improvement==PlaceImprovement.Cover?"Completed rest under requested cover":"Completed shared play with requested ball");ClearAcceptedPlaceUse();
 }
 void ClearAcceptedPlaceUse(){acceptedPlace=null;acceptedImprovement=PlaceImprovement.None;acceptedBall=null;acceptedSeason=null;}
 void ReleasePlaceRequest(){if(placeRequest==PlaceImprovement.None)return;placeRequest=PlaceImprovement.None;requestedBall=null;requestShown=false;motion?.Shelter(false);motion?.Signal(false);}
}
}
