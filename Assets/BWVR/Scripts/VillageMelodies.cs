using System;
using System.Linq;
using UnityEngine;
namespace BWVR {
// Four authored motifs. Learned familiarity/preferences are distinct from their seeded selection.
public static class VillageMelodies {
 static readonly int[][] Patterns={new[]{0,0,1,0},new[]{1,0,1,1},new[]{0,1,1,0},new[]{1,1,0,1}};
 public static int Note(int motif,int position)=>Patterns[motif][position];
 public static string Name(int motif)=>string.Join(" ",Patterns[motif].Select(n=>n==0?"low":"high"));
}
[Serializable] public sealed class VillageMelodyMemory {
 public int[] echoes=new int[4];public float[] preference={.5f,.5f,.5f,.5f};
 public bool Valid()=>echoes!=null&&echoes.Length==4&&preference!=null&&preference.Length==4&&echoes.All(n=>n>=0&&n<=1000000)&&preference.All(n=>HybridState.Number(n,0,1));
 public bool Knows(int motif)=>echoes!=null&&echoes.Length==4&&motif>=0&&motif<4&&echoes[motif]>0;
 public void Record(int motif){echoes[motif]=Math.Min(1000000,echoes[motif]+1);}
 public bool Reinforce(int motif,bool positive){if(!Knows(motif))return false;preference[motif]=Mathf.Clamp01(preference[motif]+(positive?.12f:-.12f));return true;}
 public float Bias(int motif)=>Knows(motif)?(preference[motif]-.5f)*.4f:0;
}
public sealed partial class SocialMemory {
 public bool hasMelody;public int melodyMotif,melodyEchoes;
 bool MelodyValid()=>melodyMotif>=0&&melodyMotif<4&&melodyEchoes>=0&&melodyEchoes<=rhythmGames&&hasMelody==(melodyEchoes>0);
 public void RememberMelody(int motif){hasMelody=true;melodyMotif=motif;melodyEchoes=Math.Min(rhythmGames,Math.Min(1000000,melodyEchoes+1));}
}
public sealed partial class VillagerSocial {public int MelodyChoice=>Memory.hasMelody?Memory.melodyMotif:(hybrid.State.seed^(index*31))&3;}
public sealed partial class SharedRhythm {
 int activeMelody=-1,heardCalls,echoedCalls;bool melodyCommitted;
 public int[] PerformedNotes{get;private set;}=Array.Empty<int>();public bool[] VillagerPerformed{get;private set;}=Array.Empty<bool>();
 public int CompletedMelody=>Completed&&melodyCommitted?activeMelody:-1;
 void PrepareMelody(VillagerSocial person){activeMelody=person.MelodyChoice;heardCalls=echoedCalls=0;melodyCommitted=false;PerformedNotes=new int[BeatCount];VillagerPerformed=new bool[BeatCount];}
 bool CanHearMelody(){
  var nav=creature.GetComponent<IslandNavigator>().world;if(Vector3.Distance(creature.transform.position,transform.position)>nav.Scale*12)return false;var from=creature.transform.position+Vector3.up*nav.Scale*10;var shape=GetComponent<Collider>();var end=shape!=null?shape.bounds.center:transform.position;var delta=end-from;Physics.SyncTransforms();
  foreach(var hit in Physics.RaycastAll(from,delta.normalized,delta.magnitude,~0,QueryTriggerInteraction.Ignore).OrderBy(h=>h.distance)){if(hit.transform.IsChildOf(creature.transform)||hit.transform.IsChildOf(host.transform))continue;return hit.transform==transform||hit.transform.IsChildOf(transform);}return true;
 }
 void MelodyBeat(bool villagerTurn){int position=Beats/4*2+Beats%2;int note=VillageMelodies.Note(activeMelody,position);if(villagerTurn&&CanHearMelody())heardCalls|=1<<position;if(!villagerTurn&&(heardCalls&(1<<position))!=0)echoedCalls|=1<<position;Tap(villagerTurn?hostMotion:guestMotion,note);PerformedNotes[Beats]=note;VillagerPerformed[Beats]=villagerTurn;}
 void RememberMelody(){if(melodyCommitted||activeMelody<0||heardCalls!=15||echoedCalls!=15)return;var play=creature.Season.WorldPlay;if(play.villageMelodies==null)play.villageMelodies=new VillageMelodyMemory();play.villageMelodies.Record(activeMelody);melodyCommitted=true;creature.Journal.Add(creature.State.time,"villager melody echoed",CreatureAction.PlayWithToy,host.GetComponent<HybridTarget>().id,VillageMelodies.Name(activeMelody)+"; two witnessed calls and two replies completed");}
}
}
