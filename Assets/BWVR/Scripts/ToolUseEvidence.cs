using System;
using System.Linq;
using UnityEngine;
namespace BWVR {
[Serializable] public sealed class ToolUseProof {
 public bool used,stored;public string source="";public int newLearners;public float at,amount;
 public bool Valid()=>(source==null||source.Length<=160)&&(!used||!string.IsNullOrEmpty(source))&&(!stored||used&&amount>0)&&(!stored?amount==0:true)&&newLearners>=0&&newLearners<VillagePopulation.Adults&&(used||newLearners==0)&&HybridState.Number(at,0,100000000)&&HybridState.Number(amount,0,3);
}
public sealed partial class ToolLearner {
 public int axeUses,hoeUses;
 bool UsesValid()=>axeUses>=0&&axeUses<=1000000&&hoeUses>=0&&hoeUses<=1000000&&(axeUses==0||axeOwned)&&(hoeUses==0||hoeOwned);
}
public sealed partial class VillagerResourceGathering {
 bool toolUseEvidence;
 bool TryRecentToolUse(){
  var d=world.Delivery;if(d?.status!=ToolDeliveryStatus.Delivered||d.recipient!=identity||!d.showUse||d.proof?.used==true||world.Season.Hybrid.time>d.expiresAt+120||world.FoodEmergency||world.State.NeedsStorage||!ToolMemory.Owns(d.tool)||d.tool==VillageTool.CultivatingHoe&&world.Season.observeWithoutFoodWork)return false;
  var kind=d.tool==VillageTool.BoundAxe?VillageResource.Wood:VillageResource.Wheat;if(world.GatheringNeed(kind)<=0)return false;
  foreach(var source in world.sources.Where(s=>s.kind==kind&&world.Available(s)&&world.Safe(s)&&(kind!=VillageResource.Wood||!world.State.Find(s.id).felled)).OrderBy(s=>(s.approach-transform.position).sqrMagnitude))if(TryBegin(source))return true;return false;
 }
 public bool RecordToolHarvest(VillageTool tool,int witnesses){
  if(!Enum.IsDefined(typeof(VillageTool),tool)||world.Season.paused||Current!=Phase.Harvest||Source==null||Cargo.amount<=0||!ToolMemory.Owns(tool)||toolUseEvidence)return false;toolUseEvidence=true;
  if(tool==VillageTool.BoundAxe)ToolMemory.axeUses=Math.Min(1000000,ToolMemory.axeUses+1);else ToolMemory.hoeUses=Math.Min(1000000,ToolMemory.hoeUses+1);
  var d=world.Delivery;if(d?.status==ToolDeliveryStatus.Delivered&&d.recipient==identity&&d.tool==tool&&d.proof?.stored!=true){var p=d.proof??(d.proof=new ToolUseProof());p.used=true;p.source=Source.id;p.at=world.Season.Hybrid.time;p.newLearners=Math.Min(VillagePopulation.Adults-1,p.newLearners+Mathf.Clamp(witnesses,0,VillagePopulation.Adults-1));world.Navigation.GetComponentInChildren<CreatureHybrid>().Journal.Add(world.Season.Hybrid.time,"delivered tool used",CreatureAction.IdleOrObserve,"villager-"+identity,tool+"; new learners "+p.newLearners);}
  return true;
 }
}
public sealed partial class VillageResources {
 public bool RecordToolProduce(int actor,string source,float amount){var d=Delivery;var p=d?.proof;if(Season.paused||d?.status!=ToolDeliveryStatus.Delivered||actor!=d.recipient||p?.used!=true||p.stored||p.source!=source||Season.Hybrid.time<p.at||Season.Hybrid.time-p.at>180||!HybridState.Number(amount,.001f,3)||Gatherers[actor].Current!=VillagerResourceGathering.Phase.Deposit||State.cargo[actor].amount>0)return false;p.stored=true;p.amount=amount;Navigation.GetComponentInChildren<CreatureHybrid>().Journal.Add(Season.Hybrid.time,"tool produce stored",CreatureAction.IdleOrObserve,"villager-"+actor,amount+" from "+source+" reached existing stores");return true;}
}
}
