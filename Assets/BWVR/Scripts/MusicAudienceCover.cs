using System;
using UnityEngine;
namespace BWVR {
public sealed partial class SharedRhythm {
 readonly Vector3[] audienceSpots=new Vector3[3];readonly float[] audienceRadii=new float[3];
 public bool AudienceSpotAvailable(VillagerExploration listener,Vector3 point,float radius){
  if(!Active||listener==null||!HybridState.Number(radius,.1f,4)||Array.IndexOf(audience,listener)<0)return false;float unit=creature.GetComponent<IslandNavigator>().world.Scale;
  for(int i=0;i<audience.Length;i++)if(audience[i]!=null&&audience[i]!=listener&&audienceRadii[i]>0&&Vector3.Distance(point,audienceSpots[i])<(radius+audienceRadii[i]+.2f)*unit)return false;return true;
 }
 public bool ReserveAudienceSpot(VillagerExploration listener,Vector3 point,float radius){if(!AudienceSpotAvailable(listener,point,radius))return false;int index=Array.IndexOf(audience,listener);audienceSpots[index]=point;audienceRadii[index]=radius;return true;}
 public void ReleaseAudienceSpot(VillagerExploration listener){int index=Array.IndexOf(audience,listener);if(index>=0)audienceRadii[index]=0;}
}
// A short comfort adjustment on the audience's existing motor; no extra actor controller.
public sealed partial class VillagerExploration {
 ConstructionSite musicCoverSite;Vector3 musicCoverPoint;bool musicCoverClaimed,musicCoverWalking;float musicCoverCheck,musicCoverTravel;
 public bool MovingToMusicCover=>MusicAudience&&musicCoverWalking;
 void ChooseMusicCover(){
  var god=construction.GetComponent<CreatureHybrid>();if(god.Season.Weather.rain<.5f)return;
  foreach(var site in construction.sites)if(site.ShelteredAt(nav.profile.radius+.08f,nav.profile.height,transform.position))return;
  foreach(var site in construction.sites){if(site.Paused||site.Locked||site.Visitor!=null||site.Requester!=null||site.Repairer!=null)continue;
   if(!site.TryNearbyShelterSpot(nav.profile.radius+.08f,nav.profile.height,transform.position,nav.world.Scale*6,point=>Vector3.Distance(point,music.transform.position)<nav.world.Scale*24&&nav.CanPlace(point,out _)&&world.Reachable(transform.position,point,nav.profile)&&music.AudienceSpotAvailable(this,point,nav.profile.radius),out var rest)||!music.ReserveAudienceSpot(this,rest,nav.profile.radius))continue;
   musicCoverSite=site;musicCoverPoint=rest;musicCoverClaimed=musicCoverWalking=true;musicCoverCheck=musicCoverTravel=0;nav.ResetRoute();motion?.Activity(0);return;
  }
 }
 void AdvanceMusicCover(float dt){
  if(!musicCoverClaimed)return;musicCoverCheck-=dt;
  if(musicCoverCheck<=0){musicCoverCheck=.25f;if(musicCoverSite==null||musicCoverSite.Paused||musicCoverSite.Locked||musicCoverSite.Visitor!=null||musicCoverSite.Requester!=null||musicCoverSite.Repairer!=null||!musicCoverSite.ShelteredAt(nav.profile.radius+.08f,nav.profile.height,musicCoverPoint)){ReleaseMusicCover();return;}}
  if(!musicCoverWalking)return;musicCoverTravel+=dt;
  bool arrived=nav.MoveTo(musicCoverPoint,transform.lossyScale.y*1.6f,dt);var delta=transform.position-musicCoverPoint;delta.y=0;
  if(delta.magnitude<=nav.world.Scale*.35f){musicCoverWalking=false;nav.ResetRoute();motion?.Activity(6);}
  else if(arrived||nav.Blocked||musicCoverTravel>3)ReleaseMusicCover();
 }
 void ReleaseMusicCover(){music?.ReleaseAudienceSpot(this);musicCoverSite=null;musicCoverClaimed=musicCoverWalking=false;musicCoverCheck=musicCoverTravel=0;nav?.ResetRoute();motion?.Activity(6);}
}
}
