using UnityEngine;
namespace BWVR {
public sealed partial class CreatureCuriosity {
 Vector3 catchHoldStart;
 public bool TryBeginCatch(CreatureAffordance candidate){
  if(Current!=Phase.Rest||routine.Current!=CreatureRoutine.Task.SeekingWater||candidate?.GetComponent<BeachBall>()?.BeginCatch()!=true)return false;
  routine.ResetTask();navigator.ResetRoute();target=candidate;owned=null;goalPlay=echoPlay=fetching=bowlingPlay=knockdownPlay=bowlCoursePlay=false;elapsed=clock=0;Current=Phase.CatchBall;motion?.LookAt(candidate.transform);motion?.Activity(6);Affect=CreatureAffect.Mood.Curious;return true;
 }
 bool TickCatch(float dt){
  var ball=target!=null?target.GetComponent<BeachBall>():null;
  if(ball==null||!ball.OwnsCatch||owned==null&&target.Item.held||elapsed>4){Finish(false,"Catch opportunity ended");return false;}
  motion?.LookAt(ball.transform);
  if(Current==Phase.CatchBall){
   Affect=CreatureAffect.Mood.Curious;motion?.Touch(ball.transform);float windup=Mathf.Lerp(.25f,.1f,state.worldPlay?.catchBall?.control??.15f);
   if(clock>=windup&&ball.CaptureCatch()){owned=target.Item;original=owned.transform.position;originalRotation=owned.transform.rotation;catchHoldStart=ball.CatchHands;owned.transform.position=catchHoldStart;motion?.Carry(owned.transform);motion?.PlaceAt(catchHoldStart,Quaternion.identity,1);Current=Phase.CatchReturn;clock=0;Affect=CreatureAffect.Mood.Excited;}
   else if(clock>1.5f||!ball.InFlight){Finish(false,"Ball passed out of reach");return false;}
  }else{
   Affect=CreatureAffect.Mood.Happy;motion?.Activity(5);var release=ball.CatchReleasePoint;float blend=Mathf.Clamp01(clock/.8f);var point=Vector3.Lerp(catchHoldStart,release,Mathf.SmoothStep(0,1,blend));motion?.PlaceAt(point,Quaternion.identity,1);owned.transform.position=point;
   if(clock>=.9f){motion?.Carry(null);motion?.Touch(null);if(!ball.RollCatchBack()){Finish(false,"No safe ground for a return roll");return false;}owned=null;played++;target.Discovered();lastWater=lastSocial=false;boredom=0;feedbackWindow=12;Finish(true);return false;}
  }return true;
 }
}
}
