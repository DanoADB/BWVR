using UnityEngine;
namespace BWVR {
// The social controller borrows the existing site visitor lease while performing.
public sealed partial class VillagerSocial {
 ConstructionSite performerCover;float performerCoverCheck;
 bool CoveredMusicMeeting(CreatureAffordance prop,out Vector3 at,out ConstructionSite cover){
  at=default;cover=null;if(season().Weather.rain<.5f)return false;
  var learning=hybrid.GetComponent<CreatureConstructionLearning>();var paths=hybrid.GetComponentInParent<QuestSlice>().GetComponentInChildren<IslandDiscoveries>();
  foreach(var site in learning.sites){
   if(site.IsBridge||site.Paused||site.Locked||site.Visitor!=null||site.Requester!=null||site.Repairer!=null)continue;
   if(site.TryNearbyShelterSpot(walker.profile.radius+.08f,walker.profile.height,prop.transform.position,walker.world.Scale*4.5f,point=>walker.CanPlace(point,out var safe)&&Vector3.Distance(point,safe)<walker.world.Scale*.35f&&paths.Reachable(transform.position,point,walker.profile),out at)){cover=site;return true;}
  }return false;
 }
 void ReservePerformerCover(ConstructionSite site){performerCover=site;performerCoverCheck=0;if(site!=null)site.Visitor=GetComponent<VillagerExploration>();}
 void CheckPerformerCover(float dt){performerCoverCheck-=dt;if(performerCoverCheck>0)return;performerCoverCheck=.25f;if(performerCover!=null&&(performerCover.Paused||performerCover.Locked||performerCover.Visitor!=GetComponent<VillagerExploration>()||!performerCover.ShelteredAt(walker.profile.radius+.08f,walker.profile.height,meeting)))ReleasePerformerCover();}
 void ReleasePerformerCover(){if(performerCover!=null&&performerCover.Visitor==GetComponent<VillagerExploration>())performerCover.Visitor=null;performerCover=null;}
}
}
