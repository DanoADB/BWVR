using UnityEngine;
namespace BWVR {
// Original collider/navigation footprints remain conservative rubble obstacles.
public sealed class PlayImpactTarget:MonoBehaviour {
 public string id,kind;public Bounds bounds;public VillagerSocial villager;Vector3 scale;Quaternion rotation;Renderer[] renderers;MaterialPropertyBlock tint;float currentDamage;
 public void Initialize(string stableId,string type,Bounds footprint){id=stableId;kind=type;bounds=footprint;villager=GetComponent<VillagerSocial>();scale=transform.localScale;rotation=transform.localRotation;renderers=GetComponentsInChildren<Renderer>();tint=new MaterialPropertyBlock();}
 public void Apply(float damage){currentDamage=damage;if(villager!=null)return;transform.localScale=new Vector3(scale.x,scale.y*(damage>=1?.35f:1),scale.z);transform.localRotation=rotation*Quaternion.Euler(0,0,damage>=1?(kind=="tree"?65:8):0);foreach(var r in renderers){var baseColor=r.sharedMaterial!=null&&r.sharedMaterial.HasProperty("_Color")?r.sharedMaterial.color:Color.white;tint.SetColor("_Color",Color.Lerp(baseColor,new Color(.3f,.18f,.12f),damage*.7f));r.SetPropertyBlock(tint);}}
 public Bounds Bounds(float unit){if(villager!=null)return new Bounds(transform.position+Vector3.up*unit,new Vector3(1.2f,2,1.2f)*unit);var result=bounds;if(currentDamage>=1){result.size=new Vector3(bounds.size.x,bounds.size.y*.35f,bounds.size.z);result.center=new Vector3(bounds.center.x,bounds.min.y+result.extents.y,bounds.center.z);}return result;}
}
}
