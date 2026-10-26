using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace BWVR {
public sealed partial class VillageResources {
 readonly Dictionary<string,Transform> turnipViews=new Dictionary<string,Transform>();Material turnipLeaf,turnipSkin;GameObject freshCrate;Transform freshPile;
 public FoodCrop ChooseCrop()=>(FoodSeconds<360||FutureHarvest>FoodReserveTarget)&&State.freshFood<FoodReserveTarget*.35f&&!sources.Any(s=>State.Find(s.id)?.crop==FoodCrop.Turnip&&(State.Find(s.id).growing||State.Find(s.id).remaining>0)||s.owner?.Sowing==true&&s.owner.PlannedCrop==FoodCrop.Turnip)?FoodCrop.Turnip:FoodCrop.Wheat;
 void EnsureTurnipMaterials(){if(turnipLeaf!=null)return;turnipLeaf=Material(new Color(.2f,.54f,.16f));turnipSkin=Material(new Color(.83f,.48f,.65f));}
 Transform TurnipPatch(GatheringSource node){EnsureTurnipMaterials();var root=new GameObject("Turnip leaves and roots");root.transform.SetParent(node.visual,false);
  var template=GameObject.CreatePrimitive(PrimitiveType.Sphere);template.SetActive(false);var sphere=template.GetComponent<MeshFilter>().sharedMesh;
  var cubeTemplate=GameObject.CreatePrimitive(PrimitiveType.Cube);cubeTemplate.SetActive(false);var cube=cubeTemplate.GetComponent<MeshFilter>().sharedMesh;
  for(int layer=0;layer<2;layer++){var pieces=new CombineInstance[9];for(int i=0;i<9;i++)pieces[i]=new CombineInstance{mesh=layer==0?cube:sphere,transform=Matrix4x4.TRS(new Vector3((i%3-1)*.9f,layer==0?.55f:.22f,(i/3-1)*.9f),Quaternion.Euler(0,i*47,layer==0?25:0),layer==0?new Vector3(.55f,.7f,.09f):new Vector3(.5f,.45f,.5f))};var mesh=new Mesh{name=layer==0?"Turnip leaves":"Turnip bulbs"};mesh.CombineMeshes(pieces);resourceMeshes.Add(mesh);var go=new GameObject(mesh.name);go.transform.SetParent(root.transform,false);go.AddComponent<MeshFilter>().sharedMesh=mesh;go.AddComponent<MeshRenderer>().sharedMaterial=layer==0?turnipLeaf:turnipSkin;}
  Destroy(template);Destroy(cubeTemplate);turnipViews.Add(node.id,root.transform);return root.transform;
 }
 bool RefreshTurnips(GatheringSource node,ResourceSource record){
  if(record.crop!=FoodCrop.Turnip){if(turnipViews.TryGetValue(node.id,out var old))old.gameObject.SetActive(false);node.cropStem.gameObject.SetActive(true);return false;}
  var view=turnipViews.TryGetValue(node.id,out var found)?found:TurnipPatch(node);view.gameObject.SetActive(true);node.cropStem.gameObject.SetActive(false);node.cropHead.gameObject.SetActive(false);node.visual.gameObject.SetActive(true);node.visual.localScale=new Vector3(1,record.growing?.15f+.85f*record.growth:1,1)*Navigation.Scale;view.GetChild(1).gameObject.SetActive(record.growing?record.growth>=.65f:record.remaining>0);view.GetChild(0).gameObject.SetActive(record.growing||record.remaining>0);return true;
 }
 void RefreshFreshFood(){EnsureTurnipMaterials();if(freshCrate==null){var at=Store+new Vector3(0,.3f,-2)*Navigation.Scale;freshCrate=Prop("box-open","Fresh turnip crate",at,1);freshPile=Part("Fresh turnip supply",PrimitiveType.Sphere,at+Vector3.up*Navigation.Scale*.65f,Vector3.one,turnipSkin).transform;}freshCrate.SetActive(State.freshFood>.01f);freshPile.gameObject.SetActive(State.freshFood>.01f);freshPile.localScale=new Vector3(1,Mathf.Min(1,State.freshFood/8),1)*Navigation.Scale;}
 void DisposeTurnipMaterials(){if(turnipLeaf!=null)Destroy(turnipLeaf);if(turnipSkin!=null)Destroy(turnipSkin);}
}
public sealed partial class VillagerResourceGathering {public FoodCrop PlannedCrop=>plannedCrop;}
}
