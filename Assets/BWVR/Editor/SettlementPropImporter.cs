using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace BWVR.Editor {
public static class SettlementPropImporter {
 [MenuItem("BWVR/Art/Rebuild Kenney settlement props")]
 public static void Build(){
  const string source="Assets/ThirdParty/KenneySurvival/";
  const string destination="Assets/BWVR/Resources/SettlementProps/";
  var texture=AssetDatabase.LoadAssetAtPath<Texture2D>(source+"colormap.png");
  if(texture==null)throw new InvalidOperationException("Kenney palette missing");
  var ti=(TextureImporter)AssetImporter.GetAtPath(source+"colormap.png");ti.filterMode=FilterMode.Point;ti.mipmapEnabled=true;ti.textureCompression=TextureImporterCompression.Uncompressed;ti.SaveAndReimport();
  var material=AssetDatabase.LoadAssetAtPath<Material>(destination+"Palette.mat");
  if(material==null){material=new Material(Shader.Find("BWVR/GameSurface"));AssetDatabase.CreateAsset(material,destination+"Palette.mat");}
  material.mainTexture=texture;material.color=Color.white;material.SetFloat("_Cutoff",0);material.enableInstancing=true;EditorUtility.SetDirty(material);
  foreach(var path in System.IO.Directory.GetFiles(source+"Models","*.fbx")){
   var importer=(ModelImporter)AssetImporter.GetAtPath(path);importer.addCollider=false;importer.importAnimation=false;importer.SaveAndReimport();
   var model=AssetDatabase.LoadAssetAtPath<GameObject>(path);var root=new GameObject(System.IO.Path.GetFileNameWithoutExtension(path));
   try{var child=UnityEngine.Object.Instantiate(model,root.transform);child.transform.localPosition=Vector3.zero;child.transform.localRotation=Quaternion.identity;child.transform.localScale=Vector3.one;
    var renderers=child.GetComponentsInChildren<Renderer>();if(renderers.Length==0)throw new InvalidOperationException("Empty prop: "+path);
    var bounds=renderers[0].bounds;foreach(var renderer in renderers){bounds.Encapsulate(renderer.bounds);renderer.sharedMaterials=Enumerable.Repeat(material,renderer.sharedMaterials.Length).ToArray();}
    if(bounds.size.y<=0)throw new InvalidOperationException("Invalid prop bounds: "+path);
    child.transform.localScale=Vector3.one/bounds.size.y;child.transform.localPosition=-new Vector3(bounds.center.x,bounds.min.y,bounds.center.z)/bounds.size.y;
    foreach(var collider in root.GetComponentsInChildren<Collider>())UnityEngine.Object.DestroyImmediate(collider);
    PrefabUtility.SaveAsPrefabAsset(root,destination+root.name+".prefab");
   }finally{UnityEngine.Object.DestroyImmediate(root);}
  }
  AssetDatabase.SaveAssets();Debug.Log("BWVR_SETTLEMENT_PROPS_IMPORTED: normalized, shared palette, collider-free");
 }
}
}
