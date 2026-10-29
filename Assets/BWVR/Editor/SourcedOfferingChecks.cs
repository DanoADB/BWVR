using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace BWVR.Editor {
public static class SourcedOfferingChecks {
 static readonly BindingFlags F=BindingFlags.Instance|BindingFlags.NonPublic;
 static void Require(bool value,string why){if(!value)throw new Exception(why);}
 public static void VerifyArt(){
  foreach(var path in new[]{"SettlementProps/bucket","SettlementProps/barrel-open","FoodProps/apple","FoodProps/bowl-cereal"}){
   var prefab=SourcedItemArt.Prefab(path);var bounds=SourcedItemArt.LocalBounds(prefab);
   Require(Mathf.Abs(bounds.size.y-1)<.002f&&Mathf.Abs(bounds.min.y)<.002f,"Incorrect normalized pivot/height: "+path);
   var meshes=prefab.GetComponentsInChildren<MeshFilter>();Require(meshes.Length>0&&meshes.All(m=>AssetDatabase.GetAssetPath(m.sharedMesh).StartsWith("Assets/ThirdParty/Kenney")),"Item contains unsourced mesh: "+path);
   Require(prefab.GetComponentsInChildren<Collider>(true).Length==0,"Art prefab took ownership of pickup collision");
   foreach(var renderer in prefab.GetComponentsInChildren<Renderer>())foreach(var material in renderer.sharedMaterials)Require(material!=null&&material.shader.name=="BWVR/GameSurface"&&material.enableInstancing,"Unconfigured source material: "+path);
  }
  Debug.Log("BWVR_SOURCED_OFFERING_ART_PASS: four authored CC0 source meshes, base pivots, unit heights, shared instanced materials and collider-free art");
 }
 public static void Verify(){
  Require(Application.isPlaying,"Enter Play mode");var slice=UnityEngine.Object.FindAnyObjectByType<QuestSlice>();var god=slice.GetComponentInChildren<CreatureHybrid>();var nav=slice.GetComponent<IslandNavigation>();Require(!nav.TreeNavigationPending,"Wait for navigation updates");
  slice.SetSanctuaryOpen(false);god.Interrupt("Sourced offerings QA");var field=typeof(QuestSlice).GetField("state",F);var original=(SliceState)field.GetValue(slice);var test=new SliceState{water=30,developmentFocus=true,observeWithoutFoodWork=true};field.SetValue(slice,test);
  var held=(CarryItem[])typeof(QuestSlice).GetField("held",F).GetValue(slice);Require(held.All(i=>i==null),"Release player objects before checking offerings");var offerings=slice.GetComponentsInChildren<SourcedPickup>();Require(offerings.Length==3,"Missing / duplicate sourced offerings");
  var positions=offerings.Select(i=>i.transform.position).ToArray();var rotations=offerings.Select(i=>i.transform.rotation).ToArray();var palette=slice.GetComponentInChildren<GodToolPalette>();palette.Cancel();palette.Process(new Ray(Camera.main.transform.position,Camera.main.transform.forward),false,true,false,.1f,false);
  var cistern=slice.GetComponentsInChildren<WaterVessel>().First(v=>v.name=="Village cistern");
  try{
   foreach(var view in offerings){
    var item=view.GetComponent<CarryItem>();var bounds=SourcedItemArt.LocalBounds(view.gameObject);var collider=view.GetComponent<BoxCollider>();Require(collider!=null&&(collider.center-bounds.center).magnitude<.002f&&(collider.size-bounds.size).magnitude<.002f,"Pickup bounds do not fit sourced model: "+view.name);
    Require(item.CanGrab&&view.GetComponent<MeshFilter>()==null,"Offering still has primitive root art / cannot be picked up");
    Require((bool)typeof(QuestSlice).GetMethod("TryGrab",F).Invoke(slice,new object[]{1,item}),"Native sourced pickup failed: "+view.name);Require(item.held&&!collider.enabled&&held[1]==item,"Pickup did not take native ownership");
    var ball=slice.GetComponentInChildren<BeachBall>();Require(nav.Sample(ball.Bowling.Start,nav.creature,5,out var ground),"No clear drop fixture");ground=nav.ProjectFoot(ground,nav.creature);
    typeof(QuestSlice).GetMethod("Release",F).Invoke(slice,new object[]{1,new Ray(ground+Vector3.up*nav.Scale*30,Vector3.down)});
    Require(!item.held&&collider.enabled&&held[1]==null,"Sourced release retained ownership");var terrain=nav.Terrain;Require(terrain==null||Mathf.Abs(item.transform.position.y-terrain.Project(item.transform.position).y)<nav.Scale*.05f,"Base-pivot item floated after release: "+view.name);
    if(item.kind==Offering.Water){Require(item.GetComponent<WaterVessel>().Fill==1,"Water offering did not show contents");Require((bool)typeof(QuestSlice).GetMethod("TryGrab",F).Invoke(slice,new object[]{1,item}),"Water delivery pickup failed");typeof(QuestSlice).GetMethod("Release",F).Invoke(slice,new object[]{1,new Ray(cistern.transform.position+Vector3.up*nav.Scale*30,Vector3.down)});Require(test.gifts==1&&test.water==38&&item.transform.position==item.home,"Sourced vessel broke native water delivery / source return");}
   }
   Require(test.gifts==1&&test.Resources.wood==0,"Art changed food/wood inventory rules");
   foreach(var worker in slice.GetComponentsInChildren<VillagerRoutine>()){
    var load=(Transform)typeof(VillagerRoutine).GetField("load",F).GetValue(worker);Require(load!=null,"Worker lost carried load");
    var filters=load.GetComponentsInChildren<MeshFilter>(true).Where(m=>m.name!="Visible water");Require(filters.Any()&&filters.All(m=>AssetDatabase.GetAssetPath(m.sharedMesh).StartsWith("Assets/ThirdParty/")),"Worker still carries primitive supplies");
    if(worker.bucket!=null){Require(worker.bucket.Model=="bucket","Worker bucket not sourced");worker.bucket.SetFill(1);var surface=worker.bucket.transform.Find("Visible water");float high=surface.localPosition.y;worker.bucket.SetFill(.25f);Require(surface.gameObject.activeSelf&&surface.localPosition.y<high,"Bucket fill not readable");worker.bucket.SetFill(0);Require(!surface.gameObject.activeSelf,"Empty vessel shows water");}
   }
   Require(cistern.Model=="barrel-open","Cistern body not replaced");test.water=80;slice.GetComponent<DroughtSignals>().Tick(0,test);Require(cistern.Fill==.8f,"Cistern fill stopped following simulation");
   Debug.Log("BWVR_SOURCED_OFFERING_PLAY_PASS: mesh-fit pickup collision, native ownership/release, grounded base pivots, actual water delivery/source return, unchanged food/wood rules, sourced worker loads and visible empty/full vessels");
  }finally{held[0]=held[1]=null;for(int i=0;i<offerings.Length;i++){var item=offerings[i].GetComponent<CarryItem>();item.held=false;item.GetComponent<Collider>().enabled=true;item.transform.SetPositionAndRotation(positions[i],rotations[i]);}foreach(var worker in slice.GetComponentsInChildren<VillagerRoutine>())worker.ResetTask();god.Interrupt("Sourced offerings QA complete");field.SetValue(slice,original);slice.GetComponent<DroughtSignals>().Tick(0,original);god.NewSeason();Physics.SyncTransforms();}
 }
}
}
