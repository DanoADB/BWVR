using System;
using UnityEngine;
namespace BWVR {
public sealed partial class BeachBall {
 public VillagerExploration PeerPlayer{get;private set;}public bool PeerReady{get;private set;}
 public bool PeerGame=>PeerPlayer!=null;public bool HasPartner=>CreatureJoined||PeerGame;
 Vector3 peerMeeting;VillagerExploration lastPeerKicker;
 public bool OwnsPlayer(VillagerExploration actor)=>Player==actor||PeerPlayer==actor;
 public bool TryPeerGame(VillagerExploration first,VillagerExploration second){
  if(first==null||second==null||first==second||!first.CanPeerPlay(this)||!second.CanPeerPlay(this)||creature.GetComponent<CreatureCuriosity>().HandlingBeachBall)return false;
  float time=creature.State.time;string a=first.GetComponent<HybridTarget>().id,b=second.GetComponent<HybridTarget>().id;
  if(!first.GetComponent<VillagerSocial>().Memory.PeerWilling(b,time)||!second.GetComponent<VillagerSocial>().Memory.PeerWilling(a,time)||!ReservePlay(first))return false;
  PeerPlayer=second;PeerReady=false;float best=float.MaxValue;bool found=false;float ground=nav.ProjectFoot(transform.position,nav.villager).y;
  for(int i=0;i<16;i++){var wanted=transform.position+Quaternion.Euler(0,i*22.5f,0)*Vector3.forward*nav.Scale*20;if(!nav.Sample(wanted,nav.villager,2,out var sample))continue;var at=nav.ProjectFoot(sample,nav.villager);if(Vector3.Dot(Flat(at-transform.position).normalized,Flat(first.transform.position-transform.position).normalized)>.3f||Mathf.Abs(at.y-ground)>nav.Scale*1.1f||!ClearLane(at))continue;float distance=(at-second.transform.position).sqrMagnitude;if(distance>=best)continue;best=distance;peerMeeting=at;found=true;}
  if(!found){CancelRally();return false;}
  first.BeginPeerBall(this);second.BeginPeerBall(this);first.GetComponent<VillagerSocial>().Memory.nextPeerPlayAt=second.GetComponent<VillagerSocial>().Memory.nextPeerPlayAt=time+90;
  OfferSpectators();creature.Journal.Add(time,"villager invites friend",CreatureAction.IdleOrObserve,a,b);return true;
 }
 public bool PreparePeer(float dt){if(!PeerGame||creature.Season.paused)return false;if(PeerReady)return true;var walker=PeerPlayer.GetComponent<IslandNavigator>();if(walker.MoveTo(peerMeeting,PeerPlayer.transform.lossyScale.y*1.6f,dt)){PeerReady=true;rallyClock=0;PeerPlayer.GetComponent<HumanoidMotion>()?.React();}return PeerReady;}
 bool PeerHealthy=>PeerPlayer!=null&&!PeerPlayer.GetComponent<CarryItem>().held&&PeerPlayer.GetComponent<VillagerSocial>().Memory.injury<=0&&Player.GetComponent<VillagerSocial>().Memory.injury<=0;
 void CreditPeerReturn(){
  if(RallyReceptions<2)return;var a=Player.GetComponent<VillagerSocial>().Memory;var b=PeerPlayer.GetComponent<VillagerSocial>().Memory;
  if(RallyReceptions==2){a.RememberPeer(PeerPlayer.GetComponent<HybridTarget>().id,true,creature.State.time);b.RememberPeer(Player.GetComponent<HybridTarget>().id,true,creature.State.time);}
  if(RallyReceptions==4){a.peerGames=Math.Min(1000000,a.peerGames+1);b.peerGames=Math.Min(1000000,b.peerGames+1);}
  Player.GetComponent<HumanoidMotion>()?.React();PeerPlayer.GetComponent<HumanoidMotion>()?.React();creature.Journal.Add(creature.State.time,"villager peer return",CreatureAction.IdleOrObserve,Player.GetComponent<HybridTarget>().id,PeerPlayer.GetComponent<HybridTarget>().id);
 }
 void WitnessPeerHarm(VillagerSocial witness){if(lastPeerKicker==null||witness.transform==lastPeerKicker.transform)return;witness.Memory.RememberPeer(lastPeerKicker.GetComponent<HybridTarget>().id,false,creature.State.time);}
}
}
