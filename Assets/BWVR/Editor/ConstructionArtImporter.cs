using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace BWVR.Editor {
public static class ConstructionArtImporter {
 [MenuItem("BWVR/Import CC0 construction blocks")]
 public static void Import(){
  const string folder="Assets/BWVR/Construction";Directory.CreateDirectory(folder+"/Resources");Directory.CreateDirectory(folder+"/Prefabs");AssetDatabase.Refresh();
  var material=AssetDatabase.LoadAssetAtPath<Material>(folder+"/Timber.mat");if(material==null){material=new Material(Shader.Find("BWVR/GameSurface"));material.color=new Color(.5f,.3f,.12f);material.SetFloat("_Cutoff",0);AssetDatabase.CreateAsset(material,folder+"/Timber.mat");}
  var prefabs=new List<GameObject>();
  if(AssetDatabase.LoadAssetAtPath<Material>(folder+"/Resources/ConstructionPreview.mat")==null)AssetDatabase.CreateAsset(new Material(Shader.Find("Sprites/Default")),folder+"/Resources/ConstructionPreview.mat");
  for(int i=0;i<2;i++){
   string key=i==0?"Brick":"Beam";var source=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ThirdParty/Kenney/ConstructionBlocks/bevel-lq-brick-2x"+(i==0?2:6)+".fbx");
   var root=new GameObject(key);var geometry=Object.Instantiate(source,root.transform);var renderers=geometry.GetComponentsInChildren<Renderer>();var bounds=renderers[0].bounds;foreach(var r in renderers){bounds.Encapsulate(r.bounds);r.sharedMaterial=material;}
   if(i==1&&bounds.size.z>bounds.size.x){geometry.transform.localRotation=Quaternion.Euler(0,90,0);bounds=renderers[0].bounds;foreach(var r in renderers)bounds.Encapsulate(r.bounds);}
   float scale=(i==0?2:6)/bounds.size.x;geometry.transform.localScale*=scale;geometry.transform.localPosition-=new Vector3(bounds.center.x,bounds.min.y,bounds.center.z)*scale;geometry.transform.localPosition-=Vector3.up*.6f;
   var box=root.AddComponent<BoxCollider>();box.size=new Vector3(i==0?2:6,1.2f,2);box.center=Vector3.zero;
   prefabs.Add(PrefabUtility.SaveAsPrefabAsset(root,folder+"/Prefabs/"+key+".prefab"));Object.DestroyImmediate(root);
  }
  var catalog=AssetDatabase.LoadAssetAtPath<EnvironmentArt>(folder+"/Resources/ConstructionArt.asset");if(catalog==null){catalog=ScriptableObject.CreateInstance<EnvironmentArt>();AssetDatabase.CreateAsset(catalog,folder+"/Resources/ConstructionArt.asset");}catalog.models=prefabs.ToArray();EditorUtility.SetDirty(catalog);AssetDatabase.SaveAssets();
 }
}
}
