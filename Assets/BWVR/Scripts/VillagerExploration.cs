using System.Linq;
using UnityEngine;
namespace BWVR {
// An independent leisure intention: inspect, handle a play tool or taste a berry.
public sealed partial class VillagerExploration:MonoBehaviour {
 public enum Phase{Idle,Approach,Inspect,Enjoy,Celebrate,ApproachBridge,CrossBridge,FetchTribute,CarryTribute,ApproachStructure,UseStructure,NeedApproach,ShowNeed,Beachball,WatchBall,RepairFetch,RepairCarry,RepairPlace,GatheringGuest,ComfortNeighbor}
 public Phase Current{get;private set;}public bool Active=>Current!=Phase.Idle;public int discoveries{get;private set;}
 IslandDiscoveries world;VillagerSocial social;VillagerRoutine routine;IslandNavigator nav;HumanoidMotion motion;CarryItem actor;DiscoveryObject target;Vector3 original;Quaternion rotation;float cooldown,clock,elapsed;int identity;
 CreatureBridge bridge;Vector3 nearBank,farBank;
 VillageTributes tribute;bool ownsTribute;
 public bool TryTribute(VillageTributes village){if(Active||actor.held||social.Active||(GetComponent<VillagerBlockPlay>()?.Active??false)||village==null||!village.Memory.pending||village.Basket.held)return false;tribute=village;routine.ResetTask();nav.ResetRoute();clock=elapsed=0;Current=Phase.FetchTribute;motion?.LookAt(village.Basket.transform);return true;}
 public bool TryBridge(CreatureBridge crossing){if(Active||actor.held||social.Active||(GetComponent<VillagerBlockPlay>()?.Active??false)||crossing==null||!crossing.Walkable)return false;bridge=crossing;nearBank=bridge.NearestBank(transform.position);farBank=nearBank==bridge.BankA?bridge.BankB:bridge.BankA;routine.ResetTask();nav.ResetRoute();clock=elapsed=0;Current=Phase.ApproachBridge;return true;}
 public void Initialize(IslandDiscoveries island,int index){world=island;identity=index+1;social=GetComponent<VillagerSocial>();routine=GetComponent<VillagerRoutine>();nav=GetComponent<IslandNavigator>();motion=GetComponent<HumanoidMotion>();actor=GetComponent<CarryItem>();cooldown=10+index*11;}
 public bool TryBegin(DiscoveryObject item){
  if(Active||actor.held||social.Active||(GetComponent<VillagerBlockPlay>()?.Active??false)||item==null||!item.Available||item.GetComponent<CarryItem>().held||item.GetComponent<CreatureAffordance>()?.host!=null)return false;
  if(!world.Reachable(transform.position,item.transform.position,nav.profile))return false;
  target=item;target.Owner=this;routine.ResetTask();nav.ResetRoute();clock=elapsed=0;Current=Phase.Approach;motion?.LookAt(target.transform);return true;
 }
 public bool Tick(float dt,bool paused){
  if(paused||dt<=0)return Active;
  if(actor.held){ResetTask();return false;}
  if(Current==Phase.ComfortNeighbor)return TickComfort(dt);
  if(Current==Phase.GatheringGuest)return TickGathering(dt);
  if(Current>=Phase.RepairFetch)return TickRepair(dt);
  if(Current==Phase.WatchBall)return TickWatching(dt);
  if(Current==Phase.Beachball)return TickBall(dt);
  if(Current>=Phase.ApproachStructure)return TickStructure(dt);
  if(Current==Phase.FetchTribute||Current==Phase.CarryTribute){elapsed+=dt;if(tribute==null||elapsed>180){ResetTask();return false;}var basket=tribute.Basket;if(Current==Phase.FetchTribute){if(basket.held){ResetTask();return false;}if(nav.MoveTo(basket.transform.position,transform.lossyScale.y*1.6f,dt)){basket.held=true;ownsTribute=true;basket.GetComponent<Collider>().enabled=false;motion?.Carry(basket.transform);Current=Phase.CarryTribute;nav.ResetRoute();}}else if(nav.MoveTo(tribute.Altar,transform.lossyScale.y*1.6f,dt)){motion?.Carry(null);basket.transform.position=tribute.Altar+Vector3.up*nav.world.Scale;basket.held=false;ownsTribute=false;basket.GetComponent<Collider>().enabled=true;tribute.Presented(this);ResetTask();Current=Phase.Celebrate;motion?.React();}if(nav.Blocked&&elapsed>5)ResetTask();return Active;}
  if(!Active){cooldown-=dt;if(cooldown<=0){if(ChooseComfort()||ChooseVillageBuilding()||ChooseGatheringNeed()||ChooseRepair()||ChooseWatching()||ChoosePeerPlay()||ChooseBall()||ChooseStructure())return true;if(social.Memory.bridgeCrossings==0){if(bridge==null)bridge=nav.world.GetComponentInChildren<CreatureBridge>();if(TryBridge(bridge))return true;}var choice=world.points.Where(p=>p.Unseen(identity)).SelectMany(p=>p.objects).Where(o=>o.Available&&!o.GetComponent<CarryItem>().held&&o.GetComponent<CreatureAffordance>()?.host==null).OrderBy(o=>(o.transform.position-transform.position).sqrMagnitude).FirstOrDefault();if(choice==null||!TryBegin(choice))cooldown=12;}return Active;}
  if(Current==Phase.ApproachBridge||Current==Phase.CrossBridge){elapsed+=dt;if(bridge==null||!bridge.Walkable||elapsed>240){ResetTask();return false;}if(nav.MoveTo(Current==Phase.ApproachBridge?nearBank:farBank,transform.lossyScale.y*1.6f,dt)){if(Current==Phase.ApproachBridge){Current=Phase.CrossBridge;nav.ResetRoute();}else{RememberStructure(bridge.Site);social.Memory.bridgeCrossings=System.Math.Min(1000000,social.Memory.bridgeCrossings+1);if(bridge.Site.blocks.Any(b=>b.creatureBuilt))social.Memory.trust=Mathf.Clamp01(social.Memory.trust+.03f);ResetTask();Current=Phase.Celebrate;motion?.React();}}else if(nav.Blocked&&elapsed>5)ResetTask();return Active;}
  clock+=dt;elapsed+=dt;if(Current==Phase.Celebrate){if(clock>2)ResetTask();return Active;}if(target==null||target.Owner!=this||elapsed>180){ResetTask();return false;}
  if(Current==Phase.Approach){if(nav.MoveTo(target.transform.position,transform.lossyScale.y*1.6f,dt)){Current=Phase.Inspect;clock=0;motion?.Activity(6);}else if(nav.Blocked&&elapsed>4)ResetTask();}
  else if(Current==Phase.Inspect&&clock>1.5f){var item=target.GetComponent<CarryItem>();if(item.held){ResetTask();return false;}original=target.transform.position;rotation=target.transform.rotation;item.held=true;target.GetComponent<Collider>().enabled=false;motion?.Activity(0);motion?.Carry(target.transform,target.treat);Current=Phase.Enjoy;clock=0;}
  else if(Current==Phase.Enjoy){if(!target.treat)motion?.Playful(clock);if(clock>2.5f){if(target.treat)target.Eat();if(target.point.Discover(identity)){discoveries++;social.Memory.discoveries=System.Math.Min(1000000,social.Memory.discoveries+1);social.Memory.boldness=Mathf.Clamp01(social.Memory.boldness+.025f);}ResetTask();Current=Phase.Celebrate;clock=elapsed=0;motion?.React();}}
  return Active;
 }
 public void ResetTask(){if(!Active)return;ReleaseComfort();ReleaseGathering();ReleaseRepair();ReleaseWatching();ReleaseBall();ReleaseStructure();motion?.Carry(null);motion?.Playful(0);motion?.LookAt(null);motion?.Activity(0);if(tribute!=null){if(ownsTribute){tribute.Basket.held=false;tribute.Basket.transform.position=tribute.Basket.home;tribute.Basket.GetComponent<Collider>().enabled=true;}tribute.Cancel(this);tribute=null;ownsTribute=false;}if(target!=null){if(Current==Phase.Enjoy){target.transform.SetPositionAndRotation(original,rotation);target.GetComponent<CarryItem>().held=false;}if(target.Owner==this)target.Owner=null;target.Refresh();}target=null;Current=Phase.Idle;clock=elapsed=0;cooldown=35;nav?.ResetRoute();routine?.ResetTask();}
 void OnDisable(){ResetTask();}
}
}
