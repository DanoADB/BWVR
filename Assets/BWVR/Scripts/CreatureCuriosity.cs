using System.Linq;
using UnityEngine;
namespace BWVR {
// A bounded curiosity loop yields to drought work and owns at most one carried object.
public sealed class CreatureCuriosity:MonoBehaviour {
 public enum Phase { Rest,Approach,Inspect,Lift,Play,CarryFriend,Place,BuildApproach,BuildPlace }
 public int built{get;private set;}
 Vector3 buildPoint,buildApproach,buildStart;Quaternion buildRotation;
 bool constructionFocus;
 VillagerSocial lastFriend;
 public Phase Current{get;private set;}CreatureAffect.Mood affect,feedbackMood;float reaction;bool lastWater;
 public CreatureAffect.Mood Affect{get=>reaction>0?feedbackMood:affect;private set=>affect=value;}
 public bool Responding=>reaction>0;
 public event System.Action<CreatureAffordance> Completed;public event System.Action<string> Failed;
 public int played{get;private set;}public int friendsCarried{get;private set;}public CarryItem Holding=>owned;
 CreatureAffect.Mood recentMood=CreatureAffect.Mood.Happy;
 CreatureRoutine routine;HumanoidMotion motion;IslandNavigator navigator;CreatureAffordance[] affordances;
 CreatureAffordance target;CarryItem owned;SliceState state;Vector3 original,landing;Quaternion originalRotation;float clock,elapsed,cooldown=7,boredom,emotion,feedbackWindow;bool lastSocial;
 public void Initialize(CreatureRoutine water,HumanoidMotion visual,CreatureAffordance[] objects){routine=water;motion=visual;navigator=GetComponent<IslandNavigator>();affordances=objects;}
 public bool TryBegin(CreatureAffordance candidate){
  if(Current!=Phase.Rest||candidate==null||!candidate.Available||routine.Current!=CreatureRoutine.Task.SeekingWater)return false;
  if(!navigator.world.Sample(candidate.transform.position,navigator.profile,12,out _))return false;
  if(candidate.GetComponent<ConstructionBlock>()!=null)constructionFocus=true;
  routine.ResetTask();navigator.ResetRoute();target=candidate;elapsed=clock=0;Current=Phase.Approach;motion?.LookAt(target.transform);Affect=CreatureAffect.Mood.Curious;return true;
 }
 public bool Tick(float dt,SliceState season,bool allowChoice=true){
  state=season;if(dt<=0||state.paused)return Current!=Phase.Rest;reaction=Mathf.Max(0,reaction-dt);
  state.hunger=Mathf.Clamp01(state.hunger+dt*.0035f);boredom+=dt;emotion=Mathf.Max(0,emotion-dt);feedbackWindow=Mathf.Max(0,feedbackWindow-dt);
  if(Current==Phase.Rest){
   Affect=state.hunger>.67f?CreatureAffect.Mood.Hungry:emotion>0?recentMood:boredom>32?CreatureAffect.Mood.Bored:CreatureAffect.Mood.Calm;
   cooldown-=dt;
   if(allowChoice&&cooldown<=0&&(state.Finished||state.water>=35)&&routine.Current==CreatureRoutine.Task.SeekingWater){
    var choice=affordances.Where(a=>a!=null&&a.Available).OrderByDescending(Score).FirstOrDefault();
    if(choice==null||!TryBegin(choice))cooldown=4;
   }
   return Current!=Phase.Rest;
  }
  elapsed+=dt;clock+=dt;
  if(target==null||(!owned&&target.Item.held)||elapsed>(target.GetComponentInParent<DiscoveryPoint>()!=null?120:24)){Finish(false);return false;}
  if(Current==Phase.Approach){
   Affect=CreatureAffect.Mood.Curious;
   if(navigator.MoveTo(target.transform.position,routine.speed,dt)){Current=Phase.Inspect;clock=0;motion?.Activity(6);}
   else if(navigator.Blocked){Affect=CreatureAffect.Mood.Sad;if(elapsed>5)Finish(false);}
  }else if(Current==Phase.Inspect){
   if(clock>1.8f){if(target.GetComponent<DiscoveryObject>()?.Owner!=null){Finish(false);return false;}var social=target.GetComponent<VillagerSocial>();if(social!=null&&!social.Willing){Finish(false);return false;}social?.ResetTask();owned=target.Item;original=owned.transform.position;originalRotation=owned.transform.rotation;owned.held=true;owned.GetComponent<Collider>().enabled=false;
    owned.GetComponent<ConstructionBlock>()?.Grab();
    owned.GetComponent<VillagerRoutine>()?.ResetTask();motion?.Activity(1);Current=Phase.Lift;clock=0;}
  }else if(Current==Phase.Lift){
   if(clock>.5f)motion?.Carry(owned.transform);
   if(clock>1.2f){clock=0;motion?.Activity(0);Affect=CreatureAffect.Mood.Excited;
    var block=owned.GetComponent<ConstructionBlock>();
    if(block!=null){
     if(!block.Site.TryPlan(block,out buildPoint,out buildRotation)){Finish(false);return false;}
     var direction=buildPoint-transform.position;direction.y=0;if(direction.sqrMagnitude<1e-9f)direction=transform.forward;
     buildApproach=buildPoint-direction.normalized*navigator.world.Scale*6;Current=Phase.BuildApproach;
    }else if(target.kind==CreatureAffordance.Kind.Villager){
     var away=transform.right*navigator.world.Scale*8;
     if(!SafeLanding(transform.position+away,out landing)&&!SafeLanding(original,out landing)){Finish(false);return false;}
     Current=Phase.CarryFriend;
    }else {Current=Phase.Play;var sound=target.GetComponent<AudioSource>();if(sound!=null)sound.Play();}
   }
  }else if(Current==Phase.CarryFriend){
   Affect=CreatureAffect.Mood.Happy;
   if(navigator.MoveTo(landing,routine.speed*.75f,dt)){Current=Phase.Place;clock=0;motion?.Activity(5);}
  }else if(Current==Phase.BuildApproach){
   Affect=CreatureAffect.Mood.Curious;var block=owned.GetComponent<ConstructionBlock>();block.Site.ShowPreview(block,buildPoint,buildRotation);
   if(navigator.MoveTo(buildApproach,routine.speed,dt)){buildStart=owned.transform.position;Current=Phase.BuildPlace;clock=0;motion?.Activity(5);}
  }else if(Current==Phase.BuildPlace){
   Affect=CreatureAffect.Mood.Excited;float blend=Mathf.SmoothStep(0,1,clock/1.3f);motion?.PlaceAt(buildPoint,buildRotation,blend);owned.transform.SetPositionAndRotation(Vector3.Lerp(buildStart,buildPoint,blend),Quaternion.Slerp(originalRotation,buildRotation,blend));
   if(clock>=1.4f){var block=owned.GetComponent<ConstructionBlock>();if(!block.Site.Drop(block,buildPoint,buildRotation,true)){Finish(false);return false;}lastWater=false;built++;played++;target.Discovered();NoticeDiscovery(target.GetComponentInParent<DiscoveryPoint>());boredom=0;feedbackWindow=12;lastSocial=false;owned=null;Finish(true);}
  }else if(Current==Phase.Play){
   Affect=target.kind==CreatureAffordance.Kind.Food?CreatureAffect.Mood.Hungry:CreatureAffect.Mood.Excited;
   if(target.kind==CreatureAffordance.Kind.Food)motion?.Carry(owned.transform,true);
   else {motion?.Carry(owned.transform);motion?.Playful(clock);}
   if(clock>3.5f){Current=Phase.Place;clock=0;motion?.Carry(owned.transform);motion?.Activity(5);}
  }else if(Current==Phase.Place&&clock>1.1f){
   lastWater=false;
   if(target.kind==CreatureAffordance.Kind.Villager){friendsCarried++;lastSocial=true;lastFriend=target.GetComponent<VillagerSocial>();if(lastFriend!=null){lastFriend.RememberRide();foreach(var witness in lastFriend.transform.parent.GetComponentsInChildren<VillagerSocial>())if(witness!=lastFriend&&!witness.GetComponent<CarryItem>().held&&Vector3.Distance(witness.transform.position,transform.position)<navigator.world.Scale*22)witness.WitnessRide();}}
   else {played++;lastSocial=false;if(target.kind==CreatureAffordance.Kind.Food){state.hunger=.05f;target.GetComponent<DiscoveryObject>()?.Eat();}}
   target.Discovered();NoticeDiscovery(target.GetComponentInParent<DiscoveryPoint>());boredom=0;emotion=5;feedbackWindow=12;Finish(true);
  }
  return Current!=Phase.Rest;
 }
 float Score(CreatureAffordance a){float novelty=1f/(1+a.discoveries*2);float distance=Vector3.Distance(transform.position,a.transform.position)/navigator.world.Scale;
  float drive=a.kind==CreatureAffordance.Kind.Food?state.hunger*2.5f:a.kind==CreatureAffordance.Kind.Villager?state.socialPreference:state.playPreference+.2f;
  return novelty+drive+(a.GetComponentInParent<DiscoveryPoint>()?.Unseen(0)==true?1.25f:0)-distance*.008f+(constructionFocus&&a.GetComponent<ConstructionBlock>()!=null?2.5f:0)+(a.GetComponent<VillagerSocial>()?.Attraction??0);
 }
 bool SafeLanding(Vector3 at,out Vector3 result){var friend=owned!=null?owned.GetComponent<IslandNavigator>():null;result=at;return friend!=null&&navigator.world.Sample(at,friend.profile,8,out result);}
 public void Offer(CreatureAffordance item){var hybrid=GetComponent<CreatureHybrid>();if(hybrid!=null){hybrid.Guide(GuidanceIntent.Investigate,item!=null?item.GetComponent<HybridTarget>():null);return;}if(Current==Phase.Rest)TryBegin(item);}
 public void NoticeDiscovery(DiscoveryPoint point){if(point==null||state==null||!point.Discover(0))return;state.creatureDiscoveries=Mathf.Min(6,state.creatureDiscoveries+1);state.playPreference=Mathf.Clamp01(state.playPreference+.025f);reaction=2;feedbackMood=recentMood=CreatureAffect.Mood.Happy;emotion=5;motion?.React();}
 public void NoticeGreeting(Transform villager){emotion=4;recentMood=CreatureAffect.Mood.Happy;Affect=recentMood;motion?.LookAt(villager);}
 public void NoticeDisassembly(Transform villager){villager.GetComponent<VillagerSocial>()?.RememberDispute();emotion=4;recentMood=CreatureAffect.Mood.Sad;Affect=recentMood;motion?.LookAt(villager);}
 public void RememberWaterDelivery(SliceState season){state=season;lastWater=true;lastSocial=false;feedbackWindow=12;}
 public void RememberObservation(SliceState season){state=season;feedbackWindow=0;}
 public bool Feedback(bool positive,bool allowLearning=true){if(state==null||state.paused)return false;bool learned=allowLearning&&feedbackWindow>0;if(learned){float change=positive?.12f:-.12f;if(lastWater)state.care=Mathf.Clamp01(state.care+change);else if(lastSocial){state.socialPreference=Mathf.Clamp01(state.socialPreference+change);lastFriend?.Feedback(positive);}else state.playPreference=Mathf.Clamp01(state.playPreference+change);feedbackWindow=0;}reaction=2;emotion=5;feedbackMood=recentMood=positive?CreatureAffect.Mood.Happy:CreatureAffect.Mood.Sad;Affect=recentMood;return learned;}
 public void ResetTask(){Finish(false);lastFriend=null;lastWater=false;reaction=0;constructionFocus=false;cooldown=7;boredom=emotion=feedbackWindow=0;Affect=CreatureAffect.Mood.Calm;}
 void Finish(bool success){
  var finished=target;
  motion?.Carry(null);motion?.Playful(0);motion?.PlaceAt(default,Quaternion.identity,0);motion?.LookAt(null);motion?.Activity(0);navigator?.ResetRoute();
  if(owned!=null){var item=owned;var point=original;
   var block=item.GetComponent<ConstructionBlock>();if(block!=null){block.Site.HidePreview();block.CancelHold();owned=null;}
   else {
   if(success&&target!=null&&target.kind==CreatureAffordance.Kind.Villager&&SafeLanding(landing,out var safe))point=safe;
   else if(success&&target!=null&&target.kind==CreatureAffordance.Kind.Roll){var at=original+transform.forward*navigator.world.Scale*4;if(navigator.world.Sample(at,navigator.world.villager,4,out var roll))point=roll+Vector3.up*navigator.world.Scale;}
   if(navigator.world.Terrain!=null&&item.kind==Offering.Villager)point=navigator.world.Terrain.Project(point);
   item.transform.position=point;item.transform.rotation=originalRotation;item.held=false;item.GetComponent<Collider>().enabled=true;
   if(item.kind==Offering.Villager){item.home=point;item.GetComponent<IslandNavigator>()?.ResetRoute();item.GetComponent<VillagerRoutine>()?.ResetTask();item.GetComponent<HumanoidMotion>()?.React();}
   }
  }
  if(!success&&target!=null)target.SetCooldown(12);
  owned=null;target=null;Current=Phase.Rest;clock=elapsed=0;cooldown=success?(constructionFocus?3:8):4;emotion=success?5:3;recentMood=success?CreatureAffect.Mood.Happy:CreatureAffect.Mood.Sad;Affect=recentMood;
  if(finished!=null){if(success)Completed?.Invoke(finished);else Failed?.Invoke("Interaction interrupted or unavailable");}
 }
 void OnDisable(){Finish(false);}
}
}
