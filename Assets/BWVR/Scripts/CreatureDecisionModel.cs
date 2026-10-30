using System.Collections.Generic;
using UnityEngine;
namespace BWVR {
public sealed class CreatureCandidate {
 public CreatureAction action;public CreatureSituation situation;public string target;public bool feasible=true,timber,chase,echo,spin;public float attention,motivation,urgency,need,learned,guidance,affinity,novelty,interest,risk,travel,switching,timeout=120;public string expected="",reason="";
 public float Score=>Mathf.Clamp01(attention)+Mathf.Clamp(motivation,-1,1.5f)+Mathf.Clamp(urgency,0,3)+Mathf.Clamp01(need)+Mathf.Clamp01(learned)+Mathf.Clamp01(guidance)+Mathf.Clamp01(affinity)+Mathf.Clamp01(novelty)+Mathf.Clamp(interest,-.5f,.5f)-Mathf.Clamp01(risk)-Mathf.Clamp01(travel)-Mathf.Clamp01(switching);
 public string Explain=>$"{action}: need {need:F1}, urgency {urgency:F1}, learned {learned:F1}, favorite {interest:F1}, motivation {motivation:F1}, attention {attention:F1}, guidance {guidance:F1}, travel {travel:F1}";
}
// Pure scoring, with test-controlled variation. No scene searches or motor control.
public static class CreatureDecisionModel {
 public static CreatureCandidate Select(IReadOnlyList<CreatureCandidate> candidates,int seed,int serial){CreatureCandidate best=null;float score=float.NegativeInfinity;for(int i=0;i<candidates.Count;i++){var c=candidates[i];if(!c.feasible)continue;float value=c.Score+Variation(seed,serial,c.action,c.target)*.3f;if(value>score){score=value;best=c;}}return best;}
 public static float Variation(int seed,int serial,CreatureAction action,string target){unchecked{uint h=(uint)seed^(uint)serial*747796405u^(uint)action*2891336453u;foreach(char ch in target??"")h=(h^ch)*16777619u;h^=h>>16;h*=2246822519u;h^=h>>13;return (h&65535)/32767.5f-1;}}
}
}
