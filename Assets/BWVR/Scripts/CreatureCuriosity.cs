using System.Linq;
using UnityEngine;
namespace BWVR {
// A bounded curiosity loop yields to drought work and owns at most one carried object.
public sealed partial class CreatureCuriosity:MonoBehaviour {
 public enum Phase { Rest,Approach,Inspect,Lift,Play,CarryFriend,Place,BuildApproach,BuildPlace,ThrowBall,WatchBall,RallyBall,FetchCarry,FetchPlace,RhythmGame,ThrowTree,WatchTree,ChaseBall,EchoRhythm,SpinTrick,PuzzleAssist,StormRescue,StormSupply }
 public int built{get;private set;}
 Vector3 buildPoint,buildApproach,buildStart;Quaternion buildRotation;
 bool buildCorrection;public int alignmentCorrections{get;private set;}BallGoal goalTarget;bool echoPlay;bool goalPlay;bool bowlingPlay;bool knockdownPlay;bool fetching;int fetchTicket;bool constructionFocus,askedPlaymate;float inviteGestureUntil;
 VillagerSocial lastFriend;
 public Phase Current{get;private set;}CreatureAffect.Mood affect,feedbackMood;float reaction,responseProtection;bool lastWater;
 public CreatureAffect.Mood Affect{get=>reaction>0?feedbackMood:affect;private set=>affect=value;}
 public bool Responding=>reaction>0;
 public bool FeedbackProtected=>responseProtection>0;
 public event System.Action<CreatureAffordance> Completed;public event System.Action<string> Failed;
 public int played{get;private set;}public int friendsCarried{get;private set;}public CarryItem Holding=>owned;
 CreatureAffect.Mood recentMood=CreatureAffect.Mood.Happy;
 CreatureRoutine routine;HumanoidMotion motion;IslandNavigator navigator;CreatureAffordance[] affordances;
 public bool PlayingInventedGame=>knockdownPlay;
 public bool HandlingBeachBall=>target!=null&&target.GetComponent<BeachBall>()!=null&&Current!=Phase.Rest;
 public bool UsingConstruction(ConstructionSite site)=>Current!=Phase.Rest&&target!=null&&target.GetComponent<ConstructionBlock>()?.Site==site;
 CreatureAffordance target;CarryItem owned;SliceState state;Vector3 original,landing;Quaternion originalRotation;float clock,elapsed,cooldown=7,boredom,emotion,feedbackWindow;bool lastSocial;
 public void Initialize(CreatureRoutine water,HumanoidMotion visual,CreatureAffordance[] objects){routine=water;motion=visual;navigator=GetComponent<IslandNavigator>();affordances=objects;}
 public bool TryBeginSpin(CreatureAffordance candidate){if(Current!=Phase.Rest||candidate==null||routine.Current!=CreatureRoutine.Task.SeekingWater||candidate.GetComponent<BeachBall>()?.Spin?.Begin()!=true)return false;routine.ResetTask();navigator.ResetRoute();target=candidate;owned=null;goalPlay=echoPlay=fetching=bowlingPlay=false;goalTarget=null;elapsed=clock=0;Current=Phase.SpinTrick;motion?.LookAt(null);motion?.TurnInPlace(true);Affect=CreatureAffect.Mood.Excited;return true;}
 public bool TryBeginEcho(CreatureAffordance candidate){if(Current!=Phase.Rest||candidate==null||routine.Current!=CreatureRoutine.Task.SeekingWater||candidate.GetComponent<PlayerRhythm>()?.Claim()!=true)return false;routine.ResetTask();navigator.ResetRoute();target=candidate;owned=null;echoPlay=true;fetching=bowlingPlay=false;elapsed=clock=0;Current=Phase.Approach;motion?.LookAt(target.transform);Affect=CreatureAffect.Mood.Curious;return true;}
 public bool TryBeginChase(CreatureAffordance candidate){if(Current!=Phase.Rest||candidate==null||routine.Current!=CreatureRoutine.Task.SeekingWater||candidate.GetComponent<BeachBall>()?.Chase?.Begin()!=true)return false;routine.ResetTask();navigator.ResetRoute();target=candidate;owned=null;candidate.GetComponent<BeachBall>().Goals?.Cancel();candidate.GetComponent<BeachBall>().Spin?.ClearCompletion();goalPlay=false;fetching=bowlingPlay=false;elapsed=clock=0;Current=Phase.ChaseBall;motion?.LookAt(target.transform);Affect=CreatureAffect.Mood.Excited;return true;}
 public bool TryBegin(CreatureAffordance candidate){
  if(candidate?.GetComponent<StormSupply>()!=null)return TryBeginStormSupply(candidate);
  if(candidate?.GetComponent<PuzzlePad>()!=null)return TryBeginPuzzle(candidate);
  if(Current!=Phase.Rest||candidate==null||!candidate.Available||routine.Current!=CreatureRoutine.Task.SeekingWater)return false;
  if(!navigator.world.Sample(candidate.transform.position,navigator.profile,12,out _))return false;
  var plannedBlock=candidate.GetComponent<ConstructionBlock>();if(plannedBlock!=null){if(GetComponent<CreatureConstructionLearning>()?.Memory.Knows(plannedBlock.Site.Recipe)==false)return false;constructionFocus=true;}
  if(candidate.host==null)candidate.GetComponent<SharedRhythm>()?.ResetOffer();
  if(candidate.host!=null&&!candidate.host.Accept(candidate))return false;
  var playerBall=candidate.GetComponent<BeachBall>();playerBall?.Spin?.ClearCompletion();playerBall?.Chase?.ClearCompletion();if(playerBall!=null)playerBall.JustFetched=false;routine.ResetTask();navigator.ResetRoute();target=candidate;playerBall?.Goals?.Cancel();goalTarget=playerBall?.Goals?.Plan(transform.position);goalPlay=goalTarget!=null&&(CreatureDecisionModel.Variation(GetComponent<CreatureHybrid>().State.seed,GetComponent<CreatureHybrid>().State.decisions,CreatureAction.PlayWithToy,"goal")+1)*.5f<Mathf.Lerp(.15f,.85f,GetComponent<CreatureHybrid>().Season.WorldPlay.Goals.preference);bowlingPlay=!goalPlay&&candidate.GetComponent<BeachBall>()?.Bowling?.CanImitate(transform.position)==true;fetching=candidate.GetComponent<BeachBall>()?.FetchReady==true;fetchTicket=fetching?candidate.GetComponent<BeachBall>().FetchTicket:0;knockdownPlay=playerBall!=null&&playerBall.Goals?.Turns.Active!=true&&!bowlingPlay&&!fetching&&playerBall.TryExperimentalAim(transform.position,out _)&&(CreatureDecisionModel.Variation(GetComponent<CreatureHybrid>().State.seed,GetComponent<CreatureHybrid>().State.decisions,CreatureAction.PlayWithToy,"invented-knockdown")+1)*.5f<(GetComponent<CreatureHybrid>().Season.worldPlay?.invented?.Discovered==true?Mathf.Lerp(.2f,.9f,GetComponent<CreatureHybrid>().Season.worldPlay.invented.preference):.2f);if(knockdownPlay){goalPlay=false;goalTarget=null;}askedPlaymate=false;inviteGestureUntil=0;elapsed=clock=0;Current=Phase.Approach;motion?.LookAt(target.transform);Affect=CreatureAffect.Mood.Curious;return true;
 }
 public bool Tick(float dt,SliceState season,bool allowChoice=true){
  state=season;if(dt<=0||state.paused)return Current!=Phase.Rest;reaction=Mathf.Max(0,reaction-dt);responseProtection=Mathf.Max(0,responseProtection-dt);
  state.hunger=Mathf.Clamp01(state.hunger+dt*.0035f);boredom+=dt;emotion=Mathf.Max(0,emotion-dt);feedbackWindow=Mathf.Max(0,feedbackWindow-dt);
  if(Current==Phase.Rest){
   Affect=state.hunger>.67f?CreatureAffect.Mood.Hungry:emotion>0?recentMood:state.Hybrid.Motivation.boredom>.65f?CreatureAffect.Mood.Bored:state.Hybrid.Motivation.curiosity>.7f?CreatureAffect.Mood.Curious:CreatureAffect.Mood.Calm;
   cooldown-=dt;
   if(allowChoice&&cooldown<=0&&(state.Finished||state.water>=35)&&routine.Current==CreatureRoutine.Task.SeekingWater){
    var choice=affordances.Where(a=>a!=null&&a.Available).OrderByDescending(Score).FirstOrDefault();
    if(choice==null||!TryBegin(choice))cooldown=4;
   }
   return Current!=Phase.Rest;
  }
  elapsed+=dt;clock+=dt;
  if(Current==Phase.StormSupply)return TickStormSupply(dt);
  if(Current==Phase.StormRescue)return TickStormRescue(dt);
  if(Current==Phase.PuzzleAssist)return TickPuzzle(dt);
  if(Current==Phase.SpinTrick){int outcome=target?.GetComponent<BeachBall>()?.Spin?.Step(dt)??-1;Affect=CreatureAffect.Mood.Excited;if(outcome!=0){if(outcome>0){played++;lastWater=lastSocial=false;boredom=0;feedbackWindow=12;}Finish(outcome>0);if(outcome>0)motion?.React();return false;}return true;}
  if(Current==Phase.ChaseBall){var ball=target!=null?target.GetComponent<BeachBall>():null;int outcome=ball?.Chase?.Step(navigator,routine.speed,dt)??-1;Affect=CreatureAffect.Mood.Excited;motion?.LookAt(target!=null?target.transform:null);if(outcome!=0){if(outcome>0){played++;lastWater=lastSocial=false;boredom=0;feedbackWindow=12;motion?.React();}Finish(outcome>0);return false;}return true;}
  if(target==null||(!owned&&target.Item.held)||elapsed>(target.GetComponentInParent<DiscoveryPoint>()!=null||target.GetComponent<VillageTributeProp>()!=null?120:fetching?120:target.GetComponent<ThrowableTree>()!=null?90:target.GetComponent<BeachBall>()!=null?60:24)){Finish(false);return false;}
  if(Current==Phase.EchoRhythm){int outcome=target.GetComponent<PlayerRhythm>().Tick(dt);Affect=CreatureAffect.Mood.Excited;if(outcome!=0){if(outcome>0){played++;target.Discovered();lastWater=lastSocial=false;boredom=0;feedbackWindow=12;}Finish(outcome>0);return false;}return true;}
  if(Current==Phase.RhythmGame){var rhythm=target.GetComponent<SharedRhythm>();int outcome=rhythm!=null?rhythm.Tick(dt):-1;Affect=CreatureAffect.Mood.Happy;if(outcome<0){Finish(false);return false;}if(outcome>0){played++;target.Discovered();lastWater=lastSocial=false;boredom=0;feedbackWindow=12;Finish(true);return false;}return true;}
  if(Current==Phase.RallyBall){var ball=target.GetComponent<BeachBall>();
   if(ball.Player==null||!ball.CreatureJoined){Finish(false);return false;}
   Affect=CreatureAffect.Mood.Happy;motion?.Signal(false);motion?.LookAt(ball.transform);
   if(ball.RallyReceptions>=4){played++;target.Discovered();lastWater=lastSocial=false;boredom=0;feedbackWindow=12;Finish(true);return false;}
   if(ball.CreatureTurn&&!ball.PassPending){if(ball.ApproachPass(navigator,routine.speed*.65f,dt,true)){motion?.Activity(5);ball.TryPass(true);navigator.ResetRoute();}}else{motion?.Activity(6);}
   if(navigator.Blocked&&elapsed>8)Finish(false);return Current!=Phase.Rest;
  }
  if(Current==Phase.Approach){
   var invited=target.GetComponent<BeachBall>();
   if(!fetching&&!bowlingPlay&&!goalPlay&&invited!=null&&!invited.Inviting&&!askedPlaymate){askedPlaymate=true;if(invited.TryInvitePlaymate())inviteGestureUntil=elapsed+2.5f;}
   if(invited!=null&&invited.Inviting){motion?.Signal(elapsed<inviteGestureUntil,true,invited.transform.position);
    if(invited.MeetingPoint(out var meet)){if(navigator.MoveTo(meet,routine.speed,dt)&&invited.TryJoinCreature()){Current=Phase.RallyBall;clock=0;}return true;}
    Finish(false);return false;
   }
   Affect=CreatureAffect.Mood.Curious;var destination=target.transform.position;var rhythm=target.GetComponent<SharedRhythm>();if(target.host!=null&&rhythm!=null&&!rhythm.ApproachPoint(GetComponent<CreatureHybrid>(),out destination)){Finish(false);return false;}
   if(navigator.MoveTo(destination,routine.speed,dt)){Current=Phase.Inspect;clock=0;motion?.Activity(6);}
   else if(navigator.Blocked){Affect=CreatureAffect.Mood.Sad;if(elapsed>5)Finish(false);}
  }else if(Current==Phase.Inspect){
   if(clock>1.8f){if(echoPlay){if(!target.GetComponent<PlayerRhythm>().Begin()){Finish(false);return false;}Current=Phase.EchoRhythm;clock=0;return true;}var rhythm=target.GetComponent<SharedRhythm>();if(target.host!=null&&rhythm!=null){if(!rhythm.Begin(GetComponent<CreatureHybrid>())){Finish(false);return false;}Current=Phase.RhythmGame;clock=0;return true;}if(fetching&&!target.GetComponent<BeachBall>().ClaimFetch(fetchTicket)){Finish(false);return false;}if(target.GetComponent<DiscoveryObject>()?.Owner!=null){Finish(false);return false;}var social=target.GetComponent<VillagerSocial>();if(social!=null&&!social.Willing){Finish(false);return false;}social?.ResetTask();owned=target.Item;if(owned.GetComponent<ThrowableTree>() is ThrowableTree rooted&&!rooted.Lift()){owned=null;Finish(false);return false;}original=owned.transform.position;originalRotation=owned.transform.rotation;owned.held=true;owned.GetComponent<Collider>().enabled=false;
    owned.GetComponent<ConstructionBlock>()?.Grab();
    owned.GetComponent<VillagerRoutine>()?.ResetTask();motion?.Activity(1);Current=Phase.Lift;clock=0;}
  }else if(Current==Phase.Lift){
   if(clock>.5f)motion?.Carry(owned.transform);
   if(clock>1.2f){clock=0;motion?.Activity(0);Affect=CreatureAffect.Mood.Excited;
    var block=owned.GetComponent<ConstructionBlock>();
    if(block!=null){
     if(!block.Site.TryPlan(block,out buildPoint,out buildRotation)){Finish(false);return false;}
     var direction=buildPoint-transform.position;direction.y=0;if(direction.sqrMagnitude<1e-9f)direction=transform.forward;
     buildApproach=block.Site.WorkApproach(transform.position,buildPoint,navigator.world.Scale*6);Current=Phase.BuildApproach;
    }else if(target.kind==CreatureAffordance.Kind.Villager){
     var away=transform.right*navigator.world.Scale*8;
     if(!SafeLanding(transform.position+away,out landing)&&!SafeLanding(original,out landing)){Finish(false);return false;}
     Current=Phase.CarryFriend;
    }else if(target.GetComponent<ThrowableTree>()!=null){Current=Phase.ThrowTree;}else if(target.GetComponent<BeachBall>()!=null){Current=fetching?Phase.FetchCarry:Phase.ThrowBall;}
    else {Current=Phase.Play;var sound=target.GetComponent<AudioSource>();if(IslandAudio.Current!=null){if(sound!=null)IslandAudio.Cue(IslandSound.Chime,target.transform.position,target.GetEntityId().GetHashCode());}else if(sound!=null)sound.Play();}
   }
  }else if(Current==Phase.CarryFriend){
   Affect=CreatureAffect.Mood.Happy;
   if(navigator.MoveTo(landing,routine.speed*.75f,dt)){Current=Phase.Place;clock=0;motion?.Activity(5);}
  }else if(Current==Phase.BuildApproach){
   Affect=CreatureAffect.Mood.Curious;var block=owned.GetComponent<ConstructionBlock>();block.Site.ShowPreview(block,buildPoint,buildRotation);
   if(navigator.MoveTo(buildApproach,routine.speed,dt)){buildStart=owned.transform.position;buildCorrection=state.Building.Practice.NeedsCorrection(state.Hybrid.seed,state.Building.Practice.placements);Current=Phase.BuildPlace;clock=0;motion?.Activity(5);}
  }else if(Current==Phase.BuildPlace){
   var skill=state.Building.Practice;float duration=skill.PlacementSeconds+(buildCorrection?.8f:0);Affect=buildCorrection&&clock>=skill.PlacementSeconds?CreatureAffect.Mood.Curious:CreatureAffect.Mood.Excited;float blend=Mathf.SmoothStep(0,1,clock/skill.PlacementSeconds);var aim=buildPoint+target.GetComponent<ConstructionBlock>().Site.transform.rotation*skill.Offset(clock,buildCorrection)*navigator.world.Scale;motion?.PlaceAt(aim,buildRotation,blend);owned.transform.SetPositionAndRotation(Vector3.Lerp(buildStart,aim,blend),Quaternion.Slerp(originalRotation,buildRotation,blend));
   if(clock>=duration){var block=owned.GetComponent<ConstructionBlock>();if(!block.Site.Drop(block,buildPoint,buildRotation,true)){Finish(false);return false;}skill.Confirm(buildCorrection);if(buildCorrection)alignmentCorrections++;lastWater=false;built++;played++;target.Discovered();NoticeDiscovery(target.GetComponentInParent<DiscoveryPoint>());boredom=0;feedbackWindow=12;lastSocial=false;owned=null;Finish(true);}
  }else if(Current==Phase.FetchCarry){var ball=target.GetComponent<BeachBall>();if(!ball.OwnsFetch(fetchTicket)){Finish(false);return false;}Affect=CreatureAffect.Mood.Happy;if(navigator.MoveTo(ball.FetchHome,routine.speed*.8f,dt)){Current=Phase.FetchPlace;clock=0;motion?.Activity(5);}}
  else if(Current==Phase.FetchPlace){var ball=target.GetComponent<BeachBall>();motion?.PlaceAt(ball.FetchHome,Quaternion.identity,Mathf.Clamp01(clock));if(clock>1.1f){motion?.Carry(null);if(!ball.CompleteFetch(fetchTicket)){Finish(false);return false;}owned=null;played++;target.Discovered();if(ball.Goals?.Turns.OffersCreature==true)target.cooldown=0;lastWater=lastSocial=false;feedbackWindow=12;boredom=0;Finish(true);}}
  else if(Current==Phase.ThrowTree){var tree=target.GetComponent<ThrowableTree>();Affect=CreatureAffect.Mood.Excited;var direction=tree.World.Direction(transform);transform.rotation=Quaternion.RotateTowards(transform.rotation,Quaternion.LookRotation(direction),dt*180);var release=transform.position+direction*navigator.world.Scale*8+Vector3.up*navigator.world.Scale*9;motion?.PlaceAt(release,Quaternion.identity,Mathf.Clamp01(clock/1.2f));if(clock>1.3f){owned.transform.position=release;motion?.Carry(null);motion?.PlaceAt(default,Quaternion.identity,0);if(!tree.World.Throw(tree,direction)){Finish(false);return false;}owned=null;Current=Phase.WatchTree;clock=0;motion?.Activity(6);}}
  else if(Current==Phase.WatchTree){var tree=target.GetComponent<ThrowableTree>();Affect=tree.World.Harmful?CreatureAffect.Mood.Sad:CreatureAffect.Mood.Excited;motion?.LookAt(tree.transform);if(!tree.InFlight){played++;target.Discovered();lastWater=lastSocial=false;boredom=0;feedbackWindow=12;Finish(true);}}
  else if(Current==Phase.ThrowBall){Affect=CreatureAffect.Mood.Excited;var ball=target.GetComponent<BeachBall>();if(goalPlay&&!ball.Goals.AimAt(goalTarget,transform.position,out _)){Finish(false);return false;}if(knockdownPlay&&!ball.TryExperimentalAim(transform.position,out _)){Finish(false);return false;}var direction=knockdownPlay&&ball.TryExperimentalAim(transform.position,out var inventedAim)?inventedAim:bowlingPlay?ball.Bowling.Aim(transform.position):goalPlay&&ball.Goals.AimAt(goalTarget,transform.position,out var goalAim)?goalAim:ball.Direction(transform);transform.rotation=Quaternion.RotateTowards(transform.rotation,Quaternion.LookRotation(direction),dt*240);var release=transform.position+direction*navigator.world.Scale*8+Vector3.up*navigator.world.Scale*9;motion?.PlaceAt(release,Quaternion.identity,Mathf.Clamp01(clock/.8f));if(knockdownPlay)motion?.Signal(state.worldPlay?.invented?.Discovered==true&&clock<1.6f,true,transform.position+direction*navigator.world.Scale*20);if(clock>(knockdownPlay?2.2f:.9f)){motion?.Signal(false);owned.transform.position=release;motion?.Carry(null);motion?.PlaceAt(default,Quaternion.identity,0);if(goalPlay)ball.Goals.Turns.Arm(goalTarget);if(knockdownPlay){if(!ball.RollInventedGame()){Finish(false);return false;}}else if(bowlingPlay){if(!ball.Bowling.TryCreatureRoll()){Finish(false);return false;}}else ball.Throw(direction);owned=null;Current=Phase.WatchBall;clock=0;motion?.Activity(6);}}
  else if(Current==Phase.WatchBall){var ball=target.GetComponent<BeachBall>();Affect=ball.Harmful?CreatureAffect.Mood.Sad:CreatureAffect.Mood.Excited;motion?.LookAt(ball.transform);if(clock>4.2f&&ball.Bowling?.Active!=true&&ball.Goals?.Active!=true){ball.FinishExperience();played++;target.Discovered();lastWater=lastSocial=false;boredom=0;feedbackWindow=12;Finish(true);}}
  else if(Current==Phase.Play){
   Affect=target.kind==CreatureAffordance.Kind.Food?CreatureAffect.Mood.Hungry:CreatureAffect.Mood.Excited;
   if(target.kind==CreatureAffordance.Kind.Food)motion?.Carry(owned.transform,true);
   else {motion?.Carry(owned.transform);motion?.Playful(clock);}
   if(clock>3.5f){Current=Phase.Place;clock=0;motion?.Carry(owned.transform);motion?.Activity(5);}
  }else if(Current==Phase.Place&&clock>1.1f){
   lastWater=false;
   if(target.kind==CreatureAffordance.Kind.Villager){friendsCarried++;lastSocial=true;lastFriend=target.GetComponent<VillagerSocial>();if(lastFriend!=null){lastFriend.RememberRide();foreach(var witness in lastFriend.transform.parent.GetComponentsInChildren<VillagerSocial>())if(witness!=lastFriend&&!witness.GetComponent<CarryItem>().held&&Vector3.Distance(witness.transform.position,transform.position)<navigator.world.Scale*22)witness.WitnessRide();}}
   else {played++;lastSocial=false;if(target.kind==CreatureAffordance.Kind.Food){state.hunger=.05f;target.GetComponent<DiscoveryObject>()?.Eat();target.GetComponent<VillageTributeProp>()?.owner.Eat();}}
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
 public void NoticeAttention(){NoticeResponse(CreatureAffect.Mood.Curious,1.5f);}
 public void NoticeResponse(CreatureAffect.Mood mood,float duration){if(responseProtection>0)return;reaction=duration;feedbackMood=mood;}
 public void NoticeDisassembly(Transform villager){villager.GetComponent<VillagerSocial>()?.RememberDispute();emotion=4;recentMood=CreatureAffect.Mood.Sad;Affect=recentMood;motion?.LookAt(villager);}
 public void NoticeBadPlay(Transform harmed){responseProtection=3;reaction=3;feedbackMood=recentMood=CreatureAffect.Mood.Sad;emotion=5;motion?.LookAt(harmed);}
 public void RememberWaterDelivery(SliceState season){state=season;lastWater=true;lastSocial=false;feedbackWindow=12;}
 public void RememberObservation(SliceState season){state=season;feedbackWindow=0;}
 public void InvalidateFeedback(){feedbackWindow=0;}
 // The coordinator's typed receipt is authoritative, including after reload.
 internal void PrepareFeedback(ActionReceipt receipt,SliceState season,VillagerSocial friend){state=season;lastWater=receipt.action==CreatureAction.WaterDelivery;lastSocial=receipt.action==CreatureAction.SocialInteraction||(receipt.action==CreatureAction.RespondToGuidance&&friend!=null);lastFriend=lastSocial?friend:null;feedbackWindow=12;}
 public bool Feedback(bool positive,bool allowLearning=true){if(state==null||state.paused)return false;bool learned=allowLearning&&feedbackWindow>0;if(learned){float change=positive?.12f:-.12f;if(lastWater)state.care=Mathf.Clamp01(state.care+change);else if(lastSocial){state.socialPreference=Mathf.Clamp01(state.socialPreference+change);lastFriend?.Feedback(positive);}else state.playPreference=Mathf.Clamp01(state.playPreference+change);feedbackWindow=0;}responseProtection=2;reaction=2;emotion=5;feedbackMood=recentMood=positive?CreatureAffect.Mood.Happy:CreatureAffect.Mood.Sad;Affect=recentMood;return learned;}
 public void ResetTask(){Finish(false);lastFriend=null;lastWater=false;reaction=responseProtection=0;constructionFocus=false;cooldown=7;boredom=emotion=feedbackWindow=0;Affect=CreatureAffect.Mood.Calm;}
 void Finish(bool success){
  var finished=target;if(Current==Phase.StormSupply){supplyChapter?.ReleaseSupply();supplyChapter=null;}if(Current==Phase.StormRescue){rescueChapter?.ReleaseCreature();rescueChapter=null;}if(Current==Phase.PuzzleAssist)finished?.GetComponent<PuzzlePad>()?.puzzle.Cancel();if(Current==Phase.SpinTrick)finished?.GetComponent<BeachBall>()?.Spin?.End();motion?.TurnInPlace(false);if(!success){finished?.GetComponent<BeachBall>()?.Goals?.Turns.Cancel("Creature activity interrupted");finished?.GetComponent<BeachBall>()?.Goals?.Cancel();}knockdownPlay=false;goalPlay=false;goalTarget=null;if(echoPlay)finished?.GetComponent<PlayerRhythm>()?.Cancel(success);echoPlay=false;if(Current==Phase.ChaseBall)finished?.GetComponent<BeachBall>()?.Chase?.End();if(fetching)finished?.GetComponent<BeachBall>()?.CancelFetch(fetchTicket);fetching=false;
  if(Current==Phase.RallyBall)finished?.GetComponent<BeachBall>()?.CancelRally();
  if(!success)finished?.GetComponent<SharedRhythm>()?.Cancel();
  motion?.Touch(null);motion?.Signal(false);motion?.Carry(null);motion?.Playful(0);motion?.PlaceAt(default,Quaternion.identity,0);motion?.LookAt(null);motion?.Activity(0);navigator?.ResetRoute();
  if(owned!=null){var item=owned;var point=original;
   var tree=item.GetComponent<ThrowableTree>();if(tree!=null){tree.CancelCarry();owned=null;}
   else {var block=item.GetComponent<ConstructionBlock>();if(block!=null){block.Site.HidePreview();block.CancelHold();owned=null;}
   else {
   if(success&&target!=null&&target.kind==CreatureAffordance.Kind.Villager&&SafeLanding(landing,out var safe))point=safe;
   else if(success&&target!=null&&target.kind==CreatureAffordance.Kind.Roll){var at=original+transform.forward*navigator.world.Scale*4;if(navigator.world.Sample(at,navigator.world.villager,4,out var roll))point=roll+Vector3.up*navigator.world.Scale;}
   if(navigator.world.Terrain!=null&&item.kind==Offering.Villager)point=navigator.world.Terrain.Project(point);
   item.transform.position=point;item.transform.rotation=originalRotation;item.held=false;item.GetComponent<Collider>().enabled=true;
   if(item.kind==Offering.Villager){item.home=point;item.GetComponent<IslandNavigator>()?.ResetRoute();item.GetComponent<VillagerRoutine>()?.ResetTask();item.GetComponent<HumanoidMotion>()?.React();}
   }
  }
  }
  if(!success&&target!=null)target.SetCooldown(12);
  owned=null;target=null;Current=Phase.Rest;clock=elapsed=0;cooldown=success?(constructionFocus?3:8):4;emotion=success?5:3;recentMood=success?CreatureAffect.Mood.Happy:CreatureAffect.Mood.Sad;Affect=recentMood;
  if(finished!=null){var host=finished.host;if(success)Completed?.Invoke(finished);else Failed?.Invoke("Interaction interrupted or unavailable");host?.SharedOutcome(finished,success);}
 }
 void OnDisable(){Finish(false);}
}
}
