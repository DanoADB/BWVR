using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;
namespace BWVR.Editor {
public static class PlaymateChecks {
 [MenuItem("BWVR/Verify remembered playmates and reciprocal invitations (Play mode)")]
 public static void Verify(){
  if(!EditorApplication.isPlaying)throw new Exception("Enter Play mode");
  var ball=UnityEngine.Object.FindAnyObjectByType<BeachBall>();var god=ball.Playmate;var slice=UnityEngine.Object.FindAnyObjectByType<QuestSlice>();var mind=god.GetComponent<CreatureCuriosity>();slice.SendMessage("OnApplicationPause",false);god.Interrupt("Playmate QA");
  var field=typeof(QuestSlice).GetField("state",BindingFlags.Instance|BindingFlags.NonPublic);var original=(SliceState)field.GetValue(slice);var test=new SliceState{water=100,crops=100,developmentFocus=true};test.Hybrid.time=300;test.WorldPlay.rallyFun=.3f;field.SetValue(slice,test);
  var people=slice.GetComponentsInChildren<VillagerExploration>();var positions=people.Select(p=>p.transform.position).ToArray();var held=people.Select(p=>p.GetComponent<CarryItem>().held).ToArray();var godPosition=god.transform.position;var favorite=people[1];string id=favorite.GetComponent<HybridTarget>().id;var nav=god.GetComponent<IslandNavigator>();float scale=nav.world.Scale;
  try{
   foreach(var person in people){person.ResetTask();person.GetComponent<VillagerSocial>().ResetTask();person.GetComponent<VillagerBlockPlay>().ResetTask();person.GetComponent<CarryItem>().held=false;person.GetComponent<VillagerSocial>().Memory.trust=.65f;person.GetComponent<VillagerSocial>().Memory.injury=0;var bond=test.Hybrid.Bond(person.GetComponent<HybridTarget>().id,true);bond.affection=bond.confidence=person==favorite?.9f:.3f;bond.goodEncounters=5;bond.lastGood=200;}
   ball.ReturnHome();ball.GetComponent<CreatureAffordance>().cooldown=0;
   bool staged=false;for(int i=0;i<16;i++){var direction=Quaternion.Euler(0,i*22.5f,0)*Vector3.forward;if(!nav.world.Sample(ball.transform.position+direction*scale*20,nav.profile,2,out var g)||!nav.world.Sample(ball.transform.position-direction*scale*6,nav.world.villager,2,out var v))continue;g=nav.world.ProjectFoot(g,nav.profile);if(Mathf.Abs(g.y-(ball.transform.position.y-ball.Radius*scale))>scale*1.1f)continue;god.transform.position=g;foreach(var person in people){person.transform.position=nav.world.ProjectFoot(v,nav.world.villager);person.GetComponent<IslandNavigator>().ResetRoute();}nav.ResetRoute();staged=true;break;}
   if(!staged)throw new Exception("No flat playmate fixture");
   ball.RefreshPlaymate();if(ball.ProspectivePlayer!=favorite)throw new Exception("Creature ignored its favorite among equally near willing villagers");
   test.paused=true;if(ball.TryInvitePlaymate()||ball.Player!=null)throw new Exception("Paused creature invitation started");test.paused=false;
   god.Perceive();var choices=(System.Collections.Generic.List<CreatureCandidate>)typeof(CreatureHybrid).GetField("candidates",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(god);var choice=choices.Single(c=>c.target==ball.StableId);if(choice.situation!=CreatureSituation.SafeContact)throw new Exception("Prospective social play had no safe-contact context");
   typeof(CreatureHybrid).GetMethod("BeginAction",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(god,new object[]{choice,god.Find(ball.StableId)});
   for(int i=0;i<4000&&!test.Hybrid.receipt.eligible;i++){test.Hybrid.time+=.02f;god.Tick(.02f,false);favorite.Tick(.02f,false);ball.Advance(.02f,false);}
   var remembered=test.Hybrid.Bond(id);if(remembered.acceptedInvitations!=1||ball.RallyReceptions<4||test.Hybrid.receipt.partner!=id)throw new Exception("No creature-led favorite rally/typed partner receipt: invited="+remembered.acceptedInvitations+", receptions="+ball.RallyReceptions+", partner="+test.Hybrid.receipt.partner+", phase="+mind.Current);
   float affinity=favorite.GetComponent<VillagerSocial>().Memory.affinity,preference=remembered.affection;float unrelated=people[0].GetComponent<VillagerSocial>().Memory.affinity;if(!god.Feedback(true)||remembered.affection<=preference||favorite.GetComponent<VillagerSocial>().Memory.affinity<=affinity||people[0].GetComponent<VillagerSocial>().Memory.affinity!=unrelated||god.Feedback(true))throw new Exception("Partner feedback leaked or applied twice");
   favorite.ResetTask();ball.ReturnHome();god.Interrupt("Fear/consent QA");var social=favorite.GetComponent<VillagerSocial>();social.Memory.trust=.65f;social.ReactToBall(ball.transform,true);if(!social.RecentFright||social.Willing||remembered.badEncounters!=1||favorite.TryCreaturePlayInvite(ball))throw new Exception("Fright did not change willingness and remembered caution");
   test.Hybrid.time+=91;social.ResetTask();if(!social.Willing)throw new Exception("Temporary fright never recovered");
   favorite.GetComponent<CarryItem>().held=true;ball.RefreshPlaymate();if(ball.ProspectivePlayer==favorite||favorite.TryCreaturePlayInvite(ball))throw new Exception("Invitation stole a player-held villager");favorite.GetComponent<CarryItem>().held=false;
   int bad=test.Hybrid.Bond(people[0].GetComponent<HybridTarget>().id).badEncounters;people[0].GetComponent<VillagerSocial>().ReactToBall(ball.transform,false);if(test.Hybrid.Bond(people[0].GetComponent<HybridTarget>().id).badEncounters!=bad)throw new Exception("Player harm blamed on creature bond");
   var restored=JsonUtility.FromJson<SliceState>(JsonUtility.ToJson(test));if(!restored.Valid()||restored.Hybrid.Bond(id).acceptedInvitations!=1||restored.Hybrid.Bond(id).badEncounters!=1)throw new Exception("Individual memories did not save");
   Debug.Log("BWVR_PLAYMATE_PASS: favorite chosen among equal opportunities, creature-initiated native four-reception rally, exact-partner one-shot feedback, fear/recovery, pause/consent/player ownership and saved individual memories");
  }finally{god.Interrupt("Playmate QA complete");for(int i=0;i<people.Length;i++){people[i].ResetTask();people[i].GetComponent<VillagerSocial>().ResetTask();people[i].transform.position=positions[i];people[i].GetComponent<CarryItem>().held=held[i];people[i].GetComponent<IslandNavigator>().ResetRoute();}god.transform.position=godPosition;nav.ResetRoute();field.SetValue(slice,original);ball.ResetWorld();}
 }
}
}
