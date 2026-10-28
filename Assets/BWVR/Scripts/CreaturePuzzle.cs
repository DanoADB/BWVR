using UnityEngine;
namespace BWVR {
public sealed partial class CreatureCuriosity {
 public bool TryBeginPuzzle(CreatureAffordance candidate){
  if(Current!=Phase.Rest||candidate==null||routine.Current!=CreatureRoutine.Task.SeekingWater||candidate.GetComponent<PuzzlePad>()?.puzzle.Claim()!=true)return false;
  routine.ResetTask();navigator.ResetRoute();target=candidate;owned=null;goalPlay=echoPlay=fetching=bowlingPlay=false;goalTarget=null;elapsed=clock=0;Current=Phase.PuzzleAssist;motion?.LookAt(target.GetComponent<PuzzlePad>().puzzle.Key.transform);Affect=CreatureAffect.Mood.Curious;return true;
 }
 bool TickPuzzle(float dt){var puzzle=target!=null?target.GetComponent<PuzzlePad>()?.puzzle:null;int outcome=puzzle?.Step(navigator,routine.speed,dt)??-1;Affect=puzzle?.OnPad==true?CreatureAffect.Mood.Excited:CreatureAffect.Mood.Curious;if(outcome!=0){if(outcome>0){played++;target.Discovered();lastWater=lastSocial=false;boredom=0;feedbackWindow=12;}Finish(outcome>0);if(outcome>0)motion?.React();return false;}return true;}
}
}
