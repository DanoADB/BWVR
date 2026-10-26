using System.Globalization;
namespace BWVR {
// Detached, read-only telemetry. Never use SliceState's lazy properties here.
public sealed class SpiritObservation {
 public readonly bool paused,resourcesKnown;
 public readonly int adults,children,learnedAssociations;
 public readonly float wood,wheat,resin,exposedFood;
 public readonly CreatureAffect.Mood mood;
 public readonly string weather;
 SpiritObservation(SliceState state,CreatureAffect.Mood feeling){
  paused=state.paused;adults=VillagePopulation.Adults;children=state.families?.children?.Length??0;mood=feeling;
  resourcesKnown=state.resources!=null;
  if(resourcesKnown){wood=state.resources.wood;wheat=state.resources.wheat+state.resources.freshFood;resin=state.resources.resin;exposedFood=state.resources.exposedWheat;}
  weather=state.weather==null?"Unknown":state.weather.kind.ToString();
  if(state.hybrid?.associations!=null)foreach(var association in state.hybrid.associations)if(association!=null&&association.samples>0)learnedAssociations++;
 }
 public static SpiritObservation Capture(SliceState state,CreatureAffect.Mood mood)=>state==null?null:new SpiritObservation(state,mood);
 public string Summary(){
  string Number(float value)=>value.ToString("0.#",CultureInfo.InvariantCulture);
  return "SPIRIT · AI DISCONNECTED\n"+(paused?"Paused · ":"")+"Creature: "+mood+" · Learned contexts: "+learnedAssociations+
   "\nResidents: "+adults+" adults, "+children+" children · "+weather+
   (resourcesKnown?"\nWood "+Number(wood)+" · Food "+Number(wheat)+" · Resin "+Number(resin)+"\nExposed food "+Number(exposedFood):"\nResources: not observed")+
   "\nLocal observation only · no voice or world control";
 }
}
}
