using System;
using System.Linq;
using UnityEngine;
namespace BWVR {
public sealed partial class SocialMemory {
 // Four stable site slots, with four recipe bits per site. Old saves start unknown.
 public int usefulStructures,repairs;
 public float nextRepairAt;
 bool RepairsValid()=>usefulStructures>=0&&usefulStructures<=65535&&Count(repairs)&&HybridState.Number(nextRepairAt,0,100000300);
}
// Maintenance reuses the leisure motor and the same physical support/snap rules as creature play.
public sealed partial class VillagerExploration {
 ConstructionSite repairSite;ConstructionBlock repairBlock;BuildRecipe repairRecipe;
 Vector3 repairPoint;Quaternion repairRotation;bool carryingRepair,buildingGathering;
 int SiteSlot(ConstructionSite site){if(construction==null||site==null)return -1;var clearing=site.GetComponentInParent<DiscoveryPoint>();return site.IsBridge?3:clearing!=null?clearing.id+1:construction.sites[0]==site?0:-1;}
 int StructureBit(ConstructionSite site){int index=SiteSlot(site);return index>=0&&index<4?1<<(index*4+(int)site.Recipe):0;}
 void RememberStructure(ConstructionSite site){social.Memory.usefulStructures|=StructureBit(site);}
 bool ChooseRepair(){if(construction==null)return false;foreach(var site in construction.sites.OrderBy(s=>(s.transform.position-transform.position).sqrMagnitude))if(TryRepair(site))return true;return false;}
 public bool TryRepair(ConstructionSite site){
  if(!CanStartStructure||construction==null||site==null||site.Paused||site.Locked||site.RecipeComplete||site.Snapped==0||site.Repairer!=null||site.Visitor!=null||site.Requester!=null||social.Memory.injury>0||social.RecentFright)return false;
  var god=construction.GetComponent<CreatureHybrid>();
  if(god.Season.paused||god.State.time<social.Memory.nextRepairAt||(social.Memory.usefulStructures&StructureBit(site))==0||site.Clock<site.LastCreatureBuildTime+20||god.GetComponent<CreatureCuriosity>().UsingConstruction(site)||site.blocks.Any(b=>b.Item.held||b.VillagerOwner!=null))return false;
  return StartBlockPlacement(site,false);
 }
 bool StartBlockPlacement(ConstructionSite site,bool building){
  foreach(var block in site.blocks.Where(b=>!b.Attached).OrderBy(b=>(b.transform.position-transform.position).sqrMagnitude)){
   if(!site.TryRecipePlan(block,out var point,out var facing)||!nav.world.Sample(block.transform.position,nav.profile,8,out var fetch)||!nav.world.Sample(site.WorkApproach(transform.position,point,site.unit*3),nav.profile,8,out var place)||!world.Reachable(transform.position,fetch,nav.profile)||!world.Reachable(fetch,place,nav.profile))continue;
   buildingGathering=building;repairSite=site;repairBlock=block;repairRecipe=site.Recipe;repairPoint=point;repairRotation=facing;site.Repairer=this;
   routine.ResetTask();nav.ResetRoute();clock=elapsed=0;Current=Phase.RepairFetch;motion?.Signal(false);motion?.LookAt(block.transform);return true;
  }return false;
 }
 bool TickRepair(float dt){
  clock+=dt;elapsed+=dt;var site=repairSite;var block=repairBlock;
  if(site==null||block==null||site.Repairer!=this||site.Recipe!=repairRecipe||site.RecipeComplete||elapsed>120||social.Memory.injury>0||site.blocks.Any(b=>b!=block&&b.Item.held)||(!carryingRepair&&(block.Item.held||block.Attached))||(carryingRepair&&!block.Item.held)){ResetTask();return false;}
  float speed=transform.lossyScale.y*1.6f;
  if(Current==Phase.RepairFetch){
   var approach=block.transform.position+(transform.position-block.transform.position).normalized*site.unit*1.9f;
   if(nav.MoveTo(approach,speed,dt)){
    if(!site.TryRecipePlan(block,out repairPoint,out repairRotation)){ResetTask();return false;}
    block.Item.held=true;carryingRepair=true;block.Grab();block.GetComponent<Collider>().enabled=false;motion?.Carry(block.transform);motion?.Activity(0);Current=Phase.RepairCarry;clock=0;nav.ResetRoute();
   }
  }else if(Current==Phase.RepairCarry){
   if(!site.TryRecipePlan(block,out repairPoint,out repairRotation)){ResetTask();return false;}
   if(nav.MoveTo(site.WorkApproach(transform.position,repairPoint,site.unit*3),speed,dt)){Current=Phase.RepairPlace;clock=0;motion?.Activity(4);}
  }else if(Current==Phase.RepairPlace){
   motion?.PlaceAt(repairPoint,repairRotation,clock/1.4f);
   if(clock>=1.4f){
    motion?.Carry(null);motion?.PlaceAt(default,Quaternion.identity,0);carryingRepair=false;
    bool placed=site.Drop(block,repairPoint,repairRotation);
    if(placed){if(buildingGathering){social.Memory.builtPieces=Math.Min(1000000,social.Memory.builtPieces+1);if(site.RecipeComplete)social.Memory.gatheringsBuilt=Math.Min(1000000,social.Memory.gatheringsBuilt+1);}else social.Memory.repairs=Math.Min(1000000,social.Memory.repairs+1);var god=construction.GetComponent<CreatureHybrid>();god.Journal.Add(god.State.time,buildingGathering?"villager construction":"villager repair",CreatureAction.ConstructionPlay,GetComponent<HybridTarget>().id,site.Recipe.ToString());}
    ResetTask();if(placed){Current=Phase.Celebrate;motion?.React();}return Active;
   }
  }
  var local=site.Local(transform.position);if(!site.IsBridge&&site.Contains(local)){var feet=transform.position;feet.y=Mathf.Max(feet.y,site.transform.position.y);transform.position=feet;}
  if(nav.Blocked&&elapsed>5)ResetTask();return Active;
 }
 void ReleaseRepair(){
  if(repairSite==null)return;
  motion?.Carry(null);motion?.PlaceAt(default,Quaternion.identity,0);
  if(carryingRepair&&repairBlock!=null&&repairBlock.Item.held)repairSite.Drop(repairBlock,repairBlock.transform.position,repairBlock.transform.rotation);
  if(repairSite.Repairer==this)repairSite.Repairer=null;
  if(construction!=null)social.Memory.nextRepairAt=construction.GetComponent<CreatureHybrid>().State.time+30;
  repairSite=null;repairBlock=null;carryingRepair=buildingGathering=false;
 }
}
}
