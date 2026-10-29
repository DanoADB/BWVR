using System;
namespace BWVR {
[Serializable]public sealed class TreasureTrailState {
 public int found,shared,food;
 public bool Found(int site)=>site>=0&&site<6&&(found&(1<<site))!=0;
 public bool Shared(int site)=>site>=0&&site<6&&(shared&(1<<site))!=0;
 public bool Find(int site){if(site<0||site>=6||Found(site))return false;found|=1<<site;return true;}
 public bool Share(int site,bool edible){if(!Found(site)||Shared(site))return false;shared|=1<<site;if(edible)food|=1<<site;return true;}
 public static int Count(int mask){int n=0;for(int i=0;i<6;i++)if((mask&(1<<i))!=0)n++;return n;}
 public bool Valid()=>found>=0&&found<=63&&shared>=0&&shared<=63&&food>=0&&food<=63&&(shared&~found)==0&&(food&~shared)==0;
}
public static class TreasureTrailText {
 public static CreationBookPage Read(SliceState s,int page){page=Math.Max(0,Math.Min(6,page));var m=s.treasureTrail;
  if(page==0)return new CreationBookPage("Island Finds","Your own trail of discoveries.\n\nClearings found: "+TreasureTrailState.Count(m?.found??0)+" / 6\nFinds shared: "+TreasureTrailState.Count(m?.shared??0)+" / 6\n\nExplore, pick up a find, and bring it back.\nPlace it near the creature and let it choose.\nA finished interaction earns a field note.\n\nNo deadline. Mark a clearing for a hint.",page,7);
  int site=page-1;string toy=site%2==0?"play hammer":"seed shaker";bool done=m?.Shared(site)==true;
  string text=done?"FIELD NOTE · A discovery shared\n\nYou brought a "+(((m.food>>site)&1)!=0?"strawberry":toy)+" from this clearing.\nThe creature finished "+(((m.food>>site)&1)!=0?"eating it.":"exploring it.")+"\n\nA gift offers a possibility, not an order.\nFamiliar objects can become favorite toys.\n\nYou can still return and explore together.":"Look for a ring of five low stones.\nHere: a "+toy+" and a strawberry.\n\nFound by you: "+(m?.Found(site)==true?"yes":"not yet")+"\nAccepted from you: not yet\n\nBring a find back and place it nearby.\nLet the creature finish playing or eating.\nBusy? Offer the find again a little later.\nUse Mark clearing for a location hint.";
  return new CreationBookPage("Clearing "+(site+1),text,page,7);
 }
}
}
