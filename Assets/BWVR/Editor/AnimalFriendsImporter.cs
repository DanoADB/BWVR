using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
namespace BWVR.Editor {
// Purchased source files and generated artwork stay in the ignored local art folder.
public static class AnimalFriendsImporter {
 public const string Source="Assets/LocalLicensedArt/AnimalFriends/Gorilla";
 public const string Prefab="Assets/LocalLicensedArt/Resources/BWVRGod/Gorilla.prefab";
 const string Output="Assets/LocalLicensedArt/AnimalFriends/Generated";
 static AnimationClip Clip(string file,string name)=>AssetDatabase.LoadAllAssetsAtPath(Source+"/3D/Animations/"+file+".FBX").OfType<AnimationClip>().First(c=>c.name==name);
 [MenuItem("BWVR/Import purchased Animal Friends gorilla")]
 public static void Import(){
  if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop Play mode before importing the gorilla.");
  var modelPath=Source+"/3D/Gorilla.FBX";
  if(!File.Exists(modelPath))throw new FileNotFoundException("Run tools/prepare-animal-friends.py with the purchased Unity package first.",modelPath);
  Directory.CreateDirectory(Output);Directory.CreateDirectory(Path.GetDirectoryName(Prefab));AssetDatabase.Refresh();
  var avatar=AssetDatabase.LoadAllAssetsAtPath(modelPath).OfType<Avatar>().FirstOrDefault(a=>a.isValid&&a.isHuman);
  if(avatar==null)throw new InvalidOperationException("Animal Friends gorilla has no valid humanoid avatar.");
  foreach(var file in new[]{"Gorilla Idle","Gorilla Movement In Place","Gorilla Misc"}){
   var importer=(ModelImporter)AssetImporter.GetAtPath(Source+"/3D/Animations/"+file+".FBX");
   importer.animationType=ModelImporterAnimationType.Human;importer.avatarSetup=ModelImporterAvatarSetup.CopyFromOther;importer.sourceAvatar=avatar;
   var clips=importer.clipAnimations.Length>0?importer.clipAnimations:importer.defaultClipAnimations;
   foreach(var c in clips){c.loopTime=c.name.Contains("Idle")||c.name.Contains("In Place")||c.name.Contains("Talk");c.lockRootRotation=true;c.lockRootPositionXZ=true;}
   importer.clipAnimations=clips;importer.SaveAndReimport();
  }
  const string baseController="Assets/BWVR/Characters/IslandHumanoid.controller";string controllerPath=Output+"/Gorilla.controller";
  if(AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath)==null&&!AssetDatabase.CopyAsset(baseController,controllerPath))throw new InvalidOperationException("Missing shared BWVR humanoid controller.");
  var controller=AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);var machine=controller.layers[0].stateMachine;
  var states=machine.states.Select(s=>s.state).ToArray();var move=states.First(s=>s.name=="Idle and walk");var tree=(BlendTree)move.motion;
  tree.children=new[]{new ChildMotion{motion=Clip("Gorilla Idle","Gorilla Idle"),threshold=0,timeScale=1},new ChildMotion{motion=Clip("Gorilla Movement In Place","Gorilla Walk In Place"),threshold=1,timeScale=1}};
  states.First(s=>s.name=="Warmth reaction").motion=Clip("Gorilla Misc","Gorilla Success");
  states.First(s=>s.name=="Request water").motion=Clip("Gorilla Misc","Gorilla Talk");
  if(!controller.parameters.Any(p=>p.name=="Unease"))controller.AddParameter("Unease",AnimatorControllerParameterType.Trigger);
  var sad=states.FirstOrDefault(s=>s.name=="Unease reaction")??machine.AddState("Unease reaction");sad.motion=Clip("Gorilla Misc","Gorilla Failure");
  if(!machine.anyStateTransitions.Any(t=>t.destinationState==sad)){var enter=machine.AddAnyStateTransition(sad);enter.AddCondition(AnimatorConditionMode.If,0,"Unease");enter.AddCondition(AnimatorConditionMode.Equals,0,"Activity");enter.canTransitionToSelf=false;enter.duration=.12f;var exit=sad.AddTransition(move);exit.hasExitTime=true;exit.exitTime=.9f;exit.duration=.12f;}
  // Feedback must not replace a lift/drink/placement while hands own an object.
  foreach(var transition in machine.anyStateTransitions.Where(t=>t.destinationState.name=="Warmth reaction"))if(!transition.conditions.Any(c=>c.parameter=="Activity"))transition.AddCondition(AnimatorConditionMode.Equals,0,"Activity");
  var root=new GameObject("Animal Friends Gorilla");
  try{
   var model=UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(modelPath),root.transform);model.name="Gorilla artwork";
   var animator=model.GetComponent<Animator>();animator.runtimeAnimatorController=controller;animator.applyRootMotion=false;animator.cullingMode=AnimatorCullingMode.AlwaysAnimate;
   model.AddComponent<HumanoidHands>();
   foreach(var r in model.GetComponentsInChildren<Renderer>())r.sharedMaterials=r.sharedMaterials.Select(material=>{
    if(material==null)throw new InvalidOperationException("Missing gorilla source material.");
    string path=Output+"/"+material.name+".mat";var converted=AssetDatabase.LoadAssetAtPath<Material>(path);
    if(converted==null){converted=new Material(Shader.Find("BWVR/GameSurface"));AssetDatabase.CreateAsset(converted,path);}
    // Source color alpha stores gloss information, not holes in the fur.
    converted.mainTexture=material.mainTexture;converted.color=Color.white;converted.SetFloat("_Cutoff",material.name=="Eye Shine"?.2f:0);converted.enableInstancing=true;EditorUtility.SetDirty(converted);return converted;
   }).ToArray();
   CharacterImporter.Save(root,Prefab);root=null;
   EditorUtility.SetDirty(tree);EditorUtility.SetDirty(controller);AssetDatabase.SaveAssets();
   Debug.Log("BWVR_GORILLA_IMPORTED: humanoid, authored in-place walk/idle and facial success/failure; BWVR carry/drink IK; licensed artwork kept local");
  }finally{if(root!=null)UnityEngine.Object.DestroyImmediate(root);}
 }
}
}
