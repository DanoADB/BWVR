using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
namespace BWVR {
public sealed partial class IslandNavigation {
 public sealed class SculptBake:IDisposable {
  internal Mesh mesh;internal NavMeshData[] data=new NavMeshData[2];internal AsyncOperation[] jobs=new AsyncOperation[2];internal int revision;
  public bool Ready=>jobs[0]!=null&&jobs[1]!=null&&jobs[0].isDone&&jobs[1].isDone;
  public void Dispose(){foreach(var d in data)if(d!=null){NavMeshBuilder.Cancel(d);UnityEngine.Object.Destroy(d);}if(mesh!=null)UnityEngine.Object.Destroy(mesh);}
 }
 public SculptBake PrepareSculpt(Vector3[] vertices){
  var task=new SculptBake{revision=GeometryRevision,mesh=new Mesh{name="Pending sculpt navigation",indexFormat=UnityEngine.Rendering.IndexFormat.UInt32}};var kept=new List<int>();var triangles=Terrain.mesh.triangles;
  for(int i=0;i<triangles.Length;i+=3){var a=vertices[triangles[i]];var b=vertices[triangles[i+1]];var c=vertices[triangles[i+2]];if(Terrain.Dry((a+b+c)/3,false)&&a.y>baseY+.001f&&b.y>baseY+.001f&&c.y>baseY+.001f)kept.AddRange(new[]{triangles[i],triangles[i+1],triangles[i+2]});}
  for(int i=0;i<vertices.Length;i++)vertices[i]=ToNav(vertices[i]);task.mesh.vertices=vertices;task.mesh.triangles=kept.ToArray();task.mesh.RecalculateBounds();var bounds=task.mesh.bounds;bounds.Expand(new Vector3(10,80,10));var profiles=new[]{villager,creature};
  try{for(int i=0;i<2;i++){var p=profiles[i];var sources=new List<NavMeshBuildSource>();foreach(var original in baseSources){var s=original;if(s.sourceObject==walkMesh)s.sourceObject=task.mesh;if(s.shape==NavMeshBuildSourceShape.ModifierBox)s.size+=new Vector3(p.radius*2,0,p.radius*2);sources.Add(s);}if(p==villager&&bridge!=null&&bridge.Walkable)sources.Add(new NavMeshBuildSource{shape=NavMeshBuildSourceShape.Mesh,sourceObject=bridge.NavigationMesh,transform=Matrix4x4.identity,area=0});task.data[i]=new NavMeshData(p.id);task.jobs[i]=NavMeshBuilder.UpdateNavMeshDataAsync(task.data[i],p.settings,sources,bounds);if(task.jobs[i]==null)throw new InvalidOperationException("Could not prepare sculpt navigation");}return task;}catch{task.Dispose();throw;}
 }
 public bool SculptCurrent(SculptBake task)=>task!=null&&task.revision==GeometryRevision&&!TreeNavigationPending;
 public void PublishSculpt(SculptBake task){if(!task.Ready||!SculptCurrent(task))throw new InvalidOperationException("Sculpt navigation changed while preparing");var watch=System.Diagnostics.Stopwatch.StartNew();var oldMesh=walkMesh;for(int i=0;i<baseSources.Count;i++){var s=baseSources[i];if(s.sourceObject==walkMesh){s.sourceObject=task.mesh;baseSources[i]=s;}}walkMesh=task.mesh;task.mesh=null;var profiles=new[]{villager,creature};for(int i=0;i<2;i++){var p=profiles[i];var old=p.surface.navMeshData;p.surface.RemoveData();p.surface.navMeshData=task.data[i];task.data[i]=null;p.surface.AddData();if(old!=null)Destroy(old);}Destroy(oldMesh);GeometryRevision++;InvalidateRoutes(true);GroundSculptActors();LastTerrainBakeMs=(float)watch.Elapsed.TotalMilliseconds;}
 void GroundSculptActors(){foreach(var actor in GetComponentsInChildren<IslandNavigator>())if(actor.GetComponent<CarryItem>()?.held!=true)actor.transform.position=ProjectFoot(actor.transform.position,actor.profile);}
 public float LastTerrainBakeMs{get;private set;}
 public void RebuildTerrain(){
  if(Terrain==null||baseSources==null)return;
  var watch=System.Diagnostics.Stopwatch.StartNew();var mesh=new Mesh{name="Sculpted navigation ground",indexFormat=UnityEngine.Rendering.IndexFormat.UInt32};var vertices=Terrain.mesh.vertices;var triangles=Terrain.mesh.triangles;var kept=new List<int>();
  for(int i=0;i<triangles.Length;i+=3){var a=vertices[triangles[i]];var b=vertices[triangles[i+1]];var c=vertices[triangles[i+2]];if(Terrain.Dry((a+b+c)/3,false)&&a.y>baseY+.001f&&b.y>baseY+.001f&&c.y>baseY+.001f)kept.AddRange(new[]{triangles[i],triangles[i+1],triangles[i+2]});}
  for(int i=0;i<vertices.Length;i++)vertices[i]=ToNav(vertices[i]);mesh.vertices=vertices;mesh.triangles=kept.ToArray();mesh.RecalculateBounds();var bounds=mesh.bounds;bounds.Expand(new Vector3(10,80,10));var profiles=new[]{villager,creature};var baked=new NavMeshData[2];
  try{for(int i=0;i<2;i++){var p=profiles[i];var sources=new List<NavMeshBuildSource>();foreach(var original in baseSources){var s=original;if(s.sourceObject==walkMesh)s.sourceObject=mesh;if(s.shape==NavMeshBuildSourceShape.ModifierBox)s.size+=new Vector3(p.radius*2,0,p.radius*2);sources.Add(s);}if(p==villager&&bridge!=null&&bridge.Walkable)sources.Add(new NavMeshBuildSource{shape=NavMeshBuildSourceShape.Mesh,sourceObject=bridge.NavigationMesh,transform=Matrix4x4.identity,area=0});baked[i]=NavMeshBuilder.BuildNavMeshData(p.settings,sources,bounds,Vector3.zero,Quaternion.identity);if(baked[i]==null)throw new InvalidOperationException("Sculpt navigation bake failed");}}
  catch{foreach(var data in baked)if(data!=null)Destroy(data);Destroy(mesh);throw;}
  // No PlayerLoop runs between publishing terrain and swapping both profiles.
  CancelTreeUpdates();for(int i=0;i<baseSources.Count;i++){var s=baseSources[i];if(s.sourceObject==walkMesh){s.sourceObject=mesh;baseSources[i]=s;}}
  var oldMesh=walkMesh;walkMesh=mesh;for(int i=0;i<2;i++){var p=profiles[i];var old=p.surface.navMeshData;p.surface.RemoveData();p.surface.navMeshData=baked[i];p.surface.AddData();if(old!=null)Destroy(old);}Destroy(oldMesh);treesDirty=false;GeometryRevision++;InvalidateRoutes(true);GroundSculptActors();LastTerrainBakeMs=(float)watch.Elapsed.TotalMilliseconds;
 }
}
}
