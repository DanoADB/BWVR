using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace BWVR.Editor {
public static class VillagerPlayChecks {
 [MenuItem("BWVR/Verify villager block play (Play mode)")]
 public static void Verify(){
  if(!EditorApplication.isPlaying)throw new Exception("Enter Play mode first");
  var site=UnityEngine.Object.FindObjectsByType<ConstructionSite>().First(s=>s.GetComponentInParent<IslandDiscoveries>()==null);var actors=UnityEngine.Object.FindObjectsByType<VillagerBlockPlay>();var play=actors[0];var actor=play.GetComponent<CarryItem>();var walker=play.GetComponent<IslandNavigator>();var creature=UnityEngine.Object.FindAnyObjectByType<CreatureCuriosity>();
  creature.ResetTask();foreach(var a in actors)a.ResetTask();site.ResetConstruction();site.Paused=false;var block=site.blocks[0];var point=site.World(new Vector3(-2,.6f,0));
  if(!site.Drop(block,point,site.transform.rotation))throw new Exception("Player test piece could not attach");
  if(play.Eligible(block)||play.TryBegin(block))throw new Exception("Villager targeted a player-built piece");
  if(!site.Drop(block,point,site.transform.rotation,true))throw new Exception("Creature test piece could not attach");
  block.Item.held=true;if(play.TryBegin(block))throw new Exception("Villager took a player-held piece");block.Item.held=false;
  if(!walker.world.Sample(site.World(new Vector3(-6,0,-2)),walker.profile,20,out var start))throw new Exception("No villager approach to tray");play.transform.position=walker.world.Terrain.Project(start);walker.ResetRoute();
  if(!play.TryBegin(block)||block.VillagerOwner!=play||block.Item.CanGrab)throw new Exception("Disassembly did not reserve its target");
  var phase=play.Current;site.Paused=true;play.Tick(5,true);site.Advance(.02f);site.Paused=false;if(play.Current!=phase||!block.Attached)throw new Exception("Paused villager disassembled a brick");
  int before=play.disassembled;var original=block.Body.position;var phases=new System.Collections.Generic.HashSet<VillagerBlockPlay.Phase>();
  for(int n=0;n<1500&&play.Active;n++){phases.Add(play.Current);play.Tick(.02f,false);site.Advance(.02f);}
  if(play.Active||play.disassembled!=before+1||!phases.Contains(VillagerBlockPlay.Phase.Pry)||!phases.Contains(VillagerBlockPlay.Phase.Push))throw new Exception("Villager did not finish visible disassembly");
  var displacement=block.Body.position-original;displacement.y=0;if(block.Attached||block.VillagerOwner!=null||block.Item.held||block.Body.isKinematic||displacement.magnitude<1)throw new Exception("Brick did not detach and move under physics");
  var second=site.blocks[1];if(!site.Drop(second,site.World(new Vector3(2,.6f,0)),site.transform.rotation,true))throw new Exception("Second test piece could not attach");
  foreach(var other in actors.Where(a=>a!=play)){other.Tick(200,false);if(other.Active)throw new Exception("Tray cooldown allowed another immediate disassembly");}
  var saved=new SliceState{construction=site.Capture()};var restored=JsonUtility.FromJson<SliceState>(JsonUtility.ToJson(saved));if(!restored.Valid()||!restored.construction[1].creatureBuilt)throw new Exception("Creature-build provenance did not survive save");
  if(!play.TryBegin(second))throw new Exception("Could not test interruption");actor.held=true;play.Tick(.02f,false);actor.held=false;if(play.Active||second.VillagerOwner!=null||!second.Attached)throw new Exception("Carried villager stranded a reservation or destroyed its target");
  if(!play.TryBegin(second))throw new Exception("Could not test disable cleanup");play.enabled=false;if(second.VillagerOwner!=null)throw new Exception("Disabled villager stranded a target");play.enabled=true;
  Debug.Log("BWVR_VILLAGER_PLAY_PASS: player-built and held protection; visible inspect/pry/push; physical displacement="+displacement.magnitude+"; pause; tray cooldown; saved provenance; carry/disable cleanup");
 }
}
}
