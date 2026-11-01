using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
namespace BWVR.Editor {
public static class TerrainUndoChecks {
 static readonly BindingFlags F=BindingFlags.Instance|BindingFlags.NonPublic;
 static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
 static QuestSlice slice;static SliceState original,test;static TerrainSculptor sculpt;static GodToolPalette palette;static DivineIntervention divine;static bool unlimited,cursor;static Vector3 point,actorAt;static VillagerRoutine actor;static bool actorHeld;static string scenario;static float[] expected,waiting;static float energy,timeScale;static TerrainUndo receipt;
 public static void VerifyModel(){
  Check(new SliceState().Valid(),"Old save rejected");var edits=new TerrainEdits{source="island",width=32,undo=new TerrainUndo{available=true,changes=new[]{new TerrainChange{cell=10,before=1,after=0}}}};Check(edits.Valid()&&JsonUtility.FromJson<TerrainEdits>(JsonUtility.ToJson(edits)).Valid(),"Undo of zeroed patch failed save validation");
  edits.undo.changes[0].after=2;Check(!edits.Valid(),"Stale history accepted");edits.undo.changes[0].after=0;edits.undo.changes[0].before=float.NaN;Check(!edits.Valid(),"Nonfinite history accepted");edits.undo.changes[0].before=5;Check(!edits.Valid(),"Unbounded history accepted");edits.undo.changes[0].before=1;edits.undo.changes=new[]{edits.undo.changes[0],edits.undo.changes[0]};Check(!edits.Valid(),"Duplicate undo cell accepted");edits.undo.changes=Array.Empty<TerrainChange>();Check(!edits.Valid(),"Empty undo accepted");edits.undo=null;Check(edits.Valid(),"Legacy terrain edits require history");Debug.Log("BWVR_TERRAIN_UNDO_MODEL_PASS");
 }
 public static void Begin(string kind="success"){
  Check(slice==null,"Previous undo check unfinished");scenario=kind;timeScale=Time.timeScale;Time.timeScale=0;slice=UnityEngine.Object.FindAnyObjectByType<QuestSlice>();var field=typeof(QuestSlice).GetField("state",F);original=(SliceState)field.GetValue(slice);test=new SliceState{developmentFocus=true,observeWithoutFoodWork=true};field.SetValue(slice,test);sculpt=slice.GetComponentInChildren<TerrainSculptor>();palette=slice.GetComponentInChildren<GodToolPalette>();divine=slice.GetComponentInChildren<DivineIntervention>();unlimited=divine.tuning.unlimitedEnergy;divine.tuning.unlimitedEnergy=false;cursor=palette.UseDesktopCursor;palette.UseDesktopCursor=false;var god=slice.GetComponentInChildren<CreatureHybrid>();god.Interrupt("Terrain undo QA");sculpt.Restore(test);
  try{
   Check(!sculpt.RequestUndo(),"History invented for old save");var d=sculpt.Terrain;var nav=slice.GetComponent<IslandNavigation>();bool found=false;
   for(int z=d.bounds[1]+5;z<d.bounds[3]-5&&!found;z+=3)for(int x=d.bounds[0]+5;x<d.bounds[2]-5&&!found;x+=3){var p=d.Project(d.World(x,z,0));if(nav.Sample(p,nav.villager,1,out _)&&nav.Sample(p,nav.creature,1,out _)&&sculpt.CanSculpt(p,SpiritualTool.RaiseGround,out _)){point=p;found=true;}}
   Check(found,"No safe undo patch");Check(sculpt.CommitImmediateForValidation(point,SpiritualTool.RaiseGround),"First stroke failed");expected=(float[])d.offsets.Clone();test.Hybrid.time+=2;Check(sculpt.CommitImmediateForValidation(d.Project(point),SpiritualTool.LowerGround),"Second stroke failed");Check(test.terrainEdits.Valid()&&test.terrainEdits.Undo!=null&&test.terrainEdits.strokes==2,"No latest-stroke history");Check(!sculpt.RequestUndo(),"Undo bypassed settling cooldown");test.Hybrid.time+=2;
   if(kind=="success"){typeof(QuestSlice).GetMethod("Save",F).Invoke(slice,null);typeof(QuestSlice).GetMethod("Load",F).Invoke(slice,null);test=(SliceState)field.GetValue(slice);Check(test.Valid()&&test.terrainEdits.Undo!=null,"Native reload lost undo");}
   // No second charge, even with a finite empty energy pool.
   test.Hybrid.energy=0;energy=0;waiting=(float[])d.offsets.Clone();receipt=test.terrainEdits.Undo;var savedPoint=receipt.at;receipt.at+=Vector3.right;Check(!sculpt.CanUndo(out _),"Remote-cell history bypassed patch boundary");receipt.at=savedPoint;
   test.paused=true;Check(!sculpt.RequestUndo(),"Paused undo began");test.paused=false;
   var outside=new Ray(Camera.main.transform.position,Camera.main.transform.forward);palette.Cancel();palette.OpenCategory(GodToolCategory.Terraform);palette.Process(outside,false,true,false,.02f,false);Physics.SyncTransforms();var ray=new Ray(Camera.main.transform.position,(palette.UndoTerrainButton.bounds.center-Camera.main.transform.position).normalized);palette.Process(ray,false,true,false,.02f,false);palette.Process(ray,true,true,false,.02f,false);
   Check(sculpt.Pending&&test.terrainEdits.Undo==receipt&&d.offsets.SequenceEqual(waiting),"Undo did not prepare through real dock input, or published early");int uses=palette.Uses;palette.Process(ray,true,true,false,.02f,false);Check(palette.Uses==uses&&test.terrainEdits.Undo==receipt,"Held click repeated undo");
   if(kind=="occupied"){actor=slice.GetComponentInChildren<VillagerRoutine>();actorAt=actor.transform.position;actorHeld=actor.GetComponent<CarryItem>().held;actor.GetComponent<CarryItem>().held=true;actor.transform.position=d.Project(point);}
   else if(kind=="pause")test.paused=true;
   else if(kind=="cancel")palette.Cancel();
   else if(kind=="tracking")palette.Process(ray,true,false,false,.02f,true);
  }catch{Restore();throw;}
 }
 public static string Finish(){Check(slice!=null,"No undo check active");if(sculpt.Pending)return "TERRAIN_UNDO_PENDING";
  try{
   Check(test.Hybrid.energy==energy&&test.terrainEdits.strokes==2,"Undo changed costs or forward stroke count");
   if(scenario=="success"){
    Check(sculpt.Terrain.offsets.SequenceEqual(expected)&&test.terrainEdits.Undo==null,"Undo did not restore exactly the preceding stroke");Check(!sculpt.RequestUndo(),"Undo became a toggle or consumed earlier history");Check(test.terrainEdits.Valid(),"Undo produced invalid save");Check(test.Hybrid.Evidence.events.Count(e=>e.target=="island-sculpt-undo"&&e.source==EvidenceSource.PlayerMiracle)==1,"Undo attribution duplicated or missing");
    var nav=slice.GetComponent<IslandNavigation>();var at=sculpt.Terrain.Project(point);Check(nav.Sample(at,nav.villager,1,out _)&&nav.Sample(at,nav.creature,1,out _),"Undo lost navigation");var collider=slice.GetComponentsInChildren<MeshCollider>().First(c=>c.sharedMesh==sculpt.Terrain.mesh);Check(collider.Raycast(new Ray(at+Vector3.up,Vector3.down),out var hit,2)&&Mathf.Abs(hit.point.y-at.y)<sculpt.Terrain.scale*.01f,"Undo collision mismatches surface");var verts=sculpt.Terrain.mesh.vertices;foreach(var ball in nav.GetComponentsInChildren<BeachBall>())Check(((Mesh)typeof(BeachBall).GetField("ground",F).GetValue(ball)).vertices.SequenceEqual(verts.Select(nav.ToNav)),"Undo missed private ball collision");Check(((Mesh)typeof(TreePlayWorld).GetField("ground",F).GetValue(nav.GetComponentInChildren<TreePlayWorld>())).vertices.SequenceEqual(verts.Select(nav.ToNav)),"Undo missed private tree collision");
    typeof(QuestSlice).GetMethod("Save",F).Invoke(slice,null);typeof(QuestSlice).GetMethod("Load",F).Invoke(slice,null);test=(SliceState)typeof(QuestSlice).GetField("state",F).GetValue(slice);Check(test.Valid()&&test.terrainEdits.Undo==null&&sculpt.Terrain.offsets.SequenceEqual(expected),"Native reload resurrected consumed undo");
   }else Check(test.terrainEdits.Undo==receipt&&sculpt.Terrain.offsets.SequenceEqual(waiting)&&!test.Hybrid.Evidence.events.Any(e=>e.target=="island-sculpt-undo"),"Rejected undo mutated ground/history/evidence");
   Debug.Log("BWVR_TERRAIN_UNDO_PASS: "+scenario);return "TERRAIN_UNDO_PASS: "+scenario;
  }finally{Restore();}
 }
 public static void Restore(){if(slice==null)return;Time.timeScale=timeScale;palette.Cancel();if(actor!=null){actor.transform.position=actorAt;actor.GetComponent<CarryItem>().held=actorHeld;actor=null;}divine.tuning.unlimitedEnergy=unlimited;palette.UseDesktopCursor=cursor;typeof(QuestSlice).GetField("state",F).SetValue(slice,original);sculpt.Restore(original);slice.GetComponentInChildren<CreatureHybrid>().NewSeason();slice=null;}
}
}
