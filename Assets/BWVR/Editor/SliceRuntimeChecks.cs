using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace BWVR.Editor {
public static class SliceRuntimeChecks {
 [MenuItem("BWVR/Verify actor actions and terrain (Play mode)")]
 public static void Verify(){
  if(!EditorApplication.isPlaying)throw new Exception("Enter Play mode before checking actors");
  var slice=UnityEngine.Object.FindAnyObjectByType<QuestSlice>();var stateField=typeof(QuestSlice).GetField("state",BindingFlags.Instance|BindingFlags.NonPublic);var original=(SliceState)stateField.GetValue(slice);var test=new SliceState{paused=true};stateField.SetValue(slice,test);
  var god=slice.GetComponentInChildren<CreatureRoutine>();var workers=slice.GetComponentsInChildren<VillagerRoutine>();var water=slice.GetComponentsInChildren<CarryItem>().First(i=>i.kind==Offering.Water);
  try{
   god.ResetTask();int before=god.deliveries;var seen=new HashSet<CreatureRoutine.Task>();bool handsChecked=false,drinkChecked=false;
   for(int n=0;n<10000&&god.deliveries==before;n++){
    seen.Add(god.Current);god.Tick(.1f,false);
    if(god.Current==CreatureRoutine.Task.Carry&&!handsChecked){CheckHands(slice,false);handsChecked=true;}
    if(god.Current==CreatureRoutine.Task.Drink&&!drinkChecked){CheckHands(slice,true);drinkChecked=true;}
   }
   if(seen.Count!=5||test.gifts!=1||god.deliveries!=before+1||!handsChecked||!drinkChecked)throw new Exception("Creature did not complete exactly one pickup/drink/carry/delivery cycle");
   if(water.held||!water.GetComponent<Collider>().enabled)throw new Exception("Delivered water was not released");
   var position=god.transform.position;var phase=god.Current;god.Tick(20,true);if(god.transform.position!=position||god.Current!=phase)throw new Exception("Paused task advanced");
   foreach(var worker in workers){before=worker.completedJobs;for(int n=0;n<10000&&worker.completedJobs==before;n++)worker.Tick(.1f,false);if(worker.completedJobs!=before+1)throw new Exception("Worker failed to deliver: "+worker.role);}
   var terrain=Resources.Load<IslandTerrainData>("FirstIsland");int samples=0;float maxError=0;
   if(terrain!=null){
    foreach(var worker in workers)if(Mathf.Abs(worker.transform.lossyScale.y/terrain.scale-1.661f*QuestSlice.VillagerScale)>.01f)throw new Exception("Villager/game-unit scale mismatch");
    var visual=god.GetComponentInChildren<HumanoidMotion>();if(visual!=null&&Mathf.Abs(visual.transform.lossyScale.y/terrain.scale-13.9236f*QuestSlice.ActorScale)>.01f)throw new Exception("Creature/game-unit scale mismatch");
    foreach(var home in slice.GetComponentsInChildren<Transform>().Where(t=>t.name.StartsWith("Modular village home")))if(Mathf.Abs(home.lossyScale.y*.195f/terrain.scale-3.66f*QuestSlice.BuildingScale)>.01f)throw new Exception("House/game-unit scale mismatch");
    if(terrain.HasPlacements){
     var views=slice.GetComponentsInChildren<IslandPlacementView>(true);var expected=terrain.placements.Where(p=>p.kind is not ("drink" or "villager")).ToArray();
     if(views.Length!=expected.Length)throw new Exception("Original placement count mismatch");
     var catalog=Resources.Load<EnvironmentArt>("VillageBuildings");
     if(catalog!=null){
      var layout=new VillageLayout(terrain,catalog);var homes=views.Where(v=>v.redesigned).ToArray();
      if(homes.Length!=34||homes.Select(h=>h.modelKey).Distinct().Count()!=5)throw new Exception("Missing full medieval building variants");
      foreach(var home in homes){
       var plot=layout.plots[home.sourceLine];
       if(Vector3.Distance(home.transform.localPosition,plot.position)>.00001f||Mathf.Abs(home.transform.localScale.x-plot.scale)>.00001f||!terrain.Dry(plot.position))throw new Exception("Invalid redesigned plot or house scale");
       if(home.GetComponent<BoxCollider>()==null)throw new Exception("Missing building navigation footprint");
       foreach(var other in layout.plots)if(other.Key!=home.sourceLine&&Vector2.Distance(new Vector2(plot.position.x,plot.position.z),new Vector2(other.Value.position.x,other.Value.position.z))<plot.radius+other.Value.radius+.017f)throw new Exception("Overlapping medieval village plots");
      }
     }
     foreach(var p in expected){var view=views.Single(v=>v.sourceLine==p.line);if(!view.redesigned&&(Vector3.Distance(view.transform.position,terrain.PlacementPoint(p))>.00001f||Mathf.Abs(Mathf.DeltaAngle(view.transform.eulerAngles.y,p.yaw))>.01f&&p.pitch==0&&p.roll==0))throw new Exception("Original placement drift at line "+p.line);}
    }
    var collider=GameObject.Find("Land 1 - original island geometry").GetComponent<MeshCollider>();for(int i=0;i<100;i++){
    var point=terrain.World(Mathf.Lerp(terrain.bounds[0],terrain.bounds[2],Mathf.Repeat(i*.618f,1)),Mathf.Lerp(terrain.bounds[1],terrain.bounds[3],Mathf.Repeat(i*.414f,1)),0);if(!terrain.Dry(point,false))continue;
    if(!collider.Raycast(new Ray(new Vector3(point.x,3,point.z),Vector3.down),out var hit,4))throw new Exception("Missing terrain collider");maxError=Mathf.Max(maxError,Mathf.Abs(terrain.Project(point).y-hit.point.y));samples++;
   }if(samples<10||maxError>.001f)throw new Exception("Terrain collision and sample height disagree");}
   Debug.Log("BWVR_ACTION_TERRAIN_PASS: five creature phases; visible carry and drink hand poses; one delivery; pause; three worker jobs; terrain samples="+samples+", max error="+maxError);
  }finally{god.ResetTask();foreach(var worker in workers)worker.ResetTask();stateField.SetValue(slice,original);foreach(var motion in slice.GetComponentsInChildren<HumanoidMotion>())motion.paused=original.paused||original.Finished;}
 }
 static void CheckHands(QuestSlice slice,bool drinking){
  var motion=slice.GetComponentsInChildren<HumanoidMotion>().First(m=>m.GetComponent<CarryItem>()==null);var hands=motion.GetComponentInChildren<HumanoidHands>();var animator=motion.GetComponentInChildren<Animator>();
  if(hands.resource==null||hands.drinking!=drinking)throw new Exception("Missing held resource/pose");
  motion.paused=false;motion.SendMessage("LateUpdate");hands.AdvancePose(.4f);animator.Update(.2f);hands.SendMessage("LateUpdate");
  float reach=Vector3.Distance(animator.GetBoneTransform(HumanBodyBones.RightHand).position,hands.resource.position);
  if(reach>motion.transform.lossyScale.y*.42f)throw new Exception("Hand missed carried water: "+reach);
  if(drinking&&Vector3.Distance(animator.GetBoneTransform(HumanBodyBones.Head).position,hands.resource.position)>motion.transform.lossyScale.y*.42f)throw new Exception("Drinking water did not reach the mouth");
  if(animator.GetCurrentAnimatorClipInfo(0).Any(c=>c.clip.name.StartsWith("__")))throw new Exception("Preview animation bound at runtime");
 }
}
}
