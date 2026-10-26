using System;
using System.Linq;
using UnityEngine;
namespace BWVR {
public sealed partial class SocialMemory {
 public float faith=.2f,lastProvisionAt=-100;public int provisionsWitnessed;
 bool FaithValid()=>HybridState.Number(faith,0,1)&&HybridState.Number(lastProvisionAt,-100,100000000)&&Count(provisionsWitnessed);
 public bool WitnessProvision(float time,float fraction){if(!HybridState.Number(time,0,100000000)||!HybridState.Number(fraction,0,1)||fraction<=0||(provisionsWitnessed>0&&time<lastProvisionAt+5))return false;faith=Mathf.Clamp01(faith+.04f*fraction);provisionsWitnessed=Math.Min(1000000,provisionsWitnessed+1);lastProvisionAt=time;return true;}
}
public sealed partial class ResourceState {
 public float giftedFood,giftedWood,nextProvisionAt;
 bool ProvisionsValid()=>HybridState.Number(giftedFood,0,100000000)&&HybridState.Number(giftedWood,0,100000000)&&HybridState.Number(nextProvisionAt,0,100000300);
 public float ReceiveProvision(VillageResource kind,float amount,float time){if((kind!=VillageResource.Wood&&kind!=VillageResource.Wheat)||!HybridState.Number(amount,0,3)||!HybridState.Number(time,0,100000000)||time<nextProvisionAt)return 0;float accepted=Mathf.Min(amount,Mathf.Max(0,Limit(kind)-Stock(kind)));if(accepted<=.001f)return 0;if(kind==VillageResource.Wood){wood+=accepted;giftedWood=Mathf.Min(100000000,giftedWood+accepted);}else{wheat+=accepted;giftedFood=Mathf.Min(100000000,giftedFood+accepted);}nextProvisionAt=time+2;return accepted;}
}
public sealed partial class VillageResources {
 public float VillageFaith=>Gatherers.Length==0?0:Gatherers.Average(a=>a.GetComponent<VillagerSocial>().Memory.faith);
 public bool ProvisionVisible(VillagerSocial villager,Vector3 point,Transform observed=null){if(villager==null||!villager.isActiveAndEnabled||villager.GetComponent<CarryItem>()?.held==true)return false;var delta=point-villager.transform.position;delta.y=0;if(delta.sqrMagnitude>Navigation.Scale*Navigation.Scale*1600)return false;if(delta.sqrMagnitude>Navigation.Scale*Navigation.Scale&&Vector3.Dot(villager.transform.forward,delta.normalized)<.2f)return false;
  var eye=villager.transform.position+Vector3.up*Navigation.Scale*Navigation.villager.height*.8f;var targetCollider=observed!=null?observed.GetComponent<Collider>():null;var goal=targetCollider!=null?targetCollider.bounds.center:point+Vector3.up*Navigation.Scale;var ray=goal-eye;float distance=ray.magnitude;if(distance<Navigation.Scale*.1f)return true;
  foreach(var hit in Physics.RaycastAll(eye,ray/distance,Mathf.Max(0,distance-Navigation.Scale*.15f),~0,QueryTriggerInteraction.Ignore)){if(hit.collider.transform.IsChildOf(villager.transform)||(observed!=null&&hit.collider.transform.IsChildOf(observed)))continue;return false;}return true;
 }
 public int WitnessProvision(Vector3 point,float fraction){int witnesses=0;foreach(var gatherer in Gatherers){var social=gatherer.GetComponent<VillagerSocial>();if(!ProvisionVisible(social,point)||!social.Memory.WitnessProvision(Season.Hybrid.time,fraction))continue;witnesses++;social.GetComponent<HumanoidMotion>()?.React();social.GetComponent<VillagerAffect>()?.ProvisionSeen();}return witnesses;}
 public float ProvisionSpace(VillageResource kind)=>Mathf.Max(0,FreeSpace(kind)-(kind==VillageResource.Wheat?24-State.exposedWheat:0));
 public bool CanReceiveProvision(Vector3 point,VillageResource kind,out string reason){reason="";if(Season.paused){reason="Paused";return false;}if(kind!=VillageResource.Wood&&kind!=VillageResource.Wheat){reason="Unsupported gift";return false;}
  if(!HybridState.Number(point.x,-10000,10000)||!HybridState.Number(point.y,-10000,10000)||!HybridState.Number(point.z,-10000,10000)||!Navigation.Sample(point,Navigation.villager,.5f,out var safe)||(safe-point).sqrMagnitude>Navigation.Scale*Navigation.Scale*4||(Navigation.Terrain!=null&&(!Navigation.Terrain.Dry(point)||Mathf.Abs(Navigation.Terrain.Project(point).y-point.y)>Navigation.Scale*2))){reason="Aim at safe, dry ground";return false;}
  if(Vector3.Distance(point,Store)>Navigation.Scale*15&&!Gatherers.Any(a=>a.GetComponent<CarryItem>()?.held!=true&&Vector3.Distance(a.transform.position,point)<Navigation.Scale*30)){reason="Bring gifts near villagers or stores";return false;}if(ProvisionSpace(kind)<=.001f){reason="Village storage is full or reserved";return false;}return true;
 }
}
}
