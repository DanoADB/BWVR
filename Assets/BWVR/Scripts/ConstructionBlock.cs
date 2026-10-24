using UnityEngine;
namespace BWVR {
public sealed class ConstructionBlock:MonoBehaviour {
 public ConstructionSite Site{get;private set;}public Rigidbody Body{get;private set;}public ConfigurableJoint Connection{get;internal set;}
 public CarryItem Item{get;private set;}public Vector3 size;public Vector3 spawn;public bool Attached=>Connection!=null;
 public bool creatureBuilt;public VillagerBlockPlay VillagerOwner{get;internal set;}
 internal bool wasHeld;
 public void Initialize(ConstructionSite site,Rigidbody body,Vector3 dimensions){Site=site;Body=body;size=dimensions;Item=GetComponent<CarryItem>();spawn=body.position;}
 public void Grab(){Site.Detach(this);wasHeld=true;Body.isKinematic=true;Body.detectCollisions=false;}
 public bool CanBuild=>VillagerOwner==null&&!Attached&&Site.TryPlan(this,out _,out _);
 public void CancelHold(){Item.held=false;wasHeld=false;Site.ReturnLoose(this);}
}
}
