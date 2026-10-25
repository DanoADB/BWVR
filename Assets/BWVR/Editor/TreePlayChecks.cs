using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace BWVR.Editor {
public static class TreePlayChecks {
 static readonly BindingFlags Private=BindingFlags.NonPublic|BindingFlags.Instance;
 static QuestSlice stagedSlice;static SliceState stagedOriginal;static ThrowableTree stagedTree;static Vector3 stagedGod;static int stage;
 public static string StageNavigation(){
  if(stagedSlice!=null)throw new Exception("Finish previous navigation stage");var world=UnityEngine.Object.FindAnyObjectByType<TreePlayWorld>();var nav=world.Navigation;var god=world.GetComponentInParent<QuestSlice>().GetComponentInChildren<CreatureHybrid>();
  stagedSlice=world.GetComponentInParent<QuestSlice>();stagedOriginal=(SliceState)typeof(QuestSlice).GetField("state",Private).GetValue(stagedSlice);stagedGod=god.transform.position;god.Interrupt("Tree navigation QA");typeof(QuestSlice).GetField("state",Private).SetValue(stagedSlice,new SliceState{paused=true,developmentFocus=true});world.ResetWorld();
  try{
   stagedTree=world.Trees.Where(t=>!nav.Sample(t.Home,nav.villager,.2f,out _)&&world.Trees.All(o=>o==t||Vector3.Distance(o.Home,t.Home)>world.Unit*8)&&new[]{Vector3.forward,Vector3.back,Vector3.left,Vector3.right}.All(d=>Mathf.Abs(nav.Terrain.Project(t.Home+d*world.Unit*3).y-t.Home.y)<world.Unit*.8f)).OrderBy(t=>(t.Home-god.transform.position).sqrMagnitude).First();
   if(!nav.Sample(stagedTree.Home,god.GetComponent<IslandNavigator>().profile,16,out var at))throw new Exception("No root removal staging ground");god.transform.position=nav.ProjectFoot(at,nav.creature);world.RefreshInterest();if(!stagedTree.Lift())throw new Exception("Could not lift navigation test tree");stagedTree.Item.held=true;stage=1;return "ROOT_UPDATE_PENDING line="+stagedTree.Line;
  }catch{CleanupNavigation();throw;}
 }
 public static string VerifyNavigation(){
  if(stagedSlice==null)throw new Exception("Stage navigation first");var world=stagedTree.World;var nav=world.Navigation;if(nav.TreeNavigationPending)return "PENDING";
  try{
   if(!nav.Sample(stagedTree.Home,nav.villager,.2f,out _))throw new Exception("Old trunk obstruction remained after uprooting");
   if(stage==1){var at=nav.Terrain.Project(stagedTree.Home+Vector3.right*world.Unit*8)+Vector3.up*world.Unit*stagedTree.Radius;stagedTree.Restore(new MovedTree{line=stagedTree.Line,position=nav.ToNav(at),rotation=Quaternion.Euler(0,0,-90)});stage=2;return "ROOT_CLEAR_FALLEN_CARVE_PENDING";}
   if(nav.Sample(stagedTree.ImpactBounds.center,nav.villager,.2f,out _))throw new Exception("Fallen trunk did not carve walking ground");
   Debug.Log("BWVR_TREE_NAV_PASS: asynchronous old-trunk removal opens native walking ground; relocated horizontal tree capsule carves a new obstruction; original root remains clear");CleanupNavigation();return "TREE_NAV_PASS";
  }catch{CleanupNavigation();throw;}
 }
 static void CleanupNavigation(){if(stagedSlice==null)return;var world=stagedSlice.GetComponentInChildren<TreePlayWorld>();var god=stagedSlice.GetComponentInChildren<CreatureHybrid>();god.Interrupt("Tree navigation QA done");typeof(QuestSlice).GetField("state",Private).SetValue(stagedSlice,stagedOriginal);world.ResetWorld();god.transform.position=stagedGod;god.GetComponent<IslandNavigator>().ResetRoute();stagedSlice=null;stagedTree=null;stagedOriginal=null;stage=0;}
 [MenuItem("BWVR/Verify creature tree throwing (Play mode)")]
 public static void Verify(){
  if(!Application.isPlaying)throw new Exception("Enter Play mode");var world=UnityEngine.Object.FindAnyObjectByType<TreePlayWorld>();if(world.Trees.Count==0)throw new Exception("No real tree models registered");var slice=world.GetComponentInParent<QuestSlice>();var god=slice.GetComponentInChildren<CreatureHybrid>();var mind=god.GetComponent<CreatureCuriosity>();var nav=world.Navigation;var walker=god.GetComponent<IslandNavigator>();var state=typeof(QuestSlice).GetField("state",Private);var original=(SliceState)state.GetValue(slice);var position=god.transform.position;var residents=slice.GetComponentsInChildren<VillagerSocial>();var residentPositions=residents.Select(v=>v.transform.position).ToArray();
  god.Interrupt("Tree QA");foreach(var person in residents){person.ResetTask();person.GetComponent<VillagerExploration>().ResetTask();person.GetComponent<VillagerBlockPlay>().ResetTask();}var test=new SliceState{developmentFocus=true,hunger=.1f};state.SetValue(slice,test);world.ResetWorld();
  try{
   var tree=world.Trees.FirstOrDefault(t=>t.Available&&god.CanReach(t.transform.position));if(tree==null)throw new Exception("No reachable eligible tree");if(tree.Item.CanGrab)throw new Exception("Rooted trees accidentally became player pickups");
   if(!nav.Sample(tree.transform.position,walker.profile,16,out var at))throw new Exception("No native uprooting approach");god.transform.position=nav.ProjectFoot(at,walker.profile);walker.ResetRoute();world.RefreshInterest();var target=tree.GetComponent<HybridTarget>();
   void Begin(ThrowableTree chosen){chosen.Affordance.cooldown=0;typeof(CreatureHybrid).GetMethod("BeginAction",Private).Invoke(god,new object[]{new CreatureCandidate{action=CreatureAction.PlayWithToy,situation=CreatureSituation.PlayOpportunity,target=chosen.StableId,timeout=90},chosen.GetComponent<HybridTarget>()});if(god.Owner!=CreatureHybrid.ActionOwner.Curiosity)throw new Exception("Tree executor did not own action");}
   int obstacles=nav.obstacleCount;Begin(tree);bool held=false,paused=false;Vector3 release=default;int moved=0;
   for(int i=0;i<2400&&!(test.Hybrid.receipt.eligible&&test.Hybrid.receipt.target==target.id);i++){
    god.Tick(.02f);if(mind.Holding==tree.Item){held=true;if(!tree.Uprooted||!tree.Item.held||tree.GetComponentsInChildren<Collider>().Any(c=>c.enabled))throw new Exception("Uprooting did not release the trunk/disable carry collisions");}
    if(world.Active==tree){if(!paused){paused=true;release=tree.transform.position;var before=world.Body.position;test.paused=true;god.Tick(1);world.Advance(1,true);if(world.Body.position!=before)throw new Exception("Paused tree moved");test.paused=false;}
     if(Vector3.Distance(tree.transform.position,release)>world.Unit*2)moved++;
    }
    world.Advance(.02f,false);
   }
   if(!held||!paused||moved==0||world.Active!=null||tree.Item.held||!tree.Uprooted||nav.obstacleCount!=obstacles-1||test.WorldPlay.Trees.throws!=1||test.WorldPlay.Trees.safeThrows+test.WorldPlay.Trees.harmfulThrows!=1||!test.Hybrid.receipt.eligible)throw new Exception($"Native tree sequence failed: held={held}, flight={paused}, moved={moved}, phase={mind.Current}, active={world.Active!=null}");
   float preference=test.WorldPlay.Trees.preference;if(!god.Feedback(false)||test.WorldPlay.Trees.preference>=preference||god.Feedback(true))throw new Exception("Tree teaching attribution/dedup failed");
   // Native full-sized capsule must collide with the villager contact proxy.
   god.Interrupt("Tree collision QA");var person=residents[0];var victim=person.GetComponent<PlayImpactTarget>();float injury=person.Memory.injury,trust=person.Memory.trust;
   if(!nav.Sample(slice.GetComponentInChildren<BeachBall>().GetComponent<CarryItem>().home,nav.villager,10,out at))throw new Exception("No impact stage");person.transform.position=nav.ProjectFoot(at,nav.villager);tree.transform.SetPositionAndRotation(person.transform.position-Vector3.right*world.Unit*(tree.Height+3)+Vector3.up*world.Unit*(tree.Radius+.1f),Quaternion.Euler(0,0,-90));
   if(!world.Throw(tree,Vector3.right))throw new Exception("Tree physics throw refused");world.Body.linearVelocity=Vector3.right*16;world.Body.angularVelocity=Vector3.zero;if(world.Throw(world.Trees.First(t=>t!=tree),Vector3.forward))throw new Exception("Second concurrent tree accepted");
   for(int i=0;i<150&&person.Memory.injury==injury;i++)world.Advance(.02f,false);
   if(person.Memory.injury<=injury||person.Memory.trust>=trust||!world.Harmful)throw new Exception("Native tree capsule did not hit/injure/frighten villager");float damage=test.WorldPlay.Damage(victim.id);world.Impact(victim,20);if(test.WorldPlay.Damage(victim.id)!=damage)throw new Exception("Duplicate collision damaged twice");
   for(int i=0;i<700&&world.Active!=null;i++)world.Advance(.02f,false);
   if(world.Active!=null||test.WorldPlay.Trees.harmfulThrows<1)throw new Exception("Harmful flight did not finish exactly once");
   var saved=JsonUtility.FromJson<SliceState>(JsonUtility.ToJson(test));if(!saved.Valid()||saved.WorldPlay.Trees.moved.Length!=1||saved.WorldPlay.Trees.throws!=2||saved.WorldPlay.Damage(victim.id)<=0)throw new Exception("Moved tree/outcomes did not save");var record=saved.WorldPlay.Trees.Find(tree.Line);tree.Restore(record);if(Vector3.Distance(nav.ToNav(tree.transform.position),record.position)>.001f)throw new Exception("Moved tree pose did not restore");var resting=tree.transform.position;test.Hybrid.guidance=new GuidanceState{status=GuidanceStatus.Active,target=tree.StableId,requestedAt=test.Hybrid.time,expiresAt=test.Hybrid.time+30};if(!tree.Lift())throw new Exception("Landed tree could not be lifted again");tree.Item.held=true;tree.transform.position+=Vector3.up*world.Unit*20;tree.CancelCarry();if(Vector3.Distance(tree.transform.position,resting)>.00001f)throw new Exception("Interrupted re-carry regrew tree at planting spot");test.Hybrid.guidance.status=GuidanceStatus.None;saved.WorldPlay.Trees.moved[0].rotation=new Quaternion();if(saved.Valid())throw new Exception("Invalid saved tree quaternion accepted");if(!JsonUtility.FromJson<SliceState>("{\"version\":1}").Valid())throw new Exception("Legacy save rejected");
   var bounded=new TreePlayMemory();for(int i=0;i<32;i++)if(!bounded.Remember(i,Vector3.zero,Quaternion.identity))throw new Exception("Tree memory filled early");if(bounded.Remember(33,Vector3.zero,Quaternion.identity)||!bounded.Valid())throw new Exception("Tree save budget not bounded");
   world.RefreshInterest();var other=world.Trees.First(t=>t!=tree&&t.Available&&god.CanReach(t.transform.position));if(!nav.Sample(other.transform.position,walker.profile,16,out at))throw new Exception("No cancel stage");god.transform.position=nav.ProjectFoot(at,walker.profile);walker.ResetRoute();world.RefreshInterest();Begin(other);
   for(int i=0;i<1000&&mind.Holding==null;i++)god.Tick(.02f);if(mind.Holding!=other.Item)throw new Exception("No carried tree to cancel");int throws=test.WorldPlay.Trees.throws;god.Interrupt("Player picked up creature");if(other.Item.held||!other.GetComponent<Collider>().enabled||world.Active!=null||test.WorldPlay.Trees.throws!=throws||test.Hybrid.receipt.eligible)throw new Exception("Canceled tree carry leaked ownership/reward");
   Debug.Log("BWVR_TREE_PLAY_PASS: authored tree model; exclusive animated approach/uproot/lift/throw/watch; real capsule flight and terrain; pause; bounded flight; native villager collision/injury/fear/dedup; one contextual lesson; saved moved pose/damage and invalid/legacy checks; 32-record budget; carry cancellation");
  }finally{god.Interrupt("Tree QA done");state.SetValue(slice,original);world.ResetWorld();foreach(var person in residents)person.ResetTask();for(int i=0;i<residents.Length;i++)residents[i].transform.position=residentPositions[i];god.transform.position=position;walker.ResetRoute();foreach(var ball in slice.GetComponentsInChildren<BeachBall>())ball.ResetWorld();}
 }
}
}
