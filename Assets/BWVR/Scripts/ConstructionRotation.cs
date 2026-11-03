using UnityEngine;
namespace BWVR {
public sealed partial class QuestSlice {
 bool blockClockwiseLast,blockCounterLast;
 // Contextual A/B (desktop R/Q): free-hand teaching retains its existing path.
 bool RotateBlockInput(bool clockwise,bool counterclockwise,bool tracked,bool blocked){
  bool right=clockwise&&!blockClockwiseLast,left=counterclockwise&&!blockCounterLast;
  blockClockwiseLast=clockwise||!tracked;blockCounterLast=counterclockwise||!tracked;
  if(blocked||!tracked||clockwise==counterclockwise)return false;
  var block=held[1]!=null?held[1].GetComponent<ConstructionBlock>():null;
  if(block==null||!block.Item.held||block.Site.IsBridge||block.Site.Paused||block.Site.Locked||block.VillagerOwner!=null||!(clockwise?right:left))return false;
  var site=block.Site;var local=site.PlayerYaw(block.transform.rotation);var rotation=Quaternion.Euler(0,local.eulerAngles.y+(clockwise?90:-90),0);
  block.transform.rotation=site.transform.rotation*rotation;block.Body.rotation=rotation;site.HidePreview();return true;
 }
}
public sealed partial class ConstructionSite {
 public Quaternion PlayerYaw(Quaternion rotation){var local=Quaternion.Inverse(transform.rotation)*rotation;return Quaternion.Euler(0,Mathf.Round(local.eulerAngles.y/90)*90,0);}
 Quaternion PlayerRotation(ConstructionBlock block)=>IsBridge?transform.rotation:transform.rotation*PlayerYaw(block.transform.rotation);
 bool RecipeFacing(ConstructionBlock block)=>Vector3.Dot(block.Body.rotation*Vector3.up,Vector3.up)>.995f&&(block.Shape!=ConstructionShape.Beam||Mathf.Abs(Vector3.Dot(block.Body.rotation*Vector3.right,Vector3.right))>.995f);
}
}
