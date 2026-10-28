using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace BWVR {
// Six dynamic pins share the beachball's existing private PhysX scene and clock.
public sealed class BowlingLane {
 public const int PinCount=6;public bool Available{get;private set;}public bool Active{get;private set;}public bool LastCreatureOutcome{get;private set;}public int LastScore{get;private set;}public int FallenMask{get;private set;}
 public Vector3 Start{get;private set;}public Vector3 Target{get;private set;}public Vector3 Forward{get;private set;}public Vector3 Right=>Vector3.Cross(Vector3.up,Forward);
 public readonly Rigidbody[] Pins=new Rigidbody[PinCount];readonly Transform[] views=new Transform[PinCount];readonly Vector3[] origins=new Vector3[PinCount];readonly Renderer[] lights=new Renderer[PinCount];
 readonly BeachBall ball;readonly IslandNavigation nav;readonly CreatureHybrid god;readonly VillagerSocial[] people;readonly List<Material> materials=new List<Material>();readonly List<Mesh> meshes=new List<Mesh>();readonly MaterialPropertyBlock tint=new MaterialPropertyBlock();
 Transform root;HybridTarget target;Renderer bell;float shotTime,settled,resetIn,demonstratedUntil,sightIn;bool creatureShot,witnessed,contact;int beforeMask;BowlingMemory Memory=>god.Season.WorldPlay.Bowling;
 public BowlingLane(BeachBall sphere,IslandNavigation navigation,CreatureHybrid creature,Scene physics,IReadOnlyList<PlayImpactTarget> obstacles){
  ball=sphere;nav=navigation;god=creature;people=god.GetComponentInParent<QuestSlice>().GetComponentsInChildren<VillagerSocial>();
  Start=nav.ProjectFoot(ball.GetComponent<CarryItem>().home,nav.creature);
  for(int i=0;i<32&&!Available;i++){var direction=Quaternion.Euler(0,i*137.5f,0)*Vector3.forward;bool clear=true;
   for(int z=0;z<=40&&clear;z+=8)for(int x=-10;x<=10;x+=10){var wanted=Start+(direction*z+Vector3.Cross(Vector3.up,direction)*x)*nav.Scale;if(!nav.Sample(wanted,nav.villager,.75f,out var at)||!nav.Sample(wanted,nav.creature,.75f,out _)||Mathf.Abs(nav.ProjectFoot(at,nav.villager).y-Start.y)>nav.Scale*1.5f){clear=false;break;}foreach(var obstacle in obstacles)if(obstacle.villager==null&&obstacle.bounds.SqrDistance(wanted)<nav.Scale*nav.Scale*16){clear=false;break;}}
   if(clear){Forward=direction;Available=true;}
  }
  if(!Available){Debug.LogWarning("BWVR_BOWLING_UNAVAILABLE: no safe flat lane near the ball");return;}
  Target=Start+Forward*24*nav.Scale;root=new GameObject("Giant bowling — watch, roll, imitate").transform;root.SetParent(ball.transform.parent,false);root.position=Target;
  target=HybridTarget.Bind(root.gameObject,"bowling-"+ball.StableId,HybridTargetKind.Object);god.Register(target);
  var wood=Mat(new Color(.88f,.76f,.48f));var red=Mat(new Color(.85f,.19f,.12f));var glow=Mat(new Color(.95f,.78f,.2f));var marking=Mat(new Color(.35f,.65f,.64f));
  var pinMesh=PinMesh();meshes.Add(pinMesh);int index=0;for(int row=0;row<3;row++)for(int column=0;column<=row;column++){
   var foot=nav.ProjectFoot(Target+(Right*(column-row*.5f)*6.4f+Forward*row*5.5f)*nav.Scale,nav.villager);origins[index]=nav.ToNav(foot)+Vector3.up*3;
   var proxy=new GameObject("Bowling pin physics "+index);SceneManager.MoveGameObjectToScene(proxy,physics);var collider=proxy.AddComponent<CapsuleCollider>();collider.radius=1.25f;collider.height=6;collider.contactOffset=.01f;
   var body=proxy.AddComponent<Rigidbody>();body.mass=1.2f;body.centerOfMass=Vector3.down*.6f;body.linearDamping=.15f;body.angularDamping=.2f;body.collisionDetectionMode=CollisionDetectionMode.ContinuousDynamic;body.position=origins[index];Pins[index]=body;
   var listener=proxy.AddComponent<BowlingPinCollision>();listener.lane=this;listener.ball=ball.Body;
   var view=new GameObject("Wooden bowling pin "+(index+1)).transform;view.SetParent(root,false);view.localScale=Vector3.one*nav.Scale;views[index]=view;
   var carved=new GameObject("Carved bowling pin");carved.transform.SetParent(view,false);carved.AddComponent<MeshFilter>().sharedMesh=pinMesh;carved.AddComponent<MeshRenderer>().sharedMaterial=wood;
   Shape("Red band",PrimitiveType.Cylinder,view,Vector3.up*1.2f,new Vector3(1.04f,.12f,1.04f),red);
   Shape("Second red band",PrimitiveType.Cylinder,view,Vector3.up*1.55f,new Vector3(1.06f,.09f,1.06f),red);
   lights[index]=Shape("Result glow",PrimitiveType.Sphere,root,Vector3.zero,Vector3.one*nav.Scale*1.1f,glow).GetComponent<Renderer>();lights[index].transform.position=foot+Vector3.up*nav.Scale*.5f;lights[index].enabled=false;index++;
  }
  for(int i=0;i<7;i++){var at=nav.ProjectFoot(Start+Forward*i*3*nav.Scale,nav.villager)+Vector3.up*nav.Scale*.15f;var dot=Shape("Aim pebble",PrimitiveType.Cylinder,root,Vector3.zero,new Vector3(.7f,.12f,.7f)*nav.Scale,marking);dot.transform.position=at;}
  var reset=Shape("Reset bowling rack",PrimitiveType.Sphere,root,Vector3.zero,Vector3.one*nav.Scale*4,red);reset.transform.position=nav.ProjectFoot(Start+Right*12*nav.Scale,nav.villager)+Vector3.up*nav.Scale*3;reset.AddComponent<SphereCollider>();reset.AddComponent<SpatialTarget>().invoke=()=>TryReset();bell=reset.GetComponent<Renderer>();ResetRack(false);
 }
 static Mesh PinMesh(){
  float[] height={-3,-3,-2.8f,-2.1f,-1,0,.8f,1.4f,1.8f,2.2f,2.6f,2.9f,3},radius={0,.7f,1.1f,1.25f,1.2f,.85f,.52f,.45f,.6f,.85f,.8f,.5f,0};const int sides=16;
  var vertices=new Vector3[height.Length*(sides+1)];var uv=new Vector2[vertices.Length];var triangles=new int[(height.Length-1)*sides*6];int t=0;
  for(int ring=0;ring<height.Length;ring++)for(int side=0;side<=sides;side++){int index=ring*(sides+1)+side;float a=side*Mathf.PI*2/sides;vertices[index]=new Vector3(Mathf.Cos(a)*radius[ring],height[ring],Mathf.Sin(a)*radius[ring]);uv[index]=new Vector2(side/(float)sides,(height[ring]+3)/6);if(ring==height.Length-1||side==sides)continue;int next=index+sides+1;triangles[t++]=index;triangles[t++]=next;triangles[t++]=index+1;triangles[t++]=index+1;triangles[t++]=next;triangles[t++]=next+1;}
  var mesh=new Mesh{name="Original low-poly carved bowling pin",vertices=vertices,uv=uv,triangles=triangles};mesh.RecalculateNormals();mesh.RecalculateBounds();return mesh;
 }
 Material Mat(Color color){var m=new Material(Shader.Find("BWVR/GameSurface")){color=color,enableInstancing=true};materials.Add(m);return m;}
 GameObject Shape(string name,PrimitiveType type,Transform parent,Vector3 local,Vector3 scale,Material mat){var go=GameObject.CreatePrimitive(type);go.name=name;go.transform.SetParent(parent,false);go.transform.localPosition=local;go.transform.localScale=scale;go.GetComponent<Renderer>().sharedMaterial=mat;var c=go.GetComponent<Collider>();c.enabled=false;UnityEngine.Object.Destroy(c);return go;}
 public bool CanImitate(Vector3 actor){if(!Available||Active||FallenMask==63||Memory.observedRolls==0||ball.FetchReserved||ball.PeerGame||Vector3.Distance(actor,Start)>nav.Scale*18)return false;foreach(var person in people){if(person==null||person.GetComponent<CarryItem>().held)continue;var delta=person.transform.position-Start;float forward=Vector3.Dot(delta,Forward)/nav.Scale;if(forward>5&&forward<40&&Mathf.Abs(Vector3.Dot(delta,Right))/nav.Scale<5)return false;}return true;}
 public float DemonstrationBias=>Mathf.Clamp01((demonstratedUntil-god.State.time)/60)*.7f;
 public void ClearDemonstration(){demonstratedUntil=0;}
 public Vector3 Aim(Vector3 actor){var direction=Target-actor;direction.y=0;float spread=Mathf.Lerp(15,3,Memory.practice);float variation=CreatureDecisionModel.Variation(god.State.seed,god.State.decisions,CreatureAction.PlayWithToy,"bowling");return Quaternion.Euler(0,variation*spread,0)*direction.normalized;}
 public bool TryPlayerRoll(Vector3 velocity){if(!Available||Active||FallenMask==63)return false;var delta=ball.transform.position-Start;delta.y=0;var direction=velocity;direction.y=0;if(delta.magnitude>nav.Scale*16||direction.sqrMagnitude<.01f||Vector3.Dot(direction.normalized,Forward)<.8f)return false;Begin(false);Roll(direction.normalized,Mathf.Clamp(direction.magnitude*2,8,24));return true;}
 public bool TryCreatureRoll(){if(!CanImitate(god.transform.position))return false;var direction=Aim(god.transform.position);ball.Throw(direction,true,18);Begin(true);Roll(direction,18);return true;}
 void Roll(Vector3 direction,float speed){var foot=nav.ProjectFoot(ball.transform.position,nav.creature);ball.Body.position=nav.ToNav(foot)+Vector3.up*(ball.Radius+.1f);ball.Body.linearVelocity=direction*speed;ball.Body.angularVelocity=Vector3.Cross(Vector3.up,direction)*speed/ball.Radius;ball.Body.WakeUp();ball.transform.position=nav.ToWorld(ball.Body.position);}
 void Begin(bool creature){Active=true;creatureShot=creature;witnessed=contact=false;shotTime=settled=sightIn=0;beforeMask=FallenMask;LastCreatureOutcome=false;LastScore=0;resetIn=0;}
 public void Contact(){if(Active)contact=true;}
 public void Tick(float dt){if(!Available||god.Season.paused||dt<=0)return;
  if(ball.GetComponent<CarryItem>().held&&Active)CancelAttempt();
  for(int i=0;i<PinCount;i++){views[i].SetPositionAndRotation(nav.ToWorld(Pins[i].position),Pins[i].rotation);if(Vector3.Dot(Pins[i].rotation*Vector3.up,Vector3.up)<.65f||Pins[i].position.y<origins[i].y-1.5f)FallenMask|=1<<i;lights[i].enabled=(FallenMask&(1<<i))!=0;}
  if(Active){shotTime+=dt;if(!creatureShot&&!witnessed&&contact){sightIn-=dt;if(sightIn<=0){sightIn=.25f;witnessed=god.CanSeeCue(target)||god.CanSeeCue(ball.GetComponent<HybridTarget>());}}
   bool moving=ball.Body.linearVelocity.sqrMagnitude>1;foreach(var pin in Pins)moving|=pin.linearVelocity.sqrMagnitude>.2f||pin.angularVelocity.sqrMagnitude>.2f;settled=moving?0:settled+dt;
   if(FallenMask==63&&shotTime>=1.5f||shotTime>=12||shotTime>=2&&settled>=1.2f){LastScore=contact?Bits(FallenMask&~beforeMask):0;Memory.Complete(creatureShot,LastScore,witnessed&&!ball.Harmful);if(!creatureShot&&witnessed&&LastScore>0&&!ball.Harmful){demonstratedUntil=god.State.time+60;god.GetComponent<CreatureCuriosity>().NoticeAttention();}LastCreatureOutcome=creatureShot;Active=false;resetIn=8;
    god.Journal.Add(god.State.time,"bowling result",CreatureAction.PlayWithToy,ball.StableId,(creatureShot?"Creature":"Player")+" knocked "+LastScore+" pins"+(witnessed?"; creature witnessed":""));
    if(LastScore>0&&!ball.Harmful){IslandAudio.Cue(IslandSound.Discovery,Target,ball.GetEntityId().GetHashCode(),.8f,LastScore==6?1.3f:1);if(creatureShot){god.State.Experience(ball.StableId,.02f*LastScore);god.GetComponent<CreatureCuriosity>().NoticeResponse(CreatureAffect.Mood.Excited,2);}foreach(var p in people)if(p!=null&&!p.Active&&!p.GetComponent<VillagerExploration>().Active&&!p.GetComponent<CarryItem>().held&&Vector3.Distance(p.transform.position,Target)<nav.Scale*30&&nav.GetComponentInChildren<VillageResources>()?.ProvisionVisible(p,Target,root)==true)p.GetComponent<HumanoidMotion>()?.React();}
   }
  }else if(resetIn>0){resetIn-=dt;if(resetIn<=0&&!ball.GetComponent<CarryItem>().held&&!god.GetComponent<CreatureCuriosity>().HandlingBeachBall&&!ball.FetchReserved)ResetRack(true);else if(resetIn<=0)resetIn=1;}
  tint.SetColor("_Color",Active?new Color(.4f,.25f,.12f):new Color(.85f,.2f,.12f));bell.SetPropertyBlock(tint);
 }
 static int Bits(int mask){int count=0;for(int i=0;i<PinCount;i++)if((mask&(1<<i))!=0)count++;return count;}
 public bool TryReset(){if(!Available||Active||god.Season.paused||ball.GetComponent<CarryItem>().held||god.GetComponent<CreatureCuriosity>().HandlingBeachBall)return false;ResetRack(true);IslandAudio.Cue(IslandSound.Chime,Start,ball.GetEntityId().GetHashCode());return true;}
 public void CancelAttempt(){LastCreatureOutcome=false;if(Active){Active=false;resetIn=8;}}
 public void ResetRack(bool returnBall){if(!Available)return;Active=false;LastCreatureOutcome=false;LastScore=FallenMask=0;resetIn=0;if(returnBall)ball.ReturnHome();for(int i=0;i<PinCount;i++){Pins[i].position=origins[i];Pins[i].rotation=Quaternion.identity;Pins[i].linearVelocity=Pins[i].angularVelocity=Vector3.zero;Pins[i].WakeUp();views[i].SetPositionAndRotation(nav.ToWorld(origins[i]),Quaternion.identity);lights[i].enabled=false;}}
 public void Dispose(){foreach(var mesh in meshes)if(mesh!=null)UnityEngine.Object.Destroy(mesh);foreach(var m in materials)if(m!=null)UnityEngine.Object.Destroy(m);if(root!=null)UnityEngine.Object.Destroy(root.gameObject);}
}

}
