using UnityEngine;
namespace BWVR {
public enum PuzzleCue {Unavailable,NeedsKey,WaitingForHelper,HelperApproaching,Opening,ReadyToRoll,BallPassing,Solved}
// Presentation reads the puzzle; lights and sounds never award an outcome or drive its motor.
public sealed partial class CooperativePuzzle {
 public PuzzleCue Cue{get;private set;}
 public int ReadyChimes{get;private set;}
 public bool LaunchVisible=>launchRing!=null&&launchRing.enabled;
 public float CueClock=>cueClock;
 LineRenderer launchRing,arrow;Transform bluePulse,goldPulse;float cueClock,nextReadySound;bool cueInitialized;
 public static PuzzleCue SelectCue(bool available,bool completed,bool docked,bool active,bool onPad,bool open,bool passing)=>!available?PuzzleCue.Unavailable:completed?PuzzleCue.Solved:!docked?PuzzleCue.NeedsKey:!active?PuzzleCue.WaitingForHelper:!onPad?PuzzleCue.HelperApproaching:!open?PuzzleCue.Opening:passing?PuzzleCue.BallPassing:PuzzleCue.ReadyToRoll;
 void BuildCues(){
  launchRing=Line("Ball launch circle",48,true,.35f);for(int i=0;i<48;i++){float a=i*Mathf.PI/24;launchRing.SetPosition(i,new Vector3(Mathf.Cos(a)*6,.22f,-23+Mathf.Sin(a)*6));}
  arrow=Line("Ball direction chevron",3,false,.5f);arrow.SetPosition(0,new Vector3(-3,.23f,-14));arrow.SetPosition(1,new Vector3(0,.23f,-10));arrow.SetPosition(2,new Vector3(3,.23f,-14));
  bluePulse=Shape(Root,"Blue input signal",PrimitiveType.Sphere,Vector3.zero,Vector3.one*.9f,blue).transform;
  goldPulse=Shape(Root,"Paw input signal",PrimitiveType.Sphere,Vector3.zero,Vector3.one*.9f,gold).transform;
  ResetCues();
 }
 LineRenderer Line(string label,int points,bool loop,float width){var go=new GameObject(label);go.transform.SetParent(Root,false);var line=go.AddComponent<LineRenderer>();line.useWorldSpace=false;line.loop=loop;line.positionCount=points;line.widthMultiplier=nav.Scale*width;line.sharedMaterial=gold;line.shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off;line.receiveShadows=false;return line;}
 void ResetCues(){cueClock=nextReadySound=0;ReadyChimes=0;cueInitialized=false;PaintCues(0,false);}
 void PaintCues(float dt,bool sound=true){
  if(launchRing==null)return;cueClock+=Mathf.Max(0,dt);
  var next=SelectCue(Available,Completed,Docked,Active,OnPad,Open,crossing);
  if(sound&&cueInitialized&&next==PuzzleCue.ReadyToRoll&&Cue!=next&&god.State.time>=nextReadySound){nextReadySound=god.State.time+3;ReadyChimes++;IslandAudio.Cue(IslandSound.Chime,Pad.transform.position,Key.GetEntityId().GetHashCode(),.55f,1.3f);}
  Cue=next;cueInitialized=true;float pulse=.5f+.5f*Mathf.Sin(cueClock*2);
  Tint(socketLight,Docked?Color.white:Color.Lerp(blue.color,Color.white,pulse*.2f));
  Tint(padLight,OnPad?Color.white:Docked?Color.Lerp(gold.color,Color.white,pulse*.35f):gold.color);
  Tint(star,Completed?new Color(.5f,1,.7f):Open?gold.color:glow.color);
  launchRing.enabled=arrow.enabled=Cue==PuzzleCue.ReadyToRoll||Cue==PuzzleCue.BallPassing;
  var launchColor=Color.Lerp(gold.color,Color.white,pulse*.35f);Tint(launchRing,launchColor);Tint(arrow,launchColor);
  bluePulse.gameObject.SetActive(Available&&Docked);goldPulse.gameObject.SetActive(Available&&OnPad);
  float travel=Mathf.Repeat(cueClock*.25f,1);bluePulse.localPosition=SignalPoint(-1,travel);goldPulse.localPosition=SignalPoint(1,travel);
 }
 static Vector3 SignalPoint(int side,float t)=>t<.65f?Vector3.Lerp(new Vector3(side*19,.6f,-12),new Vector3(side*19,.6f,2),t/.65f):Vector3.Lerp(new Vector3(side*19,.6f,2),new Vector3(side*11,.6f,2),(t-.65f)/.35f);
 public void PresentHelper(HumanoidMotion motion){if(motion==null)return;motion.LookAt(OnPad?ball.transform:Key.transform);motion.Signal(false,Open&&!crossing,LaunchPoint-Vector3.up*nav.Scale*ball.Radius);}
}
}
