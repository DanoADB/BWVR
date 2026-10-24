using System;
using UnityEngine;
namespace BWVR {
[Serializable]public sealed class PlayDamage {public string id;public float amount;}
[Serializable]public sealed class WorldPlayMemory {
 public float rallyFun;public int rallyReturns,villagerKicks;
 public float nextKick;public int kicks;
 public float throwingFun=.25f,caution=.2f;public int throws,safeThrows,harmfulThrows;public PlayDamage[] damage=Array.Empty<PlayDamage>();
 public float Damage(string id){foreach(var d in damage)if(d.id==id)return d.amount;return 0;}
 public float Hurt(string id,float amount){foreach(var d in damage)if(d.id==id)return d.amount=Mathf.Clamp01(d.amount+amount);if(damage.Length>=256)return 0;Array.Resize(ref damage,damage.Length+1);var added=new PlayDamage{id=id,amount=Mathf.Clamp01(amount)};damage[damage.Length-1]=added;return added.amount;}
 public bool Valid(){if(!HybridState.Number(rallyFun,0,1)||rallyReturns<0||rallyReturns>1000000||villagerKicks<0||villagerKicks>1000000||!HybridState.Number(nextKick,0,float.MaxValue)||kicks<0||kicks>1000000||!HybridState.Number(throwingFun,0,1)||!HybridState.Number(caution,0,1)||throws<0||safeThrows<0||harmfulThrows<0||throws>1000000||safeThrows>throws||harmfulThrows>throws||safeThrows+harmfulThrows>throws||damage==null||damage.Length>256)return false;var ids=new System.Collections.Generic.HashSet<string>();foreach(var d in damage)if(d==null||string.IsNullOrEmpty(d.id)||d.id.Length>160||!ids.Add(d.id)||!HybridState.Number(d.amount,0,1))return false;return true;}
}
}
