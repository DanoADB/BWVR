using System;
using UnityEngine;
namespace BWVR {
public enum VillageResource { Wood, Wheat }
[Serializable] public sealed class ResourceSource {
 public string id;public VillageResource kind;public float remaining=12;public bool felled;
}
[Serializable] public sealed class ResourceCargo {
 public string source;public VillageResource kind;public float amount;
 public void Clear(){source=null;amount=0;}
}
[Serializable] public sealed class ResourceState {
 public const float WoodCapacity=36,WheatCapacity=48;
 public float wood,wheat=6,consumed;public int deliveries;public ResourceSource[] sources=Array.Empty<ResourceSource>();
 public ResourceCargo[] cargo={new ResourceCargo(),new ResourceCargo(),new ResourceCargo()};
 public float Stock(VillageResource kind)=>kind==VillageResource.Wood?wood:wheat;
 public static float Capacity(VillageResource kind)=>kind==VillageResource.Wood?WoodCapacity:WheatCapacity;
 public ResourceSource Find(string id){foreach(var source in sources)if(source.id==id)return source;return null;}
 public ResourceSource Ensure(string id,VillageResource kind){var source=Find(id);if(source!=null)return source;if(sources.Length>=64)return null;source=new ResourceSource{id=id,kind=kind};Array.Resize(ref sources,sources.Length+1);sources[sources.Length-1]=source;return source;}
 public float Harvest(int actor,string sourceId,float amount){var source=Find(sourceId);if(source==null||cargo[actor].amount>0||float.IsNaN(amount)||float.IsInfinity(amount))return 0;amount=Mathf.Min(3,Mathf.Max(0,amount),source.remaining);source.remaining-=amount;cargo[actor].source=sourceId;cargo[actor].kind=source.kind;cargo[actor].amount=amount;return amount;}
 public float Deposit(int actor){var load=cargo[actor];if(load.amount<=0)return 0;float accepted=Mathf.Min(load.amount,Capacity(load.kind)-Stock(load.kind));if(load.kind==VillageResource.Wood)wood+=accepted;else wheat+=accepted;var source=Find(load.source);if(source!=null)source.remaining=Mathf.Min(12,source.remaining+load.amount-accepted);load.Clear();if(accepted>0)deliveries=Math.Min(1000000,deliveries+1);return accepted;}
 public void Recover(int actor){var load=cargo[actor];var source=Find(load.source);if(source!=null)source.remaining=Mathf.Min(12,source.remaining+load.amount);load.Clear();}
 public void Consume(float dt,int people){if(dt<=0||float.IsNaN(dt)||float.IsInfinity(dt))return;float eaten=Mathf.Min(wheat,dt*Mathf.Clamp(people,0,3)/90f);wheat-=eaten;consumed=Mathf.Min(100000000,consumed+eaten);}
 public bool Valid(){if(!HybridState.Number(wood,0,WoodCapacity)||!HybridState.Number(wheat,0,WheatCapacity)||!HybridState.Number(consumed,0,100000000)||deliveries<0||deliveries>1000000||sources==null||sources.Length>64||cargo==null||cargo.Length!=3)return false;var ids=new System.Collections.Generic.HashSet<string>();foreach(var source in sources)if(source==null||string.IsNullOrEmpty(source.id)||source.id.Length>160||!ids.Add(source.id)||!Enum.IsDefined(typeof(VillageResource),source.kind)||!HybridState.Number(source.remaining,0,12)||(source.felled&&source.kind!=VillageResource.Wood))return false;var loads=new System.Collections.Generic.Dictionary<string,float>();foreach(var load in cargo){if(load==null||!Enum.IsDefined(typeof(VillageResource),load.kind)||!HybridState.Number(load.amount,0,3))return false;if(load.amount<=0)continue;var source=Find(load.source);if(source==null||source.kind!=load.kind)return false;loads.TryGetValue(source.id,out float total);loads[source.id]=total+load.amount;if(source.remaining+loads[source.id]>12.001f)return false;}return true;}
}
}
