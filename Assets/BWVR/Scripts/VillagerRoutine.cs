using System;
using UnityEngine;
namespace BWVR {
public sealed class VillagerRoutine:MonoBehaviour {
 public bool Worshipping{get;private set;}
 public enum Role{WaterGatherer,Farmer,Forester} public Role role;public int completedJobs{get;private set;}
 public bool Working=>phase==1;public bool Signalling=>phase>=4;public int requestsShown{get;private set;}public WaterVessel bucket{get;private set;} IslandNavigator navigator; CarryItem item;HumanoidMotion motion;Transform load;Vector3 work,store,requestSpot;Func<Vector3,Vector3> ground;Action<Role> completed;int phase;float timer,celebration;CreatureRoutine listener;
 int lastRequestJob=-1;public void ResetRequestCycle(){lastRequestJob=-1;}
 public void Initialize(Role job,Vector3 workplace,Vector3 destination,Func<Vector3,Vector3> sample,Action<Role> onComplete){navigator=GetComponent<IslandNavigator>();role=job;work=workplace;store=destination;ground=sample;completed=onComplete;item=GetComponent<CarryItem>();motion=GetComponent<HumanoidMotion>();
  if(role==Role.WaterGatherer){bucket=WaterVessel.Create(transform,"Water gatherer bucket",transform.lossyScale.y*.24f,transform.lossyScale.y*.10f,new Color(.26f,.15f,.055f));load=bucket.transform;}
  else{var g=GameObject.CreatePrimitive(job==Role.Forester?PrimitiveType.Cube:PrimitiveType.Sphere);g.name=job+" carried supplies";UnityEngine.Object.Destroy(g.GetComponent<Collider>());g.transform.localScale=Vector3.one*(transform.lossyScale.y*.21f);var mat=new Material(Shader.Find("BWVR/GameSurface"));mat.color=job==Role.Farmer?new Color(.95f,.7f,.25f):new Color(.45f,.25f,.1f);g.GetComponent<Renderer>().sharedMaterial=mat;load=g.transform;load.SetParent(transform,true);}ResetTask();
 }
 public bool RequestWater(CreatureRoutine god){if(role!=Role.WaterGatherer||phase!=0||item.held||celebration>0||lastRequestJob==completedJobs)return false;lastRequestJob=completedJobs;listener=god;var side=store-god.transform.position;side.y=0;if(side.sqrMagnitude<1e-10f)side=Vector3.forward;float distance=navigator!=null?navigator.world.Scale*(navigator.world.Terrain!=null?8:3):transform.lossyScale.y*3;requestSpot=store+side.normalized*distance;phase=4;timer=0;navigator?.ResetRoute();return true;}
 public void Celebrate(Transform god){if(item.held||Signalling)return;celebration=1.2f;motion?.LookAt(god);motion?.Signal(true);}
 public void BeginWorship(){ResetTask();Worshipping=true;if(load!=null)load.gameObject.SetActive(false);motion?.Carry(null);}
 public void ResetTask(){Worshipping=false;motion?.Pray(false);celebration=0;listener=null;motion?.LookAt(null);motion?.Signal(false);navigator?.ResetRoute();phase=0;timer=0;motion?.Activity(0);motion?.Carry(bucket!=null?load:null);if(bucket!=null)bucket.SetFill(0);if(load!=null)load.gameObject.SetActive(bucket!=null);}
 public void Tick(float dt,bool paused){if(Worshipping)return;if(item.held){navigator?.ResetRoute();motion?.Signal(false);return;}if(paused||dt<=0)return;
  if(celebration>0){celebration=Mathf.Max(0,celebration-dt);if(celebration==0){motion?.LookAt(null);motion?.Signal(false);}return;}timer+=dt;
  if(phase==4){motion?.Activity(0);if(Move(requestSpot,dt)){phase=5;timer=0;requestsShown++;motion?.LookAt(listener!=null?listener.transform:null);motion?.Signal(false,false,default,65);listener?.Notice(transform);}return;}
  if(phase==5){if(listener!=null)Face(listener.transform.position);if(timer>=1.8f){phase=6;timer=0;motion?.Signal(true);motion?.Activity(6);}return;}
  if(phase==6){if(timer>1.4f){Face(work);motion?.LookAt(null);motion?.Signal(false,true,work);}else if(listener!=null)Face(listener.transform.position);if(timer>3){phase=0;timer=0;listener=null;motion?.LookAt(null);motion?.Signal(false);motion?.Activity(0);navigator?.ResetRoute();}return;}
  if(bucket!=null){if(phase==1)bucket.SetFill(Mathf.Clamp01(timer/3));else if(phase==3)bucket.SetFill(1-timer/1.2f);}
  if(phase==0){motion?.Activity(0);if(Move(work,dt)){phase=1;timer=0;motion?.Activity(role==Role.WaterGatherer?1:role==Role.Farmer?3:4);}}
  else if(phase==1&&timer>(role==Role.Farmer?6:3)){phase=2;timer=0;load.gameObject.SetActive(true);motion?.Carry(load);motion?.Activity(0);}
  else if(phase==2&&Move(role==Role.WaterGatherer?store:item.home,dt)){phase=3;timer=0;motion?.Activity(5);}
  else if(phase==3&&timer>1.2f){completedJobs++;completed?.Invoke(role);ResetTask();}
 }
 void Face(Vector3 point){var direction=point-transform.position;direction.y=0;if(direction.sqrMagnitude>1e-10f)transform.rotation=Quaternion.LookRotation(direction);}
 bool Move(Vector3 target,float dt){if(navigator!=null)return navigator.MoveTo(target,transform.lossyScale.y*1.8f,dt);var direction=target-transform.position;direction.y=0;if(direction.magnitude<transform.lossyScale.y*.35f)return true;transform.rotation=Quaternion.LookRotation(direction);transform.position=ground(Vector3.MoveTowards(transform.position,new Vector3(target.x,transform.position.y,target.z),dt*transform.lossyScale.y*1.8f));return false;}
}
}
