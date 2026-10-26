using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace BWVR.Editor {
public static class CreatureResponseChecks {
 static readonly BindingFlags Flags=BindingFlags.Instance|BindingFlags.NonPublic;
 static void Require(bool ok,string why){if(!ok)throw new Exception(why);}
 [MenuItem("BWVR/Verify outcome evidence model")]
 public static void VerifyModel(){
  var legacy=JsonUtility.FromJson<HybridState>("{}");Require(legacy.Valid()&&legacy.Evidence.events.Length==0,"Legacy save gained fabricated history");
  var history=new OutcomeLedger();for(int i=0;i<140;i++)history.Add(i,EvidenceKind.ActionCompleted,EvidenceSource.Autonomous,CreatureAction.PlayWithToy,"ball","Confirmed native completion");
  Require(history.events.Length==96&&history.events[0].id==45&&history.completedCreatureActions==140&&history.Valid(140),"History is not bounded/ordered");
  var saved=JsonUtility.FromJson<OutcomeLedger>(JsonUtility.ToJson(history));Require(saved.Valid(140)&&saved.nextId==141&&saved.completedCreatureActions==140,"Evidence reload changed counts/identity");
  saved.events[0].time=float.NaN;Require(!saved.Valid(140),"Nonfinite evidence accepted");saved=JsonUtility.FromJson<OutcomeLedger>(JsonUtility.ToJson(history));saved.events[1].id=saved.events[0].id;Require(!saved.Valid(140),"Duplicate evidence IDs accepted");
  saved=JsonUtility.FromJson<OutcomeLedger>(JsonUtility.ToJson(history));saved.events[0].actor="player";Require(!saved.Valid(140),"Player attributed creature action accepted");
  saved=JsonUtility.FromJson<OutcomeLedger>(JsonUtility.ToJson(history));saved.events[0].relatedId=saved.events[0].id;Require(!saved.Valid(140),"Forward/self evidence link accepted");
  saved=JsonUtility.FromJson<OutcomeLedger>(JsonUtility.ToJson(history));saved.events[0].source=EvidenceSource.PlayerMiracle;Require(!saved.Valid(140),"Miracle masqueraded as creature work");
  var season=new SliceState();season.Hybrid.intention.evidenceId=77;Require(!season.Valid(),"Orphaned action evidence accepted");
  Debug.Log("BWVR_RESPONSE_MODEL_PASS: legacy empty history; 96 records; monotonic identities; saved counts; corrupt time/IDs/actor/links/source rejected");
 }
 [MenuItem("BWVR/Verify creature responses and evidence (Play mode)")]
 public static void Verify(){
  Require(Application.isPlaying,"Enter Play mode first");VerifyModel();
  var slice=UnityEngine.Object.FindAnyObjectByType<QuestSlice>();var god=slice.GetComponentInChildren<CreatureHybrid>();var mind=god.GetComponent<CreatureCuriosity>();var nav=god.GetComponent<IslandNavigator>();var hands=god.GetComponentInChildren<HumanoidHands>();
  var stateField=typeof(QuestSlice).GetField("state",Flags);var original=(SliceState)stateField.GetValue(slice);var position=god.transform.position;var rotation=god.transform.rotation;
  var toy=god.Targets.First(t=>t.GetComponent<SharedRhythm>()!=null);var item=toy.GetComponent<CarryItem>();var toyAt=toy.transform.position;bool held=item.held;
  var spot=god.Find(GodToolPalette.SpotId);var spotAt=spot.transform.position;GameObject blocker=null;
  var test=new SliceState{developmentFocus=true,water=100,hunger=.1f};god.Interrupt("Response QA setup");stateField.SetValue(slice,test);god.NewSeason();
  try{
   var yard=slice.GetComponentsInChildren<ConstructionSite>().First(s=>!s.IsBridge);
   Require(nav.world.Sample(yard.transform.position+Vector3.forward*nav.world.Scale*18,nav.profile,20,out var safe),"No response test stage");
   god.transform.position=nav.world.ProjectFoot(safe,nav.profile);god.transform.rotation=Quaternion.LookRotation(yard.transform.position-god.transform.position);nav.ResetRoute();
   float unit=nav.world.Scale;item.held=false;bool clear=false;
   // Loose blocks legitimately occlude the yard. Find an actually visible stage.
   for(int i=0;i<16;i++){god.transform.rotation=Quaternion.Euler(0,i*22.5f,0);toy.transform.position=god.transform.position+god.transform.forward*unit*10+Vector3.up*unit;if(god.CanSeeCue(toy)&&god.CanReach(toy)){clear=true;break;}}
   Require(clear,"No unobstructed response test stage");Vector3 facing=god.transform.forward;test.Hybrid.time=1;
   Require(god.NoticePlaced(item)&&god.Response==CreatureResponse.Noticed&&god.Owner==CreatureHybrid.ActionOwner.None,"Visible placement did not get a noncommand acknowledgment");
   int notices=test.Hybrid.Evidence.events.Count(e=>e.kind==EvidenceKind.Noticed);Require(notices==1&&test.Hybrid.associations.Length==0,"Notice fabricated learning/history");
   test.Hybrid.time+=1;item.held=true;Require(!god.NoticePlaced(item),"Held placement acknowledged");item.held=false;
   toy.transform.position=god.transform.position-god.transform.forward*unit*10;Require(!god.NoticePlaced(item),"Rear placement acknowledged");
   toy.transform.position=god.transform.position+god.transform.forward*unit*10+Vector3.up*unit;
   blocker=new GameObject("Temporary response occluder");blocker.transform.position=(god.transform.position+Vector3.up*unit*10+toy.transform.position)*.5f;blocker.AddComponent<BoxCollider>().size=Vector3.one*unit*7;
   Require(!god.NoticePlaced(item)&&test.Hybrid.Evidence.events.Count(e=>e.kind==EvidenceKind.Noticed)==notices,"Occluded cue received acknowledgment");
   Require(nav.world.Sample(toy.transform.position,nav.profile,8,out var point),"No ground cue stage");spot.transform.position=nav.world.ProjectFoot(point,nav.profile);
   Require(god.Guide(GuidanceIntent.Investigate,toy)&&!test.Hybrid.guidance.noticed,"Occluded suggestion falsely noticed");
   UnityEngine.Object.DestroyImmediate(blocker);blocker=null;god.Tick(.01f);Require(test.Hybrid.guidance.noticed,"Revealed suggestion not perceived");
   god.Interrupt("Busy stage");var other=god.Find("cistern");Begin(god,other,0);god.transform.rotation=Quaternion.LookRotation(toy.transform.position-god.transform.position);
   Require(god.Guide(GuidanceIntent.Investigate,toy),"Busy suggestion rejected instead of weighed");var owner=god.Owner;var intention=test.Hybrid.intention;
   god.Tick(1.35f);Require(god.Response==CreatureResponse.Busy&&god.Owner==owner&&ReferenceEquals(intention,test.Hybrid.intention),"Acknowledgment stole action ownership");
   float time=test.Hybrid.time;int count=test.Hybrid.Evidence.events.Length;test.paused=true;god.Tick(20);Require(test.Hybrid.time==time&&test.Hybrid.Evidence.events.Length==count&&god.Response==CreatureResponse.Busy,"Pause advanced response/history");test.paused=false;
   mind.Tick(.01f,test,false);mind.Feedback(false,false);test.Hybrid.time+=1;god.transform.rotation=Quaternion.LookRotation(toy.transform.position-god.transform.position);god.NoticePlaced(item);Require(mind.Affect==CreatureAffect.Mood.Sad,"Cue overwrote player feedback");
   god.Interrupt("Chosen stage");god.transform.position=nav.world.ProjectFoot(safe,nav.profile);god.transform.rotation=Quaternion.LookRotation(facing);nav.ResetRoute();
   Require(nav.world.Sample(god.transform.position+god.transform.forward*unit*4,nav.profile,8,out point),"No completion stage");spot.transform.position=nav.world.ProjectFoot(point,nav.profile);god.transform.rotation=Quaternion.LookRotation(spot.transform.position-god.transform.position);
   Require(god.Guide(GuidanceIntent.Investigate,toy),"Chosen cue unavailable");Begin(god,toy,.95f);Require(god.Response==CreatureResponse.Chosen&&test.Hybrid.intention.source==EvidenceSource.PlayerGuidance,"Choice not distinguished from notice");
   int actionId=test.Hybrid.intention.evidenceId;god.Tick(.25f);Require(hands!=null&&Mathf.Abs(hands.attentionPose.y)>.01f&&Mathf.Abs(hands.attentionPose.x)<.001f,"Chosen response did not drive head-only nod");
   int completions=test.Hybrid.Evidence.completedCreatureActions;
   for(int i=0;i<300&&test.Hybrid.Evidence.completedCreatureActions==completions;i++)god.Tick(.1f);
   Require(test.Hybrid.Evidence.completedCreatureActions==completions+1&&god.Response==CreatureResponse.Completed,"Native observation did not confirm completion");
   var evidence=test.Hybrid.Evidence.events.Last(e=>e.kind==EvidenceKind.ActionCompleted);Require(evidence.actor=="creature"&&evidence.source==EvidenceSource.PlayerGuidance&&evidence.relatedId==actionId&&evidence.target==toy.id,"Completion attribution lost");
   typeof(CreatureHybrid).GetMethod("Complete",Flags).Invoke(god,null);Require(test.Hybrid.Evidence.completedCreatureActions==completions+1,"Duplicate completion recorded");
   var restored=JsonUtility.FromJson<SliceState>(JsonUtility.ToJson(test));Require(restored.Valid(),"Evidence state round trip invalid");stateField.SetValue(slice,restored);test=restored;god.NewSeason();Require(test.Hybrid.Evidence.completedCreatureActions==completions+1,"Restore replayed a completion");
   Begin(god,spot,0);int interrupted=test.Hybrid.Evidence.events.Count(e=>e.kind==EvidenceKind.ActionInterrupted);god.Interrupt("Target unavailable");god.Interrupt("Duplicate cleanup");Require(test.Hybrid.Evidence.events.Count(e=>e.kind==EvidenceKind.ActionInterrupted)==interrupted+1&&test.Hybrid.Evidence.completedCreatureActions==completions+1&&!test.Hybrid.receipt.eligible,"Interruption became success or duplicated");
   Begin(god,toy,0);toy.gameObject.SetActive(false);god.Tick(.1f);toy.gameObject.SetActive(true);Require(god.Response==CreatureResponse.Unavailable&&!test.Hybrid.receipt.eligible,"Unavailable target did not produce a safe failure response");
   test.Hybrid.time+=1;test.water=20;var miracles=slice.GetComponentInChildren<DivineIntervention>();Require(miracles.TryUse(SpiritualTool.Rain,god.Find("cistern")),"Miracle stage failed");
   evidence=test.Hybrid.Evidence.events.Last();Require(evidence.kind==EvidenceKind.Miracle&&evidence.actor=="player"&&test.Hybrid.Evidence.playerMiracles==1&&test.Hybrid.Evidence.completedCreatureActions==completions+1&&!god.Feedback(true)&&test.Hybrid.associations.Length==0,"Miracle entered creature learning/achievements");
   Require(test.Valid(),"Final response state invalid");
   Debug.Log("BWVR_RESPONSE_RUNTIME_PASS: visible/occluded/rear/held cues; noncommand notice; delayed busy; exclusive owner; paused clocks; feedback priority; head-only nod; native completion once; attributed saved history; interruption once; miracle separated from learning");
  }finally{
   if(blocker!=null)UnityEngine.Object.DestroyImmediate(blocker);god.Interrupt("Response QA complete");stateField.SetValue(slice,original);god.transform.SetPositionAndRotation(position,rotation);toy.gameObject.SetActive(true);toy.transform.position=toyAt;item.held=held;spot.transform.position=spotAt;nav.ResetRoute();god.NewSeason();
  }
 }
 static void Begin(CreatureHybrid god,HybridTarget target,float guidance){typeof(CreatureHybrid).GetMethod("BeginAction",Flags).Invoke(god,new object[]{new CreatureCandidate{action=CreatureAction.InvestigateObject,situation=CreatureSituation.PlayOpportunity,target=target.id,guidance=guidance,timeout=90,expected="Inspect target"},target});}
}
}
