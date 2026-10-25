using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace BWVR.Editor {
public static class MoodAndComfortChecks {
 public static void VerifyFace(){
  void Require(bool ok,string why){if(!ok)throw new Exception(why);}
  var source=AssetDatabase.LoadAssetAtPath<GameObject>(AnimalFriendsImporter.Prefab);Require(source!=null,"Import purchased gorilla first");
  var clone=UnityEngine.Object.Instantiate(source);
  try{
   var face=clone.GetComponentInChildren<GorillaMoodFace>();Require(face!=null&&face.profile.Valid(),"Missing valid mood profile");
   var transforms=face.GetComponentsInChildren<Transform>();var bones=face.profile.bones.Select(n=>transforms.First(t=>t.name==n)).ToArray();
   foreach(CreatureAffect.Mood mood in Enum.GetValues(typeof(CreatureAffect.Mood))){face.ApplyMood(mood,4);Require(face.CurrentMood==mood,"Mood did not select");for(int i=0;i<bones.Length;i++)Require(Quaternion.Angle(bones[i].localRotation,face.profile.moods[(int)mood].rotations[i])<.1f,"Face failed to reach target");}
   for(int x=1;x<=6;x++)for(int y=x+1;y<=6;y++)Require(Enumerable.Range(0,bones.Length).Sum(i=>Quaternion.Angle(face.profile.moods[x].rotations[i],face.profile.moods[y].rotations[i]))>.5f,"Two mood faces are identical: "+x+" / "+y);
   var before=bones.Select(b=>b.localRotation).ToArray();face.ApplyMood(CreatureAffect.Mood.Sad,0);Require(bones.Select((b,i)=>Quaternion.Angle(b.localRotation,before[i])).All(a=>a<.01f),"Paused face advanced");
   bones[0].localRotation*=Quaternion.Euler(8,0,0);var authored=bones[0].localRotation;face.ApplyMood(CreatureAffect.Mood.Happy,.2f,true);Require(Quaternion.Angle(authored,bones[0].localRotation)<.01f,"Mood replaced authored reaction");
   Debug.Log("BWVR_MOOD_FACE_PASS: six distinct authored-derived facial poses, smooth convergence, zero-time freeze and authored reaction ownership");
  }finally{UnityEngine.Object.DestroyImmediate(clone);}
 }
 public static void VerifyComfort(){
  if(!EditorApplication.isPlaying)throw new Exception("Enter Play mode first");void Require(bool ok,string why){if(!ok)throw new Exception(why);}
  var slice=UnityEngine.Object.FindAnyObjectByType<QuestSlice>();var god=UnityEngine.Object.FindAnyObjectByType<CreatureHybrid>();var residents=slice.GetComponentsInChildren<VillagerExploration>();
  var helper=residents[0];var friend=residents[1];var other=residents[2];var positions=residents.Select(r=>r.transform.position).ToArray();
  var field=typeof(QuestSlice).GetField("state",BindingFlags.NonPublic|BindingFlags.Instance);var original=field.GetValue(slice);var test=new SliceState{developmentFocus=true,water=100,crops=100};
  foreach(var r in residents){r.ResetTask();r.GetComponent<VillagerSocial>().ResetTask();r.GetComponent<VillagerBlockPlay>().ResetTask();}field.SetValue(slice,test);
  void Stage(){foreach(var r in residents)r.ResetTask();var nav=helper.GetComponent<IslandNavigator>();var site=god.GetComponent<CreatureConstructionLearning>().sites[0];foreach(var r in residents){Require(nav.world.Sample(site.World(new Vector3(r==helper?0:r==friend?9:-6,0,-13)),nav.profile,6,out var at),"No comfort approach");r.transform.position=nav.world.ProjectFoot(at,nav.profile);r.GetComponent<IslandNavigator>().ResetRoute();}}
  void Step(){test.Hybrid.time+=.05f;helper.Tick(.05f,false);friend.Tick(.05f,false);}
  try{
   Stage();Require(!helper.TryComfort(friend),"Offered comfort without distress");test.Social(1).injury=.3f;float trust=test.Social(1).trust;
   helper.Tick(36,false);Require(helper.Comforting&&friend.Comforting,"Idle chooser did not notice distressed neighbor");Require(!other.TryComfort(friend),"Second helper stole recipient");
   var at=helper.transform.position;helper.Tick(8,true);friend.Tick(8,true);Require(helper.transform.position==at&&test.Social(1).comfortReceived==0,"Pause advanced reassurance");
   for(int i=0;i<600&&test.Social(1).comfortReceived==0;i++)Step();Require(test.Social(0).comfortGiven==1&&test.Social(1).comfortReceived==1,"Native comfort approach/completion failed");
   Require(test.Social(1).injury==.3f&&test.Social(1).trust==trust,"Comfort magically healed or changed creature trust");Require(test.Social(1).Peer(helper.GetComponent<HybridTarget>().id)?.good==1,"No friendship from comfort");
   var saved=JsonUtility.FromJson<SliceState>(JsonUtility.ToJson(test));Require(saved.Valid()&&saved.Social(1).comfortReceived==1&&saved.Social(1).nextComfortAt>test.Hybrid.time,"Comfort memory did not save");saved.Social(1).nextComfortAt=float.NaN;Require(!saved.Valid(),"Invalid memory accepted");
   Stage();Require(!helper.TryComfort(friend),"Comfort cooldown ignored");test.Social(0).nextComfortAt=test.Social(1).nextComfortAt=0;Require(helper.TryComfort(friend),"Comfort retry rejected");friend.GetComponent<CarryItem>().held=true;helper.Tick(.05f,false);friend.GetComponent<CarryItem>().held=false;Require(!helper.Comforting&&!friend.Comforting&&test.Social(1).comfortReceived==1,"Pickup did not release both roles without reward");
   Stage();test.Social(0).nextComfortAt=test.Social(1).nextComfortAt=0;test.Social(1).injury=0;test.Social(1).frights=1;test.Social(1).lastFrightAt=test.Hybrid.time;Require(helper.TryComfort(friend),"Frightened neighbor rejected comfort");for(int i=0;i<600&&test.Social(1).comfortReceived<2;i++)Step();Require(test.Social(1).comfortReceived==2&&friend.GetComponent<VillagerSocial>().RecentFright,"Comfort erased creature-fright memory");
   Debug.Log("BWVR_COMFORT_PASS: idle distress choice, native approach, reciprocal friendship, no healing/creature-trust/fear rewrite, exclusive roles, pause, cooldown, grab cleanup and saved memory");
  }finally{foreach(var r in residents){r.GetComponent<CarryItem>().held=false;r.ResetTask();}field.SetValue(slice,original);for(int i=0;i<residents.Length;i++){residents[i].transform.position=positions[i];residents[i].GetComponent<IslandNavigator>().ResetRoute();}}
 }
}
}
