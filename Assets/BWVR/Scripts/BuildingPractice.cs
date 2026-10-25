using System;
using UnityEngine;
namespace BWVR {
// Dexterity follows completed physical placements, separately from taught preference.
[Serializable] public sealed class BuildingPractice {
 public float control=.15f;public int placements,corrections;
 public float PlacementSeconds=>Mathf.Lerp(2.6f,1.4f,control);
 public float Wobble=>Mathf.Lerp(.55f,.04f,control);
 public bool NeedsCorrection(int seed,int serial)=>
  (CreatureDecisionModel.Variation(seed,serial,CreatureAction.ConstructionPlay,"alignment")+1)*.5f<(1-control)*.7f;
 public Vector3 Offset(float elapsed,bool correction){
  float reach=Mathf.Clamp01(elapsed/PlacementSeconds);
  float settle=correction?1-Mathf.Clamp01((elapsed-PlacementSeconds)/.8f):0;
  return new Vector3(Mathf.Sin(elapsed*7)*Wobble*(1-reach)+settle*.95f,0,Mathf.Sin(elapsed*5)*Wobble*.4f*(1-reach));
 }
 public void Confirm(bool corrected){placements=Math.Min(1000000,placements+1);if(corrected)corrections=Math.Min(placements,corrections+1);control=Mathf.Clamp01(control+(1-control)*.07f);}
 public bool Valid()=>HybridState.Number(control,0,1)&&placements>=0&&placements<=1000000&&corrections>=0&&corrections<=placements;
}
}
