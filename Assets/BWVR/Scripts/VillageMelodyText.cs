using System.Linq;
namespace BWVR {
public static partial class CreationBookText {
 static string PersonMelody(SocialMemory m)=>m.hasMelody?"Phrase practiced: "+VillageMelodies.Name(m.melodyMotif):m.hasBorrowedMelody?"Phrase learned: "+VillageMelodies.Name(m.borrowedMelody):"Phrase: not yet learned";
 static CreationBookPage CreatureMelodies(SliceState s){var memory=s.worldPlay?.villageMelodies;string lines=string.Join("\n",Enumerable.Range(0,4).Select(i=>VillageMelodies.Name(i)+"\n"+(memory?.Knows(i)==true?"Echoes "+memory.echoes[i]+" · Preference "+P(memory.preference[i]):"Not yet echoed")));return new CreationBookPage("Village phrases",lines+"\n\nVillagers learn through complete gatherings.\nPraise an echo to shape future choices.",4,5);}
}
}
