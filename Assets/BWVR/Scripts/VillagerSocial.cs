using System;
using UnityEngine;
namespace BWVR {
[Serializable] public sealed partial class SocialMemory {
 public float trust=.45f,affinity=.5f,boldness=.5f;
 public float injury,lastAssistance,lastFrightAt=-100;
 public int greetings,rides,witnessed,disputes,discoveries,bridgeCrossings,sharedPlay,treatsOffered,missedInvitations,frights;
 public float nextInvitationAt,lastSharedAt;
 public int ballReturns,ballGames;
 public float restNeed=.5f,nextBuildingRequest;public int usedForms,constructionUses,buildRequests,buildDisappointments;
 public bool Valid()=>FiresValid()&&CultureValid()&&BuildThanksValid()&&TricksValid()&&PlacesValid()&&MusicValid()&&ReligionValid()&&FaithValid()&&ShelterValid()&&InvitationsValid()&&ComfortValid()&&GatheringsValid()&&VillageBuildingValid()&&RepairsValid()&&(ballLearning==null||ballLearning.Valid())&&PeersValid()&&HybridState.Number(lastFrightAt,-100,100000000)&&Count(ballReturns)&&Count(ballGames)&&Unit(trust)&&Unit(affinity)&&Unit(boldness)&&Unit(injury)&&Unit(restNeed)&&HybridState.Number(nextBuildingRequest,0,100000300)&&usedForms>=0&&usedForms<=15&&Count(constructionUses)&&Count(buildRequests)&&Count(buildDisappointments)&&HybridState.Number(lastAssistance,0,100000000)&&HybridState.Number(nextInvitationAt,0,100000300)&&HybridState.Number(lastSharedAt,0,100000000)&&Count(greetings)&&Count(rides)&&Count(witnessed)&&Count(disputes)&&Count(discoveries)&&Count(bridgeCrossings)&&Count(sharedPlay)&&Count(treatsOffered)&&Count(missedInvitations)&&Count(frights);
 static bool Unit(float n)=>!float.IsNaN(n)&&!float.IsInfinity(n)&&n>=0&&n<=1;
 static bool Count(int n)=>n>=0&&n<=1000000;
}
// Social attention competes with other intentions; memories belong to the saved season.
public sealed partial class VillagerSocial:MonoBehaviour {
 public enum Phase{Idle,Approach,Greet,Wait,FetchGift,CarryGift,PresentGift,WatchGift,Recoil}
 public Phase Current{get;private set;}public bool Active=>Current!=Phase.Idle;
 public SocialMemory Memory=>season().Social(index);
 public bool RecentFright=>Memory.frights>0&&season().Hybrid.time-Memory.lastFrightAt<90;
 public bool Willing=>!routine.BuildingCathedral&&!routine.Worshipping&&GetComponentInParent<QuestSlice>()?.GetComponentInChildren<VillageFamilies>()?.Owns(GetComponent<VillagerRoutine>())!=true&&GetComponent<VillagerResourceGathering>()?.Active!=true&&Memory.trust>=.4f&&Memory.injury<=0&&!RecentFright;
 public float Attraction=>Memory.affinity*.65f+Memory.trust*.35f;
 CreatureCuriosity creature;IslandNavigator walker;HumanoidMotion motion;CarryItem item;VillagerRoutine routine;
 Func<SliceState> season;int index;float cooldown,clock,elapsed;
 CreatureHybrid hybrid;CreatureAffordance[] sharingObjects;Vector3 giftOrigin,meeting,offeringPoint;Quaternion giftRotation;bool ownsGift;
 public CreatureAffordance Offering{get;private set;}public bool Ready=>Current==Phase.PresentGift;public bool Sharing=>Current>=Phase.FetchGift&&Current<=Phase.WatchGift;
 Transform frightening;Vector3 retreat;
 public void ReactToBall(Transform ball,bool blameCreature){if(item.held)return;IslandAudio.Cue(IslandSound.Uneasy,transform.position,GetEntityId().GetHashCode(),.75f,1.25f);GetComponent<VillagerResourceGathering>()?.Cancel();ResetTask();GetComponent<VillagerExploration>()?.ResetTask();GetComponent<VillagerBlockPlay>()?.ResetTask();routine.ResetTask();if(blameCreature){Memory.trust=Mathf.Clamp01(Memory.trust-.18f);Memory.affinity=Mathf.Clamp01(Memory.affinity-.06f);Memory.frights=Math.Min(1000000,Memory.frights+1);Memory.lastFrightAt=season().Hybrid.time;hybrid?.State.RememberBond(GetComponent<HybridTarget>().id,false);}frightening=ball;var away=transform.position-ball.position;away.y=0;if(away.sqrMagnitude<1e-10f)away=transform.right;retreat=transform.position+away.normalized*walker.world.Scale*5;walker.ResetRoute();Current=Phase.Recoil;clock=elapsed=0;motion?.Signal(false,true,ball.position,-20);motion?.LookAt(ball);}
 public void ConfigureSharing(CreatureHybrid god,CreatureAffordance[] objects){hybrid=god;sharingObjects=objects;}
 bool Shareable(CreatureAffordance prop){if(prop==null||prop.host!=null||!prop.Available||prop.GetComponent<ConstructionBlock>()!=null||prop.GetComponent<BeachBall>()!=null||prop.kind==CreatureAffordance.Kind.Villager)return false;return prop.kind!=CreatureAffordance.Kind.Food||prop.GetComponent<DiscoveryObject>()?.treat==true;}
 public bool TryShare(CreatureAffordance prop){
  if(hybrid==null||season().paused||Active||item.held||!Willing||(!season().developmentFocus&&!season().Finished&&season().water<45)||Memory.nextInvitationAt>season().Hybrid.time||hybrid.Owner!=CreatureHybrid.ActionOwner.None||creature.Current!=CreatureCuriosity.Phase.Rest||(GetComponent<VillagerExploration>()?.Active??false)||(GetComponent<VillagerBlockPlay>()?.Active??false)||!Shareable(prop))return false;
  if(Vector3.Distance(transform.position,prop.transform.position)>walker.world.Scale*120||!hybrid.CanReach(prop.GetComponent<HybridTarget>())||!walker.world.Sample(prop.transform.position,walker.profile,8,out _))return false;
  ConstructionSite cover=null;var place=KnownMusicPlace(prop);if(place!=null){if(place.Paused||place.Locked||(!CoveredMusicMeeting(prop,out meeting,out cover)&&!MusicMeeting(prop,out meeting)))return false;}else{
  var delta=transform.position-creature.transform.position;delta.y=0;if(delta.sqrMagnitude<1e-10f)delta=-creature.transform.forward;
  if(!walker.world.Sample(creature.transform.position+delta.normalized*walker.world.Scale*7,walker.profile,8,out meeting)||!walker.world.Sample(creature.transform.position+delta.normalized*walker.world.Scale*4,walker.world.creature,8,out offeringPoint))return false;}
  musicPlace=place;musicAnchor=prop.transform.position;ReservePerformerCover(cover);
  Offering=prop;prop.host=this;prop.GetComponent<SharedRhythm>()?.ResetOffer();giftOrigin=prop.transform.position;giftRotation=prop.transform.rotation;routine.ResetTask();walker.ResetRoute();clock=elapsed=0;Current=Phase.FetchGift;motion?.LookAt(prop.transform);return true;
 }
 bool ChooseShare(){if(sharingObjects==null||hybrid==null||Memory.nextInvitationAt>season().Hybrid.time)return false;CreatureAffordance best=null;float score=float.NegativeInfinity;foreach(var prop in sharingObjects){if(!Shareable(prop))continue;float distance=Vector3.Distance(transform.position,prop.transform.position)/walker.world.Scale;float value=(prop.kind==CreatureAffordance.Kind.Food?season().hunger*2:.8f)-distance*.015f;if(value>score){score=value;best=prop;}}return TryShare(best);}
 public void Initialize(CreatureCuriosity god,int identity,Func<SliceState> current){creature=god;index=identity;season=current;walker=GetComponent<IslandNavigator>();motion=GetComponent<HumanoidMotion>();item=GetComponent<CarryItem>();routine=GetComponent<VillagerRoutine>();cooldown=18+identity*13;}
 public bool TryBegin(){
  if(Active||item.held||RecentFright||creature.Current!=CreatureCuriosity.Phase.Rest||(GetComponent<VillagerExploration>()?.Active??false)||(GetComponent<VillagerBlockPlay>()?.Active??false))return false;
  var delta=transform.position-creature.transform.position;delta.y=0;
  if(delta.sqrMagnitude<1e-10f)delta=-creature.transform.forward;
  if(!walker.world.Sample(creature.transform.position+delta.normalized*walker.world.Scale*5,walker.profile,8,out _))return false;
  routine.ResetTask();walker.ResetRoute();Current=Phase.Approach;clock=elapsed=0;motion?.LookAt(creature.transform);return true;
 }
 public bool Tick(float dt,bool paused){
  if(paused||dt<=0)return Active;
  if(item.held){ResetTask();return false;}
  if(Current==Phase.Recoil){clock+=dt;walker.MoveTo(retreat,transform.lossyScale.y*2,dt);if(frightening!=null)motion?.Signal(false,true,frightening.position,-20);if(clock>3)ResetTask();return Active;}
  if(!Active){cooldown-=dt;if(cooldown<=0){if(ChooseShare())return true;if(!TryBegin())cooldown=6;}return Active;}
  if(Sharing)return TickSharing(dt);
  clock+=dt;elapsed+=dt;
  if(elapsed>22||creature.Current!=CreatureCuriosity.Phase.Rest){ResetTask();return false;}
  var delta=transform.position-creature.transform.position;delta.y=0;
  if(Current==Phase.Approach){
   if(delta.sqrMagnitude<1e-10f)delta=-creature.transform.forward;
   if(walker.MoveTo(creature.transform.position+delta.normalized*walker.world.Scale*5,transform.lossyScale.y*1.6f,dt)){Current=Phase.Greet;clock=0;motion?.Signal(true);}
   if(walker.Blocked&&elapsed>5)ResetTask();
  }else if(Current==Phase.Greet){
   if(delta.magnitude>walker.world.Scale*9){ResetTask();return false;}
   Face();if(clock>=2){RememberGreeting();creature.NoticeGreeting(transform);motion?.React();motion?.Signal(false);Current=Phase.Wait;clock=0;}
  }else if(Current==Phase.Wait){
   Face();if(clock>=1.5f){var invite=GetComponent<CreatureAffordance>();bool willing=Willing;ResetTask();if(willing)creature.Offer(invite);}
  }
  return Active;
 }
 bool TickSharing(float dt){
  clock+=dt;elapsed+=dt;
  if(Offering==null||Offering.host!=this||elapsed>90){ResetTask();return false;}
  CheckPerformerCover(dt);var prop=Offering;var carried=prop.Item;
  if(musicPlace!=null&&(musicPlace.Paused||musicPlace.Locked||carried.held||Vector3.Distance(prop.transform.position,musicAnchor)>walker.world.Scale*.5f)){ResetTask();return false;}
  if(Current==Phase.FetchGift&&musicPlace!=null){
   if(walker.MoveTo(meeting,transform.lossyScale.y*1.6f,dt)){PresentOffering(prop);}else if(walker.Blocked&&elapsed>5)ResetTask();return Active;
  }
  if(Current==Phase.FetchGift){
   if(carried.held||prop.GetComponent<DiscoveryObject>()?.Owner!=null){ResetTask();return false;}
   if(walker.MoveTo(prop.transform.position,transform.lossyScale.y*1.6f,dt)){ownsGift=true;carried.held=true;prop.GetComponent<Collider>().enabled=false;motion?.Carry(prop.transform);motion?.Activity(0);Current=Phase.CarryGift;clock=0;walker.ResetRoute();}
   else if(walker.Blocked&&elapsed>5)ResetTask();
  }else if(Current==Phase.CarryGift){
   if(!carried.held||Vector3.Distance(creature.transform.position,meeting)>walker.world.Scale*24){ResetTask();return false;}
   if(walker.MoveTo(meeting,transform.lossyScale.y*1.6f,dt)){motion?.Carry(null);motion?.Activity(5);if(prop.GetComponent<SharedRhythm>()!=null){var toward=creature.transform.position-meeting;toward.y=0;if(toward.sqrMagnitude<1e-10f)toward=-transform.forward;offeringPoint=meeting+toward.normalized*walker.world.Scale;giftRotation=Quaternion.LookRotation(toward.normalized);}offeringPoint=walker.world.ProjectFoot(offeringPoint,walker.world.creature)+Vector3.up*walker.world.Scale*(prop.GetComponent<SharedRhythm>()!=null?.3f:.7f);prop.transform.SetPositionAndRotation(offeringPoint,giftRotation);carried.held=false;ownsGift=false;prop.GetComponent<Collider>().enabled=true;PresentOffering(prop);}
   else if(walker.Blocked&&elapsed>5)ResetTask();
  }else if(Current==Phase.PresentGift){
   Face();prop.GetComponent<SharedRhythm>()?.Offer(this,hybrid,dt);if(carried.held){ResetTask();return false;}
   if(clock>18){Memory.missedInvitations=Math.Min(1000000,Memory.missedInvitations+1);Memory.nextInvitationAt=season().Hybrid.time+100;ResetTask();}
  }else if(Current==Phase.WatchGift){
   Face();if(clock>35){ResetTask();return false;}
  }
  return Active;
 }
 public bool Accept(CreatureAffordance prop){if(prop!=Offering||!Ready)return false;Current=Phase.WatchGift;clock=0;motion?.Signal(false);motion?.Activity(6);return true;}
 public void SharedOutcome(CreatureAffordance prop,bool success){if(prop!=Offering||Current!=Phase.WatchGift)return;if(success){var memory=Memory;if(prop.GetComponent<SharedRhythm>()?.Completed==true){RememberPlacePerformance();memory.rhythmGames=Math.Min(1000000,memory.rhythmGames+1);hybrid.Season.WorldPlay.rhythmGames=Math.Min(1000000,hybrid.Season.WorldPlay.rhythmGames+1);}if(prop.kind==CreatureAffordance.Kind.Food)memory.treatsOffered=Math.Min(1000000,memory.treatsOffered+1);else memory.sharedPlay=Math.Min(1000000,memory.sharedPlay+1);memory.trust=Mathf.Clamp01(memory.trust+.06f);memory.affinity=Mathf.Clamp01(memory.affinity+.04f);memory.lastSharedAt=season().Hybrid.time;hybrid.State.RememberBond(GetComponent<HybridTarget>().id,true);memory.nextInvitationAt=season().Hybrid.time+Mathf.Lerp(70,40,memory.boldness);hybrid.Journal.Add(season().Hybrid.time,"shared experience",prop.kind==CreatureAffordance.Kind.Food?CreatureAction.EatFood:CreatureAction.PlayWithToy,GetComponent<HybridTarget>().id,"Completed together");}ResetTask();if(success){IslandAudio.Cue(IslandSound.Content,transform.position,GetEntityId().GetHashCode(),.7f,1.25f);motion?.React();creature.NoticeGreeting(transform);}}
 void Face(){var at=creature.transform.position-transform.position;at.y=0;if(at.sqrMagnitude>1e-10f)transform.rotation=Quaternion.LookRotation(at);}
 public void RememberGreeting(){hybrid?.State.RememberBond(GetComponent<HybridTarget>().id,true);var m=Memory;m.greetings=Math.Min(1000000,m.greetings+1);m.trust=Mathf.Clamp01(m.trust+.08f);m.affinity=Mathf.Clamp01(m.affinity+.025f);}
 public void RememberRide(){hybrid?.State.RememberBond(GetComponent<HybridTarget>().id,true);var m=Memory;m.rides=Math.Min(1000000,m.rides+1);m.trust=Mathf.Clamp01(m.trust+.12f);m.affinity=Mathf.Clamp01(m.affinity+.08f);}
 public void RememberDispute(){hybrid?.State.RememberBond(GetComponent<HybridTarget>().id,false);var m=Memory;m.disputes=Math.Min(1000000,m.disputes+1);m.affinity=Mathf.Clamp01(m.affinity-.12f);}
 public void WitnessRide(){var m=Memory;m.witnessed=Math.Min(1000000,m.witnessed+1);m.trust=Mathf.Clamp01(m.trust+.03f);}
 public void Feedback(bool positive){Memory.affinity=Mathf.Clamp01(Memory.affinity+(positive?.08f:-.08f));}
 public void ResetTask(){ReleasePerformerCover();if(!Active)return;if(Offering!=null){var prop=Offering;prop.GetComponent<SharedRhythm>()?.Cancel();if(ownsGift){prop.Item.held=false;prop.transform.SetPositionAndRotation(giftOrigin,giftRotation);prop.GetComponent<Collider>().enabled=true;motion?.Carry(null);}if(prop.host==this)prop.host=null;prop.GetComponent<DiscoveryObject>()?.Refresh();Memory.nextInvitationAt=Mathf.Max(Memory.nextInvitationAt,season().Hybrid.time+30);}Offering=null;musicPlace=null;ownsGift=false;Current=Phase.Idle;clock=elapsed=0;cooldown=Mathf.Lerp(65,25,Memory.boldness)*Mathf.Lerp(1.5f,.75f,Memory.trust);motion?.Signal(false);motion?.LookAt(null);motion?.Activity(0);walker?.ResetRoute();routine?.ResetTask();}
 void OnDisable(){ResetTask();}
}
}
