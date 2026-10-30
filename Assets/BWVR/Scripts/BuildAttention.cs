using System.Linq;
using UnityEngine;
namespace BWVR {
public sealed partial class CreatureHybrid {
 bool pointedBuild;int buildPointEvidence;float nextBuildPoint;SliceState pointSeason;
 public bool IsPlayerBuildAnchor(ConstructionBlock block){
  if(block==null||!block.gameObject.activeInHierarchy||!block.Attached||block.Item.held||block.creatureBuilt||block.Site.IsBridge)return false;
  var site=block.Site;var learning=GetComponent<CreatureConstructionLearning>();if(learning==null)return false;if(!learning.sites.Contains(site))return false;int slot=site.ContributionSlot;if(slot<0||slot>2)return false;var records=Season.placeContributions?.records;if(records==null)return false;int index=site.blocks.IndexOf(block);var latest=records.LastOrDefault(r=>r.site==slot&&r.block==index);return latest?.actor=="player";
 }
 public bool PointToPlayerBuild(ConstructionBlock block){
  if(Season.paused||suspended||GetComponent<CarryItem>()?.held==true||!IsPlayerBuildAnchor(block))return false;if(pointSeason!=Season){pointSeason=Season;nextBuildPoint=0;}if(State.time<nextBuildPoint)return false;
  var site=block.Site;var target=block.GetComponent<HybridTarget>();if(site.Paused||site.Locked||site.RecipeComplete||site.Repairer!=null||site.Visitor!=null||site.Requester!=null||site.blocks.Any(b=>b.Item.held||b.VillagerOwner!=null)||target==null||Find(target.id)!=target||!CanSeeCue(target)||!Season.Building.Knows(site.Recipe)||!site.blocks.Any(b=>b!=block&&b.CanBuild&&b.GetComponent<CreatureAffordance>().Available))return false;
  buildPointEvidence=State.Evidence.Add(State.time,EvidenceKind.GuidanceOffered,EvidenceSource.PlayerGuidance,CreatureAction.ConstructionPlay,target.id,"Look here: continue this shared build");if(buildPointEvidence==0)return false;
  pointedBuild=true;buildInvitation=block;buildInvitationSeason=Season;gatewayInvitation=GatewayNeedsPartner(site);buildInvitationDuration=gatewayInvitation?75:45;buildInvitationUntil=State.time+buildInvitationDuration;nextBuildPoint=State.time+8;
  placedAttention=target;attentionUntil=State.time+12;NoticeCue(target,"I noticed your shared-building invitation",EvidenceSource.PlayerGuidance,buildPointEvidence);Journal.Add(State.time,"pointed build invitation",CreatureAction.ConstructionPlay,target.id,"A deliberate gesture renews attention without a new placement");nextPerception=nextDecision=0;return true;
 }
 void ApplySharedBuildingCue(CreatureCandidate c,HybridTarget target){float bias=SharedBuildingBias(target);if(pointedBuild&&bias>0){c.guidance=Mathf.Max(c.guidance,bias);c.guidanceEvidenceId=buildPointEvidence;}else c.attention=Mathf.Max(c.attention,bias);}
}
public sealed partial class QuestSlice {
 ConstructionBlock pendingBuildTap;float buildTapSeconds;SliceState buildTapSeason;
 const float BuildGrabDelay=.32f;
 // On the right Quest trigger, only a current player-built anchor gets tap/hold arbitration.
 bool BuildAttentionPointer(int side,bool pressed,bool wasPressed,bool tracked,bool sharedCarry,Collider hit,float dt,bool blocked){
  if(side!=1)return false;if(blocked||!tracked||held.Any(i=>i!=null)){pendingBuildTap=null;buildTapSeconds=0;return false;}var block=hit!=null?hit.GetComponentInParent<ConstructionBlock>():null;
  if(!sharedCarry){pendingBuildTap=null;buildTapSeconds=0;if(pressed&&!wasPressed&&hybrid.IsPlayerBuildAnchor(block)){hybrid.PointToPlayerBuild(block);return true;}return false;}
  if(pendingBuildTap!=null){
   var anchor=pendingBuildTap;if(buildTapSeason!=state||block!=anchor||!hybrid.IsPlayerBuildAnchor(anchor)){pendingBuildTap=null;buildTapSeconds=0;return true;}
   if(!pressed){pendingBuildTap=null;buildTapSeconds=0;if(wasPressed)hybrid.PointToPlayerBuild(anchor);return true;}
   buildTapSeconds+=Mathf.Clamp(dt,0,.1f);if(buildTapSeconds>=BuildGrabDelay){pendingBuildTap=null;buildTapSeconds=0;TryGrab(side,anchor.Item);return false;}return true;
  }
  if(pressed&&!wasPressed&&hybrid.IsPlayerBuildAnchor(block)){pendingBuildTap=block;buildTapSeason=state;buildTapSeconds=0;return true;}return false;
 }
}
}
