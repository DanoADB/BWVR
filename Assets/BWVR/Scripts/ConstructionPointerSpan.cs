using UnityEngine;
namespace BWVR {
public sealed partial class ConstructionSite {
 // A support's top rim can suggest a braced span. Its centre keeps ordinary stacking.
 bool TryPointerSpan(ConstructionBlock block,ConstructionBlock support,Vector3 desired,out Vector3 world){
  world=default;if(IsBridge||(block.Shape!=ConstructionShape.Slab&&block.Shape!=ConstructionShape.Beam)||!RootedCoverSupport(support))return false;
  var point=Local(desired);var hit=support.PhysicsBounds;
  if(Mathf.Abs(point.x-hit.center.x)<hit.extents.x*.45f&&Mathf.Abs(point.z-hit.center.z)<hit.extents.z*.45f)return false;
  float y=block.size.y*.5f+Mathf.Round((point.y-block.size.y*.5f)/LayerHeight)*LayerHeight;
  if(Mathf.Abs(y-point.y)>.08f||y<block.size.y*.5f||y>block.size.y*.5f+23*LayerHeight)return false;
  var aligned=PlayerYaw(PlayerRotation(block));var rotated=aligned*block.size;var size=new Vector3(Mathf.Abs(rotated.x),block.size.y,Mathf.Abs(rotated.z));var half=size*.5f;float best=float.PositiveInfinity;Vector3 selected=default;
  int minX=Mathf.CeilToInt((point.x-half.x+.1f)/2),maxX=Mathf.FloorToInt((point.x+half.x-.1f)/2),minZ=Mathf.CeilToInt((point.z-half.z+.1f)/2),maxZ=Mathf.FloorToInt((point.z+half.z-.1f)/2);
  // At most nine Slab candidates or three Beam candidates, all at the contacted height.
  for(int x=minX;x<=maxX;x++)for(int z=minZ;z<=maxZ;z++){
   var at=new Vector3(x*2,y,z*2);if(Mathf.Abs(at.x)+half.x>HalfSize.x-1||Mathf.Abs(at.z)+half.z>HalfSize.y-1)continue;
   float distance=(at-point).sqrMagnitude;if(distance>=best||!BracedUnder(new Bounds(at,size),block)||!Support(at,block,out _,aligned)||!Empty(block,at,aligned))continue;
   best=distance;selected=at;
  }
  if(float.IsPositiveInfinity(best))return false;world=World(selected);return true;
 }
}
}
