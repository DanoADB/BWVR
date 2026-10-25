using System;
using UnityEngine;
namespace BWVR {
// Motor practice and willingness are distinct: discouragement does not erase a skill.
[Serializable] public sealed class GentleBallSkill {
 public float control=.2f,preference=.5f,lastPractice=-100;public int practices;
 public float AimSpread=>Mathf.Lerp(5,.6f,control);
 public bool Practice(float time){if(time-lastPractice<12)return false;lastPractice=time;control=Mathf.Clamp01(control+(1-control)*.08f);practices=Math.Min(1000000,practices+1);return true;}
 public void Reinforce(bool positive){preference=Mathf.Clamp01(preference+(positive?.12f:-.12f));}
 public Vector3 Aim(Vector3 direction,int seed,int serial,string target){return Quaternion.Euler(0,AimSpread*CreatureDecisionModel.Variation(seed,serial,CreatureAction.PlayWithToy,target+"-gentle-aim"),0)*direction;}
 public bool Valid()=>HybridState.Number(control,0,1)&&HybridState.Number(preference,0,1)&&HybridState.Number(lastPractice,-100,100000000)&&practices>=0&&practices<=1000000;
}
}
