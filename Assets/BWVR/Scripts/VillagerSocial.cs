using System;
using UnityEngine;
namespace BWVR {
[Serializable] public sealed class SocialMemory {
 public float trust=.45f,affinity=.5f,boldness=.5f;
 public float injury,lastAssistance;
 public int greetings,rides,witnessed,disputes,discoveries,bridgeCrossings;
 public bool Valid()=>Unit(trust)&&Unit(affinity)&&Unit(boldness)&&Unit(injury)&&HybridState.Number(lastAssistance,0,100000000)&&Count(greetings)&&Count(rides)&&Count(witnessed)&&Count(disputes)&&Count(discoveries)&&Count(bridgeCrossings);
 static bool Unit(float n)=>!float.IsNaN(n)&&!float.IsInfinity(n)&&n>=0&&n<=1;
 static bool Count(int n)=>n>=0&&n<=1000000;
}
// Social attention competes with other intentions; memories belong to the saved season.
public sealed class VillagerSocial:MonoBehaviour {
 public enum Phase{Idle,Approach,Greet,Wait}
 public Phase Current{get;private set;}public bool Active=>Current!=Phase.Idle;
 public SocialMemory Memory=>season().Social(index);
 public bool Willing=>Memory.trust>=.4f&&Memory.injury<=0;
 public float Attraction=>Memory.affinity*.65f+Memory.trust*.35f;
 CreatureCuriosity creature;IslandNavigator walker;HumanoidMotion motion;CarryItem item;VillagerRoutine routine;
 Func<SliceState> season;int index;float cooldown,clock,elapsed;
 public void Initialize(CreatureCuriosity god,int identity,Func<SliceState> current){creature=god;index=identity;season=current;walker=GetComponent<IslandNavigator>();motion=GetComponent<HumanoidMotion>();item=GetComponent<CarryItem>();routine=GetComponent<VillagerRoutine>();cooldown=18+identity*13;}
 public bool TryBegin(){
  if(Active||item.held||creature.Current!=CreatureCuriosity.Phase.Rest||(GetComponent<VillagerExploration>()?.Active??false)||(GetComponent<VillagerBlockPlay>()?.Active??false))return false;
  var delta=transform.position-creature.transform.position;delta.y=0;
  if(delta.sqrMagnitude<1e-10f)delta=-creature.transform.forward;
  if(!walker.world.Sample(creature.transform.position+delta.normalized*walker.world.Scale*5,walker.profile,8,out _))return false;
  routine.ResetTask();walker.ResetRoute();Current=Phase.Approach;clock=elapsed=0;motion?.LookAt(creature.transform);return true;
 }
 public bool Tick(float dt,bool paused){
  if(paused||dt<=0)return Active;
  if(item.held){ResetTask();return false;}
  if(!Active){cooldown-=dt;if(cooldown<=0){if(!TryBegin())cooldown=6;}return Active;}
  clock+=dt;elapsed+=dt;
  if(elapsed>22||creature.Current!=CreatureCuriosity.Phase.Rest){ResetTask();return false;}
  var delta=transform.position-creature.transform.position;delta.y=0;
  if(Current==Phase.Approach){
   if(delta.sqrMagnitude<1e-10f)delta=-creature.transform.forward;
   if(walker.MoveTo(creature.transform.position+delta.normalized*walker.world.Scale*5,transform.lossyScale.y*1.6f,dt)){Current=Phase.Greet;clock=0;motion?.Signal(true);}
   if(walker.Blocked&&elapsed>5)ResetTask();
  }else if(Current==Phase.Greet){
   if(delta.magnitude>walker.world.Scale*9){ResetTask();return false;}
   Face();if(clock>=2){RememberGreeting();creature.NoticeGreeting(transform);motion?.React();motion?.Signal(false);Current=Phase.Wait;clock=0;}
  }else if(Current==Phase.Wait){
   Face();if(clock>=1.5f){var invite=GetComponent<CreatureAffordance>();bool willing=Willing;ResetTask();if(willing)creature.Offer(invite);}
  }
  return Active;
 }
 void Face(){var at=creature.transform.position-transform.position;at.y=0;if(at.sqrMagnitude>1e-10f)transform.rotation=Quaternion.LookRotation(at);}
 public void RememberGreeting(){var m=Memory;m.greetings=Math.Min(1000000,m.greetings+1);m.trust=Mathf.Clamp01(m.trust+.08f);m.affinity=Mathf.Clamp01(m.affinity+.025f);}
 public void RememberRide(){var m=Memory;m.rides=Math.Min(1000000,m.rides+1);m.trust=Mathf.Clamp01(m.trust+.12f);m.affinity=Mathf.Clamp01(m.affinity+.08f);}
 public void RememberDispute(){var m=Memory;m.disputes=Math.Min(1000000,m.disputes+1);m.affinity=Mathf.Clamp01(m.affinity-.12f);}
 public void WitnessRide(){var m=Memory;m.witnessed=Math.Min(1000000,m.witnessed+1);m.trust=Mathf.Clamp01(m.trust+.03f);}
 public void Feedback(bool positive){Memory.affinity=Mathf.Clamp01(Memory.affinity+(positive?.08f:-.08f));}
 public void ResetTask(){if(!Active)return;Current=Phase.Idle;clock=elapsed=0;cooldown=Mathf.Lerp(65,25,Memory.boldness)*Mathf.Lerp(1.5f,.75f,Memory.trust);motion?.Signal(false);motion?.LookAt(null);motion?.Activity(0);walker?.ResetRoute();routine?.ResetTask();}
 void OnDisable(){ResetTask();}
}
}
