using UnityEngine;
namespace BWVR {
// A portable physical opening; the existing carry controller places it on the island.
public sealed class BallGoal:MonoBehaviour {
 public GoalCourse course;public int index;
 public bool Place(Vector3 point)=>course.Place(this,point);
}
}
