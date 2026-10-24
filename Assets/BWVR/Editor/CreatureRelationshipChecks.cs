using System;
using UnityEngine;
using UnityEditor;
namespace BWVR.Editor {
public static class CreatureRelationshipChecks {
 [MenuItem("BWVR/Verify remembered playmate model")]
 public static void Model(){
  var state=new HybridState();var a=new CreatureCandidate{action=CreatureAction.PlayWithToy,target="a",need=.5f};var b=new CreatureCandidate{action=CreatureAction.PlayWithToy,target="b",need=.5f};
  int Count(){int count=0;for(int i=0;i<500;i++)if(CreatureDecisionModel.Select(new[]{a,b},1347,i)==a)count++;return count;}
  int neutral=Count();for(int i=0;i<10;i++){state.time+=9;state.RememberBond("villager-0",true);}a.affinity=(state.PlaymateAffinity("villager-0")-.5f)*.5f;int favorite=Count();if(favorite<=neutral+50)throw new Exception("Familiar playmate has no meaningful decision effect");
  var bond=state.Bond("villager-0");int samples=bond.goodEncounters;if(state.RememberBond("villager-0",true)||bond.goodEncounters!=samples)throw new Exception("Repeated contact farms relationship");float before=state.PlaymateAffinity("villager-0");state.RememberBond("villager-0",false);if(state.PlaymateAffinity("villager-0")>=before)throw new Exception("Frightening encounter has no caution effect");
  var restored=JsonUtility.FromJson<HybridState>(JsonUtility.ToJson(state));if(!restored.Valid()||restored.bonds.Length!=1||restored.Bond("villager-0").badEncounters!=1)throw new Exception("Relationship memory did not save");restored.bonds[0].confidence=float.NaN;if(restored.Valid())throw new Exception("Invalid bond accepted");
  var legacy=JsonUtility.FromJson<HybridState>("{}");if(!legacy.Valid()||legacy.bonds.Length!=0)throw new Exception("Old saves lost compatibility");
  Debug.Log("BWVR_RELATIONSHIP_MODEL_PASS: favorite choices "+neutral+" -> "+favorite+"/500, bounded individual good/bad memories, deduplication and legacy/invalid save checks");
 }
}
}
