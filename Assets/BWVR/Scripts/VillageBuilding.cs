using System.Linq;
namespace BWVR {
public sealed partial class SocialMemory {
 public int gatheringSite=-1,builtPieces,gatheringsBuilt;
 bool VillageBuildingValid()=>gatheringSite>=-1&&gatheringSite<=2&&Count(builtPieces)&&Count(gatheringsBuilt);
}
public sealed partial class VillagerExploration {
 bool ChooseGatheringNeed()=>social.Memory.restNeed>.65f&&ChooseStructure();
 bool GatheringRequested(ConstructionSite site){
  int slot=SiteSlot(site);if(slot<0||slot>2)return false;
  var god=construction.GetComponent<CreatureHybrid>();var memories=god.Season.relationships;
  return memories!=null&&memories.Any(m=>m!=null&&m.gatheringSite==slot&&m.restNeed>.65f&&god.State.time>=m.nextBuildingRequest-80);
 }
 bool ChooseVillageBuilding(){if(construction==null)return false;foreach(var site in construction.sites.OrderBy(s=>(s.transform.position-transform.position).sqrMagnitude))if(TryVillageBuild(site))return true;return false;}
 // The first village crafting capability is an authored three-piece gathering bench.
 // Neighbors take turns with the existing placement motor and site lease.
 public bool TryVillageBuild(ConstructionSite site){
  if(!CanStartStructure||construction==null||site==null||site.IsBridge||site.Paused||site.Locked||site.RecipeComplete||site.Repairer!=null||site.Requester!=null||site.Visitor!=null||social.Memory.injury>0||social.RecentFright||!GatheringRequested(site))return false;
  var god=construction.GetComponent<CreatureHybrid>();
  if(god.Season.paused||(!god.Season.developmentFocus&&!god.Season.Finished&&(god.Season.water<45||god.Season.crops<45))||god.State.time<social.Memory.nextRepairAt||site.Clock<site.LastCreatureBuildTime+20||god.GetComponent<CreatureCuriosity>().UsingConstruction(site)||site.blocks.Any(b=>b.Item.held||b.VillagerOwner!=null))return false;
  if(site.Recipe!=BuildRecipe.Bench){if(site.Snapped>0||!site.SelectRecipe(BuildRecipe.Bench))return false;}
  return StartBlockPlacement(site,true);
 }
}
}
