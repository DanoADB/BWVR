using System;
using System.Linq;
using TMPro;
using UnityEngine;
namespace BWVR {
public sealed partial class QuestSlice {
 GameObject sanctuaryHome,sanctuaryOptions,sanctuaryHelp,sanctuaryRestart;
 float sanctuarySavedUntil;TMP_Text sanctuarySummary,pauseCaption,saveCaption;Material sanctuaryStone,sanctuaryFont;UnityEngine.UI.Image sanctuaryHover;Color sanctuaryHoverColor;Collider[] sanctuaryColliders;
 public bool MenuOpen=>panel!=null&&panel.activeInHierarchy;
 static readonly Color ParchmentInk=new Color(.19f,.12f,.065f),StoneInk=new Color(.98f,.94f,.80f);
 GameObject SanctuaryPage(string name){var go=new GameObject(name,typeof(RectTransform));go.transform.SetParent(panel.transform,false);go.GetComponent<RectTransform>().sizeDelta=new Vector2(580,720);return go;}
 TMP_Text SanctuaryLabel(string name,Transform parent,Vector2 at,Vector2 size,string text,int pixels,Color color){
  var go=new GameObject(name,typeof(RectTransform));go.SetActive(false);go.transform.SetParent(parent,false);var label=go.AddComponent<TextMeshProUGUI>();label.font=Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");label.fontSharedMaterial=sanctuaryFont;label.fontSize=pixels;label.color=color;label.alignment=TextAlignmentOptions.Center;label.richText=false;label.raycastTarget=false;label.text=text;label.rectTransform.sizeDelta=size;label.rectTransform.anchoredPosition=at;label.rectTransform.localPosition+=Vector3.back*48;go.SetActive(true);return label;
 }
 TMP_Text SanctuaryButton(string title,Transform parent,Vector2 at,Action action,bool primary=false){
  var go=new GameObject(title,typeof(RectTransform),typeof(BoxCollider),typeof(SpatialTarget));go.transform.SetParent(parent,false);go.GetComponent<RectTransform>().anchoredPosition=at;var size=primary?new Vector2(220,190):new Vector2(240,68);go.GetComponent<RectTransform>().sizeDelta=size;var hit=go.GetComponent<BoxCollider>();hit.isTrigger=true;hit.center=Vector3.back*20;hit.size=new Vector3(size.x,size.y,55);go.GetComponent<SpatialTarget>().invoke=action;
  var disk=go.AddComponent<UnityEngine.UI.Image>();disk.material=sanctuaryStone;disk.raycastTarget=false;
  return SanctuaryLabel("Caption",go.transform,Vector2.zero,primary?new Vector2(170,120):new Vector2(210,56),title,primary?28:23,StoneInk);
 }
 void BuildSanctuary(){
  bookMaterial=new Material(Resources.Load<Shader>("Book/CreationBookUI")){name="Private parchment UI"};
  sanctuaryStone=new Material(Resources.Load<Shader>("Sanctuary/StoneUI")){name="Shared private menu rounded stone buttons"};
  sanctuaryFont=new Material(Resources.Load<Material>("Sanctuary/TextOverlay")){name="Private sanctuary SDF text"};
  panel.transform.Find("Obsidian panel").GetComponent<UnityEngine.UI.Image>().material=bookMaterial;
  sanctuaryHome=SanctuaryPage("Sanctuary home");sanctuaryOptions=SanctuaryPage("Sanctuary options");sanctuaryHelp=SanctuaryPage("Sanctuary help");sanctuaryRestart=SanctuaryPage("Confirm new season");
  var blocker=panel.AddComponent<BoxCollider>();blocker.isTrigger=true;blocker.center=new Vector3(0,0,20);blocker.size=new Vector3(600,740,8);
  SanctuaryLabel("Sanctuary title",sanctuaryHome.transform,new Vector2(0,305),new Vector2(540,45),"SANCTUARY",32,ParchmentInk);
  sanctuarySummary=SanctuaryLabel("Season summary",sanctuaryHome.transform,new Vector2(0,245),new Vector2(530,60),"",21,ParchmentInk);
  SanctuaryButton("God tools",sanctuaryHome.transform,new Vector2(-135,100),()=>{SetSanctuaryOpen(false);godTools.Toggle();},true);
  SanctuaryButton("Book of\nCreation",sanctuaryHome.transform,new Vector2(135,100),()=>ShowCreationBook(true),true);
  pauseCaption=SanctuaryButton("Pause",sanctuaryHome.transform,new Vector2(-135,-115),()=>{state.paused=!state.paused;RefreshSanctuary();},true);
  saveCaption=SanctuaryButton("Save",sanctuaryHome.transform,new Vector2(135,-115),()=>{Save();if(saveError==null)sanctuarySavedUntil=Time.unscaledTime+3;RefreshSanctuary();},true);
  SanctuaryButton("Settings",sanctuaryHome.transform,new Vector2(-135,-285),()=>ShowSanctuaryPage(sanctuaryOptions));SanctuaryButton("Close",sanctuaryHome.transform,new Vector2(135,-285),()=>SetSanctuaryOpen(false));
  SanctuaryLabel("Open hint",sanctuaryHome.transform,new Vector2(0,-345),new Vector2(540,30),"X opens / closes · point + right front trigger",17,ParchmentInk);
  SanctuaryLabel("Settings title",sanctuaryOptions.transform,new Vector2(0,305),new Vector2(540,45),"SETTINGS",32,ParchmentInk);
  SanctuaryButton("Sound",sanctuaryOptions.transform,new Vector2(-135,160),()=>ShowAudioSettings(true));SanctuaryButton("Help / details",sanctuaryOptions.transform,new Vector2(135,160),()=>ShowSanctuaryPage(sanctuaryHelp));
  SanctuaryButton("Spirit: show / hide",sanctuaryOptions.transform,new Vector2(-135,50),()=>spirit.SetVisible(!spirit.Visible));SanctuaryButton("Begin again",sanctuaryOptions.transform,new Vector2(135,50),()=>ShowSanctuaryPage(sanctuaryRestart));
  SanctuaryButton("Gathering Storm",sanctuaryOptions.transform,new Vector2(0,-75),()=>{if(Chapter.State.Active||Chapter.Start())SetSanctuaryOpen(false);});
  SanctuaryButton("End chapter",sanctuaryOptions.transform,new Vector2(0,-175),()=>{if(Chapter.Stop())SetSanctuaryOpen(false);});
  SanctuaryButton("Back",sanctuaryOptions.transform,new Vector2(0,-285),()=>ShowSanctuaryPage(sanctuaryHome));
  SanctuaryLabel("Help title",sanctuaryHelp.transform,new Vector2(0,305),new Vector2(540,45),"HOW TO PLAY",32,ParchmentInk);
  SanctuaryLabel("Controls",sanctuaryHelp.transform,new Vector2(0,145),new Vector2(520,215),"Left stick: travel · right stick: turn / height\nRight SIDE grip + right stick: zoom\nRight FRONT trigger: hold to carry, release to place\nMove and release a ball to toss · A / B: teach\nTurn left wrist underside toward you: God Tools\nDesktop: Tab menu · T tools · E carry · F select\nSpace + release E: toss · 1 / 2: teach",22,ParchmentInk);
  status=Label("Season and guidance",sanctuaryHelp.transform,new Vector3(0,-100,-48),new Vector2(520,200),1,20);status.material=bookMaterial;status.color=ParchmentInk;status.raycastTarget=false;status.verticalOverflow=VerticalWrapMode.Truncate;
  SanctuaryButton("Details: show / hide",sanctuaryHelp.transform,new Vector2(0,-240),()=>SetHelp(!helpVisible));SanctuaryButton("Back",sanctuaryHelp.transform,new Vector2(0,-325),()=>ShowSanctuaryPage(sanctuaryOptions));
  SanctuaryLabel("Restart title",sanctuaryRestart.transform,new Vector2(0,245),new Vector2(520,60),"BEGIN A NEW SEASON?",30,ParchmentInk);
  SanctuaryLabel("Restart warning",sanctuaryRestart.transform,new Vector2(0,90),new Vector2(500,155),"This replaces your current season and its learned history.\n\nYour next save will contain the new season.",25,ParchmentInk);
  SanctuaryButton("Keep playing",sanctuaryRestart.transform,new Vector2(-135,-110),()=>ShowSanctuaryPage(sanctuaryOptions));SanctuaryButton("Start new season",sanctuaryRestart.transform,new Vector2(135,-110),()=>{RestartSeason();ShowSanctuaryPage(sanctuaryHome);});
 }
 void ShowSanctuaryPage(GameObject page){SetSanctuaryHover(null);foreach(Transform child in panel.transform)if(child.name!="Obsidian panel")child.gameObject.SetActive(child.gameObject==page);RefreshSanctuary();}
 public bool SetSanctuaryOpen(bool show){if(show&&held.Any(i=>i!=null))return false;if(show){godTools?.Cancel();ShowSanctuaryPage(sanctuaryHome);}panel.SetActive(show);SetSanctuaryHover(null);return true;}
 void RefreshSanctuary(){if(sanctuarySummary==null)return;sanctuarySummary.text=(Chapter!=null&&Chapter.State.phase!=StormPhase.Dormant?Chapter.Description:developmentFocus?"Exploration":"The Long Dry")+(state.paused?" · Paused":"")+" · Energy "+(miracles.UnlimitedEnergy?"unlimited":Mathf.FloorToInt(state.Hybrid.energy).ToString())+(state.playOuting?.phase==PlayOutingPhase.Active?" · Play Day "+state.playOuting.Count+"/3":state.playOuting?.phase==PlayOutingPhase.Complete?" · Play Day complete":"")+(saveError==null?"":"\n"+saveError);pauseCaption.text=state.paused?"Resume":"Pause";saveCaption.text=Time.unscaledTime<sanctuarySavedUntil?"Saved":"Save";}
 // Private world-space UI retains the game's physical ray/SpatialTarget input path.
 // Consume both hands even when pointing off the scroll. Tracking and edge gates prevent click-through.
 bool SanctuaryPointer(int side,bool pressed,bool wasPressed,Collider hit,bool tracked){
  if(!MenuOpen)return false;if(side==1){var target=tracked&&hit!=null&&hit.transform.IsChildOf(panel.transform)?hit.GetComponent<SpatialTarget>():null;SetSanctuaryHover(target!=null?target.GetComponent<UnityEngine.UI.Image>():null);if(pressed&&!wasPressed&&target!=null)target.Activate();}return true;
 }
 // Menu is rendered privately over scenery; its physical targets must use the same visibility rule.
 bool SanctuaryRaycast(Ray ray,float distance,out RaycastHit hit){hit=default;bool found=false;foreach(var collider in sanctuaryColliders){if(collider==null||!collider.enabled||!collider.gameObject.activeInHierarchy)continue;if(collider.Raycast(ray,out var candidate,distance)){found=true;distance=candidate.distance;hit=candidate;}}return found;}
 void SetSanctuaryHover(UnityEngine.UI.Image image){if(sanctuaryHover==image)return;if(sanctuaryHover!=null)sanctuaryHover.color=sanctuaryHoverColor;sanctuaryHover=image;if(image!=null){sanctuaryHoverColor=image.color;image.color=new Color(1.15f,1.12f,.92f);}}
 void StyleSanctuarySubview(GameObject view){foreach(var text in view.GetComponentsInChildren<UnityEngine.UI.Text>(true)){text.color=ParchmentInk;text.material=bookMaterial;text.raycastTarget=false;}foreach(var target in view.GetComponentsInChildren<SpatialTarget>(true)){var image=target.GetComponent<UnityEngine.UI.Image>();if(image!=null){image.material=sanctuaryStone;image.color=Color.white;image.raycastTarget=false;}var hit=target.GetComponent<BoxCollider>();hit.isTrigger=true;hit.center=Vector3.back*20;hit.size=new Vector3(hit.size.x,hit.size.y,55);var text=target.GetComponentInChildren<UnityEngine.UI.Text>();text.color=StoneInk;text.transform.localPosition=Vector3.back*48;}}

 void RestartSeason(){foreach(var job in jobs)job.GetComponent<VillagerSocial>().ResetTask();state=new SliceState{developmentFocus=developmentFocus,observeWithoutFoodWork=developmentFocus&&observeWithoutFoodWork};hybrid?.NewSeason();miracles?.ResetWorld();creatureTrip=0;outcomeShown=false;signals.ResetRequests();curiosity?.ResetTask();construction?.ResetConstruction();discoveries?.ResetWorld();learning?.ResetWorld();foreach(var ball in balls)ball.ResetWorld();treePlay?.ResetWorld();weather?.ResetWorld();resources?.ResetWorld();families?.ResetWorld();tributes?.ResetWorld();cathedral?.ResetWorld();creatureRoutine.ResetTask();foreach(var job in jobs){job.GetComponent<VillagerExploration>().ResetTask();job.GetComponent<VillagerSocial>().ResetTask();job.ResetTask();}notice="A new dry season begins.";}
}
}
