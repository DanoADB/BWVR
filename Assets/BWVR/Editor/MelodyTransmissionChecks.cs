using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
namespace BWVR.Editor {
public static class MelodyTransmissionChecks {
 static readonly BindingFlags F=BindingFlags.Instance|BindingFlags.NonPublic;
 static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
 public static void VerifyModel(){
  var state=new SliceState();var person=state.Social(1);
  Check(!person.HearMelody(2,0)&&!person.hasBorrowedMelody&&!person.hasMelody,"One hearing fabricated knowledge");
  Check(person.HearMelody(2,0)&&person.hasBorrowedMelody&&person.borrowedMelody==2&&person.melodyTeacher==0&&!person.hasMelody&&person.melodyEchoes==0,"Listening fabricated performance");
  Check(!person.HearMelody(1,2)&&!person.HearMelody(1,2)&&person.borrowedMelody==2,"Later hearing overwrote a learned phrase");
  string before=JsonUtility.ToJson(state);var page=CreationBookText.Read(state,CreationBookSection.People,1);
  Check(page.text.Contains("Phrase learned: "+VillageMelodies.Name(2)),"Learned phrase missing from profile");
  for(int i=0;i<5;i++)Check(CreationBookText.Read(state,CreationBookSection.Creature,i).count==5,"Creature page count inconsistent");
  Check(before==JsonUtility.ToJson(state),"Book initialized melody history");
  Check(JsonUtility.FromJson<SliceState>(before).Valid(),"Transmission save invalid");
  person.melodyHeard=new[]{0,0,1,0};Check(!state.Valid(),"Unsubstantiated learned phrase accepted");
  person.melodyHeard=new[]{0,0,2};Check(!state.Valid(),"Malformed phrase counts accepted");
  person.melodyHeard=new[]{0,0,2,0};person.melodyTeacher=-1;Check(!state.Valid(),"Invalid teacher accepted");
  Check(new SliceState().Valid(),"Legacy season rejected");
  Debug.Log("BWVR_MELODY_TRANSMISSION_MODEL_PASS: two-hearing threshold, knowledge/performance separation, retained phrase, bounded valid save and pure Book pages");
 }
 public static void VerifyRuntime(){
  string saved=null;int learned=-1;var slice=UnityEngine.Object.FindAnyObjectByType<QuestSlice>();
  PlaceMusicChecks.VerifyWithCapture(null,continued:(music,originalHost,god)=>{
   var people=slice.GetComponentsInChildren<VillagerSocial>();var listener=people[1];var audience=listener.GetComponent<VillagerExploration>();var nav=god.GetComponent<IslandNavigator>();var site=god.GetComponent<CreatureConstructionLearning>().sites[0];var prop=music.GetComponent<CreatureAffordance>();var state=god.Season;
   foreach(var person in people){person.Memory.melodyHeard=null;person.Memory.hasBorrowedMelody=false;}
   learned=originalHost.MelodyChoice;
   void Play(VillagerSocial host,bool late=false,bool blocked=false){
    god.Interrupt("Transmission QA stage");music.ResetOffer();prop.host=null;prop.Item.held=false;
    foreach(var person in people){person.ResetTask();person.GetComponent<VillagerExploration>().ResetTask();person.GetComponent<VillagerResourceGathering>().Cancel();person.GetComponent<VillagerBlockPlay>().ResetTask();person.Memory.nextMusicAt=state.Hybrid.time+1000;}
    state.Hybrid.motivation=new CreatureMotivation();state.hunger=.1f;state.water=100;state.Weather.rain=0;
    void At(Transform target,IslandNavigator route,Vector3 wanted){Check(nav.world.Sample(wanted,route.profile,8,out var at),"No transmission stage");target.position=nav.world.ProjectFoot(at,route.profile);route.ResetRoute();}
    At(god.transform,nav,site.World(new Vector3(0,0,-28)));At(host.transform,host.GetComponent<IslandNavigator>(),site.World(new Vector3(10,0,-12)));
    prop.transform.SetPositionAndRotation(site.World(new Vector3(0,.3f,-12)),site.transform.rotation);god.transform.LookAt(new Vector3(prop.transform.position.x,god.transform.position.y,prop.transform.position.z));
    if(host!=listener){At(listener.transform,listener.GetComponent<IslandNavigator>(),site.World(new Vector3(12,0,0)));listener.Memory.nextMusicAt=late?state.Hybrid.time+1000:0;listener.Memory.musicInterest=.8f;listener.Memory.trust=.7f;listener.Memory.injury=0;}
    host.Memory.trust=.8f;host.Memory.nextInvitationAt=0;foreach(var target in god.Targets)if(target.affordance!=null)target.affordance.cooldown=state.Hybrid.time+1000;prop.cooldown=0;Physics.SyncTransforms();
    Check(host.TryShare(prop),"Learned host did not offer a chime");for(int i=0;i<1800&&host.Active&&!host.Ready;i++){state.Hybrid.time+=.05f;host.Tick(.05f,false);}Check(host.Ready,"Chime not presented");
    if(host!=listener){var resources=slice.GetComponentInChildren<VillageResources>();bool placed=false;for(int k=0;k<24&&!placed;k++){float a=k*Mathf.PI/12;var wanted=host.transform.position+new Vector3(Mathf.Cos(a),0,Mathf.Sin(a))*nav.world.Scale*8;if(!nav.world.Sample(wanted,listener.GetComponent<IslandNavigator>().profile,2,out var at))continue;listener.transform.position=nav.world.ProjectFoot(at,listener.GetComponent<IslandNavigator>().profile);listener.GetComponent<IslandNavigator>().ResetRoute();Physics.SyncTransforms();placed=resources.ProvisionVisible(listener,host.transform.position,host.transform);}Check(placed,"No visible listener stage");}
    GameObject wall=null;bool changed=false;int games=host.Memory.rhythmGames;
    try{for(int i=0;i<1800&&host.Memory.rhythmGames==games;i++){
      if(music.Active&&music.Beats<=1&&blocked&&wall==null){var from=listener.transform.position+Vector3.up*listener.GetComponent<IslandNavigator>().profile.height*.8f;var end=music.GetComponent<Collider>().bounds.center;wall=new GameObject("QA listener sound blocker");wall.transform.position=Vector3.Lerp(from,end,.5f);wall.AddComponent<BoxCollider>().size=Vector3.one*nav.world.Scale*4;Physics.SyncTransforms();}
      if(music.Active&&music.Beats>=3&&!changed){changed=true;state.paused=true;int beats=music.Beats;var calls=typeof(VillagerExploration).GetField("musicCalls",F);int heard=(int)calls.GetValue(audience);audience.HearVillageMelody(music,2);music.Tick(2);Check(beats==music.Beats&&heard==(int)calls.GetValue(audience),"Paused duet progressed or taught a note");state.paused=false;if(late){listener.Memory.nextMusicAt=0;Check(audience.TryWatchMusic(music),("Late listener not admitted: "+audience.Current+" social "+listener.Current+" visible "+audience.CanSeeMusic(music)+" held "+listener.GetComponent<CarryItem>().held+" audience "+music.AudienceCount+" fright "+listener.RecentFright));}}
      god.Tick(.05f);host.Tick(.05f,false);foreach(var person in people){var watching=person.GetComponent<VillagerExploration>();if(watching.MusicAudience)watching.Tick(.05f,false);}
     }
     Check(music.Completed&&host.Memory.rhythmGames==games+1,"Native transmission performance failed: "+god.Explanation);
    }finally{if(wall!=null)UnityEngine.Object.DestroyImmediate(wall);}
   }
   Play(originalHost,late:true);Check(listener.Memory.melodyHeard==null,"Late arrival learned unheard calls");
   Play(originalHost,blocked:true);Check(listener.Memory.melodyHeard==null,"Occluded listener learned a phrase");
   Play(originalHost);Check(listener.Memory.melodyHeard?[learned]==1&&!listener.Memory.hasBorrowedMelody&&!listener.Memory.hasMelody,"First complete hearing not tracked honestly");
   int count=listener.Memory.melodyHeard[learned];Check(!audience.WitnessMusic(music)&&listener.Memory.melodyHeard[learned]==count,"Completion learned twice");
   Play(originalHost);Check(listener.Memory.hasBorrowedMelody&&listener.MelodyChoice==learned&&listener.Memory.melodyTeacher==originalHost.MelodyTeacherIdentity&&!listener.Memory.hasMelody,"Two witnessed performances did not transmit source phrase");
   Play(listener);Check(music.CompletedMelody==learned&&listener.Memory.hasMelody&&listener.Memory.melodyMotif==learned&&listener.Memory.melodyEchoes==1&&state.Hybrid.receipt.partner==listener.GetComponent<HybridTarget>().id,"Learner did not perform adopted phrase as actual host");
   Check(CreationBookText.Read(state,CreationBookSection.People,1).text.Contains("Phrase practiced:"),"Practice missing from Book");saved=JsonUtility.ToJson(state);Check(state.Valid(),"Native transmitted state invalid");
  });
  var field=typeof(QuestSlice).GetField("state",F);var original=field.GetValue(slice);var creature=slice.GetComponentInChildren<CreatureHybrid>();
  try{creature.Interrupt("Transmission persistence QA");field.SetValue(slice,JsonUtility.FromJson<SliceState>(saved));typeof(QuestSlice).GetMethod("Save",F).Invoke(slice,null);typeof(QuestSlice).GetMethod("Load",F).Invoke(slice,null);var loaded=(SliceState)field.GetValue(slice);Check(loaded.Valid()&&loaded.Social(1).borrowedMelody==learned&&loaded.Social(1).hasMelody&&loaded.Social(1).melodyHeard[learned]==2,"Native Save/Load lost learned source or practice");}finally{field.SetValue(slice,original);creature.NewSeason();}
  Debug.Log("BWVR_MELODY_TRANSMISSION_RUNTIME_PASS: late/occluded listeners excluded, paused notes ignored, two real comfortable gatherings, once-only learning, actual learner-host eight-beat duet and native Save/Load");
 }
}
}
