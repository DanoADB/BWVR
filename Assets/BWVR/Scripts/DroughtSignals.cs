using UnityEngine;
namespace BWVR {
public sealed class DroughtSignals:MonoBehaviour {
 WaterVessel cistern;Material ground;VillagerRoutine[] villagers;CreatureRoutine creature;float requestClock=2;
 public int requests{get;private set;}public int celebrations{get;private set;}
 public void Initialize(WaterVessel reservoir,Material surface,VillagerRoutine[] workers,CreatureRoutine god){cistern=reservoir;ground=surface;villagers=workers;creature=god;}
 public void Tick(float dt,SliceState state){
  cistern.SetFill(state.water/100);if(ground.HasProperty("_Drought"))ground.SetFloat("_Drought",1-state.water/100);if(ground.HasProperty("_CropHealth"))ground.SetFloat("_CropHealth",state.crops/100);
  if(state.developmentFocus||state.paused||state.Finished||dt<=0)return;requestClock-=dt;
  if(requestClock<=0&&state.water<65){foreach(var villager in villagers)if(villager.role==VillagerRoutine.Role.WaterGatherer&&villager.RequestWater(creature)){requests++;requestClock=30;return;}requestClock=2;}
 }
 public void ResetRequests(){requestClock=2;foreach(var villager in villagers)villager.ResetRequestCycle();}
 public void WaterArrived(){celebrations++;foreach(var villager in villagers)villager.Celebrate(creature.transform);}
}
}
