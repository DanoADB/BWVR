using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
namespace BWVR.Editor {
public static class BridgeChecks {
 [MenuItem("BWVR/Verify useful physical bridge (Play mode)")]
 public static void Verify(){
  if(!EditorApplication.isPlaying)throw new Exception("Enter Play mode first");var slice=UnityEngine.Object.FindAnyObjectByType<QuestSlice>();var hybrid=UnityEngine.Object.FindAnyObjectByType<CreatureHybrid>();var learning=hybrid.GetComponent<CreatureConstructionLearning>();var bridge=learning.Bridge;if(bridge==null)throw new Exception("No useful creek site found");var site=bridge.Site;var mind=hybrid.GetComponent<CreatureCuriosity>();var nav=hybrid.GetComponent<IslandNavigator>();var field=typeof(QuestSlice).GetField("state",BindingFlags.Instance|BindingFlags.NonPublic);var original=(SliceState)field.GetValue(slice);var godPosition=hybrid.transform.position;hybrid.Interrupt("Bridge QA");var state=new SliceState{seconds=0,water=100,hunger=.1f};field.SetValue(slice,state);var actor=UnityEngine.Object.FindObjectsByType<VillagerExploration>().First(a=>a.GetComponent<VillagerSocial>().Memory.injury<=0);var actorPosition=actor.transform.position;var walker=actor.GetComponent<IslandNavigator>();
  try{
   site.Paused=false;site.ResetConstruction();learning.ResetWorld();if(site.blocks.Any(b=>b.CanBuild)||bridge.Walkable)throw new Exception("Unobserved gap was buildable/walkable");
   if(!nav.world.Sample(bridge.BankA,nav.profile,8,out var start))throw new Exception("God cannot reach bank");hybrid.transform.position=nav.world.ProjectFoot(start,nav.profile);nav.ResetRoute();
   if(!hybrid.Guide(GuidanceIntent.Investigate,bridge.Example.GetComponent<HybridTarget>()))throw new Exception("Gap cue rejected");for(int i=0;i<1200&&!state.Building.Knows(BuildRecipe.Bridge);i++)hybrid.Tick(.1f);if(site.Locked)throw new Exception("Gap was not learned");hybrid.Interrupt("Build bridge");
   for(int i=0;i<3;i++){
    var block=site.blocks.First(b=>b.CanBuild);var a=block.GetComponent<CreatureAffordance>();a.cooldown=0;mind.ResetTask();if(!mind.TryBegin(a))throw new Exception("God rejected bridge piece");
    for(int n=0;n<1600&&mind.Current!=CreatureCuriosity.Phase.Rest;n++){mind.Tick(.1f,state);site.Advance(.02f);}if(!block.Attached)throw new Exception("Bridge placement failed at piece "+i+", "+hybrid.transform.position);learning.Placed(block);
    if(i<2&&bridge.Walkable)throw new Exception("Incomplete bridge opened navigation");
   }
   for(int i=0;i<250;i++)site.Advance(.02f);if(!site.RecipeComplete||!bridge.Walkable)throw new Exception("Complete bridge unstable");
   var path=new NavMeshPath();var filter=new NavMeshQueryFilter{agentTypeID=walker.profile.id,areaMask=1};if(!nav.world.Sample(bridge.BankA,walker.profile,1,out var aBank)||!nav.world.Sample(bridge.BankB,walker.profile,1,out var bBank)||!NavMesh.CalculatePath(nav.world.ToNav(aBank),nav.world.ToNav(bBank),filter,path)||path.status!=NavMeshPathStatus.PathComplete)throw new Exception("Bridge has no complete native path");float length=0;for(int i=1;i<path.corners.Length;i++)length+=Vector3.Distance(path.corners[i-1],path.corners[i]);if(length>=bridge.PreviousDistance*.85f)throw new Exception("Bridge did not shorten native route: "+length+" versus "+bridge.PreviousDistance);
   actor.ResetTask();actor.GetComponent<VillagerSocial>().ResetTask();actor.GetComponent<VillagerBlockPlay>().ResetTask();actor.transform.position=bridge.BankA;walker.ResetRoute();int before=actor.GetComponent<VillagerSocial>().Memory.bridgeCrossings;bool deck=false;if(!actor.TryBridge(bridge))throw new Exception("Villager rejected crossing");
   for(int i=0;i<3600&&actor.GetComponent<VillagerSocial>().Memory.bridgeCrossings==before;i++){actor.Tick(.05f,false);if(bridge.OverWater(actor.transform.position)){deck=true;if(!bridge.Project(actor.transform.position,out var raised)||Mathf.Abs(raised.y-actor.transform.position.y)>nav.world.Scale*.1f)throw new Exception("Villager flattened into water");}}
   if(!deck||actor.GetComponent<VillagerSocial>().Memory.bridgeCrossings!=before+1)throw new Exception("Native villager crossing did not finish");
   learning.Capture();var save=JsonUtility.FromJson<SliceState>(JsonUtility.ToJson(state));if(!save.Valid()||save.Building.bridge.Count(r=>r.support>=-1)!=3)throw new Exception("Bridge state did not save");
   actor.transform.position=site.World(new Vector3(0,.6f,0));site.blocks.First(b=>b.Attached&&b.size.x<3).Grab();if(bridge.Walkable||!nav.world.Terrain.Dry(actor.transform.position,false))throw new Exception("Broken bridge remained open or stranded villager");
   bridge.Restore(save.Building.bridge,true);if(!bridge.Walkable)throw new Exception("Saved bridge did not restore routing");
   Debug.Log("BWVR_BRIDGE_PASS: observed gap; existing native God reach/carry/place executor; three physical pieces; no incomplete passage; land route "+bridge.PreviousDistance.ToString("F1")+" -> "+length.ToString("F1")+"; actual native villager crossing above water; removal recovery; saved physical bridge and routing restoration");
  }finally{hybrid.Interrupt("Bridge QA complete");actor.ResetTask();actor.transform.position=actorPosition;walker.ResetRoute();hybrid.transform.position=godPosition;nav.ResetRoute();site.ResetConstruction();field.SetValue(slice,original);}
 }
}
}
