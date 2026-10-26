using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace BWVR.Editor {
public static class ActorMovementChecks {
 static void Require(bool value,string message){if(!value)throw new Exception(message);}
 [MenuItem("BWVR/Verify actor movement and carrying (Play mode)")]
 public static void Verify(){
  Require(Application.isPlaying,"Enter Play mode first");
  var slice=UnityEngine.Object.FindAnyObjectByType<QuestSlice>();var nav=slice.GetComponentInChildren<IslandNavigation>();
  var origin=slice.GetComponentInChildren<BeachBall>().transform.position;
  var temporary=new GameObject("Temporary actor movement verification");temporary.transform.SetParent(slice.transform,false);
  try{
   Vector3 center=default;bool found=false;
   for(int i=0;i<160&&!found;i++){var wanted=origin+Quaternion.Euler(0,i*137.5f,0)*Vector3.forward*nav.Scale*(30+i);
    if(!nav.Sample(wanted,nav.creature,1,out center))continue;found=true;
    foreach(var offset in new[]{new Vector3(-16,0,0),new Vector3(16,0,0),new Vector3(0,0,-7),new Vector3(0,0,7)}){
     var at=center+offset*nav.Scale;if(!nav.Sample(at,nav.creature,.1f,out var safe)||!nav.SegmentClear(center,safe,nav.creature)){found=false;break;}
    }
   }Require(found,"No open actor avoidance fixture");
   IslandNavigator Actor(string name,Vector3 at,bool large){var go=new GameObject(name);go.transform.SetParent(temporary.transform,false);go.transform.position=nav.ProjectFoot(at,large?nav.creature:nav.villager);go.AddComponent<CarryItem>();var n=go.AddComponent<IslandNavigator>();n.Initialize(nav,large);return n;}
   var big=Actor("Creature body",center-Vector3.right*nav.Scale*12,true);var small=Actor("Villager body",center,false);
   float clearance=(big.profile.radius+small.profile.radius)*nav.Scale;
   Require(!big.CanPlace(small.transform.position,out _),"Placement overlapped another actor");
   Require(!big.BodyClear(center-Vector3.right*nav.Scale*12,center+Vector3.right*nav.Scale*12),"Swept collision tunneled through actor");
   bool arrived=false;float closest=float.MaxValue;
   for(int i=0;i<600&&!arrived;i++){arrived=big.MoveTo(center+Vector3.right*nav.Scale*12,nav.Scale*6,.1f);var d=big.transform.position-small.transform.position;d.y=0;closest=Mathf.Min(closest,d.magnitude);Require(d.magnitude>=clearance-nav.Scale*.03f,"Creature overlapped stationary villager");}
   Require(arrived,"Creature could not steer around stationary villager");
   big.ResetRoute();arrived=false;for(int i=0;i<600&&!arrived;i++)arrived=big.MoveTo(small.transform.position,nav.Scale*6,.1f);
   Require(arrived&&Vector3.Distance(big.transform.position,small.transform.position)>=clearance-nav.Scale*.03f,"Social approach failed or entered target body");
   small.GetComponent<CarryItem>().held=true;Require(big.BodyClear(small.transform.position,small.transform.position),"Carried villager remained a ground obstacle");small.GetComponent<CarryItem>().held=false;
   big.transform.position=nav.ProjectFoot(center-Vector3.right*nav.Scale*12,nav.creature);small.transform.position=nav.ProjectFoot(center+Vector3.right*nav.Scale*12,nav.villager);big.ResetRoute();small.ResetRoute();bool first=false,second=false;
   for(int i=0;i<600&&(!first||!second);i++){if(!first)first=big.MoveTo(center+Vector3.right*nav.Scale*12,nav.Scale*6,.1f);if(!second)second=small.MoveTo(center-Vector3.right*nav.Scale*12,nav.Scale*6,.1f);var d=big.transform.position-small.transform.position;d.y=0;Require(d.magnitude>=clearance-nav.Scale*.03f,"Head-on actors overlapped");}
   Require(first&&second,"Head-on actors failed to pass");
   CarryWalk(temporary.transform,nav,center);GroundedGorilla(temporary.transform);
   Debug.Log("BWVR_ACTOR_MOVEMENT_PASS: carrying walk/leg motion/hand pose/pause; gorilla walk sole alignment; swept cross-profile body clearance, placement, stationary detour, social reach, held exclusion and head-on passing");
  }finally{UnityEngine.Object.DestroyImmediate(temporary);}
 }
 static void CarryWalk(Transform parent,IslandNavigation nav,Vector3 center){
  var go=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Villagers/Villager0"),parent);go.AddComponent<CarryItem>();go.transform.position=center;go.transform.localScale=Vector3.one*nav.Scale*1.661f*QuestSlice.VillagerScale;
  var motion=go.GetComponent<HumanoidMotion>();var a=go.GetComponentInChildren<Animator>();var hands=go.GetComponentInChildren<HumanoidHands>();var load=new GameObject("Visible cargo");load.transform.SetParent(parent,false);motion.Carry(load.transform);motion.Activity(5);go.transform.position+=Vector3.right*nav.Scale;motion.SendMessage("LateUpdate");
  Require(a.GetFloat("Speed")==1&&a.GetInteger("Activity")==0&&a.speed>0,"Cargo or work pose suppressed walking");
  a.Play("Idle and walk",0,.1f);a.Update(0);var foot=a.GetBoneTransform(HumanBodyBones.LeftFoot);var before=go.transform.InverseTransformPoint(foot.position);a.Play("Idle and walk",0,.6f);a.Update(0);
  Require(Vector3.Distance(before,go.transform.InverseTransformPoint(foot.position))>.001f,"Carrying legs did not animate");hands.SendMessage("LateUpdate");Require(Vector3.Distance(load.transform.position,go.transform.position)<go.transform.lossyScale.y,"Load left carrying pose");
  motion.paused=true;motion.SendMessage("LateUpdate");Require(a.speed==0&&hands.paused,"Pause did not freeze carrying animation");
 }
 static void GroundedGorilla(Transform parent){
  var source=Resources.Load<GameObject>("BWVRGod/Gorilla");Require(source!=null,"Missing purchased gorilla");var g=UnityEngine.Object.Instantiate(source,parent);g.transform.localPosition=Vector3.down*.06f;var a=g.GetComponentInChildren<Animator>();var mesh=new Mesh();
  try{a.Rebind();a.SetFloat("Speed",1);foreach(float phase in new[]{0f,.2f,.4f,.6f,.8f}){a.Play("Idle and walk",0,phase);a.Update(0);float low=float.MaxValue;foreach(var r in g.GetComponentsInChildren<SkinnedMeshRenderer>()){r.BakeMesh(mesh);foreach(var v in mesh.vertices)low=Mathf.Min(low,parent.InverseTransformPoint(r.transform.TransformPoint(v)).y);}Require(Mathf.Abs(low)<.012f,"Gorilla walk floats or sinks at phase "+phase+": "+low);}}
  finally{UnityEngine.Object.DestroyImmediate(mesh);}
 }
}
}
