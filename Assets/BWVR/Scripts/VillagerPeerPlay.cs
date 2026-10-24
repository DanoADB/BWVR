using System.Linq;
using UnityEngine;
namespace BWVR {
public sealed partial class VillagerExploration {
 VillagerExploration[] neighbors=System.Array.Empty<VillagerExploration>();
 public void ConfigurePeers(VillagerExploration[] residents){neighbors=residents;}
 public bool CanPeerPlay(BeachBall ball)=>ball!=null&&CanStartStructure&&social.Memory.injury<=0&&!ball.Playmate.Season.paused&&ball.Playmate.State.time>=social.Memory.nextPeerPlayAt&&(ball.Playmate.Season.developmentFocus||ball.Playmate.Season.Finished||(ball.Playmate.Season.water>=45&&ball.Playmate.Season.crops>=45));
 public VillagerExploration PreferredPeer(BeachBall ball){if(!CanPeerPlay(ball))return null;float time=ball.Playmate.State.time;string own=GetComponent<HybridTarget>().id;return neighbors.Where(p=>p!=null&&p!=this&&p.CanPeerPlay(ball)&&social.Memory.PeerWilling(p.GetComponent<HybridTarget>().id,time)&&p.social.Memory.PeerWilling(own,time)&&Vector3.Distance(p.transform.position,ball.transform.position)<nav.world.Scale*120).OrderByDescending(p=>social.Memory.PeerAppeal(p.GetComponent<HybridTarget>().id)*.8f-Vector3.Distance(transform.position,p.transform.position)/nav.world.Scale*.004f).FirstOrDefault();}
 bool ChoosePeerPlay(){foreach(var ball in beachballs){if(ball==null||Vector3.Distance(transform.position,ball.transform.position)>nav.world.Scale*120)continue;var friend=PreferredPeer(ball);if(friend!=null&&ball.TryPeerGame(this,friend))return true;}return false;}
 public void BeginPeerBall(BeachBall ball){playingBall=ball;soloNudges=0;routine.ResetTask();nav.ResetRoute();clock=elapsed=0;Current=Phase.Beachball;motion?.LookAt(ball.transform);motion?.Signal(true,true,ball.transform.position);}
}
}
