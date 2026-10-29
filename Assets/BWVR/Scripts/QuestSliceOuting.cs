using System.Linq;
using UnityEngine;
namespace BWVR {
public sealed partial class QuestSlice {
 LineRenderer outingBeacon;Material outingBeaconMaterial;Transform outingTarget;float outingMarkTime;PlayOutingState outingBound;
 public bool BeginPlayOuting(){if(state.paused||applicationSuspended||held.Any(i=>i!=null))return false;var p=state.playOuting??(state.playOuting=new PlayOutingState());if(!p.Begin(state.worldPlay))return false;outingBound=p;ClearOutingMark();notice="Play Day begun. Open the Outings pages for things to try.";return true;}
 public bool EndPlayOuting(){if(state.paused||applicationSuspended||state.playOuting?.End()!=true)return false;ClearOutingMark();notice="Outing ended. Your creature's learning stays.";return true;}
 public bool MarkPlayActivity(int page){if(state.paused||applicationSuspended||held.Any(i=>i!=null)||state.playOuting?.phase!=PlayOutingPhase.Active)return false;int activity=page==0?state.playOuting.Next:Mathf.Clamp((page-1)%3,0,2);var ball=balls.FirstOrDefault();Transform target=activity==0?playerMusic.FirstOrDefault()?.transform:activity==1?ball?.Goals?.Gates.FirstOrDefault(g=>g!=null&&g.gameObject.activeInHierarchy)?.transform:ball?.Puzzle?.Available==true?ball.Puzzle.Root:null;if(target==null||!target.gameObject.activeInHierarchy){notice="This activity has no available place yet.";return false;}
  if(outingBeacon==null){var root=new GameObject("Private outing location beacon");root.transform.SetParent(transform,false);outingBeacon=root.AddComponent<LineRenderer>();outingBeacon.useWorldSpace=false;outingBeacon.positionCount=20;outingBeaconMaterial=new Material(Shader.Find("BWVR/GameSurface")){color=new Color(.25f,.9f,.8f)};outingBeacon.sharedMaterial=outingBeaconMaterial;outingBeacon.shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off;float u=navigation.Scale;outingBeacon.widthMultiplier=.18f*u;outingBeacon.SetPosition(0,Vector3.zero);outingBeacon.SetPosition(1,Vector3.up*12*u);for(int i=2;i<20;i++){float a=(i-2)*Mathf.PI*2/17;outingBeacon.SetPosition(i,new Vector3(Mathf.Cos(a)*3,12,Mathf.Sin(a)*3)*u);}}
  outingBound=state.playOuting;outingTarget=target;outingMarkTime=45;outingBeacon.transform.position=target.position;outingBeacon.gameObject.SetActive(true);SetSanctuaryOpen(false);notice="A teal beacon marks the activity for 45 seconds.";return true;
 }
 void ClearOutingMark(){outingMarkTime=0;outingTarget=null;if(outingBeacon!=null)outingBeacon.gameObject.SetActive(false);}
 void AdvancePlayOuting(float dt,bool paused){if(outingBound!=state.playOuting){outingBound=state.playOuting;ClearOutingMark();}if(paused)return;int earned=state.playOuting?.Observe(state.worldPlay,false)??0;if(earned!=0){IslandAudio.Cue(IslandSound.Discovery,hybrid.transform.position,64910,.5f);notice=state.playOuting.phase==PlayOutingPhase.Complete?"Play Day complete. New field notes are in the Book.":"An outing experiment is recorded in the Book.";}
  if(outingMarkTime>0){outingMarkTime=Mathf.Max(0,outingMarkTime-dt);if(outingMarkTime<=0||outingTarget==null||!outingTarget.gameObject.activeInHierarchy)ClearOutingMark();else outingBeacon.transform.position=outingTarget.position;}
 }
 void DisposePlayOuting(){if(outingBeacon!=null)Destroy(outingBeacon.gameObject);if(outingBeaconMaterial!=null)Destroy(outingBeaconMaterial);}
}
}
