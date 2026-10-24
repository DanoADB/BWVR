using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace BWVR.Editor {
public static class DevelopmentFocusChecks {
 [MenuItem("BWVR/Verify exploration development focus (Play mode)")]
 public static void Verify(){
  if(!EditorApplication.isPlaying)throw new Exception("Enter Play mode first");var slice=UnityEngine.Object.FindAnyObjectByType<QuestSlice>();var god=UnityEngine.Object.FindAnyObjectByType<CreatureHybrid>();var stateField=typeof(QuestSlice).GetField("state",BindingFlags.Instance|BindingFlags.NonPublic);var original=(SliceState)stateField.GetValue(slice);bool mode=slice.developmentFocus;god.Interrupt("Development focus QA");var test=new SliceState{water=10,crops=10};stateField.SetValue(slice,test);var signals=slice.GetComponent<DroughtSignals>();int requests=signals.requests;
  try{
   slice.developmentFocus=true;for(int i=0;i<300;i++){slice.Simulate(.1f);if(god.Owner==CreatureHybrid.ActionOwner.Water)throw new Exception("Exploration forced creature water work");}
   if(test.seconds!=180||test.water<10||test.crops<10||signals.requests!=requests||test.Hybrid.time<29.9f)throw new Exception("Development focus froze the wrong clock or retained drought pressure");
   god.Interrupt("Check explicit help");if(!god.Guide(GuidanceIntent.Help,god.Find("cistern")))throw new Exception("Development water help unavailable");god.Perceive();var candidates=(List<CreatureCandidate>)typeof(CreatureHybrid).GetField("candidates",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(god);if(!candidates.Single(c=>c.action==CreatureAction.WaterDelivery).feasible)throw new Exception("Explicit water help blocked");
   god.Interrupt("Check drought restoration");test=new SliceState{water=14};stateField.SetValue(slice,test);god.NewSeason();slice.developmentFocus=false;slice.Simulate(.5f);if(test.seconds>=180||test.water>=14||god.Owner!=CreatureHybrid.ActionOwner.Water)throw new Exception("Normal drought priorities were lost");
   slice.developmentFocus=true;slice.Simulate(.5f);if(god.Owner==CreatureHybrid.ActionOwner.Water||god.GetComponent<CreatureCuriosity>().Holding?.kind==Offering.Water)throw new Exception("Switching focus retained water ownership");
   Debug.Log("BWVR_DEVELOPMENT_FOCUS_PASS: 30 seconds of real simulation without drought pressure, distress or forced creature water; simulation clock and autonomous actors continue; explicit water help remains feasible; normal drought mode restored; live focus switch cleans water ownership");
  }finally{god.Interrupt("Development focus QA complete");slice.developmentFocus=mode;stateField.SetValue(slice,original);}
 }
}
}
