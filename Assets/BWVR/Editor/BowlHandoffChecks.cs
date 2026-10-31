using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
namespace BWVR.Editor {
public static class BowlHandoffChecks {
 static readonly BindingFlags F=BindingFlags.Instance|BindingFlags.NonPublic;
 static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
 public static void Verify(Action<BeachBall,ConstructionSite,string> capture=null){int observed=0;BowlCreatureTurnChecks.Verify(false,false,(ball,site,stage)=>{
  var state=ball.GetComponentInParent<QuestSlice>().GetComponentInChildren<CreatureHybrid>().Season;var expected=stage=="offered"?BowlHandoffPhase.CreatureInvited:stage=="carrying"?BowlHandoffPhase.CreatureActing:stage=="settling"?BowlHandoffPhase.Settling:BowlHandoffPhase.YourShot;int lines=stage=="offered"?4:stage=="carrying"?3:stage=="settling"?2:1;
  string before=JsonUtility.ToJson(state);bool reserved=site.BuildBowlReserved;ball.RefreshBowlHandoff();Check(ball.HandoffPhase==expected,"Wrong handoff phase: "+stage+" / "+ball.HandoffPhase);var cues=(BowlHandoffCues)typeof(BeachBall).GetField("handoffCues",F).GetValue(ball);var root=(GameObject)typeof(BowlHandoffCues).GetField("root",F).GetValue(cues);Check(cues.VisibleLines==lines,"Unexpected guide budget: "+stage+" / "+cues.VisibleLines);Check(root.GetComponentsInChildren<LineRenderer>(true).Length==4&&root.GetComponentsInChildren<LineRenderer>(true).Sum(l=>l.positionCount)==83&&root.GetComponentsInChildren<Collider>(true).Length==0&&root.GetComponentsInChildren<CarryItem>(true).Length==0&&root.GetComponentsInChildren<HybridTarget>(true).Length==0,"Visual acquired gameplay affordances or grew beyond fixed budget");
  ball.RefreshBowlHandoff(true);Check(ball.HandoffPhase==BowlHandoffPhase.None&&cues.VisibleLines==0,"Paused handoff visible");ball.RefreshBowlHandoff();Check(ball.HandoffPhase==expected,"Pause consumed handoff");if(stage=="offered"){var item=ball.GetComponent<CarryItem>();item.held=true;ball.RefreshBowlHandoff();Check(ball.HandoffPhase==BowlHandoffPhase.None&&cues.VisibleLines==0,"Player-held ball retained turn cue");item.held=false;ball.RefreshBowlHandoff();}
  Check(before==JsonUtility.ToJson(state)&&reserved==site.BuildBowlReserved,"Rendering changed progress, receipt, energy, time or ownership");capture?.Invoke(ball,site,stage);observed++;
 });Check(observed==4,"Native turn stages missing");Debug.Log("BWVR_BOWL_HANDOFF_PASS: native offered/carrying/settling/player transitions, fixed four-line 83-vertex visuals, no colliders/items/AI targets, pause and player-carry exclusion, pure state/receipt/time/energy/ownership projection");}
}
}
