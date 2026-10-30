using UnityEngine;
namespace BWVR {
public sealed partial class SharedRhythm {
 public VillagerExploration Ritualist{get;private set;}
 public bool ClaimRitual(VillagerExploration actor){if(actor==null||Ritualist!=null||Active||GetComponent<CarryItem>().held||GetComponent<CreatureAffordance>().host!=null||GetComponent<PlayerRhythm>()?.Reserved==true)return false;Ritualist=actor;hostMotion=actor.GetComponent<HumanoidMotion>();return true;}
 public void RitualNote(VillagerExploration actor,int note){if(Ritualist==actor&&note>=0&&note<2)Tap(hostMotion,note);}
 public void RitualFade(float dt){if(Ritualist!=null)Fade(dt);}
 public void ReleaseRitual(VillagerExploration actor){if(actor==null||Ritualist!=actor)return;ClearPose();Ritualist=null;hostMotion=null;}
}
public sealed partial class VillagerExploration {
 SharedRhythm ritualInstrument;float ritualElapsed,ritualBeat;int ritualNotes;bool ritualArrived;
 bool TryRainRitual(){
  var belief=social.Memory.rainBelief;if(culture==null||!CanStartStructure||belief==null||belief.Conviction<.45f||culture.Season.paused||culture.Season.water>=45||culture.Season.Weather.rain>=.15f||culture.Season.Hybrid.time<belief.nextRitual||social.Memory.injury>0||social.RecentFright)return false;
  var target=culture.Creature.Find(belief.instrument);var music=target!=null?target.GetComponent<SharedRhythm>():null;if(music==null||!music.isActiveAndEnabled||target.affordance?.Available!=true||Vector3.Distance(transform.position,music.transform.position)>nav.world.Scale*80||!world.Reachable(transform.position,music.transform.position,nav.profile)||!music.ClaimRitual(this))return false;
  ritualInstrument=music;ritualElapsed=ritualBeat=0;ritualNotes=0;ritualArrived=false;belief.nextRitual=culture.Season.Hybrid.time+180;routine.BeginHandsFreeActivity();nav.ResetRoute();Current=Phase.RainRitual;motion?.LookAt(music.transform);return true;
 }
 bool TickRainRitual(float dt){
  ritualElapsed+=dt;var music=ritualInstrument;if(music==null||!music.isActiveAndEnabled||music.Ritualist!=this||music.GetComponent<CarryItem>().held||social.RecentFright||social.Memory.injury>0||ritualElapsed>30||culture.Season.Weather.rain>=.35f){ResetTask();return false;}music.RitualFade(dt);
  if(!ritualArrived){if(nav.MoveTo(music.transform.position,transform.lossyScale.y*1.6f,dt)){ritualArrived=true;ritualBeat=.2f;}else if(nav.Blocked&&ritualElapsed>5){ResetTask();return false;}return true;}
  if(Vector3.Distance(transform.position,music.transform.position)>nav.world.Scale*7){ResetTask();return false;}ritualBeat-=dt;
  if(ritualNotes<4&&ritualBeat<=0){music.RitualNote(this,ritualNotes%2);ritualNotes++;ritualBeat=.7f;}
  if(ritualNotes==4&&ritualBeat<=0){var belief=social.Memory.rainBelief;belief.rituals=System.Math.Min(1000000,belief.rituals+1);culture.Music(music.GetComponent<CreatureAffordance>());culture.Creature.Journal.Add(culture.Season.Hybrid.time,"rain ritual",CreatureAction.SocialInteraction,GetComponent<HybridTarget>().id,"Repeated music in hope of rain; weather unchanged");ResetTask();Current=Phase.Celebrate;motion?.Signal(false,true,transform.position+Vector3.up*nav.world.Scale*12);return true;}return Active;
 }
 void ReleaseRainRitual(){if(ritualInstrument!=null){ritualInstrument.ReleaseRitual(this);motion?.Signal(false);}ritualInstrument=null;ritualElapsed=ritualBeat=0;ritualNotes=0;ritualArrived=false;}
}
}
