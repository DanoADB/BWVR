using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace BWVR.Editor {
public static class CathedralImporter {
 const string Source="Assets/ThirdParty/SigilsVault/Cathedrals",Output="Assets/BWVR/Cathedrals";
 [MenuItem("BWVR/Import cathedral stages")]
 public static void Import(){
  Directory.CreateDirectory(Output+"/Materials");Directory.CreateDirectory(Output+"/Meshes");Directory.CreateDirectory(Output+"/Resources/Cathedrals");AssetDatabase.Refresh();
  foreach(var name in new[]{"trim_sheet_512x512","window_diffuse_512x512","door_diffuse_512x512"}){string path=Source+"/"+name+".png";var ti=(TextureImporter)AssetImporter.GetAtPath(path);ti.mipmapEnabled=true;ti.maxTextureSize=512;ti.textureCompression=TextureImporterCompression.Compressed;ti.SaveAndReimport();}
  var mats=new Material[3];for(int i=0;i<3;i++){string path=Output+"/Materials/"+(i==0?"Stone":i==1?"Windows":"Doors")+".mat";mats[i]=AssetDatabase.LoadAssetAtPath<Material>(path);if(mats[i]==null){mats[i]=new Material(Shader.Find("BWVR/GameSurface"));AssetDatabase.CreateAsset(mats[i],path);}mats[i].mainTexture=AssetDatabase.LoadAssetAtPath<Texture2D>(Source+"/"+(i==0?"trim_sheet_512x512":i==1?"window_diffuse_512x512":"door_diffuse_512x512")+".png");mats[i].color=Color.white;mats[i].SetFloat("_Cutoff",0);mats[i].enableInstancing=true;EditorUtility.SetDirty(mats[i]);}
  foreach(var key in new[]{"Shrine","Chapel","VillageChurch","GothicChurch","GrandCathedral"}){
   var model=AssetDatabase.LoadAssetAtPath<GameObject>(Source+"/"+key+".fbx");if(model==null)throw new Exception("Missing cathedral "+key);
   var temp=UnityEngine.Object.Instantiate(model);temp.transform.rotation=Quaternion.Euler(0,180,0)*temp.transform.rotation;var meshFilters=temp.GetComponentsInChildren<MeshFilter>();var root=new GameObject(key);
   for(int group=0;group<3;group++){
    var combines=new List<CombineInstance>();foreach(var f in meshFilters){var slots=f.GetComponent<Renderer>().sharedMaterials;for(int s=0;s<f.sharedMesh.subMeshCount;s++)if((slots[s].name.Contains("window")?1:slots[s].name.Contains("door")?2:0)==group)combines.Add(new CombineInstance{mesh=f.sharedMesh,subMeshIndex=s,transform=f.transform.localToWorldMatrix});}
    if(combines.Count==0)continue;var mesh=new Mesh{name=key+(group==0?"Stone":group==1?"Windows":"Doors")};mesh.indexFormat=UnityEngine.Rendering.IndexFormat.UInt32;mesh.CombineMeshes(combines.ToArray(),true,true);mesh.RecalculateBounds();string path=Output+"/Meshes/"+mesh.name+".asset";var old=AssetDatabase.LoadAssetAtPath<Mesh>(path);if(old!=null){old.Clear();old.indexFormat=mesh.indexFormat;old.vertices=mesh.vertices;old.normals=mesh.normals;old.uv=mesh.uv;old.tangents=mesh.tangents;old.triangles=mesh.triangles;old.RecalculateBounds();EditorUtility.SetDirty(old);UnityEngine.Object.DestroyImmediate(mesh);mesh=old;}else AssetDatabase.CreateAsset(mesh,path);
    var go=new GameObject(group==0?"Stone and roof":group==1?"Stained windows":"Timber doors");go.transform.SetParent(root.transform,false);go.AddComponent<MeshFilter>().sharedMesh=mesh;go.AddComponent<MeshRenderer>().sharedMaterial=mats[group];
   }
   UnityEngine.Object.DestroyImmediate(temp);var rs=root.GetComponentsInChildren<Renderer>();var bounds=rs[0].bounds;foreach(var r in rs)bounds.Encapsulate(r.bounds);
   // Common authored scale preserves door size across upgrades; front edge is the shared anchor.
   foreach(var child in root.GetComponentsInChildren<MeshFilter>()){child.transform.localScale=Vector3.one*.6f;child.transform.localPosition=-new Vector3(bounds.center.x,bounds.min.y,bounds.min.z)*.6f;}
   var box=root.AddComponent<BoxCollider>();box.size=bounds.size*.6f;box.center=new Vector3(0,box.size.y/2,box.size.z/2);
   PrefabUtility.SaveAsPrefabAsset(root,Output+"/Resources/Cathedrals/"+key+".prefab");Debug.Log("BWVR_CATHEDRAL_ART: "+key+" "+box.size+" "+root.GetComponentsInChildren<MeshFilter>().Sum(f=>f.sharedMesh.triangles.Length/3)+" tris");UnityEngine.Object.DestroyImmediate(root);
  }
  AssetDatabase.SaveAssets();Debug.Log("BWVR_CATHEDRALS_IMPORTED");
 }
}
}

