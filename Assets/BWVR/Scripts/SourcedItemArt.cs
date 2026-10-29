using System;
using UnityEngine;
namespace BWVR {
// Selected CC0 prefabs are normalized to height one with their base at zero.
// Presentation can change without changing the gameplay root, IDs or inventory.
public static class SourcedItemArt {
 public static GameObject Prefab(string path)=>Resources.Load<GameObject>(path)??throw new InvalidOperationException("Missing sourced item model: "+path);
 public static GameObject Create(string path,Transform parent,Vector3 position,float height,bool centered=false){
  var root=new GameObject(System.IO.Path.GetFileName(path));var model=UnityEngine.Object.Instantiate(Prefab(path),root.transform);
  model.transform.localPosition=centered?Vector3.down*.5f:Vector3.zero;model.transform.localRotation=Quaternion.identity;model.transform.localScale=Vector3.one;
  root.transform.localScale=Vector3.one*height;root.transform.SetParent(parent,true);root.transform.position=position;return root;
 }
 public static Bounds LocalBounds(GameObject root){
  bool found=false;var bounds=new Bounds();
  foreach(var filter in root.GetComponentsInChildren<MeshFilter>(true)){
   if(filter.sharedMesh==null)continue;var b=filter.sharedMesh.bounds;
   for(int i=0;i<8;i++){var v=b.center+Vector3.Scale(b.extents,new Vector3((i&1)==0?-1:1,(i&2)==0?-1:1,(i&4)==0?-1:1));var p=root.transform.InverseTransformPoint(filter.transform.TransformPoint(v));if(!found){bounds=new Bounds(p,Vector3.zero);found=true;}else bounds.Encapsulate(p);}
  }
  if(!found)throw new InvalidOperationException("Sourced item has no mesh: "+root.name);return bounds;
 }
 public static void AddCollider(GameObject root){var bounds=LocalBounds(root);var box=root.AddComponent<BoxCollider>();box.center=bounds.center;box.size=bounds.size;}
 public static GameObject OfferingModel(Transform parent,string name,Offering kind,Vector3 at,float height){
  GameObject root;
  if(kind==Offering.Water){var vessel=WaterVessel.Create(parent,name,height,height*.42f);vessel.SetFill(1);root=vessel.gameObject;root.transform.position=at;}
  else if(kind==Offering.Food||kind==Offering.Wood)root=Create(kind==Offering.Food?"FoodProps/apple":"SettlementProps/resource-wood",parent,at,height);
  else return null;
  root.name=name;root.AddComponent<SourcedPickup>().modelPath=kind==Offering.Water?"SettlementProps/bucket":kind==Offering.Food?"FoodProps/apple":"SettlementProps/resource-wood";AddCollider(root);return root;
 }
}
}
