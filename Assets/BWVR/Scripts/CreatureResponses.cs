using System.Linq;
using UnityEngine;
namespace BWVR {
public enum CreatureResponse {None,Noticed,Chosen,Busy,Unavailable,Completed}
public sealed partial class CreatureHybrid {
 public CreatureResponse Response{get;private set;}
 public string ResponseTarget{get;private set;}="";
 public string ResponseReason{get;private set;}="";
 public string ResponseDetails=>Response==CreatureResponse.None?"":Response+": "+ResponseReason;
 float responseStarted,responseUntil,nextCueCheck,busyAt;HybridTarget responseLook,busyTarget;bool responsePositive;EvidenceSource busySource;
 public bool CanSeeCue(HybridTarget target,bool allowHeldToy=false,bool closeCue=false){
  if(target==null||!target.gameObject.activeInHierarchy||(target.GetComponent<CarryItem>()?.held==true&&!allowHeldToy))return false;
  var direction=target.transform.position-transform.position;direction.y=0;float unit=nav.world.Scale;
  if(direction.magnitude>unit*45||(!(closeCue&&direction.magnitude<unit*4)&&(direction.magnitude<unit*.5f||Vector3.Dot(transform.forward,direction.normalized)<.35f)))return false;
  Physics.SyncTransforms();var collider=target.GetComponent<Collider>();var eye=transform.position+Vector3.up*unit*10;
  var end=collider!=null&&collider.enabled?collider.bounds.center:target.transform.position+Vector3.up*unit;
  var toward=end-eye;
  foreach(var hit in Physics.RaycastAll(eye,toward.normalized,toward.magnitude,~0,QueryTriggerInteraction.Ignore).OrderBy(h=>h.distance)){
   if(hit.transform.IsChildOf(transform))continue;
   if(hit.transform==target.transform||hit.transform.IsChildOf(target.transform))return true;
   return false;
  }return true;
 }
 void Respond(CreatureResponse kind,HybridTarget target,string reason,bool mustSee=false,bool positive=true,EvidenceSource source=EvidenceSource.Autonomous){
  if(Season.paused||suspended||GetComponent<CarryItem>()?.held==true||(mustSee&&!CanSeeCue(target)))return;
  if(kind==CreatureResponse.Noticed||kind==CreatureResponse.Chosen||kind==CreatureResponse.Unavailable||kind==CreatureResponse.Completed)IslandAudio.Cue(kind==CreatureResponse.Unavailable||kind==CreatureResponse.Completed&&!positive?IslandSound.Uneasy:kind==CreatureResponse.Noticed?IslandSound.Notice:IslandSound.Content,transform.position,GetEntityId().GetHashCode());
  Response=kind;ResponseTarget=target!=null?target.id:"";ResponseReason=reason;responseStarted=State.time;responsePositive=positive;
  responseUntil=State.time+(kind==CreatureResponse.Noticed?1.2f:1.5f);responseLook=CanSeeCue(target)?target:null;
  if(kind==CreatureResponse.Noticed)curiosity.NoticeResponse(CreatureAffect.Mood.Curious,1.2f);
  else if(kind==CreatureResponse.Chosen)curiosity.NoticeResponse(CreatureAffect.Mood.Excited,1.5f);
  else if(kind==CreatureResponse.Unavailable)curiosity.NoticeResponse(CreatureAffect.Mood.Sad,1.2f);
  else if(kind==CreatureResponse.Completed){curiosity.NoticeResponse(positive?CreatureAffect.Mood.Happy:CreatureAffect.Mood.Sad,1.5f);if(positive&&!curiosity.FeedbackProtected)motion?.React();}
  if(kind==CreatureResponse.Noticed||kind==CreatureResponse.Busy)
   State.Evidence.Add(State.time,kind==CreatureResponse.Noticed?EvidenceKind.Noticed:EvidenceKind.Busy,
    source,CreatureAction.RespondToGuidance,ResponseTarget,reason,source==EvidenceSource.PlayerGuidance?State.guidance.evidenceId:0);
 }
 void NoticeCue(HybridTarget target,string reason,EvidenceSource source){
  Respond(CreatureResponse.Noticed,target,reason,true,true,source);
  if(Owner!=ActionOwner.None&&State.intention.target!=target.id){busyTarget=target;busyAt=State.time+1.3f;busySource=source;}
 }
 void NoticeGuidance(){
  if(Season.paused||suspended||GetComponent<CarryItem>()?.held==true)return;
  var g=State.guidance;if(g.status!=GuidanceStatus.Active||g.noticed||State.time>=g.expiresAt)return;var target=Find(g.target);
  if(!CanSeeCue(target))return;g.noticed=true;NoticeCue(target,"I noticed your suggestion",EvidenceSource.PlayerGuidance);
 }
 void AdvanceResponse(){
  if(State.time>=nextCueCheck){nextCueCheck=State.time+.25f;NoticeGuidance();}
  if(busyTarget!=null&&State.time>=busyAt){var target=busyTarget;busyTarget=null;
   bool current=(target==placedAttention&&State.time<attentionUntil)||(State.guidance.status==GuidanceStatus.Active&&State.guidance.target==target.id);
   if(current&&Owner!=ActionOwner.None&&State.intention.target!=target.id)Respond(CreatureResponse.Busy,target,"Continuing my current activity",true,true,busySource);
  }
  if(Response==CreatureResponse.None)return;
  if(State.time>=responseUntil){ClearResponse();return;}
  if(curiosity.FeedbackProtected){motion?.RespondTo(null,false,0);return;}
  float progress=Mathf.Clamp01((State.time-responseStarted)/1.2f);
  // Head-only overlay; current hands, activity, route and carry owner stay authoritative.
  motion?.RespondTo(responseLook!=null?responseLook.transform:null,Response==CreatureResponse.Chosen||(Response==CreatureResponse.Completed&&responsePositive),
   Response==CreatureResponse.Noticed?0:progress);
 }
 void FailAction(string reason){var target=Find(State.intention.target);RememberFailure(State.intention,target,reason);Interrupt(reason);Respond(CreatureResponse.Unavailable,target,reason);}
 void ClearResponse(){Response=CreatureResponse.None;ResponseTarget=ResponseReason="";responseLook=null;motion?.RespondTo(null,false,0);}
 void ResetResponses(){ClearResponse();busyTarget=null;nextCueCheck=0;}
}
}
