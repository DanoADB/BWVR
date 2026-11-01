using System;
using System.Collections.Generic;
using UnityEngine;
namespace BWVR {
public sealed partial class TerrainSculptor {
 public bool CanUndo(out string why){
  EnsureSeason();why="No terrain stroke to undo";var edits=god.Season.terrainEdits;var undo=edits?.Undo;if(undo==null)return false;
  if(god.Season.paused||miracles.Suspended){why="Paused";return false;}
  if(Pending){why="Preparing safe ground";return false;}
  if(nav.TreeNavigationPending){why="Navigation is updating";return false;}
  if(god.Season.Hybrid.time<nextAt){why="Ground is settling";return false;}
  if(!edits.Valid()||edits.width!=Terrain.width||edits.source!=Terrain.sourceHash){why="Terrain history no longer matches this island";return false;}
  // Validate every affected cell, not just the centre, before preparing navigation.
  foreach(var c in undo.changes){int x=c.cell%Terrain.width,z=c.cell/Terrain.width;var point=Terrain.World(x,z,0);if(Vector2.Distance(new Vector2(point.x,point.z),new Vector2(undo.at.x,undo.at.z))>=Radius*Terrain.scale+.00001f||Mathf.Abs(Terrain.offsets[c.cell]-c.after)>.00001f){why="Terrain history no longer matches this patch";return false;}}
  if(!Clear(Terrain.Project(undo.at),out why))return false;why="";return true;
 }
 public bool RequestUndo(){
  if(!CanUndo(out var why)){Reason=why;return false;}
  var undo=god.Season.terrainEdits.Undo;var offsets=(float[])Terrain.offsets.Clone();foreach(var c in undo.changes)offsets[c.cell]=c.before;
  return PrepareChange(offsets,Terrain.Project(undo.at),SpiritualTool.SmoothGround,undo);
 }
 void RecordUndo(TerrainUndo undo){
  var edits=god.Season.terrainEdits;edits.undo=null;var saved=new List<TerrainDelta>();for(int i=0;i<Terrain.offsets.Length;i++)if(Mathf.Abs(Terrain.offsets[i])>.00001f)saved.Add(new TerrainDelta{cell=i,meters=Terrain.offsets[i]});edits.cells=saved.ToArray();edits.nextStrokeAt=nextAt=god.Season.Hybrid.time+Cooldown;
  // Reversal neither refunds the original miracle nor charges a second time.
  god.InvalidateLesson("Player terrain undo is not creature work");god.Season.Hybrid.Evidence.Add(god.Season.Hybrid.time,EvidenceKind.Miracle,EvidenceSource.PlayerMiracle,CreatureAction.IdleOrObserve,"island-sculpt-undo","Reversed latest sculpt; "+undo.changes.Length+" grid points; no refund");IslandAudio.Cue(IslandSound.Notice,undo.at,GetEntityId().GetHashCode(),.4f);Reason="";Applied?.Invoke();
 }
}
}
