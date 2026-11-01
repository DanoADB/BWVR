using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
namespace BWVR.Editor {
public static class ForestRegrowthChecks {
 static readonly BindingFlags F=BindingFlags.Instance|BindingFlags.NonPublic;
 static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
 static QuestSlice slice;static SliceState original,test;static VillageResources world;static GodToolPalette palette;static DivineIntervention divine;static GatheringSource node;static float timeScale;static bool unlimited,cursor;static Vector3[] positions;static bool[] held;static IslandNavigator[] actors;
 public static void Begin(){
  Check(slice==null,"Previous forest check unfinished");node=null;slice=UnityEngine.Object.FindAnyObjectByType<QuestSlice>();original=(SliceState)typeof(QuestSlice).GetField("state",F).GetValue(slice);world=slice.GetComponentInChildren<VillageResources>();palette=slice.GetComponentInChildren<GodToolPalette>();divine=slice.GetComponentInChildren<DivineIntervention>();unlimited=divine.tuning.unlimitedEnergy;divine.tuning.unlimitedEnergy=false;cursor=palette.UseDesktopCursor;palette.UseDesktopCursor=false;timeScale=Time.timeScale;Time.timeScale=0;
  try{
   slice.GetComponentInChildren<CreatureHybrid>().Interrupt("Forest brush QA");foreach(var a in world.Gatherers){a.Cancel();a.GetComponent<VillagerSocial>().ResetTask();a.GetComponent<VillagerExploration>().ResetTask();a.GetComponent<VillagerBlockPlay>().ResetTask();}
   test=new SliceState{developmentFocus=true,observeWithoutFoodWork=true};typeof(QuestSlice).GetField("state",F).SetValue(slice,test);world.ResetWorld();actors=slice.GetComponentsInChildren<IslandNavigator>();positions=actors.Select(a=>a.transform.position).ToArray();held=actors.Select(a=>a.GetComponent<CarryItem>().held).ToArray();foreach(var a in actors){a.transform.position=world.StoreApproach;a.GetComponent<CarryItem>().held=true;}
   node=world.sources.First(n=>n.kind==VillageResource.Wood&&world.Safe(n)&&world.Navigation.Terrain.Dry(n.at)&&Vector3.Distance(n.at,world.StoreApproach)>world.Navigation.Scale*15);
   var modelKey=node.tree.GetComponent<IslandPlacementView>().modelKey;Check(!string.IsNullOrEmpty(modelKey)&&node.sapling.GetComponentInChildren<MeshFilter>(true).sharedMesh==Resources.Load<EnvironmentArt>("EnvironmentArt").Find(modelKey).GetComponentInChildren<MeshFilter>(true).sharedMesh,"Sapling does not match mature tree species");
   for(int i=0;i<4;i++){Check(world.State.Harvest(0,node.id,3)==3,"Native finite harvest failed");if(i<3)world.State.Deposit(0);}world.Refresh();palette.Cancel();
  }catch{Restore();throw;}
 }
 public static string Exercise(Action<string,GatheringSource> capture=null){
  Check(slice!=null,"No forest fixture");if(world.Navigation.TreeNavigationPending)return "FOREST_ROUTES_PENDING";
  try{
   var brush=palette.Brush;var record=world.State.Find(node.id);var nav=world.Navigation;float energy=test.Hybrid.energy;Check(!brush.CanRegrow(node.at,out _)&&!record.growing,"Uncollected cargo erased by growth");world.State.Deposit(0);float wood=world.State.wood;
   string before=JsonUtility.ToJson(test);Check(brush.CanRegrow(node.at,out _)&&JsonUtility.ToJson(test)==before,"Preview mutated simulation");capture?.Invoke("stump",node);
   node.owner=world.Gatherers[1];Check(!brush.RegrowPulse(node.at),"Villager lease stolen");node.owner=null;node.tree.ResourceReserved=true;Check(!brush.RegrowPulse(node.at),"Tree reservation stolen");node.tree.ResourceReserved=false;
   var old=actors[0].transform.position;actors[0].transform.position=node.at;Check(!brush.RegrowPulse(node.at),"Regrowth introduced root beneath actor");actors[0].transform.position=old;
   test.paused=true;Check(!brush.RegrowPulse(node.at),"Paused forest tool mutated tree");test.paused=false;divine.Suspended=true;Check(!brush.RegrowPulse(node.at),"Suspended forest tool mutated tree");divine.Suspended=false;
   test.Hybrid.energy=2;Check(!brush.RegrowPulse(node.at)&&!record.growing&&record.plantings==0,"Unaffordable growth applied");test.Hybrid.energy=energy;
   Check(!brush.CanRegrow(new Vector3(float.NaN,0,0),out _)&&!brush.CanRegrow(node.at+Vector3.up,out _),"Invalid ground accepted");
   var ray=new Ray(node.at+Vector3.up*.2f,Vector3.down);palette.Select(SpiritualTool.RegrowTrees);Check(!palette.Apply(SpiritualTool.RegrowTrees,null,node.at),"Instant use bypassed held channel");palette.Process(ray,true,true,false,.02f,false);Check(record.plantings==0,"Selection press leaked into growth");palette.Process(ray,false,true,false,.02f,false);int pulses=brush.Pulses;palette.Process(ray,true,true,false,.02f,false);
   Check(record.growing&&record.plantings==1&&record.remaining==0&&brush.Pulses==pulses+1&&Mathf.Abs(test.Hybrid.energy-(energy-WorldBrush.RegrowCost))<.001f,"Native held tool did not plant one paid sapling");float growth=record.growth;palette.Process(ray,true,true,false,.02f,false);Check(record.growth==growth&&brush.Pulses==pulses+1,"Held frame duplicated a pulse");Check(node.sapling.gameObject.activeSelf&&!node.visual.gameObject.activeSelf&&node.tree.Harvested,"Sapling/stump/tree views conflict");capture?.Invoke("sapling",node);
   palette.Process(ray,true,false,false,.02f,true);Check(!brush.Active&&!palette.Selected.HasValue,"Tracking loss retained growth channel");
   typeof(QuestSlice).GetMethod("Save",F).Invoke(slice,null);typeof(QuestSlice).GetMethod("Load",F).Invoke(slice,null);test=(SliceState)typeof(QuestSlice).GetField("state",F).GetValue(slice);world.Refresh();record=world.State.Find(node.id);Check(test.Valid()&&record.growing&&Mathf.Abs(record.growth-growth)<.00001f&&record.plantings==1,"Native save/resume lost sapling");
   // Exhaustion latches until a real release; restoring energy while held does not restart.
   test.Hybrid.time+=1;test.Hybrid.energy=2;Check(!brush.ProcessRegrow(node.at,true,.05f),"Exhausted channel applied");test.Hybrid.energy=100;Check(!brush.ProcessRegrow(node.at,true,.05f)&&record.growth==growth,"Held exhausted tool auto-restarted");brush.ProcessRegrow(node.at,false,.05f);
   int paid=0;for(int i=0;i<16&&record.growing;i++){test.Hybrid.time+=WorldBrush.PulseSeconds;Check(brush.ProcessRegrow(node.at,true,.05f),"Growth pulse failed: "+brush.Reason);paid++;}Check(!record.growing&&!record.felled&&record.regrown==1&&record.plantings==1&&record.remaining==12&&world.State.wood==wood,"Growth minted stores or failed to restore one finite tree");Check(Mathf.Abs(test.Hybrid.energy-(100-paid*WorldBrush.RegrowCost))<.001f,"Growth charge duplicated");
   Check(!node.sapling.gameObject.activeSelf&&!node.visual.gameObject.activeSelf&&!node.tree.Harvested,"Mature tree did not replace sapling");test.Hybrid.time+=1;float completeEnergy=test.Hybrid.energy;Check(!brush.RegrowPulse(node.at)&&test.Hybrid.energy==completeEnergy&&record.regrown==1,"Mature tree farmed repeat growth");capture?.Invoke("mature",node);
   Check(test.Hybrid.Evidence.events.Count(e=>e.target==node.id&&e.source==EvidenceSource.PlayerMiracle)==paid+1,"Growth evidence misattributed or duplicated");Check(!slice.GetComponentInChildren<CreatureHybrid>().State.Evidence.events.Any(e=>e.target==node.id&&e.source==EvidenceSource.Autonomous),"Player growth fabricated creature work");palette.Cancel();
   typeof(QuestSlice).GetMethod("Save",F).Invoke(slice,null);typeof(QuestSlice).GetMethod("Load",F).Invoke(slice,null);test=(SliceState)typeof(QuestSlice).GetField("state",F).GetValue(slice);Check(test.Valid()&&test.Resources.Find(node.id).regrown==1&&test.Resources.Find(node.id).remaining==12,"Mature tree did not survive native reload");
   Debug.Log("BWVR_FOREST_REGROWTH_PASS: finite native harvest/cargo guards; read-only preview; leases/actors/pause/suspend/energy; real held tool/cooldown/tracking; saved sapling; exactly-once mature tree; paid local growth and player-only evidence");return "FOREST_REGROWTH_PASS";
  }finally{Restore();}
 }
 public static void Restore(){if(slice==null)return;palette.Cancel();divine.Suspended=false;divine.tuning.unlimitedEnergy=unlimited;palette.UseDesktopCursor=cursor;if(node!=null){node.owner=null;node.tree.ResourceReserved=false;}if(actors!=null)for(int i=0;i<actors.Length;i++){actors[i].transform.position=positions[i];actors[i].GetComponent<CarryItem>().held=held[i];}typeof(QuestSlice).GetField("state",F).SetValue(slice,original);world.ResetWorld();slice.GetComponentInChildren<CreatureHybrid>().NewSeason();Time.timeScale=timeScale;slice=null;}
}
}
