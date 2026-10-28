using System;
using UnityEngine;
namespace BWVR {
[Serializable]public sealed class SpinMemory {
 public int cues,spins,lastDirection;public float preference=.5f,practice;
 public bool Valid()=>cues>=0&&cues<=1000000&&spins>=0&&spins<=cues&&lastDirection>=-1&&lastDirection<=1&&HybridState.Number(preference,0,1)&&HybridState.Number(practice,0,1);
 public void Complete(int direction){spins=Math.Min(cues,spins+1);lastDirection=direction;practice=Mathf.Clamp01(practice+.04f);}
}
// Bounded gesture geometry in a stable creature-facing plane, measured in game units.
public sealed class CircleGesture {
 readonly Vector3[] points=new Vector3[64];readonly float[] times=new float[64];int count;
 public bool Drawing{get;private set;}
 public void Reset(){count=0;Drawing=false;}
 public bool Sample(Vector3 point,float now,out int direction){
  direction=0;if(!HybridState.Number(point.x,-10000,10000)||!HybridState.Number(point.y,-10000,10000)||!HybridState.Number(point.z,-10000,10000)||!HybridState.Number(now,0,100000000)){Reset();return false;}
  if(count>0&&(now<=times[count-1]||now-times[count-1]>.4f||(point-points[count-1]).magnitude>5)){Reset();}
  if(count>0&&(point-points[count-1]).sqrMagnitude<.09f){if(now-times[count-1]>.35f)Reset();return false;}
  while(count>0&&(now-times[0]>6||count==points.Length)){Array.Copy(points,1,points,0,--count);Array.Copy(times,1,times,0,count);}
  points[count]=point;times[count++]=now;if(count<5)return false;
  var min=points[0];var max=min;for(int i=1;i<count;i++){min=Vector3.Min(min,points[i]);max=Vector3.Max(max,points[i]);}var span=max-min;
  Drawing=span.x>1.5f&&span.y>1.5f&&span.z<2.5f&&now-times[0]>=.4f;
  if(count<16||now-times[0]<2||span.x<4||span.x>16||span.y<4||span.y>16||span.z>2.5f||span.x/span.y<.65f||span.x/span.y>1.5f||(points[count-1]-points[0]).magnitude>2.5f)return false;
  var center=(min+max)*.5f;float angle=0,travel=0;
  for(int i=1;i<count;i++){var a=new Vector2(points[i-1].x-center.x,points[i-1].y-center.y);var b=new Vector2(points[i].x-center.x,points[i].y-center.y);if(a.magnitude<1.5f||b.magnitude<1.5f)return false;float step=Vector2.SignedAngle(a,b);if(Mathf.Abs(step)>55)return false;angle+=step;travel+=Mathf.Abs(step);}
  if(Mathf.Abs(angle)<300||Mathf.Abs(angle)>440||travel-Mathf.Abs(angle)>45)return false;direction=angle>0?1:-1;Reset();return true;
 }
}
// A toy gesture invites a stationary turn. Curiosity remains the sole movement/action owner.
public sealed partial class BallSpin {
 readonly BeachBall ball;readonly CreatureHybrid god;readonly IslandNavigation nav;readonly CarryItem item;readonly CircleGesture gesture=new CircleGesture();
 static readonly Vector3[] Steps={Vector3.forward,Vector3.back,Vector3.left,Vector3.right};
 Vector3 anchor,lastPoint;bool havePoint;Quaternion frame,start;float sampleIn,until,cooldown,turned,safetyIn,verticalUntil;int direction;bool haveFrame,pending,noticed;
 public bool Active{get;private set;}public bool JustCompleted{get;private set;}
 public Vector3 Origin=>anchor;public Quaternion StartRotation=>start;
 public float Turned=>turned;public bool Drawing=>(gesture.Drawing||god.State.time<verticalUntil)&&!Active;
 bool Held=>item.held&&ball.Chase.PlayerHolding?.Invoke()==true;
 bool Allowed=>ball.Chase.InteractionAllowed?.Invoke()==true&&god.GetComponent<CarryItem>()?.held!=true&&ball.Goals?.Turns.Active!=true;
 public bool Ready=>pending&&!Active&&Held&&Allowed&&!god.Season.paused&&god.State.time<until&&god.CanSeeCue(ball.GetComponent<HybridTarget>(),true);
 public BallSpin(BeachBall ball,CreatureHybrid god,IslandNavigation nav){this.ball=ball;this.god=god;this.nav=nav;item=ball.GetComponent<CarryItem>();}
 public void Advance(float dt){
  if(dt<=0||god.Season.paused||Active)return;
  if(!Held||!Allowed||ball.Harmful||god.State.time<cooldown){ClearGesture();pending=false;return;}
  if(pending){if(god.State.time>=until){pending=false;ClearGesture();}return;}
  sampleIn-=dt;if(sampleIn>0)return;sampleIn=.1f;
  if(!god.CanSeeCue(ball.GetComponent<HybridTarget>(),true)||Vector3.Distance(ball.transform.position,god.transform.position)>nav.Scale*28){ClearGesture();return;}
  if(!haveFrame){anchor=god.transform.position;frame=god.transform.rotation;haveFrame=true;}
  if(Vector3.Distance(god.transform.position,anchor)>nav.Scale*3||Quaternion.Angle(frame,god.transform.rotation)>25){ClearGesture();return;}
  var point=Quaternion.Inverse(frame)*(ball.transform.position-anchor)/nav.Scale;
  if(point.y<3||point.y>22||point.z<6||point.z>24){ClearGesture();return;}
  if(havePoint&&Mathf.Abs(point.y-lastPoint.y)>.3f&&Mathf.Abs(point.z-lastPoint.z)<.4f)verticalUntil=god.State.time+.35f;lastPoint=point;havePoint=true;
  if(gesture.Sample(point,god.State.time,out var spin)){
   direction=spin;pending=true;until=god.State.time+8;var memory=god.Season.WorldPlay.Spin;memory.cues=Math.Min(1000000,memory.cues+1);NoticeCurve();IslandAudio.Cue(IslandSound.Notice,god.transform.position,ball.GetEntityId().GetHashCode(),.6f);
  }else if(Drawing&&!noticed){noticed=true;NoticeCurve();}
 }
 void NoticeCurve(){var mind=god.GetComponent<CreatureCuriosity>();if(mind.Current==CreatureCuriosity.Phase.ChaseBall)god.Interrupt("Curved toy motion superseded following");mind.NoticeAttention();}
 void ClearGesture(){gesture.Reset();haveFrame=havePoint=noticed=false;sampleIn=verticalUntil=0;}
 public bool Begin(){if(!Ready||!SafeSpot())return false;ball.CancelFetch();ball.CancelRally();ball.Goals?.Cancel();ball.Chase.End();ball.Chase.ClearCompletion();ball.JustFetched=false;Active=true;JustCompleted=false;turned=0;anchor=god.transform.position;start=god.transform.rotation;safetyIn=0;pending=false;ClearGesture();InviteAudience();return true;}
 bool SafeSpot(){var navigator=god.GetComponent<IslandNavigator>();if(!navigator.CanPlace(god.transform.position,out var foot))return false;float radius=nav.creature.radius+.5f;foreach(var step in Steps)if(!nav.Sample(foot+step*radius*nav.Scale,nav.creature,1,out var edge)||Mathf.Abs(nav.ProjectFoot(edge,nav.creature).y-foot.y)>nav.Scale*1.25f)return false;return true;}
 public int Step(float dt){
  if(!Active||!Held||!Allowed||ball.Harmful||Vector3.Distance(ball.transform.position,anchor)>nav.Scale*28||Vector3.Distance(god.transform.position,anchor)>nav.Scale*.5f)return -1;
  if(god.Season.paused||dt<=0)return 0;safetyIn-=dt;if(safetyIn<=0){safetyIn=.1f;if(!SafeSpot())return -1;}turned=Mathf.Min(360,turned+Mathf.Min(dt,.1f)*120);god.transform.rotation=Quaternion.AngleAxis(direction*turned,Vector3.up)*start;
  if(turned<360)return 0;
  if(!god.CanSeeCue(ball.GetComponent<HybridTarget>(),true))return -1;
  god.Season.WorldPlay.Spin.Complete(direction);JustCompleted=true;FinishAudience(true);End();god.Journal.Add(god.State.time,"toy spin",CreatureAction.PlayWithToy,ball.StableId,"Completed a grounded turn from a witnessed toy circle");return 1;
 }
 public void End(){FinishAudience(false);if(Active||pending)cooldown=god.State.time+12;Active=pending=false;ClearGesture();}
 public void Cancel(){End();JustCompleted=false;}
 public void ClearCompletion(){JustCompleted=false;}
 public void Reset(){End();JustCompleted=false;cooldown=until=turned=0;}
}
}
