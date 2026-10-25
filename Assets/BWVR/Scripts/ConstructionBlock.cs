using UnityEngine;
namespace BWVR {
public sealed class ConstructionBlock:MonoBehaviour {
 public ConstructionSite Site{get;private set;}public Rigidbody Body{get;private set;}public ConfigurableJoint Connection{get;internal set;}
 public CarryItem Item{get;private set;}public Vector3 size;public Vector3 spawn;public bool Attached=>Connection!=null;
 public ConstructionShape Shape;public bool RecipePiece=>Shape==ConstructionShape.Brick||Shape==ConstructionShape.Beam;
 public Bounds PhysicsBounds{get{var x=Body.rotation*Vector3.right*size.x*.5f;var y=Body.rotation*Vector3.up*size.y*.5f;var z=Body.rotation*Vector3.forward*size.z*.5f;return new Bounds(Body.position,new Vector3(Mathf.Abs(x.x)+Mathf.Abs(y.x)+Mathf.Abs(z.x),Mathf.Abs(x.y)+Mathf.Abs(y.y)+Mathf.Abs(z.y),Mathf.Abs(x.z)+Mathf.Abs(y.z)+Mathf.Abs(z.z))*2);}}
 public bool creatureBuilt;public VillagerBlockPlay VillagerOwner{get;internal set;}
 internal bool wasHeld;
 public void Initialize(ConstructionSite site,Rigidbody body,Vector3 dimensions){Site=site;Body=body;size=dimensions;Item=GetComponent<CarryItem>();spawn=body.position;}
 public void Grab(){Site.Detach(this);wasHeld=true;Body.isKinematic=true;Body.detectCollisions=false;}
 public bool CanBuild=>Site.Repairer==null&&VillagerOwner==null&&!Attached&&Site.TryPlan(this,out _,out _);
 public void CancelHold(){Item.held=false;wasHeld=false;Site.ReturnLoose(this);}
}
}
