using System;
using UnityEngine;
namespace BWVR {
public enum StormPhase {Dormant,Calm,Warning,Storm,Recovery,Complete,Stopped}
public enum RescueSource {None,Player,Creature,Independent}
[Serializable]public sealed class StormSupplyState {
 public const float Timber=6;
 public bool spawned,delivered,playerArrivalPending;
 public RescueSource source;
 public Vector3 grounded,destination;
 public bool Valid()=>Enum.IsDefined(typeof(RescueSource),source)&&source!=RescueSource.Independent&&
  (!delivered||spawned)&&(!playerArrivalPending||spawned&&!delivered)&&((source!=RescueSource.None)==delivered)&&Point(grounded)&&Point(destination);
 static bool Point(Vector3 p)=>HybridState.Number(p.x,-10000,10000)&&HybridState.Number(p.y,-10000,10000)&&HybridState.Number(p.z,-10000,10000);
}
[Serializable]public sealed class StormChapterState {
 public StormRecoveryState recovery;public StormFireState fire;public StormSupplyState supply;
 public StormPhase phase;public float elapsed,distress;public int episode,villager=-1;
 public bool opportunity,resolved,playerArrivalPending,previousAutomatic=true;public RescueSource source;
 public Vector3 refuge,actorPoint;public float frightAt;
 public bool Active=>phase>=StormPhase.Calm&&phase<=StormPhase.Recovery;
 public static float Duration(StormPhase p)=>p==StormPhase.Calm?180:p==StormPhase.Warning?35:p==StormPhase.Storm?150:p==StormPhase.Recovery?45:0;
 public void Step(float dt,bool paused){if(!Active||paused||!HybridState.Number(dt,0,1000000)||dt<=0)return;elapsed+=Mathf.Min(dt,1);if(elapsed>=Duration(phase)){elapsed=0;phase=(StormPhase)((int)phase+1);}}
 public bool Resolve(RescueSource by){if(!opportunity||resolved||by==RescueSource.None||!Enum.IsDefined(typeof(RescueSource),by))return false;resolved=true;playerArrivalPending=false;source=by;return true;}
 public bool Valid()=>(recovery==null||recovery.Valid()&&(!recovery.recorded||resolved&&(phase==StormPhase.Recovery||phase==StormPhase.Complete||phase==StormPhase.Stopped))&&(!recovery.thanked||source==RescueSource.Creature))&&(fire==null||fire.Valid())&&(supply==null||supply.Valid())&&Enum.IsDefined(typeof(StormPhase),phase)&&Enum.IsDefined(typeof(RescueSource),source)&&HybridState.Number(elapsed,0,Duration(phase))&&HybridState.Number(distress,0,100000000)&&episode>=0&&episode<=1000000&&villager>=-1&&villager<VillagePopulation.Adults&&HybridState.Number(frightAt,0,100000000)&&Point(refuge)&&Point(actorPoint)&&(!opportunity||villager>=0)&&(!resolved||opportunity)&&(!playerArrivalPending||opportunity&&!resolved)&&((source!=RescueSource.None)==resolved);
 static bool Point(Vector3 p)=>HybridState.Number(p.x,-10000,10000)&&HybridState.Number(p.y,-10000,10000)&&HybridState.Number(p.z,-10000,10000);
}
}
