using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
namespace BWVR.Editor {
public static class GorillaChecks {
 [MenuItem("BWVR/Verify purchased gorilla artwork")]
 public static void Verify(){
  void Require(bool ok,string why){if(!ok)throw new Exception(why);}
  var prefab=AssetDatabase.LoadAssetAtPath<GameObject>(AnimalFriendsImporter.Prefab);Require(prefab!=null,"Import the purchased gorilla first.");
  var animator=prefab.GetComponentInChildren<Animator>();Require(animator!=null&&animator.avatar.isValid&&animator.avatar.isHuman&&!animator.applyRootMotion,"Invalid humanoid or root motion ownership");
  Require(prefab.GetComponent<HumanoidMotion>()!=null&&animator.GetComponent<HumanoidHands>()!=null,"Missing existing interaction adapter");
  var controller=(AnimatorController)animator.runtimeAnimatorController;var states=controller.layers[0].stateMachine.states.Select(s=>s.state).ToArray();
  var tree=(BlendTree)states.First(s=>s.name=="Idle and walk").motion;
  Require(tree.children[0].motion.name=="Gorilla Idle"&&tree.children[1].motion.name=="Gorilla Walk In Place","Not using authored in-place locomotion");
  Require(states.First(s=>s.name=="Warmth reaction").motion.name=="Gorilla Success"&&states.First(s=>s.name=="Unease reaction").motion.name=="Gorilla Failure","Missing authored feedback");
  var success=(AnimationClip)states.First(s=>s.name=="Warmth reaction").motion;Require(success.humanMotion&&AnimationUtility.GetCurveBindings(success).Any(b=>b.path.Contains("Eyelid")),"Authored facial animation lost");
  Require(states.First(s=>s.name=="Pickup").motion!=null&&states.First(s=>s.name=="Drinking reach").motion!=null&&controller.layers[0].iKPass,"Missing pickup/drink/IK");
  foreach(var r in prefab.GetComponentsInChildren<Renderer>())foreach(var m in r.sharedMaterials)Require(m!=null&&m.shader.name=="BWVR/GameSurface"&&m.mainTexture!=null&&m.GetFloat("_Cutoff")== (m.name=="Eye Shine"?.2f:0),"Unsupported/missing gorilla material");
  var root=UnityEngine.Object.Instantiate(prefab);
  try{
   var renderers=root.GetComponentsInChildren<Renderer>();var bounds=renderers[0].bounds;foreach(var r in renderers)bounds.Encapsulate(r.bounds);
   Require(Mathf.Abs(bounds.size.y-1)<.02f&&Mathf.Abs(bounds.min.y)<.02f,"Gorilla feet/normalized scale mismatch");
   var a=root.GetComponentInChildren<Animator>();Require(a.GetBoneTransform(HumanBodyBones.LeftHand)!=null&&a.GetBoneTransform(HumanBodyBones.RightHand)!=null&&a.GetBoneTransform(HumanBodyBones.Head)!=null,"Missing carry/look bones");
  }finally{UnityEngine.Object.DestroyImmediate(root);}
  Debug.Log("BWVR_GORILLA_PASS: local humanoid prefab, normalized feet/height, authored in-place movement and facial feedback, retained pickup/drink/hand IK, Quest surface materials");
 }
 public static string Capture(CreatureAffect.Mood? mood=null){
  var prefab=AssetDatabase.LoadAssetAtPath<GameObject>(AnimalFriendsImporter.Prefab);if(prefab==null)throw new Exception("Gorilla prefab missing");
  var preview=new PreviewRenderUtility();var model=UnityEngine.Object.Instantiate(prefab);Texture2D image=null;
  try{
   preview.AddSingleGO(model);var a=model.GetComponentInChildren<Animator>();a.Rebind();a.Play("Idle and walk",0,0);a.Update(0);if(mood.HasValue)model.GetComponentInChildren<GorillaMoodFace>().ApplyMood(mood.Value,4);
   preview.camera.transform.position=new Vector3(1.4f,.8f,2.5f);preview.camera.transform.LookAt(new Vector3(0,.53f,0));preview.camera.orthographic=true;preview.camera.orthographicSize=.72f;preview.camera.nearClipPlane=.01f;preview.camera.farClipPlane=10;preview.camera.backgroundColor=new Color(.06f,.085f,.09f);preview.camera.clearFlags=CameraClearFlags.SolidColor;
   preview.lights[0].intensity=1.1f;preview.lights[0].transform.rotation=Quaternion.Euler(35,25,0);preview.lights[1].intensity=.7f;preview.ambientColor=new Color(.5f,.5f,.5f);
   preview.BeginStaticPreview(new Rect(0,0,1000,1000));preview.camera.Render();image=preview.EndStaticPreview();Directory.CreateDirectory(".local/gorilla-preview");var path=Path.GetFullPath(".local/gorilla-preview/animal-friends-gorilla"+(mood.HasValue?"-"+mood.Value:"")+".png");File.WriteAllBytes(path,image.EncodeToPNG());return path;
  }finally{if(image!=null)UnityEngine.Object.DestroyImmediate(image);preview.Cleanup();}
 }
}
}
