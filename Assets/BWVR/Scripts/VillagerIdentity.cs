using System;
using UnityEngine;
namespace BWVR {
public enum VillagerGender {Man,Woman}
public static class VillagePopulation {
 public const int Adults=12,Children=3,Maximum=Adults+Children;
 public static bool Supported(int count)=>count==3||count==6||count==Adults;
 public static T[] Create<T>(Func<int,T> make){var result=new T[Adults];for(int i=0;i<result.Length;i++)result[i]=make(i);return result;}
 public static void Expand<T>(ref T[] entries,Func<int,T> make){if(entries==null||entries.Length==Adults||!Supported(entries.Length))return;int previous=entries.Length;Array.Resize(ref entries,Adults);for(int i=previous;i<Adults;i++)entries[i]=make(i);}
 public static int Model(int index)=>index%2==1?1:index%6==2?2:0;
 public static VillagerRoutine.Role Role(int index)=>index==0?VillagerRoutine.Role.WaterGatherer:index>=6?(index%2==0?VillagerRoutine.Role.Farmer:VillagerRoutine.Role.Forester):index%2==0?VillagerRoutine.Role.Forester:VillagerRoutine.Role.Farmer;
 public static SocialMemory Social(int i)=>new SocialMemory{boldness=.25f+(i%3)*.3f};
 public static VillagerTraits Traits(int i)=>new VillagerTraits{boldness=.25f+(i%3)*.3f,sociability=new[]{.7f,.5f,.4f,.65f,.45f,.75f}[i%6],diligence=new[]{.4f,.8f,.6f,.7f,.65f,.55f}[i%6],stature=new[]{.35f,.65f,.5f,.45f,.6f,.4f}[i%6]};
 public static void Place(IslandNavigator walker){var origin=walker.transform.position;for(int i=0;i<100;i++){float angle=i*137.5f*Mathf.Deg2Rad;var wanted=i==0?origin:origin+new Vector3(Mathf.Cos(angle),0,Mathf.Sin(angle))*walker.world.Scale*(4+i*.35f);if(walker.CanPlace(wanted,out var point)){walker.transform.position=point;walker.ResetRoute();return;}}throw new InvalidOperationException("No clear villager spawn: "+walker.name);}
}
public sealed class VillagerIdentity:MonoBehaviour {
 public int Index{get;private set;}public VillagerGender Gender=>Index%2==0?VillagerGender.Man:VillagerGender.Woman;
 public void Initialize(int index){Index=index;gameObject.name=new[]{"Alden","Mira","Rowan","Tessa","Bram","Elin","Corin","Nora","Darin","Lena","Ewan","Faye"}[index]+" (villager "+index+")";if(index<3)return;var tint=new[]{Color.white,Color.white,Color.white,new Color(.65f,.88f,.67f),new Color(.64f,.77f,1),new Color(.88f,.64f,.88f),new Color(.94f,.75f,.48f),new Color(.52f,.87f,.88f),new Color(.9f,.59f,.5f),new Color(.63f,.69f,.94f),new Color(.78f,.83f,.46f),new Color(.86f,.6f,.73f)}[index];var block=new MaterialPropertyBlock();block.SetColor("_Color",tint);foreach(var r in GetComponentsInChildren<SkinnedMeshRenderer>()){var mats=r.sharedMaterials;for(int i=0;i<mats.Length;i++)if(mats[i]!=null&&(mats[i].name.Contains("Peasant")||mats[i].name.Contains("Ranger")))r.SetPropertyBlock(block,i);}}
}
}
