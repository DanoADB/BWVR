using System.Linq;
using UnityEngine;
namespace BWVR {
// Occasional independent toy play, arbitrated alongside the existing routine.
public sealed class VillagerBlockPlay:MonoBehaviour {
 public enum Phase{Idle,Approach,Inspect,Pry,Push}
 public Phase Current{get;private set;}public bool Active=>Current!=Phase.Idle;public int disassembled{get;private set;}
 ConstructionSite site;ConstructionBlock target;CreatureCuriosity creature;VillagerRoutine routine;CarryItem actor;HumanoidMotion motion;IslandNavigator navigator;
 float cooldown,clock,elapsed;Vector3 away,start;int personality;
 public void Initialize(ConstructionSite construction,CreatureCuriosity god,int index){site=construction;creature=god;personality=index;cooldown=75+index*25;routine=GetComponent<VillagerRoutine>();actor=GetComponent<CarryItem>();motion=GetComponent<HumanoidMotion>();navigator=GetComponent<IslandNavigator>();}
 public bool Eligible(ConstructionBlock block)=>block!=null&&block.Site==site&&site.Visitor==null&&block.creatureBuilt&&block.Attached&&!block.Item.held&&block.VillagerOwner==null&&block.size.x<3&&block.Body.position.y<=1.9f;
 public bool TryBegin(ConstructionBlock block){
  if(Active||actor.held||!Eligible(block))return false;
  if(!navigator.world.Sample(block.transform.position,navigator.profile,10,out _))return false;
  target=block;target.VillagerOwner=this;site.nextVillagerPlayAt=site.Clock+90;routine.ResetTask();navigator.ResetRoute();clock=elapsed=0;Current=Phase.Approach;motion?.LookAt(target.transform);return true;
 }
 public bool Tick(float dt,bool paused){
  if(paused||dt<=0)return Active;
  if(actor.held){if(Active)ResetTask();return false;}
  if(!Active){
   cooldown-=dt;if(site==null||cooldown>0||site.Clock<site.nextVillagerPlayAt||site.Clock<site.LastCreatureBuildTime+20)return false;
   var candidate=site.blocks.Where(Eligible).OrderBy(b=>(b.transform.position-transform.position).sqrMagnitude).FirstOrDefault();
   if(candidate==null||!TryBegin(candidate))cooldown=10;return Active;
  }
  elapsed+=dt;clock+=dt;
  if(target==null||target.Item.held||target.VillagerOwner!=this||elapsed>30||(Current!=Phase.Push&&!target.Attached)){ResetTask();return false;}
  if(Current==Phase.Approach){
   var from=transform.position-target.transform.position;from.y=0;if(from.sqrMagnitude<1e-10f)from=-transform.forward;
   if(Move(target.transform.position+from.normalized*site.unit*1.9f,dt)){Current=Phase.Inspect;clock=0;motion?.Activity(6);}
  }else if(Current==Phase.Inspect){Face();StandOnTray();if(clock>=1.4f){Current=Phase.Pry;clock=0;motion?.Activity(4);motion?.Signal(false,true,target.transform.position);}}
  else if(Current==Phase.Pry){
   Face();StandOnTray();if(clock>=2){site.Detach(target);target.Body.WakeUp();disassembled++;creature?.NoticeDisassembly(transform);start=target.Body.position;
    away=target.spawn;away.y=start.y;if((away-start).sqrMagnitude<4)away=start+Vector3.back*4;
    Current=Phase.Push;clock=0;motion?.Signal(false);motion?.Activity(4);
   }
  }else if(Current==Phase.Push){
   var delta=away-target.Body.position;delta.y=0;
   if(delta.magnitude<.8f||clock>8){Stop();return false;}
   var behind=target.transform.position-delta.normalized*site.unit*1.9f;Move(behind,dt);Face();motion?.Activity(4);
  }
  return Active;
 }
 public void ApplyPhysicsPush(){
  if(Current!=Phase.Push||target==null)return;var delta=away-target.Body.position;delta.y=0;var velocity=target.Body.linearVelocity;velocity.y=0;
  target.Body.WakeUp();target.Body.AddForce(delta.normalized*14-velocity*4,ForceMode.Acceleration);
 }
 bool Move(Vector3 point,float dt){motion?.Activity(0);bool arrived=navigator.MoveTo(point,transform.lossyScale.y*1.6f,dt);StandOnTray();return arrived;}
 void StandOnTray(){if(site==null)return;var at=site.Local(transform.position);if(Mathf.Abs(at.x)<9.4f&&Mathf.Abs(at.z)<9.4f){var point=transform.position;point.y=Mathf.Max(point.y,site.transform.position.y);transform.position=point;}}
 void LateUpdate(){if(actor!=null&&!actor.held&&site!=null&&!site.Paused)StandOnTray();}
 void Face(){if(target==null)return;var direction=target.transform.position-transform.position;direction.y=0;if(direction.sqrMagnitude>1e-10f)transform.rotation=Quaternion.LookRotation(direction);}
 public void ResetTask(){Stop();}
 void Stop(){if(target!=null&&target.VillagerOwner==this)target.VillagerOwner=null;target=null;Current=Phase.Idle;clock=elapsed=0;cooldown=100+personality*15+Random.Range(0,30);motion?.LookAt(null);motion?.Signal(false);motion?.Activity(0);navigator?.ResetRoute();routine?.ResetTask();}
 void OnDisable(){Stop();}
}
}
