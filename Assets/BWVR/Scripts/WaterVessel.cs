using UnityEngine;
namespace BWVR {
// An open container makes an empty/full state readable without a meter or caption.
public sealed class WaterVessel:MonoBehaviour {
 Transform water;public float Fill{get;private set;}
 public static WaterVessel Create(Transform parent,string name,float height,float radius,Color color){
  var root=new GameObject(name);root.transform.localScale=Vector3.one*height;root.transform.SetParent(parent,true);
  var vessel=root.AddComponent<WaterVessel>();var mesh=new Mesh{name="Open water container"};var vertices=new System.Collections.Generic.List<Vector3>();var triangles=new System.Collections.Generic.List<int>();
  float outer=radius/height,inner=outer*.78f;
  void Quad(Vector3 a,Vector3 b,Vector3 c,Vector3 d){int i=vertices.Count;vertices.AddRange(new[]{a,b,c,d});triangles.AddRange(new[]{i,i+1,i+2,i,i+2,i+3});}
  for(int i=0;i<12;i++){float a=i*Mathf.PI/6,b=(i+1)*Mathf.PI/6;var x=new Vector3(Mathf.Cos(a),0,Mathf.Sin(a));var y=new Vector3(Mathf.Cos(b),0,Mathf.Sin(b));
   Quad(x*outer*.8f,y*outer*.8f,y*outer+Vector3.up,x*outer+Vector3.up);
   Quad(y*inner+Vector3.up,x*inner+Vector3.up,x*inner*.8f+Vector3.up*.08f,y*inner*.8f+Vector3.up*.08f);
   Quad(x*inner+Vector3.up,y*inner+Vector3.up,y*outer+Vector3.up,x*outer+Vector3.up);
   Quad(Vector3.up*.08f,x*inner*.8f+Vector3.up*.08f,y*inner*.8f+Vector3.up*.08f,Vector3.up*.08f);
  }
  mesh.SetVertices(vertices);mesh.SetTriangles(triangles,0);mesh.RecalculateNormals();mesh.RecalculateBounds();root.AddComponent<MeshFilter>().sharedMesh=mesh;
  var material=new Material(Shader.Find("BWVR/GameSurface")){color=color};root.AddComponent<MeshRenderer>().sharedMaterial=material;
  var surface=GameObject.CreatePrimitive(PrimitiveType.Cylinder);surface.name="Visible water";surface.transform.SetParent(root.transform,false);surface.transform.localScale=new Vector3(inner*1.8f,.012f,inner*1.8f);Object.Destroy(surface.GetComponent<Collider>());surface.GetComponent<Renderer>().sharedMaterial=new Material(Shader.Find("BWVR/GameSurface")){color=new Color(.12f,.53f,.69f)};vessel.water=surface.transform;vessel.SetFill(0);return vessel;
 }
 public void SetFill(float amount){Fill=Mathf.Clamp01(amount);water.gameObject.SetActive(Fill>.01f);water.localPosition=Vector3.up*(.1f+Fill*.78f);}
 void OnDestroy(){var mesh=GetComponent<MeshFilter>();if(mesh!=null)Destroy(mesh.sharedMesh);var renderer=GetComponent<Renderer>();if(renderer!=null)Destroy(renderer.sharedMaterial);if(water!=null)Destroy(water.GetComponent<Renderer>().sharedMaterial);}
}
}
