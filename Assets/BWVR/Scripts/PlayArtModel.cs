using UnityEngine;
namespace BWVR {
// Provenance only; this artwork never owns gameplay, collision or simulation state.
public sealed class PlayArtModel:MonoBehaviour {public string id,source,creator;public string license="CC0-1.0";}
public static class SourcedPlayArt {
 public static GameObject Part(string id,Transform parent,string label,Vector3 position,Vector3 size,Material tint=null){
  var go=Object.Instantiate(SourcedItemArt.Prefab("PlayArt/"+id),parent,false);go.name=label;go.transform.localPosition=position;go.transform.localScale=size;
  if(tint!=null){var view=go.GetComponent<Renderer>();var slots=view.sharedMaterials;for(int i=0;i<slots.Length;i++)slots[i]=tint;view.sharedMaterials=slots;}return go;
 }
}
}
