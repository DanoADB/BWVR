using System;
using System.Collections.Generic;
using UnityEngine;
namespace BWVR {
[Serializable] public sealed class TerrainDelta {public int cell;public float meters;}
[Serializable] public sealed class TerrainChange {public int cell;public float before,after;}
[Serializable] public sealed class TerrainUndo {
 public bool available;public Vector3 at;public TerrainChange[] changes=Array.Empty<TerrainChange>();
 public bool Valid(int width,Dictionary<int,float> current){
  if(!HybridState.Number(at.x,-100,100)||!HybridState.Number(at.y,-100,100)||!HybridState.Number(at.z,-100,100)||changes==null||changes.Length==0||changes.Length>TerrainEdits.Limit)return false;
  int restored=0;foreach(var value in current.Values)if(Mathf.Abs(value)>.00001f)restored++;var seen=new HashSet<int>();foreach(var c in changes){if(c==null||c.cell<0||c.cell>=width*width||!seen.Add(c.cell)||!HybridState.Number(c.before,-TerrainEdits.MaxMeters,TerrainEdits.MaxMeters)||!HybridState.Number(c.after,-TerrainEdits.MaxMeters,TerrainEdits.MaxMeters)||Mathf.Abs(c.before-c.after)<=.00001f)return false;current.TryGetValue(c.cell,out float value);if(Mathf.Abs(value-c.after)>.00001f)return false;if(Mathf.Abs(value)<=.00001f&&Mathf.Abs(c.before)>.00001f)restored++;else if(Mathf.Abs(value)>.00001f&&Mathf.Abs(c.before)<=.00001f)restored--;}return restored<=TerrainEdits.Limit;
 }
}
[Serializable] public sealed class TerrainEdits {
 public const int Limit=2048;public const float MaxMeters=4;
 public float nextStrokeAt;public string source="";public int width,strokes;public TerrainDelta[] cells=Array.Empty<TerrainDelta>();public TerrainUndo undo;public TerrainUndo Undo=>undo?.available==true?undo:null;
 public bool Valid(){if(!HybridState.Number(nextStrokeAt,0,1000000000)||cells==null||cells.Length>Limit||strokes<0||strokes>100000||source==null||source.Length>128)return false;if((cells.Length>0||Undo!=null)&&(width<2||width>1024||source.Length==0))return false;var values=new Dictionary<int,float>();foreach(var c in cells){if(c==null||c.cell<0||c.cell>=width*width||values.ContainsKey(c.cell)||!HybridState.Number(c.meters,-MaxMeters,MaxMeters))return false;values.Add(c.cell,c.meters);}return undo==null||(!undo.available&&(undo.changes==null||undo.changes.Length<=Limit))||undo.Valid(width,values);}
}
}
