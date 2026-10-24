using UnityEngine;
namespace BWVR {
public sealed partial class BeachBall {
 public int ObservationSession{get;private set;}
 public bool VisibleTo(VillagerExploration spectator){
  if(item.held||spectator==null||spectator.GetComponent<CarryItem>().held||Vector3.Distance(spectator.transform.position,transform.position)>nav.Scale*55)return false;
  var eye=nav.ToNav(spectator.transform.position+Vector3.up*nav.Scale*2.6f);var destination=Body.position;var ray=destination-eye;float length=ray.magnitude;if(length<.01f)return true;
  if(!physics.Raycast(eye,ray/length,out var hit,length,~0,QueryTriggerInteraction.Ignore))return true;
  if(hit.rigidbody==Body)return true;var person=hit.collider.GetComponent<BallCollisionProxy>()?.target?.villager?.GetComponent<VillagerExploration>();return person!=null&&OwnsPlayer(person);
 }
 void OfferSpectators(){if(!HasPartner)return;foreach(var resident in villagers){var observer=resident.GetComponent<VillagerExploration>();if(!OwnsPlayer(observer))observer.TryWatchBall(this);}}
 void NotifyObservers(bool safe){
  if(!HasPartner)return;string demonstrator=lastPeerKicker!=null?lastPeerKicker.GetComponent<HybridTarget>().id:creatureThrow?"creature":Player.GetComponent<HybridTarget>().id;
  foreach(var resident in villagers){var observer=resident.GetComponent<VillagerExploration>();if(!OwnsPlayer(observer))observer.WitnessBallOutcome(this,safe,demonstrator);}
 }
 void PracticeParticipants(){Player.GetComponent<VillagerSocial>().Memory.BallLesson.Practice(creature.State.time);if(PeerGame)PeerPlayer.GetComponent<VillagerSocial>().Memory.BallLesson.Practice(creature.State.time);}
}
}
