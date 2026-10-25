using System.Collections.Generic;
using UnityEngine;
namespace BWVR {
public sealed partial class VillageResources {
 readonly List<Mesh> resourceMeshes=new List<Mesh>();
 GameObject WheatPatch(Vector3 at){var root=new GameObject("Harvestable wheat stalks");root.transform.SetParent(transform,false);root.transform.position=at;root.transform.localScale=Vector3.one*Navigation.Scale;
  var template=GameObject.CreatePrimitive(PrimitiveType.Cube);template.SetActive(false);var cube=template.GetComponent<MeshFilter>().sharedMesh;Destroy(template);
  for(int layer=0;layer<2;layer++){var pieces=new CombineInstance[9];for(int i=0;i<9;i++){var position=new Vector3((i%3-1)*.8f,layer==0?.45f:1.05f,(i/3-1)*.8f);var scale=layer==0?new Vector3(.06f,.9f,.06f):new Vector3(.2f,.4f,.17f);pieces[i]=new CombineInstance{mesh=cube,transform=Matrix4x4.TRS(position,Quaternion.Euler(0,i*37,layer==0?0:8),scale)};}var mesh=new Mesh{name=layer==0?"Batched wheat stems":"Batched grain heads"};mesh.CombineMeshes(pieces);resourceMeshes.Add(mesh);var go=new GameObject(mesh.name);go.transform.SetParent(root.transform,false);go.AddComponent<MeshFilter>().sharedMesh=mesh;go.AddComponent<MeshRenderer>().sharedMaterial=layer==0?roof:grain;}
  return root;
 }
 public GameObject CreateAxe(){var root=new GameObject("Woodcutter hand axe");root.transform.SetParent(transform,false);root.transform.localScale=Vector3.one*Navigation.Scale;AxePart(root.transform,"Handle",new Vector3(0,.3f,0),new Vector3(.13f,1.3f,.13f),timber);AxePart(root.transform,"Axe head",new Vector3(.2f,.82f,0),new Vector3(.7f,.3f,.2f),roof);return root;}
 void AxePart(Transform parent,string name,Vector3 position,Vector3 size,Material material){var go=GameObject.CreatePrimitive(PrimitiveType.Cube);go.name=name;go.transform.SetParent(parent,false);go.transform.localPosition=position;go.transform.localScale=size;go.GetComponent<Renderer>().sharedMaterial=material;var collider=go.GetComponent<Collider>();collider.enabled=false;Destroy(collider);}
}
}
