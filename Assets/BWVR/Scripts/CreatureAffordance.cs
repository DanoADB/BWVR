using UnityEngine;
namespace BWVR {
public sealed class CreatureAffordance:MonoBehaviour {
 public enum Kind { Roll,Turn,Chime,Food,Villager }
 public Kind kind;public int discoveries;public float cooldown;
 public CarryItem Item=>GetComponent<CarryItem>();
 public bool Available=>isActiveAndEnabled&&Item!=null&&!Item.held&&Time.time>=cooldown&&(GetComponent<ConstructionBlock>()==null||GetComponent<ConstructionBlock>().CanBuild)&&(GetComponent<VillagerSocial>()==null||GetComponent<VillagerSocial>().Willing);
 public void Discovered(){discoveries++;cooldown=Time.time+18;}
}
}
