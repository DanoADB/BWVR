using System.Linq;
using UnityEngine;
namespace BWVR {
public sealed partial class VillageResources {
 MaterialPropertyBlock cropTint;
 public float ReservedSeed=>Gatherers==null?0:Gatherers.Count(a=>a.Sowing||a.PreparingFarm)*ResourceState.WheatSeedCost;
 public bool CanSow(GatheringSource node){
  if(node==null||node.kind!=VillageResource.Wheat||node.owner!=null||!State.DepletedField(node.id)||!Safe(node))return false;
  return State.wheat-ReservedSeed>=ResourceState.WheatSeedCost&&sources.Count(s=>s.kind==VillageResource.Wheat&&(State.Find(s.id)?.growing==true||s.owner?.Sowing==true))<3;
 }
 public GameObject CreateSeedPouch()=>Part("Sowing seed pouch",PrimitiveType.Sphere,Store,new Vector3(.8f,.9f,.7f),grain);
 // Clear decorative undergrowth once at construction, never trees or actor targets.
 public void ClearCropFootprints(){
  var meadow=Navigation.transform.Find("Meadow and forest understory");if(meadow==null)return;
  var plots=sources.Where(s=>s.kind==VillageResource.Wheat&&(!s.expansion||State.Find(s.id)?.farmPrepared==true)).Select(s=>new Bounds(s.at+Vector3.up*Navigation.Scale*3,new Vector3(5,8,5)*Navigation.Scale)).ToArray();
  foreach(Transform plant in meadow){if(!plant.gameObject.activeSelf||plant.GetComponentsInChildren<Collider>().Length>0)continue;var bounds=TreePlayWorld.VisualBounds(plant.gameObject);foreach(var plot in plots)if(plot.Intersects(bounds)){plant.gameObject.SetActive(false);break;}}
 }
 void AdvanceCrops(float dt){foreach(var node in sources)if(node.kind==VillageResource.Wheat&&Safe(node))State.Grow(node.id,dt);}
 void RefreshWheat(GatheringSource node,ResourceSource record){
  if(node.expansion){node.sapling.gameObject.SetActive(record.farmPrepared);if(!record.farmPrepared){node.visual.gameObject.SetActive(false);return;}}
  if(cropTint==null)cropTint=new MaterialPropertyBlock();
  if(node.cropStem==null)node.cropStem=node.visual.GetChild(0).GetComponent<Renderer>();
  if(node.cropHead==null)node.cropHead=node.visual.GetChild(1).GetComponent<Renderer>();
  float height=record.growing?.12f+.88f*record.growth:.15f+.85f*record.remaining/12;
  node.visual.localScale=new Vector3(1,height,1)*Navigation.Scale;node.visual.gameObject.SetActive(true);
  node.cropHead.gameObject.SetActive(record.growing?record.growth>=.65f:record.remaining>.001f);
  float ripeness=record.growing?Mathf.InverseLerp(.65f,1,record.growth):record.remaining>0?1:0;
  cropTint.Clear();cropTint.SetColor("_Color",record.growing?Color.Lerp(new Color(.2f,.55f,.15f),new Color(.7f,.57f,.22f),ripeness):new Color(.55f,.4f,.18f));node.cropStem.SetPropertyBlock(cropTint);
  cropTint.SetColor("_Color",Color.Lerp(new Color(.32f,.66f,.18f),new Color(.95f,.72f,.24f),ripeness));node.cropHead.SetPropertyBlock(cropTint);
 }
}
}
