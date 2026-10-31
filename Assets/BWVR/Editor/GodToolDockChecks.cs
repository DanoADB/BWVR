using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
namespace BWVR.Editor {
public static class GodToolDockChecks {
 static readonly BindingFlags F=BindingFlags.Instance|BindingFlags.NonPublic;
 static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
 public static void Verify(Action<string> capture=null){
  var slice=UnityEngine.Object.FindAnyObjectByType<QuestSlice>();var p=slice.GetComponentInChildren<GodToolPalette>();var eye=Camera.main;var god=slice.GetComponentInChildren<CreatureHybrid>();var field=typeof(QuestSlice).GetField("state",F);var original=(SliceState)field.GetValue(slice);var test=new SliceState{developmentFocus=true,observeWithoutFoodWork=true};var oldCursor=p.UseDesktopCursor;var oldRotation=eye.transform.rotation;var choices=(Collider[])typeof(GodToolPalette).GetField("choices",F).GetValue(p);bool bookSeen=false,menuSeen=false;
  var book=(Action)typeof(GodToolPalette).GetField("showBook",F).GetValue(p);var menu=(Action)typeof(GodToolPalette).GetField("showMenu",F).GetValue(p);field.SetValue(slice,test);god.Interrupt("God tool dock QA");p.UseDesktopCursor=false;p.Cancel();slice.SetSanctuaryOpen(false);var outside=new Ray(eye.transform.position,eye.transform.forward);
  void Frame(Ray ray,bool pressed,float dt=.02f){p.Process(ray,pressed,true,false,dt,false);}
  Ray Point(Collider c){Physics.SyncTransforms();return new Ray(eye.transform.position,(c.bounds.center-eye.transform.position).normalized);}
  void Click(Collider c){Frame(Point(c),false);Frame(Point(c),true);}
  try{
   Frame(outside,false);Check(p.View.activeInHierarchy&&!p.Open&&!p.ClaimsInput,"Idle dock blocks play or is invisible");for(int i=0;i<80;i++)Frame(outside,false,.1f);Check(p.DockOpacity<=.3f,"Idle dock did not fade");capture?.Invoke("idle");
   Check(p.CategoryButtons.Concat(p.UtilityButtons).All(c=>c.isTrigger&&c.gameObject.layer==2),"Dock leaked into ordinary physics/raycast layers");
   var groups=Enum.GetValues(typeof(GodToolCategory)).Cast<GodToolCategory>().ToArray();int[] counts={3,4,2,2,3};
   foreach(var group in groups){Click(p.CategoryButtons[(int)group]);Check(p.Open&&p.ActiveCategory==group,"Category pointer failed: "+group);var visible=Enumerable.Range(0,GodToolPalette.ToolCount).Where(i=>choices[i].gameObject.activeInHierarchy).ToArray();Check(visible.Length==counts[(int)group],"Incorrect category tool count: "+group);foreach(int i in visible){var img=choices[i].GetComponentInChildren<UnityEngine.UI.RawImage>();Check(img!=null&&img.texture!=null,"Tool glyph missing");}Frame(Point(p.CategoryButtons[(int)group]),false);capture?.Invoke(group.ToString());}
   p.OpenCategory(GodToolCategory.Weather);Frame(outside,false);int uses=p.Uses;float energy=test.Hybrid.energy;Click(choices[3]);Check(p.Selected==SpiritualTool.Rain&&!p.Open&&p.Uses==uses&&test.Hybrid.energy==energy,"Selection cast a tool");Frame(outside,true);Check(p.Uses==uses&&!p.Brush.Active,"Selection press leaked into rain");Frame(outside,false);
   Click(p.UtilityButtons[0]);Check(!p.Selected.HasValue&&!p.Open,"Empty hand retained tool");Frame(outside,false);Check(!p.ClaimsInput,"Empty hand retained ownership");
   p.OpenCategory(GodToolCategory.Terraform);Frame(outside,false);Click(choices[11]);Check(p.Selected==SpiritualTool.RaiseGround&&!p.Open,"Terraform selection failed");Frame(outside,false);capture?.Invoke("equipped");
   // Aiming back into UI cannot finish a charged world action.
   Frame(Point(p.CategoryButtons[1]),false);Check(p.DockHovered&&!p.Sculptor.Pending&&!p.Brush.Active,"Dock hover retained a world action");
   p.Cancel();Frame(outside,false);p.Toggle();Frame(outside,false);var fixedAt=p.View.transform.position;eye.transform.Rotate(0,20,0);Frame(outside,false);Check(Vector3.Distance(fixedAt,p.View.transform.position)<.000001f,"Expanded dock chased pointer/head");eye.transform.rotation=oldRotation;p.Cancel();Frame(outside,false);
   // Real menu routes, not test-only stand-ins.
   p.BindDock(()=>{bookSeen=true;book();},()=>{menuSeen=true;menu();});Click(p.UtilityButtons[1]);Check(bookSeen&&slice.CreationBookOpen&&!p.View.activeSelf&&!p.Selected.HasValue,"Book shortcut failed");slice.SetSanctuaryOpen(false);Frame(outside,false);Click(p.UtilityButtons[2]);Check(menuSeen&&slice.MenuOpen&&!p.View.activeSelf,"Menu shortcut failed");slice.SetSanctuaryOpen(false);Frame(outside,false);
   p.Select(SpiritualTool.WindFan);p.Process(outside,true,true,true,.02f,true);Check(!p.View.activeSelf&&!p.Selected.HasValue&&!p.Brush.Active,"Busy/zoom state retained dock/tool");p.Process(outside,true,true,false,.02f,true);Check(!p.Open&&!p.Selected.HasValue,"Unblocked held press replayed selection");p.Process(outside,false,true,false,.02f,true);p.Process(outside,true,false,false,.02f,true);Check(!p.View.activeSelf,"Tracking loss retained dock");
   Check(test.Valid()&&test.terrainEdits==null&&p.Uses==uses,"Dock changed simulation state");Debug.Log("BWVR_GOD_TOOL_DOCK_PASS: five groups/all 14 tools; sourced glyphs; fading; pointer expansion/equip; release guard; empty hand; stable expanded pose; real Book/menu; blocked/tracking gates; no world mutation");
  }finally{eye.transform.rotation=oldRotation;slice.SetSanctuaryOpen(false);p.BindDock(book,menu);p.UseDesktopCursor=oldCursor;p.Cancel();p.Process(outside,false,true,false,.02f,false);field.SetValue(slice,original);god.NewSeason();p.RestoreSign();}
 }
}
}
