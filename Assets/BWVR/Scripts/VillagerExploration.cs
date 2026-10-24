using System.Linq;
using UnityEngine;
namespace BWVR {
// An independent leisure intention: inspect, handle a play tool or taste a berry.
public sealed class VillagerExploration:MonoBehaviour {
 public enum Phase{Idle,Approach,Inspect,Enjoy,Celebrate}
 public Phase Current{get;private set;}public bool Active=>Current!=Phase.Idle;public int discoveries{get;private set;}
 IslandDiscoveries world;VillagerSocial social;VillagerRoutine routine;IslandNavigator nav;HumanoidMotion motion;CarryItem actor;DiscoveryObject target;Vector3 original;Quaternion rotation;float cooldown,clock,elapsed;int identity;
 public void Initialize(IslandDiscoveries island,int index){world=island;identity=index+1;social=GetComponent<VillagerSocial>();routine=GetComponent<VillagerRoutine>();nav=GetComponent<IslandNavigator>();motion=GetComponent<HumanoidMotion>();actor=GetComponent<CarryItem>();cooldown=10+index*11;}
 public bool TryBegin(DiscoveryObject item){
  if(Active||actor.held||social.Active||(GetComponent<VillagerBlockPlay>()?.Active??false)||item==null||!item.Available||item.GetComponent<CarryItem>().held)return false;
  if(!world.Reachable(transform.position,item.transform.position,nav.profile))return false;
  target=item;target.Owner=this;routine.ResetTask();nav.ResetRoute();clock=elapsed=0;Current=Phase.Approach;motion?.LookAt(target.transform);return true;
 }
 public bool Tick(float dt,bool paused){
  if(paused||dt<=0)return Active;
  if(actor.held){ResetTask();return false;}
  if(!Active){cooldown-=dt;if(cooldown<=0){var choice=world.points.Where(p=>p.Unseen(identity)).SelectMany(p=>p.objects).Where(o=>o.Available&&!o.GetComponent<CarryItem>().held).OrderBy(o=>(o.transform.position-transform.position).sqrMagnitude).FirstOrDefault();if(choice==null||!TryBegin(choice))cooldown=12;}return Active;}
  clock+=dt;elapsed+=dt;if(Current==Phase.Celebrate){if(clock>2)ResetTask();return Active;}if(target==null||target.Owner!=this||elapsed>180){ResetTask();return false;}
  if(Current==Phase.Approach){if(nav.MoveTo(target.transform.position,transform.lossyScale.y*1.6f,dt)){Current=Phase.Inspect;clock=0;motion?.Activity(6);}else if(nav.Blocked&&elapsed>4)ResetTask();}
  else if(Current==Phase.Inspect&&clock>1.5f){var item=target.GetComponent<CarryItem>();if(item.held){ResetTask();return false;}original=target.transform.position;rotation=target.transform.rotation;item.held=true;target.GetComponent<Collider>().enabled=false;motion?.Activity(0);motion?.Carry(target.transform,target.treat);Current=Phase.Enjoy;clock=0;}
  else if(Current==Phase.Enjoy){if(!target.treat)motion?.Playful(clock);if(clock>2.5f){if(target.treat)target.Eat();if(target.point.Discover(identity)){discoveries++;social.Memory.discoveries=System.Math.Min(1000000,social.Memory.discoveries+1);social.Memory.boldness=Mathf.Clamp01(social.Memory.boldness+.025f);}ResetTask();Current=Phase.Celebrate;clock=elapsed=0;motion?.React();}}
  return Active;
 }
 public void ResetTask(){if(!Active)return;motion?.Carry(null);motion?.Playful(0);motion?.LookAt(null);motion?.Activity(0);if(target!=null){if(Current==Phase.Enjoy){target.transform.SetPositionAndRotation(original,rotation);target.GetComponent<CarryItem>().held=false;}if(target.Owner==this)target.Owner=null;target.Refresh();}target=null;Current=Phase.Idle;clock=elapsed=0;cooldown=35;nav?.ResetRoute();routine?.ResetTask();}
 void OnDisable(){ResetTask();}
}
}
