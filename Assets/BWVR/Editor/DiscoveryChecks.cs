using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace BWVR.Editor {
public static class DiscoveryChecks {
 [MenuItem("BWVR/Verify island discoveries (Play mode)")]
 public static void Verify(){
  if(!EditorApplication.isPlaying)throw new Exception("Enter Play mode first");
  var world=UnityEngine.Object.FindAnyObjectByType<IslandDiscoveries>();var mind=UnityEngine.Object.FindAnyObjectByType<CreatureCuriosity>();var nav=mind.GetComponent<IslandNavigator>();var water=mind.GetComponent<CreatureRoutine>();var explorer=UnityEngine.Object.FindObjectsByType<VillagerExploration>()[0];
  foreach(var a in UnityEngine.Object.FindObjectsByType<VillagerExploration>())a.ResetTask();foreach(var a in UnityEngine.Object.FindObjectsByType<VillagerSocial>())a.ResetTask();foreach(var a in UnityEngine.Object.FindObjectsByType<VillagerBlockPlay>())a.ResetTask();mind.ResetTask();water.ResetTask();
  if(world.points.Count!=6||world.points.Count(p=>p.construction!=null)!=2||world.points.Sum(p=>p.objects.Count)!=12||world.points.Where(p=>p.construction!=null).Sum(p=>p.construction.blocks.Count)!=16)throw new Exception("Discovery inventory incomplete");
  foreach(var p in world.points){p.Record.visitors=0;p.Record.restock=0;foreach(var o in p.objects){o.Refresh();if(!nav.world.Terrain.Dry(o.transform.position)||!world.Reachable(mind.transform.position,o.transform.position,nav.world.creature)||!world.Reachable(explorer.transform.position,o.transform.position,nav.world.villager))throw new Exception("Unreachable discovery "+p.id);}}
  var actorStart=explorer.transform.position;var creatureStart=mind.transform.position;var point=world.points[2];var tool=point.objects.First(o=>!o.treat);var actorNav=explorer.GetComponent<IslandNavigator>();
  if(!actorNav.world.Sample(tool.transform.position,actorNav.profile,8,out var at))throw new Exception("No explorer approach");explorer.transform.position=actorNav.world.Terrain.Project(at);actorNav.ResetRoute();int before=explorer.discoveries;
  if(!explorer.TryBegin(tool)||tool.GetComponent<CarryItem>().CanGrab||tool.GetComponent<CreatureAffordance>().Available)throw new Exception("Explorer reservation failed");
  var phase=explorer.Current;explorer.Tick(10,true);if(explorer.Current!=phase)throw new Exception("Paused exploration advanced");
  for(int i=0;i<3500&&explorer.Active;i++)explorer.Tick(.02f,false);
  if(explorer.Active||explorer.discoveries!=before+1||tool.Owner!=null||tool.GetComponent<CarryItem>().held||explorer.GetComponent<VillagerSocial>().Memory.discoveries<1)throw new Exception("Explorer did not enjoy and remember finding");
  if(point.Unseen(1)&&point.Unseen(2)&&point.Unseen(3))throw new Exception("Villager discovery not recorded");
  if(!explorer.TryBegin(tool))throw new Exception("No interruption check");explorer.GetComponent<CarryItem>().held=true;explorer.Tick(.1f,false);explorer.GetComponent<CarryItem>().held=false;if(explorer.Active||tool.Owner!=null)throw new Exception("Carried explorer stranded object");
  var treat=world.points[3].objects.First(o=>o.treat);var offer=treat.GetComponent<CreatureAffordance>();offer.cooldown=0;
  if(!nav.world.Sample(treat.transform.position,nav.profile,8,out at))throw new Exception("No creature treat approach");mind.transform.position=nav.world.Terrain.Project(at);nav.ResetRoute();mind.ResetTask();water.ResetTask();
  var season=new SliceState{seconds=0,water=100,hunger=.8f};if(!mind.TryBegin(offer))throw new Exception("Creature ignored treat");
  for(int i=0;i<700&&mind.Current!=CreatureCuriosity.Phase.Rest;i++)mind.Tick(.1f,season);
  world.Advance(.01f,false);
  if(mind.Current!=CreatureCuriosity.Phase.Rest||season.creatureDiscoveries!=1||season.hunger>.15f||mind.Affect!=CreatureAffect.Mood.Happy||treat.Available||treat.GetComponent<CarryItem>().CanGrab||treat.GetComponent<Collider>().enabled)throw new Exception("Treat did not reward and deplete safely");
  float preference=season.playPreference;mind.NoticeDiscovery(treat.point);if(season.creatureDiscoveries!=1||season.playPreference!=preference)throw new Exception("Repeated visit farmed discovery reward");
  float remaining=treat.point.Record.restock;world.Advance(20,true);if(treat.point.Record.restock!=remaining)throw new Exception("Paused treat respawned");world.Advance(90,false);if(!treat.Available||!treat.GetComponent<Collider>().enabled)throw new Exception("Treat did not replenish");
  var tray=world.points[0].construction;tray.ResetConstruction();if(!tray.Drop(tray.blocks[0],tray.World(new Vector3(-2,.6f,0)),tray.transform.rotation,true))throw new Exception("Exploration brick did not snap");world.Capture();
  var snapshot=new SliceState{discoveries=world.points.OrderBy(p=>p.id).Select(p=>p.Record).ToArray()};var restored=JsonUtility.FromJson<SliceState>(JsonUtility.ToJson(snapshot));if(!restored.Valid()||restored.discoveries[0].blocks.Length!=8||!restored.discoveries[0].blocks[0].creatureBuilt||restored.discoveries[3].visitors!=treat.point.Record.visitors)throw new Exception("Discoveries and caches did not save");
  restored.discoveries[0].restock=float.NaN;if(restored.Valid())throw new Exception("Invalid discovery timer accepted");if(!new SliceState().Valid())throw new Exception("Legacy season rejected");
  explorer.ResetTask();mind.ResetTask();water.ResetTask();explorer.transform.position=actorStart;actorNav.ResetRoute();mind.transform.position=creatureStart;nav.ResetRoute();
  var distant=world.points.OrderByDescending(p=>(p.transform.position-explorer.transform.position).sqrMagnitude).SelectMany(p=>p.objects).First(o=>!o.treat);before=explorer.discoveries;distant.point.Record.visitors=0;
  if(!explorer.TryBegin(distant))throw new Exception("Explorer would not set out for distant clearing");
  for(int i=0;i<9100&&explorer.Active;i++)explorer.Tick(.02f,false);
  if(explorer.Active||explorer.discoveries!=before+1)throw new Exception("Distant discovery timed out before arrival");
  explorer.transform.position=actorStart;actorNav.ResetRoute();
  Debug.Log("BWVR_DISCOVERY_PASS: six reachable clearings; 16 additional snap bricks; 6 tools and 6 treats; villager exploration/reward/ownership; creature delight and hunger; depletion/replenishment; pause; no repeated discovery farming; cache and memory saves; legacy validation");
 }
}
}
