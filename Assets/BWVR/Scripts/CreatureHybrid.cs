using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
namespace BWVR {
// One owner delegates to existing executors. Perception, guidance and learning are
// small services here; none introduces a parallel animation/movement controller.
public sealed class CreatureHybrid:MonoBehaviour {
 public enum ActionOwner{None,Water,Curiosity,Observe}
 public ActionOwner Owner{get;private set;}public string Explanation{get;private set;}="Watching";
 public readonly WorldEventJournal Journal=new WorldEventJournal();
 public readonly List<HybridTarget> Targets=new List<HybridTarget>();
 public void RegisterObject(CreatureAffordance a,string id){var target=HybridTarget.Bind(a.gameObject,id,HybridTargetKind.Object);Register(target);a.simulationTime=()=>State.time;a.cooldown=State.Cooldown(id);a.rememberCooldown=until=>State.RememberCooldown(id,until);}
 readonly Dictionary<string,HybridTarget> byId=new Dictionary<string,HybridTarget>();readonly List<CreatureCandidate> candidates=new List<CreatureCandidate>();
 Func<SliceState> season;CreatureCuriosity curiosity;CreatureRoutine water;IslandNavigator nav;HumanoidMotion motion;HybridTarget observing;float nextPerception,nextDecision,observeClock;bool suppress,suspended,resume;
 public SliceState Season=>season();public HybridState State=>Season.Hybrid;
 public void Initialize(CreatureRoutine routine,CreatureCuriosity mind,HumanoidMotion visual,CreatureAffordance[] objects,VillagerRoutine[] villagers,GameObject cistern,CarryItem offering,Func<SliceState> current){
  season=current;water=routine;curiosity=mind;motion=visual;nav=GetComponent<IslandNavigator>();
  for(int i=0;i<villagers.Length;i++)Register(HybridTarget.Bind(villagers[i].gameObject,"villager-"+i,HybridTargetKind.Villager));
  for(int i=0;i<objects.Length;i++){var a=objects[i];var t=a.GetComponent<HybridTarget>()??HybridTarget.Bind(a.gameObject,"object-"+i,HybridTargetKind.Object);Register(t);a.simulationTime=()=>State.time;string id=t.id;a.cooldown=State.Cooldown(id);a.rememberCooldown=until=>State.RememberCooldown(id,until);}
  Register(HybridTarget.Bind(cistern,"cistern",HybridTargetKind.Cistern));Register(HybridTarget.Bind(offering.gameObject,"water",HybridTargetKind.Water));
  if(cistern.GetComponent<Collider>()==null){var renderers=cistern.GetComponentsInChildren<Renderer>();if(renderers.Length>0){var bounds=renderers[0].bounds;foreach(var r in renderers)bounds.Encapsulate(r.bounds);var box=cistern.AddComponent<BoxCollider>();box.center=cistern.transform.InverseTransformPoint(bounds.center);box.size=bounds.size;}}
  var cue=cistern.GetComponent<SpatialTarget>()??cistern.AddComponent<SpatialTarget>();cue.invoke=()=>Guide(GuidanceIntent.Help,Find("cistern"));
  var waterCue=offering.GetComponent<SpatialTarget>()??offering.gameObject.AddComponent<SpatialTarget>();waterCue.invoke=()=>Guide(GuidanceIntent.Help,Find("water"));
  curiosity.Completed+=CuriosityCompleted;curiosity.Failed+=CuriosityFailed;water.Completed+=WaterCompleted;resume=State.intention.active;
 }
 public void Register(HybridTarget target){if(byId.ContainsKey(target.id))return;byId.Add(target.id,target);Targets.Add(target);}
 public HybridTarget Find(string id)=>id!=null&&byId.TryGetValue(id,out var t)?t:null;
 public bool CanReach(HybridTarget target){if(target==null||!target.gameObject.activeInHierarchy)return false;return CanReach(target.transform.position);}
 public bool CanReach(Vector3 at){if(!nav.world.Sample(transform.position,nav.profile,12,out var start)||!nav.world.Sample(at,nav.profile,12,out var end))return false;var path=new NavMeshPath();return NavMesh.CalculatePath(nav.world.ToNav(start),nav.world.ToNav(end),new NavMeshQueryFilter{agentTypeID=nav.profile.id,areaMask=1},path)&&path.status==NavMeshPathStatus.PathComplete;}
 public bool Guide(GuidanceIntent intent,HybridTarget target){
  if(Season.paused||suspended)return false;var g=State.guidance;if(g.status==GuidanceStatus.Active){Journal.Add(State.time,"guidance superseded",CreatureAction.RespondToGuidance,g.target,"New request");if(Owner==ActionOwner.Observe)Interrupt("Guidance superseded");}
  g=State.guidance=new GuidanceState{intention=intent,target=target!=null?target.id:"",requestedAt=State.time,expiresAt=State.time+30,status=GuidanceStatus.Active};
  if(target==null){Reject(GuidanceStatus.Rejected,"Target unavailable");return false;}
  if(Owner==ActionOwner.Water&&Season.water<25&&intent!=GuidanceIntent.Help){Reject(GuidanceStatus.Rejected,"Occupied with urgent water assistance");return false;}
  if(target.GetComponent<CarryItem>()?.held==true){Reject(GuidanceStatus.Rejected,"Target unavailable while carried");return false;}
  if(intent==GuidanceIntent.Protect&&(target.villager==null||target.villager.Memory.injury<=0)){Reject(GuidanceStatus.Rejected,"No credible vulnerability to protect");return false;}
  if(intent==GuidanceIntent.Help&&target.kind==HybridTargetKind.Object&&target.villager==null){Reject(GuidanceStatus.Rejected,"No relevant help capability");return false;}
  if(!CanReach(target)){Reject(GuidanceStatus.Rejected,"Target unreachable");return false;}
  if(intent==GuidanceIntent.Help&&(target.kind==HybridTargetKind.Cistern||target.kind==HybridTargetKind.Water)&&Season.water>=85){Reject(GuidanceStatus.Rejected,"Water problem no longer exists");return false;}
  motion?.LookAt(target.transform);curiosity.NoticeGreeting(target.transform);Journal.Add(State.time,"guidance",CreatureAction.RespondToGuidance,g.target,intent.ToString());nextPerception=nextDecision=0;return true;
 }
 void Reject(GuidanceStatus status,string reason){var g=State.guidance;g.status=status;g.reason=reason;Explanation=reason;Journal.Add(State.time,"guidance "+status,CreatureAction.RespondToGuidance,g.target,reason);}
 public void Tick(float dt,bool advanceClock=true){
  if(Season.paused||suspended||dt<=0||float.IsNaN(dt)||float.IsInfinity(dt))return;if(advanceClock)State.time=Mathf.Min(100000000,State.time+dt);
  if(State.guidance.status==GuidanceStatus.Active&&State.time>=State.guidance.expiresAt)Reject(GuidanceStatus.Expired,"Request expired");
  if(State.guidance.status==GuidanceStatus.Active&&Find(State.guidance.target)==null)Reject(GuidanceStatus.Abandoned,"Target unavailable");
  if(State.guidance.status==GuidanceStatus.Active){var g=State.guidance;var target=Find(g.target);bool solved=(g.intention==GuidanceIntent.Protect&&target?.villager!=null&&target.villager.Memory.injury<=0)||(g.intention==GuidanceIntent.Help&&(g.target=="cistern"||g.target=="water")&&Season.water>=85);if(solved){if(Owner==ActionOwner.Observe&&State.intention.target==g.target)Interrupt("Problem no longer exists");Reject(GuidanceStatus.Abandoned,"Problem no longer exists");}}
  if(Owner!=ActionOwner.None&&State.time>State.intention.deadline)Interrupt("Action timed out");
  if(!Season.Finished&&Season.water<18&&(Owner==ActionOwner.Curiosity||Owner==ActionOwner.Observe)&&water.WaterAvailable)Interrupt("Critical water shortage supersedes leisure");
  if(Owner==ActionOwner.None&&State.time>=nextDecision){nextDecision=State.time+.5f;if(State.time>=nextPerception){Perceive();nextPerception=State.time+.25f;}
   CreatureCandidate choice=null;if(resume){resume=false;var prior=State.intention;foreach(var c in candidates)if(c.action==prior.action&&c.target==prior.target&&c.feasible&&prior.deadline>State.time){choice=c;c.timeout=Mathf.Max(1,prior.deadline-State.time);break;}State.intention.active=false;if(choice==null)Journal.Add(State.time,"resume abandoned",prior.action,prior.target,"Saved intention no longer feasible");}
   for(int tries=0;tries<4;tries++){if(choice==null)choice=CreatureDecisionModel.Select(candidates,State.seed,State.decisions);if(choice==null)break;var target=Find(choice.target);if(choice.action==CreatureAction.IdleOrObserve||CanReach(target)){BeginAction(choice,target);break;}choice.feasible=false;if(State.guidance.status==GuidanceStatus.Active&&State.guidance.target==choice.target)Reject(GuidanceStatus.Rejected,"Target unreachable");choice=null;}
   State.decisions=Math.Min(100000000,State.decisions+1);
  }
  // Tick curiosity's needs/affect even during water work, but disable its separate selector.
  curiosity.Tick(dt,Season,false);
  if(Owner==ActionOwner.Water)water.Tick(dt,false);
  else if(Owner==ActionOwner.Observe){if(observing==null||!observing.gameObject.activeInHierarchy||observing.GetComponent<CarryItem>()?.held==true){Interrupt("Observed target unavailable");return;}if(nav.MoveTo(observing.transform.position,water.speed,dt)){motion?.Activity(6);observeClock+=dt;if(observeClock>=2){if(observing.villager!=null){var m=observing.villager.Memory;m.trust=Mathf.Clamp01(m.trust+.04f);m.lastAssistance=State.time;observing.GetComponent<HumanoidMotion>()?.React();}curiosity.NoticeGreeting(observing.transform);Complete();}}else if(nav.Blocked&&State.time-State.intention.startedAt>5)Interrupt("Target unreachable");}
 }
 public void Perceive(){
  candidates.Clear();float shortage=Season.Finished?0:Mathf.Clamp01((65-Season.water)/45);
  Add(new CreatureCandidate{action=CreatureAction.WaterDelivery,situation=Season.water<65?CreatureSituation.WaterShortage:CreatureSituation.Calm,target="water",feasible=!Season.Finished&&water.WaterAvailable,urgency=shortage*2.5f,need=Season.care*.65f,timeout=180,expected="Village water increases"},Find("water"));
  foreach(var t in Targets){var inspiration=t.GetComponent<ConstructionInspiration>();if(inspiration!=null){if(!Season.Building.Knows(inspiration.recipe))Add(new CreatureCandidate{action=CreatureAction.InvestigateObject,situation=CreatureSituation.ConstructionOpportunity,target=t.id,need=.4f,novelty=.9f,timeout=120,expected="Observe a building idea"},t);continue;}var a=t.affordance;if(a==null||!a.Available)continue;float distance=Vector3.Distance(transform.position,t.transform.position)/nav.world.Scale;if(distance>350)continue;
   var action=a.kind==CreatureAffordance.Kind.Food?CreatureAction.EatFood:a.kind==CreatureAffordance.Kind.Villager?CreatureAction.SocialInteraction:a.GetComponent<ConstructionBlock>()!=null?CreatureAction.ConstructionPlay:CreatureAction.PlayWithToy;
   var situation=action==CreatureAction.EatFood?CreatureSituation.Hungry:action==CreatureAction.SocialInteraction?CreatureSituation.SafeContact:action==CreatureAction.ConstructionPlay?CreatureSituation.ConstructionOpportunity:CreatureSituation.PlayOpportunity;
   var block=a.GetComponent<ConstructionBlock>();
   Add(new CreatureCandidate{action=action,situation=situation,target=t.id,feasible=!(shortage>=1&&action!=CreatureAction.EatFood),urgency=block!=null&&block.Site.Snapped>0&&!block.Site.RecipeComplete?.45f:0,need=action==CreatureAction.EatFood?Season.hunger:action==CreatureAction.SocialInteraction?Season.socialPreference*.65f:Season.playPreference*.65f,affinity=t.villager!=null?t.villager.Attraction*.35f:0,novelty=.5f/(1+a.discoveries),timeout=block?.Site.IsBridge==true||a.GetComponentInParent<DiscoveryPoint>()!=null?180:30,expected="Safe interaction"},t);
  }
   var g=State.guidance;if(g.status==GuidanceStatus.Active){var t=Find(g.target);if(t!=null){if(g.intention==GuidanceIntent.Investigate||g.intention==GuidanceIntent.Protect||(g.intention==GuidanceIntent.Help&&t.villager!=null))Add(new CreatureCandidate{action=g.intention==GuidanceIntent.Investigate?CreatureAction.InvestigateObject:CreatureAction.RespondToGuidance,situation=t.GetComponent<ConstructionInspiration>()!=null?CreatureSituation.ConstructionOpportunity:g.intention==GuidanceIntent.Investigate&&t.villager==null?CreatureSituation.PlayOpportunity:CreatureSituation.HelpRequested,target=t.id,need=.3f,guidance=.95f,timeout=90,expected=g.intention==GuidanceIntent.Protect?"Attend a vulnerable villager without moving them":"Inspect target"},t);}}
  candidates.Add(new CreatureCandidate{action=CreatureAction.IdleOrObserve,situation=CreatureSituation.Calm,target="",need=.1f,timeout=2,expected="Observe"});
 }
 void Add(CreatureCandidate c,HybridTarget t){c.feasible&=t!=null&&t.gameObject.activeInHierarchy;c.learned=State.Preference(c.situation,c.action)*.9f;c.travel=t!=null?Mathf.Clamp01(Vector3.Distance(transform.position,t.transform.position)/(nav.world.Scale*350))*.55f:0;var g=State.guidance;if(g.status==GuidanceStatus.Active&&(c.target==g.target||(c.action==CreatureAction.WaterDelivery&&g.intention==GuidanceIntent.Help&&(g.target=="cistern"||g.target=="water"||Find(g.target)?.villager?.GetComponent<VillagerRoutine>()?.role==VillagerRoutine.Role.WaterGatherer))))c.guidance=.95f;candidates.Add(c);}
 void BeginAction(CreatureCandidate c,HybridTarget target){
  if(c.action==CreatureAction.IdleOrObserve){Explanation="No feasible activity is more important right now";return;}
  if(c.action==CreatureAction.WaterDelivery){Owner=ActionOwner.Water;water.ResetTask();}
  else if(c.action==CreatureAction.RespondToGuidance||c.action==CreatureAction.InvestigateObject){Owner=ActionOwner.Observe;observing=target;observeClock=0;nav.ResetRoute();motion?.LookAt(target.transform);}
  else {if(!curiosity.TryBegin(target.affordance)){nextPerception=0;return;}Owner=ActionOwner.Curiosity;}
  State.intention=new CreatureIntention{active=true,action=c.action,situation=c.situation,target=c.target,startedAt=State.time,deadline=State.time+c.timeout};Explanation=c.action+": "+(c.urgency>1?"water shortage":c.guidance>0?"your cue":c.need>.75f?"hunger":c.novelty>.3f?"unfamiliar opportunity":"learned preference");Journal.Add(State.time,"start",c.action,c.target,c.Explain);
 }
 void CuriosityCompleted(CreatureAffordance target){if(!suppress&&Owner==ActionOwner.Curiosity){GetComponent<CreatureConstructionLearning>()?.Placed(target.GetComponent<ConstructionBlock>());Complete();}}
 void CuriosityFailed(string reason){if(!suppress&&Owner==ActionOwner.Curiosity)Interrupt(reason);}
 void WaterCompleted(){if(!suppress&&Owner==ActionOwner.Water)Complete();}
 void Complete(){var intent=State.intention;var block=Find(intent.target)?.affordance?.GetComponent<ConstructionBlock>();State.receipt=new ActionReceipt{eligible=true,action=intent.action,situation=intent.situation,target=intent.target,completedAt=State.time,constructionRecipe=block!=null?(int)block.Site.Recipe:-1};Journal.Add(State.time,"complete",intent.action,intent.target,"Outcome confirmed");
  if(Owner==ActionOwner.Observe&&observing!=null)GetComponent<CreatureConstructionLearning>()?.Observe(observing.GetComponent<ConstructionInspiration>());
  if(Owner==ActionOwner.Observe)curiosity.RememberObservation(Season);
  if((intent.action==CreatureAction.WaterDelivery||intent.action==CreatureAction.SocialInteraction||(intent.action==CreatureAction.RespondToGuidance&&Find(intent.target)?.villager!=null))&&State.time-State.lastBenefit>=8){State.energy=Mathf.Min(100,State.energy+1);State.lastBenefit=State.time;}
  var g=State.guidance;if(g.status==GuidanceStatus.Active&&(g.target==intent.target||(intent.action==CreatureAction.WaterDelivery&&g.intention==GuidanceIntent.Help)))g.status=GuidanceStatus.Completed;
  intent.active=false;Owner=ActionOwner.None;observing=null;motion?.LookAt(null);motion?.Activity(0);nav.ResetRoute();nextDecision=State.time+.5f;nextPerception=0;
 }
 public bool Feedback(bool positive){if(Season.paused||suspended)return false;bool learned=State.Reinforce(positive);curiosity.Feedback(positive,learned);if(learned&&State.receipt.action==CreatureAction.ConstructionPlay&&State.receipt.constructionRecipe>=0)GetComponent<CreatureConstructionLearning>()?.Reinforce((BuildRecipe)State.receipt.constructionRecipe,positive);Journal.Add(State.time,learned?"reinforced":"feedback without lesson",State.receipt.action,State.receipt.target,positive?"Praise":"Discourage");return learned;}
 public void InvalidateLesson(string reason){State.receipt.eligible=false;curiosity.InvalidateFeedback();Journal.Add(State.time,"lesson cleared",State.receipt.action,State.receipt.target,reason);}
 public void Interrupt(string reason,bool keepIntention=false){if(season==null)return;var prior=State.intention;suppress=true;curiosity.ResetTask();water.ResetTask();suppress=false;motion?.LookAt(null);motion?.Activity(0);nav.ResetRoute();observing=null;Owner=ActionOwner.None;State.receipt.eligible=false;prior.active=keepIntention&&prior.active;resume=prior.active;Journal.Add(State.time,"interrupted",prior.action,prior.target,reason);Explanation=reason;nextDecision=State.time+.5f;nextPerception=0;}
 public void Suspend(bool value){if(value&&!suspended)Interrupt("Application suspended",true);suspended=value;}
 public void NewSeason(){Interrupt("New season");resume=false;nextDecision=nextPerception=0;foreach(var t in Targets)if(t.affordance!=null)t.affordance.cooldown=0;}
 void OnDisable(){if(season!=null)Interrupt("Creature disabled");}
 void OnDestroy(){if(curiosity!=null){curiosity.Completed-=CuriosityCompleted;curiosity.Failed-=CuriosityFailed;}if(water!=null)water.Completed-=WaterCompleted;}
}
}
