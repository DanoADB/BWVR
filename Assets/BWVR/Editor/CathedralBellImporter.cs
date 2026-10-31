using System.Linq;
using UnityEditor;
using UnityEngine;
namespace BWVR.Editor {
public static class CathedralBellImporter {
 [MenuItem("BWVR/Art/Import cathedral gathering bell")]
 public static void Import(){const string source="Assets/ThirdParty/Quaternius/CathedralBell/Bell.fbx",output="Assets/BWVR/Cathedrals/Resources/Cathedrals/";AssetDatabase.Refresh();var importer=(ModelImporter)AssetImporter.GetAtPath(source);importer.importAnimation=false;importer.addCollider=false;importer.SaveAndReimport();var original=AssetDatabase.LoadAssetAtPath<GameObject>(source);var root=new GameObject("GatheringBell");
  try{var model=Object.Instantiate(original,root.transform);foreach(var renderer in model.GetComponentsInChildren<Renderer>()){var slots=renderer.sharedMaterials;for(int i=0;i<slots.Length;i++){string name=slots[i].name.Replace(" ","-");string path=output+"Bell-"+name+".mat";var material=AssetDatabase.LoadAssetAtPath<Material>(path);if(material==null){material=new Material(Shader.Find("BWVR/GameSurface"));AssetDatabase.CreateAsset(material,path);}material.color=slots[i].HasProperty("_Color")?slots[i].color:Color.white;material.SetFloat("_Cutoff",0);EditorUtility.SetDirty(material);slots[i]=material;}renderer.sharedMaterials=slots;}var bounds=SourcedItemArt.LocalBounds(model);model.transform.localScale=Vector3.one/bounds.size.y;model.transform.localPosition=-new Vector3(bounds.center.x,bounds.min.y,bounds.center.z)/bounds.size.y;PrefabUtility.SaveAsPrefabAsset(root,output+"GatheringBell.prefab");var audio=(AudioImporter)AssetImporter.GetAtPath("Assets/BWVR/Resources/IslandAudio/bell.wav");audio.forceToMono=true;audio.SaveAndReimport();AssetDatabase.SaveAssets();Debug.Log("BWVR_BELL_IMPORTED: "+model.GetComponentsInChildren<MeshFilter>().Sum(m=>m.sharedMesh.triangles.Length/3)+" triangles");}finally{Object.DestroyImmediate(root);}
 }
}
}
