using UnityEngine;
using UnityEngine.AI;
namespace BWVR {
public sealed class IslandNavigator:MonoBehaviour {
 public bool Blocked{get;private set;}public int completedRoutes{get;private set;}public int detours{get;private set;}
 public IslandNavigation world{get;private set;}public IslandNavigation.Profile profile{get;private set;}
 NavMeshAgent agent;NavMeshPath path;Vector3[] corners;Vector3 requested;int corner;float retry;bool hasTarget;
 public void Initialize(IslandNavigation navigation,bool large){world=navigation;profile=large?world.creature:world.villager;path=new NavMeshPath();
  var proxy=new GameObject(name+" routing agent");proxy.transform.SetParent(world.transform,false);
  if(!world.Sample(transform.position,profile,20,out var start)){Blocked=true;Destroy(proxy);return;}transform.position=world.Terrain!=null?world.Terrain.Project(start):start;proxy.transform.position=world.ToNav(start);
  proxy.SetActive(false);agent=proxy.AddComponent<NavMeshAgent>();agent.agentTypeID=profile.id;agent.radius=profile.radius;agent.height=profile.height;agent.areaMask=1;agent.updatePosition=false;agent.updateRotation=false;agent.obstacleAvoidanceType=ObstacleAvoidanceType.NoObstacleAvoidance;proxy.SetActive(true);agent.isStopped=true;
 }
 public void ResetRoute(){hasTarget=false;corners=null;Blocked=false;retry=0;}
 public bool CanPlace(Vector3 position,out Vector3 result){if(!world.Sample(position,profile,profile.radius*.8f,out result))return false;if(world.Terrain!=null)result=world.Terrain.Project(result);return true;}
 public bool MoveTo(Vector3 target,float speed,float dt){
  if(agent==null){Blocked=true;return false;}if(dt<=0)return false;retry-=dt;
  if(!hasTarget||(target-requested).sqrMagnitude>world.Scale*world.Scale*.25f){hasTarget=true;requested=target;corners=null;retry=0;}
  if(corners==null){if(retry>0)return false;retry=1;
   // Re-enter safely after the player has carried and placed an actor.
   if(!world.Sample(transform.position,profile,profile.radius+1,out var start)||!agent.Warp(world.ToNav(start))){Blocked=true;return false;}
   if(!world.Sample(target,profile,profile.radius*2+8,out var goal)||!agent.CalculatePath(world.ToNav(goal),path)||path.status!=NavMeshPathStatus.PathComplete){Blocked=true;return false;}
   corners=path.corners;corner=1;Blocked=false;if(corners.Length>2)detours++;
  }
  float budget=dt*speed/world.Scale;
  while(corner<corners.Length&&budget>0){var before=agent.nextPosition;var delta=corners[corner]-before;delta.y=0;float distance=delta.magnitude;
   if(distance<.05f){corner++;continue;}float travel=Mathf.Min(distance,budget);agent.Move(delta/distance*travel);var after=agent.nextPosition;var actual=after-before;actual.y=0;
   if(actual.sqrMagnitude<1e-10f){corners=null;Blocked=true;retry=0;return false;}
   transform.rotation=Quaternion.LookRotation(actual);budget-=travel;
   var remaining=corners[corner]-after;remaining.y=0;if(remaining.magnitude<.05f)corner++;else break;
  }
  var position=world.ToWorld(agent.nextPosition);transform.position=world.Terrain!=null?world.Terrain.Project(position):position;
  if(corner>=corners.Length){completedRoutes++;return true;}return false;
 }
 void OnDestroy(){if(agent!=null)Destroy(agent.gameObject);}
}
}
