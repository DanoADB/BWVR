using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace BWVR.Editor {
public static class PlayerBallPlacementChecks {
 [MenuItem("BWVR/Verify native beachball placement (Play mode)")]
 public static void Verify(){
  void Require(bool ok,string why){if(!ok)throw new Exception(why);}
  Require(EditorApplication.isPlaying,"Enter Play mode first");
  var slice=UnityEngine.Object.FindAnyObjectByType<QuestSlice>();var god=slice.GetComponentInChildren<CreatureHybrid>();var nav=god.GetComponent<IslandNavigator>();var ball=slice.GetComponentInChildren<BeachBall>();var item=ball.GetComponent<CarryItem>();var flags=BindingFlags.Instance|BindingFlags.NonPublic;
  var field=typeof(QuestSlice).GetField("state",flags);var original=(SliceState)field.GetValue(slice);var held=(CarryItem[])typeof(QuestSlice).GetField("held",flags).GetValue(slice);
  Require(held[0]==null&&held[1]==null&&!item.held,"Release player-held objects first");var position=ball.transform.position;var rotation=ball.transform.rotation;
  god.Interrupt("Player ball placement QA");ball.ReturnHome();var test=new SliceState{developmentFocus=true,observeWithoutFoodWork=true,water=100,crops=100};field.SetValue(slice,test);god.NewSeason();
  var palette=slice.GetComponentInChildren<GodToolPalette>();palette.Cancel();palette.Process(new Ray(Camera.main.transform.position,Camera.main.transform.forward),false,true,false,.1f,false);
  bool Grab()=>(bool)typeof(QuestSlice).GetMethod("TryGrab",flags).Invoke(slice,new object[]{1,item});
  void Release(Ray ray)=>typeof(QuestSlice).GetMethod("Release",flags).Invoke(slice,new object[]{1,ray});
  Ray aim=default;Vector3 ground=default;bool found=false;
  try{
   for(int i=0;i<32&&!found;i++){var wanted=item.home+Quaternion.Euler(0,i*137.5f,0)*Vector3.forward*nav.world.Scale*(18+i);aim=new Ray(wanted+Vector3.up*.2f,Vector3.down);var args=new object[]{item,aim,Vector3.zero};found=(bool)typeof(QuestSlice).GetMethod("TryDropPoint",flags).Invoke(slice,args);if(found)ground=(Vector3)args[2];}
   Require(found,"No dry placement target");
   void Check(){Require(!item.held&&held[1]==null&&!ball.Body.isKinematic,"Release retained player/kinematic ownership");Require(Vector3.Distance(nav.world.ToWorld(ball.Body.position),ball.transform.position)<nav.world.Scale*.01f,"Physics body disagrees with rendered ball on release");Require(Mathf.Abs(ball.transform.position.y-ground.y-ball.Radius*nav.world.Scale)<nav.world.Scale*.01f,"Ball bottom does not meet the aimed ground");Require(!ball.InFlight&&!ball.FetchReserved&&!ball.PeerGame&&test.WorldPlay.throws==0,"Placement retained a game/throw or credited creature");}
   Require(Grab(),"Quick native grab refused");Release(aim);Check();var released=ball.transform.position;ball.Advance(.02f,false);Require(Vector3.Distance(ball.transform.position,released)<nav.world.Scale*2,"Next physics step snapped ball to old location");
   Require(Grab(),"Held native grab refused");ball.Advance(.02f,false);Release(aim);Check();
   test.paused=true;Require(Grab(),"Paused native grab refused");Release(aim);Check();released=ball.transform.position;ball.Advance(1,true);Require(ball.transform.position==released,"Paused placement moved");test.paused=false;
   Require(ball.PlayerToss(Vector3.right*.5f),"Fetch setup toss refused");Require(Grab(),"In-flight native grab refused");Release(aim);Check();
   Require(Grab(),"Invalid-target native grab refused");Release(new Ray(ground+Vector3.up*.2f,Vector3.up));Require(Vector3.Distance(ball.transform.position,item.home)<nav.world.Scale*.01f&&Vector3.Distance(nav.world.ToWorld(ball.Body.position),item.home)<nav.world.Scale*.01f&&!ball.InFlight&&!ball.FetchReserved,"Invalid target did not return visual/physics/claims home together");
   Require(JsonUtility.FromJson<SliceState>(JsonUtility.ToJson(test)).Valid(),"Native ball placement invalidated save");Debug.Log("BWVR_PLAYER_BALL_PLACEMENT_PASS: quick and held native release, radius-correct ground contact, immediate private-body handoff, next physics step, paused placement, fetch cancellation and invalid-target safe return");
  }finally{held[1]=null;item.held=false;field.SetValue(slice,original);god.NewSeason();ball.ResetWorld();ball.transform.SetPositionAndRotation(position,rotation);ball.Body.position=nav.world.ToNav(position);ball.Body.rotation=rotation;}
 }
}
}
