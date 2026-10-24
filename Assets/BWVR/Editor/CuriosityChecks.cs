using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace BWVR.Editor {
public static class CuriosityChecks {
 [MenuItem("BWVR/Verify creature curiosity and safe carrying (Play mode)")]
 public static void Verify(){
  if(!EditorApplication.isPlaying)throw new Exception("Enter Play mode first");
  var mind=UnityEngine.Object.FindAnyObjectByType<CreatureCuriosity>();var routine=mind.GetComponent<CreatureRoutine>();var nav=mind.GetComponent<IslandNavigator>();var motion=mind.GetComponentInChildren<HumanoidMotion>();
  var objects=UnityEngine.Object.FindObjectsByType<CreatureAffordance>(FindObjectsSortMode.None);var season=new SliceState();var start=mind.transform.position;
  try{
   mind.ResetTask();routine.ResetTask();
   foreach(var kind in new[]{CreatureAffordance.Kind.Roll,CreatureAffordance.Kind.Turn,CreatureAffordance.Kind.Chime,CreatureAffordance.Kind.Food,CreatureAffordance.Kind.Villager}){
    var target=objects.First(a=>a.kind==kind);target.cooldown=0;var original=target.transform.position;
    if(!nav.world.Sample(original,nav.profile,20,out var safe))throw new Exception("Unreachable affordance "+kind);
    mind.transform.position=nav.world.Terrain.Project(safe);nav.ResetRoute();mind.ResetTask();routine.ResetTask();
    int before=kind==CreatureAffordance.Kind.Villager?mind.friendsCarried:mind.played;bool carried=false;
    if(!mind.TryBegin(target))throw new Exception("Would not investigate "+kind);
    for(int n=0;n<500&&mind.Current!=CreatureCuriosity.Phase.Rest;n++){
     mind.Tick(.1f,season);
     if(mind.Holding!=null){
      carried=true;if(!target.Item.held||target.GetComponent<Collider>().enabled)throw new Exception("Target not exclusively reserved");
      if(motion!=null){motion.SendMessage("LateUpdate");var hands=motion.GetComponentInChildren<HumanoidHands>();hands.SendMessage("LateUpdate");}
      var phase=mind.Current;season.paused=true;mind.Tick(5,season);season.paused=false;if(mind.Current!=phase)throw new Exception("Paused curiosity advanced");
     }
    }
    if(!carried||(kind==CreatureAffordance.Kind.Villager?mind.friendsCarried:mind.played)!=before+1)throw new Exception("Interaction did not finish: "+kind);
    if(target.Item.held||!target.GetComponent<Collider>().enabled)throw new Exception("Target not released");
    if(kind==CreatureAffordance.Kind.Villager){var walker=target.GetComponent<IslandNavigator>();if(!walker.CanPlace(target.transform.position,out _))throw new Exception("Friend dropped outside walkable ground");var worker=target.GetComponent<VillagerRoutine>();int jobs=worker.completedJobs;for(int n=0;n<10000&&worker.completedJobs==jobs;n++)worker.Tick(.1f,false);if(worker.completedJobs!=jobs+1)throw new Exception("Carried villager did not resume work");}
   }
   var toy=objects.First(a=>a.kind==CreatureAffordance.Kind.Roll);toy.cooldown=0;mind.ResetTask();routine.ResetTask();toy.Item.held=true;
   if(mind.TryBegin(toy))throw new Exception("Took a player's held toy");toy.Item.held=false;
   mind.transform.position=nav.world.Terrain.Project(toy.transform.position);nav.ResetRoute();
   if(!mind.TryBegin(toy))throw new Exception("Could not retry toy");
   for(int n=0;n<100&&mind.Holding==null;n++)mind.Tick(.1f,season);
   mind.enabled=false;if(toy.Item.held||!toy.GetComponent<Collider>().enabled)throw new Exception("Interrupted carry stranded toy");mind.enabled=true;
   mind.ResetTask();routine.ResetTask();season.hunger=.9f;mind.Tick(.1f,season);if(mind.Affect!=CreatureAffect.Mood.Hungry)throw new Exception("Hunger has no affect signal");
   var restored=JsonUtility.FromJson<SliceState>(JsonUtility.ToJson(season));if(!restored.Valid()||restored.hunger!=season.hunger)throw new Exception("Curiosity needs did not survive save");
   Debug.Log("BWVR_CURIOSITY_PASS: all three toys, food, gentle villager carry/drop and resumed job; exclusive ownership; pause; interruption cleanup; hunger icon state; save round trip");
  }finally{mind.enabled=true;mind.ResetTask();routine.ResetTask();mind.transform.position=start;nav.ResetRoute();}
 }
}
}
