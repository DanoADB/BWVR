using System;
using System.Collections.Generic;
using UnityEngine;
namespace BWVR {
public enum IslandSound {Step,Grasp,Place,Snap,Ball,Pour,Harvest,Notice,Content,Uneasy,Miracle,Discovery,Chime,Bird,Thunder}
// Presentation only. No audio event grants a resource, teaches, or drives AI perception.
public sealed class IslandAudio:MonoBehaviour {
 public const int VoiceLimit=8;
 public static IslandAudio Current{get;private set;}
 public float Master{get;private set;}public float Ambience{get;private set;}public float Effects{get;private set;}
 public int Played{get;private set;}public int Dropped{get;private set;}public int ActiveVoices{get{int n=0;foreach(var v in voices)if(v.left>0)n++;return n;}}
 public bool Frozen=>frozen||suspended||season==null||season().paused;
 public float RainLevel=>beds[2].volume;
 public float NearDistance=>Mathf.Max(unit*12,rig.lossyScale.x*.4f);
 public float FarDistance=>Mathf.Max(unit*180,rig.lossyScale.x*5);
 Func<SliceState> season;IslandNavigation nav;Transform head,rig;float unit,clock,birdAt=8,duck;bool frozen,suspended;int birdSeed=743;
 AudioClip[] clips;readonly AudioSource[] beds=new AudioSource[3];readonly float[] bedGain=new float[3];readonly Voice[] voices=new Voice[VoiceLimit];readonly float[] groupAt=new float[15];readonly float[] emitterAt=new float[64];
 sealed class Voice {public AudioSource sound;public float left,gain;public int priority;}
 sealed class Walker {public HumanoidMotion motion;public CarryItem item;public Vector3 previous;public float distance;}
 readonly List<Walker> walkers=new List<Walker>();
 public static IslandAudio Build(Transform parent,IslandNavigation navigation,Transform head,Transform rig,Func<SliceState> state){
  var go=new GameObject("Island soundscape — bounded playback");go.transform.SetParent(parent,false);var a=go.AddComponent<IslandAudio>();a.nav=navigation;a.unit=navigation.Scale;a.head=head;a.rig=rig;a.season=state;Current=a;a.Initialize();foreach(var m in parent.GetComponentsInChildren<HumanoidMotion>())a.RegisterWalker(m);return a;
 }
 void Initialize(){
  Master=Read("master",.8f);Ambience=Read("ambience",.65f);Effects=Read("effects",.8f);
  var names=new[]{"step","grasp","place","snap","ball","pour","harvest","notice","content","uneasy","miracle","discovery","chime","bird","thunder"};clips=new AudioClip[names.Length];for(int i=0;i<names.Length;i++)clips[i]=Resources.Load<AudioClip>("IslandAudio/"+names[i]);
  string[] loops={"surf","wind","rain"};for(int i=0;i<3;i++){beds[i]=Source(loops[i]+" ambience",false);beds[i].loop=true;beds[i].priority=180;beds[i].clip=Resources.Load<AudioClip>("IslandAudio/"+loops[i]);beds[i].volume=0;if(beds[i].clip!=null)beds[i].Play();}
  for(int i=0;i<VoiceLimit;i++)voices[i]=new Voice{sound=Source("Context voice "+i,true)};
 }
 AudioSource Source(string label,bool spatial){var go=new GameObject(label);go.transform.SetParent(transform,false);var s=go.AddComponent<AudioSource>();s.playOnAwake=false;s.spatialBlend=spatial?1:0;s.dopplerLevel=0;s.reverbZoneMix=0;s.rolloffMode=AudioRolloffMode.Linear;s.priority=spatial?100:180;return s;}
 static float Read(string name,float fallback){float v=PlayerPrefs.GetFloat("BWVR.Audio."+name,fallback);return float.IsNaN(v)||float.IsInfinity(v)?fallback:Mathf.Clamp01(v);}
 public void SetMix(float master,float ambience,float effects,bool persist=true){
  if(!Finite(master)||!Finite(ambience)||!Finite(effects))return;Master=Mathf.Clamp01(master);Ambience=Mathf.Clamp01(ambience);Effects=Mathf.Clamp01(effects);ApplyMix();
  if(persist){PlayerPrefs.SetFloat("BWVR.Audio.master",Master);PlayerPrefs.SetFloat("BWVR.Audio.ambience",Ambience);PlayerPrefs.SetFloat("BWVR.Audio.effects",Effects);PlayerPrefs.Save();}
 }
 static bool Finite(float x)=>!float.IsNaN(x)&&!float.IsInfinity(x);
 public static bool Cue(IslandSound kind,Vector3 at,int emitter=0,float strength=1,float pitch=1)=>Current!=null&&Current.Emit(kind,at,emitter,strength,pitch);
 public bool Emit(IslandSound kind,Vector3 at,int emitter=0,float strength=1,float pitch=1){
  int k=(int)kind;if(Frozen||k<0||k>=clips.Length||clips[k]==null||Master*Effects<=0||!Finite(strength)||!Finite(pitch)||!Finite(at.x)||!Finite(at.y)||!Finite(at.z))return false;
  int slot=unchecked((emitter*397)^k)&63;float gate=kind==IslandSound.Step?.07f:kind==IslandSound.Ball?.1f:kind==IslandSound.Harvest?.2f:kind==IslandSound.Notice||kind==IslandSound.Content||kind==IslandSound.Uneasy?2:kind==IslandSound.Thunder?3:.07f;
  if((kind==IslandSound.Ball||kind==IslandSound.Place)&&clock<1)return false;
  if(clock<groupAt[k]||clock<emitterAt[slot]||Vector3.Distance(head.position,at)>FarDistance){Dropped++;return false;}
  int priority=kind==IslandSound.Step||kind==IslandSound.Bird?0:kind==IslandSound.Miracle||kind==IslandSound.Thunder||kind==IslandSound.Grasp?2:1;
  Voice voice=null;foreach(var v in voices)if(v.left<=0){voice=v;break;}
  if(voice==null)foreach(var v in voices)if(v.priority<priority&&(voice==null||v.priority<voice.priority))voice=v;
  if(voice==null){Dropped++;return false;}
  groupAt[k]=clock+gate;emitterAt[slot]=clock+(kind==IslandSound.Step?.3f:kind==IslandSound.Ball?.14f:.2f);
  voice.sound.Stop();voice.sound.transform.position=at;voice.sound.clip=clips[k];voice.sound.pitch=Mathf.Clamp(pitch,.65f,1.4f);voice.priority=priority;voice.sound.priority=priority==2?60:priority==1?100:160;
  float gain=kind==IslandSound.Step?.13f:kind==IslandSound.Thunder?.25f:kind==IslandSound.Bird?.16f:kind==IslandSound.Notice||kind==IslandSound.Content||kind==IslandSound.Uneasy?.14f:.28f;
  voice.gain=gain*Mathf.Clamp(strength,.05f,1);voice.left=voice.sound.clip.length/voice.sound.pitch;Distances(voice.sound);ApplyMix();voice.sound.Play();Played++;if(priority>0)duck=.65f;return true;
 }
 void Distances(AudioSource s){s.minDistance=NearDistance;s.maxDistance=FarDistance;}
 public void RegisterWalker(HumanoidMotion motion){if(motion==null||walkers.Exists(w=>w.motion==motion))return;var item=motion.GetComponentInParent<CarryItem>();if(item!=null)walkers.Add(new Walker{motion=motion,item=item,previous=motion.transform.position});}
 public void SetFrozen(bool pause){if(frozen==pause)return;frozen=pause;foreach(var v in voices){v.sound.Stop();v.left=0;}foreach(var b in beds){if(pause)b.Pause();else b.UnPause();}foreach(var w in walkers){if(w.motion!=null)w.previous=w.motion.transform.position;w.distance=0;}}
 public void Suspend(bool value){suspended=value;SetFrozen(value||season().paused);}
 public void Advance(float dt,bool paused){
  SetFrozen(paused||suspended);if(Frozen||dt<=0||!Finite(dt))return;dt=Mathf.Min(dt,.1f);clock+=dt;duck=Mathf.Max(0,duck-dt);
  for(int i=0;i<voices.Length;i++){var v=voices[i];v.left=Mathf.Max(0,v.left-dt);Distances(v.sound);}
  var weather=season().Weather;float rain=Mathf.Max(weather.rain,season().Hybrid.rainRemaining>0?.6f:0);
  for(int i=0;i<3;i++){float target=i==0?.18f+weather.wind*.08f:i==1?.035f+weather.wind*.18f:rain*.25f;bedGain[i]=Mathf.MoveTowards(bedGain[i],target,dt*.09f);}ApplyMix();
  if(clock>=birdAt){birdSeed=(int)((long)birdSeed*48271%2147483647);birdAt=clock+9+birdSeed%14;if(weather.rain<.25f&&weather.wind<.5f){float angle=(birdSeed%360)*Mathf.Deg2Rad;var at=head.position+new Vector3(Mathf.Cos(angle),-.25f,Mathf.Sin(angle))*NearDistance*2;Emit(IslandSound.Bird,at,777,.8f,1+(birdSeed%10)*.015f);}}
  for(int i=walkers.Count-1;i>=0;i--){var w=walkers[i];if(w.motion==null){walkers.RemoveAt(i);continue;}var at=w.motion.transform.position;var movement=at-w.previous;w.previous=at;float step=w.motion.transform.lossyScale.y*.42f;float distance=new Vector2(movement.x,movement.z).magnitude;
   if(w.item.held||w.motion.paused||distance>Mathf.Max(unit*12,step*2)||Mathf.Abs(at.y-nav.ProjectFoot(at,nav.villager).y)>unit*2){w.distance=0;continue;}
   if(distance<unit*.001f)continue;w.distance+=distance;if(w.distance>=step){w.distance%=Mathf.Max(unit,step);Emit(IslandSound.Step,at,w.item.GetEntityId().GetHashCode(),w.item.kind==Offering.Creature?1:.65f,w.item.kind==Offering.Creature?.7f:1);}
  }
 }
 void ApplyMix(){float headroom=Mathf.Min(1,2f/Mathf.Max(1,ActiveVoices));for(int i=0;i<3;i++)if(beds[i]!=null)beds[i].volume=bedGain[i]*Master*Ambience*(duck>0?.72f:1);foreach(var v in voices)if(v!=null)v.sound.volume=v.gain*Master*Effects*headroom;}
 void OnDisable(){foreach(var b in beds)if(b!=null)b.Stop();foreach(var v in voices)if(v!=null){v.sound.Stop();v.left=0;}}
 void OnDestroy(){if(Current==this)Current=null;}
}
}
