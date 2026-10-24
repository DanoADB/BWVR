using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace BWVR.Editor {
public static class SocialChecks {
 [MenuItem("BWVR/Verify social learning (Play mode)")]
 public static void Verify(){
  if(!EditorApplication.isPlaying)throw new Exception("Enter Play mode first");
  var actors=UnityEngine.Object.FindObjectsByType<VillagerSocial>();var creature=UnityEngine.Object.FindAnyObjectByType<CreatureCuriosity>();var water=creature.GetComponent<CreatureRoutine>();
  foreach(var a in actors){a.ResetTask();a.GetComponent<VillagerBlockPlay>().ResetTask();}creature.ResetTask();water.ResetTask();
  var actor=actors[0];var memory=actor.Memory;var offer=actor.GetComponent<CreatureAffordance>();offer.cooldown=0;
  float initial=memory.trust;memory.trust=.2f;
  if(offer.Available||creature.TryBegin(offer))throw new Exception("Creature ignored unwilling villager");
  var nav=actor.GetComponent<IslandNavigator>();var original=actor.transform.position;
  if(!nav.world.Sample(creature.transform.position+creature.transform.right*nav.world.Scale*5,nav.profile,15,out var at))throw new Exception("No greeting approach");
  actor.transform.position=nav.world.Terrain.Project(at);nav.ResetRoute();
  int greetings=memory.greetings;if(!actor.TryBegin())throw new Exception("Villager could not initiate greeting");
  var phase=actor.Current;actor.Tick(10,true);if(actor.Current!=phase||memory.greetings!=greetings)throw new Exception("Paused social intent advanced");
  for(int i=0;i<1100&&actor.Active;i++)actor.Tick(.02f,false);
  if(actor.Active||memory.greetings!=greetings+1||memory.trust<=.2f)throw new Exception("Greeting did not finish and teach trust");
  if(offer.Available)throw new Exception("Single greeting erased cautious villager boundary");
  memory.trust=initial;offer.cooldown=0;creature.ResetTask();water.ResetTask();
  int rides=memory.rides;float affinity=memory.affinity;
  var witness=actors.First(a=>a!=actor);var witnessOriginal=witness.transform.position;
  witness.transform.position=creature.transform.position+creature.transform.right*nav.world.Scale*7;
  int observed=witness.Memory.witnessed;
  if(!creature.TryBegin(offer))throw new Exception("Willing villager could not play with creature");
  var season=new SliceState{seconds=0,water=100};
  for(int i=0;i<1300&&creature.Current!=CreatureCuriosity.Phase.Rest;i++)creature.Tick(.02f,season);
  if(creature.Current!=CreatureCuriosity.Phase.Rest||memory.rides!=rides+1||memory.affinity<=affinity||actor.GetComponent<CarryItem>().held)throw new Exception("Friendly carry did not teach and safely release");
  if(witness.Memory.witnessed!=observed+1)throw new Exception("Nearby villager did not learn by observing safe ride");
  float before=memory.affinity;creature.Feedback(true);if(memory.affinity<=before)throw new Exception("Creature feedback did not reinforce individual preference");
  before=memory.affinity;creature.NoticeDisassembly(actor.transform);if(memory.affinity>=before||memory.disputes==0)throw new Exception("Dispute had no individual consequence");
  var snapshot=new SliceState{relationships=actors.Select(a=>a.Memory).ToArray()};var restored=JsonUtility.FromJson<SliceState>(JsonUtility.ToJson(snapshot));
  if(!restored.Valid()||restored.relationships[0].rides!=memory.rides)throw new Exception("Relationship memory did not round trip");
  restored.relationships[0].trust=float.NaN;if(restored.Valid())throw new Exception("Nonfinite relationship accepted");
  if(!JsonUtility.FromJson<SliceState>(JsonUtility.ToJson(new SliceState())).Valid())throw new Exception("Legacy empty relationship array rejected");
  creature.ResetTask();water.ResetTask();actor.ResetTask();actor.Tick(100,false);
  if(!actor.Active)throw new Exception("Social cooldown never initiated a villager intention");
  actor.GetComponent<CarryItem>().held=true;actor.Tick(.02f,false);actor.GetComponent<CarryItem>().held=false;
  if(actor.Active)throw new Exception("Player carry failed to interrupt greeting");
  creature.ResetTask();water.ResetTask();actor.ResetTask();actor.transform.position=original;nav.ResetRoute();witness.transform.position=witnessOriginal;witness.GetComponent<IslandNavigator>().ResetRoute();
  Debug.Log("BWVR_SOCIAL_PASS: autonomous greeting; pause; consent; safe carry; nearby observational trust; individual feedback/dispute; save validation and legacy support");
 }
}
}
