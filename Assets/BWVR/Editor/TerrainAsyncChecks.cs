using System;
using System.Linq;
using UnityEngine;
namespace BWVR.Editor {
public static partial class TerrainSculptChecks {
 static QuestSlice asyncSlice;static SliceState asyncOriginal,asyncTest;static TerrainSculptor asyncSculpt;static Vector3 asyncAt,asyncActorPosition;static VillagerRoutine asyncActor;static bool rejectOccupied,asyncHeld;static float asyncHeight;
 public static void BeginAsync(bool occupy=false){Check(asyncSlice==null,"Previous async check unfinished");asyncSlice=UnityEngine.Object.FindAnyObjectByType<QuestSlice>();var field=typeof(QuestSlice).GetField("state",F);asyncOriginal=(SliceState)field.GetValue(asyncSlice);asyncTest=new SliceState{developmentFocus=true,observeWithoutFoodWork=true};field.SetValue(asyncSlice,asyncTest);asyncSculpt=asyncSlice.GetComponentInChildren<TerrainSculptor>();asyncSculpt.Restore(asyncTest);rejectOccupied=occupy;}
 public static void RequestAsync(){Check(asyncSlice!=null,"No staged async check");var d=asyncSculpt.Terrain;bool found=false;
  try{for(int z=d.bounds[1]+5;z<d.bounds[3]-5&&!found;z+=3)for(int x=d.bounds[0]+5;x<d.bounds[2]-5&&!found;x+=3){var p=d.Project(d.World(x,z,0));if(asyncSculpt.CanSculpt(p,SpiritualTool.RaiseGround,out _)){asyncAt=p;found=true;}}Check(found,"No async test patch");asyncHeight=d.Project(asyncAt).y;Check(asyncSculpt.Request(asyncAt,SpiritualTool.RaiseGround)&&asyncSculpt.Pending,"No asynchronous preparation");Check(asyncTest.terrainEdits==null&&d.Project(asyncAt).y==asyncHeight,"Request exposed unbaked terrain");if(rejectOccupied){asyncActor=asyncSlice.GetComponentInChildren<VillagerRoutine>();asyncActorPosition=asyncActor.transform.position;asyncHeld=asyncActor.GetComponent<CarryItem>().held;asyncActor.GetComponent<CarryItem>().held=true;asyncActor.transform.position=asyncAt;}}catch{RestoreAsync();throw;}
 }
 public static string FinishAsync(){Check(asyncSlice!=null,"No async test");if(asyncSculpt.Pending)return "SCULPT_ASYNC_PENDING";try{
   if(rejectOccupied)Check(asyncTest.terrainEdits==null&&asyncSculpt.Terrain.Project(asyncAt).y==asyncHeight,"Actor entered patch but stroke published");
   else{Check(asyncTest.terrainEdits?.strokes==1&&asyncSculpt.Terrain.Project(asyncAt).y>asyncHeight,"Prepared stroke did not publish: "+asyncSculpt.Reason);var nav=asyncSlice.GetComponent<IslandNavigation>();Check(nav.Sample(asyncAt,nav.villager,1,out _)&&nav.Sample(asyncAt,nav.creature,1,out _),"Prepared nav data is unusable");var points=asyncSculpt.Terrain.mesh.vertices;foreach(var ball in nav.GetComponentsInChildren<BeachBall>()){var mesh=(Mesh)typeof(BeachBall).GetField("ground",F).GetValue(ball);Check(mesh.vertices.SequenceEqual(points.Select(nav.ToNav)),"Ball ground was not updated");}var trees=nav.GetComponentInChildren<TreePlayWorld>();var treeMesh=(Mesh)typeof(TreePlayWorld).GetField("ground",F).GetValue(trees);Check(treeMesh.vertices.SequenceEqual(points.Select(nav.ToNav)),"Tree flight ground was not updated");Debug.Log("BWVR_SCULPT_ASYNC_PUBLISH_MS: "+asyncSculpt.LastPublishMs);}
   Debug.Log(rejectOccupied?"BWVR_SCULPT_ASYNC_OCCUPANCY_PASS":"BWVR_SCULPT_ASYNC_PASS");return rejectOccupied?"SCULPT_ASYNC_OCCUPANCY_PASS":"SCULPT_ASYNC_PASS";
  }finally{RestoreAsync();}
 }
 static void RestoreAsync(){if(asyncSlice==null)return;asyncSculpt.CancelPending();if(asyncActor!=null){asyncActor.transform.position=asyncActorPosition;asyncActor.GetComponent<CarryItem>().held=asyncHeld;asyncActor=null;}typeof(QuestSlice).GetField("state",F).SetValue(asyncSlice,asyncOriginal);asyncSculpt.Restore(asyncOriginal);asyncSlice=null;asyncOriginal=null;asyncTest=null;asyncSculpt=null;}
}
}
