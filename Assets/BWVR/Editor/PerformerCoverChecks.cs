using System;
using System.Linq;
using UnityEngine;
namespace BWVR.Editor {
public static class PerformerCoverChecks {
 static void Require(bool ok,string why){if(!ok)throw new Exception(why);}
 public static void Verify()=>Run(0);
 public static void VerifyRemoval()=>Run(1);
 public static void VerifyDry()=>Run(2);
 public static void VerifyOccupied()=>Run(3);
 static void Run(int mode){
  ConstructionSite site=null;ConstructionBlock roof=null;Vector3 rest=default;VillagerExploration occupant=null;
  try{PlaceMusicChecks.VerifyWithCapture(null,(instrument,host,god)=>{
   Require(site.TryShelterSpot(host.GetComponent<IslandNavigator>().profile.radius+.08f,host.GetComponent<IslandNavigator>().profile.height,out rest,out _),"Fixture has no useful performer cover");
   if(mode==2||mode==3){Require(site.Visitor==occupant,"Dry performance claimed shelter or occupied cover was stolen");return;}
   Require(site.Visitor==host.GetComponent<VillagerExploration>()&&Vector3.Distance(host.transform.position,rest)<site.unit*.35f,"Rainy performer ignored reachable cover or failed to reserve it");
   var neighbor=god.GetComponentInParent<QuestSlice>().GetComponentsInChildren<VillagerExploration>().First(p=>p.gameObject!=host.gameObject);neighbor.GetComponent<VillagerSocial>().Memory.restNeed=.95f;
   Require(!neighbor.TryShelter(site),"Resting neighbor stole performer cover");
   if(mode==1){var at=host.transform.position;site.ReturnLoose(roof);host.Tick(.3f,false);Require(site.Visitor==null&&host.transform.position==at&&host.Ready,"Roof removal retained lease, teleported performer or cancelled invitation");}
  },(instrument,host,god)=>{
   var memory=host.Memory.Place(0);Require(memory!=null&&memory.visits==2&&memory.uncomfortable==(mode==1||mode==3?1:0),"Performance memory ignored actual cover/weather");Require(site.Visitor==occupant,"Completed performance leaked or stole visitor lease");
   Debug.Log("BWVR_PERFORMER_COVER_PASS mode="+mode+": native covered approach, existing visitor lease, rain-sensitive outcome, completion/pickup/timeout cleanup");
  },(instrument,host,god)=>{
   site=god.GetComponent<CreatureConstructionLearning>().sites[0];if(occupant!=null&&site.Visitor==occupant)site.Visitor=null;Require(site.Visitor==null,"Previous invitation leaked cover");site.ResetConstruction();god.Season.Weather.rain=mode==2?0:1;
   ConstructionBlock Put(ConstructionShape shape,Vector3 at,bool rotate=false){var b=site.blocks.First(p=>p.Shape==shape&&!p.Attached);Require(site.Drop(b,site.World(at),site.transform.rotation*Quaternion.Euler(0,rotate?90:0,0)),"Could not stage nearby supported cover: "+shape+" at "+at);return b;}
   Put(ConstructionShape.Column,new Vector3(4,2.4f,-14));Put(ConstructionShape.Column,new Vector3(4,2.4f,-10));roof=Put(ConstructionShape.Slab,new Vector3(4,5.4f,-12),true);
   if(mode==3){occupant=god.GetComponentInParent<QuestSlice>().GetComponentsInChildren<VillagerExploration>().First(p=>p.gameObject!=host.gameObject);site.Visitor=occupant;}
  });}finally{if(occupant!=null&&site!=null&&site.Visitor==occupant)site.Visitor=null;}
 }
}
}
