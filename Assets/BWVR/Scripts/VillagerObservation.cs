using UnityEngine;
namespace BWVR {
public sealed partial class VillagerExploration {
 BeachBall observingBall;int observedSession;float visibleTime,sightRefresh,watchTime;bool inSight,receivedLesson;
 public bool TryWatchBall(BeachBall ball){
  if(ball==null||ball.Playmate.Season.paused)return false;if(Current==Phase.WatchBall&&observingBall==ball&&observedSession==ball.ObservationSession)return true;
  if(!CanStartStructure||!ball.HasPartner||ball.OwnsPlayer(this)||social.Memory.injury>0||ball.Playmate.Season.paused||(!ball.Playmate.Season.developmentFocus&&!ball.Playmate.Season.Finished&&(ball.Playmate.Season.water<45||ball.Playmate.Season.crops<45))||!ball.VisibleTo(this))return false;
  observingBall=ball;observedSession=ball.ObservationSession;visibleTime=watchTime=sightRefresh=0;inSight=true;receivedLesson=false;routine.ResetTask();nav.ResetRoute();Current=Phase.WatchBall;motion?.LookAt(ball.transform);motion?.Activity(6);return true;
 }
 bool ChooseWatching(){foreach(var ball in beachballs)if(TryWatchBall(ball))return true;return false;}
 bool TickWatching(float dt){
  if(observingBall==null||observingBall.ObservationSession!=observedSession||!observingBall.HasPartner||watchTime>30){ResetTask();return false;}
  var look=observingBall.transform.position-transform.position;look.y=0;if(look.sqrMagnitude>1e-10f)transform.rotation=Quaternion.RotateTowards(transform.rotation,Quaternion.LookRotation(look),dt*120);
  watchTime+=dt;sightRefresh-=dt;if(sightRefresh<=0){sightRefresh=.25f;inSight=observingBall.VisibleTo(this);}if(inSight&&FacingBall(observingBall))visibleTime+=dt;motion?.LookAt(observingBall.transform);return true;
 }
 bool FacingBall(BeachBall ball){var look=ball.transform.position-transform.position;look.y=0;return look.sqrMagnitude<1e-10f||Vector3.Dot(transform.forward,look.normalized)>.2588f;}
 public bool WitnessBallOutcome(BeachBall ball,bool safe,string demonstrator){
  if(Current!=Phase.WatchBall||observingBall!=ball||observedSession!=ball.ObservationSession||receivedLesson||visibleTime<1.5f||!FacingBall(ball)||!ball.VisibleTo(this)||ball.Playmate.Season.paused)return false;
  receivedLesson=true;bool learned=social.Memory.BallLesson.Observe(safe,demonstrator,ball.Playmate.State.time);if(learned){if(safe)motion?.React();ball.Playmate.Journal.Add(ball.Playmate.State.time,safe?"villager learns by watching":"villager observes risky play",CreatureAction.IdleOrObserve,GetComponent<HybridTarget>().id,demonstrator);}return learned;
 }
 void ReleaseWatching(){observingBall=null;visibleTime=watchTime=0;receivedLesson=false;}
}
}
