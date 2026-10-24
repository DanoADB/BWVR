using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace BWVR.Editor {
public static class ConstructionChecks {
 [MenuItem("BWVR/Verify physical construction (Play mode)")]
 public static void Verify(){
  if(!EditorApplication.isPlaying)throw new Exception("Enter Play mode first");
  var site=UnityEngine.Object.FindObjectsByType<ConstructionSite>().First(s=>s.GetComponentInParent<IslandDiscoveries>()==null);if(site.Snapped!=0)throw new Exception("Run on a fresh construction tray");site.Paused=false;site.SelectRecipe(BuildRecipe.Arch);
  var mind=UnityEngine.Object.FindAnyObjectByType<CreatureCuriosity>();var routine=mind.GetComponent<CreatureRoutine>();mind.ResetTask();routine.ResetTask();
  var loose=site.blocks[0];loose.Body.position=loose.spawn+Vector3.up*2;loose.Body.transform.position=loose.Body.position;float before=loose.Body.position.y;for(int i=0;i<100;i++)site.Advance(.02f);
  if(loose.Body.position.y>before-.02f||Mathf.Abs(loose.Body.position.y-.6f)>.03f)throw new Exception("Loose brick did not fall and settle");
  if(site.FindSnap(loose,site.World(new Vector3(-2,5,0)),site.transform.rotation,out _,out _,out _))throw new Exception("Unsupported brick snapped in midair");
  if(site.FindSnap(loose,site.World(new Vector3(-2,.6f,0)),Quaternion.Euler(42,0,0),out _,out _,out _))throw new Exception("Tilted brick accepted");
  var season=new SliceState();var start=mind.transform.position;int built=mind.built;
  try{
   // Start close to the tray but keep all routing and reach/place phases active.
   var walker=mind.GetComponent<IslandNavigator>();if(!walker.world.Sample(site.World(new Vector3(0,0,-12)),walker.profile,20,out var safe))throw new Exception("No creature approach to construction");mind.transform.position=walker.world.Terrain.Project(safe);walker.ResetRoute();
   for(int piece=0;piece<8;piece++){
    var block=site.blocks.First(b=>!b.Attached&&b.CanBuild);var affordance=block.GetComponent<CreatureAffordance>();affordance.cooldown=0;routine.ResetTask();
    if(!mind.TryBegin(affordance))throw new Exception("Creature refused available brick");
    bool placement=false;
    for(int n=0;n<500&&mind.Current!=CreatureCuriosity.Phase.Rest;n++){
     mind.Tick(.1f,season);site.Advance(.02f);placement|=mind.Current==CreatureCuriosity.Phase.BuildPlace;
    }
    if(!placement||mind.built!=built+piece+1||!block.Attached||block.Item.held||!block.creatureBuilt)throw new Exception("Creature failed to attach brick "+piece+" at phase "+mind.Current);
    if(site.blocks.Count(b=>b.Item.held)>0)throw new Exception("Construction stranded a held piece");
   }
   var positions=site.blocks.Select(b=>b.Body.position).ToArray();for(int i=0;i<250;i++)site.Advance(.02f);
   if(site.Snapped!=8||site.blocks.Select((b,i)=>Vector3.Distance(b.Body.position,positions[i])).Max()>.02f)throw new Exception("Finished arch unstable");
   season.construction=site.Capture();var restored=JsonUtility.FromJson<SliceState>(JsonUtility.ToJson(season));if(!restored.Valid())throw new Exception("Construction save rejected");site.Restore(restored.construction);for(int i=0;i<100;i++)site.Advance(.02f);
   if(site.Snapped!=8||site.blocks.Select((b,i)=>Vector3.Distance(b.Body.position,positions[i])).Max()>.02f)throw new Exception("Restored assembly unstable");
   restored.construction[0].support=0;if(restored.Valid())throw new Exception("Cyclic connection accepted in save data");
   site.Paused=true;loose.Body.AddForce(Vector3.right*10,ForceMode.Impulse);var still=loose.Body.position;site.Advance(.02f);if(loose.Body.position!=still)throw new Exception("Paused physics advanced");site.Paused=false;site.SelectRecipe(BuildRecipe.Arch);
   var probe=site.blocks[7];if(site.FindSnap(probe,site.World(new Vector3(-2,.6f,0)),site.transform.rotation,out _,out _,out _))throw new Exception("Occupied connection accepted");
   // Removing a support detaches only its direct connections, leaving physics to settle the rest.
   var support=site.blocks.First(b=>b.Attached&&b.Body.position.y<1);support.Item.held=true;support.Grab();
   if(support.Attached||site.blocks.Any(b=>b.Connection!=null&&b.Connection.connectedBody==support.Body))throw new Exception("Grab did not detach brick and its direct connections");support.CancelHold();
   Debug.Log("BWVR_CONSTRUCTION_PASS: gravity and settling; unsupported/tilted/occupied rejection; creature assembled all eight arch pieces; five-second stability; save/resume and corrupt graph rejection; pause; detachable connections");
  }finally{mind.ResetTask();routine.ResetTask();mind.transform.position=start;mind.GetComponent<IslandNavigator>().ResetRoute();site.Paused=true;}
 }
}
}
