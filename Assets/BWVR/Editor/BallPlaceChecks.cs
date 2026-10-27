using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace BWVR.Editor {
public static class BallPlaceChecks {
 static void Require(bool ok,string why){if(!ok)throw new Exception(why);}
 public static void Model(){
  var m=new SocialMemory();m.RememberPlace(0,true,0);m.RememberPlace(0,true,100);m.RememberPlace(0,true,200);
  var low=new SocialMemory();low.RememberPlace(0,false,0);low.RememberPlace(0,false,100);
  int Count(SocialMemory place,bool peer,bool comfortable)=>Enumerable.Range(0,1000).Count(i=>peer?m.WantsPeerPlay(1347,i,"place-tester",place.PlacePlayBias(0,comfortable)):m.WantsCreaturePlay(1347,i,"place-tester",.5f,place.PlacePlayBias(0,comfortable)));
  Require(m.PlacePlayBias(1,true)==0&&Mathf.Abs(m.PlacePlayBias(0,true))<=.15f,"Unknown place biased play or bias unbounded");
  Require(Count(m,false,true)>Count(low,false,true)+80&&Count(m,true,true)>Count(low,true,true)+80,"Place experience did not materially influence autonomous play");
  Require(Count(m,true,false)<Count(m,true,true)&&Count(m,false,false)<Count(m,false,true),"Rain has no influence on place play choice");
  Require(m.WantsPeerPlay(1,1,"x",-100)==m.WantsPeerPlay(1,1,"x",-.15f),"Caller bypassed bias bound");
  Require(JsonUtility.FromJson<SocialMemory>(JsonUtility.ToJson(m)).Valid()&&JsonUtility.FromJson<SocialMemory>("{}").Valid(),"Play place save/legacy invalid");
  Debug.Log($"BWVR_BALL_PLACE_MODEL_PASS: creature invitations {Count(low,false,true)} -> {Count(m,false,true)}/1000; peer invitations {Count(low,true,true)} -> {Count(m,true,true)}/1000; rain lowers both; bounded bias, legacy/save validation");
 }
 public static void VerifyCreature(){
  var site=UnityEngine.Object.FindAnyObjectByType<CreatureConstructionLearning>().sites[0];var position=site.transform.position;
  try{BallRallyChecks.VerifyWithCapture((ball,actor,state)=>{site.transform.position=(actor.transform.position+ball.Playmate.transform.position)*.5f;state.Weather.rain=0;},(ball,actor,state)=>Require(actor.GetComponent<VillagerSocial>().Memory.Place(0)?.visits==1,"Creature rally did not record participant place"));}
  finally{site.transform.position=position;}
 }
 public static void VerifyFar()=>Run(0,false);
 public static void VerifyDry()=>Run(0);
 public static void VerifyRain()=>Run(1);
 static void Run(float rain,bool nearby=true){
  Require(Application.isPlaying,"Enter Play mode");var slice=UnityEngine.Object.FindAnyObjectByType<QuestSlice>();var ball=slice.GetComponentInChildren<BeachBall>();var god=ball.Playmate;var site=god.GetComponent<CreatureConstructionLearning>().sites[0];var people=slice.GetComponentsInChildren<VillagerExploration>();var first=people[0];var friend=people[1];var nav=first.GetComponent<IslandNavigator>();float unit=nav.world.Scale;var flags=BindingFlags.Instance|BindingFlags.NonPublic;var field=typeof(QuestSlice).GetField("state",flags);var original=field.GetValue(slice);var positions=people.Select(p=>p.transform.position).ToArray();var rotations=people.Select(p=>p.transform.rotation).ToArray();var siteAt=site.transform.position;var godAt=god.transform.position;var test=new SliceState{water=100,crops=100,developmentFocus=true,observeWithoutFoodWork=true};test.Weather.rain=rain;test.Hybrid.time=200;god.Interrupt("Ball place QA");foreach(var p in people){p.ResetTask();p.GetComponent<VillagerSocial>().ResetTask();p.GetComponent<VillagerResourceGathering>().Cancel();p.GetComponent<VillagerBlockPlay>().ResetTask();}field.SetValue(slice,test);slice.GetComponentInChildren<VillageFamilies>().ResetWorld();
  void Reset(){foreach(var p in people){p.ResetTask();p.GetComponent<VillagerSocial>().ResetTask();p.GetComponent<CarryItem>().held=false;p.GetComponent<VillagerSocial>().Memory.nextPeerPlayAt=0;}ball.ReturnHome();}
  bool Stage(){for(int i=0;i<16;i++){Reset();var direction=Quaternion.Euler(0,i*22.5f,0)*Vector3.forward;var center=ball.transform.position;if(!nav.world.Sample(center-direction*unit*6,nav.profile,2,out var a)||!nav.world.Sample(center+direction*unit*20,nav.profile,2,out var b))continue;first.transform.position=nav.world.ProjectFoot(a,nav.profile);friend.transform.position=nav.world.ProjectFoot(b,nav.profile);foreach(var p in people)p.GetComponent<IslandNavigator>().ResetRoute();if(Mathf.Abs(friend.transform.position.y-(center.y-ball.Radius*unit))>unit*1.1f)continue;if(ball.TryPeerGame(first,friend)){// Stage the place around the known reachable rally lane; do not alter terrain or fake receptions.
    site.transform.position=(first.transform.position+friend.transform.position)*.5f+(nearby?Vector3.zero:Vector3.right*unit*1000);return true;}}return false;}
  void Step(){test.Hybrid.time+=.02f;first.Tick(.02f,false);friend.Tick(.02f,false);ball.Advance(.02f,false);}
  try{
   Require(Stage(),"No native peer lane");test.paused=true;first.Tick(2,true);ball.Advance(2,true);Require(!first.WitnessBallPlay(ball)&&test.Social(0).Place(0)==null,"Paused or unfinished invitation rewarded");test.paused=false;
   for(int i=0;i<4000&&ball.RallyReceptions<4;i++){Step();if(ball.RallyReceptions<4)Require(test.Social(0).Place(0)==null&&test.Social(1).Place(0)==null,"Incomplete rally credited place");}
   Require(ball.RallyReceptions==4,"Native four-reception game did not finish");if(!nearby){Require(test.Social(0).Place(0)==null&&test.Social(1).Place(0)==null,"Distant yard got fictional place credit");Debug.Log("BWVR_BALL_PLACE_FAR_PASS");return;}foreach(var index in new[]{0,1})Require(test.Social(index).Place(0)?.visits==1&&test.Social(index).Place(0).uncomfortable==(rain==0?0:1),"Participant place memory missing/weather wrong: "+index);
   test.Hybrid.time+=100;Require(!first.WitnessBallPlay(ball)&&!friend.WitnessBallPlay(ball)&&test.Social(0).Place(0).visits==1,"Completed game re-credited after memory cooldown");
   var save=JsonUtility.FromJson<SliceState>(JsonUtility.ToJson(test));Require(save.Valid()&&save.Social(1).Place(0).visits==1,"Native play place save invalid");
   Require(Stage(),"Cancellation staging failed");int visits=test.Social(0).Place(0).visits;friend.GetComponent<CarryItem>().held=true;friend.Tick(.02f,false);first.Tick(.02f,false);Require(!ball.PeerGame&&!first.WitnessBallPlay(ball)&&test.Social(0).Place(0).visits==visits,"Pickup cancellation rewarded place");
   Debug.Log("BWVR_BALL_PLACE_RUNTIME_PASS: native four physical receptions, both participant memories, rain context, no incomplete/paused/canceled credit, session deduplication beyond cooldown, persistence");
  }finally{foreach(var p in people){p.GetComponent<CarryItem>().held=false;p.ResetTask();p.GetComponent<VillagerSocial>().ResetTask();}ball.CancelRally();site.transform.position=siteAt;for(int i=0;i<people.Length;i++){people[i].transform.SetPositionAndRotation(positions[i],rotations[i]);people[i].GetComponent<IslandNavigator>().ResetRoute();}god.transform.position=godAt;god.GetComponent<IslandNavigator>().ResetRoute();field.SetValue(slice,original);ball.ResetWorld();}
 }
}
}
