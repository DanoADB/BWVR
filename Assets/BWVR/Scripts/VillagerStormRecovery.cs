using System.Linq;
using UnityEngine;
namespace BWVR {
public sealed partial class VillagerExploration {
 StormChapter recoveryChapter;float recoveryElapsed,recoveryGesture;
 public bool StormThanking=>Current==Phase.StormThanks;
 public bool TryStormThanks(StormChapter chapter){
  if(!CanStartStructure||chapter==null||!chapter.NeedsStormThanks||social.Memory.injury>0||social.RecentFright||chapter.Creature.GetComponent<CarryItem>().held||Vector3.Distance(transform.position,chapter.Creature.position)>nav.world.Scale*80)return false;
  var side=transform.position-chapter.Creature.position;side.y=0;if(side.sqrMagnitude<1e-10f)side=-chapter.Creature.forward;
  if(!nav.world.Sample(chapter.Creature.position+side.normalized*nav.world.Scale*6,nav.profile,6,out var goal)||!world.Reachable(transform.position,goal,nav.profile))return false;
  routine.BeginHandsFreeActivity();nav.ResetRoute();recoveryChapter=chapter;recoveryElapsed=recoveryGesture=0;Current=Phase.StormThanks;motion?.LookAt(chapter.Creature);return true;
 }
 bool TickStormThanks(float dt){
  recoveryElapsed+=dt;var chapter=recoveryChapter;
  if(chapter==null||!chapter.NeedsStormThanks||chapter.Creature.GetComponent<CarryItem>().held||social.Memory.injury>0||social.RecentFright||recoveryElapsed>20||Vector3.Distance(transform.position,chapter.Creature.position)>nav.world.Scale*80){ResetTask();return false;}
  var side=transform.position-chapter.Creature.position;side.y=0;motion?.LookAt(chapter.Creature);
  if(side.magnitude<=nav.world.Scale*8){if(side.sqrMagnitude>1e-10f)transform.rotation=Quaternion.LookRotation(-side.normalized);motion?.Signal(true);recoveryGesture+=dt;if(recoveryGesture>=2&&chapter.ConfirmStormThanks(this)){motion?.React();IslandAudio.Cue(IslandSound.Content,transform.position,GetEntityId().GetHashCode(),.5f);ResetTask();Current=Phase.Celebrate;return true;}}
  else {recoveryGesture=0;motion?.Signal(false);nav.MoveTo(chapter.Creature.position+side.normalized*nav.world.Scale*6,transform.lossyScale.y*1.6f,dt);if(nav.Blocked&&recoveryElapsed>5){ResetTask();return false;}}
  return Active;
 }
 public bool TryStormReturnPlay(StormChapter chapter){
  if(!CanStartStructure||chapter==null||!chapter.WantsRecoveryPlay||social.Memory.injury>0||social.RecentFright||chapter.Creature.GetComponent<CarryItem>().held||Vector3.Distance(transform.position,chapter.Creature.position)>nav.world.Scale*80)return false;
  foreach(var ball in beachballs.Where(b=>b!=null&&Vector3.Distance(transform.position,b.transform.position)<nav.world.Scale*80).OrderBy(b=>(b.transform.position-transform.position).sqrMagnitude))if(TryBall(ball)){recoveryChapter=chapter;return true;}return false;
 }
 public bool StormInviteOffered(StormChapter chapter,BeachBall ball)=>recoveryChapter==chapter&&playingBall==ball&&ballInvitationOffered;
 void RecordStormInvitation(BeachBall ball){var chapter=recoveryChapter;if(chapter!=null)chapter.ConfirmStormInvitation(this,ball);}
 public void CancelStormRecovery(StormChapter chapter){if(recoveryChapter==chapter)ResetTask();}
 void ReleaseStormRecovery(){if(StormThanking)motion?.Signal(false);recoveryChapter=null;recoveryElapsed=recoveryGesture=0;}
}
}
