using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace BWVR.Editor {
public static class FarmingChecks {
 static void Require(bool ok,string why){if(!ok)throw new Exception(why);}
 [MenuItem("BWVR/Verify renewable wheat model")]
 public static void VerifyModel(){
  var s=new ResourceState();var field=s.Ensure("wheat",VillageResource.Wheat);Require(!s.Sow(field.id)&&!s.Plant(field.id),"Standing crop replanted or treated as a tree");
  for(int i=0;i<4;i++){Require(s.Harvest(0,field.id,3)==3,"Finite crop failed");if(i==3)Require(!s.Sow(field.id),"Sowing erased undelivered harvest");s.Deposit(0);}
  Require(s.wheat==18&&s.Sow(field.id)&&s.wheat==17&&s.seedUsed==1&&field.growing&&!field.felled&&field.plantings==1,"Seed cost or crop planting incorrect");
  Require(!s.Sow(field.id)&&s.wheat==17&&s.Harvest(1,field.id,3)==0,"Duplicate seed spend or immature harvest");
  Require(!s.Grow(field.id,float.NaN)&&!s.Grow(field.id,float.PositiveInfinity)&&!s.Grow(field.id,-1)&&!s.Grow(field.id,90)&&Mathf.Abs(field.growth-.5f)<.001f,"Growth time invalid");
  var saved=JsonUtility.FromJson<ResourceState>(JsonUtility.ToJson(s));Require(saved.Valid()&&saved.Find(field.id).growth==.5f&&saved.seedUsed==1,"Saved field stage/cost lost");
  Require(saved.Grow(field.id,1000)&&saved.Find(field.id).remaining==12&&saved.Find(field.id).regrown==1&&!saved.Grow(field.id,1000),"Maturity credited repeated yields");
  for(int i=0;i<4;i++){saved.Harvest(0,field.id,3);saved.Deposit(0);}Require(saved.wheat+saved.seedUsed==30&&saved.Valid(),"Crop/seed accounting did not conserve grain");
  saved.wheat=0;Require(!saved.Sow(field.id)&&saved.seedUsed==1,"Empty store supplied free seed");saved.wheat=1;Require(saved.Sow(field.id)&&saved.wheat==0&&saved.seedUsed==2,"Single remaining seed failed");
  var invalid=JsonUtility.FromJson<ResourceState>(JsonUtility.ToJson(saved));invalid.Find(field.id).felled=true;Require(!invalid.Valid(),"Wheat accepted tree felling state");invalid=JsonUtility.FromJson<ResourceState>(JsonUtility.ToJson(saved));invalid.Find(field.id).remaining=1;Require(!invalid.Valid(),"Growing crop has harvestable grain");invalid=JsonUtility.FromJson<ResourceState>(JsonUtility.ToJson(saved));invalid.seedUsed=-1;Require(!invalid.Valid(),"Invalid seed ledger accepted");
  var protectedSeed=new ResourceState{wheat=2};protectedSeed.Consume(900,3,1);Require(protectedSeed.wheat==1&&protectedSeed.consumed==1,"Residents ate reserved sowing seed");protectedSeed.Consume(90,3);Require(protectedSeed.wheat==0,"Released seed remained reserved");
  Require(new SliceState().Valid(),"Legacy save invalid");Debug.Log("BWVR_FARMING_MODEL_PASS: finite crops, seed payment/reservation, pending-cargo protection, no immature harvest, saved growth, exactly-once maturation and conservation");
 }
 [MenuItem("BWVR/Verify native wheat sowing (Play mode)")]
 public static void VerifyRuntime(){
  Require(EditorApplication.isPlaying,"Enter Play mode");var world=UnityEngine.Object.FindAnyObjectByType<VillageResources>();var slice=world.GetComponentInParent<QuestSlice>();var field=typeof(QuestSlice).GetField("state",BindingFlags.Instance|BindingFlags.NonPublic);var original=(SliceState)field.GetValue(slice);var positions=world.Gatherers.Select(a=>a.transform.position).ToArray();slice.GetComponentInChildren<CreatureHybrid>().Interrupt("Farming QA");
  foreach(var a in world.Gatherers){a.Cancel();a.GetComponent<VillagerSocial>().ResetTask();a.GetComponent<VillagerExploration>().ResetTask();a.GetComponent<VillagerBlockPlay>().ResetTask();}
  var test=new SliceState{developmentFocus=true};field.SetValue(slice,test);world.ResetWorld();
  try{
   var plots=world.sources.Where(s=>s.kind==VillageResource.Wheat).Take(2).ToArray();Require(plots.Length==2,"Missing wheat plots");foreach(var plot in plots)test.Resources.Find(plot.id).remaining=0;test.Resources.wheat=1;world.Refresh();var node=plots[0];var record=test.Resources.Find(node.id);var actor=world.Gatherers[1];actor.transform.position=node.approach;actor.GetComponent<IslandNavigator>().ResetRoute();
   Require(actor.TrySow(node)&&!world.Gatherers[2].TrySow(node)&&!world.Gatherers[2].TrySow(plots[1]),"Plot/seed reservations overlap");actor.Tick(30,true);world.Advance(30,true);Require(!record.growing&&test.Resources.wheat==1,"Paused sowing charged seed");
   actor.GetComponent<CarryItem>().held=true;actor.Tick(.1f,false);actor.GetComponent<CarryItem>().held=false;Require(!actor.Active&&node.owner==null&&!record.growing&&test.Resources.seedUsed==0&&test.Resources.wheat==1,"Interrupted sowing lost seed or ownership");
   Require(actor.TrySow(node),"Cannot retry sowing");for(int n=0;n<4000&&actor.Active;n++)actor.Tick(.05f,false);Require(!actor.Active&&record.growing&&record.plantings==1&&test.Resources.wheat==0&&test.Resources.seedUsed==1&&node.owner==null,"Villager failed to sow exactly once");
   Require(!world.Available(node)&&!node.cropHead.gameObject.activeSelf,"Immature crop looks harvestable");float initial=node.visual.localScale.y;world.Advance(90,false);Require(record.growth>.49f&&node.visual.localScale.y>initial&&!world.Available(node),"Crop did not visibly grow");float growth=record.growth;world.Advance(30,true);Require(record.growth==growth,"Paused crop grew");
   var saved=JsonUtility.FromJson<SliceState>(JsonUtility.ToJson(test));Require(saved.Valid(),"Crop save invalid");field.SetValue(slice,saved);world.ResetWorld();record=saved.Resources.Find(node.id);Require(record.growing&&record.growth==growth&&saved.Resources.seedUsed==1,"Reload lost stage/cost");
   world.Advance(91,false);Require(!record.growing&&record.regrown==1&&record.remaining==12&&node.cropHead.gameObject.activeSelf&&world.Available(node),"Ripened wheat unavailable");Require(actor.TryBegin(node),"Ripe crop could not be harvested");for(int n=0;n<4000&&actor.Active;n++)actor.Tick(.05f,false);Require(saved.Resources.wheat==3&&record.remaining==9&&!actor.Active&&saved.Valid(),"Renewed wheat was not delivered exactly once");
   Require(node.visual.GetComponentsInChildren<Collider>(true).Length==0&&node.visual.GetComponentsInChildren<Rigidbody>(true).Length==0,"Crop art added plant physics");
   Debug.Log("BWVR_FARMING_RUNTIME_PASS: native sowing, exclusive plot/seed, cancel/retry, paid planting, growth visuals, pause/reload, ripe harvest/delivery and bounded collider-free crops");
  }finally{foreach(var a in world.Gatherers)a.Cancel();field.SetValue(slice,original);world.ResetWorld();for(int i=0;i<positions.Length;i++){world.Gatherers[i].transform.position=positions[i];world.Gatherers[i].GetComponent<IslandNavigator>().ResetRoute();}}
 }
}
}
