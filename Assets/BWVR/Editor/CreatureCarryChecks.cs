using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace BWVR.Editor {
public static class CreatureCarryChecks {
 [MenuItem("BWVR/Verify creature pickup (Play mode)")]
 public static void Verify(){
  if(!Application.isPlaying)throw new Exception("Enter Play mode");
  var slice=UnityEngine.Object.FindAnyObjectByType<QuestSlice>();var god=slice.GetComponentInChildren<CreatureHybrid>();var item=god.GetComponent<CarryItem>();var nav=god.GetComponent<IslandNavigator>();
  var flags=BindingFlags.Instance|BindingFlags.NonPublic;var grab=typeof(QuestSlice).GetMethod("TryGrab",flags);var release=typeof(QuestSlice).GetMethod("Release",flags);var position=god.transform.position;var home=item.home;
  try{
   if((bool)grab.Invoke(slice,new object[]{0,item}))throw new Exception("Left grip picked up creature");
   if(!(bool)grab.Invoke(slice,new object[]{1,item})||!item.held||item.GetComponent<Collider>().enabled||god.Owner!=CreatureHybrid.ActionOwner.None||god.State.intention.active)throw new Exception("Pickup did not release action ownership");
   var lifted=position+Vector3.up*.3f;god.transform.position=lifted;int decisions=god.State.decisions;god.Tick(5);
   if(god.transform.position!=lifted||god.State.decisions!=decisions)throw new Exception("Creature acted while held");
   if((bool)grab.Invoke(slice,new object[]{1,item}))throw new Exception("Double grab accepted");
   release.Invoke(slice,new object[]{1,new Ray(position+Vector3.up*2,Vector3.down)});
   if(item.held||!item.GetComponent<Collider>().enabled||!nav.CanPlace(god.transform.position,out var safe)||Vector3.Distance(safe,god.transform.position)>.001f)throw new Exception("Creature did not land on walkable ground");
   var placed=god.transform.position;if(!(bool)grab.Invoke(slice,new object[]{1,item}))throw new Exception("Regrab failed");god.transform.position+=Vector3.up;
   release.Invoke(slice,new object[]{1,new Ray(position+Vector3.up*2,Vector3.up)});
   if(item.held||Vector3.Distance(placed,god.transform.position)>.0001f)throw new Exception("Invalid drop did not return to pickup position");
   god.Tick(1);if(god.State.decisions<=decisions)throw new Exception("Autonomy did not resume");
   Debug.Log("BWVR_CREATURE_CARRY_PASS: right-only pickup, exclusive ownership, no decisions or movement while held, double-grab rejection, walkable drop, invalid-drop fallback and autonomous resume");
  }finally{god.Interrupt("Creature carry QA done");item.held=false;item.GetComponent<Collider>().enabled=true;god.transform.position=position;item.home=home;nav.ResetRoute();}
 }
}
}
