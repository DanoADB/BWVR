using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace BWVR.Editor {
public static class CatchAudienceChecks {
 static readonly BindingFlags F=BindingFlags.Instance|BindingFlags.NonPublic;
 static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
 public static void Verify(Action<string,BeachBall,VillagerExploration> capture=null){
  var slice=UnityEngine.Object.FindAnyObjectByType<QuestSlice>();var god=slice.GetComponentInChildren<CreatureHybrid>();var nav=slice.GetComponent<IslandNavigation>();var viewers=slice.GetComponentsInChildren<VillagerExploration>().Take(4).ToArray();Check(viewers.Length==4,"Need four viewers");var good=viewers[0];var hidden=viewers[1];var carried=viewers[2];var worker=viewers[3];GameObject wall=null;BallLearning memory=null,unseen=null,held=null,busy=null;int offered=0,cheers=0;float initialDrive=0,confident=0;bool safeSeen=false,badSeen=false;
  void TickViewers(float dt,BeachBall ball){foreach(var v in viewers)if(v.Current==VillagerExploration.Phase.WatchBall)v.Tick(dt,ball.Playmate.Season.paused);}
  try{
   CatchGameChecks.VerifyRuntime((stage,ball)=>{
    if(stage=="offered"){
     offered++;foreach(var v in viewers){v.ResetTask();v.GetComponent<VillagerSocial>().ResetTask();v.GetComponent<VillagerRoutine>().ResetTask();v.GetComponent<CarryItem>().held=true;}
     if(offered>2)return;
     if(offered==2)god.State.time+=13; // Separate real learning receipts beyond the existing cooldown.
     if(wall==null){wall=new GameObject("QA private sight occluder");SceneManager.MoveGameObjectToScene(wall,(Scene)typeof(BeachBall).GetField("scene",F).GetValue(ball));wall.AddComponent<BoxCollider>().size=new Vector3(.4f,25,40);}
     var centre=god.transform.position;var forward=god.transform.forward;var right=god.transform.right;float u=nav.Scale;
     good.GetComponent<CarryItem>().held=false;good.transform.position=nav.ProjectFoot(centre+(right*16+forward*10)*u,nav.villager);good.transform.rotation=Quaternion.LookRotation(ball.transform.position-good.transform.position);
     hidden.GetComponent<CarryItem>().held=false;hidden.transform.position=nav.ProjectFoot(centre+(-right*16+forward*10)*u,nav.villager);hidden.transform.rotation=Quaternion.LookRotation(ball.transform.position-hidden.transform.position);
     carried.transform.position=nav.ProjectFoot(centre+(right*18+forward*12)*u,nav.villager);
     worker.GetComponent<CarryItem>().held=false;worker.transform.position=nav.ProjectFoot(centre+(right*20+forward*12)*u,nav.villager);worker.GetComponent<VillagerRoutine>().BeginCathedral();
     wall.transform.SetPositionAndRotation(nav.ToNav(centre+(-right*8+forward*10+Vector3.up*8)*u),Quaternion.LookRotation(forward));Physics.SyncTransforms();
     memory=good.GetComponent<VillagerSocial>().Memory.BallLesson;unseen=hidden.GetComponent<VillagerSocial>().Memory.BallLesson;held=carried.GetComponent<VillagerSocial>().Memory.BallLesson;busy=worker.GetComponent<VillagerSocial>().Memory.BallLesson;if(offered==1)initialDrive=good.GetComponent<VillagerSocial>().Memory.PeerPlayDrive;
     Check(!ball.HasWatchableGame&&!good.WitnessCatchRoll(ball),"A tossed ball fabricated a completed show");return;
    }
    if(stage.EndsWith("-caught")){
     Check(good.Current==VillagerExploration.Phase.WatchBall,"Visible free villager did not autonomously watch: "+good.Current+" sight="+ball.VisibleTo(good)+" idle="+good.GetComponent<VillagerRoutine>().AvailableForBell+" show="+ball.CatchShowActive+" gate="+typeof(VillagerExploration).GetProperty("CanStartStructure",F).GetValue(good));Check(hidden.Current!=VillagerExploration.Phase.WatchBall&&carried.Current!=VillagerExploration.Phase.WatchBall&&worker.Current!=VillagerExploration.Phase.WatchBall,"Occluded/carried/working villager was commandeered");Check(!good.CheeringCatch&&!good.WitnessCatchRoll(ball)&&!good.WitnessBallOutcome(ball,true,"creature"),"Anticipation produced applause or a safe lesson");return;
    }
    if(!stage.EndsWith("-rolled"))return;
    Check(good.CheeringCatch&&!good.WitnessCatchRoll(ball),"Full witnessed roll did not cheer exactly once");cheers++;capture?.Invoke(stage,ball,good);Check(!hidden.CheeringCatch&&!carried.CheeringCatch&&!worker.CheeringCatch,"Applause leaked to nonwitnesses");
    if(offered==1){
     Check(memory.seenGood==0,"Launch alone claimed a safe outcome");bool paused=god.Season.paused;god.Season.paused=true;string before=JsonUtility.ToJson(god.Season);TickViewers(2,ball);ball.Advance(2,true);Check(before==JsonUtility.ToJson(god.Season)&&good.CheeringCatch,"Pause advanced lesson/cheer");god.Season.paused=paused;
     for(int i=0;i<220&&ball.CatchShowActive;i++){ball.Advance(.02f,false);god.Tick(.02f);TickViewers(.02f,ball);}Check(!ball.CatchShowActive&&memory.seenGood==1&&memory.seenBad==0&&memory.demonstrator=="creature","Completed harm-free return did not teach one local lesson");confident=memory.confidence;Check(good.GetComponent<VillagerSocial>().Memory.PeerPlayDrive>initialDrive,"Observed play did not affect peer-play willingness");safeSeen=true;capture?.Invoke("safe-memory",ball,good);
    }else{
     var target=((System.Collections.Generic.List<PlayImpactTarget>)typeof(BeachBall).GetField("targets",F).GetValue(ball)).First(t=>t.kind=="tree");ball.Impact(target,10);Check(!ball.CatchShowActive&&memory.seenGood==1&&memory.seenBad==1&&memory.confidence<confident&&!good.CheeringCatch,"Confirmed harmful-impact receipt did not correct confidence/stop cheering");badSeen=true;capture?.Invoke("harm-memory",ball,good);
    }
    int lessons=memory.seenGood+memory.seenBad;ball.FinishExperience();TickViewers(.02f,ball);Check(memory.seenGood+memory.seenBad==lessons&&!ball.HasWatchableGame,"Outcome replay taught twice or retained spectators");
   },TickViewers,loaded=>{int id=int.Parse(good.GetComponent<HybridTarget>().id.Substring("villager-".Length));var saved=loaded.Social(id).ballLearning;Check(saved!=null&&saved.seenGood==1&&saved.seenBad==1&&saved.confidence==memory.confidence,"Native reload lost individual audience memory");});
   Check(safeSeen&&badSeen&&cheers==2&&memory.seenGood==1&&memory.seenBad==1&&unseen.seenGood+unseen.seenBad+held.seenGood+held.seenBad+busy.seenGood+busy.seenBad==0,"Mixed local audience memory invalid");Check(JsonUtility.FromJson<BallLearning>(JsonUtility.ToJson(memory)).Valid(),"Audience learning failed save round trip");
   Debug.Log("BWVR_CATCH_AUDIENCE_PASS: native toss/utility/physical catch; free visible spectator versus private occluder/player carry/construction; witnessed clap once; pause; actual harm-free flight; staged harmful-impact receipt; confidence/peer-drive; duplicate and saved learning guards");
  }finally{if(wall!=null)UnityEngine.Object.DestroyImmediate(wall);foreach(var v in viewers){v.ResetTask();v.GetComponent<VillagerRoutine>().ResetTask();}}
 }
}
}
