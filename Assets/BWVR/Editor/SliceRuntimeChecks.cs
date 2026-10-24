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
   for(int n=0;n<1200&&god.deliveries==before;n++){
    seen.Add(god.Current);god.Tick(.1f,false);
    if(god.Current==CreatureRoutine.Task.Carry&&!handsChecked){CheckHands(slice,false);handsChecked=true;}
    if(god.Current==CreatureRoutine.Task.Drink&&!drinkChecked){CheckHands(slice,true);drinkChecked=true;}
   }
   if(seen.Count!=5||test.gifts!=1||god.deliveries!=before+1||!handsChecked||!drinkChecked)throw new Exception("Creature did not complete exactly one pickup/drink/carry/delivery cycle");
   if(water.held||!water.GetComponent<Collider>().enabled)throw new Exception("Delivered water was not released");
   var position=god.transform.position;var phase=god.Current;god.Tick(20,true);if(god.transform.position!=position||god.Current!=phase)throw new Exception("Paused task advanced");
   foreach(var worker in workers){before=worker.completedJobs;for(int n=0;n<1600&&worker.completedJobs==before;n++)worker.Tick(.1f,false);if(worker.completedJobs!=before+1)throw new Exception("Worker failed to deliver: "+worker.role);}
   var terrain=Resources.Load<IslandTerrainData>("FirstIsland");int samples=0;float maxError=0;
   if(terrain!=null){var collider=GameObject.Find("Land 1 - original island geometry").GetComponent<MeshCollider>();for(int i=0;i<100;i++){
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
  if(reach>.14f)throw new Exception("Hand missed carried water: "+reach);
  if(drinking&&Vector3.Distance(animator.GetBoneTransform(HumanBodyBones.Head).position,hands.resource.position)>.14f)throw new Exception("Drinking water did not reach the mouth");
  if(animator.GetCurrentAnimatorClipInfo(0).Any(c=>c.clip.name.StartsWith("__")))throw new Exception("Preview animation bound at runtime");
 }
}
}
