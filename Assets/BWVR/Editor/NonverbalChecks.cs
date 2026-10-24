using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace BWVR.Editor {
public static class NonverbalChecks {
 [MenuItem("BWVR/Verify nonverbal drought cues (Play mode)")]
 public static void Verify(){
  if(!Application.isPlaying)throw new Exception("Enter Play mode first");
  var slice=UnityEngine.Object.FindAnyObjectByType<QuestSlice>();var flags=BindingFlags.Instance|BindingFlags.NonPublic;var stateField=typeof(QuestSlice).GetField("state",flags);var original=(SliceState)stateField.GetValue(slice);var test=new SliceState{paused=true,water=30,crops=20};stateField.SetValue(slice,test);
  var signals=slice.GetComponent<DroughtSignals>();var god=slice.GetComponentInChildren<CreatureRoutine>();var gatherer=slice.GetComponentsInChildren<VillagerRoutine>().First(v=>v.role==VillagerRoutine.Role.WaterGatherer);bool previousHelp=slice.HelpVisible;
  try{
   slice.SetHelp(false);var label=(UnityEngine.UI.Text)typeof(QuestSlice).GetField("creatureLabel",flags).GetValue(slice);if(label.gameObject.activeSelf)throw new Exception("Floating intent visible with help off");slice.SetHelp(true);if(!label.gameObject.activeSelf)throw new Exception("Optional intent help unavailable");slice.SetHelp(false);
   int requests=signals.requests;signals.Tick(100,test);if(signals.requests!=requests)throw new Exception("Paused village started a request");var cistern=slice.GetComponentsInChildren<WaterVessel>().First(v=>v.name=="Village cistern");if(Mathf.Abs(cistern.Fill-.3f)>.001f)throw new Exception("Cistern does not show water amount");
   god.ResetTask();gatherer.ResetTask();signals.ResetRequests();test.paused=false;signals.Tick(2.1f,test);if(signals.requests!=requests+1||!gatherer.Signalling)throw new Exception("Low water did not trigger an autonomous request");test.paused=true;
   god.ResetTask();gatherer.ResetTask();int shown=gatherer.requestsShown,noticed=god.acknowledgements;if(!gatherer.RequestWater(god))throw new Exception("Water request refused");
   for(int i=0;i<10000&&gatherer.requestsShown==shown;i++)gatherer.Tick(.1f,false);
   if(gatherer.requestsShown!=shown+1||gatherer.bucket.Fill!=0)throw new Exception("Empty bucket demonstration failed");
   var hands=gatherer.GetComponentInChildren<HumanoidHands>();if(hands==null||hands.lookTarget==null||Mathf.Abs(hands.displayTilt)<40)throw new Exception("Missing gaze or empty-bucket gesture");
   var position=gatherer.transform.position;gatherer.Tick(10,true);if(gatherer.transform.position!=position||!gatherer.Signalling)throw new Exception("Paused demonstration advanced");
   bool beckoned=false,pointed=false;for(int i=0;i<70&&gatherer.Signalling;i++){gatherer.Tick(.1f,false);beckoned|=hands.beckon;pointed|=hands.indicating;}
   if(!beckoned||!pointed||gatherer.Signalling||god.acknowledgements!=noticed+1)throw new Exception("Request, acknowledgement and return-to-work failed");
   bool filled=false;int jobs=gatherer.completedJobs;for(int i=0;i<10000&&gatherer.completedJobs==jobs;i++){gatherer.Tick(.1f,false);filled|=gatherer.bucket.Fill>.9f;}
   if(!filled||gatherer.completedJobs!=jobs+1||gatherer.bucket.Fill!=0)throw new Exception("Bucket did not fill, deliver and empty");
   int gifts=test.gifts;signals.WaterArrived();if(test.gifts!=gifts||!hands.beckon)throw new Exception("Celebration granted a reward or did not react");
   test.water=80;signals.Tick(0,test);if(Mathf.Abs(cistern.Fill-.8f)>.001f)throw new Exception("Visible water failed to rise");
   Debug.Log("BWVR_NONVERBAL_PASS: optional text; visible water level; paused request; empty bucket; gaze; beckon; point; creature acknowledgement; independent work; fill and delivery; celebration without extra reward");
  }finally{god.ResetTask();foreach(var job in slice.GetComponentsInChildren<VillagerRoutine>())job.ResetTask();slice.SetHelp(previousHelp);stateField.SetValue(slice,original);signals.Tick(0,original);}
 }
}
}
