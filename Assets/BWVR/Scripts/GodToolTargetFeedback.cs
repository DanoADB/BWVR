using UnityEngine;
namespace BWVR {
public sealed partial class GodToolPalette {
 GameObject targetFeedback;UnityEngine.UI.Text targetFeedbackText;
 void BuildTargetFeedback(){
  targetFeedback=new GameObject("Tool target rejection",typeof(RectTransform),typeof(Canvas));targetFeedback.transform.SetParent(transform,false);
  targetFeedback.GetComponent<RectTransform>().sizeDelta=new Vector2(380,72);var canvas=targetFeedback.GetComponent<Canvas>();canvas.renderMode=RenderMode.WorldSpace;canvas.overrideSorting=true;canvas.sortingOrder=102;
  var plate=Panel("Target reason background",targetFeedback.transform,new Vector2(380,72),Vector2.zero,new Color(.15f,.2f,.18f,.95f));plate.material=buttonStone;plate.raycastTarget=false;
  targetFeedbackText=Text("Target reason",targetFeedback.transform,Vector2.zero,new Vector2(352,64),20);targetFeedbackText.material=dockInk;targetFeedbackText.supportRichText=false;targetFeedbackText.resizeTextForBestFit=true;targetFeedbackText.resizeTextMinSize=16;targetFeedbackText.resizeTextMaxSize=20;targetFeedbackText.text="";targetFeedback.SetActive(false);
 }
 void RefreshTargetFeedback(){
  if(targetFeedback==null)return;bool show=Selected.HasValue&&!Open&&!DockHovered&&!PreviewValid&&preview!=null&&preview.activeSelf&&!Season.paused&&!miracles.Suspended&&!string.IsNullOrWhiteSpace(Reason);
  targetFeedback.SetActive(show);if(!show)return;
  targetFeedbackText.text=Reason;float distance=Mathf.Max(rig.localScale.x*.35f,Vector3.Distance(eye.transform.position,PreviewPoint));targetFeedback.transform.localScale=Vector3.one*(distance*.00035f);
  targetFeedback.transform.position=PreviewPoint+eye.transform.up*(distance*.045f);var forward=targetFeedback.transform.position-eye.transform.position;if(forward.sqrMagnitude>1e-10f)targetFeedback.transform.rotation=Quaternion.LookRotation(forward,eye.transform.up);
 }
 public bool Process(Ray ray,bool pressed,bool tracked,bool blocked,float dt,bool xr){try{return ProcessTools(ray,pressed,tracked,blocked,dt,xr);}finally{RefreshTargetFeedback();}}
}
}
