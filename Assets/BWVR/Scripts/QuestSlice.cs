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
 public const float VillagerScale=ActorScale*1.15f;
 public const float BuildingScale=ActorScale*1.15f;
 [Tooltip("Pause drought pressure and keep water assistance in the background while developing interactions.")]
 public bool developmentFocus=true;
 BeachBall[] balls=Array.Empty<BeachBall>();
 VillageTributes tributes;
 IslandNavigation navigation; IslandTerrainData terrain; Collider groundCollider; const float Floor=.82f; SliceState state=new SliceState(); Camera eye; Transform rig,left,right,creature; Text status,creatureLabel; GameObject panel; Font font; GameArtCatalog art; OriginalCowMotion cowMotion; HumanoidMotion godMotion; HumanoidMotion[] humanoids=Array.Empty<HumanoidMotion>();
 readonly Transform[] aims=new Transform[2]; readonly QuestControls.Hand[] controllerHands=new QuestControls.Hand[2]; bool turnHeld,recenterLast,zoomHeld; Vector3 zoomFocus;
 readonly List<CarryItem> items=new List<CarryItem>(); readonly List<Transform> workers=new List<Transform>();
 readonly ThrowMotion[] throwMotion={new ThrowMotion(),new ThrowMotion()};
 readonly PickupHand[] pickupHands=new PickupHand[2];
 readonly CarryItem[] held=new CarryItem[2]; readonly bool[] lastGrip=new bool[2],lastTrigger=new bool[2]; readonly LineRenderer[] rays=new LineRenderer[2];
 DroughtSignals signals;bool helpVisible,outcomeShown;string saveError; CreatureRoutine creatureRoutine; CreatureCuriosity curiosity; ConstructionSite construction; IslandDiscoveries discoveries; CreatureHybrid hybrid;DivineIntervention miracles;CreatureConstructionLearning learning;bool applicationSuspended; VillagerRoutine[] jobs; float clock,saveClock,creatureTrip; bool returning,menuLast,trackingConfigured,viewPlaced; string notice="Their creature is their god. Your touch guides its instincts.";
 Vector3 cistern=new Vector3(-.32f,Floor,.25f),well=new Vector3(.4f,Floor,-.2f);
 bool praiseLast,discourageLast;float feedbackReady;
 string SavePath=>Path.Combine(Application.persistentDataPath,"bwvr-season-v1.json");
 void Start(){terrain=Resources.Load<IslandTerrainData>("FirstIsland");art=Resources.Load<GameArtCatalog>("BWVRArt/Catalog");font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");Load();state.developmentFocus=developmentFocus;Build();humanoids=GetComponentsInChildren<HumanoidMotion>();}
 Material Mat(Color c){var m=new Material(Shader.Find("Universal Render Pipeline/Lit")??Shader.Find("Standard"));m.color=c;return m;}
 GameObject Shape(string name,PrimitiveType shape,Vector3 position,Vector3 scale,Color color,Transform parent=null){var g=GameObject.CreatePrimitive(shape);g.name=name;g.transform.SetParent(parent??transform,false);g.transform.localPosition=position;g.transform.localScale=scale;g.GetComponent<Renderer>().sharedMaterial=Mat(color);return g;}
 IslandPlacement Nearest(string kind,Vector3 from,System.Func<IslandPlacement,bool> filter=null)=>terrain?.placements?.Where(p=>p.kind==kind&&(filter==null||filter(p))).OrderBy(p=>(terrain.PlacementPoint(p)-from).sqrMagnitude).FirstOrDefault();
 Vector3 Landmark(string kind,Vector3 fallback,System.Func<IslandPlacement,bool> filter=null){var p=Nearest(kind,fallback,filter);return p==null?fallback:terrain.PlacementPoint(p);}
 Vector3 Site(Vector3 at){if(terrain==null)return at;return new Vector3(at.x*.1f,at.y,at.z*.1f);}
 // Heights are in original mesh/game units; island import owns the uniform conversion.
 float Size(float previous,float originalUnits)=>terrain==null?previous:originalUnits*terrain.scale;
 void Build(){
  well=Site(well);cistern=Site(cistern);
  RenderSettings.ambientMode=UnityEngine.Rendering.AmbientMode.Trilight;RenderSettings.ambientSkyColor=new Color(.58f,.69f,.79f);RenderSettings.ambientEquatorColor=new Color(.43f,.48f,.43f);RenderSettings.ambientGroundColor=new Color(.25f,.27f,.21f);RenderSettings.fog=true;RenderSettings.fogMode=FogMode.ExponentialSquared;RenderSettings.fogColor=new Color(.54f,.68f,.73f);RenderSettings.fogDensity=.065f;
  var sun=new GameObject("Warm morning sun").AddComponent<Light>();sun.type=LightType.Directional;sun.color=new Color(1,.91f,.77f);sun.intensity=1.05f;sun.transform.rotation=Quaternion.Euler(44,-35,0);
  rig=new GameObject("XR Origin - seated or standing").transform;rig.position=new Vector3(0,0,-1.2f);
  var cam=new GameObject("Head");cam.tag="MainCamera";cam.transform.SetParent(rig,false);eye=cam.AddComponent<Camera>();eye.nearClipPlane=.03f;eye.farClipPlane=40;eye.backgroundColor=RenderSettings.fogColor;eye.clearFlags=CameraClearFlags.SolidColor;cam.AddComponent<AudioListener>();eye.transform.localPosition=new Vector3(0,1.55f,0);
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
  var god=Resources.Load<GameObject>("BWVRGod/Gorilla")??Resources.Load<GameObject>("BWVRGod/Puglin");if(god!=null){var instance=Instantiate(god,creature);instance.transform.localPosition=Vector3.zero;instance.transform.localScale=Vector3.one*Size(.34f,13.9236f);Destroy(instance.GetComponent<Collider>());godMotion=instance.GetComponent<HumanoidMotion>();}
  if(god==null&&Original("cow",Vector3.zero,Size(.34f,13.9236f),creature)==null){
  Shape("Creature body",PrimitiveType.Capsule,new Vector3(0,.13f,0),new Vector3(.15f,.17f,.14f),Color.white,creature);
  Shape("Creature head",PrimitiveType.Sphere,new Vector3(0,.27f,-.015f),new Vector3(.13f,.12f,.11f),new Color(.9f,.85f,.73f),creature);
  for(int i=-1;i<=1;i+=2)Shape("Creature horn",PrimitiveType.Capsule,new Vector3(i*.065f,.32f,-.015f),new Vector3(.025f,.04f,.025f),new Color(.35f,.3f,.25f),creature);
  }
  if(god==null&&creature.GetComponentInChildren<OriginalCowMotion>()==null&&terrain!=null)foreach(Transform visual in creature){visual.localPosition*=Size(.34f,13.9236f)/.34f;visual.localScale*=Size(.34f,13.9236f)/.34f;}
  creature.gameObject.AddComponent<IslandNavigator>().Initialize(navigation,true);
  cowMotion=creature.GetComponentInChildren<OriginalCowMotion>();
  creatureRoutine=creature.gameObject.AddComponent<CreatureRoutine>();creatureRoutine.speed=(.07f+state.care*.08f)*(terrain!=null?.1f:1);creatureRoutine.Initialize(godMotion,waterOffering,well,cistern,()=>{DeliverWater();curiosity?.RememberWaterDelivery(state);notice="The creature delivered water to the village.";},Ground);
  for(int i=0;i<workers.Count;i++){jobs[i]=workers[i].gameObject.AddComponent<VillagerRoutine>();jobs[i].Initialize((VillagerRoutine.Role)i,i==0?well:i==1?Landmark("field",Site(new Vector3(.2f,Floor,.26f)),p=>p.town==0):Landmark("tree",Site(new Vector3(-.64f,Floor,-.12f)),p=>p.variant<13),cistern,Ground,role=>{if(role==VillagerRoutine.Role.WaterGatherer)state.water=Mathf.Min(100,state.water+.7f);else if(role==VillagerRoutine.Role.Farmer)state.crops=Mathf.Min(100,state.crops+1.2f);notice=role+" finished a delivery.";});}
  var affordances=CreaturePlayground.Build(transform,navigation,cistern,items.ToArray());
  construction=GetComponentInChildren<ConstructionSite>();construction?.Restore(state.construction);
  discoveries=IslandDiscoveries.Build(transform,navigation,cistern,construction,()=>state);affordances=GetComponentsInChildren<CreatureAffordance>();
  curiosity=creature.gameObject.AddComponent<CreatureCuriosity>();curiosity.Initialize(creatureRoutine,godMotion,affordances);
  for(int i=0;i<jobs.Length;i++){jobs[i].gameObject.AddComponent<VillagerBlockPlay>().Initialize(construction,curiosity,i);jobs[i].gameObject.AddComponent<VillagerSocial>().Initialize(curiosity,i,()=>state);jobs[i].gameObject.AddComponent<VillagerExploration>().Initialize(discoveries,i);}
  hybrid=creature.gameObject.AddComponent<CreatureHybrid>();hybrid.Initialize(creatureRoutine,curiosity,godMotion,affordances,jobs,reservoir.gameObject,waterOffering,()=>state);
  for(int i=0;i<items.Count;i++)if(items[i].GetComponent<HybridTarget>()==null)hybrid.Register(HybridTarget.Bind(items[i].gameObject,"resource-"+i,HybridTargetKind.Object));
  foreach(var job in jobs)job.GetComponent<VillagerSocial>().ConfigureSharing(hybrid,affordances);
  balls=GetComponentsInChildren<BeachBall>();foreach(var ball in balls)ball.Configure(hybrid,jobs);foreach(var job in jobs)job.GetComponent<VillagerExploration>().ConfigureBalls(balls);foreach(var job in jobs)job.GetComponent<VillagerExploration>().ConfigurePeers(jobs.Select(j=>j.GetComponent<VillagerExploration>()).ToArray());
  miracles=DivineIntervention.Build(transform,hybrid,navigation,cistern,jobs,()=>state);
  learning=creature.gameObject.AddComponent<CreatureConstructionLearning>();learning.Initialize(hybrid,construction,discoveries,()=>state);
  foreach(var job in jobs)job.GetComponent<VillagerExploration>().ConfigureConstruction(learning);
  tributes=VillageTributes.Build(transform,hybrid,jobs,navigation,cistern);
  foreach(var affordance in affordances){var cue=affordance.gameObject.AddComponent<SpatialTarget>();var offered=affordance;cue.invoke=()=>curiosity.Offer(offered);}
  creature.gameObject.AddComponent<CreatureAffect>().Initialize(curiosity,terrain!=null?terrain.scale:.025f);
  signals=gameObject.AddComponent<DroughtSignals>();signals.Initialize(reservoir,ground.sharedMaterial,jobs,creatureRoutine);
  Debug.Log("BWVR_ART_ACTIVE: environment="+(environment!=null)+", creature="+(god!=null?god.name:"original/prototype fallback")+", original fallback="+(art!=null));
  var creatureItem=creature.gameObject.AddComponent<CarryItem>();creatureItem.kind=Offering.Creature;creatureItem.home=creature.position;
  var target=creature.gameObject.AddComponent<SpatialTarget>();target.invoke=()=>GiveFeedback(true);var box=creature.gameObject.AddComponent<BoxCollider>();box.center=new Vector3(0,Size(.18f,7),0);box.size=new Vector3(Size(.2f,8),Size(.4f,15),Size(.2f,8));
  creatureLabel=Label("Creature intent",creature,new Vector3(0,Size(.44f,17),0),new Vector2(420,70),terrain!=null?.00007f:.0007f,22);creatureLabel.text="THEIR GOD · WATCHING";creatureLabel.gameObject.SetActive(false);
  panel=new GameObject("Wrist sanctuary");panel.transform.SetParent(left,false);panel.transform.localPosition=new Vector3(0,.13f,.12f);panel.transform.localRotation=Quaternion.Euler(25,0,0);
  var canvas=panel.AddComponent<Canvas>();canvas.renderMode=RenderMode.WorldSpace;var rect=panel.GetComponent<RectTransform>();rect.sizeDelta=new Vector2(600,560);panel.transform.localScale=Vector3.one*.0008f;
  var bg=new GameObject("Obsidian panel",typeof(RectTransform),typeof(Image));bg.transform.SetParent(panel.transform,false);bg.GetComponent<RectTransform>().sizeDelta=rect.sizeDelta;bg.GetComponent<Image>().color=new Color(.035f,.095f,.11f,.96f);
  status=Label("Season and guidance",panel.transform,new Vector3(0,130,0),new Vector2(540,260),1,25);status.alignment=TextAnchor.UpperLeft;
  Button("Pause / resume",new Vector3(-140,-70,0),()=>{state.paused=!state.paused;notice=state.paused?"Season paused.":"Season resumed.";});
  Button("Save season",new Vector3(140,-70,0),Save);
  Button("Begin again",new Vector3(-140,-160,0),()=>{foreach(var job in jobs)job.GetComponent<VillagerSocial>().ResetTask();state=new SliceState{developmentFocus=developmentFocus};hybrid?.NewSeason();miracles?.ResetWorld();creatureTrip=0;outcomeShown=false;signals.ResetRequests();curiosity?.ResetTask();construction?.ResetConstruction();discoveries?.ResetWorld();learning?.ResetWorld();foreach(var ball in balls)ball.ResetWorld();tributes?.ResetWorld();creatureRoutine.ResetTask();foreach(var job in jobs){job.GetComponent<VillagerExploration>().ResetTask();job.GetComponent<VillagerSocial>().ResetTask();job.ResetTask();}notice="A new dry season begins.";});
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
 Transform Controller(string name,int side){var g=new GameObject(name);g.transform.SetParent(rig,false);aims[side]=new GameObject(name+" aim").transform;aims[side].SetParent(rig,false);Shape("Controller presence",PrimitiveType.Sphere,Vector3.zero,Vector3.one*.035f,new Color(.85f,.72f,.42f),g.transform);var line=g.AddComponent<LineRenderer>();line.positionCount=2;line.startWidth=.002f;line.endWidth=.001f;line.material=new Material(Shader.Find("Sprites/Default"));line.startColor=line.endColor=new Color(.6f,.85f,.8f,.7f);rays[side]=line;pickupHands[side]=PickupHand.Create(transform,side);return g.transform;}
 CarryItem Item(string name,Offering kind,Vector3 at,PrimitiveType primitive,Color color,float size){var modern=kind==Offering.Villager?Resources.Load<GameObject>("Villagers/Villager"+(items.Count%3)):null;GameObject fitted=null;if(modern!=null){fitted=Instantiate(modern,transform);fitted.transform.position=Ground(at);fitted.transform.localScale=Vector3.one*Size(.085f,1.661f);}var original=fitted??(kind==Offering.Villager?Original(items.Count%2==0?"woman":"man",Ground(at),Size(.07f,1.661f)):null);var g=original??Shape(name,primitive,at,Vector3.one*size,color);if(kind==Offering.Villager)g.transform.localScale*=VillagerScale;var item=g.AddComponent<CarryItem>();item.kind=kind;item.home=g.transform.position;items.Add(item);return item;}
 Text Label(string name,Transform parent,Vector3 position,Vector2 size,float scale,int fontSize){var g=new GameObject(name,typeof(RectTransform));g.transform.SetParent(parent,false);g.transform.localPosition=position;g.transform.localScale=Vector3.one*scale;if(!parent.GetComponentInParent<Canvas>()){var c=g.AddComponent<Canvas>();c.renderMode=RenderMode.WorldSpace;}var t=g.AddComponent<Text>();t.font=font;t.fontSize=fontSize;t.color=new Color(.94f,.9f,.74f);t.alignment=TextAnchor.MiddleCenter;t.horizontalOverflow=HorizontalWrapMode.Wrap;g.GetComponent<RectTransform>().sizeDelta=size;return t;}
 void Button(string title,Vector3 position,Action action){var g=new GameObject(title,typeof(RectTransform),typeof(Image),typeof(BoxCollider),typeof(SpatialTarget));g.transform.SetParent(panel.transform,false);g.transform.localPosition=position;g.GetComponent<RectTransform>().sizeDelta=new Vector2(260,68);g.GetComponent<Image>().color=new Color(.13f,.28f,.29f);g.GetComponent<BoxCollider>().size=new Vector3(260,68,15);g.GetComponent<SpatialTarget>().invoke=action;Label("Caption",g.transform,Vector3.zero,new Vector2(250,65),1,24).text=title;}
 void Update(){if(eye==null||applicationSuspended)return;foreach(var motion in humanoids)motion.paused=state.paused||(state.Finished&&motion.GetComponent<CarryItem>()!=null&&motion.GetComponent<CarryItem>().kind!=Offering.Creature&&!(motion.GetComponent<VillagerBlockPlay>()?.Active??false)&&!(motion.GetComponent<VillagerSocial>()?.Active??false)&&!(motion.GetComponent<VillagerExploration>()?.Active??false));if(cowMotion!=null)cowMotion.paused=state.paused;clock+=Time.deltaTime;bool xr=XRSettings.isDeviceActive;
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
  bool praise=xr?controllerHands[1].praise:Keyboard.current!=null&&Keyboard.current.digit1Key.isPressed;
  bool discourage=xr?controllerHands[1].discourage:Keyboard.current!=null&&Keyboard.current.digit2Key.isPressed;
  if(praise!=discourage&&((praise&&!praiseLast)||(discourage&&!discourageLast))){var pointer=xr?aims[1]:right;if((!xr||controllerHands[1].tracked)&&Physics.Raycast(pointer.position,pointer.forward,out var feedbackHit,8*rig.localScale.x)&&feedbackHit.collider.GetComponentInParent<CreatureCuriosity>()==curiosity)GiveFeedback(praise);}
  praiseLast=praise;discourageLast=discourage;
  for(int i=0;i<2;i++){bool grip=xr&&i==1?controllerHands[i].trigger:controllerHands[i].grip,trigger=controllerHands[i].trigger;if(!xr){grip=i==1&&Keyboard.current!=null&&Keyboard.current.eKey.isPressed;trigger=i==1&&Keyboard.current!=null&&Keyboard.current.fKey.isPressed;}
   var hand=i==0?left:right;var pointer=xr?aims[i]:hand;var ray=new Ray(pointer.position,pointer.forward);Physics.Raycast(ray,out var hit,8*rig.localScale.x);rays[i].SetPosition(0,ray.origin);rays[i].SetPosition(1,hit.collider?hit.point:ray.GetPoint(2*rig.localScale.x));rays[i].enabled=xr?controllerHands[i].tracked:i==1;
   if(grip&&!lastGrip[i]&&hit.collider)TryGrab(i,hit.collider.GetComponentInParent<CarryItem>());
   if(held[i]!=null){if(xr&&!controllerHands[i].tracked){var item=held[i];held[i]=null;item.held=false;item.transform.position=item.home;item.GetComponent<Collider>().enabled=true;}else{held[i].transform.position=hand.position+hand.forward*(.16f*rig.localScale.x);throwMotion[i].Sample(held[i].transform.position,Time.unscaledTime,rig.localScale.x);var block=held[i].GetComponent<ConstructionBlock>();if(block!=null)block.Site.PreviewRay(block,ray);if(!grip)Release(i,ray);}}
   UpdatePickupHand(i,ray,hit,xr?controllerHands[i].tracked:i==1);
   // Right side grip is reserved for zoom; the front trigger carries or interacts.
   bool interact=trigger&&!lastTrigger[i]&&held[i]==null;
   if(interact&&hit.collider)hit.collider.GetComponentInParent<SpatialTarget>()?.Activate();lastGrip[i]=grip;lastTrigger[i]=trigger;
  }
  bool menu=controllerHands[0].menu;if(!xr)menu=Keyboard.current!=null&&Keyboard.current.tabKey.isPressed;if(menu&&!menuLast)panel.SetActive(!panel.activeSelf);menuLast=menu;
  Simulate(Time.deltaTime);if(!developmentFocus&&state.Finished&&!outcomeShown){outcomeShown=true;panel.SetActive(true);}
  status.text=SanctuaryText(xr);
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
 bool TryGrab(int side,CarryItem item){
  if(item==null||!item.CanGrab||held[side]!=null||(item.kind==Offering.Creature&&side!=1))return false;
  if(item.kind==Offering.Creature){item.home=item.transform.position;hybrid.Interrupt("Picked up by the player");}
  throwMotion[side].Reset();held[side]=item;item.held=true;item.GetComponent<Collider>().enabled=false;item.GetComponent<ConstructionBlock>()?.Grab();notice="Release the carry button over clear ground to place.";return true;
 }
 void Release(int i,Ray ray){var item=held[i];var toss=throwMotion[i].Velocity;if(!XRSettings.isDeviceActive&&Keyboard.current!=null&&Keyboard.current.spaceKey.isPressed)toss=ray.direction*(.5f*rig.localScale.x);throwMotion[i].Reset();held[i]=null;item.held=false;item.GetComponent<Collider>().enabled=true;var tool=item.GetComponent<DivineTool>();if(tool!=null){UseSpiritualTool(tool,ray);return;}var ball=item.GetComponent<BeachBall>();if(ball!=null&&!state.paused&&toss.magnitude>.12f*rig.localScale.x&&ball.PlayerToss(toss,rig.localScale.x)){notice="Tossed. The play circle marks its return spot.";return;}var block=item.GetComponent<ConstructionBlock>();if(block!=null){notice=block.Site.DropRay(block,ray)?"Blocks snapped together.":"Loose block released on the construction green.";hybrid.NoticePlaced(item);return;}Vector3 point;
  if(!TryDropPoint(item,ray,out point)){item.transform.position=item.home;notice="Returned safely: place on dry, clear walking ground.";return;}
  if(item.kind==Offering.Villager||item.kind==Offering.Creature)item.GetComponent<IslandNavigator>()?.ResetRoute();
  if(item.GetComponent<SharedRhythm>()!=null)item.transform.rotation=Quaternion.Euler(0,item.transform.eulerAngles.y,0);
  item.transform.position=point+Vector3.up*(item.kind==Offering.Villager||item.kind==Offering.Creature?0:item.GetComponent<SharedRhythm>()!=null?item.transform.lossyScale.y*.5f:Size(.04f,2));
  if(item.kind==Offering.Water&&Vector3.Distance(point,cistern)<Size(.18f,8)){DeliverWater();notice="Water delivered. Villagers celebrate their creature's gift.";item.transform.position=item.home;}
  else if(item.kind==Offering.Food&&Vector3.Distance(point,creature.position)<Size(.2f,8)){notice="Food offered. The creature can choose to investigate and eat.";}
  else {if(item.kind==Offering.Villager)item.home=item.transform.position;notice="Placed on the island.";}
  hybrid.NoticePlaced(item);
 }
 bool TryDropPoint(CarryItem item,Ray ray,out Vector3 point){
  point=ray.GetPoint(2*rig.localScale.x);
  if(terrain!=null){if(!groundCollider.Raycast(ray,out var hit,8*rig.localScale.x))return false;point=hit.point;if(!terrain.Dry(point))return false;}
  else{var plane=new Plane(Vector3.up,new Vector3(0,Floor,0));if(!plane.Raycast(ray,out float distance)||distance>8*rig.localScale.x)return false;point=ray.GetPoint(distance);if(new Vector2(point.x,point.z).magnitude>.82f)return false;}
  var walker=item.kind==Offering.Villager||item.kind==Offering.Creature?item.GetComponent<IslandNavigator>():null;
  if(walker!=null){if(!walker.CanPlace(point,out var safe))return false;point=safe;}return true;
 }
 void UpdatePickupHand(int side,Ray ray,RaycastHit hit,bool tracked){
  var item=held[side];bool carrying=item!=null;var point=hit.collider?hit.point:ray.GetPoint(2*rig.localScale.x);var normal=hit.collider?hit.normal:Vector3.up;bool valid=true,visible=hit.collider!=null;
  if(carrying&&item.GetComponent<DivineTool>()==null){
   var block=item.GetComponent<ConstructionBlock>();
   valid=block!=null?block.Site.PlacementPoint(block,ray,out point):TryDropPoint(item,ray,out point);normal=Vector3.up;visible=true;
  }
  pickupHands[side].Show(tracked&&visible,point,normal,ray.direction,rig.localScale.x,carrying,valid,hit.collider!=null&&hit.collider.GetComponentInParent<CarryItem>()?.CanGrab==true);
  if(tracked&&visible)rays[side].SetPosition(1,point);
 }
 void UseSpiritualTool(DivineTool tool,Ray ray){
  HybridTarget target=null;foreach(var hit in Physics.RaycastAll(ray,8*rig.localScale.x).OrderBy(h=>h.distance)){if(hit.collider.GetComponentInParent<DivineTool>()!=null)continue;target=hit.collider.GetComponentInParent<HybridTarget>();break;}
  bool accepted=held[0]==null&&held[1]==null&&miracles.TryUse(tool.kind,target);tool.ReturnHome();notice=accepted?"Intention offered.":miracles.LastReason;
  if(XRSettings.isDeviceActive)InputDevices.GetDeviceAtXRNode(XRNode.RightHand).SendHapticImpulse(0,accepted?.25f:.08f,.08f);
 }
 Vector3 Ground(Vector3 at){if(terrain!=null)return terrain.Project(at);at.y=Floor;return at;}
 public void Simulate(float dt){
   state.developmentFocus=developmentFocus;
  if(dt<=0||float.IsNaN(dt)||float.IsInfinity(dt))return;bool paused=state.paused||applicationSuspended;
   if(construction!=null)construction.Paused=paused;if(learning!=null)foreach(var site in learning.sites)site.Paused=paused;discoveries?.Advance(dt,paused);
  if(!paused){state.Hybrid.time=Mathf.Min(100000000,state.Hybrid.time+dt);state.Step(dt);learning?.Tick();tributes?.Tick(dt,false);AnimateVillage(dt);}foreach(var ball in balls)ball.Advance(dt,paused);miracles?.Tick(paused?0:dt,paused);signals.Tick(paused?0:dt,state);
 }
 void AnimateVillage(float dt){
  foreach(var job in jobs){var social=job.GetComponent<VillagerSocial>();var explore=job.GetComponent<VillagerExploration>();var play=job.GetComponent<VillagerBlockPlay>();
   social.Memory.restNeed=Mathf.Clamp01(social.Memory.restNeed+dt*.002f);
   bool urgent=!state.developmentFocus&&!state.Finished&&((job.role==VillagerRoutine.Role.WaterGatherer&&state.water<45)||(job.role==VillagerRoutine.Role.Farmer&&state.crops<45));
   if(urgent){if(social.Active)social.ResetTask();if(explore.Active)explore.ResetTask();if(play.Active)play.ResetTask();job.Tick(dt,false);continue;}
   bool playing=social.Tick(dt,false);if(!playing)playing=explore.Tick(dt,false);if(!playing)playing=play.Tick(dt,false);if(!playing&&!state.Finished)job.Tick(dt,false);
  }
  creatureRoutine.speed=(.07f+state.care*.08f)*(terrain!=null?.1f:1);hybrid.Tick(dt,false);
  creatureLabel.text="THEIR GOD · "+(helpVisible?hybrid.Explanation:creatureRoutine.Current.ToString().ToUpperInvariant());
 }
 public bool HelpVisible=>helpVisible;
 public void SetHelp(bool enabled){helpVisible=enabled;if(creatureLabel!=null)creatureLabel.gameObject.SetActive(enabled);if(status!=null){status.fontSize=enabled?20:25;status.rectTransform.sizeDelta=new Vector2(540,enabled?300:260);}}
 string SanctuaryText(bool xr){
  string title=developmentFocus?(state.paused?"EXPLORATION · Paused":"EXPLORATION"):("THE LONG DRY"+(state.Finished?(state.Won?" · Village safe":" · Village struggling"):state.paused?" · Paused":""));
  if(!helpVisible)return title+"\nEnergy "+Mathf.FloorToInt(state.Hybrid.energy)+(saveError!=null?"\n"+saveError:"");
  var g=state.Hybrid.guidance;string reason=g.reason.Length>48?g.reason.Substring(0,48):g.reason;
  return title+" · "+Mathf.CeilToInt(state.seconds)+"s\nWater "+Mathf.RoundToInt(state.water)+" · Crops "+Mathf.RoundToInt(state.crops)+" · Energy "+Mathf.FloorToInt(state.Hybrid.energy)+"\n"+hybrid.Explanation+"\nCue: "+g.status+(reason.Length>0?" · "+reason:"")+"\nRight front trigger: hold to carry the creature or objects · release to place\nMove and release a held ball to toss · A/B: teach\nCarry cloud to water; sprig to injured villager\nHelp / shield / lens: offer over a target\nX menu · right grip + stick: zoom"+(xr?"":"\nDesktop: 1/2 teach · E carry · F select · Space + release E toss");
 }
 public bool GiveFeedback(bool positive){
  if(state.paused||curiosity==null||held[0]!=null||held[1]!=null||Time.unscaledTime<feedbackReady)return false;
  feedbackReady=Time.unscaledTime+.5f;bool learned=hybrid!=null?hybrid.Feedback(positive):curiosity.Feedback(positive);godMotion?.Feedback(positive);
  if(XRSettings.isDeviceActive)InputDevices.GetDeviceAtXRNode(XRNode.RightHand).SendHapticImpulse(0,positive?.25f:.55f,.08f);
  notice=learned?(positive?"That action encouraged.":"That action discouraged."):"The creature feels your presence. No recent action to teach.";return true;
 }
 void DeliverWater(){int before=state.gifts;state.Deliver();if(state.gifts>before)signals?.WaterArrived();}
 void Save(){try{if(construction!=null)state.construction=construction.Capture();discoveries?.Capture();learning?.Capture();File.WriteAllText(SavePath+".tmp",JsonUtility.ToJson(state,true));File.Copy(SavePath+".tmp",SavePath,true);saveError=null;notice="Season saved on this device.";}catch(Exception e){saveError="Save failed. Please try again.";notice="Could not save: "+e.Message;if(panel!=null)panel.SetActive(true);Debug.LogError("BWVR_SAVE_FAILED: "+e.Message);}}
 void Load(){if(!File.Exists(SavePath))return;try{var s=JsonUtility.FromJson<SliceState>(File.ReadAllText(SavePath));if(s==null||!s.Valid())throw new Exception("Invalid season data");state=s;state.paused=false;notice="Season restored.";}catch(Exception){saveError="Saved season unavailable. A fresh season is ready.";notice=saveError;}}
 void OnApplicationPause(bool paused){applicationSuspended=paused;if(miracles!=null){miracles.Suspended=paused;miracles.Tick(0,paused||state.paused);}hybrid?.Suspend(paused);foreach(var motion in humanoids)motion.paused=paused||state.paused;if(cowMotion!=null)cowMotion.paused=paused||state.paused;if(paused){if(jobs!=null)foreach(var job in jobs)job.GetComponent<VillagerSocial>().ResetTask();if(construction!=null)construction.Paused=true;if(learning!=null)foreach(var site in learning.sites)site.Paused=true;discoveries?.Advance(0,true);for(int i=0;i<2;i++)if(held[i]!=null){held[i].transform.position=held[i].home;held[i].held=false;held[i].GetComponent<Collider>().enabled=true;held[i]=null;}Save();}}
 void OnApplicationQuit(){Save();}
}
}
