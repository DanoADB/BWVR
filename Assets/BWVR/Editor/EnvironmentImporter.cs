using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace BWVR.Editor {
public static class EnvironmentImporter {
 const string Root="Assets/ThirdParty/Quaternius", Output="Assets/BWVR/Environment";
 [MenuItem("BWVR/Import free environment artwork")]
 public static void Import(){
  Directory.CreateDirectory(Output+"/Materials");Directory.CreateDirectory(Output+"/Prefabs");Directory.CreateDirectory(Output+"/Resources");AssetDatabase.Refresh();
  var prefabs=new List<GameObject>();
  foreach(string pack in new[]{"Nature","Village"}){
   var materials=new Dictionary<string,Material>();
   foreach(var line in File.ReadAllLines(Root+"/"+pack+"/material-map.txt")){
    var fields=line.Split('|');var path=Output+"/Materials/"+pack+"_"+fields[1]+".mat";
    var mat=AssetDatabase.LoadAssetAtPath<Material>(path);
    if(mat==null){mat=new Material(Shader.Find("BWVR/GameSurface"));AssetDatabase.CreateAsset(mat,path);}
    var texPath=Root+"/"+pack+"/Textures/"+fields[2];
    var ti=AssetImporter.GetAtPath(texPath) as TextureImporter;
    if(ti!=null&&ti.maxTextureSize!=1024){ti.maxTextureSize=1024;ti.mipmapEnabled=true;ti.SaveAndReimport();}
    mat.mainTexture=AssetDatabase.LoadAssetAtPath<Texture2D>(texPath);if(mat.mainTexture==null)throw new Exception("Missing environment texture: "+texPath);
    mat.SetFloat("_Cutoff",pack=="Nature"?.2f:0);mat.enableInstancing=true;EditorUtility.SetDirty(mat);materials[fields[1]]=mat;
   }
   if(pack=="Village"){
    var glassPath=Output+"/Materials/WindowGlass.mat";var glass=AssetDatabase.LoadAssetAtPath<Material>(glassPath);
    if(glass==null){glass=new Material(Shader.Find("BWVR/GameSurface"));glass.color=new Color(.15f,.27f,.29f);AssetDatabase.CreateAsset(glass,glassPath);}materials["MI_WindowGlass"]=glass;
   }
   foreach(var file in Directory.GetFiles(Root+"/"+pack+"/Models","*.fbx")){
    var path=file.Replace('\\','/');var model=AssetDatabase.LoadAssetAtPath<GameObject>(path);if(model==null)throw new Exception("Missing model "+path);
    var root=new GameObject(Path.GetFileNameWithoutExtension(path));var geo=UnityEngine.Object.Instantiate(model,root.transform);
    foreach(var renderer in geo.GetComponentsInChildren<Renderer>()){
     var slots=renderer.sharedMaterials;for(int i=0;i<slots.Length;i++){var name=slots[i].name.Replace(" (Instance)","");if(!materials.TryGetValue(name,out slots[i]))throw new Exception("Unmapped material: "+name);}renderer.sharedMaterials=slots;
    }
    var renderers=geo.GetComponentsInChildren<Renderer>();var bounds=renderers[0].bounds;foreach(var r in renderers)bounds.Encapsulate(r.bounds);
    // Unit bounding box permits assembling modular buildings without depending on FBX pivots.
    var size=bounds.size;var fitRoot=new GameObject("Normalized geometry").transform;fitRoot.SetParent(root.transform,false);geo.transform.SetParent(fitRoot,true);geo.transform.position-=new Vector3(bounds.center.x,bounds.min.y,bounds.center.z);
    fitRoot.localScale=new Vector3(1/Mathf.Max(size.x,.001f),1/Mathf.Max(size.y,.001f),1/Mathf.Max(size.z,.001f));
    prefabs.Add(PrefabUtility.SaveAsPrefabAsset(root,Output+"/Prefabs/"+root.name+".prefab"));UnityEngine.Object.DestroyImmediate(root);
   }
  }
  var catalog=AssetDatabase.LoadAssetAtPath<EnvironmentArt>(Output+"/Resources/EnvironmentArt.asset");if(catalog==null){catalog=ScriptableObject.CreateInstance<EnvironmentArt>();AssetDatabase.CreateAsset(catalog,Output+"/Resources/EnvironmentArt.asset");}
  catalog.models=prefabs.ToArray();EditorUtility.SetDirty(catalog);AssetDatabase.SaveAssets();Debug.Log("BWVR_ENVIRONMENT_IMPORTED: "+prefabs.Count+" CC0 models");
 }
}
}
