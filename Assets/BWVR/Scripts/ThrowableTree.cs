using UnityEngine;
using UnityEngine.AI;
namespace BWVR {
// Authored tree visuals stay intact; a pooled full-sized body supplies flight physics.
public sealed class ThrowableTree:MonoBehaviour {
 public TreePlayWorld World{get;private set;}public int Line{get;private set;}public bool Uprooted{get;private set;}public float Height{get;private set;}public float Radius{get;private set;}
 public CarryItem Item{get;private set;}public CreatureAffordance Affordance{get;private set;}public Vector3 Home{get;private set;}public Quaternion HomeRotation{get;private set;}
 public string StableId=>"tree-"+Line;public bool Available=>World.Available(this);public bool InFlight=>World.Active==this;
 public Vector3 RestPosition{get;private set;}Quaternion restRotation;GameObject obstacle;NavMeshObstacle carve;Collider[] colliders;bool[] colliderStates;
 public void Initialize(TreePlayWorld world,int line,float height){World=world;Line=line;Height=height;Radius=Mathf.Clamp(height*.07f,.45f,1.2f);Home=transform.position;HomeRotation=transform.rotation;
  Item=gameObject.AddComponent<CarryItem>();Item.kind=Offering.Tree;Item.home=Home;Affordance=gameObject.AddComponent<CreatureAffordance>();Affordance.kind=CreatureAffordance.Kind.Tree;
  var box=gameObject.AddComponent<BoxCollider>();var inverse=transform.worldToLocalMatrix;box.center=inverse.MultiplyPoint(transform.position+Vector3.up*Height*world.Unit*.5f);box.size=new Vector3(Radius*2,Height,Radius*2)*world.Unit;box.size=new Vector3(box.size.x/Mathf.Abs(transform.lossyScale.x),box.size.y/Mathf.Abs(transform.lossyScale.y),box.size.z/Mathf.Abs(transform.lossyScale.z));
  colliders=GetComponentsInChildren<Collider>();colliderStates=new bool[colliders.Length];for(int i=0;i<colliders.Length;i++)colliderStates[i]=colliders[i]!=null&&colliders[i].enabled;
  obstacle=new GameObject("Fallen tree route obstacle "+line);obstacle.transform.SetParent(world.transform,false);carve=obstacle.AddComponent<NavMeshObstacle>();carve.shape=NavMeshObstacleShape.Capsule;carve.height=height;carve.radius=Radius;carve.center=Vector3.up*height*.5f;carve.carving=true;carve.carveOnlyStationary=true;obstacle.SetActive(false);
 }
 public void Collisions(bool enabled){for(int i=0;i<colliders.Length;i++)if(colliders[i]!=null)colliders[i].enabled=enabled&&colliderStates[i];}
 public bool Lift(){if(!Available)return false;RestPosition=transform.position;restRotation=transform.rotation;if(!Uprooted){if(!World.Memory.Remember(Line,World.Navigation.ToNav(RestPosition),restRotation))return false;Uprooted=true;World.Navigation.SetTreeRooted(Line,false);World.Uprooted(this);}SetObstacle(false);Collisions(false);return true;}
 public void Landed(){RestPosition=transform.position;restRotation=transform.rotation;World.Memory.Remember(Line,World.Navigation.ToNav(RestPosition),restRotation);SetObstacle(true);}
 public void CancelCarry(){Item.held=false;transform.SetPositionAndRotation(RestPosition,restRotation);Collisions(true);if(Uprooted){World.Memory.Remember(Line,World.Navigation.ToNav(RestPosition),restRotation);SetObstacle(true);}}
 public void Restore(MovedTree saved){Item.held=false;Collisions(true);Uprooted=saved!=null;RestPosition=saved!=null?World.Navigation.ToWorld(saved.position):Home;restRotation=saved!=null?saved.rotation:HomeRotation;transform.SetPositionAndRotation(RestPosition,restRotation);World.Navigation.SetTreeRooted(Line,!Uprooted);SetObstacle(Uprooted);}
 public void SetObstacle(bool active){var at=World.Navigation.ToNav(transform.position);bool changed=obstacle.activeSelf!=active||(active&&((obstacle.transform.position-at).sqrMagnitude>.000001f||Quaternion.Angle(obstacle.transform.rotation,transform.rotation)>.1f));obstacle.transform.SetPositionAndRotation(at,transform.rotation);obstacle.SetActive(active);if(changed)World.Navigation.TreeObstacleChanged();}
 public Bounds ImpactBounds {get {var up=transform.up*Height*World.Unit;var width=Vector3.one*Radius*2*World.Unit;return new Bounds(transform.position+up*.5f,new Vector3(Mathf.Abs(up.x),Mathf.Abs(up.y),Mathf.Abs(up.z))+width);}}
 void OnDestroy(){if(obstacle!=null)Destroy(obstacle);}
}
}
