using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
namespace BWVR.Editor {
public static class BallGoalChecks {
 static readonly BindingFlags Flags=BindingFlags.Instance|BindingFlags.NonPublic;
 static void Require(bool ok,string message){if(!ok)throw new Exception(message);}
 public static void VerifyModel(){
  Require(JsonUtility.FromJson<WorldPlayMemory>("{}").Valid(),"Historical play save rejected");
  Require(GoalCourse.Crosses(new Vector3(0,4,-8),new Vector3(0,4,8),3.5f),"Fast full opening crossing missed");
  Require(GoalCourse.Crosses(new Vector3(0,4,8),new Vector3(0,4,-8),3.5f),"Reverse goal missed");
  Require(!GoalCourse.Crosses(new Vector3(8,4,-8),new Vector3(8,4,8),3.5f),"Ball clipping post counted");
  Require(!GoalCourse.Crosses(new Vector3(0,14,-8),new Vector3(0,14,8),3.5f),"Ball clipping lintel counted");
  Require(!GoalCourse.Crosses(new Vector3(0,4,-8),new Vector3(0,4,-1),3.5f),"Near miss counted");
  var m=new GoalMemory();m.Complete(false,2,false);m.Complete(false,1,true);m.Complete(true,2,false);
  Require(m.Valid()&&m.playerGoals==3&&m.observedGoals==1&&m.creatureGoals==2&&m.practice>0,"Typed learning counts incorrect");m.observedGoals=4;Require(!m.Valid(),"Invented observation accepted");m.observedGoals=1;m.preference=float.NaN;Require(!m.Valid(),"Invalid preference accepted");
  var h=new HybridState();h.receipt.goal=true;Require(!h.Valid(),"Goal assigned to unrelated action");h.receipt.action=CreatureAction.PlayWithToy;Require(h.Valid(),"Goal lesson rejected");h.receipt.fetched=true;Require(!h.Valid(),"Fetch and goal conflated");
  Debug.Log("BWVR_GOALS_MODEL_PASS: swept full-ball crossing, misses, reverse direction, historical saves and typed bounded learning");
 }
 public static void Verify(Action<BeachBall,BallGoal> capture=null){
  Require(Application.isPlaying,"Enter Play mode");var slice=UnityEngine.Object.FindAnyObjectByType<QuestSlice>();slice.SetSanctuaryOpen(false);var god=slice.GetComponentInChildren<CreatureHybrid>();var ball=slice.GetComponentInChildren<BeachBall>();var course=ball.Goals;var gate=course.Gates[0];var lane=ball.Bowling;var nav=god.GetComponent<IslandNavigator>();float unit=nav.world.Scale;
  Require(course.Gates.All(g=>g.gameObject.activeSelf),"A default goal has no usable island site");
  var stateField=typeof(QuestSlice).GetField("state",Flags);var original=(SliceState)stateField.GetValue(slice);var held=(CarryItem[])typeof(QuestSlice).GetField("held",Flags).GetValue(slice);Require(held.All(i=>i==null),"Release held objects");
  var people=slice.GetComponentsInChildren<VillagerSocial>();var points=people.Select(p=>p.transform.position).ToArray();var godAt=god.transform.position;var godRotation=god.transform.rotation;var poses=course.Gates.Select(g=>g.transform.position).ToArray();var rotations=course.Gates.Select(g=>g.transform.rotation).ToArray();var cooldowns=god.Targets.Where(t=>t.affordance!=null).ToDictionary(t=>t.affordance,t=>t.affordance.cooldown);
  var test=new SliceState{developmentFocus=true,observeWithoutFoodWork=true,water=100,crops=100,hunger=.05f,playPreference=1};
  void Cancel(){god.Interrupt("Goal QA");foreach(var p in people){p.ResetTask();p.GetComponent<VillagerExploration>().ResetTask();p.GetComponent<VillagerResourceGathering>().Cancel();p.GetComponent<VillagerBlockPlay>().ResetTask();}ball.ReturnHome();}
  void Warm(){for(int i=0;i<12;i++){test.Hybrid.time+=.05f;ball.Advance(.05f,false);}}
  void Advance(int ticks=270){for(int i=0;i<ticks;i++){test.Hybrid.time+=.05f;ball.Advance(.05f,false);}}
  void Launch(float side=0,bool creature=false){ball.PlaceOnGround(lane.Start+lane.Right*side*unit);Warm();ball.Throw(lane.Forward,creature,18);ball.Body.linearVelocity=lane.Forward*18+Vector3.up*.1f;}
  void Observer(bool visible){god.transform.position=nav.world.ProjectFoot(lane.Start-lane.Right*(visible?12:100)*unit,nav.profile);god.transform.LookAt(gate.transform.position);nav.ResetRoute();Physics.SyncTransforms();}
  try{
   Cancel();stateField.SetValue(slice,test);god.NewSeason();foreach(var p in people)p.transform.position+=Vector3.right*unit*300;course.ResetWorld();gate.transform.rotation=Quaternion.LookRotation(lane.Forward);Require(gate.Place(lane.Target),"Cannot place goal on flat test lane");foreach(var pin in lane.Pins)pin.GetComponent<Collider>().enabled=false;Observer(true);Warm();Require(!course.TryAim(god.transform.position,out _),"Creature knew game before witnessing");
   // Exercise the actual controller carry/release route, including flat placement.
   var palette=slice.GetComponentInChildren<GodToolPalette>();palette.Cancel();palette.Process(new Ray(Camera.main.transform.position,Camera.main.transform.forward),false,true,false,.1f,false);
   Require((bool)typeof(QuestSlice).GetMethod("TryGrab",Flags).Invoke(slice,new object[]{1,gate.GetComponent<CarryItem>()}),"Native goal grab failed");ball.Advance(.05f,false);Require(!course.TryAim(god.transform.position,out _),"Held goal was aimable");
   typeof(QuestSlice).GetMethod("Release",Flags).Invoke(slice,new object[]{1,new Ray(lane.Target+Vector3.up*unit*30,Vector3.down)});Require(held[1]==null&&!gate.GetComponent<CarryItem>().held&&Mathf.Abs(gate.transform.position.y-nav.world.ProjectFoot(gate.transform.position,nav.profile).y)<unit*.1f,"Goal release floated above terrain");
   ball.PlaceOnGround(lane.Start);Warm();Require((bool)typeof(QuestSlice).GetMethod("TryGrab",Flags).Invoke(slice,new object[]{1,ball.GetComponent<CarryItem>()}),"Native ball grab failed");var motion=(ThrowMotion[])typeof(QuestSlice).GetField("throwMotion",Flags).GetValue(slice);var rig=(Transform)typeof(QuestSlice).GetField("rig",Flags).GetValue(slice);for(int i=0;i<4;i++)motion[1].Sample(lane.Start+lane.Forward*i*.02f*rig.localScale.x,i*.02f,rig.localScale.x);typeof(QuestSlice).GetMethod("Release",Flags).Invoke(slice,new object[]{1,new Ray(lane.Start,lane.Forward)});Require(held[1]==null&&ball.InFlight,"Native release did not launch ball");var before=ball.Body.position;test.paused=true;ball.Advance(1,true);Require(ball.Body.position==before&&test.WorldPlay.Goals.playerGoals==0,"Paused goal advanced");test.paused=false;Advance();Require(test.WorldPlay.Goals.playerGoals==1&&test.WorldPlay.Goals.observedGoals==1,"Physical player goal not witnessed/scored: "+test.WorldPlay.Goals.playerGoals+"/"+test.WorldPlay.Goals.observedGoals);
   Advance(30);Require(test.WorldPlay.Goals.playerGoals==1,"One launch counted twice");ball.CancelFetch();Observer(true);Require(course.TryAim(god.transform.position,out _),"Witnessed goal not an available aim");capture?.Invoke(ball,gate);
   // Miss a real post: collision must alter the launched trajectory and no goal may be awarded.
   Launch(11);bool bounced=false;for(int i=0;i<270;i++){test.Hybrid.time+=.05f;ball.Advance(.05f,false);bounced|=Vector3.Dot(ball.Body.linearVelocity,lane.Forward)<-1;}Require(bounced,"Physical post did not deflect ball");Require(test.WorldPlay.Goals.playerGoals==1,"Post strike counted as goal");
   Launch();for(int i=0;i<20;i++){test.Hybrid.time+=.05f;ball.Advance(.05f,false);}gate.GetComponent<CarryItem>().held=true;Advance(80);gate.GetComponent<CarryItem>().held=false;Require(test.WorldPlay.Goals.playerGoals==1,"Held/moving arch created a goal");gate.Place(lane.Target);
   Launch();Advance(35);ball.GetComponent<CarryItem>().held=true;ball.Advance(.05f,false);ball.GetComponent<CarryItem>().held=false;Advance();Require(test.WorldPlay.Goals.playerGoals==1,"Picked-up ball credited unfinished goal");
   // A physical crossing followed by an actual damage callback must discard the pending result.
   Launch();Advance(35);var harm=people[0].GetComponent<PlayImpactTarget>();ball.Impact(harm,10);Advance();Require(ball.Harmful&&test.WorldPlay.Goals.playerGoals==1,"Harmful shot earned goal credit");
   Launch();Observer(false);Advance();Require(test.WorldPlay.Goals.playerGoals==2&&test.WorldPlay.Goals.observedGoals==1,"Unwitnessed goal taught creature");
   Cancel();Observer(true);ball.PlaceOnGround(lane.Start);Warm();test.WorldPlay.Goals.preference=1;foreach(var a in cooldowns.Keys)if(a!=ball.GetComponent<CreatureAffordance>())a.cooldown=test.Hybrid.time+200;test.WorldPlay.Bowling.observedRolls=0;
   bool selected=false;for(int i=0;i<2200&&!test.Hybrid.receipt.goal;i++){god.Tick(.05f);ball.Advance(.05f,false);selected|=god.State.intention.active&&god.State.intention.target==ball.StableId;}
   Require(selected&&test.WorldPlay.Goals.creatureGoals>0&&test.Hybrid.receipt.goal,"Utility-selected creature did not score: "+god.Explanation+" phase "+god.GetComponent<CreatureCuriosity>().Current+" goals "+test.WorldPlay.Goals.creatureGoals+" journal "+string.Join(" | ",god.Journal.Capture().Select(e=>e.kind+":"+e.reason)));
   Require(test.Hybrid.receipt.partner.Length==0&&!test.Hybrid.receipt.gentleRally,"Goal attributed to a previous playmate");float pref=test.WorldPlay.Goals.preference;Require(god.Feedback(false)&&!god.Feedback(false)&&test.WorldPlay.Goals.preference<pref,"Goal feedback missing or repeatable");course.Capture();var saved=JsonUtility.FromJson<SliceState>(JsonUtility.ToJson(test));Require(saved.Valid()&&saved.WorldPlay.Goals.layout.Length==3,"Goal state failed save roundtrip");
   var savedAt=gate.transform.position;gate.transform.position+=lane.Right*unit*4;stateField.SetValue(slice,saved);god.NewSeason();ball.ResetWorld();Require(Vector3.Distance(gate.transform.position,savedAt)<unit*.1f,"Saved goal layout not restored");var first=saved.WorldPlay.Goals.layout[0];saved.WorldPlay.Goals.layout[0]=saved.WorldPlay.Goals.layout[1];saved.WorldPlay.Goals.layout[1]=first;saved.paused=true;course.ResetWorld();Require(Vector3.Distance(gate.transform.position,nav.world.ToWorld(saved.WorldPlay.Goals.layout[0].point))<unit*.1f,"Paused/swapped layout restore depended on old gate locations");saved.paused=false;
   Debug.Log("BWVR_GOALS_PLAY_PASS: native grab/drop, real ball crossing and post misses, witnessed learning, utility-selected creature goal, pause, held/moving cancellation, harm exclusion, once-only feedback and layout save/resume");
  }finally{
   foreach(var item in held)if(item!=null){item.held=false;item.GetComponent<Collider>().enabled=true;}held[0]=held[1]=null;Cancel();foreach(var pin in lane.Pins)pin.GetComponent<Collider>().enabled=true;stateField.SetValue(slice,original);god.NewSeason();ball.ResetWorld();for(int i=0;i<3;i++){course.Gates[i].transform.SetPositionAndRotation(poses[i],rotations[i]);course.Gates[i].GetComponent<CarryItem>().home=poses[i];course.Gates[i].GetComponent<CarryItem>().held=false;}foreach(var c in cooldowns)c.Key.cooldown=c.Value;god.transform.SetPositionAndRotation(godAt,godRotation);nav.ResetRoute();for(int i=0;i<people.Length;i++){people[i].transform.position=points[i];people[i].GetComponent<IslandNavigator>().ResetRoute();}Physics.SyncTransforms();
  }
 }
}
}
