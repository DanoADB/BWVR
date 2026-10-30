using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
namespace BWVR {
public enum CreationBookSection {Creature,People,Provisions,Places,Outings,Finds}
public readonly struct CreationBookPage {
 public readonly string title,text;public readonly int index,count;
 public CreationBookPage(string title,string text,int index=0,int count=1){this.title=title;this.text=text;this.index=index;this.count=count;}
}
// Reads raw saved fields only: no lazy initialization, receipts, commands or simulation ownership.
public static class CreationBookText {
 static string N(float value)=>value.ToString("0.#",CultureInfo.InvariantCulture);
 static string P(float value)=>(value*100).ToString("0",CultureInfo.InvariantCulture)+"%";
 static string Short(string value,int length=40){value=(value??"Unknown").Replace("\n"," ").Replace("\r"," ");return value.Length<=length?value:value.Substring(0,length-1)+"…";}
 static string Person(int index,string[] names)=>names!=null&&index>=0&&index<names.Length?Short(names[index],26):"Villager "+(index+1);
 public static string ActionName(CreatureAction action)=>new[]{"Water delivery","Eating","Investigating","Play","Social contact","Building","Guidance response","Observing"}[(int)action];
 static string Target(string id,IReadOnlyDictionary<string,string> names){if(names!=null&&id!=null&&names.TryGetValue(id,out var name))return Short(name,28);return id=="object-12"?"Beachball":id=="object-13"?"Chime":id=="wood-gift"?"Wood gift":id=="food-gift"?"Food gift":id=="cistern"?"Village water":id=="ground-rain-brush"?"Local rain":id=="ground-wind-fan"?"Wind fan":"Unidentified object";}
 public static CreationBookPage Read(SliceState s,CreationBookSection section,int page=0,string[] names=null,IReadOnlyDictionary<string,string> targets=null){
  if(s==null)return new CreationBookPage("No record","This season has no recorded state.");
  switch(section){case CreationBookSection.Finds:return TreasureTrailText.Read(s,page);case CreationBookSection.Outings:return PlayOutingText.Read(s,page);case CreationBookSection.Creature:return Creature(s,targets);case CreationBookSection.People:return People(s,page,names);case CreationBookSection.Provisions:return Provisions(s);default:return Places(s,page,targets,names);}
 }
 static CreationBookPage Creature(SliceState s,IReadOnlyDictionary<string,string> targets){
  var h=s.hybrid;string activity=h?.intention==null?"Unknown":h.intention.active?ActionName(h.intention.action):"No active plan";
  var favorite=h?.interests?.Where(i=>i!=null&&i.experiences>0).OrderByDescending(i=>i.appeal).FirstOrDefault();
  string needs=h?.motivation==null?"Curiosity / boredom: not recorded":"Curiosity "+P(h.motivation.curiosity)+" · Boredom "+P(h.motivation.boredom);
  var learned=h?.associations?.Where(a=>a!=null&&a.samples>0).OrderByDescending(a=>a.samples).Take(2).ToArray();
  string lessons=learned==null||learned.Length==0?"No reinforced lessons recorded.":string.Join("\n",learned.Select(a=>ActionName(a.action)+": preference "+P(a.preference)+" ("+a.samples+")"));
  string forms=s.building==null?"Unknown":string.Join(", ",Enum.GetValues(typeof(BuildRecipe)).Cast<BuildRecipe>().Where(r=>s.building.Knows(r)).Select(r=>r.ToString()));
  return new CreationBookPage("Their god","Plan: "+activity+"\nHunger "+P(s.hunger)+"\n"+needs+"\nFavorite: "+(favorite==null?"not recorded":Target(favorite.target,targets))+"\nKnown forms: "+(forms.Length==0?"none recorded":forms)+"\nPhysical placements: "+(s.building?.practice==null?"unknown":s.building.practice.placements.ToString())+"\n\nLearned preferences (samples)\n"+lessons);
 }
 static CreationBookPage People(SliceState s,int page,string[] names){
  int adults=s.relationships?.Length??s.families?.adults?.Length??0,children=s.families?.children?.Length??0,count=adults+children;if(count==0)return new CreationBookPage("People","Individual records are not available yet.");page=Math.Max(0,Math.Min(page,count-1));
  if(page>=adults){var c=s.families.children[page-adults];var t=c?.traits;return new CreationBookPage(Short(c?.id??"Child"),"Parents: "+Person(c.parentA,names)+" / "+Person(c.parentB,names)+(t==null?"\nInherited traits: unknown":"\nBoldness "+P(t.boldness)+"\nSociability "+P(t.sociability)+" · Diligence "+P(t.diligence))+"\n\nIndividual activity memories: unknown",page,count);}
  var m=s.relationships!=null&&page<s.relationships.Length?s.relationships[page]:null;if(m==null)return new CreationBookPage(Person(page,names),"Individual memories are not recorded.",page,count);
  var tool=s.tools?.learners!=null&&page<s.tools.learners.Length?s.tools.learners[page]:null;
  string skills=tool==null?"Tool practice: unknown":"Wood work "+tool.woodPractice+" · Farming "+tool.farmPractice+"\nAxe: "+(tool.axeOwned?"equipped":tool.axeKnown?"learned":"not learned")+" · Hoe: "+(tool.hoeOwned?"equipped":tool.hoeKnown?"learned":"not learned");
  return new CreationBookPage(Person(page,names),"Trust "+P(m.trust)+" · Affection "+P(m.affinity)+"\nFaith "+P(m.faith)+" · Rest need "+P(m.restNeed)+"\nInjury "+P(m.injury)+"\n\nBall games "+(m.ballGames+m.peerGames)+" · Music visits "+m.musicGatherings+"\nShared rests "+m.sharedRests+" · Shelter rests "+m.shelterRests+"\nConstruction uses "+m.constructionUses+" · Thanks "+m.buildThanks+"\n\n"+skills,page,count);
 }
 static CreationBookPage Provisions(SliceState s){
  var r=s.resources;if(r==null)return new CreationBookPage("Provisions","Resource stocks are not recorded yet.");
  int fields=r.sources?.Count(x=>x!=null&&x.kind==VillageResource.Wheat&&x.growing)??0,trees=r.sources?.Count(x=>x!=null&&x.kind==VillageResource.Wood&&x.growing)??0;
  return new CreationBookPage("Village stores","Food stored "+N(r.wheat+r.freshFood)+" / "+N(r.Limit(VillageResource.Wheat))+"\nWood "+N(r.wood)+" / "+N(r.Limit(VillageResource.Wood))+"\nResin "+N(r.resin)+" / "+N(r.Limit(VillageResource.Resin))+"\n\nExposed food "+N(r.exposedWheat)+" · Spoiled "+N(r.spoiled)+"\nDeliveries "+r.deliveries+" · Store upgrades "+r.storageUpgrades+"\nGrowing fields "+fields+" · Trees regrowing "+trees+"\n\nPlayer gifts recorded\nFood "+N(r.giftedFood)+" · Wood "+N(r.giftedWood));
 }
 static CreationBookPage Places(SliceState s,int page,IReadOnlyDictionary<string,string> targets,string[] names){
  var memories=(s.relationships??Array.Empty<SocialMemory>()).Where(m=>m!=null).SelectMany(m=>m.gatheringPlaces??Array.Empty<GatheringPlaceMemory>()).Where(p=>p!=null).ToArray();var receipts=s.placeContributions?.records??Array.Empty<PlacePlacementReceipt>();var slots=memories.Select(p=>p.slot).Concat(receipts.Select(r=>r.site)).Distinct().OrderBy(i=>i).ToArray();int places=Math.Max(1,slots.Length),count=places*2+3;page=Math.Max(0,Math.Min(page,count-1));
  if(page==count-1)return new CreationBookPage("Knockdown play","Stack blocks, then roll or throw the beachball at them.\n\nGentle taps move loose pieces. Hard hits can break snapped connections.\n\nRebuild and try another shape. Held and carried blocks are protected.\n\nVillagers notice if a build they are using comes apart.",page,count);
  if(page==count-2)return new CreationBookPage("Build with me","Snap a block into place while the creature watches.\n\nFor 45 seconds, it may choose another loose block in this yard and continue a known form.\n\nBuild alongside it. Praise a helpful placement with A. Take your block back to withdraw the invitation.\n\nThis is an invitation, not a guaranteed command.",page,count);
  if(page==places*2){var ledger=s.hybrid?.evidence;var entries=ledger?.events?.Where(e=>e!=null&&(e.kind==EvidenceKind.ActionCompleted||e.kind==EvidenceKind.Miracle||e.kind==EvidenceKind.Provision)).Reverse().Take(2).ToArray();
   string records=entries==null||entries.Length==0?"No saved outcome evidence yet.":string.Join("\n\n",entries.Select(e=>(e.actor=="creature"?"Creature":e.actor=="player"?"Player":"System")+" · "+(e.kind==EvidenceKind.Miracle?"Miracle":e.kind==EvidenceKind.Provision?"Gift":ActionName(e.action))+" · "+N(e.time)+"s\n"+Source(e.source)+": "+Target(e.target,targets)+"\n"+Short(e.outcome,42)));
   return new CreationBookPage("Latest island evidence",records+"\n\nEarlier history / place attribution: unknown.\nOnly the latest 96 entries are retained.",page,count);
  }
  if(page%2==1)return Contributors(s,slots.Length==0?0:slots[page/2],page,count,names);
  if(slots.Length==0)return new CreationBookPage("Gathering places","No person-visits recorded yet.\n\nPlace contributors: unknown.\nUse Next for contributor records.",page,count);
  int slot=slots[page/2];var recordsForPlace=memories.Where(p=>p.slot==slot).ToArray();int visits=recordsForPlace.Sum(p=>p.visits),bad=recordsForPlace.Sum(p=>p.uncomfortable);
  return new CreationBookPage(slot==0?"Shared gathering yard":"Former clearing "+slot,"Recorded person-visits "+visits+"\nComfortable "+(visits-bad)+" · Exposed "+bad+"\nMean remembered appeal "+(recordsForPlace.Length==0?"unknown":P(recordsForPlace.Average(p=>p.appeal)))+"\n\nRequests "+recordsForPlace.Sum(p=>p.requests)+" · Accepted "+recordsForPlace.Sum(p=>p.answers)+"\nUsed after request: cover "+recordsForPlace.Sum(p=>p.coverUses)+" · ball "+recordsForPlace.Sum(p=>p.ballUses)+"\n\nUse Next for placement contributors.\nVisit totals count each participant.",page,count);
 }
 static CreationBookPage Contributors(SliceState s,int slot,int page,int count,string[] names){
  var records=(s.placeContributions?.records??Array.Empty<PlacePlacementReceipt>()).Where(r=>r.site==slot).ToArray();
  string details=records.Length==0?"No placement contributors recorded yet.":string.Join("\n",records.GroupBy(r=>r.actor).OrderByDescending(g=>g.Max(r=>r.id)).Take(3).Select(g=>ContributorName(g.Key,names)+": "+g.Count()+" snaps · "+g.Count(r=>r.repair)+" repairs"));
  return new CreationBookPage(slot==0?"Who placed blocks here":"Clearing "+slot+" contributors","Recent contributors (up to 3)\n"+details+"\n\nPlacement does not prove useful construction.\nEarlier contributors: unknown.\nLast 48 recorded placements retained.",page,count);
 }
 static string ContributorName(string id,string[] names)=>id=="player"?"Player":id=="creature"?"Creature":int.TryParse(id.Substring(9),out int index)?Person(index,names):"Unknown";
 static string Source(EvidenceSource s)=>s==EvidenceSource.PlayerGuidance?"Player-guided":s==EvidenceSource.PlayerPlacement?"Player placement":s==EvidenceSource.PlayerMiracle?"Direct intervention":"Autonomous";
}
}
