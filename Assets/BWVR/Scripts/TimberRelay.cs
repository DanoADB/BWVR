using System;
using System.Linq;
using UnityEngine;
namespace BWVR {
// Only completed, deposited loads become history. Runtime invitations own no stock.
[Serializable] public sealed class TimberRelayMemory {
 public bool creature;public int count,cutter,carrier;public string source="";public float amount,time;
 public bool Valid()=>count==0?!creature&&cutter==0&&carrier==0&&string.IsNullOrEmpty(source)&&amount==0&&time==0:count>0&&count<=1000000&&cutter>=0&&cutter<VillagePopulation.Adults&&(creature?carrier==-1:carrier>=0&&carrier<VillagePopulation.Adults&&cutter!=carrier)&&!string.IsNullOrEmpty(source)&&source.Length<=160&&HybridState.Number(amount,.001f,3)&&HybridState.Number(time,0,100000000);
}
public sealed partial class ResourceState {
 public bool TransferTimber(int from,int to){
  if(from<0||to<0||from>=cargo.Length||to>=cargo.Length||from==to)return false;
  var a=cargo[from];var b=cargo[to];var node=Find(a.source);
  if(a.kind!=VillageResource.Wood||!HybridState.Number(a.amount,.001f,3)||b.amount>0||node==null||node.kind!=VillageResource.Wood||!node.felled||node.growing)return false;
  b.source=a.source;b.kind=a.kind;b.amount=a.amount;a.Clear();return true;
 }
}
public sealed partial class VillagerResourceGathering {
 VillagerResourceGathering relayWorker,relayHelper;int relayCutter=-1;float relayLookClock;
 bool TryRelayOpportunity(float dt){relayLookClock-=dt;if(relayLookClock>0)return false;relayLookClock=.25f;return TryTimberRelay();}
 bool RelayFree=>CanWatchTools&&!routine.Worshipping&&!routine.BuildingCathedral&&world.GetComponentInParent<QuestSlice>().GetComponentInChildren<VillageFamilies>()?.Owns(routine)!=true&&social.Memory.injury==0&&!world.State.NeedsStorage;
 bool RelayWilling(VillagerResourceGathering other)=>other!=this&&social.Memory.PeerWilling("villager-"+other.identity,world.Season.Hybrid.time)&&other.social.Memory.PeerWilling("villager-"+identity,world.Season.Hybrid.time);
 bool RelayVisible(VillagerResourceGathering other)=>Vector3.Distance(transform.position,other.transform.position)<=world.Navigation.Scale*18&&world.ProvisionVisible(social,other.transform.position,other.transform);
 bool OfferTimber(){
  if(Source?.kind!=VillageResource.Wood||practiceKind!=1||!ToolMemory.axeOwned||Cargo.amount<=0||world.FoodEmergency||world.State.NeedsStorage)return false;
  Physics.SyncTransforms();if(!world.Gatherers.Any(a=>a.RelayFree&&a.RelayWilling(this)&&a.RelayVisible(this))&&!CreatureCanNotice())return false;
  Current=Phase.OfferWood;clock=0;motion?.LookAt(null);motion?.Signal(true);return true;
 }
 public bool TryTimberRelay(){
  if(world.Season.paused||!RelayFree||!world.Gatherers.Any(a=>a.Current==Phase.OfferWood&&!a.CreatureTimberClaimed&&a.relayHelper==null))return false;Physics.SyncTransforms();
  foreach(var worker in world.Gatherers.Where(a=>a.Current==Phase.OfferWood&&!a.CreatureTimberClaimed&&a.relayHelper==null&&a.Cargo.amount>0&&a.Source?.owner==a&&RelayWilling(a)&&RelayVisible(a)).OrderBy(a=>(a.transform.position-transform.position).sqrMagnitude)){
   if(!world.GetComponentInParent<QuestSlice>().GetComponentInChildren<IslandDiscoveries>().Reachable(transform.position,worker.transform.position,nav.profile))continue;
   relayWorker=worker;worker.relayHelper=this;Source=worker.Source;Reserved=0;routine.BeginHandsFreeActivity();nav.ResetRoute();clock=elapsed=0;Current=Phase.RelayApproach;motion?.LookAt(worker.transform);worker.motion?.LookAt(transform);return true;
  }return false;
 }
 bool TickTimberRelay(float dt){
  if(Current==Phase.OfferWood){if(CreatureTimberClaimed){if(clock>120)Cancel();return Active;}if(clock>=2&&relayHelper==null)motion?.Signal(false,true,world.StoreApproach);if(clock>(relayHelper==null?6:20)){ReleaseTimberRelay();Current=Phase.Carry;clock=0;motion?.Signal(false);nav.ResetRoute();}return Active;}
  var worker=relayWorker;
  if(worker==null||worker.Current!=Phase.OfferWood||worker.relayHelper!=this||worker.actor.held||worker.Busy||worker.Source!=Source||worker.Cargo.amount<=0||elapsed>20||!RelayWilling(worker)){Cancel();return false;}
  if(Current==Phase.RelayApproach){
   var side=transform.position-worker.transform.position;side.y=0;if(side.sqrMagnitude<1e-10f)side=worker.transform.right;var at=worker.transform.position+side.normalized*nav.world.Scale*3;
   if(nav.MoveTo(at,transform.lossyScale.y*1.6f,dt)){Current=Phase.RelayTake;clock=0;Face(worker.transform.position);motion?.Touch(worker.timber.transform);worker.motion?.Signal(false);}
   else if(nav.Blocked&&elapsed>5)Cancel();
  }else if(clock>=1.2f){
   Physics.SyncTransforms();if(Vector3.Distance(transform.position,worker.transform.position)>nav.world.Scale*5||!RelayVisible(worker)||Source.owner!=worker||!world.State.TransferTimber(worker.identity,identity)){Cancel();return false;}
   relayCutter=worker.identity;relayWorker=null;worker.relayHelper=null;Source.owner=this;Reserved=Cargo.amount;worker.Cancel();worker.cooldown=24;motion?.Touch(null);motion?.LookAt(null);timber.SetActive(true);timber.transform.position=transform.position+Vector3.up*nav.world.Scale;motion?.Carry(timber.transform);motion?.Activity(0);Current=Phase.Carry;clock=0;elapsed=0;nav.ResetRoute();
  }return Active;
 }
 void ReleaseTimberRelay(){
  ClearCreatureTimber();bool involved=relayHelper!=null||relayWorker!=null||relayCutter>=0||Current==Phase.OfferWood||Current==Phase.RelayApproach||Current==Phase.RelayTake;var helper=relayHelper;var worker=relayWorker;relayHelper=relayWorker=null;relayCutter=-1;if(involved){motion?.Touch(null);motion?.Signal(false);}
  if(worker!=null&&worker.relayHelper==this){worker.relayHelper=null;worker.motion?.LookAt(null);if(worker.Current==Phase.OfferWood)worker.motion?.Signal(true);}
  if(helper!=null&&helper.relayWorker==this){helper.relayWorker=null;helper.Cancel();}
 }
 void RecordTimberRelay(float amount){
  if(relayCutter<0||relayCutter==identity||world.Season.paused||Current!=Phase.Deposit||Cargo.amount>0||Source?.kind!=VillageResource.Wood||!HybridState.Number(amount,.001f,3))return;
  int cutter=relayCutter;relayCutter=-1;var prior=world.Season.timberRelay;
  world.Season.timberRelay=new TimberRelayMemory{count=Math.Min(1000000,(prior?.count??0)+1),cutter=cutter,carrier=identity,source=Source.id,amount=amount,time=world.Season.Hybrid.time};
  social.Memory.RememberPeer("villager-"+cutter,true,world.Season.Hybrid.time);world.Gatherers[cutter].social.Memory.RememberPeer("villager-"+identity,true,world.Season.Hybrid.time);
  world.RecordRelayedToolProduce(cutter,Source.id,amount);world.Navigation.GetComponentInChildren<CreatureHybrid>().Journal.Add(world.Season.Hybrid.time,"timber relay stored",CreatureAction.IdleOrObserve,"villager-"+identity,amount+" wood; cut by villager-"+cutter);GetComponent<VillagerAffect>()?.Reassured();
 }
}
public sealed partial class VillageResources {
 // Called only by a carrier's confirmed native Deposit receipt above.
 internal void RecordRelayedToolProduce(int cutter,string source,float amount){RecordDeliveredToolStock(cutter,source,amount);}
}
}
