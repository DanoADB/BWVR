using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
namespace BWVR.Editor {
public static class ObservationChecks {
 [MenuItem("BWVR/Verify observational learning model")]
 public static void Model(){var memory=new SocialMemory();int Count(){int c=0;for(int i=0;i<500;i++)if(memory.WantsPeerPlay(1347,i,"observer"))c++;return c;}int neutral=Count();for(int i=0;i<5;i++)memory.BallLesson.Observe(true,"teacher",i*20);int learned=Count();if(learned<neutral+100)throw new Exception("Observed success has no meaningful imitation bias");if(memory.BallLesson.Observe(true,"teacher",80))throw new Exception("Repeated observation rewarded");for(int i=0;i<6;i++)memory.BallLesson.Observe(false,"teacher",100+i*20);if(Count()>=neutral)throw new Exception("Observed risk does not reduce interest");var restored=JsonUtility.FromJson<SocialMemory>(JsonUtility.ToJson(memory));if(!restored.Valid()||restored.BallLesson.seenGood!=5||!JsonUtility.FromJson<SocialMemory>("{}").Valid())throw new Exception("Observation/legacy save failed");restored.BallLesson.confidence=float.NaN;if(restored.Valid())throw new Exception("Invalid confidence accepted");Debug.Log("BWVR_OBSERVATION_MODEL_PASS: peer-play choices "+neutral+" -> "+learned+"/500 after witnessed success, risk response, deduplication and save validation");}
 [MenuItem("BWVR/Verify visible observation and imitation (Play mode)")]
 public static void Verify(){
  if(!EditorApplication.isPlaying)throw new Exception("Enter Play mode");Model();var ball=UnityEngine.Object.FindAnyObjectByType<BeachBall>();var god=ball.Playmate;var slice=UnityEngine.Object.FindAnyObjectByType<QuestSlice>();slice.SendMessage("OnApplicationPause",false);god.Interrupt("Observation QA");var field=typeof(QuestSlice).GetField("state",BindingFlags.Instance|BindingFlags.NonPublic);var original=(SliceState)field.GetValue(slice);var test=new SliceState{water=100,crops=100,developmentFocus=true};test.Hybrid.time=200;field.SetValue(slice,test);
  var people=slice.GetComponentsInChildren<VillagerExploration>();var positions=people.Select(p=>p.transform.position).ToArray();var a=people[0];var b=people[1];var observer=people[2];var nav=a.GetComponent<IslandNavigator>();float scale=nav.world.Scale;GameObject wall=null;
  void Reset(){foreach(var p in people){p.ResetTask();p.GetComponent<VillagerSocial>().ResetTask();p.GetComponent<VillagerBlockPlay>().ResetTask();p.GetComponent<CarryItem>().held=false;p.GetComponent<VillagerSocial>().Memory.nextPeerPlayAt=0;p.GetComponent<VillagerSocial>().Memory.injury=0;}ball.ReturnHome();}
  bool Stage(){for(int i=0;i<16;i++){Reset();var d=Quaternion.Euler(0,i*22.5f,0)*Vector3.forward;var center=ball.transform.position;if(!nav.world.Sample(center-d*scale*6,nav.profile,2,out var p)||!nav.world.Sample(center+d*scale*20,nav.profile,2,out var q)||!nav.world.Sample(center+Quaternion.Euler(0,90,0)*d*scale*12,nav.profile,2,out var v))continue;p=nav.world.ProjectFoot(p,nav.profile);q=nav.world.ProjectFoot(q,nav.profile);if(Mathf.Abs(q.y-(center.y-ball.Radius*scale))>scale*1.1f)continue;a.transform.position=p;b.transform.position=q;observer.transform.position=nav.world.ProjectFoot(v,nav.profile);foreach(var actor in people)actor.GetComponent<IslandNavigator>().ResetRoute();if(ball.TryPeerGame(a,b))return true;}return false;}
  void Step(){test.Hybrid.time+=.02f;a.Tick(.02f,false);b.Tick(.02f,false);observer.Tick(.02f,false);ball.Advance(.02f,false);}
  try{
   if(!Stage())throw new Exception("No observation fixture");ball.Advance(.02f,false);if(!ball.VisibleTo(observer)||!observer.TryWatchBall(ball))throw new Exception("Nearby spectator could not watch physical play");
   // Native private-scene raycast must reject a real intervening collider.
   wall=new GameObject("Observation test occluder");SceneManager.MoveGameObjectToScene(wall,ball.Body.gameObject.scene);wall.transform.position=(nav.world.ToNav(observer.transform.position+Vector3.up*scale*2.6f)+ball.Body.position)*.5f;wall.AddComponent<BoxCollider>().size=new Vector3(2,6,2);Physics.SyncTransforms();if(ball.VisibleTo(observer))throw new Exception("Observer saw through a blocker");UnityEngine.Object.DestroyImmediate(wall);wall=null;Physics.SyncTransforms();
   var lesson=observer.GetComponent<VillagerSocial>().Memory.BallLesson;test.paused=true;if(observer.WitnessBallOutcome(ball,true,"teacher"))throw new Exception("Paused observation learned");test.paused=false;
   if(observer.WitnessBallOutcome(ball,true,"teacher"))throw new Exception("Instant glance learned a skill");
   for(int i=0;i<4000&&a.GetComponent<VillagerSocial>().Memory.peerGames==0;i++)Step();
   if(ball.RallyReceptions<4||lesson.seenGood!=1||lesson.practicedGames!=0||lesson.demonstrator!=b.GetComponent<HybridTarget>().id)throw new Exception("Confirmed observed exchange did not teach exactly once: receptions="+ball.RallyReceptions+", seen="+lesson.seenGood+", phase="+observer.Current);
   if(a.GetComponent<VillagerSocial>().Memory.BallLesson.practicedGames!=1||b.GetComponent<VillagerSocial>().Memory.BallLesson.practicedGames!=1||observer.GetComponent<VillagerSocial>().Memory.peers.Length!=0||test.Hybrid.bonds.Length!=0)throw new Exception("Observation confused with practice, friendship or creature teaching");
   if(observer.WitnessBallOutcome(ball,true,"teacher")||lesson.seenGood!=1)throw new Exception("Outcome learned twice");
   int seen=lesson.seenGood;observer.GetComponent<CarryItem>().held=true;observer.Tick(.02f,false);if(observer.WitnessBallOutcome(ball,true,"teacher")||observer.Active)throw new Exception("Carried observer kept learning");observer.GetComponent<CarryItem>().held=false;
   Reset();test.Hybrid.time+=20;if(!Stage())throw new Exception("Could not stage negative observation");ball.Advance(.02f,false);if(!observer.TryWatchBall(ball))throw new Exception("Observer unavailable");for(int i=0;i<200;i++){test.Hybrid.time+=.02f;observer.Tick(.02f,false);}float confidence=lesson.confidence;
   // Observation outcome boundary: existing BeachBallChecks verifies native impact generation.
   if(!observer.WitnessBallOutcome(ball,false,a.GetComponent<HybridTarget>().id)||lesson.confidence>=confidence||lesson.seenBad!=1||lesson.seenGood!=seen)throw new Exception("Witnessed bad outcome did not teach caution");
   var save=JsonUtility.FromJson<SliceState>(JsonUtility.ToJson(test));if(!save.Valid()||save.Social(2).BallLesson.seenGood!=1||save.Social(2).BallLesson.seenBad!=1)throw new Exception("Observed behavior did not persist");
   Debug.Log("BWVR_OBSERVATION_PASS: native visible four-reception exchange teaches one spectator; real collider occlusion; minimum watching time; pause/carry gates; actual demonstrator; practice vs observation; injected bad-outcome response and persistence");
  }finally{if(wall!=null)UnityEngine.Object.DestroyImmediate(wall);god.Interrupt("Observation QA complete");for(int i=0;i<people.Length;i++){people[i].ResetTask();people[i].GetComponent<VillagerSocial>().ResetTask();people[i].GetComponent<CarryItem>().held=false;people[i].transform.position=positions[i];people[i].GetComponent<IslandNavigator>().ResetRoute();}field.SetValue(slice,original);ball.ResetWorld();}
 }
}
}
