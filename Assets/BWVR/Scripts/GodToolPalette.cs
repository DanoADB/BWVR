using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
namespace BWVR {
// A private, bounded tool context. No tool preview participates in actor perception.
public sealed class GodToolPalette:MonoBehaviour {
 public const string SpotId="player-guidance-spot";
 public bool Open{get;private set;}public SpiritualTool? Selected{get;private set;}
 public bool PreviewValid{get;private set;}public Vector3 PreviewPoint{get;private set;}
 public string Reason{get;private set;}="";public int Uses{get;private set;}
 public bool ClaimsInput=>Open||Selected.HasValue||releaseGate;
 public GameObject View=>view;public GameObject Sign=>sign;
 DivineIntervention miracles;CreatureHybrid creature;IslandNavigation nav;Collider ground;
 Transform left,right,rig;Camera eye;Font font;GameObject view,wand,preview,sign;HybridTarget spot,probe;
 readonly Collider[] choices=new Collider[7];GameObject[] glyphs=new GameObject[3];
 UnityEngine.UI.Text energy,hint;UnityEngine.UI.Image energyFill;Material previewMaterial;
 bool releaseGate,pressedLast,gestureLatched,gestureOpen;float dwell,previewTimer;int wandKind=-1;
 Vector3 desktopAnchor;Quaternion desktopRotation;
 HybridTarget aimed;Action<bool,string> feedback;
 SliceState Season=>creature.Season;
 public static GodToolPalette Build(Transform parent,Transform left,Transform right,Transform rig,Camera eye,Font font,DivineIntervention miracles,CreatureHybrid creature,IslandNavigation nav,Collider ground,Action<bool,string> feedback){
  var root=new GameObject("Summonable god tools");root.transform.SetParent(parent,false);var p=root.AddComponent<GodToolPalette>();p.left=left;p.right=right;p.rig=rig;p.eye=eye;p.font=font;p.miracles=miracles;p.creature=creature;p.nav=nav;p.ground=ground;p.feedback=feedback;p.Build();return p;
 }
 void Build(){
  view=new GameObject("Underside wrist tool palette",typeof(UnityEngine.Canvas));view.transform.SetParent(transform,false);view.GetComponent<UnityEngine.Canvas>().renderMode=RenderMode.WorldSpace;view.GetComponent<RectTransform>().sizeDelta=new Vector2(510,320);
  var bg=Panel("Palette backing",view.transform,new Vector2(510,320),Vector2.zero,new Color(.025f,.075f,.085f,.97f));bg.raycastTarget=false;
  Text("Title",view.transform,new Vector2(0,134),new Vector2(460,35),24).text="GOD TOOLS";
  for(int i=0;i<5;i++){int column=i<3?i:i-3;float x=i<3?(column-1)*158:(column-.5f)*158;float y=i<3?58:-44;var b=Panel(((SpiritualTool)i).ToString(),view.transform,new Vector2(146,94),new Vector2(x,y),new Color(.12f,.25f,.25f));var hit=b.gameObject.AddComponent<BoxCollider>();hit.size=new Vector3(146,94,12);choices[i]=hit;
   var symbol=Symbol((SpiritualTool)i,b.transform);symbol.transform.localPosition=new Vector3(0,7,-16);symbol.transform.localScale=Vector3.one*18;
   Text("Tool caption",b.transform,new Vector2(0,-32),new Vector2(140,24),17).text=((SpiritualTool)i).ToString()+(i==3?" · "+miracles.tuning.rainCost:i==4?" · "+miracles.tuning.healingCost:"");
  }
  for(int i=5;i<7;i++){var b=Panel(i==5?"Clear sign":"Dismiss tools",view.transform,new Vector2(146,32),new Vector2((i==5?-1:1)*158,-127),new Color(.12f,.25f,.25f));var hit=b.gameObject.AddComponent<BoxCollider>();hit.size=new Vector3(146,32,12);choices[i]=hit;Text("Caption",b.transform,Vector2.zero,new Vector2(140,30),16).text=i==5?"Clear sign":"Close";}
  energy=Text("Spiritual energy",view.transform,new Vector2(0,-109),new Vector2(155,25),16);
  Panel("Energy track",view.transform,new Vector2(140,6),new Vector2(0,-133),new Color(.2f,.3f,.3f));energyFill=Panel("Energy remaining",view.transform,new Vector2(140,6),new Vector2(0,-133),new Color(.5f,.85f,.75f));energyFill.rectTransform.pivot=new Vector2(0,.5f);energyFill.rectTransform.anchoredPosition=new Vector2(-70,-133);
  var card=new GameObject("Selected tool hint",typeof(UnityEngine.Canvas));card.transform.SetParent(transform,false);card.GetComponent<UnityEngine.Canvas>().renderMode=RenderMode.WorldSpace;hint=Text("Use hint",card.transform,Vector2.zero,new Vector2(470,60),20);hint.transform.parent.localScale=Vector3.one*.00065f;hint.gameObject.SetActive(false);
  spot=HybridTarget.Bind(new GameObject("Saved guidance point"),SpotId,HybridTargetKind.Object);spot.transform.SetParent(transform,false);spot.transform.position=creature.transform.position;creature.Register(spot);
  probe=HybridTarget.Bind(new GameObject("Private placement probe"),"private-preview",HybridTargetKind.Object);probe.transform.SetParent(transform,false);
  sign=new GameObject("Player guidance sign");sign.transform.SetParent(transform,false);var prefab=Resources.Load<GameObject>("SettlementProps/signpost-single");if(prefab!=null){var art=Instantiate(prefab,sign.transform);art.transform.localPosition=Vector3.zero;art.transform.localScale=Vector3.one*7;foreach(var c in art.GetComponentsInChildren<Collider>())c.enabled=false;}
  for(int i=0;i<3;i++){glyphs[i]=Symbol((SpiritualTool)i,sign.transform);glyphs[i].transform.localPosition=new Vector3(0,5,-.6f);glyphs[i].transform.localScale=Vector3.one*1.6f;}
  sign.transform.localScale=Vector3.one*nav.Scale;sign.SetActive(false);
  previewMaterial=new Material(Shader.Find("BWVR/GameSurface")){color=new Color(.4f,1,.8f)};
  preview=new GameObject("Tool target ring");preview.transform.SetParent(transform,false);var ring=preview.AddComponent<LineRenderer>();ring.sharedMaterial=previewMaterial;ring.useWorldSpace=false;ring.loop=true;ring.positionCount=40;ring.widthMultiplier=nav.Scale*.35f;ring.shadowCastingMode=ShadowCastingMode.Off;
  for(int i=0;i<40;i++){float a=i*Mathf.PI/20;ring.SetPosition(i,new Vector3(Mathf.Cos(a),.25f,Mathf.Sin(a))*nav.Scale*5);}
  preview.SetActive(false);view.SetActive(false);RestoreSign();
 }
 UnityEngine.UI.Image Panel(string name,Transform parent,Vector2 size,Vector2 at,Color color){var g=new GameObject(name,typeof(RectTransform),typeof(UnityEngine.UI.Image));g.transform.SetParent(parent,false);var image=g.GetComponent<UnityEngine.UI.Image>();image.color=color;image.rectTransform.sizeDelta=size;image.rectTransform.anchoredPosition=at;return image;}
 UnityEngine.UI.Text Text(string name,Transform parent,Vector2 at,Vector2 size,int pixels){var g=new GameObject(name,typeof(RectTransform),typeof(UnityEngine.UI.Text));g.transform.SetParent(parent,false);var text=g.GetComponent<UnityEngine.UI.Text>();text.font=font;text.fontSize=pixels;text.alignment=TextAnchor.MiddleCenter;text.color=new Color(.96f,.91f,.74f);text.raycastTarget=false;text.rectTransform.sizeDelta=size;text.rectTransform.anchoredPosition=at;return text;}
 // Copy only mesh views: no CarryItem, colliders, scripts or private material copies.
 GameObject Symbol(SpiritualTool kind,Transform parent){var root=new GameObject(kind+" pictogram");root.transform.SetParent(parent,false);var source=miracles.tools.Find(t=>t.kind==kind);if(source==null)return root;foreach(var r in source.GetComponentsInChildren<MeshRenderer>()){var filter=r.GetComponent<MeshFilter>();if(filter==null)continue;var g=new GameObject(r.name,typeof(MeshFilter),typeof(MeshRenderer));g.transform.SetParent(root.transform,false);g.transform.localPosition=source.transform.InverseTransformPoint(r.transform.position);g.transform.localRotation=Quaternion.Inverse(source.transform.rotation)*r.transform.rotation;g.transform.localScale=new Vector3(r.transform.lossyScale.x/source.transform.lossyScale.x,r.transform.lossyScale.y/source.transform.lossyScale.y,r.transform.lossyScale.z/source.transform.lossyScale.z);g.GetComponent<MeshFilter>().sharedMesh=filter.sharedMesh;var clone=g.GetComponent<MeshRenderer>();clone.sharedMaterials=r.sharedMaterials;clone.shadowCastingMode=ShadowCastingMode.Off;clone.receiveShadows=false;}return root;}
 public void Toggle(){if(Open){Cancel();return;}if(Season.paused||miracles.Suspended)return;Selected=null;if(wand!=null)wand.SetActive(false);preview.SetActive(false);hint.gameObject.SetActive(false);Open=true;releaseGate=true;gestureOpen=false;desktopAnchor=rig.InverseTransformPoint(eye.transform.position+eye.transform.forward*.55f*rig.localScale.x);desktopRotation=Quaternion.Inverse(rig.rotation)*eye.transform.rotation;view.SetActive(true);}
 public void Select(SpiritualTool tool){if(Season.paused||miracles.Suspended||!Enum.IsDefined(typeof(SpiritualTool),tool))return;Selected=tool;Open=false;view.SetActive(false);releaseGate=true;previewTimer=0;ShowWand(tool);}
 void ShowWand(SpiritualTool tool){if(wandKind==(int)tool&&wand!=null){wand.SetActive(true);return;}if(wand!=null)Destroy(wand);wand=Symbol(tool,transform);wandKind=(int)tool;}
 public void Cancel(){Open=false;Selected=null;releaseGate=true;dwell=0;gestureOpen=false;if(view!=null)view.SetActive(false);if(wand!=null)wand.SetActive(false);if(preview!=null)preview.SetActive(false);if(hint!=null)hint.gameObject.SetActive(false);PreviewValid=false;}
 public void ClearSign(){if(Season.Hybrid.guidance.playerSign)creature.CancelGuidance("Sign removed by the player");sign.SetActive(false);}
 public void RestoreSign(){var g=Season.Hybrid.guidance;if(!g.playerSign||g.status!=GuidanceStatus.Active)return;if(g.target==SpotId){if(nav.Terrain!=null&&!nav.Terrain.Dry(g.spot)||!creature.CanReach(g.spot)){creature.CancelGuidance("Saved sign is unreachable");return;}spot.transform.position=g.spot;}RefreshSign();}
 void RefreshSign(){var g=Season.Hybrid.guidance;bool visible=g.playerSign&&g.status==GuidanceStatus.Active&&Season.Hybrid.time<g.expiresAt;sign.SetActive(visible);if(!visible)return;sign.transform.position=g.spot;var forward=Vector3.ProjectOnPlane(eye.transform.position-sign.transform.position,Vector3.up);if(forward.sqrMagnitude>1e-8f)sign.transform.rotation=Quaternion.LookRotation(-forward);for(int i=0;i<3;i++)glyphs[i].SetActive(i==(int)g.intention);}
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
  if(Open){bool pointing=Physics.Raycast(ray,out var hit,8*scale);int hovered=pointing?Array.IndexOf(choices,hit.collider):-1;energy.text=$"Energy {Season.Hybrid.energy:0}";energyFill.rectTransform.sizeDelta=new Vector2(140*Mathf.Clamp01(Season.Hybrid.energy/Mathf.Max(1,miracles.tuning.maximum)),6);for(int i=0;i<5;i++){var kind=(SpiritualTool)i;float cost=kind==SpiritualTool.Rain?miracles.tuning.rainCost:kind==SpiritualTool.Heal?miracles.tuning.healingCost:0;choices[i].GetComponent<UnityEngine.UI.Image>().color=i==hovered?new Color(.25f,.45f,.4f):Season.Hybrid.energy>=cost?new Color(.12f,.25f,.25f):new Color(.13f,.14f,.14f);}
   if(edge){int index=hovered;if(index<5&&index>=0)Select((SpiritualTool)index);else if(index==5){ClearSign();releaseGate=true;}else if(index==6)Cancel();}return true;
  }
  if(!Selected.HasValue)return owned;
  wand.transform.SetPositionAndRotation(right.position+right.forward*.16f*scale,right.rotation);wand.transform.localScale=Vector3.one*.022f*scale;
  hint.gameObject.SetActive(true);hint.transform.parent.SetPositionAndRotation(right.position+right.forward*.23f*scale-right.up*.07f*scale,Quaternion.LookRotation(right.position-eye.transform.position));hint.transform.parent.localScale=Vector3.one*.00065f*scale;
  previewTimer-=dt;if(previewTimer<=0||edge){previewTimer=.1f;Aim(ray);}
  hint.text=Selected+" · "+(PreviewValid?"Trigger to use":Reason);if(edge){bool accepted=PreviewValid&&Apply(Selected.Value,aimed,PreviewPoint);releaseGate=true;if(!accepted)feedback?.Invoke(false,Reason);}
  return true;
 }
 void Aim(Ray ray){aimed=null;PreviewPoint=ray.GetPoint(2*rig.localScale.x);bool groundHit=false;
  foreach(var hit in Physics.RaycastAll(ray,8*rig.localScale.x).OrderBy(h=>h.distance)){if(hit.collider.GetComponentInParent<GodToolPalette>()==this||hit.collider.GetComponentInParent<DivineTool>()!=null)continue;PreviewPoint=hit.point;aimed=hit.collider.GetComponentInParent<HybridTarget>();groundHit=hit.collider==ground;break;}
  if(aimed==null&&Selected==SpiritualTool.Investigate&&groundHit&&(nav.Terrain==null||nav.Terrain.Dry(PreviewPoint))){probe.transform.position=PreviewPoint;aimed=probe;}
  PreviewValid=miracles.CanUse(Selected.Value,aimed,out var reason);Reason=reason;preview.SetActive(true);preview.transform.position=aimed?.villager!=null?aimed.transform.position:PreviewPoint;previewMaterial.color=PreviewValid?new Color(.4f,1,.8f):new Color(1,.25f,.18f);
 }
 public bool Apply(SpiritualTool tool,HybridTarget target,Vector3 point){
  if(!HybridState.Number(point.x,-10000,10000)||!HybridState.Number(point.y,-10000,10000)||!HybridState.Number(point.z,-10000,10000)){Reason="Invalid placement";return false;}if(!miracles.CanUse(tool,target,out var reason)){Reason=reason;return false;}
  if(target==probe){spot.transform.position=point;target=spot;}
  if(!miracles.TryUse(tool,target)){Reason=miracles.LastReason;return false;}
  if(tool<=SpiritualTool.Investigate){var g=Season.Hybrid.guidance;g.playerSign=true;g.spot=nav.Terrain!=null?nav.Terrain.Project(point):new Vector3(point.x,.82f,point.z);RefreshSign();}
  Uses++;Reason="";feedback?.Invoke(true,tool<=SpiritualTool.Investigate?"Sign placed. The creature chooses its response.":tool+" cast.");return true;
 }
 void OnDestroy(){if(previewMaterial!=null)Destroy(previewMaterial);}
}
}
