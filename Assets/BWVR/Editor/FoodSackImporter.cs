using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace BWVR.Editor {
public static class FoodSackImporter {
 [MenuItem("BWVR/Art/Rebuild food sack")]
 public static void Build(){
  const string path="Assets/ThirdParty/QuaterniusFoodSack/BagOpen.fbx",folder="Assets/BWVR/Resources/SettlementProps/";
  var importer=(ModelImporter)AssetImporter.GetAtPath(path);importer.addCollider=false;importer.importAnimation=false;importer.SaveAndReimport();
  Material Surface(string name,Color color){var m=AssetDatabase.LoadAssetAtPath<Material>(folder+name+".mat");if(m==null){m=new Material(Shader.Find("BWVR/GameSurface"));AssetDatabase.CreateAsset(m,folder+name+".mat");}m.color=color;m.SetFloat("_Cutoff",0);m.enableInstancing=true;EditorUtility.SetDirty(m);return m;}
  var cloth=Surface("SackCloth",new Color(.230634f,.181047f,.103077f).gamma);var inside=Surface("SackInside",new Color(.213407f,.09224f,.03175f).gamma);
  var root=new GameObject("food-sack");try{var model=UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(path),root.transform);var renderers=model.GetComponentsInChildren<Renderer>();if(renderers.Length==0)throw new Exception("Sack mesh missing");var b=renderers[0].bounds;foreach(var r in renderers){b.Encapsulate(r.bounds);r.sharedMaterials=r.sharedMaterials.Select(m=>m!=null&&m.name.Contains("Inside")?inside:cloth).ToArray();}model.transform.localScale/=b.size.y;model.transform.localPosition=-new Vector3(b.center.x,b.min.y,b.center.z)/b.size.y;
   var fill=new GameObject("Sourced cereal grain pile");fill.transform.SetParent(root.transform,false);fill.transform.localPosition=Vector3.up*.8f;fill.AddComponent<MeshFilter>().sharedMesh=GrainPile();fill.AddComponent<MeshRenderer>().sharedMaterial=SourcedItemArt.Prefab("FoodProps/bowl-cereal").GetComponentInChildren<Renderer>().sharedMaterial;
   PrefabUtility.SaveAsPrefabAsset(root,folder+"food-sack.prefab");AssetDatabase.SaveAssets();
  }finally{UnityEngine.Object.DestroyImmediate(root);}
  }
 // Extract only the authored golden cereal piece; retain its original triangles,
 // normals and palette UVs. The sack insert contains nine copies, no primitive mesh.
 static Mesh GrainPile(){
  var source=SourcedItemArt.Prefab("FoodProps/bowl-cereal").GetComponentInChildren<MeshFilter>().sharedMesh;
  var positions=source.vertices;var normals=source.normals;var uv=source.uv;var triangles=source.triangles;
  var vertices=new System.Collections.Generic.List<Vector3>();var ns=new System.Collections.Generic.List<Vector3>();var tex=new System.Collections.Generic.List<Vector2>();var faces=new System.Collections.Generic.List<int>();var mapped=new System.Collections.Generic.Dictionary<int,int>();
  for(int i=0;i<triangles.Length;i+=3){if(!Enumerable.Range(0,3).All(k=>uv[triangles[i+k]].x>.70f&&uv[triangles[i+k]].x<.75f))continue;
   for(int k=0;k<3;k++){int index=triangles[i+k];if(!mapped.TryGetValue(index,out int target)){target=vertices.Count;mapped.Add(index,target);vertices.Add(positions[index]);ns.Add(normals[index]);tex.Add(uv[index]);}faces.Add(target);}}
  if(faces.Count==0)throw new Exception("Kenney source cereal region missing; review source UV layout");
  var grain=new Mesh{name="Authored Kenney cereal piece"};grain.SetVertices(vertices);grain.SetNormals(ns);grain.SetUVs(0,tex);grain.SetTriangles(faces,0);grain.RecalculateBounds();var bounds=grain.bounds;float span=Mathf.Max(bounds.size.x,bounds.size.y,bounds.size.z);grain.vertices=vertices.Select(v=>(v-bounds.center)/span).ToArray();grain.RecalculateBounds();
  var copies=new CombineInstance[9];for(int i=0;i<9;i++){float angle=i*Mathf.PI/4;var at=i==0?Vector3.up*.03f:new Vector3(Mathf.Cos(angle)*.14f,0,Mathf.Sin(angle)*.14f);copies[i]=new CombineInstance{mesh=grain,transform=Matrix4x4.TRS(at,Quaternion.Euler(i%2*12,i*43,0),Vector3.one*.14f)};}
  var pile=new Mesh{name="Sourced Kenney cereal pile"};pile.CombineMeshes(copies);UnityEngine.Object.DestroyImmediate(grain);
  const string path="Assets/ThirdParty/KenneyFood/Derived/SackGrainPile.asset";System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));AssetDatabase.Refresh();var saved=AssetDatabase.LoadAssetAtPath<Mesh>(path);if(saved==null){AssetDatabase.CreateAsset(pile,path);return pile;}EditorUtility.CopySerialized(pile,saved);UnityEngine.Object.DestroyImmediate(pile);EditorUtility.SetDirty(saved);return saved;
 }
}
}
