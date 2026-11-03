using System;
using System.Linq;
using UnityEngine;
namespace BWVR {
// A single confirmed pose, never reconstructed from historical placement totals.
[Serializable] public sealed class BlockReturnTicket {
 public int receipt,site,block;public Vector3 position;public Quaternion rotation=Quaternion.identity;public bool consumed;
 public bool Valid()=>receipt==0?site==0&&block==0&&position==Vector3.zero&&rotation==Quaternion.identity&&!consumed:receipt>0&&receipt<1000000000&&site>=0&&site<3&&block>=0&&block<ConstructionSite.YardPieces&&HybridState.Number(position.x,-1000,1000)&&HybridState.Number(position.y,-1000,1000)&&HybridState.Number(position.z,-1000,1000)&&HybridState.Number(rotation.x,-1,1)&&HybridState.Number(rotation.y,-1,1)&&HybridState.Number(rotation.z,-1,1)&&HybridState.Number(rotation.w,-1,1)&&Mathf.Abs(Quaternion.Dot(rotation,rotation)-1)<.01f;
}
public sealed partial class QuestSlice {
 bool TryBlockReturn(out ConstructionBlock block,out string reason){
  block=null;var ticket=state.blockReturn;reason="Place a block first.";
  if(ticket==null||ticket.receipt==0||!ticket.Valid()||ticket.consumed)return false;
  reason="Resume and release held objects.";if(state.paused||applicationSuspended||held.Any(h=>h!=null))return false;
  var site=learning?.sites.FirstOrDefault(s=>s!=null&&s.ContributionSlot==ticket.site);
  reason="The yard is busy. Try again later.";
  if(site==null||site.IsBridge||site.Paused||site.Locked||site.BuildBowlReserved||site.blocks.Any(b=>b.Item.held||b.VillagerOwner!=null)||site.Visitor!=null||site.Requester!=null||site.Repairer!=null||curiosity.UsingConstruction(site)||state.gatewayProject?.phase==GatewayPhase.Active||state.buildBowl?.phase==GatewayPhase.Active)return false;
  reason="That placement has changed.";
  if(ticket.block>=site.blocks.Count)return false;var candidate=site.blocks[ticket.block];
  var latest=state.placeContributions?.records?.LastOrDefault(r=>r.site==ticket.site&&r.block==ticket.block);
  if(latest==null||latest.id!=ticket.receipt||latest.actor!="player"||!candidate.Attached||candidate.creatureBuilt||candidate.Item.held||candidate.VillagerOwner!=null||Vector3.Distance(candidate.Body.position,ticket.position)>.05f||Quaternion.Angle(candidate.Body.rotation,ticket.rotation)>2)return false;
  reason="Other pieces depend on this block.";
  if(site.blocks.Any(b=>b!=candidate&&b.Connection!=null&&b.Connection.connectedBody==candidate.Body))return false;
  block=candidate;reason="Return your latest safe placement to supply.";return true;
 }
 public bool ReturnLastBlock(){
  if(!TryBlockReturn(out var block,out var reason)){notice=reason;return false;}
  block.Site.ReturnLoose(block);state.blockReturn.consumed=true;notice="Block returned to the supply area.";return true;
 }
 bool BookBlockReturnPage=>bookSection==CreationBookSection.Places&&CreationBookText.Read(state,bookSection,bookPage,bookPeople,bookTargets).title=="Build with me";
}
}
