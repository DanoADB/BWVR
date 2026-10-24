using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
namespace BWVR.Editor {
public static class CharacterImporter {
 const string Root="Assets/ThirdParty/Quaternius/Characters",Output="Assets/BWVR/Characters";
 const string Library="Assets/ThirdParty/Quaternius/Animations/UAL1_Standard.fbx";
 static void Humanoid(string path){var importer=(ModelImporter)AssetImporter.GetAtPath(path);importer.animationType=ModelImporterAnimationType.Human;
  if(path.EndsWith("Female_Peasant.fbx")){var description=importer.humanDescription;var bones=description.human.Where(b=>b.humanName!="Head").ToList();bones.Add(new HumanBone{boneName="Head",humanName="Head",limit=new HumanLimit{useDefaultValues=true}});description.human=bones.ToArray();importer.humanDescription=description;}
  if(path==Library){var clips=importer.defaultClipAnimations;foreach(var c in clips)c.loopTime=c.name.Contains("Loop");importer.clipAnimations=clips;}
  importer.SaveAndReimport();var avatar=AssetDatabase.LoadAllAssetsAtPath(path).OfType<Avatar>().FirstOrDefault();if(avatar==null||!avatar.isValid||!avatar.isHuman)throw new Exception("Invalid humanoid avatar: "+path);
 }
 static RuntimeAnimatorController Controller(){
  var path=Output+"/IslandHumanoid.controller";
  // FBX imports also expose static __preview__ clips. They must never drive a player.
  var clips=AssetDatabase.LoadAllAssetsAtPath(Library).OfType<AnimationClip>().Where(c=>!c.name.StartsWith("__")).ToArray();AnimationClip Clip(string name)=>clips.First(c=>c.name.EndsWith("|"+name));
  var existing=AssetDatabase.LoadAssetAtPath<AnimatorController>(path);
  if(existing!=null){
   var states=existing.layers[0].stateMachine.states.Select(s=>s.state).ToArray();var tree=(BlendTree)states.First(s=>s.name=="Idle and walk").motion;
   tree.children=new[]{new ChildMotion{motion=Clip("Idle_Loop"),threshold=0,timeScale=1},new ChildMotion{motion=Clip("Walk_Loop"),threshold=1,timeScale=1}};
   states.First(s=>s.name=="Warmth reaction").motion=Clip("Interact");EditorUtility.SetDirty(tree);EditorUtility.SetDirty(existing);return existing;
  }
  var controller=AnimatorController.CreateAnimatorControllerAtPath(path);controller.AddParameter("Speed",AnimatorControllerParameterType.Float);controller.AddParameter("Warmth",AnimatorControllerParameterType.Trigger);
  var machine=controller.layers[0].stateMachine;var move=machine.AddState("Idle and walk");var blend=new BlendTree{name="UAL idle walk",blendParameter="Speed",useAutomaticThresholds=false};AssetDatabase.AddObjectToAsset(blend,controller);blend.AddChild(Clip("Idle_Loop"),0);blend.AddChild(Clip("Walk_Loop"),1);move.motion=blend;machine.defaultState=move;
  var react=machine.AddState("Warmth reaction");react.motion=Clip("Interact");var enter=machine.AddAnyStateTransition(react);enter.AddCondition(AnimatorConditionMode.If,0,"Warmth");enter.duration=.12f;enter.canTransitionToSelf=false;var exit=react.AddTransition(move);exit.hasExitTime=true;exit.exitTime=.9f;exit.duration=.12f;return controller;
 }
 static Material Surface(string key,string texture,Color tint){
  var path=Output+"/"+key+".mat";var material=AssetDatabase.LoadAssetAtPath<Material>(path);if(material==null){material=new Material(Shader.Find("BWVR/GameSurface"));AssetDatabase.CreateAsset(material,path);}material.color=tint;
  if(texture!=null){material.mainTexture=AssetDatabase.LoadAssetAtPath<Texture2D>(texture);if(material.mainTexture==null)throw new Exception("Missing character texture "+texture);var importer=(TextureImporter)AssetImporter.GetAtPath(texture);if(importer.maxTextureSize!=1024){importer.maxTextureSize=1024;importer.SaveAndReimport();}}
  material.SetFloat("_Cutoff",.2f);material.enableInstancing=true;EditorUtility.SetDirty(material);return material;
 }
 static void Configure(GameObject model,RuntimeAnimatorController controller){var animator=model.GetComponent<Animator>();animator.runtimeAnimatorController=controller;animator.applyRootMotion=false;animator.cullingMode=AnimatorCullingMode.AlwaysAnimate;}
 static void Save(GameObject root,string path){
  var renderers=root.GetComponentsInChildren<Renderer>();var bounds=renderers[0].bounds;foreach(var r in renderers)bounds.Encapsulate(r.bounds);
  var content=new GameObject("Unit-height artwork").transform;content.SetParent(root.transform,false);foreach(var child in root.transform.Cast<Transform>().Where(t=>t!=content).ToArray())child.SetParent(content,true);
  foreach(Transform child in content)child.position-=new Vector3(bounds.center.x,bounds.min.y,bounds.center.z);content.localScale=Vector3.one/Mathf.Max(bounds.size.y,.01f);
  root.AddComponent<HumanoidMotion>();var collider=root.AddComponent<BoxCollider>();collider.center=new Vector3(0,.5f,0);collider.size=new Vector3(.45f,1,.35f);
  PrefabUtility.SaveAsPrefabAsset(root,path);UnityEngine.Object.DestroyImmediate(root);
 }
 [MenuItem("BWVR/Import Quaternius villagers and creature")]
 public static void Import(){
  Directory.CreateDirectory(Output);Directory.CreateDirectory(Root+"/Resources/Villagers");AssetDatabase.Refresh();Humanoid(Library);var controller=Controller();
  for(int index=0;index<3;index++){
   string sex=index==1?"Female":"Male",outfit=index==2?"Male_Ranger":sex+"_Peasant";var modelPath=Root+"/Models/"+outfit+".fbx";var basePath=Root+"/Models/Superhero_"+sex+"_FullBody.fbx";Humanoid(modelPath);Humanoid(basePath);
   var root=new GameObject("Villager"+index);var model=UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(modelPath),root.transform);Configure(model,controller);
   string outfitTexture=index==2?"T_Ranger_BaseColor.png":index==1?"T_Peasant_2_BaseColor.png":"T_Peasant_BaseColor.png";
   var cloth=Surface(outfit,Root+"/Textures/"+outfitTexture,Color.white);var skin=Surface(sex+"Skin",Root+"/Textures/"+(sex=="Male"?"T_Superhero_Male_Ligh.png":"T_Superhero_Female_Light_BaseColor.png"),Color.white);var eyes=Surface("Eyes",Root+"/Textures/T_Eye_Brown.png",Color.white);var hair=Surface("Hair",null,new Color(.14f,.075f,.035f));
   foreach(var r in model.GetComponentsInChildren<Renderer>())r.sharedMaterials=r.sharedMaterials.Select(m=>m.name.Contains("Regular")?skin:cloth).ToArray();
   // Clothing replaces the body. Extract only the base character head to avoid hidden body clipping.
   var original=UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(basePath));var originalAnimator=original.GetComponent<Animator>();var neck=originalAnimator.GetBoneTransform(HumanBodyBones.Neck);var head=model.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Head);
   foreach(var r in original.GetComponentsInChildren<SkinnedMeshRenderer>()){
    var baked=new Mesh();r.BakeMesh(baked);var vertices=baked.vertices;var triangles=baked.triangles;var kept=new System.Collections.Generic.List<int>();
    for(int i=0;i<triangles.Length;i+=3)if(Enumerable.Range(0,3).All(j=>r.transform.TransformPoint(vertices[triangles[i+j]]).y>=neck.position.y-.015f)){kept.Add(triangles[i]);kept.Add(triangles[i+1]);kept.Add(triangles[i+2]);}
    if(kept.Count==0){UnityEngine.Object.DestroyImmediate(baked);continue;}var sourceHead=originalAnimator.GetBoneTransform(HumanBodyBones.Head);for(int i=0;i<vertices.Length;i++)vertices[i]=sourceHead.InverseTransformPoint(r.transform.TransformPoint(vertices[i]));baked.vertices=vertices;baked.triangles=kept.ToArray();baked.RecalculateBounds();
    var meshPath=Output+"/"+sex+"_"+r.name+"Head.asset";var saved=AssetDatabase.LoadAssetAtPath<Mesh>(meshPath);if(saved==null)AssetDatabase.CreateAsset(baked,meshPath);else{EditorUtility.CopySerialized(baked,saved);UnityEngine.Object.DestroyImmediate(baked);baked=saved;}
    var part=new GameObject("Base character "+r.name);part.transform.SetParent(head,false);part.AddComponent<MeshFilter>().sharedMesh=baked;part.AddComponent<MeshRenderer>().sharedMaterial=r.name.Contains("Eyes")?eyes:r.name.Contains("Eyebrow")?hair:skin;
   }
   UnityEngine.Object.DestroyImmediate(original);Save(root,Root+"/Resources/Villagers/Villager"+index+".prefab");
  }
  const string creaturePath="Assets/LocalLicensedArt/Models/Puglin.fbx";
  if(File.Exists(creaturePath)){
   Humanoid(creaturePath);Directory.CreateDirectory("Assets/LocalLicensedArt/Resources/BWVRGod");AssetDatabase.Refresh();var root=new GameObject("Puglin");var model=UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(creaturePath),root.transform);Configure(model,controller);
   var path="Assets/LocalLicensedArt/Puglin.mat";var material=AssetDatabase.LoadAssetAtPath<Material>(path);if(material==null){material=new Material(Shader.Find("BWVR/GameSurface"));AssetDatabase.CreateAsset(material,path);}material.mainTexture=AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/LocalLicensedArt/Textures/T_Puglin_BaseColor_2.png");if(material.mainTexture==null)throw new Exception("Missing Puglin texture");
   foreach(var r in model.GetComponentsInChildren<Renderer>()){if(r.name.Contains("Stick")){UnityEngine.Object.DestroyImmediate(r.gameObject);continue;}r.sharedMaterial=material;}Save(root,"Assets/LocalLicensedArt/Resources/BWVRGod/Puglin.prefab");
  }
  AssetDatabase.SaveAssets();Debug.Log("BWVR_CHARACTERS_IMPORTED: three clothed villagers, UAL humanoid animation, optional local Puglin");
 }
}
}
