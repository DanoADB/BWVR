using System;
using UnityEngine;
namespace BWVR {
public enum CreatureAction {WaterDelivery,EatFood,InvestigateObject,PlayWithToy,SocialInteraction,ConstructionPlay,RespondToGuidance,IdleOrObserve}
public enum CreatureSituation {WaterShortage,Hungry,HelpRequested,SafeContact,PlayOpportunity,ConstructionOpportunity,Calm}
public enum GuidanceIntent {Help,Protect,Investigate}
public enum GuidanceStatus {None,Active,Completed,Rejected,Expired,Superseded,Abandoned}
[Serializable] public sealed class LearnedAssociation {
 public CreatureSituation situation;public CreatureAction action;public float preference=.5f;public int samples;public float reinforcedAt;
}
[Serializable] public sealed class GuidanceState {
 public GuidanceIntent intention;public string target="";public float requestedAt,expiresAt;public GuidanceStatus status;public string reason="";
}
[Serializable] public sealed class ActionReceipt {
 public CreatureAction action;public CreatureSituation situation;public string target="";public float completedAt;public bool eligible,consumed;
 public int constructionRecipe=-1;
}
[Serializable] public sealed class CreatureIntention {
 public bool active;public CreatureAction action;public CreatureSituation situation;public string target="";public float startedAt,deadline;
}
[Serializable] public sealed class TargetCooldown {public string target;public float until;}
[Serializable] public sealed class HybridState {
 public float time,energy=60,lastBenefit=-100,rainRemaining,nextMiracleAt;public int seed=1347,decisions;public bool injuryIntroduced,energyInitialized;
 public GuidanceState guidance=new GuidanceState();public CreatureIntention intention=new CreatureIntention();public ActionReceipt receipt=new ActionReceipt();
 public LearnedAssociation[] associations=Array.Empty<LearnedAssociation>();
 public TargetCooldown[] cooldowns=Array.Empty<TargetCooldown>();
 public float Cooldown(string target){foreach(var c in cooldowns)if(c.target==target)return c.until;return 0;}
 public void RememberCooldown(string target,float until){foreach(var c in cooldowns)if(c.target==target){c.until=until;return;}if(cooldowns.Length>=128)return;Array.Resize(ref cooldowns,cooldowns.Length+1);cooldowns[cooldowns.Length-1]=new TargetCooldown{target=target,until=until};}
 public LearnedAssociation Association(CreatureSituation situation,CreatureAction action,bool create=false){foreach(var a in associations)if(a.situation==situation&&a.action==action)return a;if(!create||associations.Length>=24)return null;var next=new LearnedAssociation{situation=situation,action=action};Array.Resize(ref associations,associations.Length+1);associations[associations.Length-1]=next;return next;}
 public float Preference(CreatureSituation situation,CreatureAction action)=>Association(situation,action)?.preference??.5f;
 public bool Reinforce(bool positive){var r=receipt;if(!r.eligible||r.consumed||time-r.completedAt>12||time<r.completedAt)return false;var a=Association(r.situation,r.action,true);if(a==null)return false;a.preference=Mathf.Clamp01(a.preference+(positive?.12f:-.12f));a.samples=Math.Min(1000000,a.samples+1);a.reinforcedAt=time;r.consumed=true;return true;}
 public bool Valid(){
  if(!Number(time,0,100000000)||!Number(energy,0,100)||!Number(lastBenefit,-100,100000000)||!Number(rainRemaining,0,6)||!Number(nextMiracleAt,0,100000001)||decisions<0||decisions>100000000||guidance==null||intention==null||receipt==null||associations==null||associations.Length>24||cooldowns==null||cooldowns.Length>128)return false;
  var ids=new System.Collections.Generic.HashSet<string>();foreach(var c in cooldowns)if(c==null||!Text(c.target)||!ids.Add(c.target)||!Number(c.until,0,100000180))return false;
  if(!Enum.IsDefined(typeof(GuidanceIntent),guidance.intention)||!Enum.IsDefined(typeof(GuidanceStatus),guidance.status)||!Text(guidance.target)||!Text(guidance.reason)||!Number(guidance.requestedAt,0,time)||!Number(guidance.expiresAt,0,100000060))return false;
  if(!Enum.IsDefined(typeof(CreatureAction),intention.action)||!Enum.IsDefined(typeof(CreatureSituation),intention.situation)||!Text(intention.target)||!Number(intention.startedAt,0,time)||!Number(intention.deadline,0,100000300))return false;
  if(!Enum.IsDefined(typeof(CreatureAction),receipt.action)||!Enum.IsDefined(typeof(CreatureSituation),receipt.situation)||!Text(receipt.target)||!Number(receipt.completedAt,0,time)||receipt.constructionRecipe< -1||receipt.constructionRecipe>3)return false;
  var keys=new System.Collections.Generic.HashSet<int>();foreach(var a in associations)if(a==null||!Enum.IsDefined(typeof(CreatureSituation),a.situation)||!Enum.IsDefined(typeof(CreatureAction),a.action)||!Number(a.preference,0,1)||!Number(a.reinforcedAt,0,time)||a.samples<0||a.samples>1000000||!keys.Add((int)a.situation*16+(int)a.action))return false;return true;
 }
 public static bool Number(float value,float min,float max)=>!float.IsNaN(value)&&!float.IsInfinity(value)&&value>=min&&value<=max;
 static bool Text(string s)=>s!=null&&s.Length<=160;
}
}
