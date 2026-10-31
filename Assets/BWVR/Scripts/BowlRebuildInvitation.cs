using System.Linq;
using UnityEngine;
namespace BWVR {
// A short nonverbal invitation, with no motor ownership, rewards or saved intention.
public sealed partial class CreatureHybrid {
 BuildBowlProject bowlRebuildProject;ConstructionSite bowlRebuildSite;int bowlRebuildAttempt,bowlRebuildShot,bowlRebuildReceipt;float bowlRebuildUntil,bowlRebuildCheck,bowlGestureUntil;bool bowlGestureShown,bowlGesturing;GatewayCues bowlRebuildCues;
 public ConstructionBlock BowlRebuildPiece=>bowlRebuildCues?.SuggestedPiece;
 public bool BowlRebuildGesturing=>bowlGesturing;
 public bool BowlRebuildPending=>bowlRebuildProject!=null&&bowlRebuildProject==Season.buildBowl&&bowlRebuildProject.phase==GatewayPhase.Active&&bowlRebuildProject.attempts==bowlRebuildAttempt&&bowlRebuildProject.shots==bowlRebuildShot&&bowlRebuildProject.placementCursor==bowlRebuildReceipt&&State.time<bowlRebuildUntil;
 public void OfferBowlRebuild(BuildBowlProject project,ConstructionSite site,int shot){
  if(project==null||project!=Season.buildBowl||project.phase!=GatewayPhase.Active||project.lastReturnedShot!=shot||project.returns==0||site!=GetComponent<CreatureConstructionLearning>()?.sites[0]||Season.paused||suspended)return;
  if(bowlRebuildProject==project&&bowlRebuildAttempt==project.attempts&&bowlRebuildShot==shot)return;ClearBowlRebuild();bowlRebuildProject=project;bowlRebuildSite=site;bowlRebuildAttempt=project.attempts;bowlRebuildShot=shot;bowlRebuildReceipt=project.placementCursor;bowlRebuildUntil=State.time+30;bowlRebuildCheck=0;
 }
 void StopBowlGesture(){if(bowlGesturing&&Owner==ActionOwner.None){motion?.Signal(false);motion?.LookAt(null);}bowlGesturing=false;}
 public void ClearBowlRebuild(){StopBowlGesture();bowlRebuildCues?.Hide();bowlRebuildProject=null;bowlRebuildSite=null;bowlGestureShown=false;}
 public void RefreshBowlRebuild(){
  if(!BowlRebuildPending){ClearBowlRebuild();return;}var site=bowlRebuildSite;
  if(Season.paused||suspended){StopBowlGesture();bowlRebuildCues?.Hide();return;}
  if(GetComponent<CarryItem>().held||site==null||site.Paused||site.Locked||site.BuildBowlReserved||site.RecipeComplete||site.Repairer!=null||site.Requester!=null||site.Visitor!=null||site.blocks.Any(b=>b.Item.held||b.VillagerOwner!=null)){ClearBowlRebuild();return;}
  if(Owner!=ActionOwner.None||curiosity.Current!=CreatureCuriosity.Phase.Rest||curiosity.FeedbackProtected){StopBowlGesture();bowlRebuildCues?.Hide();return;}
  if(bowlGesturing&&State.time>=bowlGestureUntil)StopBowlGesture();if(State.time<bowlRebuildCheck)return;bowlRebuildCheck=State.time+.25f;bowlRebuildCues?.Hide();if(!Season.Building.Knows(site.Recipe))return;
  foreach(var block in site.blocks){var target=block.GetComponent<HybridTarget>();if(!block.gameObject.activeInHierarchy||!block.RecipePiece||!block.CanBuild||target==null||Find(target.id)!=target||!CanSeeCue(target)||!site.TryRecipePlan(block,out var at,out var facing))continue;
   if(bowlRebuildCues==null)bowlRebuildCues=new GatewayCues(transform.parent);bowlRebuildCues.SuggestPiece(block,at,facing);
   if(!bowlGestureShown){bowlGestureShown=bowlGesturing=true;bowlGestureUntil=State.time+2;motion?.LookAt(block.transform);motion?.Signal(true,true,block.transform.position);Journal.Add(State.time,"rebuild invitation",CreatureAction.IdleOrObserve,target.id,"Pointed to a loose piece and feasible slot after physically returning the ball; no command or reward");}break;
  }
 }
 void DisposeBowlRebuild(){ClearBowlRebuild();bowlRebuildCues?.Dispose();bowlRebuildCues=null;}
}
}
