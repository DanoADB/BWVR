using System;
using UnityEngine;
namespace BWVR {
[Serializable] public sealed class InventedGameMemory {
 public int safeKnockdowns,invitations,replays;public float preference=.5f,nextInvite;
 public bool Discovered=>safeKnockdowns>=3;
 public bool Valid()=>safeKnockdowns>=0&&safeKnockdowns<=1000000&&invitations>=0&&invitations<=1000000&&replays>=0&&replays<=safeKnockdowns&&replays<=invitations&&HybridState.Number(preference,0,1)&&HybridState.Number(nextInvite,0,100000300);
 public void Confirm(bool replay){safeKnockdowns=Math.Min(1000000,safeKnockdowns+1);if(replay)replays=Math.Min(safeKnockdowns,Math.Min(invitations,replays+1));preference=Mathf.Clamp01(preference+.03f);}
 public void Retract(bool replay){safeKnockdowns=Math.Max(0,safeKnockdowns-1);if(replay)replays=Math.Max(0,replays-1);preference=Mathf.Clamp01(preference-.03f);}
}
public sealed partial class BeachBall {
 bool knockedThisThrow,inventedAttempt,inventionCredited;float inventionStarted;ConstructionBlock inventedTarget;
 public bool LastInventedOutcome=>inventionCredited&&inventedAttempt&&!Harmful;
 public float InventedAttraction=>TryBowlTurnStance(out _)?.55f:TryInventedAim(creature.transform.position,out _)? .3f+(creature.Season.worldPlay.invented.preference*.3f):0;
 void ResetInventionThrow(){knockedThisThrow=inventedAttempt=inventionCredited=false;inventedTarget=null;inventionStarted=creature!=null?creature.State.time:0;}
 void ConfirmInvention(){
  if(!knockedThisThrow||Harmful||!creatureThrow)return;var play=creature.Season.WorldPlay;var memory=play.invented??(play.invented=new InventedGameMemory());bool known=memory.Discovered;memory.Confirm(inventedAttempt);inventionCredited=true;
  creature.Journal.Add(creature.State.time,!known&&memory.Discovered?"game discovered":"safe knockdown",CreatureAction.PlayWithToy,StableId,"A safe throw toppled snapped toy pieces"+(inventedAttempt?" during a remembered game":""));
 }
 void RetractInvention(){if(!inventionCredited)return;creature.Season.worldPlay?.invented?.Retract(inventedAttempt);inventionCredited=false;var receipt=creature.State.receipt;if(receipt.knockdown&&receipt.target==StableId&&receipt.completedAt>=inventionStarted)creature.InvalidateLesson("Late harm invalidated the knockdown result");}
 public bool TryInventedAim(Vector3 actor,out Vector3 direction)=>TryKnockdownAim(actor,out direction,true);
 public bool TryExperimentalAim(Vector3 actor,out Vector3 direction)=>TryBowlTurnAim(actor,out direction)||TryKnockdownAim(actor,out direction,false);
 bool TryKnockdownAim(Vector3 actor,out Vector3 direction,bool known,ConstructionBlock only=null){
  direction=default;inventedTarget=null;var memory=creature?.Season.worldPlay?.invented;if(creature==null||known&&memory?.Discovered!=true||creature.Season.paused||InFlight||only==null&&creature.State.time<(memory?.nextInvite??0)||FetchReserved||PeerGame||Player!=null||Goals?.Active==true||Goals?.Turns.Active==true||Puzzle?.Active==true||item.held&&creature.GetComponent<CreatureCuriosity>().Holding!=item)return false;
  float best=45*45;foreach(var block in constructionBlocks){if(only!=null&&block!=only||!BlockContactAvailable(block)||!block.Attached||block.Site.IsBridge||block.Site.Visitor!=null||block.Site.Requester!=null||block.Site.Repairer!=null||block.Site.Recipe==BuildRecipe.Bench&&block.Site.RecipeComplete)continue;bool claimed=false;foreach(var b in block.Site.blocks)if(b.Item.held||b.VillagerOwner!=null){claimed=true;break;}if(claimed)continue;
   var point=block.Site.World(block.Body.position);var delta=point-actor;delta.y=0;float distance=delta.sqrMagnitude/(nav.Scale*nav.Scale);if(distance<13*13||distance>=best||!nav.SegmentClear(actor,point,nav.creature))continue;bool occupied=false;foreach(var person in villagers){if(person==null)continue;var offset=person.transform.position-actor;float t=Mathf.Clamp01(Vector3.Dot(offset,delta)/delta.sqrMagnitude);if(Vector3.Distance(person.transform.position,point)<nav.Scale*12||(offset-delta*t).magnitude<nav.Scale*(Radius+3)){occupied=true;break;}}if(occupied)continue;best=distance;inventedTarget=block;direction=delta.normalized;
  }return inventedTarget!=null;
 }
 public bool RollInventedGame(){
  if(!TryExperimentalAim(creature.transform.position,out var direction)||!nav.Sample(creature.transform.position+direction*nav.Scale*7,nav.creature,3,out var foot))return false;
  var target=inventedTarget;bool course=TryBowlTurnAim(creature.transform.position,out var courseDirection);if(course)direction=courseDirection;var courseProject=turnProject;var courseTarget=turnTarget;int courseAttempt=turnAttempt,courseShot=turnShot;if(course)target=courseTarget;var play=creature.Season.WorldPlay;var memory=play.invented??(play.invented=new InventedGameMemory());bool known=memory.Discovered;Throw(direction,true,14);if(course)BeginBowlCreatureShot(courseProject,courseTarget,courseAttempt,courseShot);inventedAttempt=known;if(known)memory.invitations=Math.Min(1000000,memory.invitations+1);memory.nextInvite=creature.State.time+(known?90:45);Body.position=nav.ToNav(nav.ProjectFoot(foot,nav.creature))+Vector3.up*(Radius+.1f);Body.linearVelocity=direction*14;Body.angularVelocity=Vector3.Cross(Vector3.up,direction)*14/Radius;Body.WakeUp();transform.position=nav.ToWorld(Body.position);creature.Journal.Add(creature.State.time,known?"game invitation":"play experiment",CreatureAction.PlayWithToy,StableId,known?"Beckoned toward toy blocks, then rolled to knock them down":"Tried a short roll at toy blocks");return target!=null;
 }
}
}
