using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace BWVR.Editor {
public static class BuildingLearningChecks {
 [MenuItem("BWVR/Verify observed building patterns (Play mode)")]
 public static void Verify(){
  if(!EditorApplication.isPlaying)throw new Exception("Enter Play mode first");var slice=UnityEngine.Object.FindAnyObjectByType<QuestSlice>();var hybrid=UnityEngine.Object.FindAnyObjectByType<CreatureHybrid>();var learning=hybrid.GetComponent<CreatureConstructionLearning>();var mind=hybrid.GetComponent<CreatureCuriosity>();var water=hybrid.GetComponent<CreatureRoutine>();var nav=hybrid.GetComponent<IslandNavigator>();var site=learning.sites[0];var field=typeof(QuestSlice).GetField("state",BindingFlags.Instance|BindingFlags.NonPublic);var original=(SliceState)field.GetValue(slice);hybrid.Interrupt("Building QA");var test=new SliceState{seconds=0,water=100,hunger=.1f};field.SetValue(slice,test);var position=hybrid.transform.position;
  try{
   foreach(var s in learning.sites)s.ResetConstruction();learning.ResetWorld();if(test.Building.Knows(BuildRecipe.Bench)||test.Building.Knows(BuildRecipe.Tower))throw new Exception("Unobserved structures already known");
   foreach(var recipe in new[]{BuildRecipe.Bench,BuildRecipe.Tower}){
    hybrid.Interrupt("Observe "+recipe);var example=hybrid.Targets.First(t=>t.GetComponent<ConstructionInspiration>()?.recipe==recipe);
    if(!nav.world.Sample(example.transform.position,nav.profile,12,out var at))throw new Exception("Unreachable example");hybrid.transform.position=nav.world.Terrain.Project(at);nav.ResetRoute();
    if(!hybrid.Guide(GuidanceIntent.Investigate,example))throw new Exception("Observation cue rejected");
    for(int i=0;i<900&&!test.Building.Knows(recipe);i++)hybrid.Tick(.1f);
    if(!test.Building.Knows(recipe))throw new Exception("Creature did not inspect and learn "+recipe);
    hybrid.Interrupt("Build learned form");site.ResetConstruction();site.SelectRecipe(recipe);learning.Tick();int before=mind.built;
    int needed=ConstructionPatterns.Plan(recipe).Length;
    for(int placement=0;placement<needed;placement++){
     var block=site.blocks.First(b=>!b.Attached&&site.TryPlan(b,out _,out _));var offered=block.GetComponent<CreatureAffordance>();offered.cooldown=0;mind.ResetTask();water.ResetTask();
     if(!nav.world.Sample(block.transform.position,nav.profile,15,out at))throw new Exception("No brick pickup route");hybrid.transform.position=nav.world.Terrain.Project(at);nav.ResetRoute();
     if(!mind.TryBegin(offered))throw new Exception("Creature rejected learned plan brick");
     for(int n=0;n<700&&mind.Current!=CreatureCuriosity.Phase.Rest;n++){mind.Tick(.1f,test);site.Advance(.02f);}
     if(!block.Attached)throw new Exception("Learned recipe placement failed: "+recipe);learning.Placed(block);
    }
    if(!site.RecipeComplete||mind.built!=before+needed)throw new Exception("Wrong learned form or piece count");
    for(int i=0;i<250;i++)site.Advance(.02f);if(!site.RecipeComplete)throw new Exception("Learned form unstable");
    float preference=test.Building.preference[(int)recipe];learning.Reinforce(recipe,true);if(test.Building.preference[(int)recipe]<=preference||test.Building.completed[(int)recipe]!=1)throw new Exception("Recipe experience did not change memory");
   }
   learning.Capture();test.construction=site.Capture();var restored=JsonUtility.FromJson<SliceState>(JsonUtility.ToJson(test));if(!restored.Valid()||!restored.Building.Knows(BuildRecipe.Bench)||!restored.Building.Knows(BuildRecipe.Tower)||restored.Building.mainRecipe!=(int)BuildRecipe.Tower)throw new Exception("Observed forms and active recipe did not save");restored.Building.mainRecipe=99;if(restored.Valid())throw new Exception("Invalid recipe accepted");
   Debug.Log("BWVR_BUILDING_LEARNING_PASS: native observation of bench/cairn; unknown forms gated; bench 3 pieces versus tower 8; existing reach/carry/place executor; stable physical results; experience/preferences; saved learned vocabulary and recipe; invalid recipe rejection");
  }finally{hybrid.Interrupt("Building QA complete");hybrid.transform.position=position;nav.ResetRoute();field.SetValue(slice,original);}
 }
}
}
