using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
namespace BWVR {
// A private, bounded tool context. No tool preview participates in actor perception.
public sealed partial class GodToolPalette:MonoBehaviour {
 public const int ToolCount=10;
 static readonly SpiritualTool[] ToolKinds={SpiritualTool.Help,SpiritualTool.Protect,SpiritualTool.Investigate,SpiritualTool.Rain,SpiritualTool.Heal,SpiritualTool.WindFan,SpiritualTool.PlantSeeds,SpiritualTool.FoodGift,SpiritualTool.WoodGift,SpiritualTool.Whirlwind};
 public WorldBrush Brush{get;private set;}
 public const string SpotId="player-guidance-spot";
 public bool Open{get;private set;}public SpiritualTool? Selected{get;private set;}
 public bool PreviewValid{get;private set;}public Vector3 PreviewPoint{get;private set;}
 public string Reason{get;private set;}="";public int Uses{get;private set;}
 public bool ClaimsInput=>Open||Selected.HasValue||releaseGate;
 public GameObject View=>view;public GameObject Sign=>sign;
 GodToolEmblems emblems;DivineIntervention miracles;CreatureHybrid creature;IslandNavigation nav;Collider ground;
 Transform left,right,rig;Camera eye;Font font;GameObject view,wand,preview,sign;HybridTarget spot,probe;
 readonly Collider[] choices=new Collider[ToolCount+2];GameObject[] glyphs=new GameObject[3];
 UnityEngine.UI.Text energy,hint;UnityEngine.UI.Image energyFill;Material previewMaterial,buttonStone;
 bool releaseGate,pressedLast,gestureLatched,gestureOpen;float dwell,previewTimer;int wandKind=-1;
 Vector3 desktopAnchor;Quaternion desktopRotation;
 SliceState plantStrokeSeason;float nextPlantAt;
 Vector3 fanDirection=Vector3.forward;
 HybridTarget aimed;Action<bool,string> feedback;
 SliceState Season=>creature.Season;
 public static GodToolPalette Build(Transform parent,Transform left,Transform right,Transform rig,Camera eye,Font font,DivineIntervention miracles,CreatureHybrid creature,IslandNavigation nav,Collider ground,Action<bool,string> feedback){
  var root=new GameObject("Summonable god tools");root.transform.SetParent(parent,false);var p=root.AddComponent<GodToolPalette>();p.left=left;p.right=right;p.rig=rig;p.eye=eye;p.font=font;p.miracles=miracles;p.creature=creature;p.nav=nav;p.ground=ground;p.feedback=feedback;p.Brush=WorldBrush.Build(root.transform,creature,miracles,nav);p.Build();return p;
 }
 void Build(){
  emblems=new GodToolEmblems();buttonStone=new Material(Resources.Load<Shader>("Sanctuary/StoneUI")){name="Rounded tool palette buttons",renderQueue=3000};
  int rows=(ToolCount+2)/3;float height=320+Mathf.Max(0,rows-2)*102;
  view=new GameObject("Underside wrist tool palette",typeof(UnityEngine.Canvas));view.transform.SetParent(transform,false);view.GetComponent<UnityEngine.Canvas>().renderMode=RenderMode.WorldSpace;view.GetComponent<RectTransform>().sizeDelta=new Vector2(510,height);
  var bg=Panel("CC0 painted scroll",view.transform,new Vector2(700,height+218),new Vector2(25,0),Color.white);bg.sprite=Resources.Load<Sprite>("GodToolScroll/parchment");bg.raycastTarget=false;
  Text("Title",view.transform,new Vector2(0,height*.5f-26),new Vector2(460,35),24).text="GOD TOOLS";
  for(int i=0;i<ToolCount;i++){int column=i%3;float x=(column-1)*158;if(i/3==rows-1&&ToolCount%3!=0)x=ToolCount%3==1?0:(column-.5f)*158;float y=height*.5f-102-(i/3)*102;var b=Panel((ToolKinds[i]).ToString(),view.transform,new Vector2(146,94),new Vector2(x,y),new Color(0,0,0,0));var hit=b.gameObject.AddComponent<BoxCollider>();hit.size=new Vector3(146,94,12);choices[i]=hit;
   var symbol=Symbol(ToolKinds[i],b.transform);symbol.transform.localPosition=new Vector3(0,8,-16);symbol.transform.localScale=Vector3.one*18;
   Text("Tool caption",b.transform,new Vector2(0,-32),new Vector2(140,24),17).text=Title(ToolKinds[i]);
  }
  for(int i=ToolCount;i<ToolCount+2;i++){var b=Panel(i==ToolCount?"Clear cue":"Dismiss tools",view.transform,new Vector2(146,32),new Vector2((i==ToolCount?-1:1)*158,-height*.5f+33),new Color(.26f,.18f,.10f,.10f));b.material=buttonStone;b.color=Color.white;var hit=b.gameObject.AddComponent<BoxCollider>();hit.size=new Vector3(146,32,12);choices[i]=hit;Text("Caption",b.transform,Vector2.zero,new Vector2(140,30),16).text=i==ToolCount?"Clear cue":"Close";}
  Text("Icon artwork credits",view.transform,new Vector2(0,-height*.5f+8),new Vector2(490,14),10).text=GodToolEmblems.Credit;
  energy=Text("Spiritual energy",view.transform,new Vector2(0,-height*.5f+51),new Vector2(155,25),16);
  Panel("Energy track",view.transform,new Vector2(140,6),new Vector2(0,-height*.5f+27),new Color(.2f,.3f,.3f));energyFill=Panel("Energy remaining",view.transform,new Vector2(140,6),new Vector2(0,-height*.5f+27),new Color(.5f,.85f,.75f));energyFill.rectTransform.pivot=new Vector2(0,.5f);energyFill.rectTransform.anchoredPosition=new Vector2(-70,-height*.5f+27);
  foreach(var label in view.GetComponentsInChildren<UnityEngine.UI.Text>())label.color=new Color(.19f,.12f,.065f);
  foreach(var choice in choices.Skip(ToolCount))if(choice!=null)choice.GetComponentInChildren<UnityEngine.UI.Text>().color=new Color(.98f,.94f,.8f);
  var card=new GameObject("Selected tool hint",typeof(UnityEngine.Canvas));card.transform.SetParent(transform,false);card.GetComponent<UnityEngine.Canvas>().renderMode=RenderMode.WorldSpace;hint=Text("Use hint",card.transform,Vector2.zero,new Vector2(470,60),20);hint.transform.parent.localScale=Vector3.one*.00065f;hint.gameObject.SetActive(false);
  spot=HybridTarget.Bind(new GameObject("Saved guidance point"),SpotId,HybridTargetKind.Object);spot.transform.SetParent(transform,false);spot.transform.position=creature.transform.position;creature.Register(spot);
  probe=HybridTarget.Bind(new GameObject("Private placement probe"),"private-preview",HybridTargetKind.Object);probe.transform.SetParent(transform,false);
  sign=new GameObject("Player floating guidance cue");sign.transform.SetParent(transform,false);
  for(int i=0;i<3;i++){glyphs[i]=Symbol((SpiritualTool)i,sign.transform);glyphs[i].transform.localPosition=Vector3.zero;glyphs[i].transform.localScale=Vector3.one*1.6f;}
  sign.transform.localScale=Vector3.one*nav.Scale;sign.SetActive(false);
  previewMaterial=new Material(Shader.Find("BWVR/GameSurface")){color=new Color(.4f,1,.8f)};
  preview=new GameObject("Tool target ring");preview.transform.SetParent(transform,false);var ring=preview.AddComponent<LineRenderer>();ring.sharedMaterial=previewMaterial;ring.useWorldSpace=false;ring.loop=true;ring.positionCount=40;ring.widthMultiplier=nav.Scale*.35f;ring.shadowCastingMode=ShadowCastingMode.Off;
  for(int i=0;i<40;i++){float a=i*Mathf.PI/20;ring.SetPosition(i,new Vector3(Mathf.Cos(a),.25f,Mathf.Sin(a))*nav.Scale*5);}
  preview.SetActive(false);view.SetActive(false);BuildSeedTargets();RestoreSign();
 }
 UnityEngine.UI.Image Panel(string name,Transform parent,Vector2 size,Vector2 at,Color color){var g=new GameObject(name,typeof(RectTransform),typeof(UnityEngine.UI.Image));g.transform.SetParent(parent,false);var image=g.GetComponent<UnityEngine.UI.Image>();image.color=color;image.rectTransform.sizeDelta=size;image.rectTransform.anchoredPosition=at;return image;}
 UnityEngine.UI.Text Text(string name,Transform parent,Vector2 at,Vector2 size,int pixels){var g=new GameObject(name,typeof(RectTransform),typeof(UnityEngine.UI.Text));g.transform.SetParent(parent,false);var text=g.GetComponent<UnityEngine.UI.Text>();text.font=font;text.fontSize=pixels;text.alignment=TextAnchor.MiddleCenter;text.color=new Color(.96f,.91f,.74f);text.raycastTarget=false;text.rectTransform.sizeDelta=size;text.rectTransform.anchoredPosition=at;return text;}
 // Copy only mesh views: no CarryItem, colliders, scripts or private material copies.
 static string Title(SpiritualTool kind)=>kind==SpiritualTool.FoodGift?"Food":kind==SpiritualTool.WoodGift?"Wood":kind==SpiritualTool.PlantSeeds?"Plant seeds":kind==SpiritualTool.RainBrush?"Local rain":kind==SpiritualTool.WindFan?"Wind fan":kind.ToString();
 GameObject Symbol(SpiritualTool kind,Transform parent)=>emblems.Create(kind,parent);
 public void Toggle(){SeedVisibility(false);Brush.Stop();if(Open){Cancel();return;}if(Season.paused||miracles.Suspended)return;Selected=null;if(wand!=null)wand.SetActive(false);preview.SetActive(false);hint.gameObject.SetActive(false);Open=true;releaseGate=true;gestureOpen=false;desktopAnchor=rig.InverseTransformPoint(eye.transform.position+eye.transform.forward*.55f*rig.localScale.x);desktopRotation=Quaternion.Inverse(rig.rotation)*eye.transform.rotation;view.SetActive(true);}
 public void Select(SpiritualTool tool){if(Season.paused||miracles.Suspended||!Enum.IsDefined(typeof(SpiritualTool),tool))return;Brush.Stop();SeedVisibility(tool==SpiritualTool.PlantSeeds);Selected=tool;Open=false;view.SetActive(false);releaseGate=true;previewTimer=0;ShowWand(tool);}
 void ShowWand(SpiritualTool tool){if(wandKind==(int)tool&&wand!=null){wand.SetActive(true);return;}if(wand!=null)Destroy(wand);wand=Symbol(tool,transform);wandKind=(int)tool;}
 public void Cancel(){SeedVisibility(false);Brush?.Stop();Open=false;Selected=null;releaseGate=true;dwell=0;gestureOpen=false;if(view!=null)view.SetActive(false);if(wand!=null)wand.SetActive(false);if(preview!=null)preview.SetActive(false);if(hint!=null)hint.gameObject.SetActive(false);PreviewValid=false;}
 public void ClearSign(){if(Season.Hybrid.guidance.playerSign)creature.CancelGuidance("Sign removed by the player");sign.SetActive(false);}
 public void RestoreSign(){var g=Season.Hybrid.guidance;if(!g.playerSign||g.status!=GuidanceStatus.Active)return;if(g.target==SpotId){if(nav.Terrain!=null&&!nav.Terrain.Dry(g.spot)||!creature.CanReach(g.spot)){creature.CancelGuidance("Saved sign is unreachable");return;}spot.transform.position=g.spot;}RefreshSign();}
 void RefreshSign(){var g=Season.Hybrid.guidance;bool visible=g.playerSign&&g.status==GuidanceStatus.Active&&Season.Hybrid.time<g.expiresAt;sign.SetActive(visible);if(!visible)return;
  var target=g.target==SpotId?null:creature.Find(g.target);var anchor=target!=null?target.transform.position:g.spot;float height=6;var collider=target!=null?target.GetComponent<Collider>():null;if(collider!=null)height=Mathf.Clamp((collider.bounds.max.y-anchor.y)/nav.Scale+2,4,30);
  float bob=Mathf.Sin(Season.Hybrid.time*1.5f)*.22f;sign.transform.position=anchor+Vector3.up*nav.Scale*(height+bob);var facing=sign.transform.position-eye.transform.position;if(facing.sqrMagnitude>1e-10f)sign.transform.rotation=Quaternion.LookRotation(facing,Vector3.up);for(int i=0;i<3;i++)glyphs[i].SetActive(i==(int)g.intention);
 }
 // XR grip-up axis is provisional; the sanctuary fallback never requires wrist rotation.
 public void Wrist(float dt,bool xr,bool tracked,bool blocked){if(xr&&!tracked&&Open)Cancel();if(!xr||!tracked||blocked||Selected.HasValue){dwell=0;return;}var toward=eye.transform.position-left.position;float facing=Vector3.Dot(-left.up,toward.normalized);bool looking=Vector3.Dot(eye.transform.forward,-toward.normalized)>.6f&&toward.magnitude<.75f*rig.localScale.x;
  if(facing<.2f||!looking){dwell=0;gestureLatched=false;if(Open&&gestureOpen)Cancel();return;}
  if(facing>.65f&&!gestureLatched&&!Open){dwell+=Mathf.Min(dt,.05f);if(dwell>=.35f){Toggle();gestureOpen=true;gestureLatched=true;}}
 }
 public bool Process(Ray ray,bool pressed,bool tracked,bool blocked,float dt,bool xr){
  RefreshSign();bool owned=ClaimsInput;if(blocked||!tracked){if(Open||Selected.HasValue)Cancel();releaseGate=pressed;pressedLast=pressed;return !blocked&&owned;}
  if(!pressed)releaseGate=false;bool edge=pressed&&!pressedLast&&!releaseGate;pressedLast=pressed;
  var scale=rig.localScale.x;
  view.transform.SetPositionAndRotation(xr?left.position+left.up*.08f*scale+left.forward*.08f*scale:rig.TransformPoint(desktopAnchor),xr?Quaternion.LookRotation(left.position-eye.transform.position):rig.rotation*desktopRotation);view.transform.localScale=Vector3.one*.0008f*scale;
  if(Open){bool pointing=Physics.Raycast(ray,out var hit,8*scale);int hovered=pointing?Array.IndexOf(choices,hit.collider):-1;energy.text=miracles.UnlimitedEnergy?"Energy unlimited":$"Energy {Season.Hybrid.energy:0}";energyFill.rectTransform.sizeDelta=new Vector2(140*(miracles.UnlimitedEnergy?1:Mathf.Clamp01(Season.Hybrid.energy/Mathf.Max(1,miracles.tuning.maximum))),6);for(int i=0;i<ToolCount;i++){var kind=ToolKinds[i];float cost=kind==SpiritualTool.Whirlwind?WorldBrush.WhirlwindCost:kind==SpiritualTool.Rain?WorldBrush.RainCost:kind==SpiritualTool.Heal?miracles.tuning.healingCost:kind==SpiritualTool.RainBrush?WorldBrush.RainCost:kind==SpiritualTool.WindFan?WorldBrush.WindCost:kind==SpiritualTool.FoodGift||kind==SpiritualTool.WoodGift?WorldBrush.GiftCost:0;choices[i].GetComponentInChildren<UnityEngine.UI.Text>().color=miracles.CanAfford(cost)?new Color(.19f,.12f,.065f):new Color(.48f,.36f,.25f);choices[i].GetComponent<UnityEngine.UI.Image>().color=Color.clear;choices[i].transform.GetChild(0).localScale=Vector3.one*(i==hovered?19.2f:18);}
   if(edge){int index=hovered;if(index<ToolCount&&index>=0)Select(ToolKinds[index]);else if(index==ToolCount){ClearSign();releaseGate=true;}else if(index==ToolCount+1)Cancel();}return true;
  }
  if(!Selected.HasValue)return owned;
  var wandPoint=right.position+right.forward*.16f*scale;var wandFacing=wandPoint-eye.transform.position;wand.transform.SetPositionAndRotation(wandPoint,wandFacing.sqrMagnitude>1e-10f?Quaternion.LookRotation(wandFacing):right.rotation);wand.transform.localScale=Vector3.one*.022f*scale;
  hint.gameObject.SetActive(true);hint.transform.parent.SetPositionAndRotation(right.position+right.forward*.23f*scale-right.up*.07f*scale,Quaternion.LookRotation(right.position-eye.transform.position));hint.transform.parent.localScale=Vector3.one*.00065f*scale;
  previewTimer-=dt;if(previewTimer<=0||edge){previewTimer=.1f;Aim(ray);}
  if(Selected==SpiritualTool.Rain||Selected==SpiritualTool.RainBrush){if(pressed&&!releaseGate&&Brush.PulseReady)Aim(ray);bool applied=Brush.ProcessRain(PreviewPoint,pressed&&!releaseGate,dt,PreviewValid);if(applied)Uses++;hint.text="Rain · "+(Brush.Active?"Raining":PreviewValid?"Hold trigger over ground":Reason);return true;}
  if(Selected==SpiritualTool.Whirlwind){bool applied=Brush.ProcessWhirlwind(PreviewPoint,pressed&&!releaseGate,dt,PreviewValid);if(applied)Uses++;hint.text="Whirlwind · "+(Brush.Active?"Hold to spin · release to scatter":PreviewValid?"Hold over loose toys and blocks":Brush.Reason.Length>0?Brush.Reason:Reason);return true;}
  if(Selected==SpiritualTool.WindFan){if(pressed&&!releaseGate&&Brush.PulseReady)Aim(ray);bool applied=Brush.ProcessWind(PreviewPoint,fanDirection,pressed&&!releaseGate,dt,PreviewValid);if(applied)Uses++;hint.text="Wind · "+(Brush.Active?"Blowing":PreviewValid?"Hold trigger over ground":Reason);return true;}
  if(Selected==SpiritualTool.PlantSeeds){RefreshSeedTargets(dt);if(plantStrokeSeason!=Season){plantStrokeSeason=Season;nextPlantAt=0;}hint.text="Plant seeds · "+(PreviewValid?PlantCostHint():Reason);if(pressed&&!releaseGate&&PreviewValid&&Season.Hybrid.time>=nextPlantAt){Aim(ray);if(PreviewValid&&PlantAt(PreviewPoint))nextPlantAt=Season.Hybrid.time+.5f;}return true;}
  if(Selected==SpiritualTool.FoodGift||Selected==SpiritualTool.WoodGift){if(pressed&&!releaseGate&&Brush.GiftReady)Aim(ray);bool applied=Brush.ProcessGift(PreviewPoint,Selected==SpiritualTool.FoodGift,pressed&&!releaseGate,dt,PreviewValid);if(applied){Uses++;feedback?.Invoke(true,"Gift delivered · "+Brush.LastGiftWitnesses+" witnesses");}hint.text=Title(Selected.Value)+" · "+(PreviewValid?"Hold to gift village supplies":Reason);return true;}
  hint.text=Selected+" · "+(PreviewValid?"Trigger to use":Reason);if(edge){bool accepted=PreviewValid&&Apply(Selected.Value,aimed,PreviewPoint);releaseGate=true;if(!accepted)feedback?.Invoke(false,Reason);}
  return true;
 }
 void Aim(Ray ray){
  if(Selected==SpiritualTool.PlantSeeds){AimPlanting(ray);return;}
  if(Selected==SpiritualTool.Rain||Selected==SpiritualTool.RainBrush||Selected==SpiritualTool.WindFan||Selected==SpiritualTool.FoodGift||Selected==SpiritualTool.WoodGift||Selected==SpiritualTool.Whirlwind){bool hitGround=ground.Raycast(ray,out var hit,8*rig.localScale.x);PreviewPoint=hitGround?hit.point:ray.GetPoint(2*rig.localScale.x);string brushReason="Aim at island ground";fanDirection=Vector3.ProjectOnPlane(ray.direction,Vector3.up);if(fanDirection.sqrMagnitude<.0001f)fanDirection=Vector3.ProjectOnPlane(eye.transform.forward,Vector3.up);fanDirection.Normalize();PreviewValid=hitGround&&(Selected==SpiritualTool.Whirlwind?Brush.CanWhirlwind(PreviewPoint,out brushReason):Selected==SpiritualTool.Rain||Selected==SpiritualTool.RainBrush?Brush.CanRain(PreviewPoint,out brushReason):Selected==SpiritualTool.WindFan?Brush.CanWind(PreviewPoint,fanDirection,out brushReason):Brush.CanGift(PreviewPoint,Selected==SpiritualTool.FoodGift,out brushReason));Reason=brushReason;preview.SetActive(hitGround);preview.transform.position=PreviewPoint;PreviewShape(PreviewValid,Selected==SpiritualTool.Whirlwind?WorldBrush.WhirlwindRadius:12);if(PreviewValid&&Selected==SpiritualTool.WindFan)FanPreview();previewMaterial.color=PreviewValid?new Color(.4f,1,.8f):new Color(1,.25f,.18f);return;}
  PreviewShape(true,5);aimed=null;PreviewPoint=ray.GetPoint(2*rig.localScale.x);bool groundHit=false;
  foreach(var hit in Physics.RaycastAll(ray,8*rig.localScale.x).OrderBy(h=>h.distance)){if(hit.collider.GetComponentInParent<GodToolPalette>()==this||hit.collider.GetComponentInParent<DivineTool>()!=null)continue;PreviewPoint=hit.point;aimed=hit.collider.GetComponentInParent<HybridTarget>();groundHit=hit.collider==ground;break;}
  if(aimed==null&&Selected==SpiritualTool.Investigate&&groundHit&&(nav.Terrain==null||nav.Terrain.Dry(PreviewPoint))){probe.transform.position=PreviewPoint;aimed=probe;}
  PreviewValid=miracles.CanUse(Selected.Value,aimed,out var reason);Reason=reason;preview.SetActive(true);preview.transform.position=aimed?.villager!=null?aimed.transform.position:PreviewPoint;previewMaterial.color=PreviewValid?new Color(.4f,1,.8f):new Color(1,.25f,.18f);
 }
 public bool Apply(SpiritualTool tool,HybridTarget target,Vector3 point){
  if(tool==SpiritualTool.PlantSeeds)return PlantAt(point);
  if(tool==SpiritualTool.FoodGift||tool==SpiritualTool.WoodGift){bool gifted=Brush.GiftPulse(point,tool==SpiritualTool.FoodGift);Reason=Brush.Reason;if(gifted)Uses++;return gifted;}
  if(tool==SpiritualTool.Rain||tool==SpiritualTool.RainBrush||tool==SpiritualTool.WindFan||tool==SpiritualTool.Whirlwind){Reason="Hold the brush trigger over ground";return false;}
  if(!HybridState.Number(point.x,-10000,10000)||!HybridState.Number(point.y,-10000,10000)||!HybridState.Number(point.z,-10000,10000)){Reason="Invalid placement";return false;}if(!miracles.CanUse(tool,target,out var reason)){Reason=reason;return false;}
  if(target==probe){spot.transform.position=point;target=spot;}
  if(!miracles.TryUse(tool,target)){Reason=miracles.LastReason;return false;}
  if(tool<=SpiritualTool.Investigate){var g=Season.Hybrid.guidance;g.playerSign=true;g.spot=nav.Terrain!=null?nav.Terrain.Project(point):new Vector3(point.x,.82f,point.z);RefreshSign();}
  Uses++;Reason="";feedback?.Invoke(true,tool<=SpiritualTool.Investigate?"Cue placed. The creature chooses its response.":tool+" cast.");return true;
 }
 void FanPreview(){var line=preview.GetComponent<LineRenderer>();line.loop=false;line.positionCount=7;var rotation=Quaternion.LookRotation(fanDirection);var p=new[]{Vector3.zero,new Vector3(0,0,22),new Vector3(-3,0,18),new Vector3(0,0,22),new Vector3(3,0,18),new Vector3(0,0,22),Vector3.zero};for(int i=0;i<7;i++)line.SetPosition(i,(rotation*p[i]+Vector3.up*.25f)*nav.Scale);}
 void PreviewShape(bool valid,float radius){var line=preview.GetComponent<LineRenderer>();line.loop=valid;line.positionCount=valid?40:4;if(valid){for(int i=0;i<40;i++){float a=i*Mathf.PI/20;line.SetPosition(i,new Vector3(Mathf.Cos(a),.25f,Mathf.Sin(a))*nav.Scale*radius);}}else{var p=new[]{new Vector3(-1,.25f,-1),new Vector3(1,.25f,1),new Vector3(-1,.25f,1),new Vector3(1,.25f,-1)};for(int i=0;i<4;i++)line.SetPosition(i,p[i]*nav.Scale*radius*.6f);}}
 void OnDisable(){SeedVisibility(false);Brush?.Stop();}
 void OnDestroy(){if(buttonStone!=null)Destroy(buttonStone);emblems?.Dispose();if(previewMaterial!=null)Destroy(previewMaterial);}
}
}
