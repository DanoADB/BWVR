using System;
using System.Reflection;
using UnityEngine;
namespace BWVR.Editor {
public static class GodToolTargetChecks {
 static readonly BindingFlags F=BindingFlags.Instance|BindingFlags.NonPublic;
 static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
 public static void Verify(Action<GodToolPalette> capture=null){
  var q=UnityEngine.Object.FindAnyObjectByType<QuestSlice>();Check(Application.isPlaying&&q!=null,"Enter Play mode");var p=q.GetComponentInChildren<GodToolPalette>();var god=q.GetComponentInChildren<CreatureHybrid>();var nav=q.GetComponent<IslandNavigation>();var res=q.GetComponentInChildren<VillageResources>();var stateField=typeof(QuestSlice).GetField("state",F);var original=stateField.GetValue(q);var test=new SliceState{developmentFocus=true,observeWithoutFoodWork=true};test.Hybrid.time=100;test.Resources.wheat=test.Resources.Limit(VillageResource.Wheat);bool cursor=p.UseDesktopCursor;float ts=Time.timeScale;Time.timeScale=0;
  var label=(UnityEngine.UI.Text)typeof(GodToolPalette).GetField("targetFeedbackText",F).GetValue(p);var root=(GameObject)typeof(GodToolPalette).GetField("targetFeedback",F).GetValue(p);var at=nav.Terrain.Project(res.StoreApproach);var ray=new Ray(at+Vector3.up,Vector3.down);
  void Frame(bool pressed,bool tracked=true,bool blocked=false)=>p.Process(ray,pressed,tracked,blocked,.02f,true);
  try{
   stateField.SetValue(q,test);god.Interrupt("Tool target QA");q.SetSanctuaryOpen(false);p.UseDesktopCursor=false;p.Select(SpiritualTool.FoodGift);string before=JsonUtility.ToJson(test);Frame(false);Check(root.activeInHierarchy&&!p.PreviewValid&&label.text==p.Reason&&label.text.Contains("full"),"Missing real rejection beside target");Check(JsonUtility.ToJson(test)==before,"Target label changed simulation state");Check(root.GetComponentsInChildren<Collider>(true).Length==0&&!label.raycastTarget,"Target feedback can intercept input");Canvas.ForceUpdateCanvases();Check(label.preferredHeight<=label.rectTransform.rect.height+1,"Reason label overflows");capture?.Invoke(p);
   var facing=root.transform.position-Camera.main.transform.position;Check(Vector3.Dot(root.transform.forward,facing.normalized)>.99f,"Label not facing headset");float size=root.transform.localScale.x;Frame(false);Check(Mathf.Approximately(size,root.transform.localScale.x),"Stable target changed angular size");Frame(false,false);Check(!root.activeSelf&&!p.Selected.HasValue,"Tracking loss retained rejection label");p.Select(SpiritualTool.FoodGift);Frame(false);Check(root.activeSelf,"Re-equipped label missing");Frame(false,true,true);Check(!root.activeSelf,"Menu/busy path retained label");p.Select(SpiritualTool.FoodGift);Frame(false);p.Cancel();Check(!root.activeSelf,"Empty hand retained label");p.OpenCategory(GodToolCategory.Offerings);Frame(false);Check(!root.activeSelf,"Category menu has world rejection label");
   Debug.Log("BWVR_TOOL_TARGET_FEEDBACK_PASS: real full-store ray rejection, pure view, collider-free layout, facing/angular size, tracking, blocked/menu and cancellation cleanup");
  }finally{p.Cancel();p.UseDesktopCursor=cursor;stateField.SetValue(q,original);god.NewSeason();Time.timeScale=ts;}
 }
}
}
