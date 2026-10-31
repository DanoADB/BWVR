using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace BWVR {
// One bounded stroke, committed synchronously with both navigation profiles.
// The imported height asset/mesh are never mutated.
public sealed partial class TerrainSculptor:MonoBehaviour {
 public const float Radius=30,Cost=8,ChargeSeconds=.5f,Cooldown=1.5f;
 public IslandTerrainData Terrain{get;private set;}public string Reason{get;private set;}="";
 public Vector3[] Preview{get;private set;}=Array.Empty<Vector3>();
 Mesh runtimeMesh;MeshCollider ground;IslandNavigation nav;CreatureHybrid god;DivineIntervention miracles;SliceState known;
 readonly Collider[] hits=new Collider[128];float nextAt;Vector3[] baseVertices;int[] vertexCells;
 public static TerrainSculptor Build(Transform parent,IslandTerrainData terrain,MeshCollider collider){if(terrain==null||collider==null)return null;var go=new GameObject("Bounded island sculpting");go.transform.SetParent(parent,false);var s=go.AddComponent<TerrainSculptor>();s.Terrain=Instantiate(terrain);s.Terrain.name=terrain.name+" runtime heights";s.Terrain.offsets=new float[terrain.width*terrain.width];s.runtimeMesh=Instantiate(terrain.mesh);s.runtimeMesh.name="Sculpted island runtime mesh";s.Terrain.mesh=s.runtimeMesh;s.baseVertices=terrain.mesh.vertices;s.vertexCells=new int[s.baseVertices.Length];for(int i=0;i<s.baseVertices.Length;i++){var v=s.baseVertices[i];int x=Mathf.RoundToInt((v.x/terrain.scale+terrain.origin.x)/terrain.cellSize),z=Mathf.RoundToInt((v.z/terrain.scale+terrain.origin.y)/terrain.cellSize);s.vertexCells[i]=z*terrain.width+x;}s.ground=collider;s.PublishMesh();return s;}
 public void Configure(IslandNavigation navigation,CreatureHybrid creature,DivineIntervention divine){nav=navigation;god=creature;miracles=divine;}
 public void Restore(SliceState state,bool rebuild=true){CancelPending();var data=state.terrainEdits;if(data!=null&&(!data.Valid()||data.cells.Length>0&&(data.width!=Terrain.width||data.source!=Terrain.sourceHash)))throw new InvalidOperationException("Terrain edits do not match this island");known=state;nextAt=data?.nextStrokeAt??0;Array.Clear(Terrain.offsets,0,Terrain.offsets.Length);if(data!=null&&data.cells.Length>0){if(!data.Valid()||data.width!=Terrain.width||data.source!=Terrain.sourceHash)throw new InvalidOperationException("Terrain edits do not match this island");foreach(var c in data.cells)Terrain.offsets[c.cell]=c.meters;}PublishMesh();if(rebuild&&nav!=null){RefreshPhysics();nav.RebuildTerrain();}}
 void EnsureSeason(){if(known!=god.Season)Restore(god.Season);}
 void PublishMesh(){var v=(Vector3[])baseVertices.Clone();for(int i=0;i<v.Length;i++)v[i].y+=Terrain.offsets[vertexCells[i]]*Terrain.scale;runtimeMesh.vertices=v;runtimeMesh.RecalculateNormals();runtimeMesh.RecalculateBounds();ground.sharedMesh=null;ground.sharedMesh=runtimeMesh;ground.GetComponent<MeshFilter>().sharedMesh=runtimeMesh;}
 bool Clear(Vector3 at,out string why){why="Use clear ground away from people, trees and structures";float reach=(Radius+Terrain.cellSize*1.5f)*Terrain.scale;var centre=new Vector2(at.x,at.z);
  foreach(var p in Terrain.placements){if(p.kind is not ("tree" or "building" or "field" or "well" or "rock" or "feature"))continue;var pos=Terrain.PlacementPoint(p);float extra=p.kind=="building"?30:Mathf.Max(5,p.size*3);if(Vector2.Distance(centre,new Vector2(pos.x,pos.z))<reach+extra*Terrain.scale){why="Keep clear of "+p.kind;return false;}}
  foreach(var actor in nav.GetComponentsInChildren<IslandNavigator>())if(Vector2.Distance(centre,new Vector2(actor.transform.position.x,actor.transform.position.z))<reach+actor.profile.radius*Terrain.scale){why="Keep clear of actors";return false;}
  foreach(var resources in nav.GetComponentsInChildren<VillageResources>())foreach(var node in resources.sources)if(Vector2.Distance(centre,new Vector2(node.at.x,node.at.z))<reach+Terrain.scale*8){why="Keep clear of resource plots";return false;}
  foreach(var tree in nav.GetComponentsInChildren<ThrowableTree>())if(tree.Uprooted&&Vector2.Distance(centre,new Vector2(tree.transform.position.x,tree.transform.position.z))<reach+Terrain.scale*10){why="Keep clear of moved trees";return false;}
  // Construction and ball physics use private scenes, so inspect their authoritative bounds too.
  foreach(var block in nav.GetComponentsInChildren<ConstructionBlock>()){var b=block.PhysicsBounds;var world=block.Site.World(b.center);if(Vector2.Distance(centre,new Vector2(world.x,world.z))<reach+new Vector2(b.extents.x,b.extents.z).magnitude*block.Site.unit){why="Keep clear of construction pieces";return false;}}
  foreach(var ball in nav.GetComponentsInChildren<BeachBall>())if(Vector2.Distance(centre,new Vector2(ball.transform.position.x,ball.transform.position.z))<reach+5*Terrain.scale)return false;
  Physics.SyncTransforms();int n=Physics.OverlapBoxNonAlloc(at+Vector3.up*100*Terrain.scale,new Vector3(reach,150*Terrain.scale,reach),hits,Quaternion.identity,~0,QueryTriggerInteraction.Ignore);if(n==hits.Length){why="Too many nearby objects";return false;}for(int i=0;i<n;i++){var h=hits[i];if(h==ground||!h.transform.IsChildOf(nav.transform)||h.GetComponentInParent<GodToolPalette>()!=null||h.GetComponentInParent<DivineTool>()!=null)continue;why="Keep clear of "+h.name;return false;}return true;
 }
 bool Propose(Vector3 at,SpiritualTool mode,out Dictionary<int,float> changes,out string why){changes=new Dictionary<int,float>();why="Aim at dry, gentle island ground";if(!HybridState.Number(at.x,-100,100)||!HybridState.Number(at.y,-100,100)||!HybridState.Number(at.z,-100,100)||!Terrain.Dry(at)||Mathf.Abs(Terrain.Project(at).y-at.y)>Terrain.scale*2)return false;if(!Clear(at,out why))return false;
  int cx=Mathf.RoundToInt((at.x/Terrain.scale+Terrain.origin.x)/Terrain.cellSize),cz=Mathf.RoundToInt((at.z/Terrain.scale+Terrain.origin.y)/Terrain.cellSize);if(cx<5||cz<5||cx>=Terrain.width-5||cz>=Terrain.width-5){why="Outside the sculptable island";return false;}float average=Terrain.Project(at).y;int ring=Mathf.CeilToInt(Radius/Terrain.cellSize);
  for(int z=cz-ring;z<=cz+ring;z++)for(int x=cx-ring;x<=cx+ring;x++){if(x<1||z<1||x>=Terrain.width-1||z>=Terrain.width-1)continue;int cell=z*Terrain.width+x;var pos=Terrain.World(x,z,Terrain.Altitude(x,z));float d=Vector2.Distance(new Vector2(at.x,at.z),new Vector2(pos.x,pos.z))/Terrain.scale;if(d>=Radius)continue;float weight=1-d*d/(Radius*Radius);weight*=weight;float old=Terrain.offsets[cell];float value=mode==SpiritualTool.SmoothGround?Mathf.Lerp(old,(average-pos.y)/Terrain.scale,weight*.25f):old+(mode==SpiritualTool.RaiseGround?1:-1)*weight;value=Mathf.Clamp(value,-TerrainEdits.MaxMeters,TerrainEdits.MaxMeters);
   // Protect shoreline and slope classification in the whole affected grid neighbourhood.
   if(!Terrain.Dry(pos)||pos.y+value*Terrain.scale<Terrain.SeaLevel+Terrain.scale*3){why="Keep away from shorelines and steep ground";return false;}if(Mathf.Abs(value-old)>.0001f)changes[cell]=value;
  }
  if(changes.Count==0){why="This patch is at its limit";return false;}int count=Terrain.offsets.Count(v=>Mathf.Abs(v)>.00001f);foreach(var c in changes)if(Mathf.Abs(Terrain.offsets[c.Key])<=.00001f)count++;if(count>TerrainEdits.Limit){why="Island sculpting budget reached";return false;}why="";return true;
 }
 public bool CanSculpt(Vector3 at,SpiritualTool mode,out string why){EnsureSeason();Preview=Array.Empty<Vector3>();why="Unknown sculpt tool";if(!IsSculpt(mode))return false;if(god.Season.paused||miracles.Suspended){why="Paused";return false;}if(Pending){why="Preparing safe ground";return false;}if(nav.TreeNavigationPending){why="Navigation is updating";return false;}if(god.Season.Hybrid.time<nextAt){why="Ground is settling";return false;}if(!miracles.CanAfford(Cost)){why="Not enough spiritual energy";return false;}if(!Propose(at,mode,out var changes,out why))return false;Preview=changes.Select(c=>{int x=c.Key%Terrain.width,z=c.Key/Terrain.width;var v=Terrain.World(x,z,Terrain.Altitude(x,z));v.y+=c.Value*Terrain.scale;return v;}).ToArray();return true;}
 #if UNITY_EDITOR
 // Synchronous core used only by bounded Editor regression checks.
 public bool CommitImmediateForValidation(Vector3 at,SpiritualTool mode){if(!CanSculpt(at,mode,out var why)){Reason=why;return false;}Propose(at,mode,out var changes,out _);var before=(float[])Terrain.offsets.Clone();foreach(var c in changes)Terrain.offsets[c.Key]=c.Value;try{PublishMesh();RefreshPhysics();nav.RebuildTerrain();}catch(Exception e){Terrain.offsets=before;PublishMesh();RefreshPhysics();Reason="Terrain update failed; stroke restored";Debug.LogError("BWVR_SCULPT_FAILED: "+e.Message);return false;}
  RecordStroke(at,mode,changes.Count);return true;
 }
 #endif
 void RecordStroke(Vector3 at,SpiritualTool mode,int changed){
  var memory=new TerrainEdits{nextStrokeAt=god.Season.Hybrid.time+Cooldown,source=Terrain.sourceHash,width=Terrain.width,strokes=Mathf.Min(100000,(god.Season.terrainEdits?.strokes??0)+1)};var saved=new List<TerrainDelta>();for(int i=0;i<Terrain.offsets.Length;i++)if(Mathf.Abs(Terrain.offsets[i])>.00001f)saved.Add(new TerrainDelta{cell=i,meters=Terrain.offsets[i]});memory.cells=saved.ToArray();god.Season.terrainEdits=memory;miracles.SpendEnergy(Cost);nextAt=god.Season.Hybrid.time+Cooldown;god.InvalidateLesson("Player sculpting is not creature work");god.Season.Hybrid.Evidence.Add(god.Season.Hybrid.time,EvidenceKind.Miracle,EvidenceSource.PlayerMiracle,CreatureAction.IdleOrObserve,"island-sculpt",mode+"; "+changed+" grid points; cost "+miracles.EnergyCost(Cost));IslandAudio.Cue(IslandSound.Notice,at,GetEntityId().GetHashCode(),.4f);Reason="";Applied?.Invoke();
 }
 void RefreshPhysics(){foreach(var ball in nav.GetComponentsInChildren<BeachBall>())ball.RefreshSculptedGround();foreach(var tree in nav.GetComponentsInChildren<TreePlayWorld>())tree.RefreshSculptedGround();}
 public static bool IsSculpt(SpiritualTool tool)=>tool is SpiritualTool.RaiseGround or SpiritualTool.LowerGround or SpiritualTool.SmoothGround;
 void OnDestroy(){CancelPending();if(runtimeMesh!=null)Destroy(runtimeMesh);if(Terrain!=null)Destroy(Terrain);}
}
}
