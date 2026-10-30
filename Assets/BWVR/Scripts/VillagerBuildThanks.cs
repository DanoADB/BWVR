using System;
using System.Linq;
using UnityEngine;
namespace BWVR {
public sealed partial class ConstructionSite {
 // Only the pieces satisfying the current form count; decorative extras do not.
 public float CreatureRecipeShare {
  get {if(!RecipeComplete)return 0;int helped=0;foreach(var p in Pattern){var b=blocks.FirstOrDefault(piece=>Matches(piece,p));if(b!=null&&b.creatureBuilt)helped++;}return (float)helped/Pattern.Length;}
 }
}
public sealed partial class SocialMemory {
 public int helpfulBuildUses,buildThanks;
 bool BuildThanksValid()=>Count(helpfulBuildUses)&&helpfulBuildUses<=constructionUses&&Count(buildThanks)&&buildThanks<=helpfulBuildUses;
}
public sealed partial class VillagerExploration {
 CreatureHybrid buildBenefactor;float thanksElapsed,thanksGesture;
 bool CreditHelpfulStructure(ConstructionSite site,bool first,bool comfortable){
  float share=site.CreatureRecipeShare;if(!first||!comfortable||share<=0)return false;
  var m=social.Memory;m.trust=Mathf.Clamp01(m.trust+.06f*share);m.affinity=Mathf.Clamp01(m.affinity+.03f*share);m.helpfulBuildUses=Math.Min(1000000,m.helpfulBuildUses+1);
  construction.Reinforce(site.Recipe,true,share);return true;
 }
 bool BeginBuildThanks(CreatureHybrid god){
  if(god==null||god.Season.paused||god.GetComponent<CarryItem>()?.held==true||social.Memory.injury>0||social.RecentFright||Vector3.Distance(transform.position,god.transform.position)>nav.world.Scale*60)return false;
  var delta=transform.position-god.transform.position;delta.y=0;if(delta.sqrMagnitude<1e-10f)delta=-god.transform.forward;
  if(!nav.world.Sample(god.transform.position+delta.normalized*nav.world.Scale*6,nav.profile,6,out var goal)||!world.Reachable(transform.position,goal,nav.profile))return false;
  buildBenefactor=god;thanksElapsed=thanksGesture=0;Current=Phase.BuildThanks;motion?.LookAt(god.transform);return true;
 }
 bool TickBuildThanks(float dt){
  thanksElapsed+=dt;var god=buildBenefactor;
  if(god==null||!god.isActiveAndEnabled||god.GetComponent<CarryItem>()?.held==true||social.Memory.injury>0||social.RecentFright||thanksElapsed>12||Vector3.Distance(transform.position,god.transform.position)>nav.world.Scale*60){ResetTask();return false;}
  var delta=transform.position-god.transform.position;delta.y=0;motion?.LookAt(god.transform);
  if(delta.magnitude<=nav.world.Scale*8){
   if(delta.sqrMagnitude>1e-10f)transform.rotation=Quaternion.LookRotation(-delta.normalized);
   motion?.Signal(true);thanksGesture+=dt;
   if(thanksGesture>=1.5f){social.Memory.buildThanks=Math.Min(social.Memory.helpfulBuildUses,social.Memory.buildThanks+1);GetComponent<VillagerAffect>()?.Reassured();motion?.React();IslandAudio.Cue(IslandSound.Content,transform.position,GetEntityId().GetHashCode(),.5f);god.Journal.Add(god.State.time,"building thanks",CreatureAction.SocialInteraction,GetComponent<HybridTarget>().id,"A villager thanked the creature after useful construction");if(god.Owner==CreatureHybrid.ActionOwner.None)god.GetComponent<CreatureCuriosity>().NoticeGreeting(transform);ResetTask();Current=Phase.Celebrate;return true;}
  }else{
   thanksGesture=0;motion?.Signal(false);var goal=god.transform.position+delta.normalized*nav.world.Scale*6;nav.MoveTo(goal,transform.lossyScale.y*1.6f,dt);if(nav.Blocked&&thanksElapsed>5){ResetTask();return false;}
  }return Active;
 }
 void ReleaseBuildThanks(){if(buildBenefactor!=null)motion?.Signal(false);buildBenefactor=null;thanksElapsed=thanksGesture=0;}
}
}
