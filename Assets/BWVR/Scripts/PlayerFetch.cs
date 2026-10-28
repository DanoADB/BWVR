using UnityEngine;
namespace BWVR {
// Short, bounded hand-motion history. Stale samples/tracking jumps never become throws.
public sealed class ThrowMotion {
 Vector3 previous,velocity;float last;int samples;
 public void Reset(){samples=0;velocity=Vector3.zero;}
 public Vector3 Velocity=>samples>=3?velocity:Vector3.zero;
 public void Sample(Vector3 point,float now,float scale){
  float dt=now-last;if(samples==0||dt<=0||dt>.15f||(point-previous).magnitude>scale*.5f){Reset();samples=1;}
  else{velocity=Vector3.Lerp(velocity,(point-previous)/dt,.5f);samples=Mathf.Min(3,samples+1);}
  previous=point;last=now;
 }
}
public sealed partial class BeachBall {
 public int FetchTicket{get;private set;}public bool JustFetched{get;internal set;}
 bool fetchPending,fetchClaimed;float fetchUntil;LineRenderer fetchRing;
 public Vector3 FetchHome=>item.home;
 public bool FetchReady=>fetchPending&&!fetchClaimed&&creature.State.time<fetchUntil&&!item.held&&!InFlight&&!Harmful&&Body.linearVelocity.magnitude<6;
 public bool FetchReserved=>fetchPending&&creature.State.time<fetchUntil;
 // A physical toss invites play; it does not issue a motor command or reward the creature.
 public bool PlayerToss(Vector3 worldVelocity,float playerScale=1){
  if(float.IsNaN(worldVelocity.sqrMagnitude)||float.IsInfinity(worldVelocity.sqrMagnitude)||float.IsNaN(playerScale)||float.IsInfinity(playerScale)||playerScale<=0||creature==null||creature.Season.paused||worldVelocity.sqrMagnitude<1e-10f||!nav.Sample(transform.position,nav.creature,12,out var ground))return false;
  var velocity=Vector3.ClampMagnitude(worldVelocity/playerScale*18,24);if(velocity.magnitude<1)return false;
  CancelRally();transform.position=new Vector3(ground.x,Mathf.Clamp(transform.position.y,ground.y+Radius*nav.Scale,ground.y+30*nav.Scale),ground.z);
  Throw(velocity,false,velocity.magnitude);Body.linearVelocity=velocity+Vector3.up*3;
  fetchPending=true;fetchClaimed=false;fetchUntil=creature.State.time+90;FetchTicket=FetchTicket==int.MaxValue?1:FetchTicket+1;
  GetComponent<CreatureAffordance>().cooldown=0;ShowFetchRing();if(Bowling?.TryPlayerRoll(velocity)==true)CancelFetch();return true;
 }
 void ShowFetchRing(){
  if(fetchRing==null){var go=new GameObject("Fetch return circle");go.transform.SetParent(transform.parent,false);fetchRing=go.AddComponent<LineRenderer>();fetchRing.useWorldSpace=true;fetchRing.sharedMaterial=material;fetchRing.loop=true;fetchRing.positionCount=32;fetchRing.widthMultiplier=nav.Scale*.4f;fetchRing.startColor=fetchRing.endColor=new Color(1,.8f,.25f);fetchRing.shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off;}
  var centre=nav.ProjectFoot(item.home,nav.creature)+Vector3.up*nav.Scale*.2f;for(int i=0;i<32;i++){float a=i*Mathf.PI*2/32;fetchRing.SetPosition(i,centre+new Vector3(Mathf.Cos(a),0,Mathf.Sin(a))*nav.Scale*6);}fetchRing.enabled=true;
 }
 public bool ClaimFetch(int ticket){if(ticket!=FetchTicket||!FetchReady)return false;fetchClaimed=true;return true;}
 public bool OwnsFetch(int ticket)=>fetchPending&&fetchClaimed&&ticket==FetchTicket&&creature.State.time<fetchUntil;
 public bool CompleteFetch(int ticket){if(!OwnsFetch(ticket))return false;ReturnHome();JustFetched=true;creature.Season.WorldPlay.fetchReturns=System.Math.Min(1000000,creature.Season.WorldPlay.fetchReturns+1);creature.Journal.Add(creature.State.time,"fetch returned",CreatureAction.PlayWithToy,StableId,"Ball carried to the play circle");return true;}
 public void CancelFetch(int ticket=-1){if(ticket!=-1&&ticket!=FetchTicket)return;fetchPending=fetchClaimed=false;if(fetchRing!=null)fetchRing.enabled=false;}
 void AdvanceFetch(){if(fetchPending&&(creature.State.time>=fetchUntil||Harmful))CancelFetch();}
}
}
