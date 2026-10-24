using UnityEngine;
using UnityEngine.Rendering;
namespace BWVR {
public sealed class IslandOcean:MonoBehaviour {
 public const int Resolution=64;
 Mesh mesh;Texture2D shore;Material material;
 public static void Build(Transform parent,IslandTerrainData terrain){
  var go=new GameObject("Living ocean - animated waves and shore foam");go.transform.SetParent(parent,false);
  go.AddComponent<IslandOcean>().Initialize(terrain);
 }
 void Initialize(IslandTerrainData terrain){
  float sea=terrain!=null?terrain.SeaLevel+.0015f:.79f;
  var center=terrain!=null?terrain.mesh.bounds.center:Vector3.zero;center.y=sea;transform.localPosition=center;
  var vertices=new Vector3[(Resolution+1)*(Resolution+1)+4];var triangles=new int[Resolution*Resolution*6+24];
  const float span=14;
  for(int z=0;z<=Resolution;z++)for(int x=0;x<=Resolution;x++)vertices[z*(Resolution+1)+x]=new Vector3((x/(float)Resolution-.5f)*span,0,(z/(float)Resolution-.5f)*span);
  int n=0;for(int z=0;z<Resolution;z++)for(int x=0;x<Resolution;x++){int a=z*(Resolution+1)+x;triangles[n++]=a;triangles[n++]=a+Resolution+1;triangles[n++]=a+1;triangles[n++]=a+1;triangles[n++]=a+Resolution+1;triangles[n++]=a+Resolution+2;}
  int firstOuter=(Resolution+1)*(Resolution+1);
  int[] corners={0,Resolution*(Resolution+1),Resolution*(Resolution+1)+Resolution,Resolution};
  for(int i=0;i<4;i++){var inner=vertices[corners[i]];vertices[firstOuter+i]=new Vector3(Mathf.Sign(inner.x)*60,0,Mathf.Sign(inner.z)*60);}
  for(int i=0;i<4;i++){int next=(i+1)%4;triangles[n++]=corners[i];triangles[n++]=firstOuter+i;triangles[n++]=firstOuter+next;triangles[n++]=corners[i];triangles[n++]=firstOuter+next;triangles[n++]=corners[next];}
  mesh=new Mesh{name="Ocean 64x64 shared grid"};mesh.vertices=vertices;mesh.triangles=triangles;mesh.RecalculateNormals();mesh.bounds=new Bounds(Vector3.zero,new Vector3(120,.1f,120));
  gameObject.AddComponent<MeshFilter>().sharedMesh=mesh;
  var source=Resources.Load<Material>("IslandOcean");
  if(source==null)throw new System.Exception("Import ocean material before playing");
  material=new Material(source);
  if(terrain!=null)BakeShore(terrain);
  var renderer=gameObject.AddComponent<MeshRenderer>();renderer.sharedMaterial=material;renderer.shadowCastingMode=ShadowCastingMode.Off;renderer.receiveShadows=true;
  Debug.Log("BWVR_OCEAN_READY: 8200 wave triangles, one material, one 256px shore map; no reflections/refraction passes");
 }
 void BakeShore(IslandTerrainData terrain){
  const int size=256;var b=terrain.mesh.bounds;var min=b.min-new Vector3(.2f,0,.2f);var span=b.size+new Vector3(.4f,0,.4f);
  var distance=new float[size*size];var wet=new bool[size*size];
  for(int z=0;z<size;z++)for(int x=0;x<size;x++){int i=z*size+x;var at=new Vector3(min.x+x/(float)(size-1)*span.x,0,min.z+z/(float)(size-1)*span.z);wet[i]=terrain.Project(at).y<transform.localPosition.y;distance[i]=wet[i]?10000:0;}
  // Two-pass distance transform: no per-frame terrain queries or depth texture needed.
  for(int z=0;z<size;z++)for(int x=0;x<size;x++){int i=z*size+x;if(x>0)distance[i]=Mathf.Min(distance[i],distance[i-1]+1);if(z>0)distance[i]=Mathf.Min(distance[i],distance[i-size]+1);}
  for(int z=size-1;z>=0;z--)for(int x=size-1;x>=0;x--){int i=z*size+x;if(x<size-1)distance[i]=Mathf.Min(distance[i],distance[i+1]+1);if(z<size-1)distance[i]=Mathf.Min(distance[i],distance[i+size]+1);}
  var pixels=new Color[size*size];
  for(int i=0;i<pixels.Length;i++){float deep=Mathf.Clamp01(distance[i]/7);float foam=wet[i]?1-Mathf.SmoothStep(0,2.5f,distance[i]):0;pixels[i]=new Color(deep,foam,wet[i]?1:0,1);}
  shore=new Texture2D(size,size,TextureFormat.RGBA32,false,true){name="Baked island shore distance",wrapMode=TextureWrapMode.Clamp,filterMode=FilterMode.Bilinear};
  shore.SetPixels(pixels);shore.Apply(false,true);material.SetTexture("_Shore",shore);material.SetVector("_ShoreRect",new Vector4(min.x,min.z,1/span.x,1/span.z));
 }
 void OnDestroy(){if(mesh!=null)Destroy(mesh);if(shore!=null)Destroy(shore);if(material!=null)Destroy(material);}
}
}
