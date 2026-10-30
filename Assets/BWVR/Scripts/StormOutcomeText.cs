using System;
namespace BWVR {
public static class StormOutcomeText {
 static string Name(int index,string[] names)=>names!=null&&index>=0&&index<names.Length?names[index]:"A villager";
 static string Outcome(StormFireState fire,string[] names){switch(fire.outcome){case StormFireOutcome.PlayerRain:return "Extinguished by your rain";case StormFireOutcome.VillagerWater:return "Extinguished by "+Name(fire.firefighter,names);case StormFireOutcome.Weather:return "Extinguished by weather";case StormFireOutcome.FuelSpent:return "Burned until fuel ran out";case StormFireOutcome.TargetLost:return "Tree moved; outcome unconfirmed";case StormFireOutcome.NoIgnition:return "Lightning did not ignite the tree";case StormFireOutcome.Stopped:return "Chapter stopped; outcome unconfirmed";case StormFireOutcome.Unresolved:return "Fire unresolved when chapter ended";default:return fire.ignited?"Tree still burning":"No fire outcome yet";}}
 public static CreationBookPage Read(SliceState state,int page,int count,string[] names){var chapter=state.stormChapter;var fire=chapter?.fire;if(chapter==null||chapter.phase==StormPhase.Dormant)return new CreationBookPage("Weathering the storm","No storm chapter recorded.\n\nStart Gathering Storm from Sanctuary Settings.\nDuring the warning, lightning may ignite an isolated tree. Use local rain, or let a villager fetch water.",page,count);
  string rescue=chapter.resolved?chapter.source+" assistance":chapter.opportunity?"Safety not yet reached":"No safety opportunity recorded";string incident=fire?.attempted==true?Outcome(fire,names)+"\nWater carried "+fire.waterDrawn.ToString("0.#")+" · Rain doses "+fire.playerPulses+"\nTree damage "+Math.Round(fire.damageBefore*100)+"% → "+Math.Round(fire.damageAfter*100)+"%":"No lightning incident recorded";
  return new CreationBookPage("Weathering the storm",chapter.phase+" · Episode "+chapter.episode+"\n"+rescue+"\n\n"+incident+"\n\nDamage remains after the weather clears.\nLocal rain can suppress burning; healing treats an injured villager separately.",page,count);
 }
}
}
