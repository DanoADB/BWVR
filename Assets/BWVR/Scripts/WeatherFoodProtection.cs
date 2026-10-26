namespace BWVR {
public sealed partial class VillagerResourceGathering {
 // Visible rain, not advance knowledge of future random weather. Never preempt a task.
 public bool TryWeatherPreparation(){if(world.Season.paused||Active||Busy||world.FoodEmergency||world.Season.Weather.rain<.15f||world.State.exposedWheat<=.01f)return false;return TryBuildStorage();}
}
}
