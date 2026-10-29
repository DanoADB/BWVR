using System;
using System.Linq;
using UnityEngine;
namespace BWVR {
public enum TributeMotive{None,Affection,Fear}
[Serializable]public sealed class TributeMemory {
 public TributeMotive motive;public int delivered,affection,fear;public float nextAt;public bool pending,available;
 public bool Valid()=>Enum.IsDefined(typeof(TributeMotive),motive)&&delivered>=0&&delivered<=1000000&&affection>=0&&fear>=0&&affection+fear==delivered&&HybridState.Number(nextAt,0,100000600)&&!(pending&&available);
 public static TributeMotive Consensus(SocialMemory[] villagers){int love=0,fear=0;foreach(var v in villagers){if(v.trust>=.8f)love++;if(v.trust<=.2f)fear++;}return love>villagers.Length/2?TributeMotive.Affection:fear>villagers.Length/2?TributeMotive.Fear:TributeMotive.None;}
}
// A settlement offering is distinct from an individual's spontaneous toy/berry gift.
public sealed class VillageTributes:MonoBehaviour {
 public CarryItem Basket{get;private set;}public VillagerExploration Courier{get;private set;}public Vector3 Altar{get;private set;}
 public TributeMemory Memory=>creature.Season.Tribute;public bool Ready=>Memory.available;CreatureHybrid creature;VillagerRoutine[] villagers;IslandNavigation nav;Material material;float retry,reaction;Vector3 storage;
 public static VillageTributes Build(Transform parent,CreatureHybrid god,VillagerRoutine[] residents,IslandNavigation navigation,Vector3 village){var root=new GameObject("Village communal tributes");root.transform.SetParent(parent,false);var system=root.AddComponent<VillageTributes>();system.Initialize(god,residents,navigation,village);return system;}
 void Initialize(CreatureHybrid god,VillagerRoutine[] residents,IslandNavigation navigation,Vector3 village){creature=god;villagers=residents;nav=navigation;if(!nav.Sample(village+Vector3.back*nav.Scale*15,nav.villager,20,out storage))storage=village;storage=nav.ProjectFoot(storage,nav.villager);if(!nav.Sample(village+Vector3.right*nav.Scale*18,nav.creature,20,out var altar))altar=storage;Altar=nav.ProjectFoot(altar,nav.creature);
  var go=new GameObject("Large communal fruit tribute");go.transform.SetParent(transform,false);go.transform.position=storage+Vector3.up*nav.Scale;go.transform.localScale=Vector3.one*nav.Scale;material=new Material(SourcedItemArt.Prefab("SettlementProps/box-open").GetComponentInChildren<Renderer>().sharedMaterial){color=new Color(.8f,.43f,.16f)};
  material.SetFloat("_Recolor",1);
  SourcedItemArt.Fit("SettlementProps/box-open",go.transform,"Communal tribute crate",Vector3.down*.1f,new Vector3(5,1.8f,5),material);
  var produce=new[]{"apple","pear","orange"};for(int i=0;i<7;i++){float angle=i*137.5f*Mathf.Deg2Rad;var center=new Vector3(Mathf.Cos(angle)*(i==0?0:1.3f),i==0?1.05f:.85f,Mathf.Sin(angle)*(i==0?0:1.3f));SourcedItemArt.Fit("FoodProps/"+produce[i%produce.Length],go.transform,"Tribute fruit "+i,center,Vector3.one*1.1f);}
  go.AddComponent<BoxCollider>().size=new Vector3(5,3,5);Basket=go.AddComponent<CarryItem>();Basket.kind=Offering.Food;Basket.home=go.transform.position;go.AddComponent<CreatureAffordance>().kind=CreatureAffordance.Kind.Food;go.AddComponent<VillageTributeProp>().owner=this;creature.RegisterObject(go.GetComponent<CreatureAffordance>(),"village-tribute");ResetWorld();
 }
 public void Tick(float dt,bool paused){if(paused||dt<=0)return;if(reaction>0){reaction=Mathf.Max(0,reaction-dt);foreach(var v in villagers){var motion=v.GetComponent<HumanoidMotion>();motion?.LookAt(reaction>0?creature.transform:null);motion?.Signal(reaction>0&&Memory.motive==TributeMotive.Affection,reaction>0&&Memory.motive==TributeMotive.Fear,creature.transform.position);}}retry-=dt;if(Memory.delivered>=1000000||Memory.available||Courier!=null||retry>0)return;retry=2;
  if(!Memory.pending){if(creature.State.time<Memory.nextAt||creature.Season.crops<12)return;var motive=TributeMemory.Consensus(villagers.Select(v=>v.GetComponent<VillagerSocial>().Memory).ToArray());if(motive==TributeMotive.None)return;Memory.motive=motive;Memory.pending=true;creature.Season.crops-=8;material.color=motive==TributeMotive.Fear?new Color(.43f,.27f,.58f):new Color(.8f,.43f,.16f);Basket.transform.position=Basket.home;Basket.gameObject.SetActive(true);}
  foreach(var actor in villagers.Select(v=>v.GetComponent<VillagerExploration>()))if(actor.TryTribute(this)){Courier=actor;break;}
 }
 public void Presented(VillagerExploration actor){if(Courier!=actor||!Memory.pending)return;Courier=null;Memory.pending=false;Memory.available=true;Memory.delivered=Math.Min(1000000,Memory.delivered+1);if(Memory.motive==TributeMotive.Affection)Memory.affection++;else Memory.fear++;Memory.nextAt=creature.State.time+180;reaction=3;creature.Journal.Add(creature.State.time,"communal tribute",CreatureAction.IdleOrObserve,"village-tribute",Memory.motive.ToString());}
 public void Cancel(VillagerExploration actor){if(Courier!=actor)return;Courier=null;retry=8;}
 public void Eat(){if(!Memory.available)return;Memory.available=false;Basket.gameObject.SetActive(false);}
 public void ResetWorld(){reaction=0;material.color=Memory.motive==TributeMotive.Fear?new Color(.43f,.27f,.58f):new Color(.8f,.43f,.16f);Courier?.ResetTask();Courier=null;Basket.held=false;Basket.transform.position=Memory.available?Altar+Vector3.up*nav.Scale:Basket.home;Basket.GetComponent<Collider>().enabled=true;Basket.gameObject.SetActive(Memory.available||Memory.pending);retry=0;}
 void OnDestroy(){if(material!=null)Destroy(material);}
}
}
