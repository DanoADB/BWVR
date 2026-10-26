using System;
using UnityEngine;
namespace BWVR {
// Useful cover is derived from the existing saved pieces, never a recipe-complete flag.
public sealed partial class ConstructionSite {
 bool RootedCoverSupport(ConstructionBlock block,int depth=0){
  if(block==null||depth>=blocks.Count||!block.Attached||block.Item.held||block.wasHeld||block.VillagerOwner!=null||block.Shape==ConstructionShape.Ramp||block.Shape==ConstructionShape.Arch)return false;
  if(Vector3.Dot(block.Body.rotation*Vector3.up,Vector3.up)<.995f||block.Body.linearVelocity.sqrMagnitude>.04f||block.Body.angularVelocity.sqrMagnitude>.04f)return false;
  var joint=block.Connection;if(joint.connectedBody==null)return Mathf.Abs(block.PhysicsBounds.min.y)<.08f;
  var parent=blocks.Find(b=>b.Body==joint.connectedBody);if(parent==null)return false;
  var a=block.PhysicsBounds;var b=parent.PhysicsBounds;
  return Mathf.Abs(a.min.y-b.max.y)<.08f&&a.min.x<b.max.x-.05f&&a.max.x>b.min.x+.05f&&a.min.z<b.max.z-.05f&&a.max.z>b.min.z+.05f&&RootedCoverSupport(parent,depth+1);
 }
 bool RoofSupported(ConstructionBlock roof){
  if(!RootedCoverSupport(roof))return false;var bounds=roof.PhysicsBounds;bool left=false,right=false,front=false,back=false;
  foreach(var b in blocks){if(b==roof||!RootedCoverSupport(b))continue;var below=b.PhysicsBounds;
   if(Mathf.Abs(below.max.y-bounds.min.y)>.08f||below.min.x>=bounds.max.x-.1f||below.max.x<=bounds.min.x+.1f||below.min.z>=bounds.max.z-.1f||below.max.z<=bounds.min.z+.1f)continue;
   left|=below.center.x<bounds.center.x-.5f;right|=below.center.x>bounds.center.x+.5f;front|=below.center.z<bounds.center.z-.5f;back|=below.center.z>bounds.center.z+.5f;
  }return (left&&right)||(front&&back);
 }
 bool ShelterLane(Vector3 point,Vector3 entry,float radius,float height){
  // Swept upright clearance prism, conservative at corners; includes all loose pieces.
  var low=Vector3.Min(point,entry);var high=Vector3.Max(point,entry);var lane=new Bounds();lane.SetMinMax(new Vector3(low.x-radius,.05f,low.z-radius),new Vector3(high.x+radius,height,high.z+radius));
  foreach(var b in blocks)if(!b.Item.held&&b.PhysicsBounds.Intersects(lane))return false;return true;
 }
 public bool TryShelterSpot(float radius,float height,out Vector3 rest,out Vector3 entrance){
  rest=entrance=default;if(IsBridge||!HybridState.Number(radius,.1f,4)||!HybridState.Number(height,.5f,12))return false;
  foreach(var roof in blocks){if((roof.Shape!=ConstructionShape.Slab&&roof.Shape!=ConstructionShape.Beam)||!RoofSupported(roof))continue;var bounds=roof.PhysicsBounds;var point=new Vector3(bounds.center.x,0,bounds.center.z);
   if(bounds.min.y<height+.2f||bounds.min.y>height+6||bounds.extents.x<radius+.2f||bounds.extents.z<radius+.2f)continue;
   foreach(var direction in new[]{Vector3.back,Vector3.forward,Vector3.left,Vector3.right}){var entry=point;if(direction.x!=0)entry.x=direction.x*(HalfSize.x+2);else entry.z=direction.z*(HalfSize.y+2);
    if(!ShelterLane(point,entry,radius,height))continue;rest=World(point);entrance=World(entry);return true;
   }
  }return false;
 }
}
public sealed partial class SocialMemory {
 public int shelterRests;public float shelteredSeconds;
 bool ShelterValid()=>Count(shelterRests)&&HybridState.Number(shelteredSeconds,0,100000000);
 public void RememberShelter(float seconds,float rain){if(!HybridState.Number(seconds,0,30)||!HybridState.Number(rain,0,1))return;shelterRests=Math.Min(1000000,shelterRests+1);shelteredSeconds=Mathf.Min(100000000,shelteredSeconds+Mathf.Clamp(seconds,0,30));restNeed=Mathf.Max(0,restNeed-.45f-.2f*Mathf.Clamp01(rain));}
}
}
