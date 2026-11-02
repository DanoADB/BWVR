using UnityEngine;
namespace BWVR {
// A short investigation on the existing Observe owner. The player keeps the ball.
public sealed partial class CreatureHybrid {
 HybridTarget heldBallCue;SliceState heldBallSeason;float heldBallReported,nextHeldBall,heldBallCheck,heldBallBlocked;Vector3 heldBallGoal;
 bool followingHeldBall;public bool FollowingHeldBall=>followingHeldBall&&Owner==ActionOwner.Observe;
 public void ReportHeldBall(CarryItem item,bool available){
  var target=available&&item!=null&&item.held&&item.GetComponent<BeachBall>()!=null?item.GetComponent<HybridTarget>():null;
  if(heldBallSeason!=Season){heldBallSeason=Season;heldBallCue=null;nextHeldBall=0;}
  if(followingHeldBall&&target!=observing&&!Season.paused&&!suspended){Interrupt("Held-ball invitation ended");nextPerception=0;nextDecision=State.time;}
  if(target!=heldBallCue&&Owner==ActionOwner.None){nextPerception=0;nextDecision=State.time;}
  heldBallCue=target;heldBallReported=State.time;
 }
 bool HeldBallGoal(out Vector3 goal){
  goal=default;if(Season.paused||suspended||heldBallSeason!=Season||heldBallCue==null||State.time-heldBallReported>.35f||GetComponent<CarryItem>().held||heldBallCue.GetComponent<CarryItem>()?.held!=true||Season.hunger>.9f)return false;
  var delta=heldBallCue.transform.position-transform.position;float height=(heldBallCue.transform.position.y-nav.world.ProjectFoot(heldBallCue.transform.position,nav.profile).y)/nav.world.Scale;delta.y=0;
  if(delta.magnitude>nav.world.Scale*40||height<0||height>250||!CanSeeCue(heldBallCue,true,true))return false;
  // Approach dry ground beneath the toy, stopping short of the player's hand.
  if(delta.magnitude<nav.world.Scale*10){goal=transform.position;return true;}
  var wanted=heldBallCue.transform.position-delta.normalized*nav.world.Scale*9;
  return nav.world.Sample(wanted,nav.profile,2,out goal)&&(nav.world.Terrain==null||nav.world.Terrain.Dry(goal))&&CanReach(goal);
 }
 void AddHeldBallPlay(float shortage){
  if(shortage>=1||State.time<nextHeldBall||!HeldBallGoal(out heldBallGoal))return;
  var c=new CreatureCandidate{action=CreatureAction.InvestigateObject,situation=CreatureSituation.PlayOpportunity,target=heldBallCue.id,need=.35f,novelty=.7f,affinity=Season.playPreference*.5f,timeout=9,expected="Watch and follow the player-held ball without taking it"};Add(c,heldBallCue);c.attention=0;c.affinity=Mathf.Min(1,c.affinity+.5f);
 }
 bool ReachCandidate(CreatureCandidate c,HybridTarget target)=>c.action==CreatureAction.InvestigateObject&&target==heldBallCue?HeldBallGoal(out heldBallGoal):c.spin?target.GetComponent<BeachBall>().Spin.Ready:c.chase?CanReach(target.GetComponent<BeachBall>().Chase.Goal):CanReach(target);
 void TickHeldBallPlay(float dt){
  if(observing!=heldBallCue||State.time-State.intention.startedAt>=8||State.time-heldBallReported>.35f||observing?.GetComponent<CarryItem>()?.held!=true){Interrupt("Held-ball invitation ended");return;}
  heldBallCheck-=dt;if(heldBallCheck<=0){heldBallCheck=.25f;if(!HeldBallGoal(out heldBallGoal)){Interrupt("Held ball is no longer a safe visible invitation");return;}}
  motion?.LookAt(observing.transform);
  if(Vector3.Distance(transform.position,heldBallGoal)>nav.world.Scale*.5f){motion?.Activity(0);nav.MoveTo(heldBallGoal,water.speed,dt);heldBallBlocked=nav.Blocked?heldBallBlocked+dt:0;if(heldBallBlocked>.75f){Interrupt("Held-ball route blocked");return;}}
  else {motion?.Activity(6);var toward=observing.transform.position-transform.position;toward.y=0;if(toward.sqrMagnitude>1e-10f)transform.rotation=Quaternion.RotateTowards(transform.rotation,Quaternion.LookRotation(toward),dt*120);}
 }
 void EndHeldBallPlay(){if(!followingHeldBall)return;followingHeldBall=false;heldBallCue=null;nextHeldBall=State.time+12;heldBallCheck=heldBallBlocked=0;}
}
}
