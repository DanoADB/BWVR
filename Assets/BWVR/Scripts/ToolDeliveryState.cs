using System;
using UnityEngine;
namespace BWVR {
public enum ToolDeliverySource {None,Player,Creature}
public enum ToolDeliveryStatus {Dormant,Offered,Delivered,Expired,Unavailable}
[Serializable] public sealed class ToolDeliveryState {
 public ToolUseProof proof;public bool showUse;public VillageTool tool;public int maker,recipient;public ToolDeliveryStatus status;public ToolDeliverySource source;public float openedAt,expiresAt;public Vector3 grounded;public bool playerArrivalPending;
 public bool Active=>status==ToolDeliveryStatus.Offered;
 public bool Valid()=>(proof==null||proof.Valid()&&(!proof.used||status==ToolDeliveryStatus.Delivered))&&(!showUse||status==ToolDeliveryStatus.Delivered)&&Enum.IsDefined(typeof(VillageTool),tool)&&Enum.IsDefined(typeof(ToolDeliveryStatus),status)&&Enum.IsDefined(typeof(ToolDeliverySource),source)&&maker>=0&&maker<VillagePopulation.Adults&&recipient>=0&&recipient<VillagePopulation.Adults&&HybridState.Number(openedAt,0,100000000)&&HybridState.Number(expiresAt,openedAt,100000180)&&HybridState.Number(grounded.x,-10000,10000)&&HybridState.Number(grounded.y,-10000,10000)&&HybridState.Number(grounded.z,-10000,10000)&&((status==ToolDeliveryStatus.Delivered)==(source!=ToolDeliverySource.None))&&(!playerArrivalPending||Active);
}
public sealed partial class ToolTechnologyState {
 public ToolDeliveryState delivery;public float nextDeliveryAt;
 bool DeliveryValid()=>(delivery==null||delivery.Valid()&&learners!=null&&delivery.maker<learners.Length&&delivery.recipient<learners.Length&&learners[delivery.maker]!=null&&learners[delivery.recipient]!=null&&(delivery.status==ToolDeliveryStatus.Dormant||learners[delivery.maker].Knows(delivery.tool))&&(delivery.status!=ToolDeliveryStatus.Delivered||learners[delivery.recipient].Owns(delivery.tool)))&&HybridState.Number(nextDeliveryAt,0,100000300);
 public bool OfferDelivery(ResourceState stock,int maker,int recipient,VillageTool tool,float time,Vector3 point){if(delivery?.Active==true||time<nextDeliveryAt||maker<0||maker>=learners.Length||recipient<0||recipient>=learners.Length||!Enum.IsDefined(typeof(VillageTool),tool)||!learners[maker].Knows(tool)||learners[recipient].Owns(tool)||stock.wood<2||stock.resin<ResinCost(tool))return false;delivery=new ToolDeliveryState{status=ToolDeliveryStatus.Offered,maker=maker,recipient=recipient,tool=tool,openedAt=time,expiresAt=time+150,grounded=point};nextDeliveryAt=time+120;return true;}
 public bool ReceiveDelivery(ResourceState stock,ToolDeliverySource by){var d=delivery;if(d?.Active!=true||(by!=ToolDeliverySource.Player&&by!=ToolDeliverySource.Creature)||!learners[d.maker].Knows(d.tool)||learners[d.recipient].Owns(d.tool)||stock.wood<2||stock.resin<ResinCost(d.tool))return false;var recipient=learners[d.recipient];if(!recipient.Knows(d.tool)){if(d.tool==VillageTool.BoundAxe)recipient.axeKnown=recipient.sawAxe=true;else recipient.hoeKnown=recipient.sawHoe=true;}if(!Craft(stock,d.recipient,d.tool))return false;d.status=ToolDeliveryStatus.Delivered;d.showUse=true;d.source=by;d.playerArrivalPending=false;return true;}
}
public sealed class ToolDelivery:MonoBehaviour {public VillageResources world;}
}
