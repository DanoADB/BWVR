using UnityEngine;
namespace BWVR {
public sealed partial class VillagerExploration {
 BeachBall[] beachballs=System.Array.Empty<BeachBall>();BeachBall playingBall;int soloNudges;
 public bool CanReceivePlayInvite(BeachBall ball)=>ball!=null&&CanStartStructure&&social.Willing&&!ball.Playmate.Season.paused&&(ball.Playmate.Season.developmentFocus||ball.Playmate.Season.Finished||(ball.Playmate.Season.water>=45&&ball.Playmate.Season.crops>=45));
 public bool TryCreaturePlayInvite(BeachBall ball){if(!CanReceivePlayInvite(ball)||!TryBall(ball))return false;motion?.React();motion?.Signal(true,true,ball.transform.position);return true;}
 public void ConfigureBalls(BeachBall[] balls){beachballs=balls;}
 bool ChooseBall(){foreach(var ball in beachballs)if(ball!=null&&Vector3.Distance(transform.position,ball.transform.position)<nav.world.Scale*120&&TryBall(ball))return true;return false;}
 public bool TryBall(BeachBall ball){
  if(!CanStartStructure||!social.Willing||ball==null||!nav.world.Sample(ball.transform.position,nav.profile,ball.Radius+2,out var foot)||!world.Reachable(transform.position,foot,nav.profile)||!ball.ReservePlay(this))return false;
  playingBall=ball;soloNudges=0;routine.ResetTask();nav.ResetRoute();clock=elapsed=0;Current=Phase.Beachball;motion?.LookAt(ball.transform);return true;
 }
 bool TickBall(float dt){
  elapsed+=dt;clock+=dt;var ball=playingBall;
  if(ball==null||!ball.OwnsPlayer(this)||(ball.PeerGame?social.Memory.injury>0:!social.Willing)||elapsed>150){ResetTask();return false;}
  motion?.LookAt(ball.transform);
  bool peer=ball.PeerGame,side=peer&&ball.PeerPlayer==this;if(peer){if(ball.RallyReceptions>=4){ResetTask();return false;}if(!ball.PeerReady){if(side)ball.PreparePeer(dt);return true;}}
  if(ball.CreatureTurn==side&&!ball.PassPending&&(ball.HasPartner||soloNudges==0)){motion?.Signal(false);if(ball.ApproachPass(nav,transform.lossyScale.y*1.6f,dt,side)&&clock>1){if((ball.HasPartner||!ball.Playmate.GetComponent<CreatureCuriosity>().HandlingBeachBall)&&ball.TryPass(side)){if(!ball.HasPartner)soloNudges++;clock=0;motion?.Activity(5);}else motion?.Signal(!ball.HasPartner,true,peer?(side?ball.Player.transform.position:ball.PeerPlayer.transform.position):ball.Playmate.transform.position);}}
  else {motion?.Activity(0);motion?.Signal(!ball.HasPartner,true,peer?(side?ball.Player.transform.position:ball.PeerPlayer.transform.position):ball.Playmate.transform.position);}
  if(nav.Blocked&&elapsed>8){ResetTask();return false;}return true;
 }
 void ReleaseBall(){if(playingBall!=null&&playingBall.OwnsPlayer(this))playingBall.CancelRally();playingBall=null;motion?.Signal(false);}
}
}
