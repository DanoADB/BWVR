using System;
using System.Linq;
using UnityEngine;
namespace BWVR.Editor {
// Staged request -> released ball -> native creature/villager rally, without injected receptions.
public static class RequestedPlayChecks {
 static void Require(bool ok,string why){if(!ok)throw new Exception(why);}
 public static void Verify()=>VerifyWithCapture(null);
 public static void VerifyWithCapture(Action<QuestSlice> capture){
  var site=UnityEngine.Object.FindAnyObjectByType<CreatureConstructionLearning>().sites[0];var original=site.transform.position;bool paused=site.Paused;
  try{BallRallyChecks.VerifyWithCapture((ball,actor,state)=>{
   var god=ball.Playmate;var nav=actor.GetComponent<IslandNavigator>();float unit=nav.world.Scale;var home=ball.transform.position;
   actor.ResetTask();site.Paused=false;site.transform.position=(actor.transform.position+god.transform.position)*.5f;
   var memory=actor.GetComponent<VillagerSocial>().Memory;memory.RememberPlace(0,true,0);memory.Place(0).appeal=.8f;memory.restNeed=.5f;memory.ballGames=1;state.Weather.rain=0;state.Hybrid.time=200;
   void PutBall(Vector3 at){ball.transform.position=at;ball.Body.position=nav.world.ToNav(at);ball.Body.linearVelocity=ball.Body.angularVelocity=Vector3.zero;Physics.SyncTransforms();}
   bool requested=false;for(int i=0;i<16&&!requested;i++){
    var desired=site.transform.position+Quaternion.Euler(0,i*22.5f,0)*Vector3.forward*unit*34;
    if(!nav.world.Sample(desired,nav.profile,3,out var at))continue;
    PutBall(nav.world.ProjectFoot(at,nav.profile)+Vector3.up*unit*ball.Radius);actor.transform.LookAt(new Vector3(ball.transform.position.x,actor.transform.position.y,ball.transform.position.z));
    requested=actor.TryPlaceImprovement(site);
   }
   Require(requested,"No visible request stage by native rally lane");
   for(int i=0;i<1600&&actor.GatheringRequest==PlaceImprovement.None&&actor.Active;i++){state.Hybrid.time+=.05f;actor.Tick(.05f,false);}
   Require(actor.GatheringRequest==PlaceImprovement.BeachBall,"Native request approach failed");
   PutBall(home);ball.GetComponent<CarryItem>().held=true;actor.Tick(.3f,false);Require(memory.Place(0).answers==0&&memory.Place(0).ballUses==0,"Held ball answered request");ball.GetComponent<CarryItem>().held=false;actor.Tick(.3f,false);
   Require(ball.Player==actor&&memory.Place(0).answers==1&&memory.Place(0).ballUses==0,"Released ball did not create pending handoff");
  },(ball,actor,state)=>{
   var memory=actor.GetComponent<VillagerSocial>().Memory;var place=memory.Place(0);Require(place.ballUses==1&&place.coverUses==0,"Four native receptions did not confirm requested play");
   actor.WitnessBallPlay(ball);Require(place.ballUses==1,"Completed rally credited requested use twice");
   string before=JsonUtility.ToJson(state);var text=CreationBookText.Read(state,CreationBookSection.Places).text;Require(text.Contains("Used after request: cover 0 · ball 1")&&before==JsonUtility.ToJson(state),"Book omitted or mutated confirmed use");
   var restored=JsonUtility.FromJson<SliceState>(before);Require(restored.Valid()&&restored.Social(0).Place(0).ballUses==1,"Requested play did not survive save round trip");capture?.Invoke(UnityEngine.Object.FindAnyObjectByType<QuestSlice>());
  });Debug.Log("BWVR_REQUESTED_PLAY_PASS: visible experienced request, native approach, held/released handoff, four native physical receptions, once-only saved use and pure Book projection");}
  finally{site.transform.position=original;site.Paused=paused;}
 }
}
}
