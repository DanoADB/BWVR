using UnityEngine;
namespace BWVR {
// Original collider/navigation footprints remain conservative rubble obstacles.
public sealed class PlayImpactTarget:MonoBehaviour {
 public string id,kind;public Bounds bounds;public VillagerSocial villager;Vector3 scale;Quaternion rotation;Renderer[] renderers;MaterialPropertyBlock tint;float currentDamage;
 sealed class TintSlot {public Renderer renderer;public int slot;public Color color;public MaterialPropertyBlock original;}
 readonly System.Collections.Generic.List<TintSlot> slots=new System.Collections.Generic.List<TintSlot>();
 public void Initialize(string stableId,string type,Bounds footprint){id=stableId;kind=type;bounds=footprint;villager=GetComponent<VillagerSocial>();scale=transform.localScale;rotation=transform.localRotation;renderers=GetComponentsInChildren<Renderer>();tint=new MaterialPropertyBlock();}
 public void Apply(float damage){if(villager!=null)return;if(damage==0&&currentDamage==0)return;
  if(slots.Count==0)foreach(var r in renderers){var materials=r.sharedMaterials;for(int i=0;i<materials.Length;i++){var original=new MaterialPropertyBlock();r.GetPropertyBlock(original,i);var color=original.HasProperty(Shader.PropertyToID("_Color"))?original.GetColor("_Color"):materials[i]!=null&&materials[i].HasProperty("_Color")?materials[i].color:Color.white;slots.Add(new TintSlot{renderer=r,slot=i,color=color,original=original});}}
  currentDamage=damage;transform.localScale=new Vector3(scale.x,scale.y*(damage>=1?.35f:1),scale.z);transform.localRotation=rotation*Quaternion.Euler(0,0,damage>=1?(kind=="tree"?65:8):0);foreach(var slot in slots){if(damage==0){slot.renderer.SetPropertyBlock(slot.original,slot.slot);continue;}slot.renderer.GetPropertyBlock(tint,slot.slot);tint.SetColor("_Color",Color.Lerp(slot.color,new Color(.3f,.18f,.12f),damage*.7f));slot.renderer.SetPropertyBlock(tint,slot.slot);}
 }
 public Bounds Bounds(float unit){if(villager!=null)return new Bounds(transform.position+Vector3.up*unit,new Vector3(1.2f,2,1.2f)*unit);var result=bounds;if(currentDamage>=1){result.size=new Vector3(bounds.size.x,bounds.size.y*.35f,bounds.size.z);result.center=new Vector3(bounds.center.x,bounds.min.y+result.extents.y,bounds.center.z);}return result;}
}
}
