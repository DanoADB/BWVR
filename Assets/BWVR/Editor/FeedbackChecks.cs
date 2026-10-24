using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace BWVR.Editor {
public static class FeedbackChecks {
 [MenuItem("BWVR/Verify creature feedback (Play mode)")]
 public static void Verify(){
  if(!EditorApplication.isPlaying)throw new Exception("Enter Play mode first");
  var mind=UnityEngine.Object.FindAnyObjectByType<CreatureCuriosity>();var routine=mind.GetComponent<CreatureRoutine>();var nav=mind.GetComponent<IslandNavigator>();var icon=mind.GetComponent<CreatureAffect>();
  var season=new SliceState{seconds=0,water=100};var start=mind.transform.position;
  try{
   mind.ResetTask();routine.ResetTask();mind.RememberWaterDelivery(season);
   float care=season.care;if(!mind.Feedback(false)||season.care>=care)throw new Exception("Negative water lesson not learned");
   icon.SendMessage("LateUpdate");if(icon.Displayed!=CreatureAffect.Mood.Sad)throw new Exception("Negative feedback lacks immediate icon");
   care=season.care;if(mind.Feedback(false)||season.care!=care)throw new Exception("Repeated feedback stacked one lesson");
   mind.RememberWaterDelivery(season);if(!mind.Feedback(true)||season.care<=care)throw new Exception("Positive water lesson not learned");
   icon.SendMessage("LateUpdate");if(icon.Displayed!=CreatureAffect.Mood.Happy)throw new Exception("Positive feedback lacks immediate icon");
   mind.RememberWaterDelivery(season);season.paused=true;care=season.care;if(mind.Feedback(false)||season.care!=care)throw new Exception("Paused feedback changed learning");season.paused=false;
   mind.Tick(12.1f,season);care=season.care;if(mind.Feedback(false)||season.care!=care)throw new Exception("Expired event still learned");
   mind.ResetTask();routine.ResetTask();var toy=UnityEngine.Object.FindObjectsByType<CreatureAffordance>().First(a=>a.kind==CreatureAffordance.Kind.Roll);toy.cooldown=0;
   if(!nav.world.Sample(toy.transform.position,nav.profile,20,out var at))throw new Exception("No toy approach");mind.transform.position=nav.world.Terrain.Project(at);nav.ResetRoute();
   if(!mind.TryBegin(toy))throw new Exception("No toy interaction");
   int before=mind.played;for(int i=0;i<300&&mind.Current!=CreatureCuriosity.Phase.Rest;i++)mind.Tick(.1f,season);
   if(mind.played!=before+1)throw new Exception("Toy interaction unfinished");
   float play=season.playPreference;care=season.care;
   if(!mind.Feedback(false)||season.playPreference>=play||season.care!=care)throw new Exception("Toy feedback trained wrong behavior");
   mind.Tick(.1f,season);if(mind.Affect!=CreatureAffect.Mood.Sad)throw new Exception("Activity immediately overwrote feedback reaction");
   var saved=JsonUtility.FromJson<SliceState>(JsonUtility.ToJson(season));if(!saved.Valid()||saved.playPreference!=season.playPreference||saved.care!=season.care)throw new Exception("Learned preferences not saved");
   Debug.Log("BWVR_FEEDBACK_PASS: positive/negative, immediate icons, one lesson per event, pause, expiry, toy versus water credit, reaction persistence and saved learning");
  }finally{mind.ResetTask();routine.ResetTask();mind.transform.position=start;nav.ResetRoute();}
 }
}
}
