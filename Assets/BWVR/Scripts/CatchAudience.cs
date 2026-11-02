using UnityEngine;
namespace BWVR {
public sealed partial class BeachBall {
 bool catchShow,catchShowReturning,preserveCatchAudience;SliceState catchShowSeason;float catchShowStarted;
 public bool CatchShowActive=>catchShow&&catchShowSeason==creature.Season&&gameObject.activeInHierarchy;
 public bool CatchShowRolled=>CatchShowActive&&catchShowReturning;
 public bool HasWatchableGame=>HasPartner||CatchShowActive;
 bool CatchShowCarried=>CatchShowActive&&creature.GetComponent<CreatureCuriosity>().Holding==item;
 void BeginCatchAudience(){
  catchShow=true;catchShowReturning=false;catchShowSeason=creature.Season;catchShowStarted=creature.State.time;ObservationSession=ObservationSession==int.MaxValue?1:ObservationSession+1;OfferSpectators();
 }
 void CatchAudienceRoll(){if(!CatchShowActive)return;catchShowReturning=true;foreach(var resident in villagers)resident.GetComponent<VillagerExploration>().WitnessCatchRoll(this);}
 void FinishCatchAudience(bool safe){
  if(!CatchShowActive||!catchShowReturning)return;
  foreach(var resident in villagers)resident.GetComponent<VillagerExploration>().WitnessBallOutcome(this,safe,"creature");CancelCatchAudience();
 }
 void CancelCatchAudience(){catchShow=catchShowReturning=false;}
 void AdvanceCatchAudience(){
  if(!catchShow)return;if(catchShowSeason!=creature.Season||!gameObject.activeInHierarchy||creature.State.time-catchShowStarted>12||item.held&&!CatchShowCarried)CancelCatchAudience();
 }
}
public sealed partial class VillagerExploration {
 bool catchCheered;float catchCheerUntil;
 public bool CheeringCatch=>Current==Phase.WatchBall&&observingBall!=null&&catchCheerUntil>observingBall.Playmate.State.time;
 public bool WitnessCatchRoll(BeachBall ball){
  if(ball==null||!ball.CatchShowRolled||ball.Playmate.Season.paused||Current!=Phase.WatchBall||observingBall!=ball||observedSession!=ball.ObservationSession||catchCheered||visibleTime<1.5f||!FacingBall(ball)||!ball.VisibleTo(this))return false;
  catchCheered=true;catchCheerUntil=ball.Playmate.State.time+1.2f;motion?.Activity(0);motion?.Clap(.25f);IslandAudio.Cue(IslandSound.Content,transform.position,GetEntityId().GetHashCode(),.25f);return true;
 }
 void TickCatchCheer(BeachBall ball){if(CheeringCatch){motion?.Activity(0);motion?.Clap(.5f+.45f*Mathf.Sin((catchCheerUntil-ball.Playmate.State.time)*15));}else motion?.Clap(0);}
}
}
