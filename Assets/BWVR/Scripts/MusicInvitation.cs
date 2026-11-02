using System.Linq;
using UnityEngine;
namespace BWVR {
public sealed partial class PlayerRhythm {
 SliceState invitationSeason;float nextVillageInvitation;
 // A spatial invitation reserves only a willing host, never the creature's action.
 public bool RequestVillageMusic(){
  if(god==null||god.Season.paused||allowed?.Invoke()!=true||Reserved||item.held||!affordance.Available||affordance.host!=null||shared.Active||shared.Ritualist!=null||god.GetComponent<CarryItem>().held)return false;
  if(invitationSeason!=god.Season){invitationSeason=god.Season;nextVillageInvitation=0;}if(god.State.time<nextVillageInvitation)return false;
  foreach(var neighbor in echoNeighbors.Where(p=>p!=null).OrderBy(p=>(p.transform.position-transform.position).sqrMagnitude)){
   var person=neighbor.GetComponent<VillagerSocial>();if(!person.TryPlayerMusic(this))continue;
   nextVillageInvitation=god.State.time+8;god.Journal.Add(god.State.time,"music invitation noticed",CreatureAction.IdleOrObserve,person.GetComponent<HybridTarget>().id,"A nearby willing host approaches the placed chime; no performance yet");return true;
  }return false;
 }
}
public sealed partial class VillagerSocial {
 bool stationaryMusic;
 internal bool TryPlayerMusic(PlayerRhythm instrument){
  if(instrument==null||!routine.AvailableForBell||!Willing||Memory.musicInterest+Memory.boldness*.2f<.4f||!GetComponent<VillagerExploration>().CanSeeEcho(instrument))return false;
  return TryShareCore(instrument.GetComponent<CreatureAffordance>(),true);
 }
}
public sealed partial class QuestSlice {
 PlayerRhythm pendingMusicTap;float musicTapSeconds;SliceState musicTapSeason;
 // The bars keep note input. Tap the body to invite; hold to use native pickup.
 bool MusicInvitationPointer(int side,bool pressed,bool wasPressed,bool tracked,bool sharedCarry,Collider hit,float dt,bool blocked){
  if(side!=1)return false;if(blocked||!tracked||held.Any(i=>i!=null)){pendingMusicTap=null;musicTapSeconds=0;return false;}
  var instrument=hit!=null&&hit.GetComponent<ChimeKey>()==null?hit.GetComponentInParent<PlayerRhythm>():null;
  if(!sharedCarry){pendingMusicTap=null;musicTapSeconds=0;if(pressed&&!wasPressed&&instrument!=null){instrument.RequestVillageMusic();return true;}return false;}
  if(pendingMusicTap!=null){
   var target=pendingMusicTap;if(musicTapSeason!=state||instrument!=target){pendingMusicTap=null;musicTapSeconds=0;return true;}
   if(!pressed){pendingMusicTap=null;musicTapSeconds=0;if(wasPressed)target.RequestVillageMusic();return true;}
   musicTapSeconds+=Mathf.Clamp(dt,0,.1f);if(musicTapSeconds>=BuildGrabDelay){pendingMusicTap=null;musicTapSeconds=0;TryGrab(side,target.GetComponent<CarryItem>());return false;}return true;
  }
  if(pressed&&!wasPressed&&instrument!=null){pendingMusicTap=instrument;musicTapSeason=state;musicTapSeconds=0;return true;}return false;
 }
}
}
