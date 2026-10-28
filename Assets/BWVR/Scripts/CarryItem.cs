using UnityEngine;
namespace BWVR {
public enum Offering { Wood, Food, Water, Villager, Toy, Creature, Tree }
public sealed class CarryItem:MonoBehaviour {public Offering kind;public bool held;public Vector3 home;public bool CanGrab=>GetComponent<PuzzlePad>()==null&&GetComponent<ThrowableTree>()==null&&!held&&(GetComponent<DiscoveryObject>()==null||GetComponent<DiscoveryObject>().Available)&&(GetComponent<ConstructionBlock>()==null||GetComponent<ConstructionBlock>().VillagerOwner==null);}
}
