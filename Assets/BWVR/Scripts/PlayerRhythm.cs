using System;
using System.Linq;
using UnityEngine;
namespace BWVR {
[Serializable]public sealed class EchoMemory {
 public int demonstrations,games;public float preference=.5f;public int[] notes=Array.Empty<int>();public float[] intervals=Array.Empty<float>();
 public bool Valid()=>demonstrations>=0&&demonstrations<=1000000&&games>=0&&games<=demonstrations&&HybridState.Number(preference,0,1)&&notes!=null&&intervals!=null&&(notes.Length==0&&intervals.Length==0||notes.Length==4&&intervals.Length==3)&&notes.All(n=>n==0||n==1)&&intervals.All(i=>HybridState.Number(i,.2f,1.2f));
 public void Remember(int[] pattern,float[] timing){games=Math.Min(demonstrations,games+1);notes=(int[])pattern.Clone();intervals=(float[])timing.Clone();}
}
public sealed class ChimeKey:MonoBehaviour {public int note;public PlayerRhythm owner;}
// The player performs a four-note invitation; the existing curiosity controller executes its echo.
public sealed class PlayerRhythm:MonoBehaviour {
 CreatureHybrid god;CarryItem item;CreatureAffordance affordance;SharedRhythm shared;HumanoidMotion performer;Func<bool> allowed;
 public bool Claimed{get;private set;}public bool Active{get;private set;}public bool Completed{get;private set;}public int Played{get;private set;}
 public int[] PlayedNotes{get;private set;}=Array.Empty<int>();public float[] PlayedTimes{get;private set;}=Array.Empty<float>();
 public bool Reserved=>count>0||Claimed||Active;
 public bool Ready=>god!=null&&count==4&&!Claimed&&!Active&&!item.held&&affordance.host==null&&!shared.Active&&!god.Season.paused&&god.State.time<expires&&god.CanSeeCue(GetComponent<HybridTarget>(),false,true);
 readonly int[] notes=new int[4];readonly float[] intervals=new float[3],flashes=new float[2];readonly Renderer[] bars=new Renderer[2],pips=new Renderer[4];MaterialPropertyBlock tint;
 int count;float lastTap=-100,expires,playClock,nextBeat;bool pending;
 static readonly Color[] Colors={new Color(.22f,.65f,.8f),new Color(1,.66f,.22f)};
 void Awake(){tint=new MaterialPropertyBlock();item=GetComponent<CarryItem>();affordance=GetComponent<CreatureAffordance>();shared=GetComponent<SharedRhythm>();foreach(var key in GetComponentsInChildren<ChimeKey>()){key.owner=this;bars[key.note]=key.GetComponent<Renderer>();Paint(bars[key.note],Colors[key.note]);}
  for(int i=0;i<4;i++){var pip=GameObject.CreatePrimitive(PrimitiveType.Sphere);pip.name="Pattern bead "+i;pip.transform.SetParent(transform,false);pip.transform.localPosition=new Vector3((i-1.5f)*.23f,1.2f,-.55f);pip.transform.localScale=new Vector3(.11f,.7f,.18f);pip.GetComponent<Collider>().enabled=false;Destroy(pip.GetComponent<Collider>());pips[i]=pip.GetComponent<Renderer>();pips[i].sharedMaterial=GetComponent<Renderer>().sharedMaterial;Paint(pips[i],new Color(.23f,.21f,.16f));}
 }
 public void Configure(CreatureHybrid creature,Func<bool> inputAllowed){god=creature;allowed=inputAllowed;}
 void Paint(Renderer view,Color color){if(view==null)return;tint.Clear();tint.SetColor("_Color",color);view.SetPropertyBlock(tint);}
 public bool Tap(int note){
  if(god==null||note<0||note>1||god.Season.paused||allowed?.Invoke()!=true||item.held||affordance.host!=null||shared.Active||Claimed||Active||god.State.time-lastTap<.15f||(god.Owner!=CreatureHybrid.ActionOwner.None&&god.State.intention.target==GetComponent<HybridTarget>().id))return false;
  float now=god.State.time;float gap=now-lastTap;lastTap=now;Play(note,null);
  if(!god.CanSeeCue(GetComponent<HybridTarget>(),false,true)){Cancel();return true;}
  if(count==4||gap>2.5f){count=0;pending=false;ClearBeads();}Completed=false;
  if(count>0)intervals[count-1]=Mathf.Clamp(gap,.2f,1.2f);notes[count]=note;Paint(pips[count],Colors[note]);count++;expires=now+(count==4?25:2.5f);
  if(count==4){pending=true;god.Season.WorldPlay.Echo.demonstrations=Math.Min(1000000,god.Season.WorldPlay.Echo.demonstrations+1);god.GetComponent<CreatureCuriosity>().NoticeAttention();}return true;
 }
 public void Advance(float dt){if(god==null||god.Season.paused||dt<=0)return;for(int i=0;i<2;i++)if(flashes[i]>0){flashes[i]=Mathf.Max(0,flashes[i]-dt);if(flashes[i]==0)Paint(bars[i],Colors[i]);}if(Reserved&&(item.held||affordance.host!=null||shared.Active||god.State.time>=expires))Cancel();}
 public bool Claim(){if(!Ready)return false;Claimed=true;Completed=false;expires=god.State.time+35;return true;}
 public bool Begin(){if(!Claimed||!pending||item.held||god.Season.paused)return false;float unit=god.GetComponent<IslandNavigator>().world.Scale;if(Vector3.Distance(god.transform.position,transform.position)>unit*10)return false;performer=god.GetComponentInChildren<HumanoidMotion>();Active=true;Played=0;PlayedNotes=new int[4];PlayedTimes=new float[4];playClock=0;nextBeat=.5f;return true;}
 public int Tick(float dt){if(!Active||!Claimed||!pending||item.held)return -1;if(god.Season.paused||dt<=0)return 0;playClock+=Mathf.Min(dt,.1f);
  if(Played<4&&playClock>=nextBeat){Play(notes[Played],performer);PlayedNotes[Played]=notes[Played];PlayedTimes[Played]=playClock;Paint(pips[Played],Color.white);if(Played<3)nextBeat+=intervals[Played];Played++;}
  if(Played==4&&playClock>=nextBeat+.4f){Completed=true;god.Season.WorldPlay.Echo.Remember(notes,intervals);god.Journal.Add(god.State.time,"rhythm echoed",CreatureAction.PlayWithToy,GetComponent<HybridTarget>().id,"Four witnessed notes echoed with the demonstrated timing");Cancel(true);return 1;}return 0;
 }
 void Play(int note,HumanoidMotion actor){Paint(bars[note],Color.white);flashes[note]=.2f;actor?.LookAt(transform);actor?.Activity(3);actor?.Touch(bars[note].transform);if(IslandAudio.Current!=null)IslandAudio.Cue(IslandSound.Chime,bars[note].transform.position,GetEntityId().GetHashCode(),1,note==0?1:1.25f);else{var sound=GetComponent<AudioSource>();if(sound!=null){sound.pitch=note==0?1:1.25f;sound.Play();}}}
 void ClearBeads(){foreach(var pip in pips)Paint(pip,new Color(.23f,.21f,.16f));}
 public void Cancel(bool keepCompletion=false){Claimed=Active=pending=false;count=0;if(!keepCompletion)Completed=false;performer?.Touch(null);performer?.Activity(0);performer=null;ClearBeads();}
 public void ResetWorld(){Cancel();lastTap=-100;Played=0;PlayedNotes=Array.Empty<int>();PlayedTimes=Array.Empty<float>();for(int i=0;i<2;i++){flashes[i]=0;Paint(bars[i],Colors[i]);}}
 void OnDisable(){Cancel();}
}
}
