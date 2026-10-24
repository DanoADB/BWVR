using UnityEngine;
namespace BWVR {
public sealed partial class VillagerExploration {
 BeachBall[] beachballs=System.Array.Empty<BeachBall>();BeachBall playingBall;int soloNudges;
 public void ConfigureBalls(BeachBall[] balls){beachballs=balls;}
 bool ChooseBall(){foreach(var ball in beachballs)if(ball!=null&&Vector3.Distance(transform.position,ball.transform.position)<nav.world.Scale*120&&TryBall(ball))return true;return false;}
 public bool TryBall(BeachBall ball){
  if(!CanStartStructure||!social.Willing||ball==null||!nav.world.Sample(ball.transform.position,nav.profile,ball.Radius+2,out var foot)||!world.Reachable(transform.position,foot,nav.profile)||!ball.ReservePlay(this))return false;
  playingBall=ball;soloNudges=0;routine.ResetTask();nav.ResetRoute();clock=elapsed=0;Current=Phase.Beachball;motion?.LookAt(ball.transform);return true;
 }
 bool TickBall(float dt){
  elapsed+=dt;clock+=dt;var ball=playingBall;
  if(ball==null||ball.Player!=this||!social.Willing||elapsed>150){ResetTask();return false;}
  motion?.LookAt(ball.transform);
  if(!ball.CreatureTurn&&!ball.PassPending&&(ball.CreatureJoined||soloNudges==0)){motion?.Signal(false);if(ball.ApproachPass(nav,transform.lossyScale.y*1.6f,dt,false)&&clock>1){if((ball.CreatureJoined||!ball.Playmate.GetComponent<CreatureCuriosity>().HandlingBeachBall)&&ball.TryPass(false)){if(!ball.CreatureJoined)soloNudges++;clock=0;motion?.Activity(5);}else motion?.Signal(!ball.CreatureJoined,true,ball.Playmate.transform.position);}}
  else {motion?.Activity(0);motion?.Signal(!ball.CreatureJoined,true,ball.Playmate.transform.position);}
  if(nav.Blocked&&elapsed>8){ResetTask();return false;}return true;
 }
 void ReleaseBall(){if(playingBall!=null&&playingBall.Player==this)playingBall.CancelRally();playingBall=null;motion?.Signal(false);}
}
}
