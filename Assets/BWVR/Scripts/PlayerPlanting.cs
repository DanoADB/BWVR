using System.Linq;
using UnityEngine;
namespace BWVR {
public sealed partial class ResourceState {
 // Atomic preparation + sowing uses the existing finite stocks and crop records.
 public bool CanPlantField(string id){var source=Find(id);if(source==null||source.kind!=VillageResource.Wheat||wheat<WheatSeedCost)return false;
  if(!source.expansionPlot||source.farmPrepared)return DepletedField(id);
  return wood>=2&&!source.growing&&source.remaining<=.001f&&source.plantings==0&&!cargo.Any(c=>c.amount>0&&c.source==id);
 }
 public bool PlantField(string id){if(!CanPlantField(id))return false;var source=Find(id);bool prepare=source.expansionPlot&&!source.farmPrepared;float beforeWood=wood,beforeUsed=farmWoodUsed;
  if(prepare&&!PrepareFarm(id))return false;if(Sow(id))return true;
  if(prepare){source.farmPrepared=false;wood=beforeWood;farmWoodUsed=beforeUsed;}return false;
 }
}
public sealed partial class VillageResources {
 public GatheringSource PlayerPlotAt(Vector3 point){if(!HybridState.Number(point.x,-10000,10000)||!HybridState.Number(point.y,-10000,10000)||!HybridState.Number(point.z,-10000,10000))return null;
  GatheringSource nearest=null;float distance=Navigation.Scale*Navigation.Scale*16;
  foreach(var node in sources){if(node.kind!=VillageResource.Wheat)continue;var delta=node.at-point;delta.y=0;if(delta.sqrMagnitude<=distance){distance=delta.sqrMagnitude;nearest=node;}}return nearest;
 }
 public bool CanPlayerPlant(Vector3 point,out string reason){reason="";if(Season.paused){reason="Paused";return false;}var node=PlayerPlotAt(point);if(node==null){reason="Aim at an empty field outline";return false;}
  if((Navigation.Terrain!=null&&(!Navigation.Terrain.Dry(point)||Mathf.Abs(Navigation.Terrain.Project(point).y-point.y)>Navigation.Scale*2))||(Navigation.Terrain==null&&Mathf.Abs(point.y-node.at.y)>Navigation.Scale*2)){reason="Aim at dry ground";return false;}
  if(node.owner!=null||!Safe(node)){reason="A villager is using this plot";return false;}var record=State.Find(node.id);if(record==null){reason="Plot unavailable";return false;}
  if(record.growing||record.remaining>.001f||(!record.expansionPlot&&!State.DepletedField(node.id))){reason="This field is already occupied";return false;}
  if(State.wheat-ReservedSeed<ResourceState.WheatSeedCost){reason="Need one unreserved seed from stores";return false;}
  if(sources.Count(s=>s.kind==VillageResource.Wheat&&(State.Find(s.id)?.growing==true||s.owner?.Sowing==true||s.owner?.PreparingFarm==true))>=3){reason="Three plots are already being planted or growing";return false;}
  if(record.expansionPlot&&!record.farmPrepared&&!CanPrepareFarm(node)){reason=FreeWood<2?"Need two unreserved timber to prepare this field":"No safe field footprint here";return false;}
  if(!State.CanPlantField(node.id)){reason="Plot or supplies unavailable";return false;}return true;
 }
 public bool TryPlayerPlant(Vector3 point,out string reason){if(!CanPlayerPlant(point,out reason))return false;var node=PlayerPlotAt(point);if(!State.PlantField(node.id)){reason="Plot changed before planting";return false;}
  ClearCropFootprints();Refresh();IslandAudio.Cue(IslandSound.Harvest,node.at,GetEntityId().GetHashCode(),.5f);return true;
 }
}
}
