using System;
using UnityEngine;
namespace BWVR {
[Serializable] public sealed class CultureMemory {
 public int lastSurvivalEpisode,saintStories;
 public bool Valid()=>lastSurvivalEpisode>=0&&lastSurvivalEpisode<=1000000&&saintStories>=0&&saintStories<=1000000;
}
[Serializable] public sealed class SaintBelief {
 public int chosen=-1,episode,visits,rumors;public float confidence,nextVisit;
 public bool Valid()=>chosen>=-1&&chosen<VillagePopulation.Adults&&episode>=0&&episode<=1000000&&visits>=0&&visits<=1000000&&rumors>=0&&rumors<=1000000&&HybridState.Number(confidence,0,1)&&HybridState.Number(nextVisit,0,100000300)&&(chosen>=0||confidence==0);
 public void Witness(int person,int storm,float faith){if(person<0||person>=VillagePopulation.Adults||storm<=episode)return;chosen=person;episode=storm;confidence=Mathf.Clamp01(.48f+faith*.35f);nextVisit=0;}
 public void Hear(int person){if(person<0||person>=VillagePopulation.Adults||chosen>=0&&chosen!=person&&confidence>=.5f)return;if(chosen!=person){chosen=person;confidence=0;}confidence=Mathf.Min(.6f,confidence+.15f);rumors=Math.Min(1000000,rumors+1);}
}
public sealed partial class SocialMemory {
 public SaintBelief saint;
 bool CultureValid()=>(saint==null||saint.Valid())&&(rainBelief==null||rainBelief.Valid());
}
// Bounded evidence and social interpretation; this service never owns an actor motor.
public sealed partial class VillageCulture {
 readonly CreatureHybrid god;readonly VillageResources world;readonly VillagerRoutine[] jobs;readonly StormChapter chapter;readonly PlayerRhythm[] musicSources;
 public VillageCulture(CreatureHybrid creature,VillageResources resources,VillagerRoutine[] people,StormChapter storm){god=creature;world=resources;jobs=people;chapter=storm;chapter.SafeArrival+=Survival;god.GetComponent<CreatureCuriosity>().Completed+=Music;musicSources=god.GetComponentInParent<QuestSlice>().GetComponentsInChildren<PlayerRhythm>();foreach(var source in musicSources)source.Demonstrated+=Music;foreach(var job in jobs)job.GetComponent<VillagerExploration>().ConfigureCulture(this);}
 public SliceState Season=>god.Season;public CreatureHybrid Creature=>god;
 public VillagerSocial Person(int index)=>index>=0&&index<jobs.Length?jobs[index].GetComponent<VillagerSocial>():null;
 void Survival(VillagerSocial person,RescueSource source,int episode){
  if(Season.paused||person==null||episode<=0)return;var memory=Season.culture??(Season.culture=new CultureMemory());if(episode<=memory.lastSurvivalEpisode)return;memory.lastSurvivalEpisode=episode;int index=Array.FindIndex(jobs,j=>j.GetComponent<VillagerSocial>()==person);if(index<0)return;int seen=0;Physics.SyncTransforms();
  foreach(var job in jobs){var witness=job.GetComponent<VillagerSocial>();if(witness==person||witness.GetComponent<CarryItem>().held||!world.ProvisionVisible(witness,person.transform.position,person.transform))continue;var belief=witness.Memory.saint??(witness.Memory.saint=new SaintBelief());belief.Witness(index,episode,witness.Memory.faith);witness.GetComponent<VillagerAffect>()?.Learned();seen++;}
  if(seen>0){memory.saintStories=Math.Min(1000000,memory.saintStories+1);god.Journal.Add(god.State.time,"chosen survivor",CreatureAction.SocialInteraction,"villager-"+index,seen+" witnesses interpreted survival as a sign; assistance "+source);}
 }
 public void SaintVisited(VillagerSocial visitor,VillagerSocial survivor){
  if(Season.paused||visitor==null||survivor==null)return;var belief=visitor.Memory.saint;int index=Array.FindIndex(jobs,j=>j.GetComponent<VillagerSocial>()==survivor);if(belief==null||belief.chosen!=index||belief.confidence<.5f)return;
  Physics.SyncTransforms();belief.visits=Math.Min(1000000,belief.visits+1);visitor.Memory.RememberPeer("villager-"+index,true,god.State.time);visitor.Memory.faith=Mathf.Clamp01(visitor.Memory.faith+.01f);visitor.GetComponent<VillagerAffect>()?.Reassured();
  foreach(var job in jobs){var listener=job.GetComponent<VillagerSocial>();if(listener==visitor||listener==survivor||listener.GetComponent<CarryItem>().held||listener.Memory.saint?.confidence>=.5f||!world.ProvisionVisible(listener,visitor.transform.position,visitor.transform)||!world.ProvisionVisible(listener,survivor.transform.position,survivor.transform))continue;var heard=listener.Memory.saint??(listener.Memory.saint=new SaintBelief());heard.Hear(index);listener.GetComponent<VillagerAffect>()?.Learned();break;}
  god.Journal.Add(god.State.time,"survivor visit",CreatureAction.SocialInteraction,visitor.GetComponent<HybridTarget>().id,"Visited a villager believed chosen by the creature");
 }
 public void Dispose(){chapter.SafeArrival-=Survival;god.GetComponent<CreatureCuriosity>().Completed-=Music;foreach(var source in musicSources)if(source!=null)source.Demonstrated-=Music;}
}
}
