using System;
using UnityEngine;
namespace BWVR {
[Serializable] public sealed class CathedralState {
 public int level,authorized,bellRings;public float nextBellAt;public float faithSeconds,timber,work;public bool sited;public Vector3 site;
 public float[] cargo=VillagePopulation.Create(i=>0f);
 public static readonly float[] Faith={.25f,.45f,.55f,.65f,.75f};
 public static readonly float[] Timber={6,12,18,24,32},Labor={120,180,240,360,480};
 public bool Building=>authorized>level;
 public float Remaining=>Building?Mathf.Max(0,Timber[level]-timber-Transit):0;
 public float Transit{get{float sum=0;foreach(float n in cargo)sum+=n;return sum;}}
 public void Advance(float dt,float faith){if(!HybridState.Number(dt,0,1)||dt<=0||!HybridState.Number(faith,0,1)||level>=5||Building)return;faithSeconds=faith>=Faith[level]?Mathf.Min(20,faithSeconds+dt):Mathf.Max(0,faithSeconds-dt*2);if(faithSeconds>=20){authorized=level+1;faithSeconds=0;}}
 public float Collect(ResourceState store,int identity,float available){if(!Building||identity<0||identity>=cargo.Length||cargo[identity]>0||!HybridState.Number(available,0,1000))return 0;float amount=Mathf.Min(3,Remaining,available,store.wood);if(amount<=0)return 0;store.wood-=amount;cargo[identity]=amount;return amount;}
 public void Deliver(int identity){if(!Building||identity<0||identity>=cargo.Length)return;timber+=cargo[identity];cargo[identity]=0;}
 public bool Build(float dt){if(!Building||timber+.001f<Timber[level]||!HybridState.Number(dt,0,1)||dt<=0)return false;work=Mathf.Min(Labor[level],work+dt);if(work<Labor[level])return false;level++;timber=work=0;return true;}
 public bool Valid(){if(bellRings<0||bellRings>1000000||!HybridState.Number(nextBellAt,0,100000300)||bellRings>0&&level==0)return false;if(level<0||level>5||authorized<level||authorized>Math.Min(5,level+1)||!HybridState.Number(faithSeconds,0,20)||!HybridState.Number(site.x,-10000,10000)||!HybridState.Number(site.y,-10000,10000)||!HybridState.Number(site.z,-10000,10000)||cargo==null||cargo.Length!=VillagePopulation.Adults)return false;foreach(float n in cargo)if(!HybridState.Number(n,0,3))return false;return Building?HybridState.Number(timber,0,Timber[level])&&timber+Transit<=Timber[level]+.001f&&HybridState.Number(work,0,Labor[level])&&(work==0||timber+.001f>=Timber[level]):timber==0&&work==0&&Transit==0;}
}
}
