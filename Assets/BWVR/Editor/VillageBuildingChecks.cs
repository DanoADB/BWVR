using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace BWVR.Editor {
public static class VillageBuildingChecks {
 [MenuItem("BWVR/Verify independent village construction (Play mode)")]
 public static void Verify(){
  if(!EditorApplication.isPlaying)throw new Exception("Enter Play mode first");
  void Require(bool ok,string why){if(!ok)throw new Exception(why);}
  var slice=UnityEngine.Object.FindAnyObjectByType<QuestSlice>();var god=UnityEngine.Object.FindAnyObjectByType<CreatureHybrid>();var learning=god.GetComponent<CreatureConstructionLearning>();var site=learning.sites[0];var actors=slice.GetComponentsInChildren<VillagerExploration>();var positions=actors.Select(a=>a.transform.position).ToArray();var records=site.Capture();var recipe=site.Recipe;
  var godPosition=god.transform.position;var field=typeof(QuestSlice).GetField("state",BindingFlags.Instance|BindingFlags.NonPublic);var original=(SliceState)field.GetValue(slice);god.Interrupt("Village construction QA");foreach(var a in actors){a.ResetTask();a.GetComponent<VillagerSocial>().ResetTask();a.GetComponent<VillagerResourceGathering>().Cancel();a.GetComponent<VillagerBlockPlay>().ResetTask();}
  var test=new SliceState{water=100,crops=100,developmentFocus=true};field.SetValue(slice,test);
  void Stage(VillagerExploration a){var nav=a.GetComponent<IslandNavigator>();Require(nav.world.Sample(site.World(new Vector3(0,0,-12)),nav.profile,12,out var at),"No site approach");a.transform.position=nav.world.ProjectFoot(at,nav.profile);nav.ResetRoute();}
  void Step(VillagerExploration a){test.Hybrid.time+=.05f;a.Tick(.05f,false);site.Advance(.02f);}
  try{
   site.Paused=false;site.ResetConstruction();site.Recipe=BuildRecipe.Arch;foreach(var a in actors)Stage(a);
   Require(!actors[0].TryVillageBuild(site),"Construction started without need");var demander=actors[0];var memory=demander.GetComponent<VillagerSocial>().Memory;memory.restNeed=.9f;
   // Let the idle leisure chooser discover the unmet gathering need.
   demander.Tick(36,false);Require(demander.Current==VillagerExploration.Phase.NeedApproach,"Idle villager did not express gathering need");for(int i=0;i<2000&&demander.Current!=VillagerExploration.Phase.ShowNeed;i++)Step(demander);Require(demander.ShowingBuildingNeed&&memory.gatheringSite==0,"Need did not identify its stable site");
   for(int i=0;i<500&&demander.Current==VillagerExploration.Phase.ShowNeed;i++)Step(demander);Require(demander.Current==VillagerExploration.Phase.RepairFetch&&site.Recipe==BuildRecipe.Bench,"Demonstration did not lead to independent construction");
   Require(!actors[1].TryVillageBuild(site),"Two villagers claimed same site");var phase=demander.Current;demander.Tick(10,true);Require(demander.Current==phase&&site.Snapped==0,"Pause advanced building");
   for(int i=0;i<4000&&memory.builtPieces==0;i++)Step(demander);Require(memory.builtPieces==1&&memory.repairs==0&&site.Snapped==1&&site.Repairer==null,"First native piece failed or was counted as repair");demander.ResetTask();
   learning.Capture();test.construction=site.Capture();var saved=JsonUtility.FromJson<SliceState>(JsonUtility.ToJson(test));Require(saved.Valid()&&saved.Social(0).gatheringSite==0&&saved.Social(0).builtPieces==1&&saved.construction.Count(r=>r.support>=-1)==1,"Partial project did not save");site.Restore(saved.construction);field.SetValue(slice,saved);test=saved;test.developmentFocus=true;
   // Other residents can contribute without making their own duplicate request.
   for(int turn=1;turn<=2;turn++){var helper=actors[turn];Stage(helper);helper.Tick(36,false);Require(helper.Current==VillagerExploration.Phase.RepairFetch,"Neighbor did not choose to help");for(int i=0;i<4000&&helper.GetComponent<VillagerSocial>().Memory.builtPieces==0;i++)Step(helper);Require(helper.GetComponent<VillagerSocial>().Memory.builtPieces==1,"Neighbor native carry/placement failed");helper.ResetTask();}
   Require(site.RecipeComplete&&test.relationships.Sum(m=>m.builtPieces)==3&&test.relationships.Sum(m=>m.gatheringsBuilt)==1&&site.blocks.Where(b=>b.Attached).All(b=>!b.creatureBuilt),"Cooperative bench or attribution failed");
   Require(test.placeContributions?.records.Count(r=>r.actor.StartsWith("villager-")&&r.site==0&&!r.repair)==3,"Cooperative contributors not recorded");
   Stage(demander);Require(demander.TryStructure(site),"Village rejected its own construction");for(int i=0;i<3000&&demander.GetComponent<VillagerSocial>().Memory.constructionUses==0;i++)Step(demander);Require(demander.GetComponent<VillagerSocial>().Memory.restNeed<.9f&&demander.GetComponent<VillagerSocial>().Memory.gatheringSite==-1,"Completed bench did not satisfy its need");
   // A mixed project keeps the creature's own knowledge and provenance intact.
   // This fixture ticks only the contributor. Idle witnesses cannot step aside,
   // so park them outside the creature's work approach rather than inside it.
   for(int i=0;i<actors.Length;i++){var a=actors[i];a.ResetTask();var n=a.GetComponent<IslandNavigator>();Require(n.world.Sample(site.World(new Vector3(24+i*4,0,-24)),n.profile,12,out var parked),"No witness parking");a.transform.position=n.world.ProjectFoot(parked,n.profile);n.ResetRoute();}site.ResetConstruction();site.Recipe=BuildRecipe.Bench;
   foreach(var p in ConstructionPatterns.Plan(BuildRecipe.Bench).Where(p=>!p.beam)){var block=site.blocks.First(b=>!b.Attached&&b.size.x<3);Require(site.Drop(block,site.World(p.position),site.transform.rotation),"Mixed supports failed");for(int i=0;i<5;i++)site.Advance(.02f);}
   var mind=god.GetComponent<CreatureCuriosity>();var beam=site.blocks.First(b=>b.Shape==ConstructionShape.Beam&&!b.Attached);var prop=beam.GetComponent<CreatureAffordance>();prop.cooldown=0;test.Building.observed=1;Require(!mind.TryBegin(prop),"Villager project silently taught creature a recipe");
   learning.Observe(god.Targets.Select(t=>t.GetComponent<ConstructionInspiration>()).First(e=>e!=null&&e.recipe==BuildRecipe.Bench));var godNav=god.GetComponent<IslandNavigator>();Require(godNav.world.Sample(site.World(new Vector3(0,0,-12)),godNav.profile,12,out var approach),"No creature work approach");god.transform.position=godNav.world.ProjectFoot(approach,godNav.profile);godNav.ResetRoute();Require(mind.TryBegin(prop),"Knowledgeable creature could not contribute");for(int i=0;i<2000&&mind.Current!=CreatureCuriosity.Phase.Rest;i++){mind.Tick(.05f,test,false);site.Advance(.02f);}Require(site.RecipeComplete&&beam.creatureBuilt&&site.blocks.Count(b=>b.Attached&&!b.creatureBuilt)==2,"Mixed native creature placement/provenance failed");
   Require(test.placeContributions.records.Any(r=>r.actor=="creature"&&r.site==0),"Native creature contribution missing");
   Debug.Log("BWVR_VILLAGE_BUILDING_PASS: idle need and gestures; stable project; independent native placement; pause/lease; partial save/reload; neighbors contribute native carry/snap turns; useful completed bench; mixed creature contribution respects learning and provenance");
  }finally{foreach(var a in actors)a.ResetTask();god.Interrupt("Village construction QA complete");god.transform.position=godPosition;god.GetComponent<IslandNavigator>().ResetRoute();site.Recipe=recipe;site.Restore(records);for(int i=0;i<actors.Length;i++){actors[i].transform.position=positions[i];actors[i].GetComponent<IslandNavigator>().ResetRoute();}field.SetValue(slice,original);}
 }
}
}
