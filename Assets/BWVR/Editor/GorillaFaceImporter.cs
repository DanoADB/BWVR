using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace BWVR.Editor {
public static class GorillaFaceImporter {
 public static void Configure(GameObject model){
  var animator=model.GetComponent<Animator>();var controller=animator.runtimeAnimatorController;
  AnimationClip Clip(string name)=>AssetDatabase.FindAssets("t:Model",new[]{AnimalFriendsImporter.Source+"/3D/Animations"}).Select(AssetDatabase.GUIDToAssetPath).SelectMany(AssetDatabase.LoadAllAssetsAtPath).OfType<AnimationClip>().First(c=>c.name==name);
  var names=new[]{"Gorilla Idle","Gorilla Idle 2","Gorilla Success","Gorilla Failure","Gorilla Talk","Gorilla Sleep","Gorilla Success"};
  var times=new[]{.2f,.7f,.65f,1.4f,1.1f,1.1f,1.2f};
  var facial=model.GetComponentsInChildren<Transform>().Where(t=>t.name.StartsWith("Gorilla ")&&(t.name.Contains("Mouth")||t.name.Contains("Eyebrow")||t.name.Contains("Eyelid")||t.name=="Gorilla Jaw")).ToArray();
  if(facial.Length<10)throw new InvalidOperationException("Gorilla facial bones missing");
  const string path="Assets/LocalLicensedArt/AnimalFriends/Generated/GorillaFace.asset";
  var profile=AssetDatabase.LoadAssetAtPath<GorillaFaceProfile>(path);if(profile==null){profile=ScriptableObject.CreateInstance<GorillaFaceProfile>();AssetDatabase.CreateAsset(profile,path);}
  profile.bones=facial.Select(t=>t.name).ToArray();profile.moods=new GorillaFaceProfile.Pose[7];
  var mixer=new AnimatorOverrideController(controller);var success=Clip("Gorilla Success");
  try{
   for(int mood=0;mood<names.Length;mood++){
    var source=Clip(names[mood]);mixer[success]=source;animator.runtimeAnimatorController=mixer;animator.Rebind();animator.Play("Warmth reaction",0,times[mood]/source.length);animator.Update(0);
    profile.moods[mood]=new GorillaFaceProfile.Pose{positions=facial.Select(t=>t.localPosition).ToArray(),rotations=facial.Select(t=>t.localRotation).ToArray(),scales=facial.Select(t=>t.localScale).ToArray()};
   }
  }finally{animator.runtimeAnimatorController=controller;animator.Rebind();animator.Play("Idle and walk",0,0);animator.Update(0);UnityEngine.Object.DestroyImmediate(mixer);}
  GorillaFaceProfile.Pose Copy(GorillaFaceProfile.Pose p)=>new GorillaFaceProfile.Pose{positions=p.positions.ToArray(),rotations=p.rotations.ToArray(),scales=p.scales.ToArray()};
  profile.blink=Copy(profile.moods[(int)CreatureAffect.Mood.Bored]);
  var original=profile.moods.Select(Copy).ToArray();
  void Mix(int destination,int source,int bone,float weight){var p=profile.moods[destination];var q=original[source];p.positions[bone]=Vector3.Lerp(p.positions[bone],q.positions[bone],weight);p.rotations[bone]=Quaternion.Slerp(p.rotations[bone],q.rotations[bone],weight);p.scales[bone]=Vector3.Lerp(p.scales[bone],q.scales[bone],weight);}
  for(int i=0;i<facial.Length;i++){
   string bone=facial[i].name;bool lid=bone.Contains("Eyelid"),mouth=bone.Contains("Mouth")||bone=="Gorilla Jaw";
   // An animation's blink frame must not become a permanently closed-eye mood.
   if(lid){Mix(2,0,i,.85f);Mix(3,0,i,.65f);Mix(4,0,i,.5f);Mix(5,0,i,.55f);Mix(6,1,i,1);}
   if(mouth){Mix(1,0,i,.8f);Mix(2,0,i,.45f);if(bone=="Gorilla Jaw")Mix(4,3,i,.55f);else Mix(4,3,i,.85f);}
   if(bone.Contains("Eyebrow")){Mix(5,0,i,.8f);Mix(4,3,i,.5f);if(bone.Contains("R Eyebrow"))Mix(1,2,i,.35f);}
  }
  EditorUtility.SetDirty(profile);var face=model.GetComponent<GorillaMoodFace>()??model.AddComponent<GorillaMoodFace>();face.profile=profile;
 }
}
}
