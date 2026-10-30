using UnityEngine;
namespace BWVR {
// Choreography only: the existing social and curiosity controllers retain ownership.
public sealed partial class SharedRhythm:MonoBehaviour {
 public bool Active{get;private set;}public bool Completed{get;private set;}public int Beats{get;private set;}
 public const int BeatCount=8;public const float Interval=.7f;
 VillagerSocial host;CreatureHybrid creature;HumanoidMotion guestMotion,hostMotion;AudioSource sound;Renderer visual;
 MaterialPropertyBlock original,pulse;float clock,nextBeat,flash,offerClock;int offeredNotes;
 void Awake(){sound=GetComponent<AudioSource>();visual=GetComponent<Renderer>();original=new MaterialPropertyBlock();pulse=new MaterialPropertyBlock();visual?.GetPropertyBlock(original);}
 public void ResetOffer(){Cancel();Completed=false;Beats=offeredNotes=0;offerClock=0;}
 public bool ApproachPoint(CreatureHybrid god,out Vector3 point){
  point=transform.position;var person=GetComponent<CreatureAffordance>().host;if(person==null)return false;var nav=god.GetComponent<IslandNavigator>();
  var away=transform.position-person.transform.position;away.y=0;if(away.sqrMagnitude<1e-10f)away=-person.transform.forward;
  return nav.world.Sample(transform.position+away.normalized*nav.world.Scale*5,nav.profile,2,out point)&&god.CanReach(point);
 }
 public void Offer(VillagerSocial person,CreatureHybrid god,float dt){
  if(dt<=0||Active||person==null||GetComponent<CarryItem>().held)return;creature=god;offerClock+=dt;Fade(dt);
  if(offeredNotes<2&&offerClock>=.3f+offeredNotes*Interval){host=person;hostMotion=person.GetComponentInChildren<HumanoidMotion>();Tap(hostMotion,offeredNotes++);}
 }
 public bool Begin(CreatureHybrid god){
  var person=GetComponent<CreatureAffordance>().host;
  if(GetComponent<PlayerRhythm>()?.Reserved==true||Ritualist!=null||Active||person==null||person.Current!=VillagerSocial.Phase.WatchGift||!person.Willing||GetComponent<CarryItem>().held||god.Season.paused)return false;
  float unit=god.GetComponent<IslandNavigator>().world.Scale;
  if(Vector3.Distance(person.transform.position,transform.position)>unit*5||Vector3.Distance(god.transform.position,transform.position)>unit*9)return false;
  host=person;creature=god;hostMotion=person.GetComponentInChildren<HumanoidMotion>();guestMotion=god.GetComponentInChildren<HumanoidMotion>();Active=true;Completed=false;Beats=0;clock=0;nextBeat=.3f;InviteAudience();return true;
 }
 // -1 interrupted, 0 in progress, 1 a fully completed reciprocal game.
 public int Tick(float dt){
  if(!Active)return Completed?1:-1;if(creature.Season.paused||dt<=0)return 0;
  float unit=creature.GetComponent<IslandNavigator>().world.Scale;
  if(host==null||host.Offering!=GetComponent<CreatureAffordance>()||host.Current!=VillagerSocial.Phase.WatchGift||!host.Willing||host.GetComponent<CarryItem>().held||GetComponent<CarryItem>().held||Vector3.Distance(host.transform.position,transform.position)>unit*6||Vector3.Distance(creature.transform.position,transform.position)>unit*10){Cancel();return -1;}
  clock+=dt;Fade(dt);hostMotion?.LookAt(creature.transform);guestMotion?.LookAt(host.transform);
  if(Beats<BeatCount&&clock>=nextBeat){bool villagerTurn=Beats%4<2;Tap(villagerTurn?hostMotion:guestMotion,Beats%2);Beats++;nextBeat=clock+Interval;}
  if(Beats>=BeatCount&&flash<=0){Completed=true;Active=false;FinishAudience(true);ClearPose();return 1;}return 0;
 }
 void Tap(HumanoidMotion performer,int note){
  hostMotion?.Touch(null);guestMotion?.Touch(null);hostMotion?.Activity(0);guestMotion?.Activity(0);
  performer?.LookAt(transform);performer?.Signal(false);performer?.Activity(performer==guestMotion?3:5);performer?.Touch(transform.Find(note==0?"Low chime bar":"High chime bar")??transform);
  if(IslandAudio.Current!=null)IslandAudio.Cue(IslandSound.Chime,transform.position,GetEntityId().GetHashCode(),1,note==0?1:1.25f);
  else if(sound!=null&&sound.clip!=null){sound.pitch=note==0?1:1.25f;sound.Play();}
  if(visual!=null){visual.GetPropertyBlock(pulse);pulse.SetColor("_Color",Color.Lerp(visual.sharedMaterial.color,Color.white,.4f));visual.SetPropertyBlock(pulse);}flash=.24f;
 }
 void Fade(float dt){if(flash<=0)return;flash=Mathf.Max(0,flash-dt);if(flash<=0){visual?.SetPropertyBlock(original);if(!Active){hostMotion?.Touch(null);hostMotion?.Activity(0);if(creature!=null)hostMotion?.LookAt(creature.transform);hostMotion?.Signal(true,true,transform.position);}}}
 void ClearPose(){hostMotion?.Touch(null);guestMotion?.Touch(null);hostMotion?.Activity(0);guestMotion?.Activity(0);visual?.SetPropertyBlock(original);if(sound!=null){sound.Stop();sound.pitch=1;}flash=0;}
 public void Cancel(){ReleaseRitual(Ritualist);Active=false;FinishAudience(false);ClearPose();host=null;creature=null;hostMotion=guestMotion=null;}
 void OnDisable(){Cancel();}
}
}
