using UnityEngine;
namespace BWVR {
public sealed partial class SocialMemory {
 public int ballInvitations,unansweredBallInvitations,rhythmGames;public float nextBallInvitationAt;
 bool InvitationsValid()=>Count(ballInvitations)&&Count(unansweredBallInvitations)&&Count(rhythmGames)&&HybridState.Number(nextBallInvitationAt,0,100000300);
 public bool WantsCreaturePlay(int seed,int serial,string identity,float control){
  float willingness=Mathf.Clamp01(.15f+trust*.35f+affinity*.2f+control*.15f);
  return (CreatureDecisionModel.Variation(seed,serial,CreatureAction.SocialInteraction,identity+"-ball-invitation")+1)*.5f<willingness;
 }
}
}
