using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
namespace BWVR.Editor {
public static class NavigationChecks {
 [MenuItem("BWVR/Verify navigation (Play mode)")]
 public static void Verify(){
  if(!Application.isPlaying)throw new Exception("Enter Play mode first");
  var world=UnityEngine.Object.FindAnyObjectByType<IslandNavigation>();if(world==null||world.villager.id==world.creature.id)throw new Exception("Missing separate navigation profiles");
  int houses=0,trees=0,water=0,steep=0;
  foreach(var view in world.GetComponentsInChildren<IslandPlacementView>()){
   if(view.kind!="building"||houses>=3)continue;var box=view.GetComponent<BoxCollider>();if(box==null)continue;
   var across=box.transform.right*(box.bounds.extents.x+world.Scale*7);if(Detour(world,view.transform.position-across,view.transform.position+across))houses++;
  }
  if(world.Terrain!=null){var terrain=world.Terrain;
   foreach(var p in terrain.placements){if(p.kind!="tree"||p.variant>=13||trees>=3)continue;var at=terrain.PlacementPoint(p);var across=Vector3.right*world.Scale*(p.size*.45f+3);if(Detour(world,at-across,at+across))trees++;}
   var vertices=terrain.mesh.vertices;var indices=terrain.mesh.triangles;
   for(int i=0;i<indices.Length&&(water<10||steep<10);i+=3){
    var a=vertices[indices[i]];var b=vertices[indices[i+1]];var c=vertices[indices[i+2]];var centre=(a+b+c)/3;
    if(water<10&&centre.y<terrain.SeaLevel+.001f&&!world.Sample(centre,world.villager,.2f,out _))water++;
    if(steep<10&&Vector3.Cross(b-a,c-a).normalized.y<Mathf.Cos(45*Mathf.Deg2Rad)&&centre.y>terrain.SeaLevel+.01f&&!world.Sample(centre,world.villager,.15f,out _))steep++;
   }
   if(houses<2||trees<2||water<5||steep<5)throw new Exception($"Insufficient navigation checks: houses={houses}, trunks={trees}, water={water}, slopes={steep}");
  }
  var actor=UnityEngine.Object.FindObjectsByType<IslandNavigator>().First(n=>n.GetComponent<VillagerRoutine>()!=null);var before=actor.transform.position;
  if(!actor.CanPlace(before,out var placed)||world.Terrain!=null&&Mathf.Abs(placed.y-world.Terrain.Project(placed).y)>.00001f)throw new Exception("Clear placement was rejected or floated above ground");
  var home=world.GetComponentsInChildren<IslandPlacementView>().FirstOrDefault(v=>v.kind=="building"&&v.GetComponent<BoxCollider>()!=null);
  if(home!=null&&actor.CanPlace(home.transform.position,out _))throw new Exception("Placement accepted inside building");
  try{actor.ResetRoute();if(actor.MoveTo(world.ToWorld(new Vector3(10000,0,10000)),world.Scale,1)||!actor.Blocked||actor.transform.position!=before)throw new Exception("Unreachable target moved or teleported actor");}
  finally{actor.ResetRoute();}
  Debug.Log($"BWVR_NAV_PASS: building detours={houses}, trunk detours={trees}, water exclusions={water}, steep exclusions={steep}; unreachable target waits without teleporting");
 }
 static bool Detour(IslandNavigation world,Vector3 from,Vector3 to){
  var profile=world.villager;if(!world.Sample(from,profile,3,out from)||!world.Sample(to,profile,3,out to)||world.SegmentClear(from,to,profile))return false;
  var path=new NavMeshPath();var filter=new NavMeshQueryFilter{agentTypeID=profile.id,areaMask=1};if(!NavMesh.CalculatePath(world.ToNav(from),world.ToNav(to),filter,path)||path.status!=NavMeshPathStatus.PathComplete||path.corners.Length<3)return false;
  float length=0;for(int i=1;i<path.corners.Length;i++)length+=Vector3.Distance(path.corners[i-1],path.corners[i]);if(length<=Vector3.Distance(world.ToNav(from),world.ToNav(to))+.05f)return false;
  return true;
 }
}
}
