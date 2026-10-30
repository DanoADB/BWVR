using System.Linq;
using UnityEngine;
namespace BWVR {
public sealed partial class BeachBall {
 BuildBowlProject bowlBound;ConstructionSite bowlSite;long bowlPlayer,bowlCreature,bowlHitPlayer,bowlHitCreature;int bowlReceipt;float bowlAge;Vector3 bowlOrigin;
 public bool BuildBowlPending{get;private set;}
 bool BuildBowlSiteFree=>bowlSite!=null&&!bowlSite.Paused&&!bowlSite.Locked&&bowlSite.Repairer==null&&bowlSite.Requester==null&&bowlSite.Visitor==null&&!creature.GetComponent<CreatureCuriosity>().UsingConstruction(bowlSite)&&!bowlSite.blocks.Any(b=>b.Item.held||b.VillagerOwner!=null);
 public void CancelBuildBowlShot(){if(bowlSite!=null&&bowlSite.buildBowlBall==this)bowlSite.buildBowlBall=null;BuildBowlPending=false;bowlBound=null;bowlSite=null;bowlPlayer=bowlCreature=bowlHitPlayer=bowlHitCreature=0;bowlAge=0;}
 void BeginBuildBowlShot(){
  CancelBuildBowlShot();var p=creature.Season.buildBowl;if(p?.phase!=GatewayPhase.Active||creature.Season.paused||PeerGame||Player!=null||Puzzle?.Active==true||Bowling?.Active==true||Goals?.Active==true||Goals?.Turns.Active==true)return;bowlSite=creature.GetComponent<CreatureConstructionLearning>().sites[0];bowlSite.RefreshBuildBowlEvidence(creature.Season);if(!p.Shared||!BuildBowlSiteFree||bowlSite.BuildBowlReserved){bowlSite=null;return;}bowlOrigin=transform.position;bowlBound=p;bowlPlayer=p.playerPieces;bowlCreature=p.creaturePieces;bowlReceipt=creature.Season.placeContributions?.nextId-1??0;BuildBowlPending=true;bowlSite.buildBowlBall=this;CancelFetch();
 }
 void BuildBowlContact(ConstructionBlock block){
  if(!BuildBowlPending||bowlBound!=creature.Season.buildBowl||!BuildBowlSiteFree||block.Site!=bowlSite||creatureThrow||!thrown||Harmful)return;int index=bowlSite.blocks.IndexOf(block);var receipt=creature.Season.placeContributions?.records.LastOrDefault(r=>r.site==0&&r.block==index);if(receipt==null||receipt.id>bowlReceipt)return;long bit=1L<<index;if(receipt.actor=="player"&&(bowlPlayer&bit)!=0)bowlHitPlayer|=bit;if(receipt.actor=="creature"&&(bowlCreature&bit)!=0)bowlHitCreature|=bit;
 }
 void AdvanceBuildBowlShot(float dt){
  if(!BuildBowlPending)return;if(bowlBound!=creature.Season.buildBowl||bowlBound.phase!=GatewayPhase.Active||item.held||Harmful||creatureThrow||PeerGame||Player!=null||Puzzle?.Active==true||Goals?.Turns.Active==true||!BuildBowlSiteFree){CancelBuildBowlShot();return;}bowlAge+=dt;if(bowlAge<12)return;var p=bowlBound;var origin=bowlOrigin;bool player=bowlHitPlayer!=0,helper=bowlHitCreature!=0;CancelBuildBowlShot();if(!p.Confirm(player,helper,creature.State.time,!Harmful,creature.Season.paused))return;creature.Journal.Add(creature.State.time,"player build-and-bowl shot",CreatureAction.IdleOrObserve,StableId,"Safe physical knockdown: player piece="+player+", creature piece="+helper);if(player||helper)IslandAudio.Cue(IslandSound.Discovery,transform.position,64915,.5f);if(p.phase==GatewayPhase.Complete)creature.GetComponent<CreatureCuriosity>().NoticeResponse(CreatureAffect.Mood.Happy,2);OfferBuildBowlReturn(p,origin);
 }
}
}
