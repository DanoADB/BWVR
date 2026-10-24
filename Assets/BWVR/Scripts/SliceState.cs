using System;
using UnityEngine;
namespace BWVR {
[Serializable] public sealed class SliceState {
 public int version=1; public float seconds=180, water=52, crops=66, trust=20, care=0.25f; public int gifts; public bool paused;
 public bool Finished=>seconds<=0; public bool Won=>Finished&&water>=25&&crops>=40;
 public void Step(float dt){if(paused||Finished||float.IsNaN(dt)||float.IsInfinity(dt)||dt<=0)return;dt=Mathf.Min(dt,seconds);seconds-=dt;water=Mathf.Clamp(water-dt*.27f,0,100);crops=Mathf.Clamp(crops-dt*(water<25?.2f:.04f),0,100);}
 public void Deliver(){if(Finished)return;water=Mathf.Min(100,water+8);gifts++;trust=Mathf.Min(100,trust+4);}
 public void Praise(){if(Finished)return;care=Mathf.Min(1,care+.12f);trust=Mathf.Min(100,trust+3);}
 public bool Valid()=>version==1&&Finite(seconds,0,180)&&Finite(water,0,100)&&Finite(crops,0,100)&&Finite(trust,0,100)&&Finite(care,0,1)&&gifts>=0&&gifts<=10000;
 static bool Finite(float n,float min,float max)=>!float.IsNaN(n)&&!float.IsInfinity(n)&&n>=min&&n<=max;
}
}
