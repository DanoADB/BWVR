using UnityEngine;
namespace BWVR {
public sealed class CreatureAffordance:MonoBehaviour {
 public enum Kind { Roll,Turn,Chime,Food,Villager }
 public Kind kind;public int discoveries;public float cooldown;
 public System.Func<float> simulationTime;public System.Action<float> rememberCooldown;
 public float Now=>simulationTime!=null?simulationTime():Time.time;
 public void SetCooldown(float seconds){cooldown=Now+seconds;rememberCooldown?.Invoke(cooldown);}
 public CarryItem Item=>GetComponent<CarryItem>();
 public bool Available=>isActiveAndEnabled&&Item!=null&&!Item.held&&Now>=cooldown&&(GetComponent<DiscoveryObject>()==null||GetComponent<DiscoveryObject>().Available)&&(GetComponent<ConstructionBlock>()==null||GetComponent<ConstructionBlock>().CanBuild)&&(GetComponent<VillagerSocial>()==null||GetComponent<VillagerSocial>().Willing);
 public void Discovered(){discoveries++;SetCooldown(18);}
}
}
