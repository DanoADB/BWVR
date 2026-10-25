using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace BWVR.Editor {
public static class CreatureSkillsChecks {
 [MenuItem("BWVR/Verify practiced building and competing desires (Play mode)")]
 public static void Verify(){
  if(!Application.isPlaying)throw new Exception("Enter Play mode");
  VerifyDecisions();
  var slice=UnityEngine.Object.FindAnyObjectByType<QuestSlice>();var god=slice.GetComponentInChildren<CreatureHybrid>();var learning=god.GetComponent<CreatureConstructionLearning>();var mind=god.GetComponent<CreatureCuriosity>();var nav=god.GetComponent<IslandNavigator>();var site=learning.sites[0];
  var field=typeof(QuestSlice).GetField("state",BindingFlags.Instance|BindingFlags.NonPublic);var original=(SliceState)field.GetValue(slice);var position=god.transform.position;var records=site.Capture();var recipe=site.Recipe;
  god.Interrupt("Skills QA");var test=new SliceState{developmentFocus=true,seconds=0,hunger=.1f};test.Building.observed=15;field.SetValue(slice,test);
  try{
   float Build(float control,bool cancel=false){
    god.Interrupt("Stage building skill");site.ResetConstruction();site.Recipe=BuildRecipe.Bench;test.Building.Practice.control=control;
    var brick=site.blocks.First(b=>b.CanBuild);var prop=brick.GetComponent<CreatureAffordance>();prop.cooldown=0;
    if(!nav.world.Sample(brick.transform.position,nav.profile,15,out var at))throw new Exception("No pickup route");god.transform.position=nav.world.ProjectFoot(at,nav.profile);nav.ResetRoute();
    if(!mind.TryBegin(prop))throw new Exception("Build refused");float placement=0;int before=test.Building.Practice.placements;
    for(int i=0;i<1800&&mind.Current!=CreatureCuriosity.Phase.Rest;i++){
     if(mind.Current==CreatureCuriosity.Phase.BuildPlace){placement+=.05f;if(cancel){god.Interrupt("Cancel placement");break;}}
     mind.Tick(.05f,test,false);site.Advance(.02f);
    }
    if(cancel){if(test.Building.Practice.placements!=before||brick.Item.held||brick.Attached)throw new Exception("Canceled placement earned practice or leaked ownership");}
    else if(!brick.Attached||test.Building.Practice.placements!=before+1)throw new Exception("Native placement did not snap and earn exactly one practice");
    return placement;
   }
   while(!test.Building.Practice.NeedsCorrection(test.Hybrid.seed,0))test.Hybrid.seed++;
   float novice=Build(.15f);if(test.Building.Practice.corrections!=1)throw new Exception("Beginner did not correct alignment");float expert=Build(1);if(novice<expert+.8f||test.Building.Practice.corrections!=1)throw new Exception("Practice did not improve native placement time/corrections");Build(.15f,true);
   god.Interrupt("Pause needs");var needs=test.Hybrid.Motivation;float boredom=needs.boredom;test.paused=true;god.Tick(20);if(needs.boredom!=boredom)throw new Exception("Paused needs advanced");test.paused=false;
   var carried=god.GetComponent<CarryItem>();carried.held=true;god.Tick(20);carried.held=false;if(needs.boredom!=boredom)throw new Exception("Carried creature's needs advanced");
   var saved=JsonUtility.FromJson<SliceState>(JsonUtility.ToJson(test));if(!saved.Valid()||saved.Building.Practice.placements!=2||saved.Building.Practice.corrections!=1)throw new Exception("Skills/needs save failed");saved.Hybrid.Motivation.boredom=float.NaN;if(saved.Valid())throw new Exception("Invalid needs accepted");saved=JsonUtility.FromJson<SliceState>(JsonUtility.ToJson(test));saved.Building.Practice.control=float.PositiveInfinity;if(saved.Valid())throw new Exception("Invalid skill accepted");
   var legacy=JsonUtility.FromJson<SliceState>("{\"version\":1,\"seconds\":180,\"water\":52,\"crops\":66}");if(!legacy.Valid()||!legacy.Hybrid.Motivation.Valid()||!legacy.Building.Practice.Valid())throw new Exception("Old save defaults failed");
   VerifyAttention(god,site,test);
   Debug.Log($"BWVR_SKILLS_PASS: native novice {novice:F2}s vs expert {expert:F2}s; corrected then snapped; cancellation earns no practice; needs/skill persistence, old defaults, invalid rejection, pause/carry gates; seeded motivation choices");
  }finally{god.GetComponent<CarryItem>().held=false;god.Interrupt("Skills QA done");site.Recipe=recipe;site.Restore(records);field.SetValue(slice,original);god.transform.position=position;nav.ResetRoute();}
 }
 static void VerifyDecisions(){
  var dexterity=new BuildingPractice();float before=dexterity.Wobble;int initialCorrections=Enumerable.Range(0,1000).Count(n=>dexterity.NeedsCorrection(17,n));for(int i=0;i<35;i++)dexterity.Confirm(false);
  if(dexterity.control<.9f||dexterity.Wobble>=before*.25f||Enumerable.Range(0,1000).Count(n=>dexterity.NeedsCorrection(17,n))>=initialCorrections/4)throw new Exception("Repeated practice did not improve precision and correction frequency");
  var needs=new CreatureMotivation{boredom=0,curiosity=0,companionship=0};
  CreatureCandidate Candidate(CreatureAction action,bool shared=false,float novelty=0,float hunger=.1f)=>new CreatureCandidate{action=action,target=action.ToString(),need=.3f,motivation=needs.Bias(action,novelty,hunger,shared)};
  needs.companionship=1;var social=Candidate(CreatureAction.SocialInteraction,true);var build=Candidate(CreatureAction.ConstructionPlay);
  if(CreatureDecisionModel.Select(new[]{social,build},7,0)!=social)throw new Exception("Companionship did not favor social contact");
  needs.companionship=0;needs.boredom=1;build=Candidate(CreatureAction.ConstructionPlay);social=Candidate(CreatureAction.SocialInteraction,true);if(CreatureDecisionModel.Select(new[]{social,build},7,0)!=build)throw new Exception("Boredom did not favor play");
  needs.Complete(CreatureAction.ConstructionPlay,false);var repeated=Candidate(CreatureAction.ConstructionPlay);var fresh=Candidate(CreatureAction.PlayWithToy);if(CreatureDecisionModel.Select(new[]{repeated,fresh},7,0)!=fresh)throw new Exception("Repetition did not encourage variety");
  needs.curiosity=1;var observed=Candidate(CreatureAction.InvestigateObject,false,1);var familiar=Candidate(CreatureAction.ConstructionPlay);if(CreatureDecisionModel.Select(new[]{familiar,observed},7,0)!=observed)throw new Exception("Curiosity did not favor an unfamiliar opportunity");
  var food=Candidate(CreatureAction.EatFood,false,0,.95f);if(CreatureDecisionModel.Select(new[]{familiar,food},7,0)!=food)throw new Exception("Hunger did not favor food");food.feasible=false;if(CreatureDecisionModel.Select(new[]{familiar,food},7,0)==food)throw new Exception("Needs bypassed feasibility");
  float satiation=needs.satiation[(int)CreatureAction.ConstructionPlay];needs.Advance(30);if(needs.satiation[(int)CreatureAction.ConstructionPlay]>=satiation)throw new Exception("Repetition never recovered");
 }
 static void VerifyAttention(CreatureHybrid god,ConstructionSite site,SliceState test){
  god.Interrupt("Placement attention QA");foreach(var resident in god.transform.parent.GetComponentsInChildren<VillagerSocial>())resident.ResetTask();
  var prop=god.Targets.First(t=>t.GetComponent<SharedRhythm>()!=null);var item=prop.GetComponent<CarryItem>();var original=item.transform.position;var rotation=god.transform.rotation;var nav=god.GetComponent<IslandNavigator>();GameObject blocker=null;
  var food=god.Targets.First(t=>t.affordance?.kind==CreatureAffordance.Kind.Food&&t.GetComponent<DiscoveryObject>()==null&&t.GetComponent<VillageTributeProp>()==null);var foodPosition=food.transform.position;float foodCooldown=food.affordance.cooldown;
  try{
   var from=site.transform.position+Vector3.forward*nav.world.Scale*18;if(!nav.world.Sample(from,nav.profile,20,out var safe))throw new Exception("No attention stage");safe=nav.world.ProjectFoot(safe,nav.profile);god.transform.position=safe;god.transform.rotation=Quaternion.LookRotation(site.transform.position-safe);
   item.transform.position=safe+god.transform.forward*nav.world.Scale*10+Vector3.up*nav.world.Scale;item.held=false;prop.affordance.cooldown=0;Physics.SyncTransforms();
   if(!god.NoticePlaced(item)||god.GetComponent<CreatureCuriosity>().Affect!=CreatureAffect.Mood.Curious)throw new Exception("Visible frontal placement did not attract attention");
   god.Perceive();var candidates=(System.Collections.Generic.List<CreatureCandidate>)typeof(CreatureHybrid).GetField("candidates",BindingFlags.NonPublic|BindingFlags.Instance).GetValue(god);
   if(!candidates.Any(c=>c.target==prop.id&&c.attention>.8f))throw new Exception("Placement did not bias native utility");
   test.Hybrid.time+=1;item.held=true;if(god.NoticePlaced(item))throw new Exception("Held object attracted pickup attention");item.held=false;item.transform.position=safe-god.transform.forward*nav.world.Scale*10;Physics.SyncTransforms();if(god.NoticePlaced(item))throw new Exception("Rear placement noticed");
   item.transform.position=safe+god.transform.forward*nav.world.Scale*10+Vector3.up*nav.world.Scale;
   blocker=new GameObject("Temporary attention occluder");blocker.transform.position=(safe+Vector3.up*nav.world.Scale*10+item.transform.position)*.5f;blocker.AddComponent<BoxCollider>().size=Vector3.one*nav.world.Scale*6;Physics.SyncTransforms();if(god.NoticePlaced(item))throw new Exception("Occluded placement noticed");UnityEngine.Object.DestroyImmediate(blocker);blocker=null;
   test.Hybrid.time+=13;god.Perceive();if(candidates.Any(c=>c.target==prop.id&&c.attention>0))throw new Exception("Placement bias did not expire");
   test.paused=true;if(god.NoticePlaced(item))throw new Exception("Paused placement noticed");test.paused=false;
   // A real leisure executor must relinquish its brick for urgent, reachable food.
   site.ResetConstruction();site.Recipe=BuildRecipe.Bench;var brick=site.blocks.First(b=>b.CanBuild);brick.GetComponent<CreatureAffordance>().cooldown=0;var target=brick.GetComponent<HybridTarget>();
   typeof(CreatureHybrid).GetMethod("BeginAction",BindingFlags.NonPublic|BindingFlags.Instance).Invoke(god,new object[]{new CreatureCandidate{action=CreatureAction.ConstructionPlay,target=target.id,timeout=120},target});
   if(god.Owner!=CreatureHybrid.ActionOwner.Curiosity)throw new Exception("Leisure did not start");if(!nav.world.Sample(safe+god.transform.forward*nav.world.Scale*4,nav.profile,8,out var foodAt))throw new Exception("No reachable food stage");food.affordance.Item.held=false;food.affordance.cooldown=0;test.hunger=.45f;var slice=god.GetComponentInParent<QuestSlice>();var binding=BindingFlags.NonPublic|BindingFlags.Instance;
   if(!(bool)typeof(QuestSlice).GetMethod("TryGrab",binding).Invoke(slice,new object[]{1,food.affordance.Item}))throw new Exception("Food pickup failed");typeof(QuestSlice).GetMethod("Release",binding).Invoke(slice,new object[]{1,new Ray(foodAt+Vector3.up*2,Vector3.down)});
   if(test.hunger!=.45f||Vector3.Distance(food.transform.position,foodAt)>nav.world.Scale*4)throw new Exception("Placed food was consumed immediately or not placed");test.hunger=.95f;god.Tick(.05f);
   if(god.State.intention.action!=CreatureAction.EatFood||god.State.receipt.eligible||brick.Item.held)throw new Exception($"Urgent hunger did not safely supersede leisure: action={god.State.intention.action}, owner={god.Owner}, receipt={god.State.receipt.eligible}, brickHeld={brick.Item.held}, food={god.Targets.Count(t=>t.affordance?.kind==CreatureAffordance.Kind.Food&&t.affordance.Available&&god.CanReach(t))}");
   test.developmentFocus=false;test.seconds=180;test.water=10;god.Tick(.05f);if(god.State.intention.action!=CreatureAction.EatFood)throw new Exception("Critical water repeatedly canceled urgent eating");
   Debug.Log("BWVR_ATTENTION_PASS: frontal visible placement gaze/curiosity/native utility; held, rear, occluded and paused rejection; expiry; urgent hunger interrupts leisure without reward");
  }finally{if(blocker!=null)UnityEngine.Object.DestroyImmediate(blocker);god.Interrupt("Attention QA done");item.held=false;item.transform.position=original;food.transform.position=foodPosition;food.affordance.cooldown=foodCooldown;god.transform.rotation=rotation;Physics.SyncTransforms();}
 }
}
}
