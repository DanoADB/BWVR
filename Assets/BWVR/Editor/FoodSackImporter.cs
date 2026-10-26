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
  var cloth=Surface("SackCloth",new Color(.230634f,.181047f,.103077f).gamma);var inside=Surface("SackInside",new Color(.213407f,.09224f,.03175f).gamma);var grain=Surface("SackGrain",new Color(.9f,.68f,.23f));
  var root=new GameObject("food-sack");try{var model=UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(path),root.transform);var renderers=model.GetComponentsInChildren<Renderer>();if(renderers.Length==0)throw new Exception("Sack mesh missing");var b=renderers[0].bounds;foreach(var r in renderers){b.Encapsulate(r.bounds);r.sharedMaterials=r.sharedMaterials.Select(m=>m!=null&&m.name.Contains("Inside")?inside:cloth).ToArray();}model.transform.localScale/=b.size.y;model.transform.localPosition=-new Vector3(b.center.x,b.min.y,b.center.z)/b.size.y;
   var fill=GameObject.CreatePrimitive(PrimitiveType.Sphere);fill.name="Visible grain in sack";fill.transform.SetParent(root.transform,false);fill.transform.localPosition=new Vector3(0,.78f,0);fill.transform.localScale=new Vector3(.48f,.16f,.4f);fill.GetComponent<Renderer>().sharedMaterial=grain;UnityEngine.Object.DestroyImmediate(fill.GetComponent<Collider>());
   PrefabUtility.SaveAsPrefabAsset(root,folder+"food-sack.prefab");AssetDatabase.SaveAssets();
  }finally{UnityEngine.Object.DestroyImmediate(root);}
 }
}
}
