using System;
using UnityEngine;
using UnityEngine.Rendering;
namespace BWVR {
// One atlas/material and three shared meshes for all palette, held and placed emblems.
public sealed class GodToolEmblems:IDisposable {
 public const string Credit="Icons: Lorc & Delapouite · game-icons.net · CC BY 3.0";
 public readonly Material Material;readonly Mesh quad,disc,ring;readonly MaterialPropertyBlock block=new MaterialPropertyBlock();
 static readonly string[] labels={"Helping hands","Heart shield","Magnifying glass","Rain cloud","Healing","Watering can","Wind trails","Seedling","Food offering","Timber gift"};
 static readonly Color[] colors={new Color(.35f,.9f,.73f),new Color(.4f,.7f,1),new Color(1,.78f,.3f),new Color(.5f,.66f,1),new Color(1,.47f,.57f),new Color(.35f,.87f,.95f),new Color(.75f,.63f,1),new Color(.55f,.9f,.4f),new Color(1,.72f,.35f),new Color(.85f,.63f,.4f)};
 public GodToolEmblems(){var atlas=Resources.Load<Texture2D>("GodToolIcons/atlas");var shader=Resources.Load<Shader>("GodToolIcons/ToolEmblem");if(atlas==null||shader==null)throw new InvalidOperationException("God tool emblem artwork is missing");Material=new Material(shader){name="Shared open-art tool emblems",mainTexture=atlas};quad=Quad();disc=Radial(0,1.24f);ring=Radial(1.25f,1.34f);}
 public GameObject Create(SpiritualTool kind,Transform parent){int index=(int)kind;if(index<0||index>=labels.Length)throw new ArgumentOutOfRangeException(nameof(kind));var root=new GameObject(labels[index]+" floating emblem");root.transform.SetParent(parent,false);
  Part("Soft rim",root.transform,ring,colors[index]*new Color(1,1,1,.18f),true,index,.065f,1.12f);
  Part("Dark enamel",root.transform,disc,new Color(.025f,.065f,.085f,.97f),true,index,.035f,1);
  Part("Colored rim",root.transform,ring,colors[index],true,index,0,1);
  Part("Open-source icon",root.transform,quad,new Color(1,.97f,.88f),false,index,-.025f,1.13f);return root;
 }
 void Part(string name,Transform parent,Mesh mesh,Color color,bool solid,int index,float z,float scale){var go=new GameObject(name,typeof(MeshFilter),typeof(MeshRenderer));go.transform.SetParent(parent,false);go.transform.localPosition=Vector3.forward*z;go.transform.localScale=Vector3.one*scale;go.GetComponent<MeshFilter>().sharedMesh=mesh;var renderer=go.GetComponent<MeshRenderer>();renderer.sharedMaterial=Material;renderer.shadowCastingMode=ShadowCastingMode.Off;renderer.receiveShadows=false;renderer.lightProbeUsage=LightProbeUsage.Off;renderer.reflectionProbeUsage=ReflectionProbeUsage.Off;
  block.Clear();block.SetColor("_Color",color);block.SetFloat("_Solid",solid?1:0);block.SetVector("_AtlasRect",new Vector4(.2f,.5f,(index%5)*.2f,.5f-(index/5)*.5f));renderer.SetPropertyBlock(block);
 }
 static Mesh Quad(){var mesh=new Mesh{name="Shared tool icon quad"};mesh.vertices=new[]{new Vector3(-1,-1,0),new Vector3(-1,1,0),new Vector3(1,1,0),new Vector3(1,-1,0)};mesh.uv=new[]{new Vector2(0,0),new Vector2(0,1),new Vector2(1,1),new Vector2(1,0)};mesh.triangles=new[]{0,1,2,0,2,3};mesh.RecalculateBounds();return mesh;}
 static Mesh Radial(float inner,float outer){const int segments=32;var vertices=new Vector3[(segments+1)*2];var triangles=new int[segments*6];for(int i=0;i<=segments;i++){float angle=i*Mathf.PI*2/segments;var direction=new Vector3(Mathf.Cos(angle),Mathf.Sin(angle),0);vertices[i*2]=direction*inner;vertices[i*2+1]=direction*outer;if(i==segments)continue;int n=i*6,v=i*2;triangles[n]=v;triangles[n+1]=v+1;triangles[n+2]=v+3;triangles[n+3]=v;triangles[n+4]=v+3;triangles[n+5]=v+2;}var mesh=new Mesh{name=inner==0?"Shared enamel disc":"Shared emblem ring",vertices=vertices,triangles=triangles};mesh.RecalculateBounds();return mesh;}
 public void Dispose(){if(Material!=null)UnityEngine.Object.Destroy(Material);if(quad!=null)UnityEngine.Object.Destroy(quad);if(disc!=null)UnityEngine.Object.Destroy(disc);if(ring!=null)UnityEngine.Object.Destroy(ring);}
}
}
