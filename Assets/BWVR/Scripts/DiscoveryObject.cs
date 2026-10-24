using UnityEngine;
namespace BWVR {
public sealed class DiscoveryObject:MonoBehaviour {
 public DiscoveryPoint point;public bool treat;public VillagerExploration Owner;
 Renderer[] visuals;CarryItem item;Collider hit;
 void Awake(){visuals=GetComponentsInChildren<Renderer>();item=GetComponent<CarryItem>();hit=GetComponent<Collider>();}
 public bool Available=>Owner==null&&(!treat||point.Record.restock<=0);
 public void Eat(){if(treat)point.Record.restock=90;}
 public void Refresh(){bool visible=!treat||point.Record.restock<=0||item.held;foreach(var r in visuals)r.enabled=visible;hit.enabled=visible&&!item.held&&Owner==null;}
}
}
