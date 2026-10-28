using System;
using UnityEngine;
namespace BWVR {
[Serializable]public sealed class GoalLayout {public Vector3 point;public float yaw;public bool Valid()=>HybridState.Number(point.x,-10000,10000)&&HybridState.Number(point.y,-10000,10000)&&HybridState.Number(point.z,-10000,10000)&&HybridState.Number(yaw,0,360);}
[Serializable]public sealed class GoalMemory {
 public int playerGoals,creatureGoals,observedGoals,sharedRounds;public float sharedEnjoyment=.4f;public float preference=.5f,practice;
 public GoalLayout[] layout=Array.Empty<GoalLayout>();
 public bool Valid()=>sharedRounds>=0&&sharedRounds<=1000000&&HybridState.Number(sharedEnjoyment,0,1)&&playerGoals>=0&&playerGoals<=1000000&&creatureGoals>=0&&creatureGoals<=1000000&&observedGoals>=0&&observedGoals<=playerGoals&&HybridState.Number(preference,0,1)&&HybridState.Number(practice,0,1)&&layout!=null&&(layout.Length==0||layout.Length==3)&&Array.TrueForAll(layout,p=>p!=null&&p.Valid());
 public void Complete(bool creature,int count,bool witnessed){count=Mathf.Clamp(count,0,3);if(creature){creatureGoals=Math.Min(1000000,creatureGoals+count);if(count>0)practice=Mathf.Clamp01(practice+.04f);}else{playerGoals=Math.Min(1000000,playerGoals+count);if(witnessed)observedGoals=Math.Min(playerGoals,observedGoals+count);}}
}
}
