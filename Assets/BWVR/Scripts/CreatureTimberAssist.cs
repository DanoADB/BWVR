using UnityEngine;
namespace BWVR {
public sealed partial class VillagerResourceGathering {
 CreatureHybrid timberCourier;bool timberTaken;
 public bool CreatureTimberTaken=>timberTaken;
 public bool CreatureTimberClaimed=>timberCourier!=null;
 public Transform OfferedTimber=>timber.transform;
 public Vector3 TimberStore=>world.StoreApproach;
 bool CreatureCanNotice(){var god=world.Navigation.GetComponentInChildren<CreatureHybrid>();return god!=null&&god.CanSeeCue(GetComponent<HybridTarget>(),false,true);}
 public bool CreatureTimberReady=>Current==Phase.OfferWood&&clock>=2&&relayHelper==null&&timberCourier==null&&Source?.owner==this&&Source.kind==VillageResource.Wood&&Cargo.amount>0&&!world.Season.paused&&!Busy&&!world.FoodEmergency&&world.Safe(Source)&&world.State.Stock(VillageResource.Wood)<world.State.Limit(VillageResource.Wood);
 public bool CanOfferCreatureTimber(CreatureHybrid god)=>CreatureTimberReady&&god!=null&&god.GetComponent<CarryItem>()?.held!=true&&god.CanSeeCue(GetComponent<HybridTarget>(),false,true)&&TimberApproach(god,out _)&&god.CanReach(TimberStore);
 public bool TimberApproach(CreatureHybrid god,out Vector3 at){
  at=default;var n=world.Navigation;var side=god.transform.position-transform.position;side.y=0;if(side.sqrMagnitude<1e-10f)side=Vector3.back;
  float gap=n.creature.radius+n.villager.radius+1.5f;
  for(int i=0;i<8;i++){var wanted=transform.position+Quaternion.Euler(0,i*45,0)*side.normalized*n.Scale*gap;if(!n.Sample(wanted,n.creature,3,out var foot))continue;foot=n.ProjectFoot(foot,n.creature);if(Vector3.Distance(foot,transform.position)>n.Scale*12||!god.GetComponent<IslandNavigator>().BodyClear(foot,foot)||!god.CanReach(foot))continue;at=foot;return true;}return false;
 }
 public bool ClaimCreatureTimber(CreatureHybrid god){if(!CanOfferCreatureTimber(god))return false;timberCourier=god;timberTaken=false;clock=elapsed=0;motion?.LookAt(god.transform);motion?.Signal(false,true,TimberStore);return true;}
 public bool CreatureTimberValid(CreatureHybrid god)=>timberCourier==god&&Current==Phase.OfferWood&&Source?.owner==this&&Cargo.amount>0&&!world.Season.paused&&!Busy&&world.Safe(Source)&&god.GetComponent<CarryItem>()?.held!=true;
 public bool TakeCreatureTimber(CreatureHybrid god){if(!CreatureTimberValid(god)||timberTaken||Vector3.Distance(god.transform.position,transform.position)>world.Navigation.Scale*12||!god.CanSeeCue(GetComponent<HybridTarget>(),false,true))return false;timberTaken=true;motion?.Carry(null);motion?.Signal(false);return true;}
 public bool StoreCreatureTimber(CreatureHybrid god){
  if(!CreatureTimberValid(god)||!timberTaken||Vector3.Distance(god.transform.position,TimberStore)>world.Navigation.Scale*8)return false;string source=Source.id;float amount=world.State.Deposit(identity);if(amount<=0)return false;
  var prior=world.Season.timberRelay;world.Season.timberRelay=new TimberRelayMemory{count=System.Math.Min(1000000,(prior?.count??0)+1),cutter=identity,carrier=-1,creature=true,source=source,amount=amount,time=world.Season.Hybrid.time};
  world.Season.gatewayProject?.Supply(world.Season.timberRelay,world.Season.paused);world.RecordRelayedToolProduce(identity,source,amount);social.Memory.trust=Mathf.Clamp01(social.Memory.trust+.04f);social.Memory.affinity=Mathf.Clamp01(social.Memory.affinity+.02f);god.State.RememberBond(GetComponent<HybridTarget>().id,true);GetComponent<VillagerAffect>()?.Reassured();god.Journal.Add(world.Season.Hybrid.time,"creature timber stored",CreatureAction.RespondToGuidance,GetComponent<HybridTarget>().id,amount+" wood; villager cut, creature carried");timberCourier=null;timberTaken=false;Cancel();world.RefreshStock();return true;
 }
 public void ReleaseCreatureTimber(CreatureHybrid god){if(timberCourier!=god)return;bool taken=timberTaken;timberCourier=null;timberTaken=false;if(taken)Cancel();else if(Current==Phase.OfferWood){Current=Phase.Carry;clock=0;motion?.Signal(false);motion?.LookAt(null);nav.ResetRoute();}}
 void ClearCreatureTimber(){timberCourier=null;timberTaken=false;}
}
public sealed partial class CreatureCuriosity {
 VillagerResourceGathering timberWorker;CreatureHybrid timberGod;Vector3 timberApproach;int timberStage;
 public bool TryBeginTimberAssist(CreatureAffordance candidate){
  var worker=candidate?.GetComponent<VillagerResourceGathering>();var god=GetComponent<CreatureHybrid>();if(Current!=Phase.Rest||routine.Current!=CreatureRoutine.Task.SeekingWater||worker==null||!worker.TimberApproach(god,out var approach)||!worker.ClaimCreatureTimber(god))return false;
  timberWorker=worker;timberGod=god;timberApproach=approach;target=candidate;owned=null;routine.ResetTask();navigator.ResetRoute();elapsed=clock=0;timberStage=0;goalPlay=echoPlay=fetching=bowlingPlay=false;Current=Phase.TimberAssist;motion?.LookAt(worker.transform);return true;
 }
 bool TickTimberAssist(float dt){
  var worker=timberWorker;if(worker==null||!worker.CreatureTimberValid(timberGod)||elapsed>110){Finish(false,"Timber task unavailable");return false;}Affect=CreatureAffect.Mood.Curious;if(timberStage==1)HumanoidMotion.FaceMovement(transform,worker.transform.position-transform.position);
  if(timberStage==0){if(navigator.MoveTo(timberApproach,routine.speed,dt)){timberStage=1;clock=0;HumanoidMotion.FaceMovement(transform,worker.transform.position-transform.position);motion?.Touch(worker.OfferedTimber);}else if(navigator.Blocked&&elapsed>8){Finish(false,"Target unreachable");return false;}}
  else if(timberStage==1&&clock>=1.2f){if(!worker.TakeCreatureTimber(timberGod)){timberGod.Journal.Add(state.Hybrid.time,"timber handoff failed",CreatureAction.RespondToGuidance,target.GetComponent<HybridTarget>().id,"gap "+Vector3.Distance(transform.position,worker.transform.position)/navigator.world.Scale+"; visible "+timberGod.CanSeeCue(target.GetComponent<HybridTarget>(),false,true));Finish(false,"Timber handoff unavailable");return false;}motion?.Touch(null);motion?.Carry(worker.OfferedTimber);timberStage=2;clock=0;navigator.ResetRoute();}
  else if(timberStage==2){if(navigator.MoveTo(worker.TimberStore,routine.speed*.8f,dt)){timberStage=3;clock=0;motion?.Activity(5);}else if(navigator.Blocked&&clock>10){Finish(false,"Target unreachable");return false;}}
  else if(timberStage==3){motion?.PlaceAt(worker.TimberStore,Quaternion.identity,Mathf.Clamp01(clock));if(clock>=1.2f){motion?.Carry(null);bool success=worker.StoreCreatureTimber(timberGod);if(success){lastSocial=true;lastWater=false;lastFriend=worker.GetComponent<VillagerSocial>();feedbackWindow=12;boredom=0;}Finish(success);return false;}}return true;
 }
 void ReleaseTimberAssist(){if(timberWorker!=null)timberWorker.ReleaseCreatureTimber(timberGod);timberWorker=null;timberGod=null;}
}
}
