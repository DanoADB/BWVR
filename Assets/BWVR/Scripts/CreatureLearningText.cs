using System;
using System.Linq;
using System.Collections.Generic;
namespace BWVR {
public static partial class CreationBookText {
 static CreationBookPage CreaturePractice(SliceState s){var build=s.building?.practice;var ball=s.worldPlay?.gentleBall;string building=build==null?"Building practice: not recorded":"Building control "+P(build.control)+" · Placements "+build.placements+"\nPlacement motion "+N(build.PlacementSeconds)+" seconds";string playing=ball==null?"Gentle ball practice: not recorded":"Gentle ball control "+P(ball.control)+"\nPractices "+ball.practices+" · Preference "+P(ball.preference);
  return new CreationBookPage("Knowledge, skill and choice",building+"\n\n"+playing+"\n\nKnown forms are knowledge.\nPractice improves physical execution.\nPraise and discouragement change willingness.\n\nSkill does not guarantee obedience.",1,4);
 }
 static CreationBookPage CreatureAttempts(SliceState s,IReadOnlyDictionary<string,string> targets){var h=s.hybrid;var records=h?.attempts?.OrderByDescending(a=>a.updatedAt).Take(2).ToArray();string text=records==null||records.Length==0?"No plan outcomes recorded yet.":string.Join("\n\n",records.Select(a=>ActionName(a.action)+" · "+Target(a.target,targets)+"\nFinished "+a.completions+" · Failed "+a.failures+(a.streak>0?"\n"+Short(a.reason,36)+" · Retry delay "+N(Math.Max(0,a.retryAt-(h?.time??0)))+"s":"")));
  return new CreationBookPage("Learning from attempts",text+"\n\nRoute failures pause retries. Move the target to reachable ground for a fresh attempt.\nPlayer interruptions are not failures.",2,4);
 }
 static CreationBookPage CreatureToolHandoff(SliceState s,string[] names){var d=s.tools?.delivery;if(d==null||d.status==ToolDeliveryStatus.Dormant)return new CreationBookPage("Workshop handoffs","No tool handoff recorded.\n\nKnown village designs can offer a workshop axe or hoe for a forester or farmer.\n\nBring the tool to its named villager, or offer Help to the creature at the tool.\nMaterials are spent only on successful delivery.",3,4);string tool=d.tool==VillageTool.BoundAxe?"Bound axe":"Cultivating hoe";string outcome=d.status==ToolDeliveryStatus.Delivered?"Delivered by "+d.source:d.status==ToolDeliveryStatus.Offered?"Waiting for a handoff":"Offer ended: "+d.status;return new CreationBookPage("Workshop handoffs",tool+" for "+Person(d.recipient,names)+"\n"+outcome+"\n\nCost: 2 wood + "+ToolTechnologyState.ResinCost(d.tool)+" resin\nPaid only on successful arrival.\n\nCarry the workshop tool to this villager, or offer Help at the tool and let the creature choose.\n\nKnowledge and equipment follow the real handoff.",3,4);}
}
}
