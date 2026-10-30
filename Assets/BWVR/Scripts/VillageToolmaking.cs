using System.Linq;
using UnityEngine;
namespace BWVR {
public sealed partial class VillageResources {
 public float ReservedToolWood=>Gatherers==null?0:Gatherers.Count(a=>a.Toolmaking)*2;
 public float ReservedToolResin=>Gatherers==null?0:Gatherers.Where(a=>a.Toolmaking).Sum(a=>ToolTechnologyState.ResinCost(a.PlannedTool));
 public float FreeResin=>Mathf.Max(0,State.resin-ReservedToolResin-DeliveryResin);
 public void ToolPractice(int identity,int work){if(Season.Tools.Practice(identity,work))ToolReceipt(identity,"tool-insight",work==1?VillageTool.BoundAxe:VillageTool.CultivatingHoe);}
 void ToolReceipt(int identity,string kind,VillageTool tool){if(kind=="tool-insight"||kind=="tool-observed"||kind=="tool-use-observed")Gatherers[identity].GetComponent<VillagerAffect>()?.Learned();var god=Navigation.GetComponentInChildren<CreatureHybrid>();god?.Journal.Add(Season.Hybrid.time,kind,CreatureAction.IdleOrObserve,"villager-"+identity,tool.ToString());}
 // Called only after a successful equipped harvest. Learning is knowledge, not inventory.
 public void ToolUsed(int maker,VillageTool tool){
  if(Season.paused||maker<0||maker>=Gatherers.Length||!Season.Tools.learners[maker].Owns(tool))return;
  var teacher=Gatherers[maker];var at=teacher.transform.position;
  for(int i=0;i<Gatherers.Length;i++){var student=Gatherers[i];if(i==maker||!student.CanWatchTools||Vector3.Distance(student.transform.position,at)>Navigation.Scale*18)continue;
   var toward=at-student.transform.position;toward.y=0;if(toward.sqrMagnitude<1e-10f||Vector3.Dot(student.transform.forward,toward.normalized)<.25f||!Navigation.SegmentClear(student.transform.position,at,Navigation.villager))continue;
   if(!Season.Tools.Observe(i,maker,tool))continue;
   student.GetComponent<VillagerRoutine>().Celebrate(teacher.transform);student.GetComponent<HumanoidMotion>()?.React();ToolReceipt(i,"tool-use-observed",tool);
  }
 }
 public void ToolCompleted(int maker,VillageTool tool){
  ToolReceipt(maker,"tool-crafted",tool);
  for(int i=0;i<Gatherers.Length;i++){var actor=Gatherers[i];if(i==maker||actor.GetComponent<CarryItem>().held||Vector3.Distance(actor.transform.position,StoreApproach)>Navigation.Scale*18||!Navigation.SegmentClear(actor.transform.position,StoreApproach,Navigation.villager))continue;
   var toward=StoreApproach-actor.transform.position;toward.y=0;if(toward.sqrMagnitude>1e-10f&&Vector3.Dot(actor.transform.forward,toward.normalized)<.25f)continue;
   if(Season.Tools.Observe(i,maker,tool)){if(!actor.Active&&!actor.GetComponent<VillagerExploration>().Active&&!actor.GetComponent<VillagerSocial>().Active)actor.GetComponent<HumanoidMotion>()?.React();ToolReceipt(i,"tool-observed",tool);}
  }
  foreach(var actor in Gatherers)actor.RefreshToolArt();
 }
 public GameObject CreateHoe(){return HandTool("tool-hoe-upgraded","Cultivating hoe",2.1f);}
 public GameObject AddToolBinding(Transform tool){var go=GameObject.CreatePrimitive(PrimitiveType.Cube);go.name="Amber resin binding";go.transform.SetParent(tool,false);go.transform.localPosition=new Vector3(0,.78f,0);go.transform.localScale=new Vector3(.25f,.25f,.25f);go.GetComponent<Renderer>().sharedMaterial=resinAmber;var hit=go.GetComponent<Collider>();hit.enabled=false;Destroy(hit);return go;}
}
public sealed partial class VillagerResourceGathering {
 public bool Toolmaking=>Current==Phase.ToolApproach||Current==Phase.ToolCraft;
 public bool CanWatchTools=>!Active&&!Busy&&!routine.Signalling&&!world.FoodEmergency;
 public bool ToolAction=>Toolmaking||Current==Phase.ToolCelebrate;
 public VillageTool PlannedTool{get;private set;}GameObject hoe,axeBinding;int practiceKind;
 ToolLearner ToolMemory=>world.Season.Tools.learners[identity];
 public float SowingSeconds=>ToolMemory.hoeOwned?9:12;
 public float FarmPreparationSeconds=>ToolMemory.hoeOwned?13.5f:18;
 public float HarvestSeconds=>Source.kind==VillageResource.Wood&&!world.State.Find(Source.id).felled?(ToolMemory.axeOwned?5.5f:8):Source.kind==VillageResource.Resin?6:Source.kind==VillageResource.Wheat&&ToolMemory.hoeOwned?3.5f:5;
 void InitializeToolArt(){axeBinding=world.AddToolBinding(axe.transform);hoe=world.CreateHoe();var a=GetComponentInChildren<Animator>();var hand=a!=null&&a.isHuman?a.GetBoneTransform(HumanBodyBones.RightHand):null;if(hand!=null){hoe.transform.SetParent(hand,true);hoe.transform.localPosition=Vector3.zero;hoe.transform.localRotation=Quaternion.Euler(0,0,-10);}RefreshToolArt();}
 public void RefreshToolArt(){if(axeBinding!=null)axeBinding.SetActive(ToolMemory.axeOwned);if(hoe!=null)hoe.SetActive(ToolMemory.hoeOwned&&(Current==Phase.Sowing||Current==Phase.PrepareFarm||Current==Phase.Harvest&&Source?.kind==VillageResource.Wheat||Current==Phase.ToolCelebrate&&PlannedTool==VillageTool.CultivatingHoe));if(axe!=null&&Current==Phase.ToolCelebrate&&PlannedTool==VillageTool.BoundAxe)axe.SetActive(true);}
 public bool TryMakeTool(){if(world.Season.paused||world.FoodEmergency||Active||Busy||world.Gatherers.Any(a=>a.ToolAction)||world.State.exposedWheat>0&&world.State.NeedsStorage)return false;
  // The more practiced capability wins when two designs are available.
  var first=ToolMemory.farmPractice>=ToolMemory.woodPractice?VillageTool.CultivatingHoe:VillageTool.BoundAxe;return TryMakeTool(first)||TryMakeTool(first==VillageTool.BoundAxe?VillageTool.CultivatingHoe:VillageTool.BoundAxe);
 }
 public bool TryMakeTool(VillageTool tool){if(world.Season.paused||world.FoodEmergency||Active||Busy||world.ToolDeliveryWaiting(identity)||world.Gatherers.Any(a=>a.ToolAction)||!world.Season.Tools.CanCraft(world.State,identity,tool)||world.FreeWood<2||world.FreeResin<ToolTechnologyState.ResinCost(tool))return false;
  PlannedTool=tool;routine.ResetTask();nav.ResetRoute();clock=elapsed=0;timber.SetActive(true);motion?.Carry(timber.transform);Current=Phase.ToolApproach;return true;
 }
 bool TickTool(float dt){if(Current==Phase.ToolCelebrate){if(clock>=2.2f)Cancel();return Active;}if(Current==Phase.ToolApproach){motion?.Activity(0);if(nav.MoveTo(world.StoreApproach,transform.lossyScale.y*1.6f,dt)){timber.SetActive(false);motion?.Carry(null);motion?.Activity(4);Current=Phase.ToolCraft;clock=0;}else if(nav.Blocked&&elapsed>5)Cancel();}
  else if(clock>=12){var tool=PlannedTool;bool made=world.Season.Tools.Craft(world.State,identity,tool);Cancel();if(made){Current=Phase.ToolCelebrate;clock=elapsed=0;world.ToolCompleted(identity,tool);motion?.Activity(6);motion?.React();}world.Refresh();}return Active;
 }
}
}
