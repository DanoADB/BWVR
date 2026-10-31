using UnityEngine;
namespace BWVR {
public sealed partial class CreatureHybrid {
 ConstructionBlock buildInvitation;SliceState buildInvitationSeason;float buildInvitationUntil,buildInvitationDuration=45;bool gatewayInvitation;
 // Receipt-driven attention, not an instruction to move or reserve the next piece.
 public void NoticePlayerBuild(ConstructionBlock block){
  if(block==null||!block.Attached||block.Item.held||block.Site.Paused||block.Site.Locked||block.Site.IsBridge||Season.paused||suspended||GetComponent<CarryItem>()?.held==true)return;
  var target=block.GetComponent<HybridTarget>();if(target==null||Find(target.id)!=target||!CanSeeCue(target))return;
  GetComponentInParent<QuestSlice>().OfferBowlCreatureTurn(block);pointedBuild=false;buildPointEvidence=0;buildInvitation=block;buildInvitationSeason=Season;gatewayInvitation=GatewayNeedsPartner(block.Site);buildInvitationDuration=gatewayInvitation?75:45;buildInvitationUntil=State.time+buildInvitationDuration;
  placedAttention=target;attentionUntil=State.time+12;nextPlacementNotice=State.time+.5f;NoticeCue(target,"You started building here",EvidenceSource.PlayerPlacement);
  Journal.Add(State.time,"shared build invitation",CreatureAction.ConstructionPlay,target.id,"A witnessed player snap invites construction in this yard");nextPerception=nextDecision=0;
 }
 float SharedBuildingBias(HybridTarget target){
  if(buildInvitationSeason!=Season||buildInvitation==null||!buildInvitation.Attached||buildInvitation.Item.held||State.time>=buildInvitationUntil||Season.paused||suspended)return 0;
  var site=buildInvitation.Site;var block=target!=null?target.GetComponent<ConstructionBlock>():null;
  if(site==null||!site.gameObject.activeInHierarchy||site.Paused||site.Locked||site.RecipeComplete||block==null||block.Site!=site||!block.CanBuild||block.Item.held)return 0;
  if(gatewayInvitation&&!GatewayNeedsPartner(site))return 0;return (gatewayInvitation?.95f:.85f)*Mathf.Clamp01((buildInvitationUntil-State.time)/buildInvitationDuration);
 }
 void ClearSharedBuilding(){buildInvitation=null;buildInvitationSeason=null;buildInvitationUntil=0;pointedBuild=false;buildPointEvidence=0;gatewayInvitation=false;buildInvitationDuration=45;}
 public void SharedBuildDetached(ConstructionBlock block){if(block==buildInvitation)ClearSharedBuilding();}
}
}
