using System.Collections.Generic;
using UnityEngine;
namespace BWVR {
// Manual, swept body clearance works across both navigation profiles. PhysX does
// not resolve overlap for the transform-driven actors, and agent.Move bypasses RVO.
public sealed partial class IslandNavigator {
 static readonly List<IslandNavigator> bodies=new List<IslandNavigator>();
 static readonly float[] angles={0,30,-30,60,-60,90,-90};
 CarryItem bodyItem;
 void RegisterBody(){bodyItem=GetComponent<CarryItem>();if(!bodies.Contains(this))bodies.Add(this);}
 void UnregisterBody(){bodies.Remove(this);}
 bool Solid {get {if(bodyItem==null)bodyItem=GetComponent<CarryItem>();return isActiveAndEnabled&&profile!=null&&(bodyItem==null||!bodyItem.held);}}
 bool Peer(IslandNavigator other,Vector3 at)=>other!=null&&other!=this&&other.world==world&&other.Solid&&at.y+profile.height*world.Scale>other.transform.position.y&&other.transform.position.y+other.profile.height*world.Scale>at.y;
 public bool BodyClear(Vector3 from,Vector3 to){
  foreach(var other in bodies){if(!Peer(other,to))continue;
   var p=new Vector2(other.transform.position.x,other.transform.position.z);var a=new Vector2(from.x,from.z);var b=new Vector2(to.x,to.z);var d=b-a;float t=d.sqrMagnitude>1e-12f?Mathf.Clamp01(Vector2.Dot(p-a,d)/d.sqrMagnitude):0;
   float radius=(profile.radius+other.profile.radius)*world.Scale;
   // Allow an already overlapping spawn to separate, never to move deeper.
   if((p-a).sqrMagnitude<radius*radius){if((p-b).sqrMagnitude<=(p-a).sqrMagnitude+1e-12f)return false;}
   else if((p-(a+d*t)).sqrMagnitude<(radius*radius))return false;
  }return true;
 }
 bool Steer(Vector3 from,Vector3 direction,float distance,out Vector3 step){
  foreach(float angle in angles){var candidate=Quaternion.Euler(0,angle,0)*direction*distance;var to=from+candidate*world.Scale;
   if(world.SegmentClear(from,to,profile)&&BodyClear(from,to)){step=candidate;return true;}
  }step=Vector3.zero;return false;
 }
 bool AtOccupiedTarget(Vector3 at,Vector3 goal){
  foreach(var other in bodies){if(!Peer(other,at))continue;var target=goal-other.transform.position;target.y=0;
   var gap=at-other.transform.position;gap.y=0;
   if(target.magnitude<=other.profile.radius*world.Scale&&gap.magnitude<=(profile.radius+other.profile.radius+.2f)*world.Scale)return true;
  }return false;
 }
}
}
