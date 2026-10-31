using System;
using UnityEngine;
namespace BWVR {
public sealed partial class TerrainSculptor {
 public event Action Applied;
 public bool Pending=>pendingBake!=null;
 public float LastPublishMs{get;private set;}
 IslandNavigation.SculptBake pendingBake;float[] pendingOffsets;Vector3 pendingPoint;SpiritualTool pendingMode;SliceState pendingSeason;float pendingBegan;
 public bool Request(Vector3 at,SpiritualTool mode){if(!CanSculpt(at,mode,out var why)){Reason=why;return false;}Propose(at,mode,out var changes,out _);pendingOffsets=(float[])Terrain.offsets.Clone();foreach(var c in changes)pendingOffsets[c.Key]=c.Value;var vertices=(Vector3[])baseVertices.Clone();for(int i=0;i<vertices.Length;i++)vertices[i].y+=pendingOffsets[vertexCells[i]]*Terrain.scale;
  try{pendingBake=nav.PrepareSculpt(vertices);pendingPoint=at;pendingMode=mode;pendingSeason=god.Season;pendingBegan=Time.realtimeSinceStartup;Reason="Preparing safe ground";return true;}catch(Exception e){CancelPending();Reason="Could not prepare ground";Debug.LogError("BWVR_SCULPT_PREPARE_FAILED: "+e.Message);return false;}
 }
 public void CancelPending(){pendingBake?.Dispose();pendingBake=null;pendingOffsets=null;}
 void Update(){
  if(!Pending)return;if(god.Season!=pendingSeason||god.Season.paused||miracles.Suspended||Time.realtimeSinceStartup-pendingBegan>20){CancelPending();Reason="Stroke canceled";return;}if(!pendingBake.Ready)return;
  var task=pendingBake;var offsets=pendingOffsets;pendingBake=null;pendingOffsets=null;
  try{if(!nav.SculptCurrent(task)){Reason="Navigation changed; stroke canceled";return;}if(!CanSculpt(pendingPoint,pendingMode,out var why)){Reason=why+"; stroke canceled";return;}
   var watch=System.Diagnostics.Stopwatch.StartNew();var before=Terrain.offsets;Terrain.offsets=offsets;
   try{PublishMesh();RefreshPhysics();nav.PublishSculpt(task);}catch{Terrain.offsets=before;PublishMesh();RefreshPhysics();throw;}
   ProposeCount(before,offsets,out int changed);RecordStroke(pendingPoint,pendingMode,changed);LastPublishMs=(float)watch.Elapsed.TotalMilliseconds;
  }catch(Exception e){Reason="Ground update failed; stroke restored";Debug.LogError("BWVR_SCULPT_PUBLISH_FAILED: "+e.Message);}finally{task.Dispose();}
 }
 static void ProposeCount(float[] a,float[] b,out int count){count=0;for(int i=0;i<a.Length;i++)if(Mathf.Abs(a[i]-b[i])>.00001f)count++;}
}
}
