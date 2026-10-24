using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.InputSystem;
using CommonUsages=UnityEngine.XR.CommonUsages;
namespace BWVR {
public sealed class QuestSlice:MonoBehaviour {
 public const float ActorScale=1.1f;
 IslandNavigation navigation; IslandTerrainData terrain; Collider groundCollider; const float Floor=.82f; SliceState state=new SliceState(); Camera eye; Transform rig,left,right,creature; Text status,creatureLabel; GameObject panel; Font font; GameArtCatalog art; OriginalCowMotion cowMotion; HumanoidMotion godMotion; HumanoidMotion[] humanoids=Array.Empty<HumanoidMotion>();
 readonly Transform[] aims=new Transform[2]; readonly QuestControls.Hand[] controllerHands=new QuestControls.Hand[2]; bool turnHeld,recenterLast,zoomHeld; Vector3 zoomFocus;
 readonly List<CarryItem> items=new List<CarryItem>(); readonly List<Transform> workers=new List<Transform>();
 readonly CarryItem[] held=new CarryItem[2]; readonly bool[] lastGrip=new bool[2],lastTrigger=new bool[2]; readonly LineRenderer[] rays=new LineRenderer[2];
 DroughtSignals signals;bool helpVisible,outcomeShown;string saveError; CreatureRoutine creatureRoutine; VillagerRoutine[] jobs; float clock,saveClock,creatureTrip; bool returning,menuLast,trackingConfigured,viewPlaced; string notice="Their creature is their god. Your touch guides its instincts.";
 Vector3 cistern=new Vector3(-.32f,Floor,.25f),well=new Vector3(.4f,Floor,-.2f);
 string SavePath=>Path.Combine(Application.persistentDataPath,"bwvr-season-v1.json");
 void Start(){terrain=Resources.Load<IslandTerrainData>("FirstIsland");art=Resources.Load<GameArtCatalog>("BWVRArt/Catalog");font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");Load();Build();humanoids=GetComponentsInChildren<HumanoidMotion>();}
 Material Mat(Color c){var m=new Material(Shader.Find("Universal Render Pipeline/Lit")??Shader.Find("Standard"));m.color=c;return m;}
 GameObject Shape(string name,PrimitiveType shape,Vector3 position,Vector3 scale,Color color,Transform parent=null){var g=GameObject.CreatePrimitive(shape);g.name=name;g.transform.SetParent(parent??transform,false);g.transform.localPosition=position;g.transform.localScale=scale;g.GetComponent<Renderer>().sharedMaterial=Mat(color);return g;}
 IslandPlacement Nearest(string kind,Vector3 from,System.Func<IslandPlacement,bool> filter=null)=>terrain?.placements?.Where(p=>p.kind==kind&&(filter==null||filter(p))).OrderBy(p=>(terrain.PlacementPoint(p)-from).sqrMagnitude).FirstOrDefault();
 Vector3 Landmark(string kind,Vector3 fallback,System.Func<IslandPlacement,bool> filter=null){var p=Nearest(kind,fallback,filter);return p==null?fallback:terrain.PlacementPoint(p);}
 Vector3 Site(Vector3 at){if(terrain==null)return at;return new Vector3(at.x*.1f,at.y,at.z*.1f);}
 // Heights are in original mesh/game units; island import owns the uniform conversion.
 float Size(float previous,float originalUnits)=>terrain==null?previous:originalUnits*terrain.scale;
 void Build(){
  well=Site(well);cistern=Site(cistern);
  RenderSettings.ambientLight=new Color(.56f,.65f,.65f);RenderSettings.fog=true;RenderSettings.fogColor=new Color(.035f,.075f,.085f);RenderSettings.fogDensity=.025f;
  var sun=new GameObject("Warm morning sun").AddComponent<Light>();sun.type=LightType.Directional;sun.intensity=1.1f;sun.transform.rotation=Quaternion.Euler(40,-35,0);
  rig=new GameObject("XR Origin - seated or standing").transform;rig.position=new Vector3(0,0,-1.2f);
  var cam=new GameObject("Head");cam.tag="MainCamera";cam.transform.SetParent(rig,false);eye=cam.AddComponent<Camera>();eye.nearClipPlane=.03f;eye.farClipPlane=40;eye.backgroundColor=new Color(.48f,.70f,.77f);eye.clearFlags=CameraClearFlags.SolidColor;cam.AddComponent<AudioListener>();eye.transform.localPosition=new Vector3(0,1.55f,0);
  left=Controller("Left hand",0);right=Controller("Right hand",1);
  Shape("Quiet horizon",PrimitiveType.Plane,new Vector3(0,-.03f,0),new Vector3(3,1,3),new Color(.035f,.075f,.085f));
  Renderer ground;
  if(terrain!=null){
   var island=new GameObject("Land 1 - original island geometry");island.transform.SetParent(transform,false);island.AddComponent<MeshFilter>().sharedMesh=terrain.mesh;ground=island.AddComponent<MeshRenderer>();groundCollider=island.AddComponent<MeshCollider>();((MeshCollider)groundCollider).sharedMesh=terrain.mesh;
   IslandOcean.Build(transform,terrain);
   if(terrain.HasPlacements){
    cistern=Landmark("building",Vector3.zero,p=>p.town==0&&p.type.Contains("STORAGE_PIT"));
    well=Landmark("well",Landmark("drink",cistern));
   }else{well=terrain.SafePoint(well);cistern=terrain.SafePoint(cistern);}
  }else{
   Shape("Island plinth",PrimitiveType.Cylinder,new Vector3(0,.39f,0),new Vector3(1.9f,.39f,1.9f),new Color(.12f,.2f,.22f));
   ground=Shape("Living island",PrimitiveType.Cylinder,new Vector3(0,.79f,0),new Vector3(1.8f,.03f,1.8f),new Color(.35f,.46f,.25f)).GetComponent<Renderer>();if(art!=null&&art.grass!=null)ground.sharedMaterial=art.grass;
  }
  var environment=Resources.Load<EnvironmentArt>("EnvironmentArt");if(environment!=null)IslandEnvironment.Build(environment,transform,ground,terrain);
  if(environment==null)for(int i=0;i<4;i++){float x=-.5f+i*.28f;if(Original("house"+(i%3+1),new Vector3(x,Floor,.48f),.18f)!=null)continue;Shape("Village dwelling",PrimitiveType.Cube,new Vector3(x,Floor+.065f,.48f),new Vector3(.17f,.13f,.16f),new Color(.74f,.57f,.36f));Shape("Roof",PrimitiveType.Cylinder,new Vector3(x,Floor+.15f,.48f),new Vector3(.22f,.04f,.21f),new Color(.35f,.21f,.13f));}
  if(environment==null)for(int i=0;i<9;i++){float a=i*.55f;Vector3 at=new Vector3(Mathf.Cos(a)*.72f,Floor,Mathf.Sin(a)*.72f);if(Original(new[]{"beech","oak","pine"}[i%3],at,.28f)!=null)continue;Shape("Tree trunk",PrimitiveType.Cylinder,at+Vector3.up*.07f,new Vector3(.025f,.07f,.025f),new Color(.3f,.2f,.12f));Shape("Tree crown",PrimitiveType.Sphere,at+Vector3.up*.18f,new Vector3(.13f,.2f,.13f),new Color(.12f,.3f,.2f));}
  var reservoir=WaterVessel.Create(transform,"Village cistern",Size(.07f,2),Size(.09f,6),new Color(.24f,.25f,.22f));reservoir.transform.position=cistern;reservoir.SetFill(state.water/100);
  if(environment!=null&&terrain!=null&&terrain.HasPlacements)IslandEnvironment.WaterSource(environment,transform,well,terrain.scale);
  Shape("Spring",PrimitiveType.Cylinder,well+Vector3.up*Size(.015f,.4f),new Vector3(Size(.14f,6),Size(.015f,.4f),Size(.14f,6)),new Color(.28f,.68f,.8f));
  navigation=gameObject.AddComponent<IslandNavigation>();navigation.Initialize(terrain,well);
  VillageSettlement.Build(transform,terrain,navigation,cistern,well);
  jobs=new VillagerRoutine[3];for(int i=0;i<3;i++){var spawn=Site(new Vector3(-.2f+i*.14f,Floor,.13f));if(terrain!=null&&terrain.HasPlacements){var starts=terrain.placements.Where(p=>p.kind=="villager").GroupBy(p=>new Vector2(p.x,p.z)).Take(3).Select(g=>g.First()).ToArray();if(i<starts.Length)spawn=terrain.PlacementPoint(starts[i]);}var w=Item("Villager",Offering.Villager,spawn,PrimitiveType.Capsule,new Color(.9f,.65f,.38f),.055f);var walker=w.gameObject.AddComponent<IslandNavigator>();walker.Initialize(navigation,false);w.GetComponent<CarryItem>().home=w.transform.position;workers.Add(w.transform);}
  var waterOffering=Item("Water offering",Offering.Water,well+Vector3.up*Size(.07f,2),PrimitiveType.Sphere,new Color(.25f,.8f,.95f),Size(.065f,1.2f));
  Item("Food offering",Offering.Food,Ground(Landmark("field",Site(new Vector3(.18f,Floor,.35f)),p=>p.town==0))+Vector3.up*Size(.04f,2),PrimitiveType.Sphere,new Color(.95f,.66f,.18f),Size(.065f,1.2f));
  Item("Wood offering",Offering.Wood,Ground(Landmark("tree",Site(new Vector3(-.5f,Floor,-.22f))))+Vector3.up*Size(.04f,2),PrimitiveType.Cube,new Color(.5f,.3f,.16f),Size(.07f,1.2f));
  creature=new GameObject("The village's god").transform;creature.SetParent(transform,false);creature.localScale=Vector3.one*ActorScale;creature.position=Ground(Site(new Vector3(0,Floor,-.12f)));
  var god=Resources.Load<GameObject>("BWVRGod/Puglin");if(god!=null){var instance=Instantiate(god,creature);instance.transform.localPosition=Vector3.zero;instance.transform.localScale=Vector3.one*Size(.34f,13.9236f);Destroy(instance.GetComponent<Collider>());godMotion=instance.GetComponent<HumanoidMotion>();}
  if(god==null&&Original("cow",Vector3.zero,Size(.34f,13.9236f),creature)==null){
  Shape("Creature body",PrimitiveType.Capsule,new Vector3(0,.13f,0),new Vector3(.15f,.17f,.14f),Color.white,creature);
  Shape("Creature head",PrimitiveType.Sphere,new Vector3(0,.27f,-.015f),new Vector3(.13f,.12f,.11f),new Color(.9f,.85f,.73f),creature);
  for(int i=-1;i<=1;i+=2)Shape("Creature horn",PrimitiveType.Capsule,new Vector3(i*.065f,.32f,-.015f),new Vector3(.025f,.04f,.025f),new Color(.35f,.3f,.25f),creature);
  }
  if(god==null&&creature.GetComponentInChildren<OriginalCowMotion>()==null&&terrain!=null)foreach(Transform visual in creature){visual.localPosition*=Size(.34f,13.9236f)/.34f;visual.localScale*=Size(.34f,13.9236f)/.34f;}
  creature.gameObject.AddComponent<IslandNavigator>().Initialize(navigation,true);
  cowMotion=creature.GetComponentInChildren<OriginalCowMotion>();
  creatureRoutine=creature.gameObject.AddComponent<CreatureRoutine>();creatureRoutine.speed=(.07f+state.care*.08f)*(terrain!=null?.1f:1);creatureRoutine.Initialize(godMotion,waterOffering,well,cistern,()=>{DeliverWater();notice="The creature delivered water to the village.";},Ground);
  for(int i=0;i<workers.Count;i++){jobs[i]=workers[i].gameObject.AddComponent<VillagerRoutine>();jobs[i].Initialize((VillagerRoutine.Role)i,i==0?well:i==1?Landmark("field",Site(new Vector3(.2f,Floor,.26f)),p=>p.town==0):Landmark("tree",Site(new Vector3(-.64f,Floor,-.12f)),p=>p.variant<13),cistern,Ground,role=>{if(role==VillagerRoutine.Role.WaterGatherer)state.water=Mathf.Min(100,state.water+.7f);else if(role==VillagerRoutine.Role.Farmer)state.crops=Mathf.Min(100,state.crops+1.2f);notice=role+" finished a delivery.";});}
  signals=gameObject.AddComponent<DroughtSignals>();signals.Initialize(reservoir,ground.sharedMaterial,jobs,creatureRoutine);
  Debug.Log("BWVR_ART_ACTIVE: environment="+(environment!=null)+", Puglin="+(god!=null)+", original fallback="+(art!=null));
  var target=creature.gameObject.AddComponent<SpatialTarget>();target.invoke=()=>{state.Praise();if(godMotion!=null)godMotion.React();notice="Warmth remembered. The creature is more likely to bring water.";};var box=creature.gameObject.AddComponent<BoxCollider>();box.center=new Vector3(0,Size(.18f,7),0);box.size=new Vector3(Size(.2f,8),Size(.4f,15),Size(.2f,8));
  creatureLabel=Label("Creature intent",creature,new Vector3(0,Size(.44f,17),0),new Vector2(420,70),terrain!=null?.00007f:.0007f,22);creatureLabel.text="THEIR GOD · WATCHING";creatureLabel.gameObject.SetActive(false);
  panel=new GameObject("Wrist sanctuary");panel.transform.SetParent(left,false);panel.transform.localPosition=new Vector3(0,.13f,.12f);panel.transform.localRotation=Quaternion.Euler(25,0,0);
  var canvas=panel.AddComponent<Canvas>();canvas.renderMode=RenderMode.WorldSpace;var rect=panel.GetComponent<RectTransform>();rect.sizeDelta=new Vector2(600,560);panel.transform.localScale=Vector3.one*.0008f;
  var bg=new GameObject("Obsidian panel",typeof(RectTransform),typeof(Image));bg.transform.SetParent(panel.transform,false);bg.GetComponent<RectTransform>().sizeDelta=rect.sizeDelta;bg.GetComponent<Image>().color=new Color(.035f,.095f,.11f,.96f);
  status=Label("Season and guidance",panel.transform,new Vector3(0,130,0),new Vector2(540,260),1,25);status.alignment=TextAnchor.UpperLeft;
  Button("Pause / resume",new Vector3(-140,-70,0),()=>{state.paused=!state.paused;notice=state.paused?"Season paused.":"Season resumed.";});
  Button("Save season",new Vector3(140,-70,0),Save);
  Button("Begin again",new Vector3(-140,-160,0),()=>{state=new SliceState();creatureTrip=0;outcomeShown=false;signals.ResetRequests();creatureRoutine.ResetTask();foreach(var job in jobs)job.ResetTask();notice="A new dry season begins.";});
  Button("Close",new Vector3(140,-160,0),()=>panel.SetActive(false));
  Button("Help / details",new Vector3(0,-240,0),()=>SetHelp(!helpVisible));
  panel.SetActive(false);
  if(!XRSettings.isDeviceActive){eye.transform.localRotation=Quaternion.Euler(25,0,0);left.localPosition=new Vector3(-.6f,1.15f,1.05f);left.localRotation=Quaternion.Euler(0,0,0);panel.transform.localRotation=Quaternion.identity;}
 }
 GameObject Original(string key,Vector3 at,float height,Transform parent=null){
  var prefab=art!=null?art.Find(key):null;if(prefab==null)return null;
  var instance=Instantiate(prefab,parent??transform);instance.name="Game art - "+key;instance.transform.localPosition=at;instance.transform.localScale=Vector3.one*height;
  if(key=="cow"){var collider=instance.GetComponent<Collider>();if(collider!=null)Destroy(collider);}
  return instance;
 }
 Transform Controller(string name,int side){var g=new GameObject(name);g.transform.SetParent(rig,false);aims[side]=new GameObject(name+" aim").transform;aims[side].SetParent(rig,false);Shape("Controller presence",PrimitiveType.Sphere,Vector3.zero,Vector3.one*.035f,new Color(.85f,.72f,.42f),g.transform);var line=g.AddComponent<LineRenderer>();line.positionCount=2;line.startWidth=.002f;line.endWidth=.001f;line.material=new Material(Shader.Find("Sprites/Default"));line.startColor=line.endColor=new Color(.6f,.85f,.8f,.7f);rays[side]=line;return g.transform;}
 CarryItem Item(string name,Offering kind,Vector3 at,PrimitiveType primitive,Color color,float size){var modern=kind==Offering.Villager?Resources.Load<GameObject>("Villagers/Villager"+(items.Count%3)):null;GameObject fitted=null;if(modern!=null){fitted=Instantiate(modern,transform);fitted.transform.position=Ground(at);fitted.transform.localScale=Vector3.one*Size(.085f,1.661f);}var original=fitted??(kind==Offering.Villager?Original(items.Count%2==0?"woman":"man",Ground(at),Size(.07f,1.661f)):null);var g=original??Shape(name,primitive,at,Vector3.one*size,color);if(kind==Offering.Villager)g.transform.localScale*=ActorScale;var item=g.AddComponent<CarryItem>();item.kind=kind;item.home=g.transform.position;items.Add(item);return item;}
 Text Label(string name,Transform parent,Vector3 position,Vector2 size,float scale,int fontSize){var g=new GameObject(name,typeof(RectTransform));g.transform.SetParent(parent,false);g.transform.localPosition=position;g.transform.localScale=Vector3.one*scale;if(!parent.GetComponentInParent<Canvas>()){var c=g.AddComponent<Canvas>();c.renderMode=RenderMode.WorldSpace;}var t=g.AddComponent<Text>();t.font=font;t.fontSize=fontSize;t.color=new Color(.94f,.9f,.74f);t.alignment=TextAnchor.MiddleCenter;t.horizontalOverflow=HorizontalWrapMode.Wrap;g.GetComponent<RectTransform>().sizeDelta=size;return t;}
 void Button(string title,Vector3 position,Action action){var g=new GameObject(title,typeof(RectTransform),typeof(Image),typeof(BoxCollider),typeof(SpatialTarget));g.transform.SetParent(panel.transform,false);g.transform.localPosition=position;g.GetComponent<RectTransform>().sizeDelta=new Vector2(260,68);g.GetComponent<Image>().color=new Color(.13f,.28f,.29f);g.GetComponent<BoxCollider>().size=new Vector3(260,68,15);g.GetComponent<SpatialTarget>().invoke=action;Label("Caption",g.transform,Vector3.zero,new Vector2(250,65),1,24).text=title;}
 void Update(){if(eye==null)return;foreach(var motion in humanoids)motion.paused=state.paused||state.Finished;if(cowMotion!=null)cowMotion.paused=state.paused||state.Finished;clock+=Time.deltaTime;state.Step(Time.deltaTime);bool xr=XRSettings.isDeviceActive;
  if(xr){if(!trackingConfigured){var inputs=new List<XRInputSubsystem>();SubsystemManager.GetSubsystems(inputs);bool floor=false;foreach(var input in inputs)floor|=input.TrySetTrackingOriginMode(TrackingOriginModeFlags.Floor);if(!floor)rig.position+=Vector3.up*1.55f;trackingConfigured=true;}Pose(XRNode.Head,eye.transform);UpdateController(0,left);UpdateController(1,right);if(!viewPlaced){var head=InputDevices.GetDeviceAtXRNode(XRNode.Head);if(head.TryGetFeatureValue(CommonUsages.isTracked,out bool tracked)&&tracked)RecenterView();}
   bool zoom=controllerHands[1].zoom;
   if(zoom&&!zoomHeld)zoomFocus=ZoomFocus();
   if(zoom&&held[0]==null&&held[1]==null){float value=controllerHands[1].stick.y;if(Mathf.Abs(value)>.2f)QuestControls.Zoom(rig,zoomFocus,value*Time.deltaTime);}
   zoomHeld=zoom;
   QuestControls.Move(rig,eye.transform,controllerHands[0].stick,zoom?Vector2.zero:controllerHands[1].stick,Time.deltaTime,ref turnHeld);
   UpdateViewScale();
   if(controllerHands[0].recenter&&!recenterLast)RecenterView();recenterLast=controllerHands[0].recenter;
   if(terrain!=null){float minimum=terrain.Project(eye.transform.position).y+.2f*rig.localScale.x;float y=Mathf.Clamp(eye.transform.position.y,minimum,Floor+3*rig.localScale.x);rig.position+=Vector3.up*(y-eye.transform.position.y);}
  }
  else Desktop();
  for(int i=0;i<2;i++){bool grip=xr&&i==1?controllerHands[i].trigger:controllerHands[i].grip,trigger=controllerHands[i].trigger;if(!xr){grip=i==1&&Keyboard.current!=null&&Keyboard.current.eKey.isPressed;trigger=i==1&&Keyboard.current!=null&&Keyboard.current.fKey.isPressed;}
   var hand=i==0?left:right;var pointer=xr?aims[i]:hand;var ray=new Ray(pointer.position,pointer.forward);Physics.Raycast(ray,out var hit,8*rig.localScale.x);rays[i].SetPosition(0,ray.origin);rays[i].SetPosition(1,hit.collider?hit.point:ray.GetPoint(2*rig.localScale.x));rays[i].enabled=xr?controllerHands[i].tracked:i==1;
   if(grip&&!lastGrip[i]&&hit.collider){var item=hit.collider.GetComponentInParent<CarryItem>();if(item!=null&&!item.held){held[i]=item;item.held=true;item.GetComponent<Collider>().enabled=false;notice="Release the carry button over the island to place. Water belongs in the blue cistern.";}}
   if(held[i]!=null){if(xr&&!controllerHands[i].tracked){var item=held[i];held[i]=null;item.held=false;item.transform.position=item.home;item.GetComponent<Collider>().enabled=true;}else{held[i].transform.position=hand.position+hand.forward*(.16f*rig.localScale.x);if(!grip)Release(i,ray);}}
   // Right side grip is reserved for zoom; the front trigger carries or interacts.
   bool interact=trigger&&!lastTrigger[i]&&held[i]==null;
   if(interact&&hit.collider)hit.collider.GetComponentInParent<SpatialTarget>()?.Activate();lastGrip[i]=grip;lastTrigger[i]=trigger;
  }
  bool menu=controllerHands[0].menu;if(!xr)menu=Keyboard.current!=null&&Keyboard.current.tabKey.isPressed;if(menu&&!menuLast)panel.SetActive(!panel.activeSelf);menuLast=menu;
  if(!state.paused&&!state.Finished)AnimateVillage();
  signals.Tick(Time.deltaTime,state);if(state.Finished&&!outcomeShown){outcomeShown=true;panel.SetActive(true);}
  status.text=helpVisible?"BWVR / THE LONG DRY\n"+Mathf.CeilToInt(state.seconds)+"s · Water "+Mathf.RoundToInt(state.water)+" · Crops "+Mathf.RoundToInt(state.crops)+"\nTrust "+Mathf.RoundToInt(state.trust)+" · Gifts "+state.gifts+"\n\n"+(state.Finished?(state.Won?"The village survives.":"The village needs a different lesson."):notice)+"\n\nGrip: carry · Trigger: warmth / select\nLeft menu: sanctuary":("THE LONG DRY\n"+(state.Finished?(state.Won?"Village safe":"Village struggling"):state.paused?"Paused":"")+(saveError!=null?"\n"+saveError:""));
  if(helpVisible&&!xr)status.text+="\nDesktop: RMB look · WASD move · E carry · F select · Tab panel";
  creatureLabel.transform.rotation=Quaternion.LookRotation(creatureLabel.transform.position-eye.transform.position);
  saveClock+=Time.deltaTime;if(saveClock>=15){saveClock=0;Save();}
 }
 // Align the virtual table once to the actual tracked head, including room offsets.
 // Keep headset pitch/roll untouched and never pull the player back as they move.
 public void RecenterView(){
  if(eye==null||rig==null)return;
  var focus=terrain!=null?terrain.Project(Vector3.zero):new Vector3(0,Floor,0);
  PlaceView(rig,eye.transform,focus);UpdateViewScale();
  viewPlaced=true;
 }
 public static void PlaceView(Transform origin,Transform head,Vector3 focus){
  origin.localScale=Vector3.one;
  origin.rotation=Quaternion.Euler(0,-head.localEulerAngles.y,0);
  var desiredHead=focus+new Vector3(0,.55f,-.85f);
  origin.position=desiredHead-origin.rotation*head.localPosition;
 }
 Vector3 ZoomFocus(){
  var ray=new Ray(eye.transform.position,eye.transform.forward);
  if(groundCollider!=null&&groundCollider.Raycast(ray,out var hit,8*rig.localScale.x))return hit.point;
  return terrain!=null?terrain.Project(Vector3.zero):new Vector3(0,Floor,0);
 }
 void UpdateViewScale(){
  float scale=rig.localScale.x;eye.nearClipPlane=.03f*scale;
  foreach(var line in rays){line.startWidth=.002f*scale;line.endWidth=.001f*scale;}
 }
 void UpdateController(int side,Transform hand){
  controllerHands[side]=QuestControls.Read(side);
  var input=controllerHands[side];if(!input.tracked)return;
  hand.SetLocalPositionAndRotation(input.position,input.rotation);
  aims[side].SetLocalPositionAndRotation(input.aimPosition,input.aimRotation);
 }
 void Pose(XRNode node,Transform target){var d=InputDevices.GetDeviceAtXRNode(node);if(d.TryGetFeatureValue(CommonUsages.devicePosition,out Vector3 p))target.localPosition=p;if(d.TryGetFeatureValue(CommonUsages.deviceRotation,out Quaternion r))target.localRotation=r;}
 void Desktop(){var k=Keyboard.current;var m=Mouse.current;if(k==null||m==null)return;if(m.rightButton.isPressed){var delta=m.delta.ReadValue()*.12f;var e=eye.transform.localEulerAngles;float pitch=Mathf.DeltaAngle(0,e.x);eye.transform.localRotation=Quaternion.Euler(Mathf.Clamp(pitch-delta.y,-65,65),e.y+delta.x,0);}var movement=new Vector3((k.dKey.isPressed?1:0)-(k.aKey.isPressed?1:0),0,(k.wKey.isPressed?1:0)-(k.sKey.isPressed?1:0));rig.position+=Quaternion.Euler(0,eye.transform.eulerAngles.y,0)*movement*Time.deltaTime*(.6f*rig.localScale.x);
  if(held[0]==null&&held[1]==null&&Mathf.Abs(m.scroll.ReadValue().y)>.01f){QuestControls.Zoom(rig,ZoomFocus(),m.scroll.ReadValue().y*.0015f);UpdateViewScale();}right.position=eye.transform.position-eye.transform.up*.08f;right.rotation=eye.transform.rotation;}
 void Release(int i,Ray ray){var item=held[i];held[i]=null;item.held=false;item.GetComponent<Collider>().enabled=true;Vector3 point;
  if(terrain!=null){if(!groundCollider.Raycast(ray,out var hit,8*rig.localScale.x)||!terrain.Dry(hit.point)){item.transform.position=item.home;notice="Returned safely: place on dry, gentle ground.";return;}point=hit.point;}
  else{var plane=new Plane(Vector3.up,new Vector3(0,Floor,0));if(!plane.Raycast(ray,out float distance)||distance>8*rig.localScale.x){item.transform.position=item.home;return;}point=ray.GetPoint(distance);if(new Vector2(point.x,point.z).magnitude>.82f){item.transform.position=item.home;notice="Returned safely to the island.";return;}}
  if(item.kind==Offering.Villager){var walker=item.GetComponent<IslandNavigator>();if(walker!=null&&!walker.CanPlace(point,out point)){item.transform.position=item.home;notice="Returned safely: villagers need a clear walking space.";return;}walker?.ResetRoute();}
  item.transform.position=point+Vector3.up*(item.kind==Offering.Villager?0:Size(.04f,2));
  if(item.kind==Offering.Water&&Vector3.Distance(point,cistern)<Size(.18f,8)){DeliverWater();notice="Water delivered. Villagers celebrate their creature's gift.";item.transform.position=item.home;}
  else if(item.kind==Offering.Food&&Vector3.Distance(point,creature.position)<Size(.2f,8)){state.Praise();notice="The creature accepts your offering.";item.transform.position=item.home;}
  else {if(item.kind==Offering.Villager)item.home=item.transform.position;notice="Placed on the island.";}
 }
 Vector3 Ground(Vector3 at){if(terrain!=null)return terrain.Project(at);at.y=Floor;return at;}
 void AnimateVillage(){foreach(var job in jobs)job.Tick(Time.deltaTime,false);creatureRoutine.speed=(.07f+state.care*.08f)*(terrain!=null?.1f:1);creatureRoutine.Tick(Time.deltaTime,false);creatureLabel.text="THEIR GOD · "+(creatureRoutine.WaitingForRoute?"WAITING FOR A SAFE ROUTE":creatureRoutine.Current.ToString().ToUpperInvariant());}
 public bool HelpVisible=>helpVisible;
 public void SetHelp(bool enabled){helpVisible=enabled;if(creatureLabel!=null)creatureLabel.gameObject.SetActive(enabled);}
 void DeliverWater(){int before=state.gifts;state.Deliver();if(state.gifts>before)signals?.WaterArrived();}
 void Save(){try{File.WriteAllText(SavePath+".tmp",JsonUtility.ToJson(state,true));File.Copy(SavePath+".tmp",SavePath,true);saveError=null;notice="Season saved on this device.";}catch(Exception e){saveError="Save failed. Please try again.";notice="Could not save: "+e.Message;if(panel!=null)panel.SetActive(true);Debug.LogError("BWVR_SAVE_FAILED: "+e.Message);}}
 void Load(){if(!File.Exists(SavePath))return;try{var s=JsonUtility.FromJson<SliceState>(File.ReadAllText(SavePath));if(s==null||!s.Valid())throw new Exception("Invalid season data");state=s;state.paused=false;notice="Season restored.";}catch(Exception){saveError="Saved season unavailable. A fresh season is ready.";notice=saveError;}}
 void OnApplicationPause(bool paused){if(paused){for(int i=0;i<2;i++)if(held[i]!=null){held[i].transform.position=held[i].home;held[i].held=false;held[i].GetComponent<Collider>().enabled=true;held[i]=null;}Save();}}
 void OnApplicationQuit(){Save();}
}
}
