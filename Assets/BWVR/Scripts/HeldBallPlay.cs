using UnityEngine;
namespace BWVR {
// One held-ball activity: the established chase service and curiosity motor.
public sealed partial class CreatureHybrid {
 public bool FollowingHeldBall=>Owner==ActionOwner.Curiosity&&curiosity.Current==CreatureCuriosity.Phase.ChaseBall;
 internal void FinishHeldBallRelease(BeachBall ball){
  if(!FollowingHeldBall||!curiosity.Chasing(ball))return;
  if(Season.paused||suspended){ball.Chase.End();return;}
  curiosity.FinishReleasedChase(ball);nextDecision=State.time;nextPerception=0;
 }
}
public sealed partial class CreatureCuriosity {
 internal bool Chasing(BeachBall ball)=>Current==Phase.ChaseBall&&target!=null&&target.GetComponent<BeachBall>()==ball;
 internal void FinishReleasedChase(BeachBall ball){if(Chasing(ball))ResolveChase(ball.Chase.Step(navigator,routine.speed,0));}
 bool ResolveChase(int outcome){
  if(outcome==0)return true;
  if(outcome==-2){GetComponent<CreatureHybrid>().Interrupt("Held-ball invitation ended");return false;}
  if(outcome>0){played++;lastWater=lastSocial=false;boredom=0;feedbackWindow=12;motion?.React();}
  Finish(outcome>0);return false;
 }
}
}
