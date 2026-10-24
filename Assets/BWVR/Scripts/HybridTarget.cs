using UnityEngine;
namespace BWVR {
public enum HybridTargetKind{Object,Cistern,Water,Villager,Inspiration}
public sealed class HybridTarget:MonoBehaviour {
 public string id;public HybridTargetKind kind;public CreatureAffordance affordance;public VillagerSocial villager;
 public static HybridTarget Bind(GameObject go,string identity,HybridTargetKind type){var t=go.GetComponent<HybridTarget>()??go.AddComponent<HybridTarget>();t.id=identity;t.kind=type;t.affordance=go.GetComponent<CreatureAffordance>();t.villager=go.GetComponent<VillagerSocial>();return t;}
}
}
