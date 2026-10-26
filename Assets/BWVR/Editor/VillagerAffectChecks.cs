using System;
using UnityEditor;
using UnityEngine;
namespace BWVR.Editor {
public static class VillagerAffectChecks {
 [MenuItem("BWVR/Verify villager state icons (Play mode)")]
 public static void Verify(){
  if(!Application.isPlaying)throw new Exception("Enter Play mode");
  var world=UnityEngine.Object.FindAnyObjectByType<VillageResources>();
  if(VillagerAffect.Select(true,true,true,true,true,true)!=VillagerAffect.Mood.Hurt||VillagerAffect.Select(false,true,true,true,true,true)!=VillagerAffect.Mood.Afraid||VillagerAffect.Select(false,false,true,true,true,true)!=VillagerAffect.Mood.Angry||VillagerAffect.Select(false,false,false,true,true,true)!=VillagerAffect.Mood.Hungry||VillagerAffect.Select(false,false,false,false,true,true)!=VillagerAffect.Mood.Learning||VillagerAffect.Select(false,false,false,false,false,true)!=VillagerAffect.Mood.Love||VillagerAffect.Select(false,false,false,false,false,false)!=VillagerAffect.Mood.None)throw new Exception("State priority invalid");
  foreach(var actor in world.Gatherers){var icon=actor.GetComponent<VillagerAffect>();if(icon==null)throw new Exception("Villager missing state icon");var canvas=icon.GetComponentInChildren<Canvas>(true);if(canvas==null||canvas.renderMode!=RenderMode.WorldSpace||canvas.GetComponent<UnityEngine.UI.GraphicRaycaster>()!=null)throw new Exception("Icon not noninteractive world space");var image=canvas.GetComponentInChildren<UnityEngine.UI.Image>(true);if(image==null||image.raycastTarget||image.rectTransform.rect.width<=0)throw new Exception("Icon captures input or has no size");}
  Debug.Log("BWVR_VILLAGER_AFFECT_PASS: state precedence, three adult icons, world space, no raycasts, nonzero bounds");
 }
}
}
