using UnityEngine;
namespace BWVR {
// Keep a familiar gathering-place instrument where it was arranged. Social retains the motor.
public sealed partial class VillagerSocial {
 ConstructionSite musicPlace;Vector3 musicAnchor;
 int MusicSlot(ConstructionSite site){var learning=hybrid.GetComponent<CreatureConstructionLearning>();var clearing=site.GetComponentInParent<DiscoveryPoint>();return clearing!=null?clearing.id+1:learning.sites[0]==site?0:-1;}
 ConstructionSite KnownMusicPlace(CreatureAffordance prop){
  if(prop.GetComponent<SharedRhythm>()==null)return null;var learning=hybrid.GetComponent<CreatureConstructionLearning>();if(learning==null)return null;
  ConstructionSite best=null;float distance=walker.world.Scale*18;
  foreach(var site in learning.sites){if(site==null||site.IsBridge)continue;var place=Memory.Place(MusicSlot(site));float gap=Vector3.Distance(prop.transform.position,site.transform.position);if(place==null||place.visits==0||gap>=distance)continue;distance=gap;best=site;}return best;
 }
 bool MusicMeeting(CreatureAffordance prop,out Vector3 at){
  var paths=hybrid.GetComponentInParent<QuestSlice>().GetComponentInChildren<IslandDiscoveries>();var away=prop.transform.position-creature.transform.position;away.y=0;if(away.sqrMagnitude<1e-10f)away=transform.forward;
  foreach(float angle in new[]{0,45,-45,90,-90}){var desired=prop.transform.position+Quaternion.Euler(0,angle,0)*away.normalized*walker.world.Scale*3;if(walker.CanPlace(desired,out at)&&paths.Reachable(transform.position,at,walker.profile))return true;}at=default;return false;
 }
 void PresentOffering(CreatureAffordance prop){Current=Phase.PresentGift;clock=0;motion?.LookAt(creature.transform);motion?.Signal(true,true,prop.transform.position);hybrid.Journal.Add(season().Hybrid.time,"invitation",prop.kind==CreatureAffordance.Kind.Food?CreatureAction.EatFood:CreatureAction.PlayWithToy,prop.GetComponent<HybridTarget>().id,GetComponent<HybridTarget>().id);}
 void RememberPlacePerformance(){if(musicPlace==null)return;bool comfortable=season().Weather.rain<.5f||musicPlace.ShelteredAt(walker.profile.radius+.08f,walker.profile.height,transform.position);Memory.RememberPlace(MusicSlot(musicPlace),comfortable,season().Hybrid.time);}
}
}
