using UnityEngine;
namespace BWVR {
public sealed partial class CreatureCuriosity {
 VillageResources deliveryWorld;int deliveryStage,deliveryRetries;
 public bool TryBeginToolDelivery(CreatureAffordance candidate){var world=candidate?.GetComponent<ToolDelivery>()?.world;if(Current!=Phase.Rest||routine.Current!=CreatureRoutine.Task.SeekingWater||world==null||!world.ClaimToolDelivery())return false;deliveryWorld=world;routine.ResetTask();navigator.ResetRoute();target=candidate;owned=null;original=candidate.transform.position;originalRotation=candidate.transform.rotation;goalPlay=echoPlay=fetching=bowlingPlay=false;elapsed=clock=0;deliveryStage=deliveryRetries=0;Current=Phase.ToolDelivery;motion?.LookAt(candidate.transform);return true;}
 bool TickToolDelivery(float dt){
  var world=deliveryWorld;if(world==null||world.Delivery?.Active!=true||!world.DeliveryClaimed||elapsed>140||target==null||owned==null&&target.Item.held||world.DeliveryPerson==null||world.DeliveryPerson.GetComponent<CarryItem>().held||world.DeliveryPerson.Memory.injury>=.7f||world.DeliveryPerson.RecentFright){Finish(false);return false;}
  Affect=CreatureAffect.Mood.Curious;
  if(deliveryStage==0){if(navigator.MoveTo(target.transform.position,routine.speed,dt)){owned=target.Item;owned.held=true;owned.GetComponent<Collider>().enabled=false;motion?.Carry(owned.transform);deliveryStage=1;clock=0;}else if(navigator.Blocked&&elapsed>8){ToolRouteFailed();return false;}}
  else if(deliveryStage==1){if(clock>.8f){deliveryStage=2;clock=0;}}
  else if(deliveryStage==2){if(!world.TryToolDestination(out var destination)){ToolRouteFailed();return false;}motion?.LookAt(world.DeliveryPerson.transform);if(navigator.MoveTo(destination,routine.speed*.8f,dt)){landing=destination;deliveryStage=3;clock=0;motion?.Activity(5);}else if(navigator.Blocked&&clock>2){if(deliveryRetries++>=3){ToolRouteFailed();return false;}world.RetryToolApproach();navigator.ResetRoute();clock=0;}}
  else {if(Vector3.Distance(landing,world.DeliveryPerson.transform.position)>navigator.world.Scale*10){deliveryStage=2;clock=0;navigator.ResetRoute();return true;}motion?.PlaceAt(landing,originalRotation,Mathf.Clamp01(clock));if(clock>1.1f){motion?.Carry(null);owned.transform.SetPositionAndRotation(landing,originalRotation);owned.held=false;owned.GetComponent<Collider>().enabled=true;owned.home=landing;owned=null;bool success=world.ConfirmToolDelivery(ToolDeliverySource.Creature);if(success){lastWater=lastSocial=false;feedbackWindow=12;boredom=0;}Finish(success);return false;}}
  return true;
 }
 void ToolRouteFailed(){GetComponent<CreatureHybrid>().Journal.Add(state.Hybrid.time,"tool route failed",CreatureAction.RespondToGuidance,target!=null?target.GetComponent<HybridTarget>().id:"workshop-tool-delivery","step "+deliveryStage+"; actor "+navigator.world.ToNav(transform.position)+"; recipient "+navigator.world.ToNav(deliveryWorld.DeliveryPerson.transform.position)+"; blocked "+navigator.Blocked);Finish(false,"Target unreachable");}
 void ReleaseToolDelivery(){deliveryWorld?.ReleaseToolDelivery();deliveryWorld=null;deliveryStage=0;}
}
}
