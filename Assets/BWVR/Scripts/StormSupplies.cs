using UnityEngine;
namespace BWVR {
// A chapter-owned physical opportunity. Utility selection and the native carry motor
// still decide and execute the response; this component never moves the creature.
public sealed class StormSupply:MonoBehaviour {public StormChapter chapter;}
public sealed partial class StormChapter {
 public const string SupplyId="chapter-storm-timber";
 public CarryItem SupplyItem{get;private set;}
 public bool SupplyClaimed{get;private set;}
 public StormSupplyState Supply=>State.supply;
 VillageResources supplyStore;Transform supplyRing;Material supplyMaterial;bool restoreSupply;float supplySearch;
 public Vector3 SupplyDestination=>nav.ToWorld(Supply.destination);
 public bool SupplyPending=>State.Active&&Supply?.spawned==true&&!Supply.delivered;
 public bool SupplyFeasible=>SupplyPending&&!season().paused&&!SupplyClaimed&&!SupplyItem.held&&
  supplyStore.State.Limit(VillageResource.Wood)-supplyStore.State.wood>=StormSupplyState.Timber&&god.CanReach(SupplyDestination);
 public bool CanOfferSupply=>SupplyFeasible&&SupplyItem.GetComponent<CreatureAffordance>().Now>=SupplyItem.GetComponent<CreatureAffordance>().cooldown&&god.CanSeeCue(SupplyItem.GetComponent<HybridTarget>(),false,true);
 void BindSupplies(){
  if(SupplyItem==null){
   supplyStore=nav.GetComponentInChildren<VillageResources>();
   var root=new GameObject("Stranded storm timber — return to stores");root.transform.SetParent(refugeView.parent,false);root.transform.position=supplyStore.Store;
   var model=supplyStore.CreateLoad(VillageResource.Wood);model.transform.SetParent(root.transform,true);model.transform.localPosition=Vector3.zero;model.transform.localScale*=3;
   var box=root.AddComponent<BoxCollider>();box.center=Vector3.up*nav.Scale*1.2f;box.size=new Vector3(4,2.4f,3)*nav.Scale;
   SupplyItem=root.AddComponent<CarryItem>();SupplyItem.kind=Offering.Wood;
   root.AddComponent<StormSupply>().chapter=this;
   var affordance=root.AddComponent<CreatureAffordance>();affordance.kind=CreatureAffordance.Kind.Turn;
   god.RegisterObject(affordance,SupplyId);
   supplyRing=new GameObject("Timber delivery — amber store ring").transform;supplyRing.SetParent(refugeView.parent,false);
   var line=supplyRing.gameObject.AddComponent<LineRenderer>();line.useWorldSpace=false;line.loop=true;line.positionCount=40;line.widthMultiplier=nav.Scale*.35f;supplyMaterial=new Material(refugeMaterial){color=new Color(1,.73f,.2f)};line.sharedMaterial=supplyMaterial;
   line.startColor=line.endColor=new Color(1,.73f,.2f);for(int i=0;i<40;i++){float a=i*Mathf.PI/20;line.SetPosition(i,new Vector3(Mathf.Cos(a)*5,.2f,Mathf.Sin(a)*5)*nav.Scale);}
  }
  SupplyClaimed=false;restoreSupply=SupplyPending;supplySearch=0;
  SupplyItem.held=false;SupplyItem.GetComponent<Collider>().enabled=true;SupplyVisibility();
 }
 void SupplyVisibility(){if(SupplyItem==null)return;SupplyItem.gameObject.SetActive(SupplyPending||SupplyItem.held);supplyRing.gameObject.SetActive(SupplyPending);if(Supply?.spawned==true)supplyRing.position=SupplyDestination;}
 void AdvanceSupplies(float dt){
  if(Supply==null&&(State.phase==StormPhase.Warning||State.phase==StormPhase.Storm))State.supply=new StormSupplyState();
  if(Supply!=null&&!Supply.spawned&&(State.phase==StormPhase.Warning||State.phase==StormPhase.Storm)){supplySearch-=dt;if(supplySearch<=0){supplySearch=2;TryStrandSupply();}}
  if(restoreSupply){if(nav.TreeNavigationPending)return;restoreSupply=false;
   if(nav.Sample(nav.ToWorld(Supply.grounded),nav.creature,3,out var at))SupplyItem.transform.position=nav.ProjectFoot(at,nav.creature);
   else SupplyItem.transform.position=SupplyDestination;
   SupplyItem.home=SupplyItem.transform.position;
  }
  if(SupplyPending&&Supply.playerArrivalPending){Supply.playerArrivalPending=false;ConfirmSupply(RescueSource.Player);}
  CaptureSupplies();SupplyVisibility();
 }
 bool TryStrandSupply(){
  if(nav.TreeNavigationPending||!nav.Sample(supplyStore.StoreApproach,nav.creature,5,out var dest))return false;
  dest=nav.ProjectFoot(dest,nav.creature);if(!god.CanReach(dest))return false;
  for(int k=0;k<32;k++){
   var wanted=dest+Quaternion.Euler(0,k*137.5f,0)*Vector3.forward*nav.Scale*(24+k*.4f);
   if(!nav.Sample(wanted,nav.creature,3,out var at)||!discoveries.Reachable(dest,at,nav.creature))continue;
   at=nav.ProjectFoot(at,nav.creature);Supply.spawned=true;Supply.grounded=nav.ToNav(at);Supply.destination=nav.ToNav(dest);
   SupplyItem.transform.position=SupplyItem.home=at;SupplyItem.GetComponent<CreatureAffordance>().cooldown=0;SupplyVisibility();
   IslandAudio.Cue(IslandSound.Notice,at,86512,.6f);god.Journal.Add(god.State.time,"storm timber stranded",CreatureAction.RespondToGuidance,SupplyId,"Recover six timber at the covered stores");return true;
  }return false;
 }
 public bool ClaimSupply(){if(!CanOfferSupply)return false;SupplyClaimed=true;return true;}
 public void ReleaseSupply(){SupplyClaimed=false;}
 public bool SupplyAtStore=>SupplyPending&&!SupplyItem.held&&Vector3.Distance(SupplyItem.transform.position,SupplyDestination)<=nav.Scale*6;
 public bool ConfirmSupply(RescueSource by){
  if(!SupplyAtStore||season().paused||(by!=RescueSource.Player&&by!=RescueSource.Creature)||by==RescueSource.Creature&&!SupplyClaimed||supplyStore.State.Limit(VillageResource.Wood)-supplyStore.State.wood<StormSupplyState.Timber)return false;
  Supply.delivered=true;Supply.source=by;Supply.playerArrivalPending=false;Supply.grounded=nav.ToNav(SupplyItem.transform.position);
  supplyStore.State.wood+=StormSupplyState.Timber;supplyStore.State.deliveries=System.Math.Min(1000000,supplyStore.State.deliveries+1);supplyStore.RefreshStock();
  if(by==RescueSource.Player&&god.State.guidance.status==GuidanceStatus.Active&&god.State.guidance.target==SupplyId)god.CancelGuidance("Timber already delivered by player");
  foreach(var job in jobs)if(Vector3.Distance(job.transform.position,SupplyDestination)<nav.Scale*45){job.GetComponent<HumanoidMotion>()?.React();job.GetComponent<VillagerAffect>()?.Reassured();}
  god.Journal.Add(god.State.time,"storm timber delivered",CreatureAction.RespondToGuidance,SupplyId,by+": six wood sheltered in the existing stores");IslandAudio.Cue(IslandSound.Discovery,SupplyDestination,86512,.7f);SupplyVisibility();return true;
 }
 void SupplyPicked(CarryItem item){if(item!=SupplyItem||!SupplyPending)return;Supply.playerArrivalPending=false;CaptureSupplies();}
 void SupplyPlaced(CarryItem item){if(item!=SupplyItem)return;CaptureSupplies();if(SupplyAtStore&&season().paused)Supply.playerArrivalPending=true;else ConfirmSupply(RescueSource.Player);SupplyVisibility();}
 void CaptureSupplies(){if(SupplyPending&&!restoreSupply&&!SupplyItem.held)Supply.grounded=nav.ToNav(SupplyItem.transform.position);}
 void DisposeSupplies(){if(supplyMaterial!=null)UnityEngine.Object.Destroy(supplyMaterial);if(SupplyItem!=null)UnityEngine.Object.Destroy(SupplyItem.gameObject);if(supplyRing!=null)UnityEngine.Object.Destroy(supplyRing.gameObject);}
}

public sealed partial class CreatureCuriosity {
 StormChapter supplyChapter;int supplyStage;
 public bool TryBeginStormSupply(CreatureAffordance candidate){
  var chapter=candidate?.GetComponent<StormSupply>()?.chapter;
  if(Current!=Phase.Rest||routine.Current!=CreatureRoutine.Task.SeekingWater||chapter==null||!chapter.ClaimSupply())return false;
  supplyChapter=chapter;routine.ResetTask();navigator.ResetRoute();target=candidate;owned=null;original=candidate.transform.position;originalRotation=candidate.transform.rotation;
  goalPlay=echoPlay=fetching=bowlingPlay=false;elapsed=clock=0;supplyStage=0;Current=Phase.StormSupply;motion?.LookAt(candidate.transform);return true;
 }
 bool TickStormSupply(float dt){
  if(supplyChapter==null||!supplyChapter.SupplyPending||!supplyChapter.SupplyClaimed||elapsed>140||target==null||owned==null&&target.Item.held){Finish(false);return false;}
  Affect=CreatureAffect.Mood.Curious;
  if(supplyStage==0){if(navigator.MoveTo(target.transform.position,routine.speed,dt)){owned=target.Item;owned.held=true;owned.GetComponent<Collider>().enabled=false;motion?.Carry(owned.transform);supplyStage=1;clock=0;}else if(navigator.Blocked&&elapsed>8){Finish(false);return false;}}
  else if(supplyStage==1){if(clock>.8f){supplyStage=2;clock=0;}}
  else if(supplyStage==2){Affect=CreatureAffect.Mood.Happy;if(navigator.MoveTo(supplyChapter.SupplyDestination,routine.speed*.8f,dt)){landing=supplyChapter.SupplyDestination;supplyStage=3;clock=0;motion?.Activity(5);}else if(navigator.Blocked&&clock>10){Finish(false);return false;}}
  else {motion?.PlaceAt(landing,originalRotation,Mathf.Clamp01(clock));if(clock>1.1f){motion?.Carry(null);owned.transform.SetPositionAndRotation(landing,originalRotation);owned.held=false;owned.GetComponent<Collider>().enabled=true;owned.home=landing;owned=null;bool success=supplyChapter.ConfirmSupply(RescueSource.Creature);if(success){lastWater=lastSocial=false;feedbackWindow=12;boredom=0;}Finish(success);return false;}}
  return true;
 }
}
}
