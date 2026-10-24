using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor;
using UnityEngine;
namespace BWVR.Editor {
public static class BuildingImporter {
 const string Source="Assets/ThirdParty/Quaternius/MedievalBuildings",Output="Assets/BWVR/Buildings";
 [MenuItem("BWVR/Import varied village buildings")]
 public static void Import(){
  Directory.CreateDirectory(Output+"/Materials");Directory.CreateDirectory(Output+"/Prefabs");Directory.CreateDirectory(Output+"/Resources");AssetDatabase.Refresh();
  var prefabs=new List<GameObject>();
  foreach(var key in new[]{"House_A","House_B","House_C","Inn","Blacksmith"}){
   var mats=new Dictionary<string,Material>();
   foreach(var line in File.ReadAllLines(Source+"/"+key+"-colors.txt")){
    var fields=line.Split('|');var path=Output+"/Materials/"+key+"_"+fields[0]+".mat";
    var m=AssetDatabase.LoadAssetAtPath<Material>(path);if(m==null){m=new Material(Shader.Find("BWVR/GameSurface"));AssetDatabase.CreateAsset(m,path);}
    float Value(int i)=>float.Parse(fields[i],CultureInfo.InvariantCulture);
    m.color=new Color(Value(1),Value(2),Value(3),Value(4)).gamma;if(key=="House_C"&&fields[0]=="RoofTiles")m.color=new Color(.53f,.22f,.12f);m.SetFloat("_Cutoff",0);m.enableInstancing=true;EditorUtility.SetDirty(m);mats[fields[0]]=m;
   }
   var model=AssetDatabase.LoadAssetAtPath<GameObject>(Source+"/"+key+".obj");if(model==null)throw new Exception("Missing building "+key);
   var root=new GameObject(key);var geometry=UnityEngine.Object.Instantiate(model,root.transform);
   var renderers=geometry.GetComponentsInChildren<Renderer>();var bounds=renderers[0].bounds;
   foreach(var r in renderers){
    bounds.Encapsulate(r.bounds);var slots=r.sharedMaterials;
    for(int i=0;i<slots.Length;i++){var name=slots[i].name.Replace(" (Instance)","");if(!mats.TryGetValue(name,out slots[i]))throw new Exception("Unmapped building color "+name);}
    r.sharedMaterials=slots;
   }
   // One uniform fit keeps the authored door, windows and roof proportions intact.
   float targetHeight=.195f*(key=="House_A"?1.2f:key=="House_B"||key=="House_C"?2.5f:2.8f);
   float fit=targetHeight/bounds.size.y;geometry.transform.localScale*=fit;
   geometry.transform.localPosition-=new Vector3(bounds.center.x,bounds.min.y,bounds.center.z)*fit;
   var box=root.AddComponent<BoxCollider>();box.center=new Vector3(0,targetHeight/2,0);box.size=new Vector3(bounds.size.x*fit,targetHeight,bounds.size.z*fit);
   prefabs.Add(PrefabUtility.SaveAsPrefabAsset(root,Output+"/Prefabs/"+key+".prefab"));UnityEngine.Object.DestroyImmediate(root);
  }
  var catalog=AssetDatabase.LoadAssetAtPath<EnvironmentArt>(Output+"/VillageBuildings.asset");if(catalog==null){catalog=ScriptableObject.CreateInstance<EnvironmentArt>();AssetDatabase.CreateAsset(catalog,Output+"/VillageBuildings.asset");}
  catalog.models=prefabs.ToArray();EditorUtility.SetDirty(catalog);AssetDatabase.SaveAssets();Debug.Log("BWVR_BUILDINGS_IMPORTED: three complete house styles, inn and blacksmith; uniform proportions");
 }
}
}
