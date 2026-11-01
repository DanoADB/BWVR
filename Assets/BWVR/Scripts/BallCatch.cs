using System;
using UnityEngine;
namespace BWVR {
[Serializable] public sealed class CatchMemory {
 public int attempts,catches,rolls;public float control=.15f,preference=.5f;
 public float Control=>attempts>0?control:.15f;public float Willingness=>attempts>0?preference:.5f;
 public bool Valid()=>attempts>=0&&attempts<=1000000&&catches>=0&&catches<=attempts&&rolls>=0&&rolls<=catches&&HybridState.Number(control,0,1)&&HybridState.Number(preference,0,1);
 public void Caught(){catches=Math.Min(attempts,catches+1);control=Mathf.Clamp01(control+(1-control)*.06f);}
}
// Prediction only offers a choice. Capture still requires actual physical proximity.
public static class CatchTrajectory {
 public static bool Intercept(Vector3 point,Vector3 velocity,Vector3 hands,float radius,out float time){time=0;if(!HybridState.Number(radius,.1f,20)||!HybridState.Number(velocity.magnitude,1,12)||Vector3.Dot(velocity,hands-point)<=0)return false;
  for(int i=0;i<=12;i++){float t=i*.1f;var future=point+velocity*t+Vector3.down*4.905f*t*t;if((future-hands).sqrMagnitude<=radius*radius){time=t;return true;}}return false;
 }
}
public sealed partial class BeachBall {
 int catchOfferTicket,catchTicket;SliceState catchSeason;Vector3 catchOrigin;bool caughtBall;
 public bool JustCatchRolled{get;private set;}
 public Vector3 CatchHands=>creature.transform.position+(creature.transform.forward*6+Vector3.up*8)*nav.Scale;
 public Vector3 CatchReleasePoint=>nav.ProjectFoot(creature.transform.position+creature.transform.forward*nav.Scale*7,nav.creature)+Vector3.up*Radius*nav.Scale;
 float CatchRadius=>Radius+Mathf.Lerp(1,2,creature.Season.worldPlay?.catchBall?.Control??.15f);
 bool CatchContext=>catchSeason==creature.Season&&!creature.Season.paused&&gameObject.activeInHierarchy&&!Harmful&&!PeerGame&&Goals?.Turns.Active!=true&&Goals?.Active!=true&&Bowling?.Active!=true&&Puzzle?.Active!=true&&!BuildBowlPending&&!BowlCreaturePending;
 public bool CatchReady=>CatchContext&&catchOfferTicket==FetchTicket&&fetchPending&&!fetchClaimed&&!item.held&&InFlight&&creature.Owner==CreatureHybrid.ActionOwner.None&&creature.CanSeeCue(GetComponent<HybridTarget>(),false,true)&&CatchTrajectory.Intercept(Body.position,Body.linearVelocity,nav.ToNav(CatchHands),CatchRadius,out _);
 void OfferCatch(Vector3 origin){catchSeason=creature.Season;catchOrigin=nav.ProjectFoot(origin,nav.creature);catchOfferTicket=FetchTicket;JustCatchRolled=false;creature.ReconsiderCatch(this);}
 public bool BeginCatch(){if(!CatchReady)return false;catchTicket=FetchTicket;fetchClaimed=true;caughtBall=false;JustCatchRolled=false;catchOfferTicket=0;var memory=creature.Season.WorldPlay.Catch;if(memory.attempts==0){memory.control=.15f;memory.preference=.5f;}memory.attempts=Math.Min(1000000,memory.attempts+1);ShowFetchRing(catchOrigin);return true;}
 public bool OwnsCatch=>CatchContext&&catchTicket!=0&&OwnsFetch(catchTicket);
 public bool CaptureCatch(){
  if(!OwnsCatch||caughtBall||item.held||!InFlight||Body.linearVelocity.magnitude>12||!creature.CanSeeCue(GetComponent<HybridTarget>(),false,true)||(Body.position-nav.ToNav(CatchHands)).sqrMagnitude>CatchRadius*CatchRadius)return false;
  FinishExperience();thrown=false;Body.linearVelocity=Body.angularVelocity=Vector3.zero;Body.isKinematic=true;item.held=true;GetComponent<Collider>().enabled=false;caughtBall=true;creature.Season.WorldPlay.Catch.Caught();IslandAudio.Cue(IslandSound.Ball,transform.position,GetEntityId().GetHashCode(),.4f);return true;
 }
 public bool RollCatchBack(){
  if(!OwnsCatch||!caughtBall||!item.held)return false;var start=creature.transform.position+creature.transform.forward*nav.Scale*7;
  if(!nav.Sample(start,nav.creature,1,out var foot)||(foot-start).sqrMagnitude>nav.Scale*nav.Scale||nav.Terrain!=null&&!nav.Terrain.Dry(foot))return false;
  var direction=catchOrigin-foot;direction.y=0;float distance=direction.magnitude/nav.Scale;if(distance<3||distance>40)return false;
  // Low launch speed and a grounded release. Subsequent collisions remain real.
  PlaceOnGround(foot);Throw(direction,true,Mathf.Clamp(distance*.5f,3,7));Body.linearVelocity=direction.normalized*Mathf.Clamp(distance*.5f,3,7)+Vector3.up;
  JustCatchRolled=true;creature.Season.WorldPlay.Catch.rolls=Math.Min(creature.Season.WorldPlay.Catch.catches,creature.Season.WorldPlay.Catch.rolls+1);return true;
 }
 public void ClearCatchCompletion()=>JustCatchRolled=false;
 public void EndCatch(bool success){if(catchTicket==FetchTicket){if(caughtBall&&!success){CancelFetch(catchTicket);if(creature.GetComponent<CreatureCuriosity>().Holding==item){Body.isKinematic=false;Body.position=nav.ToNav(transform.position);Body.linearVelocity=Body.angularVelocity=Vector3.zero;heldBefore=false;}}else fetchClaimed=false;}catchTicket=0;catchOfferTicket=0;if(!success)JustCatchRolled=false;if(!caughtBall&&fetchPending)ShowFetchRing();caughtBall=false;}
}
public sealed partial class CreatureHybrid {
 internal void ReconsiderCatch(BeachBall ball){if(Owner==ActionOwner.None&&ball.CatchReady){nextDecision=State.time;nextPerception=0;}}
}
}
