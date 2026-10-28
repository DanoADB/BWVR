using System;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace BWVR {
// Three movable goals reuse the ball's game-unit PhysX scene. No trigger callbacks or scene scans.
public sealed class GoalCourse {
 public readonly BallGoal[] Gates=new BallGoal[3];
 public GoalTurns Turns{get;private set;}
 readonly Renderer[,] turnLights=new Renderer[3,8];readonly Transform[] nets=new Transform[3];readonly Rigidbody[] netBodies=new Rigidbody[3];
 public bool Active=>tracking&&pending!=0;
 public bool LastCreatureGoal{get;private set;}
 readonly BeachBall ball;readonly CreatureHybrid god;readonly IslandNavigation nav;
 readonly VillagerSocial[] people;readonly Quaternion[] homeRotations=new Quaternion[3];
 readonly Transform root;readonly Material[] materials;readonly Rigidbody[,] posts=new Rigidbody[3,3];
 readonly Vector3[] homes=new Vector3[3],positions=new Vector3[3];readonly Quaternion[] rotations=new Quaternion[3];
 readonly float[] still=new float[3],flash=new float[3];readonly bool[] moved=new bool[3];readonly Renderer[] badges=new Renderer[3];
 readonly MaterialPropertyBlock tint=new MaterialPropertyBlock();
 Vector3 previous;float age,settled,demonstratedUntil;int pending,witnessed;bool tracking,byCreature,restoring;
 GoalMemory Memory=>god.Season.WorldPlay.Goals;
 public GoalCourse(BeachBall sphere,CreatureHybrid creature,IslandNavigation navigation,Scene physics){
  ball=sphere;god=creature;nav=navigation;people=god.GetComponentInParent<QuestSlice>().GetComponentsInChildren<VillagerSocial>();root=new GameObject("Movable beachball goals").transform;root.SetParent(ball.transform.parent,false);Turns=new GoalTurns(this,ball,god,nav);
  materials=new[]{Mat(new Color(.55f,.29f,.12f)),Mat(new Color(.16f,.65f,.64f)),Mat(new Color(.93f,.52f,.18f)),Mat(new Color(.63f,.39f,.78f))};
  var lane=ball.Bowling;var origin=lane!=null&&lane.Available?lane.Start:ball.GetComponent<CarryItem>().home;var forward=lane!=null&&lane.Available?lane.Forward:Vector3.forward;var right=Vector3.Cross(Vector3.up,forward);
  for(int i=0;i<3;i++){
   var go=new GameObject("Portable goal "+(i+1));go.transform.SetParent(root,false);go.transform.localScale=Vector3.one*nav.Scale;
   var gate=go.AddComponent<BallGoal>();gate.course=this;gate.index=i;Gates[i]=gate;go.AddComponent<CarryItem>().kind=Offering.Toy;
   // The coloured centre handle is the existing carry ray's hit surface.
   var handle=go.AddComponent<BoxCollider>();handle.center=new Vector3(0,17,0);handle.size=new Vector3(24,2,3);
   Shape(go.transform,"Left post",new Vector3(-11,8,0),new Vector3(2,16,2),materials[0]);
   Shape(go.transform,"Right post",new Vector3(11,8,0),new Vector3(2,16,2),materials[0]);
   Shape(go.transform,"Colour crossbar — grab here",new Vector3(0,17,0),new Vector3(24,2,3),materials[i+1]);
   Shape(go.transform,"Left foot",new Vector3(-11,.5f,0),new Vector3(4,1,6),materials[0]);
   Shape(go.transform,"Right foot",new Vector3(11,.5f,0),new Vector3(4,1,6),materials[0]);
   badges[i]=Shape(go.transform,"Goal celebration — invite turns",new Vector3(0,19,0),new Vector3(2,2,.8f),materials[i+1]).GetComponent<Renderer>();
   var button=badges[i].gameObject.AddComponent<BoxCollider>();button.size=Vector3.one*1.4f;var key=badges[i].gameObject.AddComponent<GoalChallengeKey>();key.game=Turns;key.index=i;
   for(int light=0;light<8;light++){bool player=light<4;int bead=light%4;turnLights[i,light]=Shape(go.transform,(player?"Player":"Creature")+" turn bead "+bead,new Vector3((player?-1:1)*(2+bead*1.6f),19,0),Vector3.one*(bead==0?1.4f:.85f),materials[player?1:2]).GetComponent<Renderer>();}
   for(int j=0;j<3;j++){var proxy=new GameObject("Goal post physics "+i+"-"+j);SceneManager.MoveGameObjectToScene(proxy,physics);var box=proxy.AddComponent<BoxCollider>();box.size=j==2?new Vector3(24,2,3):new Vector3(2,16,2);box.contactOffset=.01f;var body=proxy.AddComponent<Rigidbody>();body.isKinematic=true;posts[i,j]=body;}
   nets[i]=new GameObject("Shared game backstop net").transform;nets[i].SetParent(go.transform,false);
   for(int strand=0;strand<=8;strand++)Shape(nets[i],"Vertical rope",new Vector3(-12+strand*3,8,0),new Vector3(.15f,16,.15f),materials[0]);
   for(int strand=0;strand<=5;strand++)Shape(nets[i],"Horizontal rope",new Vector3(0,strand*3.2f,0),new Vector3(24,.15f,.15f),materials[0]);
   var net=new GameObject("Shared goal net physics "+i);SceneManager.MoveGameObjectToScene(net,physics);net.AddComponent<BoxCollider>().size=new Vector3(24,16,.25f);netBodies[i]=net.AddComponent<Rigidbody>();netBodies[i].isKinematic=true;
   bool placed=false;for(int t=0;t<32&&!placed;t++){var want=origin+(right*(38+i*30)+forward*(20+t*3))*nav.Scale;go.transform.rotation=Quaternion.LookRotation(forward);placed=Place(gate,want,false);}
   if(!placed){go.SetActive(false);Debug.LogWarning("BWVR_GOAL_UNAVAILABLE: no safe site for goal "+i);}homes[i]=go.transform.position;homeRotations[i]=go.transform.rotation;go.GetComponent<CarryItem>().home=homes[i];
  }
  ResetWorld();
 }
 static Material Mat(Color color)=>new Material(Shader.Find("BWVR/GameSurface")){color=color,enableInstancing=true};
 static GameObject Shape(Transform parent,string name,Vector3 at,Vector3 size,Material mat){var go=GameObject.CreatePrimitive(PrimitiveType.Cube);go.name=name;go.transform.SetParent(parent,false);go.transform.localPosition=at;go.transform.localScale=size;go.GetComponent<Renderer>().sharedMaterial=mat;var c=go.GetComponent<Collider>();c.enabled=false;UnityEngine.Object.Destroy(c);return go;}
 public bool Place(BallGoal gate,Vector3 point,bool capture=true){
  if(!restoring&&god.Season.paused||!nav.Sample(point,nav.villager,1,out var safe))return false;safe=nav.ProjectFoot(safe,nav.villager);var yaw=Quaternion.Euler(0,gate.transform.eulerAngles.y,0);
  foreach(float side in new[]{-11f,11f}){var foot=safe+yaw*Vector3.right*(side*nav.Scale);if(!nav.Sample(foot,nav.villager,1,out var edge)||Mathf.Abs(nav.ProjectFoot(edge,nav.villager).y-safe.y)>nav.Scale*1.5f)return false;}
  foreach(var other in Gates)if(!restoring&&other!=null&&other!=gate&&other.gameObject.activeInHierarchy&&Vector3.Distance(other.transform.position,safe)<nav.Scale*25)return false;
  gate.transform.SetPositionAndRotation(safe,yaw);gate.GetComponent<CarryItem>().home=safe;Invalidate(gate.index);Sync(gate.index);if(capture)Capture();return true;
 }
 void Invalidate(int i){Turns?.GateMoved(i);pending&=~(1<<i);witnessed&=~(1<<i);still[i]=0;flash[i]=0;}
 void Sync(int i){var gate=Gates[i];positions[i]=gate.transform.position;rotations[i]=gate.transform.rotation;bool enabled=gate.gameObject.activeInHierarchy&&!gate.GetComponent<CarryItem>().held;
  for(int j=0;j<3;j++){var body=posts[i,j];body.GetComponent<Collider>().enabled=enabled;body.rotation=gate.transform.rotation;body.position=nav.ToNav(gate.transform.position)+gate.transform.rotation*(j==2?new Vector3(0,17,0):new Vector3(j==0?-11:11,8,0));}
  bool catchBall=enabled&&Turns.Active&&Turns.GateIndex==i&&Turns.HasOrigin;float side=Vector3.Dot(Turns.Origin-gate.transform.position,gate.transform.forward)<0?1:-1;nets[i].gameObject.SetActive(catchBall);nets[i].localPosition=Vector3.forward*(side*9);netBodies[i].GetComponent<Collider>().enabled=catchBall;netBodies[i].rotation=gate.transform.rotation;netBodies[i].position=nav.ToNav(gate.transform.position)+gate.transform.rotation*new Vector3(0,8,side*9);
 }
 public void BeforeStep(float dt){for(int i=0;i<3;i++){var gate=Gates[i];bool changed=(gate.transform.position-positions[i]).sqrMagnitude>nav.Scale*nav.Scale*.0001f||Quaternion.Angle(rotations[i],gate.transform.rotation)>.1f||gate.GetComponent<CarryItem>().held;moved[i]=changed;if(changed)Invalidate(i);else still[i]+=dt;Sync(i);}}
 // Both directions count, but the complete sphere must fit through a stationary opening.
 public static bool Crosses(Vector3 before,Vector3 after,float radius){if(before.z*after.z>0||Mathf.Abs(after.z-before.z)<.0001f)return false;float t=-before.z/(after.z-before.z);if(t<=0||t>1)return false;var at=Vector3.Lerp(before,after,t);return Mathf.Abs(at.x)+radius<10&&at.y-radius>=0&&at.y+radius<16;}
 public void Begin(bool creature){Cancel();tracking=true;byCreature=creature;previous=ball.Body.position;Turns.Launch(creature,previous);}
 public void AfterStep(float dt){if(!tracking)return;var now=ball.Body.position;age+=dt;
  if(ball.GetComponent<CarryItem>().held||ball.Harmful){Turns.Cancel("Ball pickup or harmful shot");Cancel();return;}
  for(int i=0;i<3;i++){if(moved[i]||still[i]<.5f||!Gates[i].gameObject.activeInHierarchy||(pending&(1<<i))!=0)continue;var inverse=Quaternion.Inverse(Gates[i].transform.rotation);var origin=nav.ToNav(Gates[i].transform.position);if(!Crosses(inverse*(previous-origin),inverse*(now-origin),ball.Radius))continue;pending|=1<<i;if(!byCreature&&god.CanSeeCue(Gates[i].GetComponent<HybridTarget>(),false,true))witnessed|=1<<i;}
  previous=now;settled=ball.Body.linearVelocity.sqrMagnitude<1?settled+dt:0;if(age>=12||age>=1.5f&&settled>=1.2f)Finish();
 }
 public void Advance(float dt){Turns.Advance(dt);for(int i=0;i<3;i++){flash[i]=Mathf.Max(0,flash[i]-dt);tint.SetColor("_Color",flash[i]>0?Color.white:materials[i+1].color);badges[i].SetPropertyBlock(tint);PaintTurns(i);}}
 void Finish(){if(pending!=0&&!ball.Harmful){int count=Bits(pending);ball.CancelFetch();Memory.Complete(byCreature,count,false);if(!byCreature){int seen=Bits(witnessed);Memory.observedGoals=Math.Min(Memory.playerGoals,Memory.observedGoals+seen);if(seen>0){demonstratedUntil=god.State.time+60;god.GetComponent<CreatureCuriosity>().NoticeAttention();}}
   LastCreatureGoal=byCreature;for(int i=0;i<3;i++)if((pending&(1<<i))!=0){flash[i]=2;IslandAudio.Cue(IslandSound.Discovery,Gates[i].transform.position,ball.GetEntityId().GetHashCode(),.7f,1.1f+i*.1f);}
   if(byCreature){god.State.Experience(ball.StableId,.06f);god.GetComponent<CreatureCuriosity>().NoticeResponse(CreatureAffect.Mood.Excited,2);}
   god.Journal.Add(god.State.time,"ball goal",CreatureAction.PlayWithToy,ball.StableId,(byCreature?"Creature":"Player")+" passed "+count+" stationary openings without harm");}
  int outcome=pending;tracking=false;pending=witnessed=0;Turns.Outcome(byCreature,outcome);
 }
 static int Bits(int mask){int n=0;for(int i=0;i<3;i++)if((mask&(1<<i))!=0)n++;return n;}
 public bool TryAim(Vector3 actor,out Vector3 direction)=>FindAim(actor,null,true,out direction,out _);
 public BallGoal Plan(Vector3 actor){FindAim(actor,Turns.InvitedGoal,Turns.InvitedGoal==null,out _,out var goal);return goal;}
 public bool AimAt(BallGoal goal,Vector3 actor,out Vector3 direction){if(goal==null){direction=Vector3.zero;return false;}bool found=FindAim(actor,goal,false,out direction,out _);if(found)direction=Quaternion.Euler(0,CreatureDecisionModel.Variation(god.State.seed,god.State.decisions,CreatureAction.PlayWithToy,"goal-aim")*Mathf.Lerp(5,1,Memory.practice),0)*direction;return found;}
 bool FindAim(Vector3 actor,BallGoal only,bool sight,out Vector3 direction,out BallGoal selected){direction=Vector3.zero;selected=null;if(Memory.observedGoals==0||ball.FetchReserved||ball.Inviting||ball.PeerGame||ball.Harmful)return false;float nearest=float.MaxValue;
  foreach(var gate in Gates){if(only!=null&&gate!=only||!gate.gameObject.activeInHierarchy||gate.GetComponent<CarryItem>().held||still[gate.index]<.5f||sight&&!god.CanSeeCue(gate.GetComponent<HybridTarget>(),false,true))continue;var local=Quaternion.Inverse(gate.transform.rotation)*(actor-gate.transform.position)/nav.Scale;float distance=new Vector2(local.x,local.z).magnitude;if(distance<16||distance>55||Mathf.Abs(local.z)<distance*.85f||distance>=nearest||!nav.SegmentClear(actor,gate.transform.position,nav.creature))continue;
   var aim=gate.transform.position-actor;aim.y=0;bool safe=true;foreach(var p in people){var delta=p.transform.position-actor;delta.y=0;float along=Vector3.Dot(delta,aim.normalized)/nav.Scale;if(!p.GetComponent<CarryItem>().held&&along>0&&along<distance+10&&(delta-aim.normalized*Vector3.Dot(delta,aim.normalized)).magnitude/nav.Scale<ball.Radius+4){safe=false;break;}}if(!safe)continue;nearest=distance;direction=aim.normalized;selected=gate;
  }return nearest<float.MaxValue;
 }
 public float DemonstrationBias=>Mathf.Clamp01((demonstratedUntil-god.State.time)/60)*.7f;
 public void Capture(){var layout=new GoalLayout[3];for(int i=0;i<3;i++){var gate=Gates[i];var at=gate.GetComponent<CarryItem>().held?gate.GetComponent<CarryItem>().home:gate.transform.position;layout[i]=new GoalLayout{point=nav.ToNav(at),yaw=gate.transform.eulerAngles.y};}Memory.layout=layout;}
 public void Cancel(){tracking=false;pending=witnessed=0;age=settled=0;LastCreatureGoal=false;}
 public void ResetWorld(){Turns.Cancel("World reset");Cancel();demonstratedUntil=0;var layout=Memory.layout;restoring=true;try{for(int i=0;i<3;i++){var gate=Gates[i];var at=layout.Length==3?nav.ToWorld(layout[i].point):homes[i];gate.GetComponent<CarryItem>().held=false;gate.GetComponent<Collider>().enabled=true;gate.transform.rotation=layout.Length==3?Quaternion.Euler(0,layout[i].yaw,0):homeRotations[i];if(!Place(gate,at,false)){gate.transform.SetPositionAndRotation(homes[i],homeRotations[i]);gate.GetComponent<CarryItem>().home=homes[i];}var target=gate.GetComponent<HybridTarget>()??HybridTarget.Bind(gate.gameObject,"ball-goal-"+i,HybridTargetKind.Object);god.Register(target);Invalidate(i);Sync(i);}}finally{restoring=false;}}
 void PaintTurns(int i){bool chosen=Turns.GateIndex==i;for(int j=0;j<8;j++){var view=turnLights[i,j];view.enabled=chosen&&(Turns.Active||Turns.Round==3);if(!view.enabled)continue;bool player=j<4;int bead=j%4;bool on=bead==0?(player?Turns.PlayerOwnsTurn:Turns.OffersCreature||Turns.Current==GoalTurns.Phase.CreatureFlight):(((player?Turns.PlayerHits:Turns.CreatureHits)&(1<<(bead-1)))!=0);tint.SetColor("_Color",on?(player?new Color(.2f,.8f,1):new Color(1,.68f,.16f)):new Color(.18f,.17f,.14f));view.SetPropertyBlock(tint);}}
 public void Dispose(){foreach(var mat in materials)if(mat!=null)UnityEngine.Object.Destroy(mat);if(root!=null)UnityEngine.Object.Destroy(root.gameObject);}
}
}
