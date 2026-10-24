using System;
using UnityEngine;
namespace BWVR {
public sealed class VillagerRoutine:MonoBehaviour {
 public enum Role{WaterGatherer,Farmer,Forester} public Role role;public int completedJobs{get;private set;}
 public bool Working=>phase==1; CarryItem item;HumanoidMotion motion;Transform load;Vector3 work,store;Func<Vector3,Vector3> ground;Action<Role> completed;int phase;float timer;
 public void Initialize(Role job,Vector3 workplace,Vector3 destination,Func<Vector3,Vector3> sample,Action<Role> onComplete){role=job;work=workplace;store=destination;ground=sample;completed=onComplete;item=GetComponent<CarryItem>();motion=GetComponent<HumanoidMotion>();
  var g=GameObject.CreatePrimitive(job==Role.Forester?PrimitiveType.Cube:PrimitiveType.Sphere);g.name=job+" carried supplies";UnityEngine.Object.Destroy(g.GetComponent<Collider>());g.transform.localScale=Vector3.one*(transform.lossyScale.y*.21f);var mat=new Material(Shader.Find("BWVR/GameSurface"));mat.color=job==Role.WaterGatherer?new Color(.25f,.8f,.95f):job==Role.Farmer?new Color(.95f,.7f,.25f):new Color(.45f,.25f,.1f);g.GetComponent<Renderer>().sharedMaterial=mat;load=g.transform;load.SetParent(transform,true);ResetTask();
 }
 public void ResetTask(){phase=0;timer=0;motion?.Activity(0);motion?.Carry(null);if(load!=null)load.gameObject.SetActive(false);}
 public void Tick(float dt,bool paused){if(paused||item.held||dt<=0)return;timer+=dt;
  if(phase==0){motion?.Activity(0);if(Move(work,dt)){phase=1;timer=0;motion?.Activity(role==Role.WaterGatherer?1:role==Role.Farmer?3:4);}}
  else if(phase==1&&timer>(role==Role.Farmer?6:3)){phase=2;timer=0;load.gameObject.SetActive(true);motion?.Carry(load);motion?.Activity(0);}
  else if(phase==2&&Move(role==Role.WaterGatherer?store:item.home,dt)){phase=3;timer=0;motion?.Activity(5);}
  else if(phase==3&&timer>1.2f){completedJobs++;completed?.Invoke(role);ResetTask();}
 }
 bool Move(Vector3 target,float dt){var direction=target-transform.position;direction.y=0;if(direction.magnitude<transform.lossyScale.y*.35f)return true;transform.rotation=Quaternion.LookRotation(direction);transform.position=ground(Vector3.MoveTowards(transform.position,new Vector3(target.x,transform.position.y,target.z),dt*transform.lossyScale.y*1.8f));return false;}
}
}
