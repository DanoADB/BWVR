using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace BWVR.Editor {
public static class SharedActivitiesChecks {
 [MenuItem("BWVR/Verify play invitations and shared rhythm (Play mode)")]
 public static void Verify(){VerifyWithCapture(null);}
 public static void VerifyWithCapture(Action<SharedRhythm,VillagerSocial,CreatureHybrid> capture){
  if(!Application.isPlaying)throw new Exception("Enter Play mode");
  var slice=UnityEngine.Object.FindAnyObjectByType<QuestSlice>();var god=slice.GetComponentInChildren<CreatureHybrid>();var ball=slice.GetComponentInChildren<BeachBall>();var toy=slice.GetComponentsInChildren<SharedRhythm>().First();var prop=toy.GetComponent<CreatureAffordance>();var residents=slice.GetComponentsInChildren<VillagerSocial>();
  var field=typeof(QuestSlice).GetField("state",BindingFlags.Instance|BindingFlags.NonPublic);var original=field.GetValue(slice);var positions=residents.Select(a=>a.transform.position).ToArray();var godPosition=god.transform.position;var toyPosition=toy.transform.position;
  foreach(var a in residents){a.ResetTask();a.GetComponent<VillagerExploration>().ResetTask();a.GetComponent<VillagerBlockPlay>().ResetTask();}god.Interrupt("Shared activities QA");
  var test=new SliceState{developmentFocus=true,hunger=.1f};field.SetValue(slice,test);var host=residents[0];var nav=god.GetComponent<IslandNavigator>();float unit=nav.world.Scale;
  void Stage(){
   test.hunger=.1f;test.Hybrid.motivation=new CreatureMotivation(); // Isolate reciprocal-play mechanics from needs/repetition tests.
   host.ResetTask();god.Interrupt("Stage shared rhythm");toy.ResetOffer();host.Memory.nextInvitationAt=0;host.Memory.trust=.7f;host.Memory.injury=0;host.Memory.frights=0;
   if(!nav.world.Sample(ball.GetComponent<CarryItem>().home,nav.profile,12,out var safe)||!nav.world.Sample(safe+Vector3.right*unit*10,nav.world.villager,5,out var person))throw new Exception("No open rhythm stage");
   god.transform.position=nav.world.ProjectFoot(safe,nav.profile);nav.ResetRoute();host.transform.position=nav.world.ProjectFoot(person,nav.world.villager);host.GetComponent<IslandNavigator>().ResetRoute();
   foreach(var target in god.Targets)if(target.affordance!=null)target.affordance.cooldown=test.Hybrid.time+1000;
   prop.cooldown=0;prop.Item.held=false;prop.GetComponent<Collider>().enabled=true;prop.transform.position=host.transform.position+Vector3.up*unit*.7f;
   if(!host.TryShare(prop))throw new Exception("Native chime invitation refused");
   for(int i=0;i<1600&&!host.Ready&&host.Active;i++){test.Hybrid.time+=.05f;host.Tick(.05f,false);}if(!host.Ready)throw new Exception("Host did not fetch/carry/present chime");
  }
  try{
   Stage();int oldGames=host.Memory.rhythmGames,seen=0,villagerNotes=0,creatureNotes=0;bool pauseChecked=false,captured=false;
   for(int i=0;i<2000&&host.Memory.rhythmGames==oldGames;i++){
    god.Tick(.05f);host.Tick(.05f,false);
    if(toy.Active&&toy.Beats>=3&&!captured){captured=true;capture?.Invoke(toy,host,god);}
    if(toy.Beats>seen){if((toy.Beats-1)%4<2)villagerNotes++;else creatureNotes++;seen=toy.Beats;}
    if(toy.Active&&!pauseChecked){int beat=toy.Beats;test.paused=true;god.Tick(20);host.Tick(20,true);if(toy.Beats!=beat||!toy.Active)throw new Exception("Pause advanced rhythm");test.paused=false;pauseChecked=true;}
   }
   if(host.Memory.rhythmGames!=oldGames+1||test.WorldPlay.rhythmGames!=1||toy.Beats!=8||villagerNotes!=4||creatureNotes!=4||!pauseChecked||prop.Item.held||prop.host!=null)throw new Exception($"Reciprocal rhythm failed: beats={toy.Beats}, roles={villagerNotes}/{creatureNotes}, phase={god.GetComponent<CreatureCuriosity>().Current}, host={host.Current}");
   if(!test.Hybrid.receipt.rhythmGame||test.Hybrid.receipt.partner!=host.GetComponent<HybridTarget>().id)throw new Exception("Rhythm teaching misattributed");float preference=test.WorldPlay.rhythmFun;if(!god.Feedback(true)||test.WorldPlay.rhythmFun<=preference||god.Feedback(false))throw new Exception("Rhythm feedback not one-shot");
   var save=JsonUtility.FromJson<SliceState>(JsonUtility.ToJson(test));if(!save.Valid()||save.WorldPlay.rhythmGames!=1||save.Social(0).rhythmGames!=1||!save.Hybrid.receipt.rhythmGame)throw new Exception("Rhythm memory did not save");save.WorldPlay.rhythmFun=float.NaN;if(save.Valid())throw new Exception("Invalid rhythm preference accepted");
   Stage();for(int i=0;i<1000&&!toy.Active;i++){god.Tick(.05f);host.Tick(.05f,false);}if(!toy.Active)throw new Exception($"No rhythm to interrupt: {god.Owner}/{god.GetComponent<CreatureCuriosity>().Current}, host={host.Current}, hunger={test.hunger:F2}, reason={god.Explanation}");int before=host.Memory.rhythmGames;host.GetComponent<CarryItem>().held=true;host.Tick(.05f,false);god.Tick(.05f);host.GetComponent<CarryItem>().held=false;
   if(toy.Active||host.Active||host.Memory.rhythmGames!=before||test.Hybrid.receipt.eligible||god.GetComponentsInChildren<HumanoidHands>().Any(h=>h.touchTarget!=null)||host.GetComponentsInChildren<HumanoidHands>().Any(h=>h.touchTarget!=null))throw new Exception("Pickup did not cancel without shared reward");
   VerifyBallInvitation(slice,god,ball,test,host);
   Debug.Log("BWVR_SHARED_ACTIVITIES_PASS: native chime offer and utility acceptance; eight alternating animated/audio taps; pause; exactly one shared outcome and teaching receipt; save/invalid memory; actor pickup/touch cleanup; native beckon-before-roll, cooldown and relationship-sensitive invitation preference");
  }finally{
   host.GetComponent<CarryItem>().held=false;host.ResetTask();god.Interrupt("Shared activities QA done");foreach(var a in residents)a.GetComponent<VillagerExploration>().ResetTask();field.SetValue(slice,original);for(int i=0;i<residents.Length;i++){residents[i].transform.position=positions[i];residents[i].GetComponent<IslandNavigator>().ResetRoute();}god.transform.position=godPosition;nav.ResetRoute();prop.Item.held=false;prop.host=null;prop.transform.position=toyPosition;prop.GetComponent<Collider>().enabled=true;toy.Cancel();ball.ResetWorld();
  }
 }
 static void VerifyBallInvitation(QuestSlice slice,CreatureHybrid god,BeachBall ball,SliceState test,VillagerSocial host){
  host.ResetTask();god.Interrupt("Stage villager beckon");var actor=host.GetComponent<VillagerExploration>();actor.ResetTask();ball.ReturnHome();var nav=actor.GetComponent<IslandNavigator>();float unit=nav.world.Scale;bool ready=false;
  for(int i=0;i<16&&!ready;i++){
   actor.ResetTask();ball.ReturnHome();var direction=Quaternion.Euler(0,i*22.5f,0)*Vector3.forward;
   if(!nav.world.Sample(ball.transform.position+direction*unit*24,nav.world.creature,2,out var g)||!nav.world.Sample(ball.transform.position-direction*unit*6,nav.profile,2,out var v))continue;
   god.transform.position=nav.world.ProjectFoot(g,nav.world.creature);host.transform.position=nav.world.ProjectFoot(v,nav.profile);nav.ResetRoute();ready=actor.TryBall(ball);
  }
  if(!ready)throw new Exception("No invitation stage");int invitations=host.Memory.ballInvitations,kicks=test.WorldPlay.villagerKicks;
  void Step(){test.Hybrid.time+=.02f;actor.Tick(.02f,false);ball.Advance(.02f,false);}
  for(int i=0;i<4000&&host.Memory.ballInvitations==invitations&&actor.Active;i++)Step();
  var hands=host.GetComponentInChildren<HumanoidHands>();if(host.Memory.ballInvitations!=invitations+1||test.WorldPlay.villagerKicks!=kicks||hands==null||!hands.beckon||hands.lookTarget==null||hands.lookTarget==ball.transform)throw new Exception("No visible beckon before first roll");
  var position=host.transform.position;actor.Tick(10,true);ball.Advance(10,true);if(position!=host.transform.position||test.WorldPlay.villagerKicks!=kicks)throw new Exception("Paused invitation advanced");
  for(int i=0;i<1500&&test.WorldPlay.villagerKicks==kicks&&actor.Active;i++)Step();if(test.WorldPlay.villagerKicks!=kicks+1||host.Memory.ballInvitations!=invitations+1||host.Memory.nextBallInvitationAt<test.Hybrid.time+50)throw new Exception("Invitation roll or cooldown failed");
  var cautious=new SocialMemory{trust=.4f,affinity=.2f};var friendly=new SocialMemory{trust=.9f,affinity=.9f};int low=Enumerable.Range(0,500).Count(i=>cautious.WantsCreaturePlay(1347,i,"a",.2f)),high=Enumerable.Range(0,500).Count(i=>friendly.WantsCreaturePlay(1347,i,"a",.9f));if(high<=low+100)throw new Exception("Experience does not change invitation willingness");
  var saved=JsonUtility.FromJson<SliceState>(JsonUtility.ToJson(test));if(!saved.Valid()||saved.Social(0).ballInvitations!=host.Memory.ballInvitations)throw new Exception("Invitation memory did not save");actor.ResetTask();
 }
}
}
