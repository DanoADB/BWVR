using System;
using System.Linq;
using UnityEngine;
namespace BWVR {
public enum GatewayPhase {NotStarted,Active,Complete,Ended}
[Serializable] public sealed class GatewayProject {
 public GatewayPhase phase;public int attempts,completed,relayCursor,placementCursor,visitor;public float supplied,began,finished;public long playerPieces,creaturePieces;
 const long Pieces=(1L<<ConstructionSite.YardPieces)-1;
 public bool Shared=>playerPieces!=0&&creaturePieces!=0;
 public bool Begin(int relayCount,int nextPlacement,float now){if(phase==GatewayPhase.Active||attempts>=1000000||relayCount<0||nextPlacement<1||!HybridState.Number(now,0,100000000))return false;phase=GatewayPhase.Active;attempts++;relayCursor=relayCount;placementCursor=nextPlacement-1;supplied=0;playerPieces=creaturePieces=0;began=now;finished=0;visitor=0;return true;}
 public bool End(){if(phase!=GatewayPhase.Active)return false;phase=GatewayPhase.Ended;return true;}
 public bool Supply(TimberRelayMemory relay,bool paused){if(paused||phase!=GatewayPhase.Active||relay==null||!relay.Valid()||relay.count<=relayCursor||relay.time<began)return false;relayCursor=relay.count;supplied=Mathf.Min(3,supplied+relay.amount);return true;}
 public bool Placement(int slot,int piece,string actor,int receipt,float now,bool paused){if(paused||phase!=GatewayPhase.Active||slot!=0||piece<0||piece>=ConstructionSite.YardPieces||receipt<=placementCursor||now<began||!PlaceContributionLog.ActorValid(actor))return false;placementCursor=receipt;long bit=1L<<piece;playerPieces&=~bit;creaturePieces&=~bit;if(actor=="player")playerPieces|=bit;else if(actor=="creature")creaturePieces|=bit;return true;}
 public void Retain(long attached){if(phase!=GatewayPhase.Active)return;playerPieces&=attached;creaturePieces&=attached;}
 public bool Use(int person,float now,bool complete,bool paused){if(paused||phase!=GatewayPhase.Active||!complete||!Shared||supplied<3||person<0||person>=VillagePopulation.Adults||now<began)return false;phase=GatewayPhase.Complete;completed=Math.Min(1000000,completed+1);visitor=person;finished=now;return true;}
 public bool Valid()=>Enum.IsDefined(typeof(GatewayPhase),phase)&&attempts>=0&&attempts<=1000000&&completed>=0&&completed<=attempts&&relayCursor>=0&&relayCursor<=1000000&&placementCursor>=0&&placementCursor<1000000000&&visitor>=0&&visitor<VillagePopulation.Adults&&HybridState.Number(supplied,0,3)&&HybridState.Number(began,0,100000000)&&HybridState.Number(finished,0,100000000)&&playerPieces>=0&&playerPieces<=Pieces&&creaturePieces>=0&&creaturePieces<=Pieces&&(playerPieces&creaturePieces)==0&&(phase==GatewayPhase.NotStarted?attempts==0&&completed==0&&supplied==0&&playerPieces==0&&creaturePieces==0&&began==0&&finished==0:attempts>0)&&(phase!=GatewayPhase.Complete||Shared&&supplied==3&&completed>0&&finished>=began)&&(phase==GatewayPhase.Complete||finished==0);
}
public static class GatewayProjectText {
 public static CreationBookPage Read(SliceState s,int page){var p=s.gatewayProject;bool active=p?.phase==GatewayPhase.Active;page=Mathf.Clamp(page,0,1);
  if(page==1)return new CreationBookPage("Building together","Start with an unfinished main yard.\nBring three wood through a timber relay.\n\nSnap an arch block while the creature watches.\nIt may add another piece to a known form.\nAim at a pillar's inner rim to span the beam.\n\nLeave space for a villager to walk through.\nTap your block to invite again (desktop F).\nHold to pick up (desktop E).",1,2);
  string Done(bool yes)=>yes?"Done":"Open";string status=p?.phase==GatewayPhase.Complete?"A shared gateway, used by the village.":active?"Your gateway project is underway.":p?.phase==GatewayPhase.Ended?"Project ended. The world stays as you left it.":"Build something together, then see it used.";
  return new CreationBookPage("Village gateway",status+"\n\n"+Done(p?.supplied>=3)+" · Timber stored ("+(p?.supplied??0).ToString("0.#",System.Globalization.CultureInfo.InvariantCulture)+"/3)\n"+Done(p?.playerPieces>0)+" · You placed part of the arch\n"+Done(p?.creaturePieces>0)+" · Creature placed another part\n"+Done(p?.phase==GatewayPhase.Complete)+" · Villager walked through\n\nBegin to record new work. Nothing is reset.\nCompleted projects: "+(p?.completed??0),0,2);
 }
}
public sealed partial class ConstructionSite {
 public void RefreshGatewayEvidence(SliceState s){if(s.paused||s.gatewayProject?.phase!=GatewayPhase.Active||contributionSlot!=0)return;long attached=0;if(Recipe==BuildRecipe.Arch)for(int i=0;i<blocks.Count;i++)if(Pattern.Any(p=>Matches(blocks[i],p)))attached|=1L<<i;s.gatewayProject.Retain(attached);}
 void RecordGatewayPlacement(SliceState s,ConstructionBlock block,string actor,int receipt){if(Recipe!=BuildRecipe.Arch||!Pattern.Any(p=>Matches(block,p)))return;s.gatewayProject?.Placement(contributionSlot,blocks.IndexOf(block),actor,receipt,s.Hybrid.time,s.paused);}
}
public sealed partial class VillagerExploration {
 bool GatewayVisitWanted(ConstructionSite site){var p=construction.GetComponent<CreatureHybrid>().Season.gatewayProject;return p?.phase==GatewayPhase.Active&&p.supplied>=3&&p.Shared&&SiteSlot(site)==0&&site.Recipe==BuildRecipe.Arch&&social.Memory.injury==0&&!social.RecentFright;}
}
public sealed partial class QuestSlice {
 float gatewayCheck;GatewayProject gatewayBound;
 public bool BeginGatewayProject(){if(state.paused||applicationSuspended||held.Any(i=>i!=null)||state.gatewayProject?.phase==GatewayPhase.Active)return false;var site=learning.sites[0];if(site.Paused||site.Locked||site.IsBridge||site.RecipeComplete||site.Repairer!=null||site.Requester!=null||site.Visitor!=null||curiosity.UsingConstruction(site)||site.blocks.Any(b=>b.Item.held||b.VillagerOwner!=null)||site.Snapped>0&&site.Recipe!=BuildRecipe.Arch){notice="Use an unfinished arch or an empty main yard.";return false;}if(site.Recipe!=BuildRecipe.Arch&&!site.SelectRecipe(BuildRecipe.Arch))return false;var p=state.gatewayProject??(state.gatewayProject=new GatewayProject());if(!p.Begin(state.timberRelay?.count??0,state.placeContributions?.nextId??1,state.Hybrid.time))return false;gatewayBound=p;gatewayCheck=0;notice="Gateway project begun. New work will be recorded in Projects.";return true;}
 public bool EndGatewayProject(){if(state.paused||applicationSuspended||state.gatewayProject?.End()!=true)return false;ClearOutingMark();notice="Project ended. Blocks and memories remain.";return true;}
 public bool MarkGatewayProject(){if(state.paused||applicationSuspended||held.Any(i=>i!=null)||state.gatewayProject?.phase!=GatewayPhase.Active)return false;return ShowActivityBeacon(learning.sites[0].transform);}
 void AdvanceGatewayProject(float dt,bool paused){if(paused){gatewayCues?.Hide();return;}if(gatewayBound!=state.gatewayProject){gatewayBound=state.gatewayProject;gatewayCheck=0;}gatewayCheck-=dt;if(gatewayCheck>0)return;gatewayCheck=.25f;var p=state.gatewayProject;var site=learning.sites[0];if(p?.phase==GatewayPhase.Active)site.RefreshGatewayEvidence(state);if(p?.phase==GatewayPhase.Active||p?.phase==GatewayPhase.Complete&&state.Hybrid.time<=p.finished+8){if(gatewayCues==null)gatewayCues=new GatewayCues(transform);gatewayCues.Refresh(state,site,curiosity.UsingConstruction(site));}else gatewayCues?.Hide();}
 public bool GatewayUsed(VillagerExploration person,ConstructionSite site){if(state.paused||applicationSuspended||site!=learning.sites[0]||site.Recipe!=BuildRecipe.Arch||!site.RecipeComplete||person.Current!=VillagerExploration.Phase.UseStructure||site.Visitor!=person)return false;site.RefreshGatewayEvidence(state);if(state.gatewayProject?.Use(Array.IndexOf(jobs,person.GetComponent<VillagerRoutine>()),state.Hybrid.time,true,false)!=true)return false;notice="Village gateway complete. Your shared work has been used.";IslandAudio.Cue(IslandSound.Discovery,site.transform.position,64912,.5f);return true;}
}
}
