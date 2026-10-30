using UnityEngine;
namespace BWVR {
public sealed partial class VillagerExploration {
 VillageCulture culture;VillagerSocial saintTarget;float saintElapsed,saintGesture;
 public void ConfigureCulture(VillageCulture service){culture=service;}
 bool TrySaintVisit(){
  var belief=social.Memory.saint;if(culture==null||!CanStartStructure||belief==null||belief.confidence<.5f||culture.Season.paused||culture.Season.Hybrid.time<belief.nextVisit||social.RecentFright||social.Memory.injury>0)return false;
  var person=culture.Person(belief.chosen);if(person==null||person==social||person.GetComponent<CarryItem>().held||person.Memory.injury>0||person.RecentFright)return false;
  var delta=transform.position-person.transform.position;delta.y=0;if(delta.magnitude>nav.world.Scale*60)return false;if(delta.sqrMagnitude<1e-10f)delta=transform.forward;
  if(!nav.world.Sample(person.transform.position+delta.normalized*nav.world.Scale*4,nav.profile,5,out var at)||!world.Reachable(transform.position,at,nav.profile))return false;
  belief.nextVisit=culture.Season.Hybrid.time+180;saintTarget=person;saintElapsed=saintGesture=0;routine.BeginHandsFreeActivity();nav.ResetRoute();Current=Phase.SaintVisit;motion?.LookAt(person.transform);return true;
 }
 bool TickSaintVisit(float dt){
  saintElapsed+=dt;var person=saintTarget;if(person==null||!person.isActiveAndEnabled||person.GetComponent<CarryItem>().held||person.Memory.injury>0||person.RecentFright||social.Memory.injury>0||social.RecentFright||saintElapsed>20){ResetTask();return false;}
  var delta=transform.position-person.transform.position;delta.y=0;if(delta.magnitude>nav.world.Scale*65){ResetTask();return false;}motion?.LookAt(person.transform);
  if(delta.magnitude<nav.world.Scale*6){saintGesture+=dt;motion?.Signal(true);if(delta.sqrMagnitude>1e-10f)transform.rotation=Quaternion.LookRotation(-delta.normalized);if(saintGesture>=2){culture.SaintVisited(social,person);motion?.React();ResetTask();Current=Phase.Celebrate;return true;}}
  else{saintGesture=0;motion?.Signal(false);nav.MoveTo(person.transform.position+delta.normalized*nav.world.Scale*4,transform.lossyScale.y*1.6f,dt);if(nav.Blocked&&saintElapsed>5){ResetTask();return false;}}return Active;
 }
 void ReleaseSaintVisit(){if(saintTarget!=null)motion?.Signal(false);saintTarget=null;saintElapsed=saintGesture=0;}
}
}
