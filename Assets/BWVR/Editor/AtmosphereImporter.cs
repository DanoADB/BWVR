using System;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace BWVR.Editor {
public static class AtmosphereImporter {
 [MenuItem("BWVR/Import ocean material")]
 public static void ImportOcean(){
  const string folder="Assets/BWVR/Atmosphere/Resources";Directory.CreateDirectory(folder);AssetDatabase.Refresh();
  var path=folder+"/IslandOcean.mat";var m=AssetDatabase.LoadAssetAtPath<Material>(path);
  if(m==null){m=new Material(Shader.Find("BWVR/IslandWater"));AssetDatabase.CreateAsset(m,path);}
  EditorUtility.SetDirty(m);AssetDatabase.SaveAssets();
 }
 [MenuItem("BWVR/Import CC0 atmosphere artwork")]
 public static void Import(){
  const string output="Assets/BWVR/Atmosphere/Resources";
  Directory.CreateDirectory(output);AssetDatabase.Refresh();
  var model=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ThirdParty/Quaternius/Clouds/Cloud.obj");
  if(model==null)throw new Exception("Quaternius cloud missing");
  var mesh=model.GetComponentInChildren<MeshFilter>().sharedMesh;
  const string texturePath="Assets/ThirdParty/Kenney/Smoke/whitePuff00.png";
  var importer=(TextureImporter)AssetImporter.GetAtPath(texturePath);
  importer.maxTextureSize=256;importer.mipmapEnabled=true;importer.alphaIsTransparency=true;importer.wrapMode=TextureWrapMode.Clamp;importer.SaveAndReimport();
  Material Material(string name,string shader){
   var path=output+"/"+name+".mat";var m=AssetDatabase.LoadAssetAtPath<Material>(path);
   if(m==null){m=new Material(Shader.Find(shader));AssetDatabase.CreateAsset(m,path);}
   return m;
  }
  var cloud=Material("Cloud","Standard");cloud.color=new Color(.88f,.92f,.91f);cloud.SetFloat("_Glossiness",0);cloud.enableInstancing=true;EditorUtility.SetDirty(cloud);
  var fog=Material("Fog","BWVR/LocalMist");fog.mainTexture=AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);fog.SetColor("_Color",new Color(.72f,.84f,.81f,.14f));fog.enableInstancing=true;EditorUtility.SetDirty(fog);
  var data=AssetDatabase.LoadAssetAtPath<IslandAtmosphereArt>(output+"/IslandAtmosphere.asset");
  if(data==null){data=ScriptableObject.CreateInstance<IslandAtmosphereArt>();AssetDatabase.CreateAsset(data,output+"/IslandAtmosphere.asset");}
  data.cloud=mesh;data.cloudMaterial=cloud;data.fogMaterial=fog;EditorUtility.SetDirty(data);AssetDatabase.SaveAssets();
  Debug.Log("BWVR_ATMOSPHERE_IMPORTED: Quaternius CC0 cloud / Kenney CC0 fog");
 }
}
}
