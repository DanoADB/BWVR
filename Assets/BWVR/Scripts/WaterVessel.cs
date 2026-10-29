using UnityEngine;
namespace BWVR {
// The container is sourced artwork. Only its changing water surface is effect geometry.
public sealed class WaterVessel:MonoBehaviour {
 Transform water;Mesh surfaceMesh;Material surfaceMaterial;float radius,bottom,rim;
 public float Fill{get;private set;}public string Model{get;private set;}
 public static WaterVessel Create(Transform parent,string name,float height,float radius,string model="bucket"){
  var root=new GameObject(name);root.transform.localScale=Vector3.one*height;root.transform.SetParent(parent,true);
  var vessel=root.AddComponent<WaterVessel>();vessel.Model=model;vessel.radius=radius/height*.78f;
  vessel.bottom=.06f;vessel.rim=model=="bucket"?.60f:.93f;
  var body=Object.Instantiate(SourcedItemArt.Prefab("SettlementProps/"+model),root.transform);body.name="Kenney "+model;
  body.transform.localPosition=Vector3.zero;body.transform.localRotation=Quaternion.identity;body.transform.localScale=Vector3.one;
  var bounds=SourcedItemArt.LocalBounds(body);body.transform.localScale=new Vector3(radius*2/(height*bounds.size.x),1,radius*2/(height*bounds.size.z));
  // A small fan is a liquid effect, never a stand-in for the container model.
  const int segments=16;var vertices=new Vector3[segments+1];var triangles=new int[segments*3];
  for(int i=0;i<segments;i++){float a=i*Mathf.PI*2/segments;vertices[i+1]=new Vector3(Mathf.Cos(a),0,Mathf.Sin(a));triangles[i*3]=0;triangles[i*3+1]=(i+1)%segments+1;triangles[i*3+2]=i+1;}
  vessel.surfaceMesh=new Mesh{name="Dynamic water surface"};vessel.surfaceMesh.vertices=vertices;vessel.surfaceMesh.triangles=triangles;vessel.surfaceMesh.RecalculateNormals();vessel.surfaceMesh.RecalculateBounds();
  var surface=new GameObject("Visible water");surface.transform.SetParent(root.transform,false);surface.AddComponent<MeshFilter>().sharedMesh=vessel.surfaceMesh;
  vessel.surfaceMaterial=new Material(Shader.Find("BWVR/GameSurface")){color=new Color(.12f,.53f,.69f),enableInstancing=true};surface.AddComponent<MeshRenderer>().sharedMaterial=vessel.surfaceMaterial;
  vessel.water=surface.transform;vessel.SetFill(0);return vessel;
 }
 public void SetFill(float amount){Fill=float.IsNaN(amount)||float.IsInfinity(amount)?0:Mathf.Clamp01(amount);water.gameObject.SetActive(Fill>.01f);water.localPosition=Vector3.up*Mathf.Lerp(bottom,rim,Fill);float r=radius*Mathf.Lerp(.8f,1,Fill);water.localScale=new Vector3(r,1,r);}
 void OnDestroy(){if(surfaceMesh!=null)Destroy(surfaceMesh);if(surfaceMaterial!=null)Destroy(surfaceMaterial);}
}
}
