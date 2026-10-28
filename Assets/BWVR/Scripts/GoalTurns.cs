using System;
using UnityEngine;
namespace BWVR {
public sealed class GoalChallengeKey:MonoBehaviour {public GoalTurns game;public int index;}
// A temporary three-round invitation, executed entirely by the existing curiosity/fetch motor.
public sealed class GoalTurns {
 public enum Phase {None,PlayerTurn,PlayerFlight,PrepareCreature,CreatureTurn,CreatureFlight,ReturnToPlayer}
 public Phase Current{get;private set;}
 public bool Active=>Current!=Phase.None;
 public bool ReturnRequested=>Current==Phase.PrepareCreature||Current==Phase.ReturnToPlayer;
 public bool PlayerOwnsTurn=>Current==Phase.PlayerTurn||Current==Phase.PlayerFlight;
 public bool OffersCreature=>ReturnRequested||Current==Phase.CreatureTurn;
 public int Round{get;private set;}public int PlayerHits{get;private set;}public int CreatureHits{get;private set;}
 public bool HasOrigin=>haveOrigin;
 public int GateIndex{get;private set;}=-1;public Vector3 Origin{get;private set;}
 public Func<bool> InputAllowed;
 public string LastReason{get;private set;}="";
 readonly GoalCourse course;readonly BeachBall ball;readonly CreatureHybrid god;readonly IslandNavigation nav;
 float remaining;bool haveOrigin,armed;
 public GoalTurns(GoalCourse course,BeachBall ball,CreatureHybrid god,IslandNavigation nav){this.course=course;this.ball=ball;this.god=god;this.nav=nav;}
 public bool Toggle(int index){
  if(InputAllowed?.Invoke()!=true||god.Season.paused||index<0||index>=course.Gates.Length)return false;
  if(Active&&GateIndex==index){Cancel("Invitation put away");return true;}
  if(ball.GetComponent<CarryItem>().held||god.GetComponent<CreatureCuriosity>().HandlingBeachBall||ball.InFlight||ball.Inviting||ball.PeerGame||course.Active)return false;
  var gate=course.Gates[index];if(!gate.gameObject.activeInHierarchy||gate.GetComponent<CarryItem>().held)return false;
  Cancel("New invitation");GateIndex=index;Round=PlayerHits=CreatureHits=0;haveOrigin=false;ball.CancelFetch();course.Cancel();Enter(Phase.PlayerTurn,90);god.GetComponent<CreatureCuriosity>().NoticeAttention();IslandAudio.Cue(IslandSound.Chime,gate.transform.position,ball.GetEntityId().GetHashCode(),.7f);return true;
 }
 void Enter(Phase phase,float seconds){Current=phase;remaining=seconds;armed=false;}
 public void Advance(float dt){
  if(!Active||dt<=0||god.Season.paused)return;
  var gate=course.Gates[GateIndex];if(!gate.gameObject.activeInHierarchy||gate.GetComponent<CarryItem>().held||god.GetComponent<CarryItem>().held||ball.Harmful){Cancel("Play interrupted");return;}
  remaining-=dt;if(remaining<=0)Cancel("Invitation expired");
 }
 public void GateMoved(int index){if(Active&&GateIndex==index)Cancel("Goal moved");}
 public void PlayerGrab(){if(Active&&Current!=Phase.PlayerTurn)Cancel("Player took the ball out of turn");}
 public void Arm(BallGoal gate){armed=Current==Phase.CreatureTurn&&gate==course.Gates[GateIndex];}
 public void Launch(bool creature,Vector3 start){
  if(!Active)return;
  if(creature){if(Current==Phase.CreatureTurn&&armed){Enter(Phase.CreatureFlight,20);return;}Cancel("Creature chose another activity");return;}
  if(Current!=Phase.PlayerTurn){Cancel("Player threw out of turn");return;}
  var gate=course.Gates[GateIndex];var foot=nav.ProjectFoot(nav.ToWorld(start),nav.creature);var delta=(foot-gate.transform.position)/nav.Scale;delta.y=0;var local=Quaternion.Inverse(gate.transform.rotation)*delta;
  if(delta.magnitude<16||delta.magnitude>55||Mathf.Abs(local.z)<delta.magnitude*.85f||haveOrigin&&Vector3.Distance(foot,Origin)>nav.Scale*14||!god.CanSeeCue(gate.GetComponent<HybridTarget>(),false,true)||!nav.Sample(foot,nav.creature,2,out var safe)){Cancel("Throw was not a witnessed attempt from the play circle");return;}
  if(!haveOrigin){Origin=nav.ProjectFoot(safe,nav.creature);haveOrigin=true;}Enter(Phase.PlayerFlight,20);
 }
 public void Outcome(bool creature,int mask){
  if(!Active)return;
  bool hit=(mask&(1<<GateIndex))!=0;
  if(!creature&&Current==Phase.PlayerFlight){
   if(god.Season.WorldPlay.Goals.observedGoals==0){Cancel("A witnessed goal demonstration is still needed");return;}
   if(hit)PlayerHits|=1<<Round;Enter(Phase.PrepareCreature,100);ball.RequestGoalReturn();
  }else if(creature&&Current==Phase.CreatureFlight){if(hit)CreatureHits|=1<<Round;Enter(Phase.ReturnToPlayer,100);ball.RequestGoalReturn();}
 }
 public void Returned(){
  if(Current==Phase.PrepareCreature){Enter(Phase.CreatureTurn,35);ball.GetComponent<CreatureAffordance>().cooldown=0;god.GetComponent<CreatureCuriosity>().NoticeAttention();}
  else if(Current==Phase.ReturnToPlayer){
   var memory=god.Season.WorldPlay.Goals;memory.sharedRounds=Math.Min(1000000,memory.sharedRounds+1);memory.sharedEnjoyment=Mathf.Clamp01(memory.sharedEnjoyment+.035f);Round++;god.GetComponent<CreatureCuriosity>().NoticeResponse(CreatureAffect.Mood.Happy,2);god.Journal.Add(god.State.time,"shared goal round",CreatureAction.PlayWithToy,ball.StableId,"Both turns and the physical return completed");
   if(Round>=3){Enter(Phase.None,0);LastReason="Three rounds completed";ball.HideGoalCircle();IslandAudio.Cue(IslandSound.Discovery,course.Gates[GateIndex].transform.position,ball.GetEntityId().GetHashCode(),.8f,1.2f);}else{Enter(Phase.PlayerTurn,90);ball.ShowGoalCircle();}
  }
 }
 public bool WaitingFor(BallGoal gate)=>Active&&gate!=null&&gate.index==GateIndex;
 public BallGoal InvitedGoal=>Current==Phase.CreatureTurn?course.Gates[GateIndex]:null;
 public void Cancel(string reason){bool ownedReturn=ReturnRequested;Current=Phase.None;remaining=0;armed=haveOrigin=false;LastReason=reason;if(ownedReturn)ball.CancelFetch();else ball.HideGoalCircle();}
}
}
