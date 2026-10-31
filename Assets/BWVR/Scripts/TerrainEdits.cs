using System;
using System.Collections.Generic;
using UnityEngine;
namespace BWVR {
[Serializable] public sealed class TerrainDelta {public int cell;public float meters;}
[Serializable] public sealed class TerrainEdits {
 public const int Limit=2048;public const float MaxMeters=4;
 public float nextStrokeAt;public string source="";public int width,strokes;public TerrainDelta[] cells=Array.Empty<TerrainDelta>();
 public bool Valid(){if(!HybridState.Number(nextStrokeAt,0,1000000000)||cells==null||cells.Length>Limit||strokes<0||strokes>100000||source==null||source.Length>128)return false;if(cells.Length>0&&(width<2||width>1024||source.Length==0))return false;var seen=new HashSet<int>();foreach(var c in cells)if(c==null||c.cell<0||c.cell>=width*width||!seen.Add(c.cell)||!HybridState.Number(c.meters,-MaxMeters,MaxMeters))return false;return true;}
}
}
