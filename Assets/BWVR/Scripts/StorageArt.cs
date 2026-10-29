using UnityEngine;
namespace BWVR {
public sealed partial class VillageResources {
 // Fit sourced presentation to the existing storage footprint; no collision or stock ownership.
 GameObject StorePart(string model,string label,Vector3 center,Vector3 size){
  var root=new GameObject(label);root.transform.SetParent(transform,false);root.transform.position=center;root.transform.localScale=size*Navigation.Scale;
  var art=Instantiate(SourcedItemArt.Prefab("SettlementProps/"+model),root.transform);art.transform.localPosition=Vector3.zero;art.transform.localRotation=Quaternion.identity;art.transform.localScale=Vector3.one;
  var bounds=SourcedItemArt.LocalBounds(art);var fit=new Vector3(1/bounds.size.x,1/bounds.size.y,1/bounds.size.z);art.transform.localScale=fit;art.transform.localPosition=-Vector3.Scale(bounds.center,fit);return root;
 }
}
}
