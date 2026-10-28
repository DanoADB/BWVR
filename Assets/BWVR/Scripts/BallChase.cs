using System;
using UnityEngine;
namespace BWVR {
[Serializable]public sealed class ChaseMemory {
 public int games;public float enjoyment=.4f,preference=.5f,bestDistance;
 public bool Valid()=>games>=0&&games<=1000000&&HybridState.Number(enjoyment,0,1)&&HybridState.Number(preference,0,1)&&HybridState.Number(bestDistance,0,200);
 public void Complete(float distance){games=Math.Min(1000000,games+1);enjoyment=Mathf.Clamp01(enjoyment+.04f);bestDistance=Mathf.Max(bestDistance,Mathf.Clamp(distance,0,200));}
}
// A held ball invites a bounded follow game. This service has no motor or carry ownership.
public sealed class BallChase {
 readonly BeachBall ball;readonly CreatureHybrid god;readonly IslandNavigation nav;readonly CarryItem item;
 public Func<bool> PlayerHolding,InteractionAllowed;
 public bool Active{get;private set;}public bool JustCompleted{get;private set;}
 public bool Ready=>ball.Goals?.Turns.Active!=true&&!ball.Harmful&&!Active&&visible&&warm>=.75f&&god.State.time>=cooldown&&Held&&Allowed;
 public float Distance{get;private set;}public Vector3 Goal{get;private set;}
 bool Held=>item.held&&PlayerHolding?.Invoke()==true;
 bool Allowed=>InteractionAllowed?.Invoke()==true&&god.GetComponent<CarryItem>()?.held!=true;
 float sampleIn,warm,cooldown,seenAt=-100,elapsed,targetTravel,blocked;bool visible,noticed,haveSample;
 Vector3 previousToy,previousGod;
 public BallChase(BeachBall ball,CreatureHybrid god,IslandNavigation nav){this.ball=ball;this.god=god;this.nav=nav;item=ball.GetComponent<CarryItem>();}
 public void Advance(float dt){
  if(dt<=0||god.Season.paused)return;
  if(!Held||!Allowed){visible=false;warm=0;noticed=false;haveSample=false;return;}
  sampleIn-=dt;if(sampleIn>0)return;sampleIn=.25f;
  var position=ball.transform.position;float travel=haveSample?Vector3.Distance(position,previousToy)/nav.Scale:0;previousToy=position;haveSample=true;
  var projected=nav.Terrain!=null?nav.Terrain.Project(position):new Vector3(position.x,god.transform.position.y,position.z);
  visible=travel<=12&&Mathf.Abs(position.y-projected.y)<=nav.Scale*28&&god.CanSeeCue(ball.GetComponent<HybridTarget>(),true)&&nav.Sample(projected,nav.creature,6,out var foot);
  if(!visible){warm=0;return;}
  seenAt=god.State.time;warm+=.25f;if(Active)targetTravel=Mathf.Min(200,targetTravel+travel);
  var direction=projected-god.transform.position;direction.y=0;var wanted=projected-(direction.sqrMagnitude>.00000001f?direction.normalized:god.transform.forward)*(ball.Radius+nav.creature.radius+1)*nav.Scale;
  if(!nav.Sample(wanted,nav.creature,5,out var goal)){visible=false;warm=0;return;}Goal=nav.ProjectFoot(goal,nav.creature);
  if(Ready&&!noticed){noticed=true;god.GetComponent<CreatureCuriosity>().NoticeAttention();IslandAudio.Cue(IslandSound.Notice,god.transform.position,ball.GetEntityId().GetHashCode(),.6f);}
 }
 public bool Begin(){if(!Ready)return false;ball.CancelFetch();ball.CancelRally();ball.JustFetched=false;Active=true;JustCompleted=false;Distance=targetTravel=elapsed=blocked=0;previousGod=god.transform.position;return true;}
 // Existing curiosity executor owns navigation; callers interpret -1 failure / 0 running / 1 complete.
 public int Step(IslandNavigator navigator,float speed,float dt){
  if(!Active||!Allowed||god.GetComponent<CarryItem>()?.held==true)return -1;
  elapsed+=dt;if(!Held){bool success=elapsed>=3&&Distance>=8&&targetTravel>=6&&god.State.time-seenAt<=.75f&&Vector3.Distance(ball.transform.position,god.transform.position)<nav.Scale*32;if(!success)return -1;god.Season.WorldPlay.Chase.Complete(Distance);JustCompleted=true;End();god.Journal.Add(god.State.time,"shared chase",CreatureAction.PlayWithToy,ball.StableId,"Followed a player-held ball for "+Mathf.RoundToInt(Distance)+" game units");return 1;}
  if(elapsed>25||god.State.time-seenAt>1||!visible)return -1;
  navigator.MoveTo(Goal,speed*1.1f,dt);float moved=Vector3.Distance(god.transform.position,previousGod)/nav.Scale;previousGod=god.transform.position;Distance=Mathf.Min(200,Distance+Mathf.Min(moved,10));blocked=navigator.Blocked?blocked+dt:0;return blocked>2?-1:0;
 }
 public void End(){if(Active)cooldown=god.State.time+10;Active=false;visible=false;warm=0;noticed=false;haveSample=false;}
 public void ClearCompletion(){JustCompleted=false;}
 public void Reset(){End();JustCompleted=false;cooldown=sampleIn=0;seenAt=-100;}
}
}
