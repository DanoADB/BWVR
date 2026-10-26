using System;
using System.Linq;
using UnityEngine;
namespace BWVR {
public sealed partial class ResourceState {
 public float farmWoodUsed;
 public ResourceSource EnsureFarm(string id,Vector3 position){var found=Find(id);if(found!=null)return found.expansionPlot?found:null;if(sources.Length>=64||sources.Count(s=>s.expansionPlot)>=4)return null;var s=new ResourceSource{id=id,kind=VillageResource.Wheat,remaining=0,expansionPlot=true,farmPosition=position};Array.Resize(ref sources,sources.Length+1);sources[sources.Length-1]=s;return s;}
 public bool PrepareFarm(string id){var s=Find(id);if(s==null||!s.expansionPlot||s.farmPrepared||wood<2)return false;wood-=2;farmWoodUsed+=2;s.farmPrepared=true;return true;}
 bool FarmsValid(){if(!HybridState.Number(farmWoodUsed,0,8)||sources.Count(s=>s!=null&&s.expansionPlot)>4||farmWoodUsed!=sources.Count(s=>s!=null&&s.expansionPlot&&s.farmPrepared)*2)return false;
  foreach(var s in sources){if(s==null)return false;if(!s.expansionPlot){if(s.farmPrepared||s.farmPosition!=Vector3.zero)return false;continue;}
   if(s.kind!=VillageResource.Wheat||!(s.id=="farm-0"||s.id=="farm-1"||s.id=="farm-2"||s.id=="farm-3")||!HybridState.Number(s.farmPosition.x,-10000,10000)||!HybridState.Number(s.farmPosition.y,-10000,10000)||!HybridState.Number(s.farmPosition.z,-10000,10000)||(!s.farmPrepared&&(s.remaining!=0||s.growing||s.plantings!=0)))return false;
   foreach(var other in sources)if(other!=null&&other!=s&&other.expansionPlot&&(other.farmPosition-s.farmPosition).sqrMagnitude<64)return false;
  }return true;
 }
}
public sealed partial class VillageResources {
 public float ReservedFarmWood=>Gatherers==null?0:Gatherers.Count(a=>a.PreparingFarm)*2;
 public float FreeWood=>Mathf.Max(0,State.wood-ReservedFarmWood-ReservedToolWood);
 public float FutureHarvest=>sources.Where(s=>s.kind==VillageResource.Wheat).Sum(s=>{var r=State.Find(s.id);return r==null?0:r.growing?ResourceState.SourceYield(r):r.remaining;});
 public bool NeedsPlanting=>FutureHarvest<FoodReserveTarget*2&&sources.Count(s=>s.kind==VillageResource.Wheat&&(State.Find(s.id)?.growing==true||s.owner?.Sowing==true))<3;
 bool FarmSite(Vector3 at){
  if(!Navigation.Sample(at,Navigation.villager,.2f,out var foot)||!discoveries.Reachable(StoreApproach,foot,Navigation.villager))return false;
  foreach(var d in new[]{Vector3.forward,Vector3.back,Vector3.left,Vector3.right})if(!Navigation.Sample(at+d*Navigation.Scale*3,Navigation.villager,.2f,out var edge)||Mathf.Abs(edge.y-foot.y)>Navigation.Scale*.6f||!Navigation.SegmentClear(foot,edge,Navigation.villager))return false;
  return true;
 }
 void BuildExpansionPlots(){
  var views=Navigation.GetComponentsInChildren<IslandPlacementView>();var yards=Navigation.GetComponentsInChildren<ConstructionSite>();
  for(int slot=0;slot<4;slot++){string id="farm-"+slot;var saved=State.Find(id);Vector3 at=default;bool found=saved?.expansionPlot==true;if(found)at=Navigation.ToWorld(saved.farmPosition);
   for(int i=0;i<600&&!found;i++){float a=(i+slot*97)*137.5f*Mathf.Deg2Rad;var wanted=Store+new Vector3(Mathf.Cos(a),0,Mathf.Sin(a))*(20+i*.3f)*Navigation.Scale;if(!Navigation.Sample(wanted,Navigation.villager,.2f,out at))continue;at=Navigation.ProjectFoot(at,Navigation.villager);
    if(sources.Any(s=>Vector2.Distance(new Vector2(at.x,at.z),new Vector2(s.at.x,s.at.z))<Navigation.Scale*9)||views.Any(v=>(v.kind=="building"||v.kind=="tree"||v.kind=="rock")&&Vector2.Distance(new Vector2(at.x,at.z),new Vector2(v.transform.position.x,v.transform.position.z))<Navigation.Scale*9)||yards.Any(y=>y.Contains(y.Local(at),8))||!FarmSite(at))continue;found=true;
   }if(!found)continue;var record=State.EnsureFarm(id,Navigation.ToNav(at));if(record==null)continue;
   var node=new GatheringSource{id=id,kind=VillageResource.Wheat,at=at,approach=at,expansion=true};node.visual=WheatPatch(at).transform;node.visual.name="New village farm "+slot;node.sapling=FarmSoil(at,slot);sources.Add(node);
  }
 }
 Transform FarmSoil(Vector3 at,int slot){
  var vertices=new Vector3[25];var triangles=new int[96];int t=0;
  for(int z=0;z<5;z++)for(int x=0;x<5;x++){var point=at+new Vector3(x-2,0,z-2)*Navigation.Scale;var ground=Navigation.ProjectFoot(point,Navigation.villager);vertices[z*5+x]=new Vector3(x-2,(ground.y-at.y)/Navigation.Scale+.03f,z-2);if(x<4&&z<4){int a=z*5+x;triangles[t++]=a;triangles[t++]=a+5;triangles[t++]=a+1;triangles[t++]=a+1;triangles[t++]=a+5;triangles[t++]=a+6;}}
  var mesh=new Mesh{name="Terrain-following farm soil"};mesh.vertices=vertices;mesh.triangles=triangles;mesh.RecalculateNormals();mesh.RecalculateBounds();resourceMeshes.Add(mesh);var go=new GameObject("Prepared farm soil "+slot);go.transform.SetParent(transform,false);go.transform.position=at;go.transform.localScale=Vector3.one*Navigation.Scale;go.AddComponent<MeshFilter>().sharedMesh=mesh;go.AddComponent<MeshRenderer>().sharedMaterial=timber;return go.transform;
 }
 public bool CanPrepareFarm(GatheringSource node)=>node!=null&&node.expansion&&node.owner==null&&State.Find(node.id)?.farmPrepared==false&&FreeWood>=2&&State.wheat-ReservedSeed>=1&&FarmSite(node.at);
}
public sealed partial class VillagerResourceGathering {
 public bool PreparingFarm=>Current==Phase.FarmApproach||Current==Phase.PrepareFarm;
 public bool TryPrepareFarm(GatheringSource source){if(world.Season.paused||Active||Busy||!world.CanPrepareFarm(source))return false;Source=source;Reserved=0;source.owner=this;routine.ResetTask();nav.ResetRoute();clock=elapsed=0;timber.SetActive(true);motion?.Carry(timber.transform);Current=Phase.FarmApproach;motion?.LookAt(source.visual);return true;}
 public bool TryFoodProduction(){
  if(!world.NeedsPlanting||world.FoodEmergency||world.State.wheat-world.ReservedSeed<1)return false;
  var renewed=world.sources.Where(s=>s.expansion&&world.CanSow(s)).OrderBy(s=>(s.at-transform.position).sqrMagnitude).FirstOrDefault();if(renewed!=null)return TrySow(renewed,world.ChooseCrop());
  int wanted=world.FoodPopulation>3?4:2;
  if(world.sources.Count(s=>s.expansion&&world.State.Find(s.id)?.farmPrepared==true)<wanted&&!world.Gatherers.Any(a=>a.PreparingFarm)){var newPlot=world.sources.Where(world.CanPrepareFarm).OrderBy(s=>(s.at-transform.position).sqrMagnitude).FirstOrDefault();if(newPlot!=null)return TryPrepareFarm(newPlot);}
  var oldPlot=world.sources.Where(world.CanSow).OrderBy(s=>(s.at-transform.position).sqrMagnitude).FirstOrDefault();return oldPlot!=null&&TrySow(oldPlot,world.ChooseCrop());
 }
 bool TickFarm(float dt){
  if(Current==Phase.FarmApproach){motion?.Activity(0);if(nav.MoveTo(Source.approach,transform.lossyScale.y*1.6f,dt)){Current=Phase.PrepareFarm;clock=0;motion?.Carry(null);timber.SetActive(false);motion?.Activity(3);}else if(nav.Blocked&&elapsed>5)Cancel();}
  else if(clock>=FarmPreparationSeconds){bool done=world.State.PrepareFarm(Source.id);Cancel();if(done)world.ClearCropFootprints();world.Refresh();if(done)motion?.React();}return Active;
 }
}
}
