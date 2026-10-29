using UnityEngine;
namespace BWVR {
public sealed partial class CreatureCuriosity {
 StormChapter rescueChapter;int rescueStage;
 public bool TryBeginStormRescue(CreatureAffordance candidate){var chapter=candidate?.GetComponent<StormParticipant>()?.chapter;if(Current!=Phase.Rest||routine.Current!=CreatureRoutine.Task.SeekingWater||chapter==null||!chapter.ClaimCreature())return false;rescueChapter=chapter;routine.ResetTask();navigator.ResetRoute();target=candidate;owned=null;original=candidate.transform.position;originalRotation=candidate.transform.rotation;goalPlay=echoPlay=fetching=bowlingPlay=false;elapsed=clock=0;rescueStage=0;Current=Phase.StormRescue;motion?.LookAt(candidate.transform);return true;}
 bool TickStormRescue(float dt){
  if(rescueChapter==null||!rescueChapter.Pending||!rescueChapter.Claimed||elapsed>140||target==null||owned==null&&target.Item.held){Finish(false);return false;}
  Affect=CreatureAffect.Mood.Curious;
  if(rescueStage==0){if(navigator.MoveTo(target.transform.position,routine.speed,dt)){owned=target.Item;owned.held=true;owned.GetComponent<Collider>().enabled=false;motion?.Carry(owned.transform);rescueStage=1;clock=0;}else if(navigator.Blocked&&elapsed>8){Finish(false);return false;}}
  else if(rescueStage==1){if(clock>.8f){rescueStage=2;clock=0;}}
  else if(rescueStage==2){motion?.LookAt(rescueChapter.GatheringPoint);Affect=CreatureAffect.Mood.Happy;if(navigator.MoveTo(rescueChapter.Refuge,routine.speed*.75f,dt)){if(!SafeLanding(rescueChapter.Refuge,out landing)){Finish(false);return false;}rescueStage=3;clock=0;motion?.Activity(5);}else if(navigator.Blocked&&clock>10){Finish(false);return false;}}
  else {motion?.PlaceAt(landing,originalRotation,Mathf.Clamp01(clock));if(clock>1.1f){motion?.Carry(null);owned.transform.SetPositionAndRotation(landing,originalRotation);owned.held=false;owned.GetComponent<Collider>().enabled=true;owned.home=landing;owned.GetComponent<IslandNavigator>()?.ResetRoute();owned=null;bool success=rescueChapter.Confirm(RescueSource.Creature);if(success){friendsCarried++;lastWater=false;lastSocial=true;lastFriend=target.GetComponent<VillagerSocial>();feedbackWindow=12;}Finish(success);return false;}}
  return true;
 }
}
public sealed partial class VillagerExploration {
 StormChapter stormChapter;
 public bool StormEvacuating=>Current==Phase.StormEvacuation;
 public bool TryStormEvacuation(StormChapter chapter){if(chapter==null||!chapter.Pending||chapter.Person!=social||actor.held||Active||social.Active)return false;stormChapter=chapter;routine.BeginHandsFreeActivity();nav.ResetRoute();Current=Phase.StormEvacuation;return true;}
 bool TickStormEvacuation(float dt){if(stormChapter==null||!stormChapter.Pending||stormChapter.Claimed){ResetTask();return false;}motion?.LookAt(stormChapter.Creature);motion?.Signal(stormChapter.State.distress<20,true,stormChapter.Refuge);if(stormChapter.State.distress>=20){motion?.Signal(false);if(nav.MoveTo(stormChapter.Refuge,transform.lossyScale.y*1.6f,dt)){stormChapter.Confirm(RescueSource.Independent);return false;}}return true;}
 void ReleaseStormEvacuation(){stormChapter=null;}
}
}
