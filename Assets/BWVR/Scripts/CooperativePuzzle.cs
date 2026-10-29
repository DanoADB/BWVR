using System;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace BWVR {
[Serializable]public sealed class PuzzleMemory {
 public int attempts,solved;public float enjoyment=.5f;
 public bool Valid()=>attempts>=0&&attempts<=1000000&&solved>=0&&solved<=attempts&&HybridState.Number(enjoyment,0,1);
 public void Complete(){solved=Math.Min(attempts,solved+1);enjoyment=Mathf.Clamp01(enjoyment+.06f);}
}
// A fixed pad is an affordance, not a grabbable prop or a second AI controller.
public sealed class PuzzlePad:MonoBehaviour {public CooperativePuzzle puzzle;}
// The player can use either hand's normal pickup/drop to dock this toy.
public sealed class PuzzleKey:MonoBehaviour {public CooperativePuzzle puzzle;}
public sealed partial class CooperativePuzzle {
 public readonly Transform Root;public readonly PuzzlePad Pad;public readonly CarryItem Key;
 public bool Available{get;private set;}public bool Active{get;private set;}public bool Completed{get;private set;}
 public bool Docked{get;private set;}public float Lift{get;private set;}
 public bool OnPad=>Active&&FlatDistance(god.transform.position,Pad.transform.position)<nav.Scale*2&&Mathf.Abs(god.transform.position.y-Pad.transform.position.y)<nav.Scale*2;
 public bool Open=>OnPad&&Docked&&Lift>=.98f;
 public Vector3 Socket=>Root.TransformPoint(new Vector3(-19,2,-12));
 public Vector3 LaunchPoint=>Root.TransformPoint(new Vector3(0,ball.Radius,-23));
 public bool Ready=>Available&&!Active&&!Completed&&Docked&&!ball.InFlight&&!ball.Inviting&&!ball.PeerGame&&!ball.FetchReserved&&ball.Bowling?.Active!=true&&!ball.Goals.Active&&Allowed&&god.State.time>=cooldown&&!god.Season.paused&&god.CanSeeCue(Pad.GetComponent<HybridTarget>(),false,true);
 readonly BeachBall ball;readonly CreatureHybrid god;readonly IslandNavigation nav;readonly Transform shutter;
 readonly Rigidbody door;readonly Material stone,blue,gold,glow;readonly Renderer socketLight,padLight,star;
 readonly MaterialPropertyBlock paint=new MaterialPropertyBlock();
 float cooldown,elapsed,settle;bool crossing,havePrevious;Vector3 previous;
 bool Allowed=>ball.Chase.InteractionAllowed?.Invoke()==true&&god.GetComponent<CarryItem>()?.held!=true&&ball.Goals.Turns.Active!=true&&!ball.Harmful;
 PuzzleMemory Memory=>god.Season.WorldPlay.Puzzle;
 public CooperativePuzzle(BeachBall ball,CreatureHybrid god,IslandNavigation nav,Scene physics){
  this.ball=ball;this.god=god;this.nav=nav;
  Root=new GameObject("Together gate — key, paw pad and ball puzzle").transform;Root.SetParent(ball.transform.parent,false);Root.localScale=Vector3.one*nav.Scale;
  stone=Mat(new Color(.31f,.36f,.4f));blue=Mat(new Color(.16f,.7f,.85f));gold=Mat(new Color(1,.63f,.15f));glow=Mat(new Color(.4f,.43f,.44f));
  Root.rotation=Quaternion.LookRotation(ball.Bowling.Forward);
  var origin=ball.Bowling.Start;float bestSlope=float.MaxValue;for(int i=0;i<800;i++){
   float angle=i*137.5f*Mathf.Deg2Rad;var wanted=origin+new Vector3(Mathf.Cos(angle),0,Mathf.Sin(angle))*(50+i*.2f)*nav.Scale;
   var laneOffset=Quaternion.Inverse(Root.rotation)*(wanted-origin)/nav.Scale;if(Mathf.Abs(laneOffset.x)<28&&laneOffset.z>-18&&laneOffset.z<110)continue;bool overlaps=false;foreach(var gate in ball.Goals.Gates)if(Vector3.Distance(gate.transform.position,wanted)<nav.Scale*40)overlaps=true;if(overlaps)continue;
   if(!nav.Sample(wanted,nav.creature,2,out var foot))continue;foot=nav.ProjectFoot(foot,nav.creature);bool safe=true;float slope=0;
   foreach(var point in new[]{new Vector3(0,0,-23),new Vector3(0,0,12),new Vector3(19,0,-12),new Vector3(-19,0,-12),new Vector3(-12,0,0),new Vector3(12,0,0)}){
    if(!nav.Sample(foot+Root.rotation*point*nav.Scale,nav.creature,1,out var edge)){safe=false;break;}slope=Mathf.Max(slope,Mathf.Abs(nav.ProjectFoot(edge,nav.creature).y-foot.y)/nav.Scale);
   }
   if(safe&&slope<1.25f&&slope<bestSlope){Root.position=foot;Available=true;bestSlope=slope;if(slope<.25f)break;}
  }
  Shape(Root,"Stone play threshold",PrimitiveType.Cube,new Vector3(0,.05f,0),new Vector3(28,.1f,9),stone);
  SourcedPlayArt.Part("puzzle-frame",Root,"Sourced timber puzzle frame",new Vector3(0,10,0),new Vector3(28,20,4));
  for(int i=-1;i<=1;i+=2)Proxy(physics,"Puzzle pier",new Vector3(i*12,9,0),new Vector3(3,18,4));
  shutter=SourcedPlayArt.Part("shutter",Root,"Rising wooden shutter",new Vector3(0,8,0),new Vector3(21,16,1)).transform;
  door=Proxy(physics,"Puzzle shutter physics",new Vector3(0,8,0),new Vector3(21,16,1));
  // A modest flat sill avoids snagging the sphere on small sampled terrain differences.
  Proxy(physics,"Puzzle threshold physics",new Vector3(0,-.25f,0),new Vector3(28,.5f,9));
  Proxy(physics,"Puzzle catch net physics",new Vector3(0,8,12),new Vector3(28,16,.3f));
  SourcedPlayArt.Part("backstop",Root,"Sourced puzzle catch backboard",new Vector3(0,8,12),new Vector3(28,16,.3f));
  var pad=Shape(Root,"Creature paw pad",PrimitiveType.Cylinder,new Vector3(19,.1f,-12),new Vector3(12,.1f,12),gold);padLight=pad.GetComponent<Renderer>();
  var padTarget=new GameObject("Stand here to help");padTarget.transform.SetParent(Root,false);padTarget.transform.localPosition=new Vector3(19,0,-12);Pad=padTarget.AddComponent<PuzzlePad>();Pad.puzzle=this;var padItem=padTarget.AddComponent<CarryItem>();padItem.kind=Offering.Toy;padItem.home=padTarget.transform.position;var area=padTarget.AddComponent<BoxCollider>();area.center=Vector3.up*.3f;area.size=new Vector3(12,.6f,12);
  var affordance=padTarget.AddComponent<CreatureAffordance>();affordance.kind=CreatureAffordance.Kind.Turn;god.RegisterObject(affordance,"together-gate-pad");
  Shape(Root,"Blue key socket",PrimitiveType.Cylinder,new Vector3(-19,.1f,-12),new Vector3(8,.1f,8),stone);
  var socketToken=SourcedPlayArt.Part("key",Root,"Matching blue key socket",new Vector3(-19,.5f,-12),new Vector3(3,3,.5f),blue);socketToken.transform.localRotation=Quaternion.Euler(90,0,0);socketLight=socketToken.GetComponent<Renderer>();
  // Coloured inset tracks connect each input to the same shutter.
  foreach(int sign in new[]{-1,1}){Shape(Root,"Input track",PrimitiveType.Cube,new Vector3(sign*19,.12f,-5),new Vector3(.5f,.15f,14),sign<0?blue:gold);Shape(Root,"Gate input track",PrimitiveType.Cube,new Vector3(sign*15,.12f,2),new Vector3(8,.15f,.5f),sign<0?blue:gold);}
  for(int i=0;i<4;i++){var toe=Shape(pad.transform,"Paw toe",PrimitiveType.Sphere,Vector3.zero,Vector3.one,stone);toe.transform.SetParent(Root,false);toe.transform.localPosition=new Vector3(16.3f+i*1.8f,.25f,-10);toe.transform.localScale=new Vector3(1.5f,.25f,1.5f);}
  Shape(Root,"Paw sole",PrimitiveType.Sphere,new Vector3(19,.25f,-13),new Vector3(4,.25f,3),stone);
  star=Shape(Root,"Shared discovery jewel",PrimitiveType.Sphere,new Vector3(0,21,0),Vector3.one*2.5f,glow).GetComponent<Renderer>();
  var key=SourcedPlayArt.Part("key",Root,"Blue key token",Vector3.zero,Vector3.one);key.AddComponent<BoxCollider>();key.name="Blue key toy — drop into matching socket";key.transform.SetParent(Root,false);key.transform.localPosition=new Vector3(-27,2,-17);key.transform.localScale=new Vector3(4,4,.8f);key.GetComponent<Renderer>().sharedMaterial=blue;Key=key.AddComponent<CarryItem>();Key.kind=Offering.Toy;Key.home=key.transform.position;key.AddComponent<PuzzleKey>().puzzle=this;
  if(Available){nav.AddToyBarrier(Root.position+Vector3.up*nav.Scale*10,new Vector3(28,20,4)*nav.Scale,Root.rotation);nav.AddToyBarrier(Root.TransformPoint(new Vector3(0,8,12)),new Vector3(28,16,.3f)*nav.Scale,Root.rotation);}
  BuildCues();Root.gameObject.SetActive(Available);door.GetComponent<Collider>().enabled=Available;if(!Available)Debug.LogWarning("BWVR_PUZZLE_UNAVAILABLE: no clear level site for together gate");Reset();
 }
 static float FlatDistance(Vector3 a,Vector3 b){a.y=b.y=0;return Vector3.Distance(a,b);}
 static Material Mat(Color color)=>new Material(Shader.Find("BWVR/GameSurface")){color=color,enableInstancing=true};
 static GameObject Shape(Transform root,string name,PrimitiveType type,Vector3 at,Vector3 scale,Material material)=>SourcedPlayArt.Part(name.Contains("jewel")?"token":type==PrimitiveType.Cube?"plate":"stud",root,name,at,type==PrimitiveType.Cylinder?Vector3.Scale(scale,new Vector3(1,2,1)):scale,material);
 Rigidbody Proxy(Scene scene,string name,Vector3 at,Vector3 size){var go=new GameObject(name);SceneManager.MoveGameObjectToScene(go,scene);var box=go.AddComponent<BoxCollider>();box.size=size;box.contactOffset=.01f;box.enabled=Available;var body=go.AddComponent<Rigidbody>();body.isKinematic=true;body.position=nav.ToNav(Root.TransformPoint(at));body.rotation=Root.rotation;return body;}
 void Tint(Renderer view,Color color){paint.SetColor("_Color",color);view.SetPropertyBlock(paint);}
 public void Advance(float dt){
  if(!Available||god.Season.paused||dt<=0)return;
  bool was=Docked;Docked=!Key.held&&Vector3.Distance(Key.transform.position,Socket)<nav.Scale*5;
  if(Docked)Key.transform.position=Socket;
  if(ball.GetComponent<CarryItem>().held)BallMoved();
  if(Docked&&!was){god.GetComponent<CreatureCuriosity>().NoticeAttention();IslandAudio.Cue(IslandSound.Chime,Socket,Key.GetEntityId().GetHashCode(),.6f);}
  if(Completed&&!Docked&&god.Owner==CreatureHybrid.ActionOwner.None)Completed=false;
  if(Active&&!Allowed)Cancel();
  Lift=Mathf.MoveTowards(Lift,Active&&OnPad&&Docked&&Allowed?1:0,Mathf.Min(dt,.1f)*1.5f);
  shutter.localPosition=new Vector3(0,8+Lift*18,0);door.position=nav.ToNav(shutter.position);door.rotation=Root.rotation;
  PaintCues(Mathf.Min(dt,.1f));
 }
 public bool Claim(){if(!Ready||!god.CanReach(Pad.transform.position))return false;Active=true;Completed=false;crossing=havePrevious=false;elapsed=settle=0;Memory.attempts=Math.Min(1000000,Memory.attempts+1);ball.CancelFetch();ball.CancelRally();ball.Goals.Cancel();ball.Chase.End();return true;}
 public int Step(IslandNavigator walker,float speed,float dt){
  if(Completed)return 1;if(!Active||!Docked||!Allowed)return -1;if(god.Season.paused||dt<=0)return 0;
  elapsed+=dt;if(elapsed>70)return -1;
  if(!OnPad){walker.MoveTo(Pad.transform.position,speed,dt);if(walker.Blocked&&elapsed>6)return -1;}else{walker.ResetRoute();var look=Root.position-god.transform.position;look.y=0;if(look.sqrMagnitude>1e-10f)god.transform.rotation=Quaternion.RotateTowards(god.transform.rotation,Quaternion.LookRotation(look),dt*160);}
  return 0;
 }
 public void AfterPhysics(float dt){
  if(!Active||!Open||god.Season.paused||!Allowed||ball.GetComponent<CarryItem>().held){havePrevious=crossing=false;settle=0;return;}
  var now=Quaternion.Inverse(Root.rotation)*(ball.Body.position-nav.ToNav(Root.position));
  bool jump=havePrevious&&(now-previous).magnitude>Mathf.Max(2,dt*80);
  if(jump){crossing=false;settle=0;}
  if(havePrevious&&!jump&&!crossing&&previous.z<0&&now.z>=0&&GoalCourse.Crosses(previous,now,ball.Radius)){crossing=true;settle=0;}
  previous=now;havePrevious=true;
  if(crossing){if(now.z<0||ball.Harmful){crossing=false;settle=0;return;}settle+=dt;if(settle>=1){Completed=true;Active=false;Memory.Complete();god.Journal.Add(god.State.time,"cooperative puzzle",CreatureAction.PlayWithToy,Pad.GetComponent<HybridTarget>().id,"Player key, creature pad and safe physical ball passage completed together");IslandAudio.Cue(IslandSound.Discovery,Root.position,Key.GetEntityId().GetHashCode(),.8f,1.2f);}}
 }
 public void BallMoved(){havePrevious=crossing=false;settle=0;}
 public void Cancel(){if(Active||Completed)cooldown=god.State.time+12;Active=false;crossing=havePrevious=false;settle=0;PaintCues(0,false);}
 public void Reset(){Cancel();Completed=Docked=false;Lift=cooldown=elapsed=0;Key.held=false;Key.transform.position=Key.home;Key.GetComponent<Collider>().enabled=true;shutter.localPosition=new Vector3(0,8,0);door.position=nav.ToNav(shutter.position);ResetCues();}
 public void Dispose(){foreach(var mat in new[]{stone,blue,gold,glow})if(mat!=null)UnityEngine.Object.Destroy(mat);if(Root!=null)UnityEngine.Object.Destroy(Root.gameObject);}
}
}
