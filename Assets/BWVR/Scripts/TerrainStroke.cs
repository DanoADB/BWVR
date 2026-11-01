using System;
using UnityEngine;
namespace BWVR {
public sealed partial class TerrainSculptor {
 public event Action Applied;
 public bool Pending=>pendingBake!=null;
 public float LastPublishMs{get;private set;}
 IslandNavigation.SculptBake pendingBake;float[] pendingOffsets;Vector3 pendingPoint;SpiritualTool pendingMode;SliceState pendingSeason;float pendingBegan;TerrainUndo pendingUndo;
 public bool Request(Vector3 at,SpiritualTool mode){if(!CanSculpt(at,mode,out var why)){Reason=why;return false;}Propose(at,mode,out var changes,out _);pendingOffsets=(float[])Terrain.offsets.Clone();foreach(var c in changes)pendingOffsets[c.Key]=c.Value;return PrepareChange(pendingOffsets,at,mode,null);
 }
 bool PrepareChange(float[] offsets,Vector3 at,SpiritualTool mode,TerrainUndo undo){pendingOffsets=offsets;var vertices=(Vector3[])baseVertices.Clone();for(int i=0;i<vertices.Length;i++)vertices[i].y+=offsets[vertexCells[i]]*Terrain.scale;
  try{pendingBake=nav.PrepareSculpt(vertices);pendingPoint=at;pendingMode=mode;pendingUndo=undo;pendingSeason=god.Season;pendingBegan=Time.realtimeSinceStartup;Reason="Preparing safe ground";return true;}catch(Exception e){CancelPending();Reason="Could not prepare ground";Debug.LogError("BWVR_SCULPT_PREPARE_FAILED: "+e.Message);return false;}
 }
 public void CancelPending(){pendingBake?.Dispose();pendingBake=null;pendingOffsets=null;pendingUndo=null;}
 void Update(){
  if(!Pending)return;if(god.Season!=pendingSeason||god.Season.paused||miracles.Suspended||Time.realtimeSinceStartup-pendingBegan>20){CancelPending();Reason="Stroke canceled";return;}if(!pendingBake.Ready)return;
  var task=pendingBake;var offsets=pendingOffsets;var undo=pendingUndo;pendingBake=null;pendingOffsets=null;pendingUndo=null;
  try{if(!nav.SculptCurrent(task)){Reason="Navigation changed; stroke canceled";return;}bool allowed=undo!=null?ReferenceEquals(god.Season.terrainEdits?.Undo,undo)&&CanUndo(out _):CanSculpt(pendingPoint,pendingMode,out _);if(!allowed){Reason="Patch unavailable; terrain change canceled";return;}
   var watch=System.Diagnostics.Stopwatch.StartNew();var before=Terrain.offsets;Terrain.offsets=offsets;
   try{PublishMesh();RefreshPhysics();nav.PublishSculpt(task);}catch{Terrain.offsets=before;PublishMesh();RefreshPhysics();throw;}
   ProposeCount(before,offsets,out int changed);if(undo!=null)RecordUndo(undo);else RecordStroke(pendingPoint,pendingMode,changed,before);LastPublishMs=(float)watch.Elapsed.TotalMilliseconds;
  }catch(Exception e){Reason="Ground update failed; stroke restored";Debug.LogError("BWVR_SCULPT_PUBLISH_FAILED: "+e.Message);}finally{task.Dispose();}
 }
 static void ProposeCount(float[] a,float[] b,out int count){count=0;for(int i=0;i<a.Length;i++)if(Mathf.Abs(a[i]-b[i])>.00001f)count++;}
}
}
