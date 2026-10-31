using System;
using UnityEngine;
using UnityEngine.Rendering;
namespace BWVR {
// Shared icon art and beveled stone meshes; decorative only, without physics.
public sealed class GodToolEmblems:IDisposable {
 public const string Credit="Icons: Lorc & Delapouite · game-icons.net · CC BY 3.0";
 public readonly Material Material;readonly Material stoneMaterial;readonly Mesh quad,disc,ring,stone;readonly MaterialPropertyBlock block=new MaterialPropertyBlock();
 static readonly string[] labels={"Helping hands","Heart shield","Magnifying glass","Rain cloud","Healing","Watering can","Wind trails","Seedling","Food offering","Timber gift"};
 static readonly Color[] colors={new Color(.35f,.9f,.73f),new Color(.4f,.7f,1),new Color(1,.78f,.3f),new Color(.5f,.66f,1),new Color(1,.47f,.57f),new Color(.35f,.87f,.95f),new Color(.75f,.63f,1),new Color(.55f,.9f,.4f),new Color(1,.72f,.35f),new Color(.85f,.63f,.4f)};
 public GodToolEmblems(){var atlas=Resources.Load<Texture2D>("GodToolIcons/atlas");var shader=Resources.Load<Shader>("GodToolIcons/ToolEmblem");if(atlas==null||shader==null)throw new InvalidOperationException("God tool emblem artwork is missing");Material=new Material(shader){name="Shared open-art tool emblems",mainTexture=atlas};stoneMaterial=new Material(Resources.Load<Shader>("GodToolIcons/StoneToken")){name="Shared carved stone tool disks"};quad=Quad();disc=Radial(0,1.32f);ring=Radial(1.11f,1.135f);stone=Stone();}
 public GameObject Create(SpiritualTool kind,Transform parent){int index=kind==SpiritualTool.Whirlwind?6:(int)kind;if(index<0||index>=labels.Length)throw new ArgumentOutOfRangeException(nameof(kind));var root=new GameObject((kind==SpiritualTool.Whirlwind?"Whirlwind":labels[index])+" floating emblem");root.transform.SetParent(parent,false);
  Part("Stone contact shadow",root.transform,disc,new Color(.09f,.065f,.04f,.23f),true,index,.36f,1.06f);
  var body=Part("Raised beveled stone",root.transform,stone,Color.white,true,index,0,1);body.GetComponent<MeshRenderer>().sharedMaterial=stoneMaterial;
  Part("Inlaid intention ring",root.transform,ring,Color.Lerp(colors[index],new Color(.72f,.69f,.59f),.4f),true,index,-.205f,1);
  Part("Open-source icon",root.transform,quad,new Color(.98f,.94f,.80f),false,index,-.215f,.99f);return root;
 }
 GameObject Part(string name,Transform parent,Mesh mesh,Color color,bool solid,int index,float z,float scale){var go=new GameObject(name,typeof(MeshFilter),typeof(MeshRenderer));go.transform.SetParent(parent,false);go.transform.localPosition=Vector3.forward*z;go.transform.localScale=Vector3.one*scale;go.GetComponent<MeshFilter>().sharedMesh=mesh;var renderer=go.GetComponent<MeshRenderer>();renderer.sharedMaterial=Material;renderer.shadowCastingMode=ShadowCastingMode.Off;renderer.receiveShadows=false;renderer.lightProbeUsage=LightProbeUsage.Off;renderer.reflectionProbeUsage=ReflectionProbeUsage.Off;
  block.Clear();block.SetColor("_Color",color);block.SetFloat("_Solid",solid?1:0);block.SetVector("_AtlasRect",new Vector4(.2f,.5f,(index%5)*.2f,.5f-(index/5)*.5f));renderer.SetPropertyBlock(block);return go;
 }
 static Mesh Quad(){var mesh=new Mesh{name="Shared tool icon quad"};mesh.vertices=new[]{new Vector3(-1,-1,0),new Vector3(-1,1,0),new Vector3(1,1,0),new Vector3(1,-1,0)};mesh.uv=new[]{new Vector2(0,0),new Vector2(0,1),new Vector2(1,1),new Vector2(1,0)};mesh.triangles=new[]{0,1,2,0,2,3};mesh.RecalculateBounds();return mesh;}
 static Mesh Radial(float inner,float outer){const int segments=32;var vertices=new Vector3[(segments+1)*2];var triangles=new int[segments*6];for(int i=0;i<=segments;i++){float angle=i*Mathf.PI*2/segments;var direction=new Vector3(Mathf.Cos(angle),Mathf.Sin(angle),0);vertices[i*2]=direction*inner;vertices[i*2+1]=direction*outer;if(i==segments)continue;int n=i*6,v=i*2;triangles[n]=v;triangles[n+1]=v+1;triangles[n+2]=v+3;triangles[n+3]=v;triangles[n+4]=v+3;triangles[n+5]=v+2;}var mesh=new Mesh{name=inner==0?"Shared enamel disc":"Shared emblem ring",vertices=vertices,triangles=triangles};mesh.RecalculateBounds();return mesh;}
 // A shallow 48-sided cylinder with two bevels. Baked directional vertex shading
 // makes its depth legible without adding a light, normal map or per-frame work.
 static Mesh Stone(){const int count=48;var profile=new[]{new Vector2(0,-.19f),new Vector2(1.04f,-.19f),new Vector2(1.22f,-.14f),new Vector2(1.32f,-.015f),new Vector2(1.32f,.20f),new Vector2(1.17f,.28f),new Vector2(0,.28f)};var vertices=new Vector3[profile.Length*(count+1)];var uv=new Vector2[vertices.Length];var tint=new Color[vertices.Length];var triangles=new int[(profile.Length-1)*count*6];
  for(int row=0;row<profile.Length;row++)for(int i=0;i<=count;i++){int v=row*(count+1)+i;float a=i*Mathf.PI*2/count;var d=new Vector2(Mathf.Cos(a),Mathf.Sin(a));vertices[v]=new Vector3(d.x*profile[row].x,d.y*profile[row].x,profile[row].y);uv[v]=(Vector2)vertices[v]*.35f+Vector2.one*.5f;float light=row<2?.94f:row==2?.91f:row==3?.64f:row==4?.39f:.46f;if(row>=2)light+=Vector2.Dot(d,new Vector2(-.6f,.8f))*.16f;tint[v]=new Color(light,light,light,1);
   if(row==profile.Length-1||i==count)continue;int n=(row*count+i)*6;triangles[n]=v;triangles[n+1]=v+1;triangles[n+2]=v+count+2;triangles[n+3]=v;triangles[n+4]=v+count+2;triangles[n+5]=v+count+1;
  }var mesh=new Mesh{name="Shared beveled stone disk",vertices=vertices,uv=uv,colors=tint,triangles=triangles};mesh.RecalculateNormals();mesh.RecalculateBounds();return mesh;
 }
 public void Dispose(){if(Material!=null)UnityEngine.Object.Destroy(Material);if(quad!=null)UnityEngine.Object.Destroy(quad);if(disc!=null)UnityEngine.Object.Destroy(disc);if(ring!=null)UnityEngine.Object.Destroy(ring);if(stone!=null)UnityEngine.Object.Destroy(stone);if(stoneMaterial!=null)UnityEngine.Object.Destroy(stoneMaterial);}
}
}
