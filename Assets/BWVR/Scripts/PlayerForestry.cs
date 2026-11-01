using UnityEngine;
namespace BWVR {
public sealed partial class VillageResources {
 public GatheringSource PlayerTreeAt(Vector3 point){
  if(!HybridState.Number(point.x,-10000,10000)||!HybridState.Number(point.y,-10000,10000)||!HybridState.Number(point.z,-10000,10000))return null;
  GatheringSource nearest=null;float distance=Navigation.Scale*Navigation.Scale*36;
  foreach(var node in sources){if(node.kind!=VillageResource.Wood||node.tree==null)continue;var delta=node.at-point;delta.y=0;if(delta.sqrMagnitude<=distance){distance=delta.sqrMagnitude;nearest=node;}}return nearest;
 }
 public bool CanPlayerRegrow(Vector3 point,out string reason){
  reason="";if(Season.paused){reason="Paused";return false;}var node=PlayerTreeAt(point);if(node==null){reason="Aim at a harvested tree stump or sapling";return false;}
  if(Navigation.Terrain!=null&&(!Navigation.Terrain.Dry(point)||Mathf.Abs(Navigation.Terrain.Project(point).y-point.y)>Navigation.Scale*2)){reason="Aim at dry ground";return false;}
  var record=State.Find(node.id);if(record==null||node.sapling==null){reason="Tree site unavailable";return false;}
  if(node.owner!=null||node.tree.ResourceReserved||node.tree.Affordance.host!=null||!Safe(node)){reason="Tree site is occupied, moved or damaged";return false;}
  if(!record.growing&&!State.DepletedTree(node.id)){reason="Finish collecting this tree's timber first";return false;}
  if(Navigation.TreeNavigationPending){reason="Tree routes are updating";return false;}
  // Existing roots return at the sapling stage. Never introduce that footprint
  // beneath an actor; do not move actors to make a miracle succeed.
  foreach(var actor in Navigation.GetComponentsInChildren<IslandNavigator>()){var delta=actor.transform.position-node.at;delta.y=0;float clearance=(node.tree.Radius+actor.profile.radius+1)*Navigation.Scale;if(delta.sqrMagnitude<clearance*clearance){reason="Let people and the creature clear this tree site";return false;}}
  return true;
 }
 public bool TryPlayerRegrow(Vector3 point,out string reason){
  if(!CanPlayerRegrow(point,out reason))return false;var node=PlayerTreeAt(point);var record=State.Find(node.id);
  if(!record.growing&&!State.Plant(node.id)){reason="Tree site changed before planting";return false;}
  State.Grow(node.id,WorldBrush.RegrowSeconds);Refresh();return true;
 }
}
}
