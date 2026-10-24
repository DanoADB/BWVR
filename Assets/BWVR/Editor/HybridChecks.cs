using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace BWVR.Editor {
public static class HybridChecks {
 [MenuItem("BWVR/Verify hybrid decision model")]
 public static void VerifyModel(){
  var memory=new HybridState();var candidates=new List<CreatureCandidate>{new CreatureCandidate{action=CreatureAction.WaterDelivery,situation=CreatureSituation.WaterShortage,target="cistern",need=.5f,learned=.5f},new CreatureCandidate{action=CreatureAction.PlayWithToy,situation=CreatureSituation.PlayOpportunity,target="toy",need=.5f,learned=.5f}};
  int baseline=Count(candidates,.5f),praised=Count(candidates,.98f),discouraged=Count(candidates,.02f);
  if(praised<baseline+130||discouraged>baseline-130)throw new Exception("Context preference has no material choice effect");
  for(int i=0;i<100;i++)if(CreatureDecisionModel.Select(candidates,34,i).action!=CreatureDecisionModel.Select(candidates,34,i).action)throw new Exception("Seeded selection differs");
  candidates[0].feasible=false;candidates[0].urgency=100;if(CreatureDecisionModel.Select(candidates,1,1).action==CreatureAction.WaterDelivery)throw new Exception("Infeasible action selected");
  memory.receipt=new ActionReceipt{eligible=true,action=CreatureAction.WaterDelivery,situation=CreatureSituation.WaterShortage,target="cistern"};if(!memory.Reinforce(true)||memory.Reinforce(false)||memory.Preference(CreatureSituation.WaterShortage,CreatureAction.WaterDelivery)<=.5f)throw new Exception("Context reinforcement attribution failed");
  memory.receipt=new ActionReceipt{eligible=true,action=CreatureAction.EatFood,situation=CreatureSituation.Hungry};memory.time=12.01f;if(memory.Reinforce(true))throw new Exception("Expired context reinforced");
  var saved=JsonUtility.FromJson<HybridState>(JsonUtility.ToJson(memory));if(!saved.Valid()||saved.associations.Length!=1||saved.associations[0].samples!=1)throw new Exception("Context memory round trip failed");saved.energy=float.NaN;if(saved.Valid())throw new Exception("Nonfinite energy accepted");
  var journal=new WorldEventJournal();for(int i=0;i<100;i++)journal.Add(i,"complete",CreatureAction.PlayWithToy,"toy","");if(journal.Count!=48||journal.Capture()[0].time!=52||journal.Latest.time!=99)throw new Exception("Journal is not a bounded ordered ring");
  Debug.Log($"BWVR_HYBRID_MODEL_PASS: fixed seed; feasible gates; reinforcement baseline={baseline}/500, praised={praised}/500, discouraged={discouraged}/500; one receipt; expiry; save validation; 48-event journal");
 }
 static int Count(List<CreatureCandidate> candidates,float preference){candidates[0].learned=preference;int count=0;for(int i=0;i<500;i++)if(CreatureDecisionModel.Select(candidates,1347,i).action==CreatureAction.WaterDelivery)count++;return count;}
}
}
