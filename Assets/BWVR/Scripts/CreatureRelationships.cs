using System;
using UnityEngine;
namespace BWVR {
[Serializable] public sealed class CreatureBond {
 public string villager;public float affection=.5f,confidence=.5f;
 public int goodEncounters,badEncounters,acceptedInvitations,declinedInvitations;public float lastGood=-100,lastBad=-100,nextInviteAt;
}
public sealed partial class HybridState {
 public CreatureBond[] bonds=Array.Empty<CreatureBond>();
 public CreatureBond Bond(string id,bool create=false){
  if(string.IsNullOrEmpty(id))return null;foreach(var bond in bonds)if(bond.villager==id)return bond;
  if(!create||bonds.Length>=16)return null;var added=new CreatureBond{villager=id};Array.Resize(ref bonds,bonds.Length+1);bonds[bonds.Length-1]=added;return added;
 }
 public bool RememberBond(string id,bool safe){
  var bond=Bond(id,true);if(bond==null)return false;
  if(safe){if(bond.goodEncounters>0&&time-bond.lastGood<8)return false;bond.goodEncounters=Math.Min(1000000,bond.goodEncounters+1);bond.lastGood=time;bond.affection=Mathf.Clamp01(bond.affection+.04f);bond.confidence=Mathf.Clamp01(bond.confidence+.025f);}
  else {if(bond.badEncounters>0&&time-bond.lastBad<1)return false;bond.badEncounters=Math.Min(1000000,bond.badEncounters+1);bond.lastBad=time;bond.confidence=Mathf.Clamp01(bond.confidence-.15f);}
  return true;
 }
 public float PlaymateAffinity(string id){var bond=Bond(id);if(bond==null)return .5f;return Mathf.Clamp01(bond.affection*.7f+bond.confidence*.3f-(bond.badEncounters>0&&time-bond.lastBad<90?.2f:0));}
 public void TeachBond(string id,bool positive){var bond=Bond(id,true);if(bond==null)return;bond.affection=Mathf.Clamp01(bond.affection+(positive?.06f:-.06f));}
 bool BondsValid(){
  if(bonds==null||bonds.Length>16)return false;var ids=new System.Collections.Generic.HashSet<string>();
  foreach(var b in bonds)if(b==null||string.IsNullOrEmpty(b.villager)||b.villager.Length>160||!ids.Add(b.villager)||!Number(b.affection,0,1)||!Number(b.confidence,0,1)||b.acceptedInvitations<0||b.acceptedInvitations>1000000||b.declinedInvitations<0||b.declinedInvitations>1000000||b.goodEncounters<0||b.goodEncounters>1000000||b.badEncounters<0||b.badEncounters>1000000||!Number(b.lastGood,-100,time)||!Number(b.lastBad,-100,time)||!Number(b.nextInviteAt,0,100000300))return false;
  return true;
 }
}
}
