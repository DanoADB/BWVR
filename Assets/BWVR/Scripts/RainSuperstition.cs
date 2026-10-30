using System;
using UnityEngine;
namespace BWVR {
[Serializable] public sealed class RainSuperstition {
 public int matches,misses,rituals;public float until,nextRitual;public string instrument="";
 public float Conviction=>(float)matches/(matches+misses+2);
 public bool Valid()=>matches>=0&&misses>=0&&matches+misses<=20&&rituals>=0&&rituals<=1000000&&HybridState.Number(until,0,100000100)&&HybridState.Number(nextRitual,0,100000300)&&instrument!=null&&instrument.Length<=160&&(until==0||instrument.Length>0);
 public bool Hear(float now,string id,bool dry){if(!dry||until>0||string.IsNullOrEmpty(id)||id.Length>160)return false;instrument=id;until=now+45;return true;}
 public bool Observe(float now,bool raining){if(until<=0||!raining&&now<=until)return false;bool match=raining&&now<=until;if(matches+misses>=20){matches/=2;misses/=2;}if(match)matches++;else misses++;until=0;return true;}
}
public sealed partial class SocialMemory {public RainSuperstition rainBelief;}
public sealed partial class VillageCulture {
 SliceState beliefSeason;float nextCultureCheck;
 public void Music(CreatureAffordance instrument){
  if(Season.paused||instrument==null||instrument.kind!=CreatureAffordance.Kind.Chime||instrument.Item?.held==true||Season.Weather.rain>=.15f)return;var id=instrument.GetComponent<HybridTarget>()?.id;if(string.IsNullOrEmpty(id))return;Physics.SyncTransforms();
  foreach(var job in jobs){var listener=job.GetComponent<VillagerSocial>();if(!world.ProvisionVisible(listener,instrument.transform.position,instrument.transform))continue;var belief=listener.Memory.rainBelief??(listener.Memory.rainBelief=new RainSuperstition());belief.Hear(god.State.time,id,true);}
 }
 public void Advance(){
  if(Season.paused)return;if(beliefSeason!=Season){beliefSeason=Season;nextCultureCheck=0;}if(god.State.time<nextCultureCheck)return;nextCultureCheck=god.State.time+.25f;
  foreach(var job in jobs){var person=job.GetComponent<VillagerSocial>();var belief=person.Memory.rainBelief;if(belief!=null&&belief.Observe(god.State.time,Season.Weather.rain>=.35f)){person.GetComponent<VillagerAffect>()?.Learned();god.Journal.Add(god.State.time,"rain association",CreatureAction.IdleOrObserve,person.GetComponent<HybridTarget>().id,"Music/rain matches "+belief.matches+", misses "+belief.misses+"; belief, not causation");}}
 }
}
}
