using System;
using UnityEngine;
namespace BWVR {
// Generated from the owner's purchased animations; the profile itself stays local.
public sealed class GorillaFaceProfile:ScriptableObject {
 [Serializable] public sealed class Pose {public Vector3[] positions,scales;public Quaternion[] rotations;}
 public string[] bones;public Pose[] moods;public Pose blink;
 public bool Valid(){
  if(bones==null||bones.Length==0||moods==null||moods.Length!=7||blink==null)return false;
  foreach(var pose in System.Linq.Enumerable.Concat(moods,new[]{blink}))if(pose==null||pose.rotations==null||pose.rotations.Length!=bones.Length||pose.positions==null||pose.positions.Length!=bones.Length||pose.scales==null||pose.scales.Length!=bones.Length)return false;
  return true;
 }
}
}
