using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace BWVR {
 [System.Serializable] public sealed class ConstructionRecord {public Vector3 position;public Quaternion rotation;public int support=-2;public bool creatureBuilt;}
// Simulate toy-scale pieces in a private metre-scale physics scene. The tabletop
// conversion stays at its boundary; no global gravity/contact settings are changed.
public sealed class ConstructionSite:MonoBehaviour {
 public readonly List<ConstructionBlock> blocks=new List<ConstructionBlock>();public float unit{get;private set;}
 public int Snapped=>blocks.Count(b=>b.Attached);public bool Paused;public int successfulSnaps{get;private set;}
 public float LastCreatureBuildTime{get;private set;}=float.NegativeInfinity;public float nextVillagerPlayAt;
 Scene simulationScene;PhysicsScene physics;Material[] colors;Material outlineMaterial;LineRenderer preview;readonly Collider[] overlaps=new Collider[32];
 public static ConstructionSite Build(Transform parent,IslandNavigation nav,Vector3 village){
  Vector3 centre=village+new Vector3(-20,0,-20)*nav.Scale;
  for(int i=0;i<48;i++){
   float angle=i*137.5f*Mathf.Deg2Rad;var at=village+new Vector3(Mathf.Cos(angle),0,Mathf.Sin(angle))*(24+i*.35f)*nav.Scale;
   if(!nav.Sample(at,nav.creature,3,out var safe))continue;
   bool clear=true;for(int j=0;j<8;j++){float a=j*Mathf.PI/4;var edge=safe+new Vector3(Mathf.Cos(a),0,Mathf.Sin(a))*nav.Scale*11;
    if(!nav.Sample(edge,nav.villager,1,out var point)||Mathf.Abs(point.y-safe.y)>nav.Scale*.7f){clear=false;break;}}
   if(clear){centre=nav.Terrain!=null?nav.Terrain.Project(safe):safe;break;}
  }
  if(nav.Terrain!=null){float highest=nav.Terrain.Project(centre).y;for(int j=0;j<8;j++){float a=j*Mathf.PI/4;highest=Mathf.Max(highest,nav.Terrain.Project(centre+new Vector3(Mathf.Cos(a),0,Mathf.Sin(a))*nav.Scale*13).y);}centre.y=highest+nav.Scale*.1f;}
  var root=new GameObject("Creature construction green");root.transform.SetParent(parent,false);root.transform.position=centre;
  var site=root.AddComponent<ConstructionSite>();site.Initialize(nav.Scale);return site;
 }
 public Vector3 World(Vector3 point)=>transform.TransformPoint(point*unit);
 public Vector3 Local(Vector3 point)=>transform.InverseTransformPoint(point)/unit;
 void Initialize(float scale){
  unit=scale;simulationScene=SceneManager.CreateScene("BWVR toy physics "+System.Guid.NewGuid().ToString("N"),new CreateSceneParameters(LocalPhysicsMode.Physics3D));physics=simulationScene.GetPhysicsScene();
  colors=new[]{new Color(.42f,.18f,.07f),new Color(.08f,.32f,.44f),new Color(.55f,.38f,.08f),new Color(.20f,.37f,.15f)}.Select(c=>{var m=new Material(Shader.Find("BWVR/GameSurface"));m.color=c;m.SetFloat("_Cutoff",0);return m;}).ToArray();
  var pad=GameObject.CreatePrimitive(PrimitiveType.Cube);pad.name="Construction foundation - snap grid";pad.transform.SetParent(transform,false);pad.transform.localPosition=Vector3.down*unit*.5f;pad.transform.localScale=new Vector3(18,1,18)*unit;pad.GetComponent<Renderer>().sharedMaterial=colors[0];pad.GetComponent<BoxCollider>().contactOffset=unit*.02f;
  StaticBox("Foundation collider",new Vector3(0,-.5f,0),new Vector3(18,1,18));
  // Low tray edges keep loose pieces within this bounded toy sandbox.
  foreach(int side in new[]{-1,1})foreach(bool horizontal in new[]{false,true}){
   var position=horizontal?new Vector3(0,.35f,side*9):new Vector3(side*9,.35f,0);var dimensions=horizontal?new Vector3(18,1,.3f):new Vector3(.3f,1,18);
   StaticBox("Tray edge",position,dimensions);var rail=GameObject.CreatePrimitive(PrimitiveType.Cube);rail.name="Wooden toy tray rim";rail.transform.SetParent(transform,false);rail.transform.localPosition=position*unit;rail.transform.localScale=dimensions*unit;rail.GetComponent<Renderer>().sharedMaterial=colors[0];Destroy(rail.GetComponent<Collider>());
  }
  var art=Resources.Load<EnvironmentArt>("ConstructionArt");
  for(int i=0;i<8;i++){
   bool beam=i==6;var dimensions=new Vector3(beam?6:2,1.2f,2);var source=art!=null?art.Find(beam?"Beam":"Brick"):null;
   var go=source!=null?Instantiate(source,transform):GameObject.CreatePrimitive(PrimitiveType.Cube);go.name=beam?"Long construction lintel":"Construction brick "+(i+1);go.transform.SetParent(transform,false);go.transform.localScale=Vector3.one*unit;
   if(source==null){go.transform.localScale=dimensions*unit;Destroy(go.GetComponent<Collider>());}
   foreach(var r in go.GetComponentsInChildren<Renderer>())r.sharedMaterial=colors[i%colors.Length];
   var box=go.GetComponent<BoxCollider>()??go.AddComponent<BoxCollider>();box.center=Vector3.zero;box.size=source!=null?dimensions:Vector3.one;box.contactOffset=unit*.02f;
   var item=go.AddComponent<CarryItem>();item.kind=Offering.Toy;
   var proxy=new GameObject(go.name+" physics");SceneManager.MoveGameObjectToScene(proxy,simulationScene);var collider=proxy.AddComponent<BoxCollider>();collider.size=dimensions;collider.contactOffset=.005f;
   var body=proxy.AddComponent<Rigidbody>();body.mass=beam?.3f:.1f;body.interpolation=RigidbodyInterpolation.None;body.collisionDetectionMode=CollisionDetectionMode.ContinuousDynamic;body.solverIterations=12;body.solverVelocityIterations=4;body.maxAngularVelocity=12;
   body.position=new Vector3(-6+(i%3)*6,.7f,-6+(i/3)*6);if(i==4)body.position=new Vector3(6,.7f,0);if(i==5)body.position=new Vector3(-6,.7f,6);if(beam)body.position=new Vector3(0,.7f,6);if(i==7)body.position=new Vector3(6,.7f,6);
   go.transform.position=World(body.position);item.home=go.transform.position;
   var block=go.AddComponent<ConstructionBlock>();block.Initialize(this,body,dimensions);blocks.Add(block);
   go.AddComponent<CreatureAffordance>().kind=CreatureAffordance.Kind.Turn;
  }
  var p=new GameObject("Construction snap preview");p.transform.SetParent(transform,false);preview=p.AddComponent<LineRenderer>();preview.useWorldSpace=true;preview.positionCount=16;preview.startWidth=preview.endWidth=unit*.08f;preview.shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off;
  outlineMaterial=new Material(Resources.Load<Material>("ConstructionPreview"));preview.sharedMaterial=outlineMaterial;preview.enabled=false;
 }
 void StaticBox(string label,Vector3 position,Vector3 size){var go=new GameObject(label);SceneManager.MoveGameObjectToScene(go,simulationScene);go.transform.position=position;var box=go.AddComponent<BoxCollider>();box.size=size;box.contactOffset=.005f;}
 void FixedUpdate(){Advance(Time.fixedDeltaTime);}
 public void Advance(float dt){
  if(Paused||!physics.IsValid()||dt<=0)return;
  foreach(var block in blocks){if(block.Item.held){if(!block.wasHeld)block.Grab();block.Body.position=Local(block.transform.position);block.Body.rotation=Quaternion.Inverse(transform.rotation)*block.transform.rotation;}
   else if(block.wasHeld){block.wasHeld=false;Drop(block,block.transform.position,block.transform.rotation);}}
  foreach(var block in blocks)if(block.VillagerOwner!=null)block.VillagerOwner.ApplyPhysicsPush();
  physics.Simulate(Mathf.Min(dt,.04f));
  foreach(var block in blocks)if(!block.Item.held){if(block.Body.position.y< -3||block.Body.position.sqrMagnitude>1600)ReturnLoose(block);block.transform.SetPositionAndRotation(World(block.Body.position),transform.rotation*block.Body.rotation);}
 }
 public void Detach(ConstructionBlock block){
  foreach(var piece in blocks)if(piece.Connection!=null&&(piece==block||piece.Connection.connectedBody==block.Body)){piece.Connection.xMotion=piece.Connection.yMotion=piece.Connection.zMotion=ConfigurableJointMotion.Free;piece.Connection.angularXMotion=piece.Connection.angularYMotion=piece.Connection.angularZMotion=ConfigurableJointMotion.Free;Destroy(piece.Connection);piece.Connection=null;}
 }
 public void ReturnLoose(ConstructionBlock block){Detach(block);block.creatureBuilt=false;Activate(block);block.Body.transform.SetPositionAndRotation(block.spawn,Quaternion.identity);block.Body.position=block.spawn;block.Body.rotation=Quaternion.identity;block.transform.SetPositionAndRotation(World(block.spawn),transform.rotation);}
 public void ResetConstruction(){HidePreview();foreach(var block in blocks){if(block.VillagerOwner!=null)block.VillagerOwner.ResetTask();block.Item.held=false;block.wasHeld=false;ReturnLoose(block);block.GetComponent<CreatureAffordance>().cooldown=0;}LastCreatureBuildTime=float.NegativeInfinity;nextVillagerPlayAt=0;}
 void Activate(ConstructionBlock block){block.Body.isKinematic=false;block.Body.detectCollisions=true;block.Body.linearVelocity=Vector3.zero;block.Body.angularVelocity=Vector3.zero;block.Body.WakeUp();block.Item.held=false;block.wasHeld=false;block.GetComponent<Collider>().enabled=true;}
 bool Empty(ConstructionBlock block,Vector3 point,Quaternion rotation){int count=physics.OverlapBox(point,block.size*.5f-Vector3.one*.025f,overlaps,rotation);for(int i=0;i<count;i++)if(overlaps[i].attachedRigidbody!=block.Body)return false;return true;}
 bool Support(Vector3 point,ConstructionBlock block,out Rigidbody support,Quaternion rotation=default){support=null;if(Mathf.Abs(point.y-.6f)<.05f)return true;
  if(rotation==default)rotation=Quaternion.identity;var x=rotation*new Vector3(block.size.x*.5f,0,0);var z=rotation*new Vector3(0,0,block.size.z*.5f);var half=new Vector3(Mathf.Abs(x.x)+Mathf.Abs(z.x),0,Mathf.Abs(x.z)+Mathf.Abs(z.z));
  foreach(var other in blocks)if(other!=block&&!other.Item.held&&other.Attached&&Mathf.Abs(other.Body.position.y+1.2f-point.y)<.04f){var bounds=other.Body.GetComponent<BoxCollider>().bounds;
   if(bounds.max.x>point.x-half.x+.25f&&bounds.min.x<point.x+half.x-.25f&&bounds.max.z>point.z-half.z+.25f&&bounds.min.z<point.z+half.z-.25f){support=other.Body;return true;}}
  return false;
 }
 public bool TryPlan(ConstructionBlock block,out Vector3 world,out Quaternion rotation){
  for(int i=0;i<8;i++){bool beam=i==6;if(beam!=(block.size.x>3))continue;
   if(beam&&blocks.Count(b=>b.Attached&&Mathf.Abs(b.Body.position.y-3f)<.1f&&Mathf.Abs(b.Body.position.z)<.1f&&Mathf.Abs(Mathf.Abs(b.Body.position.x)-2)<.1f)<2)continue;
   var point=i<6?new Vector3(i%2==0?-2:2,.6f+(i/2)*1.2f,0):new Vector3(0,i==6?4.2f:5.4f,0);
   if(Support(point,block,out _)&&Empty(block,point,Quaternion.identity)){world=World(point);rotation=transform.rotation;return true;}
  }world=default;rotation=transform.rotation;return false;
 }
 public bool FindSnap(ConstructionBlock block,Vector3 desired,Quaternion rotation,out Vector3 target,out Quaternion aligned,out Rigidbody support){
  var point=Local(desired);var localRotation=Quaternion.Inverse(transform.rotation)*rotation;aligned=Quaternion.Euler(0,Mathf.Round(localRotation.eulerAngles.y/90)*90,0);target=default;support=null;
  if(Quaternion.Angle(localRotation,aligned)>30)return false;float best=.85f*.85f;
  for(int level=0;level<8;level++)for(int z=-6;z<=6;z+=2)for(int x=-6;x<=6;x+=2){var candidate=new Vector3(x,.6f+level*1.2f,z);float distance=(candidate-point).sqrMagnitude;if(distance>best||!Support(candidate,block,out var under,aligned)||!Empty(block,candidate,aligned))continue;best=distance;target=candidate;support=under;}
  return best<.85f*.85f;
 }
 public bool Drop(ConstructionBlock block,Vector3 desired,Quaternion rotation,bool byCreature=false){
  block.Item.held=false;block.wasHeld=false;preview.enabled=false;
  bool snap=FindSnap(block,desired,rotation,out var at,out var facing,out var under);Detach(block);
  if(snap){Activate(block);block.Body.transform.SetPositionAndRotation(at,facing);block.Body.position=at;block.Body.rotation=facing;Connect(block,under);successfulSnaps++;}
  else {var p=Local(desired);if(Mathf.Abs(p.x)>8||Mathf.Abs(p.z)>8||p.y<.6f)p=block.spawn;Activate(block);var q=Quaternion.Inverse(transform.rotation)*rotation;block.Body.transform.SetPositionAndRotation(p,q);block.Body.position=p;block.Body.rotation=q;}
  block.creatureBuilt=snap&&byCreature;if(block.creatureBuilt)LastCreatureBuildTime=Time.time;
  block.transform.SetPositionAndRotation(World(block.Body.position),transform.rotation*block.Body.rotation);return snap;
 }
 bool RayPoint(ConstructionBlock block,Ray ray,out Vector3 point){foreach(var hit in Physics.RaycastAll(ray,10).OrderBy(h=>h.distance))if(hit.collider!=block.GetComponent<Collider>()&&(hit.collider.transform==transform.GetChild(0)||hit.collider.GetComponent<ConstructionBlock>()?.Site==this)){point=hit.point+Vector3.up*unit*.6f;return true;}point=World(block.spawn);return false;}
 public bool DropRay(ConstructionBlock block,Ray ray){RayPoint(block,ray,out var point);return Drop(block,point,transform.rotation);}
 public void PreviewRay(ConstructionBlock block,Ray ray){if(RayPoint(block,ray,out var point)&&FindSnap(block,point,transform.rotation,out var local,out var rotation,out _))ShowPreview(block,World(local),transform.rotation*rotation);else preview.enabled=false;}
 public void ShowPreview(ConstructionBlock block,Vector3 point,Quaternion rotation){
  preview.enabled=true;preview.startColor=preview.endColor=new Color(.35f,1,.55f);var half=block.size*.5f*unit;
  var corners=new[]{new Vector3(-1,-1,-1),new Vector3(1,-1,-1),new Vector3(1,-1,1),new Vector3(-1,-1,1),new Vector3(-1,1,-1),new Vector3(1,1,-1),new Vector3(1,1,1),new Vector3(-1,1,1)};var order=new[]{0,1,2,3,0,4,5,1,5,6,2,6,7,3,7,4};
  for(int i=0;i<order.Length;i++)preview.SetPosition(i,point+rotation*Vector3.Scale(corners[order[i]],half));
 }
 public void HidePreview(){if(preview!=null)preview.enabled=false;}
 void Connect(ConstructionBlock block,Rigidbody support){var joint=block.Body.gameObject.AddComponent<ConfigurableJoint>();joint.autoConfigureConnectedAnchor=false;joint.connectedBody=support;joint.anchor=Vector3.zero;joint.connectedAnchor=support!=null?support.transform.InverseTransformPoint(block.Body.position):block.Body.position;joint.xMotion=joint.yMotion=joint.zMotion=ConfigurableJointMotion.Locked;joint.angularXMotion=joint.angularYMotion=joint.angularZMotion=ConfigurableJointMotion.Locked;joint.breakForce=250;joint.breakTorque=250;joint.enableCollision=false;block.Connection=joint;}
 public ConstructionRecord[] Capture()=>blocks.Select(b=>{
  bool held=b.Item.held||b.wasHeld;
  return new ConstructionRecord{position=held?b.spawn:b.Body.position,rotation=held?Quaternion.identity:b.Body.rotation,support=held||!b.Attached?-2:b.Connection.connectedBody==null?-1:blocks.FindIndex(p=>p.Body==b.Connection.connectedBody),creatureBuilt=!held&&b.creatureBuilt};
 }).ToArray();
 public static bool Valid(ConstructionRecord[] records){
  if(records==null||records.Length==0)return true;if(records.Length!=8)return false;
  for(int i=0;i<records.Length;i++){var r=records[i];if(r==null||r.support< -2||r.support>=8||!Finite(r.position.x)||!Finite(r.position.y)||!Finite(r.position.z)||r.position.sqrMagnitude>1600||!Finite(r.rotation.x)||!Finite(r.rotation.y)||!Finite(r.rotation.z)||!Finite(r.rotation.w))return false;
   float norm=r.rotation.x*r.rotation.x+r.rotation.y*r.rotation.y+r.rotation.z*r.rotation.z+r.rotation.w*r.rotation.w;if(Mathf.Abs(norm-1)>.05f)return false;
   int parent=r.support;var visited=new HashSet<int>{i};while(parent>=0){if(parent>=records.Length||records[parent]==null||!visited.Add(parent))return false;parent=records[parent].support;}
  }return true;
 }
 static bool Finite(float f)=>!float.IsNaN(f)&&!float.IsInfinity(f);
 public void Restore(ConstructionRecord[] records){
  if(records==null||records.Length==0)return;if(!Valid(records))throw new System.ArgumentException("Invalid saved construction");
  ResetConstruction();
  for(int i=0;i<blocks.Count;i++){var b=blocks[i];b.creatureBuilt=records[i].creatureBuilt;b.Body.transform.SetPositionAndRotation(records[i].position,records[i].rotation);b.Body.position=records[i].position;b.Body.rotation=records[i].rotation;b.transform.SetPositionAndRotation(World(b.Body.position),transform.rotation*b.Body.rotation);}
  for(int i=0;i<blocks.Count;i++)if(records[i].support>= -1)Connect(blocks[i],records[i].support== -1?null:blocks[records[i].support].Body);
  if(records.Any(r=>r.creatureBuilt))LastCreatureBuildTime=Time.time;
 }
 void OnDestroy(){if(colors!=null)foreach(var m in colors)Destroy(m);if(outlineMaterial!=null)Destroy(outlineMaterial);if(simulationScene.IsValid()&&simulationScene.isLoaded)SceneManager.UnloadSceneAsync(simulationScene);}
}
}
