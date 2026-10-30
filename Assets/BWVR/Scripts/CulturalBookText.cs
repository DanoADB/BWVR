using System;
using System.Linq;
namespace BWVR {
public static class CulturalBookText {
 static string Name(int index,string[] names)=>names!=null&&index>=0&&index<names.Length?names[index]:"Villager "+(index+1);
 public static CreationBookPage Read(SliceState state,int page,string[] names){
  var believers=(state.relationships??Array.Empty<SocialMemory>()).Where(m=>m?.saint!=null&&m.saint.chosen>=0).ToArray();var groups=believers.GroupBy(m=>m.saint.chosen).OrderByDescending(g=>g.Count(m=>m.saint.confidence>=.5f)).Take(2).ToArray();
  string stories=groups.Length==0?"No chosen-survivor stories recorded.":string.Join("\n\n",groups.Select(g=>Name(g.Key,names)+"\n"+g.Count(m=>m.saint.confidence>=.5f)+" believers · "+g.Count()+" heard the story\n"+g.Sum(m=>m.saint.visits)+" completed visits"));
  return new CreationBookPage("Chosen survivors",stories+"\n\nThese are villagers' interpretations of witnessed survival, not proof of supernatural powers.\nStories can spread through actual visits.");
 }
}
}
