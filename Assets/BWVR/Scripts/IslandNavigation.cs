using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
namespace BWVR {
// Routing uses original-size coordinates. Millimetre-sized tabletop agents otherwise
// lose clearance and shoreline detail to the navigation rasterizer's precision.
public sealed partial class IslandNavigation:MonoBehaviour {
 public sealed class Profile {public int id;public float radius,height;public NavMeshSurface surface;public NavMeshBuildSettings settings;}
 public Profile villager,creature;public float Scale{get;private set;}public int obstacleCount{get;private set;}
 public int GeometryRevision{get;private set;}
 public IslandTerrainData Terrain{get;private set;}Mesh walkMesh;float baseY;List<NavMeshBuildSource> baseSources;CreatureBridge bridge;
 readonly Dictionary<int,NavMeshBuildSource> trunks=new Dictionary<int,NavMeshBuildSource>();readonly HashSet<int> uprooted=new HashSet<int>();AsyncOperation[] treeUpdates;bool treesDirty;int recoverFrame=-1;
 public bool TreeNavigationPending=>treesDirty||treeUpdates!=null;
 public Vector3 ToNav(Vector3 point)=>(point-Vector3.up*baseY)/Scale;
 public Vector3 ToWorld(Vector3 point)=>point*Scale+Vector3.up*baseY;
 public void Initialize(IslandTerrainData terrain,Vector3 well){
  Terrain=terrain;Scale=terrain!=null?terrain.scale:.05f;baseY=terrain!=null?terrain.SeaLevel:.82f;
  var sources=new List<NavMeshBuildSource>();walkMesh=new Mesh{name="Navigation dry ground"};
  Vector3[] vertices;int[] triangles;
  if(terrain!=null){vertices=terrain.mesh.vertices;var kept=new List<int>();var source=terrain.mesh.triangles;
   for(int i=0;i<source.Length;i+=3){var a=vertices[source[i]];var b=vertices[source[i+1]];var c=vertices[source[i+2]];
    if(terrain.Dry((a+b+c)/3,false)&&a.y>baseY+.001f&&b.y>baseY+.001f&&c.y>baseY+.001f)kept.AddRange(new[]{source[i],source[i+1],source[i+2]});}
   triangles=kept.ToArray();for(int i=0;i<vertices.Length;i++)vertices[i]=ToNav(vertices[i]);
  }else{vertices=new Vector3[65];vertices[0]=Vector3.zero;for(int i=0;i<64;i++){float a=i*Mathf.PI/32;vertices[i+1]=new Vector3(Mathf.Cos(a),0,Mathf.Sin(a))*(.82f/Scale);}triangles=new int[64*3];for(int i=0;i<64;i++){triangles[i*3]=0;triangles[i*3+1]=(i+1)%64+1;triangles[i*3+2]=i+1;}}
  walkMesh.indexFormat=UnityEngine.Rendering.IndexFormat.UInt32;walkMesh.vertices=vertices;walkMesh.triangles=triangles;walkMesh.RecalculateBounds();
  sources.Add(new NavMeshBuildSource{shape=NavMeshBuildSourceShape.Mesh,sourceObject=walkMesh,transform=Matrix4x4.identity,area=0});
  foreach(var view in GetComponentsInChildren<IslandPlacementView>()){
   if(view.kind=="building"){
    if(view.name.Contains("STORAGE_PIT"))Box(sources,view.transform.position+Vector3.up*Scale,new Vector3(12,2,12)*Scale,view.transform.rotation);
    else foreach(var box in view.GetComponentsInChildren<BoxCollider>())Box(sources,box.transform.TransformPoint(box.center),Vector3.Scale(box.size,box.transform.lossyScale),box.transform.rotation);
   }
   else if(view.kind is "rock" or "feature"){var renderers=view.GetComponentsInChildren<Renderer>();if(renderers.Length>0){var bounds=renderers[0].bounds;foreach(var renderer in renderers)bounds.Encapsulate(renderer.bounds);Box(sources,bounds.center,bounds.size,Quaternion.identity);}}
  }
  var cleared=new HashSet<int>();
  foreach(var view in GetComponentsInChildren<IslandPlacementView>(true))if(!view.gameObject.activeInHierarchy)cleared.Add(view.sourceLine);
  // Block trunks, not broad canopies, ferns or low bushes.
  if(terrain!=null&&terrain.HasPlacements)foreach(var p in terrain.placements)if(p.kind=="tree"&&!cleared.Contains(p.line)&&!(p.variant>=13&&p.variant<=15)){
   var at=ToNav(terrain.PlacementPoint(p));float r=.45f*p.size;var trunk=new NavMeshBuildSource{shape=NavMeshBuildSourceShape.ModifierBox,transform=Matrix4x4.TRS(at+Vector3.up*10,Quaternion.identity,Vector3.one),size=new Vector3(r*2,24,r*2),area=1};sources.Add(trunk);trunks[p.line]=trunk;obstacleCount++;
  }
  if(terrain!=null)Box(sources,well+Vector3.up*Scale*3,new Vector3(5,6,4)*Scale,Quaternion.identity);
  else foreach(var box in GetComponentsInChildren<BoxCollider>())if((box.name.StartsWith("Modular village home")||box.name.Contains("Tree")||box.name.Contains("Pine")))Box(sources,box.transform.TransformPoint(box.center),Vector3.Scale(box.size,box.transform.lossyScale),box.transform.rotation);
  var watch=System.Diagnostics.Stopwatch.StartNew();
  baseSources=sources;
  villager=Bake("Villager",.4f*QuestSlice.VillagerScale,(terrain!=null?1.661f:1.7f)*QuestSlice.VillagerScale,.2f,sources);
  creature=Bake("Creature",(terrain!=null?2.2f:1.1f)*QuestSlice.ActorScale,(terrain!=null?13.9236f:6.8f)*QuestSlice.ActorScale,.6f,sources);
  Debug.Log("BWVR_NAV_READY: two profiles, "+obstacleCount+" obstacles; bake "+watch.ElapsedMilliseconds+"ms");
 }
 void Box(List<NavMeshBuildSource> sources,Vector3 position,Vector3 size,Quaternion rotation){
  var navSize=size/Scale;var centre=ToNav(position);centre.y-=navSize.y*.5f+.5f;navSize.y=Mathf.Max(navSize.y+1,30);centre.y+=navSize.y*.5f;
  sources.Add(new NavMeshBuildSource{shape=NavMeshBuildSourceShape.ModifierBox,transform=Matrix4x4.TRS(centre,rotation,Vector3.one),size=navSize,area=1});obstacleCount++;
 }
 Profile Bake(string name,float radius,float height,float voxel,List<NavMeshBuildSource> sources){
  var settings=NavMesh.CreateSettings();settings.agentRadius=radius;settings.agentHeight=height;settings.agentSlope=35;settings.agentClimb=.45f;settings.overrideVoxelSize=true;settings.voxelSize=voxel;settings.overrideTileSize=true;settings.tileSize=128;settings.minRegionArea=2;
  var bounds=walkMesh.bounds;bounds.Expand(new Vector3(10,80,10));
  // Area volumes need explicit body clearance, not just a centre-point exclusion.
  var expanded=new List<NavMeshBuildSource>(sources.Count);foreach(var source in sources){var item=source;if(item.shape==NavMeshBuildSourceShape.ModifierBox)item.size+=new Vector3(radius*2,0,radius*2);expanded.Add(item);}
  var data=NavMeshBuilder.BuildNavMeshData(settings,expanded,bounds,Vector3.zero,Quaternion.identity);if(data==null)throw new InvalidOperationException(name+" navigation bake failed");
  var go=new GameObject(name+" navigation surface (game-unit coordinates)");go.transform.SetParent(transform,false);var surface=go.AddComponent<NavMeshSurface>();surface.agentTypeID=settings.agentTypeID;surface.navMeshData=data;surface.AddData();
  return new Profile{id=settings.agentTypeID,radius=radius,height=height,surface=surface,settings=settings};
 }
 public Vector3 ProjectFoot(Vector3 position,Profile profile){if(profile==villager&&bridge!=null&&bridge.Walkable&&bridge.Project(position,out var raised))return raised;return Terrain!=null?Terrain.Project(position):position;}
 public void BridgeChanged(CreatureBridge crossing){GeometryRevision++;
  CancelTreeUpdates();
  bridge=crossing;var sources=new List<NavMeshBuildSource>();foreach(var source in baseSources){var item=source;if(item.shape==NavMeshBuildSourceShape.ModifierBox)item.size+=new Vector3(villager.radius*2,0,villager.radius*2);sources.Add(item);}
  if(crossing.Walkable)sources.Add(new NavMeshBuildSource{shape=NavMeshBuildSourceShape.Mesh,sourceObject=crossing.NavigationMesh,transform=Matrix4x4.identity,area=0});
  var bounds=walkMesh.bounds;bounds.Expand(new Vector3(10,80,10));var data=NavMeshBuilder.BuildNavMeshData(villager.settings,sources,bounds,Vector3.zero,Quaternion.identity);if(data==null)throw new InvalidOperationException("Bridge navigation bake failed");
  var previous=villager.surface.navMeshData;villager.surface.RemoveData();villager.surface.navMeshData=data;villager.surface.AddData();if(previous!=null)Destroy(previous);
  foreach(var actor in GetComponentsInChildren<IslandNavigator>())if(actor.profile==villager){if(!crossing.Walkable&&crossing.OverWater(actor.transform.position))actor.transform.position=crossing.NearestBank(actor.transform.position);actor.ResetRoute();}
 }
 // Fixed play structures share the existing bounded source/update pipeline.
 public void AddToyBarrier(Vector3 centre,Vector3 size,Quaternion rotation){GeometryRevision++;if(baseSources==null)return;Box(baseSources,centre,size,rotation);treesDirty=true;}
 public void SetTreeRooted(int line,bool rooted){if(!trunks.TryGetValue(line,out var source))return;if(rooted){if(!uprooted.Remove(line))return;baseSources.Add(source);obstacleCount++;}else{if(!uprooted.Add(line))return;baseSources.Remove(source);obstacleCount--;}GeometryRevision++;treesDirty=true;}
 public void InvalidateRoutes(bool recover=false){foreach(var actor in GetComponentsInChildren<IslandNavigator>()){if(recover&&actor.GetComponent<CarryItem>()?.held!=true&&!Sample(actor.transform.position,actor.profile,.2f,out _)&&Sample(actor.transform.position,actor.profile,16,out var safe))actor.transform.position=ProjectFoot(safe,actor.profile);actor.ResetRoute();}}
 public void TreeObstacleChanged(){InvalidateRoutes();recoverFrame=Time.frameCount+3;}
 void CancelTreeUpdates(){if(treeUpdates==null)return;foreach(var p in new[]{villager,creature})if(p?.surface?.navMeshData!=null)NavMeshBuilder.Cancel(p.surface.navMeshData);treeUpdates=null;treesDirty=true;}
 void Update(){
  if(recoverFrame>=0&&Time.frameCount>=recoverFrame){recoverFrame=-1;InvalidateRoutes(true);}
  if(treeUpdates!=null){foreach(var operation in treeUpdates)if(operation!=null&&!operation.isDone)return;treeUpdates=null;InvalidateRoutes(true);}
  if(!treesDirty||baseSources==null)return;treesDirty=false;treeUpdates=new AsyncOperation[2];var profiles=new[]{villager,creature};var bounds=walkMesh.bounds;bounds.Expand(new Vector3(10,80,10));
  for(int i=0;i<profiles.Length;i++){var p=profiles[i];var sources=new List<NavMeshBuildSource>();foreach(var original in baseSources){var source=original;if(source.shape==NavMeshBuildSourceShape.ModifierBox)source.size+=new Vector3(p.radius*2,0,p.radius*2);sources.Add(source);}if(p==villager&&bridge!=null&&bridge.Walkable)sources.Add(new NavMeshBuildSource{shape=NavMeshBuildSourceShape.Mesh,sourceObject=bridge.NavigationMesh,transform=Matrix4x4.identity,area=0});treeUpdates[i]=NavMeshBuilder.UpdateNavMeshDataAsync(p.surface.navMeshData,p.settings,sources,bounds);}
 }
 public bool Sample(Vector3 position,Profile profile,float distance,out Vector3 result){
  var ground=Terrain!=null?ProjectFoot(position,profile):new Vector3(position.x,baseY,position.z);var at=ToNav(ground);
  var filter=new NavMeshQueryFilter{agentTypeID=profile.id,areaMask=1};if(NavMesh.SamplePosition(at,out var hit,distance+1,filter)&&new Vector2(hit.position.x-at.x,hit.position.z-at.z).magnitude<=distance){result=ToWorld(hit.position);return true;}result=position;return false;
 }
 public bool SegmentClear(Vector3 from,Vector3 to,Profile profile){var end=ToNav(to);return !NavMesh.Raycast(ToNav(from),end,out var hit,new NavMeshQueryFilter{agentTypeID=profile.id,areaMask=1})||new Vector2(hit.position.x-end.x,hit.position.z-end.z).magnitude<.05f;}
 void OnDestroy(){CancelTreeUpdates();foreach(var p in new[]{villager,creature})if(p!=null){if(p.surface!=null){p.surface.RemoveData();if(p.surface.navMeshData!=null)Destroy(p.surface.navMeshData);}NavMesh.RemoveSettings(p.id);}if(walkMesh!=null)Destroy(walkMesh);}
}
}
