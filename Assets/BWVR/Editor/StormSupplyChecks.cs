using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
namespace BWVR.Editor {
public static class StormSupplyChecks {
 static readonly BindingFlags F=BindingFlags.Instance|BindingFlags.NonPublic;
 static void Check(bool value,string reason){if(!value)throw new Exception(reason);}
 public static void VerifyModel(){
  var legacy=JsonUtility.FromJson<StormChapterState>("{}");Check(legacy.Valid()&&(legacy.supply==null||!legacy.supply.spawned),"Historical chapter invented supplies");
  var s=new StormSupplyState{spawned=true};Check(s.Valid(),"Pending supplies invalid");s.delivered=true;Check(!s.Valid(),"Delivery without attribution accepted");s.source=RescueSource.Independent;Check(!s.Valid(),"Unsupported independent supply award");s.source=RescueSource.Creature;Check(s.Valid(),"Valid delivery rejected");s.destination.z=float.NaN;Check(!s.Valid(),"Nonfinite destination accepted");
  Debug.Log("BWVR_STORM_SUPPLY_MODEL_PASS: backward compatibility, finite points and valid attribution");
 }
 public static void Verify(Action<StormChapter> capture=null){
  Check(Application.isPlaying,"Enter Play mode");var slice=UnityEngine.Object.FindAnyObjectByType<QuestSlice>();slice.SetSanctuaryOpen(false);
  var chapter=slice.Chapter;var god=slice.GetComponentInChildren<CreatureHybrid>();var mind=god.GetComponent<CreatureCuriosity>();var nav=god.GetComponent<IslandNavigator>().world;
  Check(!nav.TreeNavigationPending,"Wait for navigation updates");var resources=slice.GetComponentInChildren<VillageResources>();var weather=slice.GetComponentInChildren<IslandWeather>();bool auto=weather.automatic;
  var stateField=typeof(QuestSlice).GetField("state",F);var original=(SliceState)stateField.GetValue(slice);var held=(CarryItem[])typeof(QuestSlice).GetField("held",F).GetValue(slice);Check(held.All(i=>i==null),"Release held objects first");
  var gp=god.transform.position;var gr=god.transform.rotation;var ideas=slice.GetComponentsInChildren<ConstructionInspiration>();var enabled=ideas.Select(i=>i.gameObject.activeSelf).ToArray();var cooldowns=god.Targets.Where(t=>t.affordance!=null).ToDictionary(t=>t.affordance,t=>t.affordance.cooldown);SliceState test=null;
  void Tick(float dt,bool ai){if(!test.paused)test.Hybrid.time+=dt;chapter.Advance(dt,test.paused);if(ai)god.Tick(dt,false);}
  void Stage(){
   god.Interrupt("Supply QA stage");test=new SliceState{developmentFocus=true,observeWithoutFoodWork=true,water=100,hunger=.05f};stateField.SetValue(slice,test);chapter.Advance(0,false);god.NewSeason();
   foreach(var i in ideas)i.gameObject.SetActive(false);foreach(var c in cooldowns)c.Key.cooldown=100000;
   var palette=slice.GetComponentInChildren<GodToolPalette>();palette.Cancel();palette.Process(new Ray(Camera.main.transform.position,Camera.main.transform.forward),false,true,false,.1f,false);
   Check(chapter.Start(),"Chapter could not start");chapter.State.elapsed=StormChapterState.Duration(StormPhase.Calm)-.01f;Tick(.02f,false);
   for(int k=0;k<250&&chapter.Supply?.spawned!=true;k++)Tick(.05f,false);
   Check(chapter.SupplyPending&&chapter.SupplyItem.CanGrab,"Timber not spawned in warning");
   Check(resources.State.wood==0,"Stranding minted timber before delivery");
   // Choose a clear point from which the actual timber is visible and reachable.
   bool visible=false;for(int k=0;k<32&&!visible;k++){var wanted=chapter.SupplyItem.transform.position+Quaternion.Euler(0,k*137.5f,0)*Vector3.forward*nav.Scale*12;if(!nav.Sample(wanted,nav.creature,3,out var at))continue;god.transform.position=nav.ProjectFoot(at,nav.creature);god.transform.LookAt(new Vector3(chapter.SupplyItem.transform.position.x,god.transform.position.y,chapter.SupplyItem.transform.position.z));Physics.SyncTransforms();visible=god.CanSeeCue(chapter.SupplyItem.GetComponent<HybridTarget>(),false,true);}
   Check(visible&&god.CanReach(chapter.SupplyDestination),"No clear timber sightline / delivery route");god.GetComponent<IslandNavigator>().ResetRoute();
  }
  void PlayerDeliver(){
   var item=chapter.SupplyItem;Check((bool)typeof(QuestSlice).GetMethod("TryGrab",F).Invoke(slice,new object[]{1,item}),"Native timber pickup failed");
   typeof(QuestSlice).GetMethod("Release",F).Invoke(slice,new object[]{1,new Ray(chapter.SupplyDestination+Vector3.up*nav.Scale*30,Vector3.down)});
  }
  try{
   Stage();var state=JsonUtility.ToJson(chapter.State);test.paused=true;Tick(1,true);Check(JsonUtility.ToJson(chapter.State)==state&&!chapter.ClaimSupply(),"Pause advanced supplies / allowed creature ownership");test.paused=false;
   resources.State.wood=resources.State.Limit(VillageResource.Wood);Check(!chapter.SupplyFeasible&&!god.CanGuide(GuidanceIntent.Help,chapter.SupplyItem.GetComponent<HybridTarget>(),out _),"Full storage offered unusable retrieval");
   PlayerDeliver();Check(!chapter.Supply.delivered&&resources.State.wood==resources.State.Limit(VillageResource.Wood),"Full store consumed / overfilled bundle");
   resources.State.wood=0;PlayerDeliver();Check(chapter.Supply.delivered&&chapter.Supply.source==RescueSource.Player&&resources.State.wood==6&&!test.Hybrid.receipt.eligible,"Player supply delivery misattributed");Check(!chapter.ConfirmSupply(RescueSource.Player)&&resources.State.wood==6&&!chapter.SupplyItem.CanGrab,"Duplicate supply awarded");
   Stage();test.paused=true;PlayerDeliver();Check(chapter.Supply.playerArrivalPending&&!chapter.Supply.delivered&&resources.State.wood==0,"Paused placement applied delivery");test.paused=false;Tick(.05f,false);Check(chapter.Supply.delivered&&chapter.Supply.source==RescueSource.Player,"Paused player source lost");
   Stage();Check(god.Guide(GuidanceIntent.Investigate,chapter.SupplyItem.GetComponent<HybridTarget>()),"Supply investigation rejected");god.Perceive();var offers=(System.Collections.Generic.List<CreatureCandidate>)typeof(CreatureHybrid).GetField("candidates",F).GetValue(god);Check(offers.Any(c=>c.target==StormChapter.SupplyId&&c.action==CreatureAction.InvestigateObject)&&offers.Where(c=>c.target==StormChapter.SupplyId&&c.action==CreatureAction.RespondToGuidance).All(c=>c.guidance==0),"Investigation misattributed as retrieval guidance");Check(god.Guide(GuidanceIntent.Help,chapter.SupplyItem.GetComponent<HybridTarget>()),"Help rejected useful timber");bool carried=false,selected=false;for(int k=0;k<2400&&!chapter.Supply.delivered;k++){Tick(.05f,true);selected|=mind.Current==CreatureCuriosity.Phase.StormSupply;carried|=chapter.SupplyItem.held;if(carried&&capture!=null){capture(chapter);capture=null;}}
   Check(selected&&carried&&chapter.Supply.delivered&&chapter.Supply.source==RescueSource.Creature&&resources.State.wood==6,"Creature did not retrieve/place timber: "+mind.Current+" / "+god.Explanation);
   Check(test.Hybrid.receipt.eligible&&test.Hybrid.receipt.action==CreatureAction.RespondToGuidance&&test.Hybrid.receipt.situation==CreatureSituation.HelpRequested&&test.Hybrid.receipt.target==StormChapter.SupplyId&&!test.Hybrid.receipt.fetched,"Useful retrieval mislabeled as ball game / missing contextual receipt");Check(god.Feedback(true)&&!god.Feedback(true),"Timber reinforced twice");Check(JsonUtility.FromJson<SliceState>(JsonUtility.ToJson(test)).Valid(),"Delivered supply save invalid");
   Stage();Check(god.Guide(GuidanceIntent.Help,chapter.SupplyItem.GetComponent<HybridTarget>()),"Resume cue rejected");for(int k=0;k<900&&!chapter.SupplyItem.held;k++)Tick(.05f,true);Check(chapter.SupplyItem.held,"No carried timber to save");
   typeof(QuestSlice).GetMethod("Save",F).Invoke(slice,null);var saved=JsonUtility.FromJson<SliceState>(System.IO.File.ReadAllText(System.IO.Path.Combine(Application.persistentDataPath,"bwvr-season-v1.json")));Check(saved.Valid()&&saved.StormChapter.supply.spawned&&!saved.StormChapter.supply.delivered,"Pending supply save invalid");var grounded=saved.StormChapter.supply.grounded;
   god.Interrupt("Reload carried supply");typeof(QuestSlice).GetMethod("Load",F).Invoke(slice,null);test=(SliceState)stateField.GetValue(slice);god.NewSeason();Tick(.01f,false);
   Check(!chapter.SupplyClaimed&&!chapter.SupplyItem.held&&Vector3.Distance(chapter.SupplyItem.transform.position,nav.ToWorld(grounded))<nav.Scale*3,"Reload retained carry ownership or lost grounded supply");PlayerDeliver();Check(chapter.Supply.delivered&&resources.State.wood==6,"Resumed supply not deliverable");
   Stage();Check(god.Guide(GuidanceIntent.Help,chapter.SupplyItem.GetComponent<HybridTarget>()),"Stop cue rejected");for(int k=0;k<900&&!chapter.SupplyItem.held;k++)Tick(.05f,true);Check(chapter.SupplyItem.held,"Stop fixture did not carry supply");test.paused=true;Check(chapter.Stop()&&!chapter.SupplyClaimed&&!chapter.SupplyItem.held&&resources.State.wood==0&&!test.Hybrid.receipt.eligible,"Stopping granted reward / stranded ownership");
   Debug.Log("BWVR_STORM_SUPPLY_PLAY_PASS: warning spawn, native pickup/drop, capacity rejection, once-only six-wood benefit, paused attribution, utility-selected carry/delivery, contextual teaching, grounded native Save/Load and stop cleanup");
  }finally{
   held[0]=held[1]=null;god.Interrupt("Supply QA complete");stateField.SetValue(slice,original);chapter.Advance(0,false);weather.automatic=auto;god.NewSeason();resources.Refresh();foreach(var c in cooldowns)c.Key.cooldown=c.Value;for(int i=0;i<ideas.Length;i++)ideas[i].gameObject.SetActive(enabled[i]);god.transform.SetPositionAndRotation(gp,gr);god.GetComponent<IslandNavigator>().ResetRoute();Physics.SyncTransforms();
  }
 }
}
}
