using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
namespace BWVR {
// A private, bounded tool context. No tool preview participates in actor perception.
public sealed partial class GodToolPalette:MonoBehaviour {
 public const int ToolCount=14;
 static readonly SpiritualTool[] ToolKinds={SpiritualTool.Help,SpiritualTool.Protect,SpiritualTool.Investigate,SpiritualTool.Rain,SpiritualTool.Heal,SpiritualTool.WindFan,SpiritualTool.PlantSeeds,SpiritualTool.FoodGift,SpiritualTool.WoodGift,SpiritualTool.Whirlwind,SpiritualTool.Lightning,SpiritualTool.RaiseGround,SpiritualTool.LowerGround,SpiritualTool.SmoothGround};
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
 bool releaseGate,pressedLast,gestureLatched;float dwell,previewTimer;int wandKind=-1;
 SliceState plantStrokeSeason;float nextPlantAt;
 Vector3 fanDirection=Vector3.forward;
 HybridTarget aimed;Action<bool,string> feedback;
 SliceState Season=>creature.Season;
 public static GodToolPalette Build(Transform parent,Transform left,Transform right,Transform rig,Camera eye,Font font,DivineIntervention miracles,CreatureHybrid creature,IslandNavigation nav,Collider ground,Action<bool,string> feedback){
  var root=new GameObject("Summonable god tools");root.transform.SetParent(parent,false);var p=root.AddComponent<GodToolPalette>();p.left=left;p.right=right;p.rig=rig;p.eye=eye;p.font=font;p.miracles=miracles;p.creature=creature;p.nav=nav;p.ground=ground;p.feedback=feedback;p.Brush=WorldBrush.Build(root.transform,creature,miracles,nav);p.Build();return p;
 }
 void Build(){
  emblems=new GodToolEmblems();buttonStone=new Material(Resources.Load<Shader>("Sanctuary/StoneUI")){name="Rounded tool palette buttons",renderQueue=3000};
  BuildDock();
  spot=HybridTarget.Bind(new GameObject("Saved guidance point"),SpotId,HybridTargetKind.Object);spot.transform.SetParent(transform,false);spot.transform.position=creature.transform.position;creature.Register(spot);
  probe=HybridTarget.Bind(new GameObject("Private placement probe"),"private-preview",HybridTargetKind.Object);probe.transform.SetParent(transform,false);
  sign=new GameObject("Player floating guidance cue");sign.transform.SetParent(transform,false);
  for(int i=0;i<3;i++){glyphs[i]=Symbol((SpiritualTool)i,sign.transform);glyphs[i].transform.localPosition=Vector3.zero;glyphs[i].transform.localScale=Vector3.one*1.6f;}
  sign.transform.localScale=Vector3.one*nav.Scale;sign.SetActive(false);
  previewMaterial=new Material(Shader.Find("BWVR/GameSurface")){color=new Color(.4f,1,.8f)};
  preview=new GameObject("Tool target ring");preview.transform.SetParent(transform,false);var ring=preview.AddComponent<LineRenderer>();ring.sharedMaterial=previewMaterial;ring.useWorldSpace=false;ring.loop=true;ring.positionCount=40;ring.widthMultiplier=nav.Scale*.35f;ring.shadowCastingMode=ShadowCastingMode.Off;
  for(int i=0;i<40;i++){float a=i*Mathf.PI/20;ring.SetPosition(i,new Vector3(Mathf.Cos(a),.25f,Mathf.Sin(a))*nav.Scale*5);}
  preview.SetActive(false);BuildSeedTargets();BuildSculpt();RestoreSign();
 }
 UnityEngine.UI.Image Panel(string name,Transform parent,Vector2 size,Vector2 at,Color color){var g=new GameObject(name,typeof(RectTransform),typeof(UnityEngine.UI.Image));g.transform.SetParent(parent,false);var image=g.GetComponent<UnityEngine.UI.Image>();image.color=color;image.rectTransform.sizeDelta=size;image.rectTransform.anchoredPosition=at;return image;}
 UnityEngine.UI.Text Text(string name,Transform parent,Vector2 at,Vector2 size,int pixels){var g=new GameObject(name,typeof(RectTransform),typeof(UnityEngine.UI.Text));g.transform.SetParent(parent,false);var text=g.GetComponent<UnityEngine.UI.Text>();text.font=font;text.fontSize=pixels;text.alignment=TextAnchor.MiddleCenter;text.color=new Color(.96f,.91f,.74f);text.raycastTarget=false;text.rectTransform.sizeDelta=size;text.rectTransform.anchoredPosition=at;return text;}
 // Copy only mesh views: no CarryItem, colliders, scripts or private material copies.
 static string Title(SpiritualTool kind)=>kind==SpiritualTool.RaiseGround?"Raise ground":kind==SpiritualTool.LowerGround?"Lower ground":kind==SpiritualTool.SmoothGround?"Smooth ground":kind==SpiritualTool.FoodGift?"Food":kind==SpiritualTool.WoodGift?"Wood":kind==SpiritualTool.PlantSeeds?"Plant seeds":kind==SpiritualTool.RainBrush?"Local rain":kind==SpiritualTool.WindFan?"Wind fan":kind.ToString();
 GameObject Symbol(SpiritualTool kind,Transform parent)=>emblems.Create(kind,parent);
 public void Toggle(){if(Open){Cancel();return;}if(Season.paused||miracles.Suspended)return;OpenCategory(Selected.HasValue?CategoryFor(Selected.Value):ActiveCategory);}
 public void Select(SpiritualTool tool){if(Season.paused||miracles.Suspended||!Enum.IsDefined(typeof(SpiritualTool),tool))return;CancelSculpt();SeedVisibility(tool==SpiritualTool.PlantSeeds);Brush.Stop();Selected=tool;Open=false;ActiveCategory=CategoryFor(tool);releaseGate=true;previewTimer=0;ShowWand(tool);RefreshDockRows();}
 void ShowWand(SpiritualTool tool){if(wandKind==(int)tool&&wand!=null){wand.SetActive(true);return;}if(wand!=null)Destroy(wand);wand=Symbol(tool,transform);wandKind=(int)tool;}
 public void Cancel(){CancelSculpt();SeedVisibility(false);Brush?.Stop();Open=false;Selected=null;releaseGate=true;dwell=0;if(wand!=null)wand.SetActive(false);if(preview!=null)preview.SetActive(false);if(hint!=null)hint.gameObject.SetActive(false);PreviewValid=false;RefreshDockRows();}
 public void ClearSign(){if(Season.Hybrid.guidance.playerSign)creature.CancelGuidance("Sign removed by the player");sign.SetActive(false);}
 public void RestoreSign(){var g=Season.Hybrid.guidance;if(!g.playerSign||g.status!=GuidanceStatus.Active)return;if(g.target==SpotId){if(nav.Terrain!=null&&!nav.Terrain.Dry(g.spot)||!creature.CanReach(g.spot)){creature.CancelGuidance("Saved sign is unreachable");return;}spot.transform.position=g.spot;}RefreshSign();}
 void RefreshSign(){var g=Season.Hybrid.guidance;bool visible=g.playerSign&&g.status==GuidanceStatus.Active&&Season.Hybrid.time<g.expiresAt;sign.SetActive(visible);if(!visible)return;
  var target=g.target==SpotId?null:creature.Find(g.target);var anchor=target!=null?target.transform.position:g.spot;float height=6;var collider=target!=null?target.GetComponent<Collider>():null;if(collider!=null)height=Mathf.Clamp((collider.bounds.max.y-anchor.y)/nav.Scale+2,4,30);
  float bob=Mathf.Sin(Season.Hybrid.time*1.5f)*.22f;sign.transform.position=anchor+Vector3.up*nav.Scale*(height+bob);var facing=sign.transform.position-eye.transform.position;if(facing.sqrMagnitude>1e-10f)sign.transform.rotation=Quaternion.LookRotation(facing,Vector3.up);for(int i=0;i<3;i++)glyphs[i].SetActive(i==(int)g.intention);
 }
 // XR grip-up axis is provisional; the sanctuary fallback never requires wrist rotation.
 public void Wrist(float dt,bool xr,bool tracked,bool blocked){if(xr&&!tracked&&Open)Cancel();if(!xr||!tracked||blocked||Selected.HasValue){dwell=0;return;}var toward=eye.transform.position-left.position;float facing=Vector3.Dot(-left.up,toward.normalized);bool looking=Vector3.Dot(eye.transform.forward,-toward.normalized)>.6f&&toward.magnitude<.75f*rig.localScale.x;
  if(facing<.2f||!looking){dwell=0;gestureLatched=false;return;}
  if(facing>.65f&&!gestureLatched&&!Open){dwell+=Mathf.Min(dt,.05f);if(dwell>=.35f){Toggle();gestureLatched=true;}}
 }
 public bool Process(Ray ray,bool pressed,bool tracked,bool blocked,float dt,bool xr){
  RefreshSign();bool owned=ClaimsInput;if(blocked||!tracked){if(Open||Selected.HasValue)Cancel();HideDock();releaseGate=pressed;pressedLast=pressed;dockPressedLast=pressed;return !blocked&&owned;}
  bool uiPressed=pressed||(!xr&&UnityEngine.InputSystem.Mouse.current!=null&&UnityEngine.InputSystem.Mouse.current.leftButton.isPressed);
  if(!uiPressed)releaseGate=false;bool edge=pressed&&!pressedLast&&!releaseGate;pressedLast=pressed;
  var scale=rig.localScale.x;
  if(ProcessDock(ray,uiPressed,dt,xr))return true;
  if(!Selected.HasValue)return owned;
  var wandPoint=right.position+right.forward*.16f*scale;var wandFacing=wandPoint-eye.transform.position;wand.transform.SetPositionAndRotation(wandPoint,wandFacing.sqrMagnitude>1e-10f?Quaternion.LookRotation(wandFacing):right.rotation);wand.transform.localScale=Vector3.one*.022f*scale;
  hint.gameObject.SetActive(true);
  previewTimer-=dt;if(previewTimer<=0||edge){previewTimer=Selected.HasValue&&TerrainSculptor.IsSculpt(Selected.Value)?.25f:.1f;Aim(ray);}
  if(Selected==SpiritualTool.Rain||Selected==SpiritualTool.RainBrush){if(pressed&&!releaseGate&&Brush.PulseReady)Aim(ray);bool applied=Brush.ProcessRain(PreviewPoint,pressed&&!releaseGate,dt,PreviewValid);if(applied)Uses++;hint.text="Rain · "+(Brush.Active?"Raining":PreviewValid?"Hold trigger over ground":Reason);return true;}
  if(TerrainSculptor.IsSculpt(Selected.Value))return SculptPointer(ray,pressed);
  if(Selected==SpiritualTool.Lightning)return LightningPointer(ray,pressed,dt);
  if(Selected==SpiritualTool.Whirlwind){bool applied=Brush.ProcessWhirlwind(PreviewPoint,pressed&&!releaseGate,dt,PreviewValid);if(applied)Uses++;hint.text="Whirlwind · "+(Brush.Active?"Hold to spin · release to scatter":PreviewValid?"Hold over loose toys and blocks":Brush.Reason.Length>0?Brush.Reason:Reason);return true;}
  if(Selected==SpiritualTool.WindFan){if(pressed&&!releaseGate&&Brush.PulseReady)Aim(ray);bool applied=Brush.ProcessWind(PreviewPoint,fanDirection,pressed&&!releaseGate,dt,PreviewValid);if(applied)Uses++;hint.text="Wind · "+(Brush.Active?"Blowing":PreviewValid?"Hold trigger over ground":Reason);return true;}
  if(Selected==SpiritualTool.PlantSeeds){RefreshSeedTargets(dt);if(plantStrokeSeason!=Season){plantStrokeSeason=Season;nextPlantAt=0;}hint.text="Plant seeds · "+(PreviewValid?PlantCostHint():Reason);if(pressed&&!releaseGate&&PreviewValid&&Season.Hybrid.time>=nextPlantAt){Aim(ray);if(PreviewValid&&PlantAt(PreviewPoint))nextPlantAt=Season.Hybrid.time+.5f;}return true;}
  if(Selected==SpiritualTool.FoodGift||Selected==SpiritualTool.WoodGift){if(pressed&&!releaseGate&&Brush.GiftReady)Aim(ray);bool applied=Brush.ProcessGift(PreviewPoint,Selected==SpiritualTool.FoodGift,pressed&&!releaseGate,dt,PreviewValid);if(applied){Uses++;feedback?.Invoke(true,"Gift delivered · "+Brush.LastGiftWitnesses+" witnesses");}hint.text=Title(Selected.Value)+" · "+(PreviewValid?"Hold to gift village supplies":Reason);return true;}
  hint.text=Selected+" · "+(PreviewValid?"Trigger to use":Reason);if(edge){bool accepted=PreviewValid&&Apply(Selected.Value,aimed,PreviewPoint);releaseGate=true;if(!accepted)feedback?.Invoke(false,Reason);}
  return true;
 }
 void Aim(Ray ray){if(TerrainSculptor.IsSculpt(Selected.Value)){AimSculpt(ray);return;}if(Selected==SpiritualTool.Lightning){AimLightning(ray);return;}
  if(Selected==SpiritualTool.PlantSeeds){AimPlanting(ray);return;}
  if(Selected==SpiritualTool.Rain||Selected==SpiritualTool.RainBrush||Selected==SpiritualTool.WindFan||Selected==SpiritualTool.FoodGift||Selected==SpiritualTool.WoodGift||Selected==SpiritualTool.Whirlwind){bool hitGround=ground.Raycast(ray,out var hit,8*rig.localScale.x);PreviewPoint=hitGround?hit.point:ray.GetPoint(2*rig.localScale.x);string brushReason="Aim at island ground";fanDirection=Vector3.ProjectOnPlane(ray.direction,Vector3.up);if(fanDirection.sqrMagnitude<.0001f)fanDirection=Vector3.ProjectOnPlane(eye.transform.forward,Vector3.up);fanDirection.Normalize();PreviewValid=hitGround&&(Selected==SpiritualTool.Whirlwind?Brush.CanWhirlwind(PreviewPoint,out brushReason):Selected==SpiritualTool.Rain||Selected==SpiritualTool.RainBrush?Brush.CanRain(PreviewPoint,out brushReason):Selected==SpiritualTool.WindFan?Brush.CanWind(PreviewPoint,fanDirection,out brushReason):Brush.CanGift(PreviewPoint,Selected==SpiritualTool.FoodGift,out brushReason));Reason=brushReason;preview.SetActive(hitGround);preview.transform.position=PreviewPoint;PreviewShape(PreviewValid,Selected==SpiritualTool.Whirlwind?WorldBrush.WhirlwindRadius:12);if(PreviewValid&&Selected==SpiritualTool.WindFan)FanPreview();previewMaterial.color=PreviewValid?new Color(.4f,1,.8f):new Color(1,.25f,.18f);return;}
  PreviewShape(true,5);aimed=null;PreviewPoint=ray.GetPoint(2*rig.localScale.x);bool groundHit=false;
  foreach(var hit in Physics.RaycastAll(ray,8*rig.localScale.x).OrderBy(h=>h.distance)){if(hit.collider.GetComponentInParent<GodToolPalette>()==this||hit.collider.GetComponentInParent<DivineTool>()!=null)continue;PreviewPoint=hit.point;aimed=hit.collider.GetComponentInParent<HybridTarget>();groundHit=hit.collider==ground;break;}
  if(aimed==null&&Selected==SpiritualTool.Investigate&&groundHit&&(nav.Terrain==null||nav.Terrain.Dry(PreviewPoint))){probe.transform.position=PreviewPoint;aimed=probe;}
  PreviewValid=miracles.CanUse(Selected.Value,aimed,out var reason);Reason=reason;preview.SetActive(true);preview.transform.position=aimed?.villager!=null?aimed.transform.position:PreviewPoint;previewMaterial.color=PreviewValid?new Color(.4f,1,.8f):new Color(1,.25f,.18f);
 }
 public bool Apply(SpiritualTool tool,HybridTarget target,Vector3 point){
  if(TerrainSculptor.IsSculpt(tool)){Reason="Hold to preview, then release";return false;}
  if(tool==SpiritualTool.PlantSeeds)return PlantAt(point);
  if(tool==SpiritualTool.FoodGift||tool==SpiritualTool.WoodGift){bool gifted=Brush.GiftPulse(point,tool==SpiritualTool.FoodGift);Reason=Brush.Reason;if(gifted)Uses++;return gifted;}
  if(tool==SpiritualTool.Rain||tool==SpiritualTool.RainBrush||tool==SpiritualTool.WindFan||tool==SpiritualTool.Whirlwind||tool==SpiritualTool.Lightning){Reason="Hold the brush trigger over ground";return false;}
  if(!HybridState.Number(point.x,-10000,10000)||!HybridState.Number(point.y,-10000,10000)||!HybridState.Number(point.z,-10000,10000)){Reason="Invalid placement";return false;}if(!miracles.CanUse(tool,target,out var reason)){Reason=reason;return false;}
  if(target==probe){spot.transform.position=point;target=spot;}
  if(!miracles.TryUse(tool,target)){Reason=miracles.LastReason;return false;}
  if(tool<=SpiritualTool.Investigate){var g=Season.Hybrid.guidance;g.playerSign=true;g.spot=nav.Terrain!=null?nav.Terrain.Project(point):new Vector3(point.x,.82f,point.z);RefreshSign();}
  Uses++;Reason="";feedback?.Invoke(true,tool<=SpiritualTool.Investigate?"Cue placed. The creature chooses its response.":tool+" cast.");return true;
 }
 void FanPreview(){var line=preview.GetComponent<LineRenderer>();line.loop=false;line.positionCount=7;var rotation=Quaternion.LookRotation(fanDirection);var p=new[]{Vector3.zero,new Vector3(0,0,22),new Vector3(-3,0,18),new Vector3(0,0,22),new Vector3(3,0,18),new Vector3(0,0,22),Vector3.zero};for(int i=0;i<7;i++)line.SetPosition(i,(rotation*p[i]+Vector3.up*.25f)*nav.Scale);}
 void PreviewShape(bool valid,float radius){var line=preview.GetComponent<LineRenderer>();line.loop=valid;line.positionCount=valid?40:4;if(valid){for(int i=0;i<40;i++){float a=i*Mathf.PI/20;line.SetPosition(i,new Vector3(Mathf.Cos(a),.25f,Mathf.Sin(a))*nav.Scale*radius);}}else{var p=new[]{new Vector3(-1,.25f,-1),new Vector3(1,.25f,1),new Vector3(-1,.25f,1),new Vector3(1,.25f,-1)};for(int i=0;i<4;i++)line.SetPosition(i,p[i]*nav.Scale*radius*.6f);}}
 void OnDisable(){Cancel();HideDock();}
 void OnDestroy(){DisposeDock();DisposeSculpt();if(buttonStone!=null)Destroy(buttonStone);emblems?.Dispose();if(previewMaterial!=null)Destroy(previewMaterial);}
}
}
