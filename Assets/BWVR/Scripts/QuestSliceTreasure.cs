using System.Linq;
using UnityEngine;
namespace BWVR {
public sealed partial class QuestSlice {
 SliceState treasureBound;readonly CarryItem[] treasureOffers=new CarryItem[12];readonly float[] treasureUntil=new float[12];
 void InitializeTreasureTrail(){curiosity.Completed+=TreasureCompleted;}
 void ClearTreasureOffer(){System.Array.Clear(treasureOffers,0,12);System.Array.Clear(treasureUntil,0,12);}
 static int TreasureSlot(DiscoveryObject find)=>find!=null&&find.point.id>=0&&find.point.id<6?find.point.id*2+(find.treat?1:0):-1;
 bool TreasurePicked(CarryItem item){
  if(treasureBound!=state)ClearTreasureOffer();treasureBound=state;for(int i=0;i<12;i++)if(treasureOffers[i]==item){treasureOffers[i]=null;treasureUntil[i]=0;}
  if(state.paused||applicationSuspended)return false;var find=item.GetComponent<DiscoveryObject>();if(find==null||!find.Available||Vector3.Distance(item.transform.position,item.home)>navigation.Scale*6)return false;
  var memory=state.treasureTrail??(state.treasureTrail=new TreasureTrailState());if(!memory.Find(find.point.id))return false;
  IslandAudio.Cue(IslandSound.Discovery,item.transform.position,item.GetEntityId().GetHashCode(),.55f);notice="A clearing found. Bring a discovery back to the creature.";return true;
 }
 void TreasurePlaced(CarryItem item){
  var find=item.GetComponent<DiscoveryObject>();if(state.paused||applicationSuspended||find==null||find.Owner!=null||!find.Available||state.treasureTrail?.Found(find.point.id)!=true||state.treasureTrail.Shared(find.point.id)||Vector3.Distance(item.transform.position,creature.position)>navigation.Scale*14)return;
  int slot=TreasureSlot(find);if(slot<0)return;treasureBound=state;treasureOffers[slot]=item;treasureUntil[slot]=state.Hybrid.time+60;notice="A find offered. Give the creature time to choose.";
 }
 void TreasureCompleted(CreatureAffordance action){
  if(state.paused||applicationSuspended||treasureBound!=state||action==null)return;var item=action.Item;var find=item!=null?item.GetComponent<DiscoveryObject>():null;int slot=TreasureSlot(find);if(slot<0||treasureOffers[slot]!=item||state.Hybrid.time>treasureUntil[slot]||held.Any(i=>i==item)||find.Owner!=null)return;
  if(state.treasureTrail?.Share(find.point.id,find.treat)==true){curiosity.NoticeResponse(CreatureAffect.Mood.Happy,2);IslandAudio.Cue(IslandSound.Content,creature.position,61139,.7f);hybrid.Journal.Add(state.Hybrid.time,"player-shared island find",find.treat?CreatureAction.EatFood:CreatureAction.PlayWithToy,action.GetComponent<HybridTarget>()?.id??"discovery","Clearing "+(find.point.id+1));notice="Discovery shared. A new field note is in the Book.";}int first=find.point.id*2;treasureOffers[first]=treasureOffers[first+1]=null;treasureUntil[first]=treasureUntil[first+1]=0;
 }
 void AdvanceTreasureTrail(bool paused){if(treasureBound!=state){ClearTreasureOffer();treasureBound=state;}if(paused)return;for(int i=0;i<12;i++){var item=treasureOffers[i];if(item==null||state.Hybrid.time>treasureUntil[i]||item.GetComponent<DiscoveryObject>()?.Owner!=null){treasureOffers[i]=null;treasureUntil[i]=0;}}}
 public bool MarkFindPage(int page){if(state.paused||applicationSuspended||held.Any(i=>i!=null))return false;var point=page==0?discoveries.points.OrderBy(p=>state.treasureTrail?.Found(p.id)==true?1:0).ThenBy(p=>p.id).FirstOrDefault(p=>state.treasureTrail?.Shared(p.id)!=true):discoveries.points.FirstOrDefault(p=>p.id==page-1);if(point==null||!point.gameObject.activeInHierarchy){notice="No unshared clearing is available. Choose a clearing page to revisit.";return false;}return ShowActivityBeacon(point.transform);}
 void DisposeTreasureTrail(){if(curiosity!=null)curiosity.Completed-=TreasureCompleted;ClearTreasureOffer();}
}
}
