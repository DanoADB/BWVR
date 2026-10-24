using UnityEngine;
namespace BWVR {
// Known authored examples supply a bounded shape vocabulary, not general visual AI.
public sealed class ConstructionInspiration:MonoBehaviour {
 public BuildRecipe recipe;
 public static ConstructionInspiration At(Transform parent,Vector3 point,BuildRecipe shape,string id,float unit){var go=new GameObject("Observable "+shape+" example");go.transform.SetParent(parent,false);go.transform.position=point;var example=go.AddComponent<ConstructionInspiration>();example.recipe=shape;var hit=go.AddComponent<SphereCollider>();hit.center=Vector3.up*unit;hit.radius=unit*1.5f;var target=HybridTarget.Bind(go,id,HybridTargetKind.Inspiration);return example;}
}
}
