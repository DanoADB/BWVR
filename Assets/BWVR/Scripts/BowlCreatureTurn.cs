using System.Linq;
using UnityEngine;
namespace BWVR {
// Temporary invitation and physical-shot evidence. Movement stays in the curiosity motor.
public sealed partial class BeachBall {
 BuildBowlProject turnProject,turnFlightProject;ConstructionBlock turnTarget,turnFlightTarget;ConstructionSite turnFlightSite;int turnAttempt,turnShot,turnCursor,turnFlightAttempt,turnFlightShot;float turnUntil,turnAge;bool turnHit;Vector3 turnStance;
 bool TurnContextValid=>turnProject!=null&&turnProject==creature.Season.buildBowl&&turnProject.phase==GatewayPhase.Active&&turnProject.attempts==turnAttempt&&turnProject.lastReturnedShot==turnShot&&turnProject.shots==turnShot&&turnProject.lastCreatureTurn<turnShot&&creature.State.time<turnUntil;
 public bool BowlCreaturePending=>turnFlightProject!=null;
 public bool BowlTurnOffered=>TurnContextValid&&turnTarget!=null&&turnTarget.Attached&&!turnTarget.Item.held&&!turnTarget.creatureBuilt;
 void ArmBowlCreatureTurn(BuildBowlProject p){ClearBowlCreatureTurn();if(p.phase!=GatewayPhase.Active)return;turnProject=p;turnAttempt=p.attempts;turnShot=p.lastReturnedShot;turnCursor=p.placementCursor;turnUntil=creature.State.time+75;}
 public void NoticeBowlRebuild(ConstructionBlock block){
  if(!TurnContextValid||creature.Season.paused||creature.GetComponent<CreatureCuriosity>().HandlingBeachBall||block==null||block.Site!=creature.GetComponent<CreatureConstructionLearning>().sites[0]||!block.Attached||block.Item.held||block.creatureBuilt||turnProject.placementCursor<=turnCursor)return;
  var receipt=creature.Season.placeContributions?.records.LastOrDefault(r=>r.site==0&&r.block==block.Site.blocks.IndexOf(block));if(receipt?.actor!="player"||receipt.id<=turnCursor)return;turnTarget=block;
  var away=transform.position-block.transform.position;away.y=0;if(away.sqrMagnitude<1e-10f)away=Vector3.back;bool found=false;
  for(int i=0;i<8;i++){var wanted=block.transform.position+Quaternion.Euler(0,i*45,0)*away.normalized*nav.Scale*24;if(!nav.Sample(wanted,nav.creature,2,out var point))continue;point=nav.ProjectFoot(point,nav.creature);if(!creature.CanReach(point)||!TryKnockdownAim(point,out _,false,block))continue;turnStance=point;found=true;break;}
  if(!found){turnTarget=null;return;}GetComponent<CreatureAffordance>().cooldown=0;creature.Journal.Add(creature.State.time,"creature course turn offered",CreatureAction.IdleOrObserve,StableId,"A witnessed fresh player snap offers a safe toy roll; no command or reward");
 }
 public bool TryBowlTurnStance(out Vector3 point){point=turnStance;return BowlTurnOffered&&!creature.Season.paused&&!BowlCreaturePending&&!FetchReserved&&!BuildBowlPending&&TryKnockdownAim(point,out _,false,turnTarget);}
 public bool TryBowlTurnAim(Vector3 actor,out Vector3 direction){direction=default;return BowlTurnOffered&&TryKnockdownAim(actor,out direction,false,turnTarget);}
 public void ClearBowlCreatureTurn(){turnProject=null;turnTarget=null;if(turnFlightSite!=null&&turnFlightSite.buildBowlBall==this)turnFlightSite.buildBowlBall=null;turnFlightProject=null;turnFlightTarget=null;turnFlightSite=null;turnAge=0;turnHit=false;}
 public void CancelCourseShot(){CancelBuildBowlShot();ClearBowlCreatureTurn();}
 void BeginBowlCreatureShot(BuildBowlProject p,ConstructionBlock target,int attempt,int shot){turnFlightProject=p;turnFlightTarget=target;turnFlightSite=target.Site;turnFlightAttempt=attempt;turnFlightShot=shot;turnFlightSite.buildBowlBall=this;turnAge=0;turnHit=false;}
 void BowlCreatureContact(ConstructionBlock block){if(!BowlCreaturePending||block!=turnFlightTarget||!creatureThrow||!thrown||Harmful||turnFlightProject!=creature.Season.buildBowl)return;turnHit=true;}
 void AdvanceBowlCreatureShot(float dt){
  if(!BowlCreaturePending)return;var p=turnFlightProject;var site=turnFlightSite;
  if(p!=creature.Season.buildBowl||p.phase!=GatewayPhase.Active||p.attempts!=turnFlightAttempt||p.shots!=turnFlightShot||item.held||Harmful||!creatureThrow||site.Paused||site.Locked||site.Repairer!=null||site.Visitor!=null||site.Requester!=null||site.blocks.Any(b=>b.Item.held||b.VillagerOwner!=null)){ClearBowlCreatureTurn();return;}
  turnAge+=dt;if(turnAge<12)return;bool hit=turnHit;int shot=turnFlightShot;ClearBowlCreatureTurn();if(p.RecordCreatureTurn(shot,hit)){creature.Journal.Add(creature.State.time,"creature course turn settled",CreatureAction.PlayWithToy,StableId,hit?"Safe physical hit on the rebuilt player piece":"Safe miss; player objectives unchanged");if(hit)IslandAudio.Cue(IslandSound.Discovery,transform.position,64916,.5f);}
 }
}
public sealed partial class QuestSlice {public void OfferBowlCreatureTurn(ConstructionBlock block){foreach(var ball in balls)ball.NoticeBowlRebuild(block);}}
}
