using UnityEngine;
namespace BWVR {
public sealed class CreatureAffordance:MonoBehaviour {
 public enum Kind { Roll,Turn,Chime,Food,Villager,Tree }
 public Kind kind;public int discoveries;public float cooldown;
 public VillagerSocial host;public bool SharedReady=>host!=null&&host.Ready&&host.Offering==this;
 public System.Func<float> simulationTime;public System.Action<float> rememberCooldown;
 public float Now=>simulationTime!=null?simulationTime():Time.time;
 public void SetCooldown(float seconds){cooldown=Now+seconds;rememberCooldown?.Invoke(cooldown);}
 public CarryItem Item=>GetComponent<CarryItem>();
 public bool Available=>(GetComponent<PlayerRhythm>()==null||!GetComponent<PlayerRhythm>().Reserved)&&(GetComponent<ThrowableTree>()==null||GetComponent<ThrowableTree>().Available)&&(GetComponent<BeachBall>()==null||(GetComponent<BeachBall>().Bowling?.Active!=true&&GetComponent<BeachBall>().Goals?.Active!=true&&GetComponent<BeachBall>().Goals?.Turns.PlayerOwnsTurn!=true&&!GetComponent<BeachBall>().PeerGame&&(!GetComponent<BeachBall>().FetchReserved||GetComponent<BeachBall>().FetchReady)))&&isActiveAndEnabled&&Item!=null&&!Item.held&&(host==null||SharedReady)&&(GetComponent<VillageTributeProp>()==null||GetComponent<VillageTributeProp>().Ready)&&Now>=cooldown&&(GetComponent<DiscoveryObject>()==null||GetComponent<DiscoveryObject>().Available)&&(GetComponent<ConstructionBlock>()==null||GetComponent<ConstructionBlock>().CanBuild)&&(GetComponent<VillagerSocial>()==null||GetComponent<VillagerSocial>().Willing);
 public void Discovered(){discoveries++;SetCooldown(18);}
}
}
