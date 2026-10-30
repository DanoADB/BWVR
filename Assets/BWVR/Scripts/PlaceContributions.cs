using System;
using UnityEngine;
namespace BWVR {
[Serializable] public sealed class PlacePlacementReceipt {
 public int id,site,block;public float time;public string actor="";public bool repair;
}
// Historical placement evidence, not current ownership, utility, teaching or a reward counter.
[Serializable] public sealed class PlaceContributionLog {
 public const int Capacity=48;public int nextId=1;public PlacePlacementReceipt[] records=Array.Empty<PlacePlacementReceipt>();
 public static bool ActorValid(string actor){if(actor=="player"||actor=="creature")return true;if(actor==null||!actor.StartsWith("villager-",StringComparison.Ordinal)||!int.TryParse(actor.Substring(9),out int index)||index<0||index>=VillagePopulation.Adults)return false;return actor=="villager-"+index;}
 public static bool FieldsValid(int site,int block,string actor,bool repair,float time)=>site>=0&&site<3&&block>=0&&block<ConstructionSite.YardPieces&&ActorValid(actor)&&(!repair||actor.StartsWith("villager-",StringComparison.Ordinal))&&HybridState.Number(time,0,100000000);
 public bool Add(int site,int block,string actor,bool repair,float time){
  if(!FieldsValid(site,block,actor,repair,time)||nextId>=1000000000)return false;
  if(records.Length>0){var last=records[records.Length-1];if(time<last.time||last.time==time&&last.site==site&&last.block==block&&last.actor==actor&&last.repair==repair)return false;}
  var record=new PlacePlacementReceipt{id=nextId++,site=site,block=block,actor=actor,repair=repair,time=time};int count=records.Length;
  if(count<Capacity){Array.Resize(ref records,count+1);records[count]=record;}else{Array.Copy(records,1,records,0,Capacity-1);records[Capacity-1]=record;}return true;
 }
 public bool Valid(float now){if(!HybridState.Number(now,0,100000000)||nextId<1||nextId>1000000000||records==null||records.Length>Capacity)return false;int id=0;float time=0;foreach(var r in records){if(r==null||r.id<=id||r.id>=nextId||!FieldsValid(r.site,r.block,r.actor,r.repair,r.time)||r.time<time||r.time>now)return false;id=r.id;time=r.time;}return true;}
}
public sealed partial class ConstructionSite {
 public event Action<ConstructionBlock> PlayerPlaced;
 Func<SliceState> contributionSeason;int contributionSlot=-1;internal int ContributionSlot=>contributionSlot;
 public void ConfigureContributions(int slot,Func<SliceState> season){contributionSlot=slot;contributionSeason=season;}
 void RecordConfirmedPlacement(ConstructionBlock block,string actor,bool repair){
  if(Paused||Locked||contributionSlot<0||contributionSeason==null)return;var state=contributionSeason();if(state==null||state.paused)return;int index=blocks.IndexOf(block);float now=state.hybrid?.time??0;
  if(!PlaceContributionLog.FieldsValid(contributionSlot,index,actor,repair,now))return;
  if(state.placeContributions==null)state.placeContributions=new PlaceContributionLog();if(state.placeContributions.Add(contributionSlot,index,actor,repair,now)){RecordGatewayPlacement(state,block,actor,state.placeContributions.nextId-1);state.buildBowl?.Placement(contributionSlot,index,actor,state.placeContributions.nextId-1,now,state.paused);if(actor=="player")PlayerPlaced?.Invoke(block);}
 }
}
}
