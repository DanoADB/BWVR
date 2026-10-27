using System;
using System.Linq;
using UnityEngine;
namespace BWVR.Editor {
public static class MusicCoverChecks {
 static void Require(bool ok,string why){if(!ok)throw new Exception(why);}
 public static void Verify()=>Run(0,null);
 public static void VerifyRemoval()=>Run(1,null);
 public static void VerifyPickup()=>Run(2,null);
 public static void VerifyWithCapture(Action<VillagerExploration,VillagerExploration,ConstructionSite> capture)=>Run(0,capture);
 static void Run(int mode,Action<VillagerExploration,VillagerExploration,ConstructionSite> capture){
  VillagerExploration first=null,second=null;ConstructionSite site=null;ConstructionBlock roof=null;Vector3 start=default;
  PlaceMusicChecks.VerifyWithCapture((instrument,host,god)=>{
   Require(first.MusicAudience&&second.MusicAudience,"Staged listeners did not join native audience: "+first.Current+" / "+second.Current+", visible="+first.CanSeeMusic(instrument)+" / "+second.CanSeeMusic(instrument));Require(!second.MovingToMusicCover&&!instrument.AudienceSpotAvailable(second,site.World(new Vector3(10,0,-12)),first.GetComponent<IslandNavigator>().profile.radius)&&Vector3.Distance(first.transform.position,start)>god.GetComponent<IslandNavigator>().world.Scale,"Listener did not approach cover or two listeners claimed one spot");
   Require(!instrument.ReserveAudienceSpot(null,Vector3.zero,.5f),"Null listener reserved a shelter spot");
   var at=first.transform.position;bool paused=god.Season.paused;god.Season.paused=true;first.Tick(10,true);second.Tick(10,true);Require(first.transform.position==at&&first.GetComponent<VillagerSocial>().Memory.musicGatherings==0,"Pause moved or rewarded shelter listener");god.Season.paused=paused;
   capture?.Invoke(first,second,site);
   if(mode==1){site.ReturnLoose(roof);first.Tick(.3f,false);Require(!first.MovingToMusicCover&&instrument.AudienceSpotAvailable(second,site.World(new Vector3(10,0,-12)),first.GetComponent<IslandNavigator>().profile.radius),"Lost roof retained movement/claim");}
   if(mode==2){first.GetComponent<CarryItem>().held=true;first.Tick(.05f,false);Require(!first.MusicAudience&&!first.MovingToMusicCover&&instrument.AudienceCount==1,"Listener pickup retained cover/audience lease");first.GetComponent<CarryItem>().held=false;}
  },(instrument,host,god)=>{
   var people=god.GetComponentInParent<QuestSlice>().GetComponentsInChildren<VillagerSocial>();first=people[1].GetComponent<VillagerExploration>();second=people[2].GetComponent<VillagerExploration>();site=god.GetComponent<CreatureConstructionLearning>().sites[0];god.Season.Weather.rain=1;
   ConstructionBlock Put(ConstructionShape shape,Vector3 at,bool rotate=false){var block=site.blocks.First(b=>b.Shape==shape&&!b.Attached);Require(site.Drop(block,site.World(at),site.transform.rotation*Quaternion.Euler(0,rotate?90:0,0)),"Audience canopy staging failed");return block;}
   Put(ConstructionShape.Column,new Vector3(10,2.4f,-14));Put(ConstructionShape.Column,new Vector3(10,2.4f,-10));roof=Put(ConstructionShape.Slab,new Vector3(10,5.4f,-12),true);
   foreach(var person in new[]{first,second}){var nav=person.GetComponent<IslandNavigator>();var wanted=site.World(new Vector3(person==first?13:14.8f,0,person==first?-12.15f:-19));Require(nav.world.Sample(wanted,nav.profile,.5f,out var at),"No near-cover listener stage");person.transform.position=nav.world.ProjectFoot(at,nav.profile);person.transform.LookAt(new Vector3(instrument.transform.position.x,person.transform.position.y,instrument.transform.position.z));nav.ResetRoute();person.GetComponent<VillagerSocial>().Memory.nextMusicAt=0;}start=first.transform.position;var route=second.GetComponent<IslandNavigator>();var resources=god.GetComponentInParent<QuestSlice>().GetComponentInChildren<VillageResources>();bool visible=false;for(int i=0;i<16&&!visible;i++){var wanted=instrument.transform.position+Quaternion.Euler(0,i*22.5f,0)*Vector3.forward*route.world.Scale*16;if(!route.world.Sample(wanted,route.profile,1,out var at))continue;second.transform.position=route.world.ProjectFoot(at,route.profile);second.transform.LookAt(new Vector3(host.transform.position.x,second.transform.position.y,host.transform.position.z));Physics.SyncTransforms();visible=resources.ProvisionVisible(second.GetComponent<VillagerSocial>(),host.transform.position,host.transform);}Require(visible,"No exposed audience sightline");route.ResetRoute();Physics.SyncTransforms();
  },(instrument,host,god)=>{
   var a=first.GetComponent<VillagerSocial>().Memory;var b=second.GetComponent<VillagerSocial>().Memory;
   Require(b.musicGatherings==1&&b.musicDisappointments==1&&b.musicInterest<.5f,"Exposed listener did not retain independent rainy outcome");
   Require(mode==2?a.musicGatherings==0:a.musicGatherings==1&&a.musicDisappointments==(mode==1?1:0),"Covered/canceled listener outcome incorrect");
   if(mode==0)Require(a.musicInterest>.5f&&a.Place(0)?.uncomfortable==0&&b.Place(0)?.uncomfortable==1,"One performance did not yield mixed place/interest outcomes");
   Require(instrument.AudienceCount==0&&!first.MovingToMusicCover&&!second.MovingToMusicCover&&god.Season.Valid(),"Completed audience leaked claims or invalidated save");Debug.Log("BWVR_MUSIC_COVER_PASS mode="+mode+": native rainy audience approach, exclusive roof spot, independent outcomes, pause/cover-loss/pickup cleanup and valid state");
  });
 }
}
}
