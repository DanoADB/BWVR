using UnityEngine;
namespace BWVR {
public enum Offering { Wood, Food, Water, Villager, Toy }
public sealed class CarryItem:MonoBehaviour {public Offering kind;public bool held;public Vector3 home;public bool CanGrab=>!held&&(GetComponent<DiscoveryObject>()==null||GetComponent<DiscoveryObject>().Available)&&(GetComponent<ConstructionBlock>()==null||GetComponent<ConstructionBlock>().VillagerOwner==null);}
}
