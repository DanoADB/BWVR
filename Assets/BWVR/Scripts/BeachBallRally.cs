using System;
using UnityEngine;
namespace BWVR {
// A bounded invitation/turn-taking state on the existing ball, not another movement owner.
public sealed partial class BeachBall {
 public const string RallyInterest="gentle-ball-rally";
 public VillagerExploration Player{get;private set;}
 public bool CreatureJoined{get;private set;}
 public bool CreatureTurn{get;private set;}
 public bool PassPending{get;private set;}
 public int RallyReceptions{get;private set;}
 public bool Inviting=>!PeerGame&&Player!=null&&!Harmful&&!item.held;
 public CreatureHybrid Playmate=>creature;
 float rallyClock,passClock,nextMeeting; Vector3 passOrigin,soloGoal,meeting;bool passFromCreature;
 public bool ReservePlay(VillagerExploration villager){
  if(BuildBowlPending||BowlCreaturePending||Puzzle?.Active==true||Goals?.Turns.Active==true||FetchReserved||creature==null||Player!=null||villager==null||item.held||InFlight||Body.linearVelocity.magnitude>8||creature.Season.paused)return false;
  FinishExperience();thrown=false;Harmful=false;ConfirmedPartner="";ObservationSession=ObservationSession==int.MaxValue?1:ObservationSession+1;Player=villager;soloGoal=creature.transform.position;
  float apart=Flat(soloGoal-transform.position).magnitude/nav.Scale;
  if(apart<10||apart>40||!ClearLane(soloGoal)){bool found=false;for(int i=0;i<8;i++){var wanted=transform.position+Quaternion.Euler(0,i*45,0)*Vector3.forward*nav.Scale*16;if(nav.Sample(wanted,nav.villager,2,out var at)&&Mathf.Abs(at.y-transform.position.y+nav.Scale*Radius)<nav.Scale*3&&ClearLane(at)){soloGoal=at;found=true;break;}}if(!found){Player=null;return false;}}
  CreatureJoined=CreatureTurn=PassPending=false;RallyReceptions=0;rallyClock=0;nextMeeting=0;meeting=default;return true;
 }
 public void CancelRally(){PeerPlayer=null;PeerReady=false;Player=null;CreatureJoined=CreatureTurn=PassPending=false;rallyClock=passClock=0;}
 public bool MeetingPoint(out Vector3 point){
  point=meeting;if(creature.State.time<nextMeeting)return meeting!=default;
  nextMeeting=creature.State.time+1;meeting=default;float best=float.MaxValue;float groundY=nav.ProjectFoot(transform.position,nav.creature).y;
  for(int i=0;i<16;i++){var wanted=transform.position+Quaternion.Euler(0,i*22.5f,0)*Vector3.forward*nav.Scale*20;
   if(!nav.Sample(wanted,nav.creature,2,out var sample))continue;var at=nav.ProjectFoot(sample,nav.creature);
   if(Mathf.Abs(at.y-groundY)>nav.Scale*1.1f||!ClearLane(at))continue;
   float distance=(at-creature.transform.position).sqrMagnitude;if(distance>=best||!creature.CanReach(at))continue;best=distance;meeting=at;
  }point=meeting;return meeting!=default;
 }
 public bool TryJoinCreature(){
  if(creature.Season.paused||!Inviting||CreatureJoined||Player.GetComponent<CarryItem>().held||!Player.GetComponent<VillagerSocial>().Willing||Vector3.Distance(creature.transform.position,transform.position)>nav.Scale*28||Vector3.Distance(Player.transform.position,transform.position)>nav.Scale*16)return false;
  FinishExperience();PassPending=false;CreatureJoined=true;CreatureTurn=false;rallyClock=0;if(creature.State.intention.active&&creature.State.intention.target==StableId)creature.State.intention.partner=Player.GetComponent<HybridTarget>().id;
  OfferSpectators();creature.Journal.Add(creature.State.time,"ball invitation accepted",CreatureAction.PlayWithToy,StableId,"Try gentle returns with a villager");return true;
 }
 Vector3 Receiver(bool fromCreature)=>fromCreature?Player.transform.position:PeerGame?(PeerReady?PeerPlayer.transform.position:peerMeeting):CreatureJoined?creature.transform.position:soloGoal;
 static Vector3 Flat(Vector3 delta){delta.y=0;return delta;}
 bool ClearLane(Vector3 end){
  var start=nav.ToNav(transform.position);var finish=nav.ToNav(end);var line=Flat(finish-start);float length=line.magnitude;if(length<8)return false;var direction=line/length;
  foreach(var target in targets){if(target==null||(target.villager?.GetComponent<VillagerExploration>()==Player||PeerGame&&target.villager?.GetComponent<VillagerExploration>()==PeerPlayer))continue;var bounds=target.Bounds(nav.Scale);var at=nav.ToNav(bounds.center);float along=Vector3.Dot(Flat(at-start),direction);if(along<0||along>length)continue;float side=Flat(at-start-direction*along).magnitude;float width=Mathf.Max(bounds.extents.x,bounds.extents.z)/nav.Scale;if(side<Radius+width+.5f)return false;}
  return true;
 }
 // Both actors use their existing native navigator to get behind the ball before a tap.
 public bool ApproachPass(IslandNavigator walker,float speed,float dt,bool fromCreature){
  if(creature.Season.paused||!(Inviting||PeerGame)||PassPending||CreatureTurn!=fromCreature||fromCreature&&!HasPartner)return false;
  var receiver=Receiver(fromCreature);var direction=Flat(receiver-transform.position);if(direction.sqrMagnitude<nav.Scale*nav.Scale*64)return false;
  var behind=transform.position-direction.normalized*(Radius+1.2f)*nav.Scale;
  if(!nav.Sample(behind,walker.profile,1.5f,out var safe))return false;
  walker.MoveTo(safe,speed,dt);
  return Flat(walker.transform.position-transform.position).magnitude<=nav.Scale*(Radius+2)&&Vector3.Dot(Flat(walker.transform.position-transform.position),direction.normalized)<-nav.Scale;
 }
 public bool TryPass(bool fromCreature){
  if(creature.Season.paused||!(Inviting||PeerGame)||PassPending||CreatureTurn!=fromCreature||fromCreature&&!HasPartner||Body.linearVelocity.magnitude>8||Harmful||fromCreature&&!PeerGame&&creature.State.time<creature.Season.WorldPlay.nextKick)return false;
  if(PeerGame&&!PeerReady)return false;var sender=fromCreature?(PeerGame?PeerPlayer.transform:creature.transform):Player.transform;var receiver=Receiver(fromCreature);float distance=Flat(receiver-transform.position).magnitude/nav.Scale;
  if(Flat(sender.position-transform.position).magnitude>nav.Scale*(Radius+2)||distance<10||distance>45||!ClearLane(receiver))return false;
  var direction=Flat(receiver-transform.position).normalized;if(fromCreature&&!PeerGame)direction=creature.Season.WorldPlay.GentleBall.Aim(direction,creature.State.seed,creature.Season.WorldPlay.kicks,StableId);FinishExperience();Throw(direction,fromCreature&&!PeerGame,Mathf.Clamp(distance*.35f,4,6.5f));kicking=true;kickReward=.006f;
  Body.linearVelocity=direction*Mathf.Clamp(distance*.35f,4,6.5f)+Vector3.up*.2f;
  if(PeerGame)lastPeerKicker=fromCreature?PeerPlayer:Player;
  if(fromCreature&&!PeerGame){var m=creature.Season.WorldPlay;m.kicks=Math.Min(1000000,m.kicks+1);m.nextKick=creature.State.time+1.2f;}
  else creature.Season.WorldPlay.villagerKicks=Math.Min(1000000,creature.Season.WorldPlay.villagerKicks+1);
  if(RallyReceptions==0)OfferSpectators();PassPending=true;passFromCreature=fromCreature;passClock=0;passOrigin=Body.position;
  creature.Journal.Add(creature.State.time,PeerGame?"villager peer pass":fromCreature?"creature gentle pass":"villager ball nudge",PeerGame?CreatureAction.IdleOrObserve:CreatureAction.PlayWithToy,StableId,"Physical rolling ball, not a carried prop");return true;
 }
 void AdvanceRally(float dt){
  if(Player==null)return;rallyClock+=dt;
  if(item.held||Player.GetComponent<CarryItem>().held||(PeerGame?!PeerHealthy:!Player.GetComponent<VillagerSocial>().Willing)||Harmful||rallyClock>(HasPartner&&(!PeerGame||PeerReady)?50:90)){CancelRally();return;}
  if(!PassPending)return;passClock+=dt;if(passClock>10){CancelRally();return;}
  // Solo play remains an invitation; it creates no fictional social reward.
  if(!HasPartner){if(passClock>2){PassPending=false;CreatureTurn=false;}return;}
  var receiver=Receiver(passFromCreature);float moved=Flat(Body.position-passOrigin).magnitude;
  if(moved<4||Flat(receiver-transform.position).magnitude>nav.Scale*(Radius+3)||Mathf.Abs(receiver.y-transform.position.y)>nav.Scale*(Radius+3)||Body.linearVelocity.magnitude>8)return;
  FinishExperience();Body.linearVelocity=Vector3.zero;Body.angularVelocity*=.2f;PassPending=false;CreatureTurn=!passFromCreature;RallyReceptions++;if(RallyReceptions==4){NotifyObservers(true);PracticeParticipants();Player.WitnessBallPlay(this);if(PeerGame)PeerPlayer.WitnessBallPlay(this);}if(RallyReceptions>=4&&!PeerGame)ConfirmedPartner=Player.GetComponent<HybridTarget>().id;
  if(PeerGame){CreditPeerReturn();return;}
  creature.GetComponent<CreatureCuriosity>().NoticeGreeting(Player.transform);Player.GetComponent<HumanoidMotion>()?.React();
  if(passFromCreature)creature.Season.WorldPlay.GentleBall.Practice(creature.State.time);
  if(RallyReceptions>=2){
   var m=creature.Season.WorldPlay;m.rallyReturns=Math.Min(1000000,m.rallyReturns+1);m.rallyFun=Mathf.Clamp01(m.rallyFun+.06f);creature.State.Experience(RallyInterest,.04f);creature.State.Experience(StableId,.015f);
   var social=Player.GetComponent<VillagerSocial>().Memory;social.ballReturns=Math.Min(1000000,social.ballReturns+1);
   if(RallyReceptions<=4){social.trust=Mathf.Clamp01(social.trust+.008f+creature.Season.WorldPlay.GentleBall.control*.004f);social.affinity=Mathf.Clamp01(social.affinity+.012f);}
   if(RallyReceptions==2){social.ballGames=Math.Min(1000000,social.ballGames+1);creature.State.RememberBond(Player.GetComponent<HybridTarget>().id,true);}
   creature.Journal.Add(creature.State.time,"gentle rally is fun",CreatureAction.PlayWithToy,StableId,"Confirmed alternating return with "+Player.GetComponent<HybridTarget>().id);
  }
 }
}
}
