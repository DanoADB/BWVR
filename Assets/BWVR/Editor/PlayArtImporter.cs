using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace BWVR.Editor {
public static class PlayArtImporter {
 const string Output="Assets/BWVR/Resources/PlayArt/";
 [MenuItem("BWVR/Art/Rebuild sourced play artwork")]
 public static void Build(){
  Directory.CreateDirectory(Output+"Meshes");Directory.CreateDirectory(Output+"Materials");AssetDatabase.Refresh();
  foreach(var id in new[]{"ball","token","plate","stud","goal-frame","puzzle-frame","shutter","backstop","chime-body","chime-low","chime-high","pin","key","deck","block-brick","block-beam","block-slab","block-ramp","block-arch"}){
   string path=id=="ball"?"Assets/ThirdParty/KenneyPlay/Models/ball-blue.fbx":id=="token"?"Assets/ThirdParty/KenneyPlatformer/Models/jewel.fbx":id=="key"?"Assets/ThirdParty/KenneyPlatformer/Models/key.fbx":id=="deck"?"Assets/ThirdParty/KenneySurvival/Models/structure-floor.fbx":id=="block-brick"?"Assets/ThirdParty/Kenney/ConstructionBlocks/bevel-lq-brick-2x2.fbx":id=="block-beam"?"Assets/ThirdParty/Kenney/ConstructionBlocks/bevel-lq-brick-2x6.fbx":id=="block-slab"?"Assets/ThirdParty/Kenney/ConstructionBlocks/bevel-lq-plate-4x8.fbx":id=="block-ramp"?"Assets/ThirdParty/Kenney/ConstructionBlocks/bevel-lq-brick-slope-1x2.fbx":id=="plate"?"Assets/ThirdParty/KenneyPlay/Models/block-borders.fbx":id=="stud"?"Assets/ThirdParty/Kenney/ConstructionBlocks/bevel-lq-brick-1x1-round.fbx":id=="shutter"?"Assets/ThirdParty/Quaternius/Village/Models/Door_1_Flat.fbx":"Assets/ThirdParty/PlayDerivatives/Models/"+(id=="pin"?"bowling-pin":id)+".fbx";
   var importer=(ModelImporter)AssetImporter.GetAtPath(path);importer.addCollider=false;importer.importAnimation=false;importer.SaveAndReimport();
   var instance=UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(path));var root=new GameObject(id);
   try{
    var parts=new List<CombineInstance>();var materials=new List<Material>();
    foreach(var filter in instance.GetComponentsInChildren<MeshFilter>()){
     var renderer=filter.GetComponent<Renderer>();for(int sub=0;sub<filter.sharedMesh.subMeshCount;sub++){parts.Add(new CombineInstance{mesh=filter.sharedMesh,subMeshIndex=sub,transform=filter.transform.localToWorldMatrix});materials.Add(MaterialFor(id,renderer.sharedMaterials[Mathf.Min(sub,renderer.sharedMaterials.Length-1)]));}
    }
    var mesh=new Mesh{name=id+" — normalized sourced geometry"};mesh.CombineMeshes(parts.ToArray(),false,true);mesh.RecalculateBounds();if((id=="block-beam"||id=="block-slab"||id=="block-ramp")&&mesh.bounds.size.z>mesh.bounds.size.x){var rotation=Quaternion.Euler(0,90,0);mesh.vertices=mesh.vertices.Select(v=>rotation*v).ToArray();mesh.normals=mesh.normals.Select(n=>rotation*n).ToArray();mesh.RecalculateBounds();}var bounds=mesh.bounds;var size=bounds.size;
    if(Mathf.Min(size.x,size.y,size.z)<.000001f)throw new Exception("Degenerate source bounds: "+id);
    mesh.vertices=mesh.vertices.Select(v=>Vector3.Scale(v-bounds.center,new Vector3(1/size.x,1/size.y,1/size.z))).ToArray();mesh.normals=mesh.normals.Select(n=>Vector3.Scale(n,size).normalized).ToArray();mesh.RecalculateBounds();
    if(id=="block-ramp"&&mesh.vertices.Where(v=>v.y>.2f).Average(v=>v.x)<0){var turn=Quaternion.Euler(0,180,0);mesh.vertices=mesh.vertices.Select(v=>turn*v).ToArray();mesh.normals=mesh.normals.Select(n=>turn*n).ToArray();mesh.RecalculateBounds();}
    if(id=="ball"){var v=mesh.vertices;mesh.uv=v.Select(p=>new Vector2(Mathf.Atan2(p.z,p.x)/(Mathf.PI*2)+.5f,Mathf.Asin(p.normalized.y)/Mathf.PI+.5f)).ToArray();}
    string meshPath=Output+"Meshes/"+id+".asset";var saved=AssetDatabase.LoadAssetAtPath<Mesh>(meshPath);if(saved==null){AssetDatabase.CreateAsset(mesh,meshPath);saved=mesh;}else{EditorUtility.CopySerialized(mesh,saved);UnityEngine.Object.DestroyImmediate(mesh);EditorUtility.SetDirty(saved);}
    root.AddComponent<MeshFilter>().sharedMesh=saved;root.AddComponent<MeshRenderer>().sharedMaterials=materials.ToArray();var record=root.AddComponent<PlayArtModel>();record.id=id;record.source=path;record.creator=id.StartsWith("chime")?"Innerscene":id=="pin"?"Tomcat94":id.Contains("frame")||id=="shutter"||id=="block-arch"?"Quaternius":"Kenney";
    PrefabUtility.SaveAsPrefabAsset(root,Output+id+".prefab");
    // Persist colors/textures before the next source reimport reloads assets.
    AssetDatabase.SaveAssets();
   }finally{UnityEngine.Object.DestroyImmediate(instance);UnityEngine.Object.DestroyImmediate(root);}
  }
  AssetDatabase.SaveAssets();Debug.Log("BWVR_PLAY_ART_IMPORTED: CC0 sourced geometry; centered unit bounds; gameplay colliders remain separate");
 }
 static Material MaterialFor(string id,Material original){
  string name=original==null?"Missing":original.name.Replace(" (Instance)","");
  if(id.Contains("frame")||id=="shutter"||id=="block-arch"){name=System.Text.RegularExpressions.Regex.Replace(name,@"\.\d{3}$","");var existing=AssetDatabase.LoadAssetAtPath<Material>("Assets/BWVR/Environment/Materials/Village_"+name+".mat");if(existing!=null)return existing;}
  string path=Output+"Materials/"+(id=="backstop"||id=="deck"?"Survival":id=="stud"||id.StartsWith("block-")?"Brick":id=="key"||id=="token"?"Platformer":id=="ball"||id=="plate"?"Minigolf":id+"_"+name)+".mat";
  var material=AssetDatabase.LoadAssetAtPath<Material>(path);if(material==null){material=new Material(Shader.Find("BWVR/GameSurface"));AssetDatabase.CreateAsset(material,path);}
  string texture=id.StartsWith("chime")?"Assets/ThirdParty/PlayDerivatives/Textures/"+(id=="chime-body"?"music-walnut":"music-redwood")+".png":id=="backstop"||id=="deck"?"Assets/ThirdParty/KenneySurvival/colormap.png":id=="stud"||id.StartsWith("block-")?"Assets/ThirdParty/Kenney/ConstructionBlocks/colormap.png":id=="key"||id=="token"?"Assets/ThirdParty/KenneyPlatformer/colormap.png":id=="ball"||id=="plate"?"Assets/ThirdParty/KenneyPlay/colormap.png":null;
  material.mainTexture=texture==null?null:AssetDatabase.LoadAssetAtPath<Texture2D>(texture);material.color=texture!=null?Color.white:original!=null?original.color:Color.white;material.SetFloat("_Cutoff",0);material.enableInstancing=true;EditorUtility.SetDirty(material);return material;
 }
}
}
