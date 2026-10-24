using System.Linq;
using UnityEngine;
namespace BWVR {
// A bounded curiosity loop yields to drought work and owns at most one carried object.
public sealed class CreatureCuriosity:MonoBehaviour {
 public enum Phase { Rest,Approach,Inspect,Lift,Play,CarryFriend,Place }
 public Phase Current{get;private set;}public CreatureAffect.Mood Affect{get;private set;}
 public int played{get;private set;}public int friendsCarried{get;private set;}public CarryItem Holding=>owned;
 CreatureAffect.Mood recentMood=CreatureAffect.Mood.Happy;
 CreatureRoutine routine;HumanoidMotion motion;IslandNavigator navigator;CreatureAffordance[] affordances;
 CreatureAffordance target;CarryItem owned;SliceState state;Vector3 original,landing;Quaternion originalRotation;float clock,elapsed,cooldown=7,boredom,emotion,feedbackWindow;bool lastSocial;
 public void Initialize(CreatureRoutine water,HumanoidMotion visual,CreatureAffordance[] objects){routine=water;motion=visual;navigator=GetComponent<IslandNavigator>();affordances=objects;}
 public bool TryBegin(CreatureAffordance candidate){
  if(Current!=Phase.Rest||candidate==null||!candidate.Available||routine.Current!=CreatureRoutine.Task.SeekingWater)return false;
  if(!navigator.world.Sample(candidate.transform.position,navigator.profile,12,out _))return false;
  routine.ResetTask();navigator.ResetRoute();target=candidate;elapsed=clock=0;Current=Phase.Approach;motion?.LookAt(target.transform);Affect=CreatureAffect.Mood.Curious;return true;
 }
 public bool Tick(float dt,SliceState season){
  state=season;if(dt<=0||state.paused||state.Finished)return Current!=Phase.Rest;
  state.hunger=Mathf.Clamp01(state.hunger+dt*.0035f);boredom+=dt;emotion=Mathf.Max(0,emotion-dt);feedbackWindow=Mathf.Max(0,feedbackWindow-dt);
  if(Current==Phase.Rest){
   Affect=state.hunger>.67f?CreatureAffect.Mood.Hungry:emotion>0?recentMood:boredom>32?CreatureAffect.Mood.Bored:CreatureAffect.Mood.Calm;
   cooldown-=dt;
   if(cooldown<=0&&state.water>=35&&routine.Current==CreatureRoutine.Task.SeekingWater){
    var choice=affordances.Where(a=>a!=null&&a.Available).OrderByDescending(Score).FirstOrDefault();
    if(choice==null||!TryBegin(choice))cooldown=4;
   }
   return Current!=Phase.Rest;
  }
  elapsed+=dt;clock+=dt;
  if(target==null||(!owned&&target.Item.held)||elapsed>24){Finish(false);return false;}
  if(Current==Phase.Approach){
   Affect=CreatureAffect.Mood.Curious;
   if(navigator.MoveTo(target.transform.position,routine.speed,dt)){Current=Phase.Inspect;clock=0;motion?.Activity(6);}
   else if(navigator.Blocked){Affect=CreatureAffect.Mood.Sad;if(elapsed>5)Finish(false);}
  }else if(Current==Phase.Inspect){
   if(clock>1.8f){owned=target.Item;original=owned.transform.position;originalRotation=owned.transform.rotation;owned.held=true;owned.GetComponent<Collider>().enabled=false;
    owned.GetComponent<VillagerRoutine>()?.ResetTask();motion?.Activity(1);Current=Phase.Lift;clock=0;}
  }else if(Current==Phase.Lift){
   if(clock>.5f)motion?.Carry(owned.transform);
   if(clock>1.2f){clock=0;motion?.Activity(0);Affect=CreatureAffect.Mood.Excited;
    if(target.kind==CreatureAffordance.Kind.Villager){
     var away=transform.right*navigator.world.Scale*8;
     if(!SafeLanding(transform.position+away,out landing)&&!SafeLanding(original,out landing)){Finish(false);return false;}
     Current=Phase.CarryFriend;
    }else {Current=Phase.Play;var sound=target.GetComponent<AudioSource>();if(sound!=null)sound.Play();}
   }
  }else if(Current==Phase.CarryFriend){
   Affect=CreatureAffect.Mood.Happy;
   if(navigator.MoveTo(landing,routine.speed*.75f,dt)){Current=Phase.Place;clock=0;motion?.Activity(5);}
  }else if(Current==Phase.Play){
   Affect=target.kind==CreatureAffordance.Kind.Food?CreatureAffect.Mood.Hungry:CreatureAffect.Mood.Excited;
   if(target.kind==CreatureAffordance.Kind.Food)motion?.Carry(owned.transform,true);
   else {motion?.Carry(owned.transform);motion?.Playful(clock);}
   if(clock>3.5f){Current=Phase.Place;clock=0;motion?.Carry(owned.transform);motion?.Activity(5);}
  }else if(Current==Phase.Place&&clock>1.1f){
   if(target.kind==CreatureAffordance.Kind.Villager){friendsCarried++;lastSocial=true;}
   else {played++;lastSocial=false;if(target.kind==CreatureAffordance.Kind.Food)state.hunger=.05f;}
   target.Discovered();boredom=0;emotion=5;feedbackWindow=12;Finish(true);
  }
  return Current!=Phase.Rest;
 }
 float Score(CreatureAffordance a){float novelty=1f/(1+a.discoveries*2);float distance=Vector3.Distance(transform.position,a.transform.position)/navigator.world.Scale;
  float drive=a.kind==CreatureAffordance.Kind.Food?state.hunger*2.5f:a.kind==CreatureAffordance.Kind.Villager?state.socialPreference:state.playPreference+.2f;
  return novelty+drive-distance*.012f;
 }
 bool SafeLanding(Vector3 at,out Vector3 result){var friend=owned!=null?owned.GetComponent<IslandNavigator>():null;result=at;return friend!=null&&navigator.world.Sample(at,friend.profile,8,out result);}
 public void Offer(CreatureAffordance item){if(Current==Phase.Rest)TryBegin(item);}
 public void Feedback(bool positive){if(state==null)return;if(feedbackWindow>0){float change=positive?.12f:-.12f;if(lastSocial)state.socialPreference=Mathf.Clamp01(state.socialPreference+change);else state.playPreference=Mathf.Clamp01(state.playPreference+change);feedbackWindow=0;}emotion=5;recentMood=positive?CreatureAffect.Mood.Happy:CreatureAffect.Mood.Sad;Affect=recentMood;}
 public void ResetTask(){Finish(false);cooldown=7;boredom=emotion=feedbackWindow=0;Affect=CreatureAffect.Mood.Calm;}
 void Finish(bool success){
  motion?.Carry(null);motion?.Playful(0);motion?.LookAt(null);motion?.Activity(0);navigator?.ResetRoute();
  if(owned!=null){var item=owned;var point=original;
   if(success&&target!=null&&target.kind==CreatureAffordance.Kind.Villager&&SafeLanding(landing,out var safe))point=safe;
   else if(success&&target!=null&&target.kind==CreatureAffordance.Kind.Roll){var at=original+transform.forward*navigator.world.Scale*4;if(navigator.world.Sample(at,navigator.world.villager,4,out var roll))point=roll+Vector3.up*navigator.world.Scale;}
   if(navigator.world.Terrain!=null&&item.kind==Offering.Villager)point=navigator.world.Terrain.Project(point);
   item.transform.position=point;item.transform.rotation=originalRotation;item.held=false;item.GetComponent<Collider>().enabled=true;
   if(item.kind==Offering.Villager){item.home=point;item.GetComponent<IslandNavigator>()?.ResetRoute();item.GetComponent<VillagerRoutine>()?.ResetTask();item.GetComponent<HumanoidMotion>()?.React();}
  }
  if(!success&&target!=null)target.cooldown=Time.time+12;
  owned=null;target=null;Current=Phase.Rest;clock=elapsed=0;cooldown=success?8:4;emotion=success?5:3;recentMood=success?CreatureAffect.Mood.Happy:CreatureAffect.Mood.Sad;Affect=recentMood;
 }
 void OnDisable(){Finish(false);}
}
}
