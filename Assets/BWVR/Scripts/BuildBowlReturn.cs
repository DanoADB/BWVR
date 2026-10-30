using UnityEngine;
namespace BWVR {
// A temporary destination for the existing ticketed fetch motor, not a second action controller.
public sealed partial class BeachBall {
 BuildBowlProject bowlReturnProject;int bowlReturnAttempt,bowlReturnShot;Vector3 bowlReturnPoint;
 bool BowlReturnContextValid=>bowlReturnProject==null||bowlReturnProject==creature.Season.buildBowl&&bowlReturnProject.attempts==bowlReturnAttempt&&(bowlReturnProject.phase==GatewayPhase.Active||bowlReturnProject.phase==GatewayPhase.Complete)&&bowlReturnShot==bowlReturnProject.shots&&bowlReturnShot>bowlReturnProject.lastReturnedShot;
 public bool BuildBowlReturnRequested=>bowlReturnProject!=null&&BowlReturnContextValid&&FetchReserved;
 void ClearBuildBowlReturn(){bowlReturnProject=null;bowlReturnAttempt=bowlReturnShot=0;bowlReturnPoint=default;}
 public void CancelBuildBowlReturn(){if(bowlReturnProject!=null)CancelFetch();}
 bool BowlReturnPointClear(Vector3 point,ConstructionSite site){
  if(!nav.Sample(point,nav.creature,2,out var safe)||(safe-point).sqrMagnitude>nav.Scale*nav.Scale*25)return false;
  foreach(var block in site.blocks){if(!block.Attached&&!block.Item.held)continue;var delta=block.transform.position-point;delta.y=0;if(delta.magnitude<nav.Scale*(Radius+block.size.magnitude*.5f+2))return false;}
  foreach(var person in villagers){if(person==null||person.GetComponent<CarryItem>()?.held==true)continue;var delta=person.transform.position-point;delta.y=0;if(delta.magnitude<nav.Scale*(Radius+nav.villager.radius+2))return false;}return true;
 }
 bool FindBowlReturnPoint(Vector3 origin,ConstructionSite site,out Vector3 point){
  for(int i=0;i<17;i++){var wanted=origin;if(i>0){float angle=(i-1)*45*Mathf.Deg2Rad;wanted+=new Vector3(Mathf.Cos(angle),0,Mathf.Sin(angle))*nav.Scale*(i<=8?6:12);}if(!nav.Sample(wanted,nav.creature,2,out var safe))continue;safe=nav.ProjectFoot(safe,nav.creature);if(site.Contains(site.Local(safe)))safe.y=Mathf.Max(safe.y,site.transform.position.y);if(!BowlReturnPointClear(safe,site))continue;point=safe;return true;}point=default;return false;
 }
 void OfferBuildBowlReturn(BuildBowlProject project,Vector3 origin){
  if(item.held||Harmful||creature.Season.paused||project!=creature.Season.buildBowl||project.shots<=project.lastReturnedShot)return;var site=creature.GetComponent<CreatureConstructionLearning>().sites[0];if(!FindBowlReturnPoint(origin,site,out var point))return;
  CancelFetch();bowlReturnProject=project;bowlReturnAttempt=project.attempts;bowlReturnShot=project.shots;bowlReturnPoint=point;fetchPending=true;fetchClaimed=false;fetchUntil=creature.State.time+90;FetchTicket=FetchTicket==int.MaxValue?1:FetchTicket+1;GetComponent<CreatureAffordance>().cooldown=0;ShowFetchRing(point);creature.Journal.Add(creature.State.time,"build-and-bowl return offered",CreatureAction.IdleOrObserve,StableId,"A temporary fetch invitation to the shooting spot; no completion or reward");
 }
 bool ClaimBowlReturn(){if(bowlReturnProject==null)return true;var site=creature.GetComponent<CreatureConstructionLearning>().sites[0];return !creature.Season.paused&&BowlReturnContextValid&&BowlReturnPointClear(bowlReturnPoint,site)&&creature.CanReach(bowlReturnPoint);}
 bool CompleteBowlReturn(){
  var mind=creature.GetComponent<CreatureCuriosity>();var site=creature.GetComponent<CreatureConstructionLearning>().sites[0];var delta=creature.transform.position-bowlReturnPoint;delta.y=0;
  if(!ClaimBowlReturn()||!item.held||mind.Holding!=item||mind.Current!=CreatureCuriosity.Phase.FetchPlace||delta.magnitude>nav.Scale*6||!BowlReturnPointClear(bowlReturnPoint,site))return false;
  var project=bowlReturnProject;int shot=bowlReturnShot;var point=bowlReturnPoint;PlaceOnGround(point);JustFetched=true;project.RecordReturn(shot);creature.Season.WorldPlay.fetchReturns=System.Math.Min(1000000,creature.Season.WorldPlay.fetchReturns+1);creature.Journal.Add(creature.State.time,"build-and-bowl ball returned",CreatureAction.PlayWithToy,StableId,"Physically carried and placed near the player's shooting spot");return true;
 }
}
}
