using System;
using System.Linq;
namespace BWVR {
public static class CulturalBookText {
 static string Name(int index,string[] names)=>names!=null&&index>=0&&index<names.Length?names[index]:"Villager "+(index+1);
 public static CreationBookPage Read(SliceState state,int page,string[] names){
  page=Math.Max(0,Math.Min(page,2));if(page==2){var game=state.worldPlay?.invented;return new CreationBookPage("A game discovered",game==null?"No safe knockdown experiments recorded.\n\nThe creature can discover knockdown play after three safe throws topple snapped toy blocks. Player throws do not teach this discovery.":"Knockdown play: "+(game.Discovered?"discovered":"experimenting")+"\nSafe experiments "+game.safeKnockdowns+"\nInvitations "+game.invitations+" · Successful replays "+game.replays+"\nPreference "+Math.Round(game.preference*100)+"%\n\nThe creature may beckon, then roll at a toy build. Restack blocks to offer another round.\nPraise completed play to encourage this habit.",page,3);}
  if(page==1){var beliefs=(state.relationships??Array.Empty<SocialMemory>()).Select((m,i)=>new{m,i}).Where(p=>p.m?.rainBelief!=null).OrderByDescending(p=>p.m.rainBelief.Conviction).Take(2).ToArray();string details=beliefs.Length==0?"No music/rain associations recorded.":string.Join("\n\n",beliefs.Select(p=>Name(p.i,names)+": "+Math.Round(p.m.rainBelief.Conviction*100)+"% conviction\n"+p.m.rainBelief.matches+" matches · "+p.m.rainBelief.misses+" misses\n"+p.m.rainBelief.rituals+" completed rituals"));return new CreationBookPage("Songs for rain",details+"\n\nVillagers may repeat music during shortages.\nThe ritual cannot cause rain. Failed predictions weaken their belief.",page,3);}
  var believers=(state.relationships??Array.Empty<SocialMemory>()).Where(m=>m?.saint!=null&&m.saint.chosen>=0).ToArray();var groups=believers.GroupBy(m=>m.saint.chosen).OrderByDescending(g=>g.Count(m=>m.saint.confidence>=.5f)).Take(2).ToArray();
  string stories=groups.Length==0?"No chosen-survivor stories recorded.":string.Join("\n\n",groups.Select(g=>Name(g.Key,names)+"\n"+g.Count(m=>m.saint.confidence>=.5f)+" believers · "+g.Count()+" heard the story\n"+g.Sum(m=>m.saint.visits)+" completed visits"));
  return new CreationBookPage("Chosen survivors",stories+"\n\nWitnesses call these survivors chosen by the creature. Their stories spread through visits and shared encounters.",page,3);
 }
}
}
