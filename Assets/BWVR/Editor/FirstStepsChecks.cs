using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
namespace BWVR.Editor {
public static class FirstStepsChecks {
 static readonly BindingFlags Flags=BindingFlags.Instance|BindingFlags.NonPublic;
 static void Require(bool ok,string why){if(!ok)throw new Exception(why);}
 public static void Verify(){
  Require(Application.isPlaying,"Enter Play mode");var slice=UnityEngine.Object.FindAnyObjectByType<QuestSlice>();
  object Read(string name)=>typeof(QuestSlice).GetField(name,Flags).GetValue(slice);
  object Call(string name,params object[] args)=>typeof(QuestSlice).GetMethod(name,Flags).Invoke(slice,args);
  var original=(SliceState)Read("state");var test=JsonUtility.FromJson<SliceState>(JsonUtility.ToJson(original));var stateField=typeof(QuestSlice).GetField("state",Flags);var held=(CarryItem[])Read("held");Require(held.All(i=>i==null)&&!slice.MenuOpen,"Start with empty hands and closed menu");var panel=(GameObject)Read("panel");var home=(GameObject)Read("sanctuaryHome");var guide=(GameObject)Read("firstSteps");
  SpatialTarget Find(GameObject root,string name)=>root.GetComponentsInChildren<SpatialTarget>(true).Single(t=>t.name==name);
  void Click(SpatialTarget target){Physics.SyncTransforms();object[] args={new Ray(target.transform.position-panel.transform.forward*.15f,panel.transform.forward),.3f,default(RaycastHit)};Require((bool)Call("SanctuaryRaycast",args)&&((RaycastHit)args[2]).collider==target.GetComponent<Collider>(),"Unreachable menu target: "+target.name);Call("SanctuaryPointer",1,true,false,target.GetComponent<Collider>(),true);}
  void Layout(){Canvas.ForceUpdateCanvases();foreach(var text in guide.GetComponentsInChildren<TMPro.TMP_Text>()){text.ForceMeshUpdate();Require(!text.isTextOverflowing&&text.textBounds.size.x<=text.rectTransform.rect.width+1&&text.textBounds.size.y<=text.rectTransform.rect.height+1,"Guide text overflows: "+text.text);}foreach(var button in guide.GetComponentsInChildren<SpatialTarget>()){var size=button.GetComponent<RectTransform>().sizeDelta;var hit=button.GetComponent<BoxCollider>();Require(size==new Vector2(240,60)&&hit.size.x==240&&hit.size.y==60&&hit.isTrigger,"Guide button/collider sizing mismatch");}}
  try{
   stateField.SetValue(slice,test);string before=JsonUtility.ToJson(test);Require(!guide.activeInHierarchy,"Guide opens automatically");slice.SetSanctuaryOpen(true);var entry=Find(home,"Start here");Call("SanctuaryPointer",0,true,false,entry.GetComponent<Collider>(),true);Call("SanctuaryPointer",1,true,false,entry.GetComponent<Collider>(),false);Require(!guide.activeInHierarchy,"Guide ignored input tracking / side gates");Click(entry);Require(guide.activeInHierarchy&&!home.activeSelf,"Guide did not open exclusively");Layout();Require(!((GameObject)Read("firstStepsPrevious")).activeSelf,"Previous should hide at first page");
   var next=Find(guide,"Next");Click(next);Require((int)Read("firstStepsPage")==1,"Next failed");Call("SanctuaryPointer",1,true,true,next.GetComponent<Collider>(),true);Require((int)Read("firstStepsPage")==1,"Held trigger repeated next");Layout();
   var action=Find(guide,"Try this");test.paused=true;Click(action);Require(slice.MenuOpen&&Read("outingTarget")==null,"Paused guide placed a beacon");test.paused=false;typeof(QuestSlice).GetField("applicationSuspended",Flags).SetValue(slice,true);try{Click(action);Require(slice.MenuOpen&&Read("outingTarget")==null,"Suspended guide placed a beacon");}finally{typeof(QuestSlice).GetField("applicationSuspended",Flags).SetValue(slice,false);}
   var creature=slice.GetComponentInChildren<CreatureHybrid>();held[1]=creature.GetComponent<CarryItem>();Click(action);Require(slice.MenuOpen&&Read("outingTarget")==null,"Held object lost menu ownership");held[1]=null;Click(action);Require(!slice.MenuOpen&&Read("outingTarget")!=null&&(float)Read("outingMarkTime")==45,"Chime was not marked");Require(JsonUtility.ToJson(test)==before,"Reading/marking invented progress or changed the season");Call("ClearOutingMark");
   slice.SetSanctuaryOpen(true);Click(entry);Click(next);Click(next);Layout();Require(!((GameObject)Read("firstStepsNext")).activeSelf,"Next should hide at last page");Click(action);Require(slice.CreationBookOpen&&(CreationBookSection)Read("bookSection")==CreationBookSection.Outings&&(int)Read("bookPage")==0,"Guide did not route to Play Day overview");Require(JsonUtility.ToJson(test)==before,"Opening Play Day started or reset an outing");
   slice.SetSanctuaryOpen(true);Click(entry);Click(Find(guide,"Back"));Require(home.activeInHierarchy&&!guide.activeSelf,"Guide Back failed");Click(entry);Click(action);Require(!slice.MenuOpen,"Explore freely did not close menu");
   Debug.Log("BWVR_FIRST_STEPS_PASS: hidden startup, tracked ray/edge navigation, exclusive pages, caption/target bounds, pause/suspend/held gates, chime beacon, Play Day routing and unchanged season progress");
  }finally{held[1]=null;Call("ClearOutingMark");slice.SetSanctuaryOpen(false);stateField.SetValue(slice,original);}
 }
}
}
