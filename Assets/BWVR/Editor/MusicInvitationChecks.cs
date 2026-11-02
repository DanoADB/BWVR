using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
namespace BWVR.Editor {
public static class MusicInvitationChecks {
 static readonly BindingFlags F=BindingFlags.Instance|BindingFlags.NonPublic;
 static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
 public static void Verify(Action<SharedRhythm,VillagerSocial,CreatureHybrid> capture=null){
  var slice=UnityEngine.Object.FindAnyObjectByType<QuestSlice>();var field=typeof(QuestSlice).GetField("state",F);var pointer=typeof(QuestSlice).GetMethod("MusicInvitationPointer",F);var hands=(CarryItem[])typeof(QuestSlice).GetField("held",F).GetValue(slice);bool tested=false;int accepted=0;
  bool Point(PlayerRhythm music,bool down,bool last,bool tracked=true,bool shared=true,bool blocked=false,Collider hit=null,int side=1)=>
   (bool)pointer.Invoke(slice,new object[]{side,down,last,tracked,shared,hit??music.GetComponent<Collider>(),.05f,blocked});
  void ResetHost(VillagerSocial host,CreatureHybrid god){host.ResetTask();host.Memory.nextInvitationAt=0;god.State.time+=9;}
  try{
   PlaceMusicChecks.VerifyWithCapture(capture,staged:(rhythm,host,god)=>{
    var music=rhythm.GetComponent<PlayerRhythm>();var prop=rhythm.GetComponent<CreatureAffordance>();slice.SetSanctuaryOpen(false);var palette=slice.GetComponentInChildren<GodToolPalette>();palette.Cancel();palette.Process(new Ray(Camera.main.transform.position,Camera.main.transform.forward),false,true,false,.05f,false);
    foreach(var other in slice.GetComponentsInChildren<VillagerSocial>()){other.GetComponent<VillagerRoutine>().ResetTask();other.GetComponent<CarryItem>().held=other!=host;}host.Memory.musicInterest=.7f;var toward=prop.transform.position-host.transform.position;toward.y=0;if(toward.sqrMagnitude>0)host.transform.rotation=Quaternion.LookRotation(toward);Physics.SyncTransforms();
    if(tested)return;tested=true;var anchor=prop.transform.position;var rotation=prop.transform.rotation;int games=god.Season.WorldPlay.rhythmGames;
    Check(!Point(music,true,false,side:0)&&!Point(music,true,false,tracked:false)&&!Point(music,true,false,blocked:true)&&prop.host==null,"Blocked/left/untracked input invited music");
    var key=music.GetComponentInChildren<ChimeKey>();Check(!Point(music,true,false,hit:key.GetComponent<Collider>())&&prop.host==null,"Note bar was treated as invitation body");
    god.Season.paused=true;Check(!music.RequestVillageMusic(),"Paused invitation accepted");god.Season.paused=false;prop.Item.held=true;Check(!music.RequestVillageMusic(),"Player-held chime invited");prop.Item.held=false;
    host.GetComponent<VillagerRoutine>().BeginCathedral();Check(!music.RequestVillageMusic(),"Invitation stole a working villager");host.GetComponent<VillagerRoutine>().ResetTask();
    Check((host.Memory.Place(0)?.visits??0)==0,"Fixture already knew music place");
    Check(Point(music,true,false)&&prop.host==null&&hands[1]==null,"Press grabbed or commanded before release");Check(Point(music,false,true)&&prop.host==host&&hands[1]==null,"Tap did not invite nearby idle host: sight="+host.GetComponent<VillagerExploration>().CanSeeEcho(music)+" willing="+host.Willing+" free="+host.GetComponent<VillagerRoutine>().AvailableForBell+" available="+prop.Available+" owner="+god.Owner+" menu="+slice.MenuOpen);
    for(int i=0;i<1800&&host.Active&&!host.Ready;i++){god.State.time+=.05f;host.Tick(.05f,false);Check(!prop.Item.held&&Vector3.Distance(prop.transform.position,anchor)<.001f,"Unknown-place invitation moved instrument");}
    Check(host.Ready&&!rhythm.Active&&god.Owner==CreatureHybrid.ActionOwner.None&&god.Season.WorldPlay.rhythmGames==games,"Invitation did not reach or fabricated a performance");Check(!music.RequestVillageMusic(),"Repeated gesture replaced reservation");ResetHost(host,god);
    Check(Point(music,true,false),"Hold did not begin");for(int i=0;i<7;i++)Point(music,true,true);Check(hands[1]==prop.Item&&prop.Item.held&&prop.host==null,"Hold did not use native pickup");
    typeof(QuestSlice).GetMethod("Release",F).Invoke(slice,new object[]{1,new Ray(anchor+Vector3.up*god.GetComponent<IslandNavigator>().world.Scale*12,Vector3.down)});prop.transform.SetPositionAndRotation(anchor,rotation);prop.Item.held=false;prop.GetComponent<Collider>().enabled=true;
    Point(music,true,false);Point(music,true,true,tracked:false);Point(music,false,true);Check(prop.host==null,"Tracking loss replayed a tap");Point(music,true,false);Point(music,true,true,hit:key.GetComponent<Collider>());Point(music,false,true,hit:key.GetComponent<Collider>());Check(prop.host==null,"Pointer drag replayed a tap");
    host.transform.LookAt(new Vector3(anchor.x,host.transform.position.y,anchor.z));Physics.SyncTransforms();Check(Point(music,true,false,shared:false)&&prop.host==host,"Desktop F did not invite: sight="+host.GetComponent<VillagerExploration>().CanSeeEcho(music)+" next="+host.Memory.nextInvitationAt+" time="+god.State.time+" idle="+host.GetComponent<VillagerRoutine>().AvailableForBell+" available="+prop.Available);Check(Point(music,true,true,shared:false)==false&&prop.host==host,"Held desktop F retriggered");ResetHost(host,god);Physics.SyncTransforms();
   },continued:(rhythm,host,god)=>{
    var before=god.Season;typeof(QuestSlice).GetMethod("Save",F).Invoke(slice,null);typeof(QuestSlice).GetMethod("Load",F).Invoke(slice,null);var loaded=(SliceState)field.GetValue(slice);Check(loaded.Valid()&&loaded.Social(host.MelodyTeacherIdentity).rhythmGames==before.Social(host.MelodyTeacherIdentity).rhythmGames,"Save lost completed music");Check(rhythm.GetComponent<CreatureAffordance>().host==null&&!rhythm.Active,"Reload revived invitation");
   },invite:(rhythm,host,god)=>{var music=rhythm.GetComponent<PlayerRhythm>();god.State.time+=9;bool result=music.RequestVillageMusic();if(result){accepted++;Check(rhythm.GetComponent<CreatureAffordance>().host==host,"Cue chose unavailable actor");}return result;});
   Check(tested&&accepted>=4,"Missing native invitation fixture");Debug.Log("BWVR_MUSIC_INVITATION_PASS: Quest body tap/hold, desktop edge, bar exclusion, tracking/drag/pause/carry/work guards, unfamiliar stationary approach, actual utility duet, cancellation/timeout, no fabricated practice, native Save/Load");
  }finally{pointer.Invoke(slice,new object[]{1,false,false,true,true,null,.05f,true});}
 }
}
}
