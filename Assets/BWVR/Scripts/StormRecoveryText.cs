using System;
namespace BWVR {
public static class StormRecoveryText {
 public static CreationBookPage Read(SliceState state,int page,int count,string[] names){
  var chapter=state.stormChapter;var r=chapter?.recovery;
  if(r?.recorded!=true)return new CreationBookPage("After the storm","No recovery encounter recorded.\n\nA villager who reaches safety can return to ordinary play once the storm passes.\nCreature-assisted survivors may approach and thank their rescuer first.",page,count);
  string name=names!=null&&chapter.villager>=0&&chapter.villager<names.Length?names[chapter.villager]:"The survivor";
  string thanks=chapter.source!=RescueSource.Creature?"No creature rescue to thank":r.thanked?"Thanked the creature in person":r.closed?"No thank-you visit completed":"Creature acknowledgment pending";
  string play=r.invited?"Offered a ball-game invitation":r.closed?"Recovery encounter ended":"May invite the creature to play";
  return new CreationBookPage("After the storm",name+" remembers reaching safety.\nRescue: "+chapter.source+"\n\n"+thanks+"\n"+play+"\n\nAn invitation does not guarantee participation.\nStorm damage and existing injuries remain; gratitude does not repair or heal them.",page,count);
 }
}
}
