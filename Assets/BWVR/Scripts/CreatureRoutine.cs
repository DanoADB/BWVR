using System;
using UnityEngine;
namespace BWVR {
public sealed class CreatureRoutine:MonoBehaviour {
 public enum Task { SeekingWater,Pickup,Drink,Carry,Deliver }
 public Task Current{get;private set;} public int deliveries{get;private set;} public float speed=.09f;
 IslandNavigator navigator; HumanoidMotion motion;CarryItem water;Vector3 spring,cistern;Action delivered;Func<Vector3,Vector3> ground;float timer,attention;bool ownsWater;public int acknowledgements{get;private set;}
 public bool WaitingForRoute=>navigator!=null&&navigator.Blocked;
 public bool WaterAvailable=>water!=null&&!water.held;public Vector3 WaterPosition=>water!=null?water.transform.position:spring;
 public event Action Completed;
 public void Initialize(HumanoidMotion visual,CarryItem offering,Vector3 source,Vector3 destination,Action onDelivery,Func<Vector3,Vector3> sample){navigator=GetComponent<IslandNavigator>();motion=visual;water=offering;spring=source;cistern=destination;delivered=onDelivery;ground=sample;ResetTask();}
 public bool Notice(Transform villager){if(Current!=Task.SeekingWater||ownsWater||villager==null)return false;float height=motion!=null?motion.transform.lossyScale.y:.1f;if(Vector3.Distance(transform.position,villager.position)>height*6)return false;attention=1.4f;motion?.LookAt(villager);acknowledgements++;return true;}
 public void ResetTask(){attention=0;motion?.LookAt(null);navigator?.ResetRoute();ReturnWater();Current=Task.SeekingWater;timer=0;motion?.Activity(0);}
 public void Tick(float dt,bool paused){if(paused||dt<=0)return;if(attention>0){attention=Mathf.Max(0,attention-dt);if(attention==0)motion?.LookAt(null);return;}timer+=dt;
  switch(Current){
   case Task.SeekingWater:
    motion?.Activity(0);if(!Move(water!=null&&!water.held?water.transform.position:spring,dt))break;
    if(water==null||water.held)break;ownsWater=true;water.held=true;water.GetComponent<Collider>().enabled=false;Change(Task.Pickup,1);break;
   case Task.Pickup:
    if(timer>=.55f)motion?.Carry(water.transform);if(timer>=1.15f){motion?.Carry(water.transform,true);Change(Task.Drink,2);}break;
   case Task.Drink:
    if(timer>=1.8f){motion?.Carry(water.transform);Change(Task.Carry,0);}break;
   case Task.Carry:
    if(Move(cistern,dt))Change(Task.Deliver,5);break;
   case Task.Deliver:
    if(timer>=1.1f){ReturnWater();deliveries++;delivered?.Invoke();Change(Task.SeekingWater,0);Completed?.Invoke();}break;
  }
 }
 bool Move(Vector3 target,float dt){if(navigator!=null)return navigator.MoveTo(target,speed,dt);var flat=target-transform.position;flat.y=0;if(flat.magnitude<speed*.55f)return true;
  transform.rotation=Quaternion.LookRotation(flat);transform.position=ground(Vector3.MoveTowards(transform.position,new Vector3(target.x,transform.position.y,target.z),dt*speed));return false;
 }
 void Change(Task task,int animation){Current=task;timer=0;motion?.Activity(animation);}
 void ReturnWater(){motion?.Carry(null);if(!ownsWater||water==null)return;ownsWater=false;water.held=false;water.transform.position=water.home;water.transform.rotation=Quaternion.identity;water.GetComponent<Collider>().enabled=true;}
 void OnDisable(){ReturnWater();}
}
}
