using System.Collections.Generic;
using UnityEngine;
namespace BWVR {
// Saved timber/labor drives presentation only. Workers, costs and navigation stay authoritative.
public sealed partial class VillageCathedral {
 GameObject[] masonry;Renderer[][] masonryViews,finishedViews;GameObject scaffold;GameObject[] platforms,scaffoldParts;
 readonly List<Material> masonryMaterials=new List<Material>();
 MaterialPropertyBlock buildProperties;
 public float VisibleConstructionFraction=>State.Building?Mathf.Clamp01(State.work/CathedralState.Labor[State.level]):0;
 public float ConstructionTop{get;private set;}
 void BuildConstructionView(){buildProperties=new MaterialPropertyBlock();
  var shader=Resources.Load<Shader>("Cathedrals/Construction");if(shader==null)throw new System.InvalidOperationException("Missing cathedral construction shader");
  var shared=new Dictionary<Material,Material>();masonry=new GameObject[5];masonryViews=new Renderer[5][];finishedViews=new Renderer[5][];
  for(int i=0;i<5;i++){finishedViews[i]=models[i].GetComponentsInChildren<Renderer>(true);masonry[i]=Instantiate(models[i],view);masonry[i].name=Stages[i]+" rising masonry";foreach(var collider in masonry[i].GetComponentsInChildren<Collider>(true)){collider.enabled=false;Destroy(collider);}masonryViews[i]=masonry[i].GetComponentsInChildren<Renderer>(true);
   foreach(var renderer in masonryViews[i]){var slots=renderer.sharedMaterials;for(int j=0;j<slots.Length;j++){var original=slots[j];if(!shared.TryGetValue(original,out var material)){material=new Material(original){name=original.name+" under construction",shader=shader};shared.Add(original,material);masonryMaterials.Add(material);}slots[j]=material;}renderer.sharedMaterials=slots;}masonry[i].SetActive(false);
  }
  scaffold=new GameObject("Temporary cathedral scaffolding");scaffold.transform.SetParent(view,false);scaffoldParts=new GameObject[8];for(int i=0;i<8;i++)scaffoldParts[i]=SourcedItemArt.Fit("SettlementProps/resource-planks",scaffold.transform,"Scaffold timber "+i,Vector3.zero,Vector3.one);
  platforms=new GameObject[2];for(int i=0;i<2;i++)platforms[i]=SourcedItemArt.Fit("SettlementProps/structure-floor",view,"Scaffold side platform "+i,Vector3.zero,Vector3.one);
  foreach(var root in new[]{plot,scaffold,platforms[0],platforms[1]})foreach(var collider in root.GetComponentsInChildren<Collider>(true)){collider.enabled=false;Destroy(collider);}
  scaffold.SetActive(false);foreach(var platform in platforms)platform.SetActive(false);
 }
 void RefreshConstructionView(){if(masonry==null)return;var s=State;float progress=VisibleConstructionFraction;bool rising=s.Building&&s.work>0;for(int i=0;i<5;i++){masonry[i].SetActive(rising&&i==s.level);foreach(var renderer in finishedViews[i])renderer.enabled=!rising;}
  bool supplied=s.Building&&s.timber>0;scaffold.SetActive(supplied);foreach(var platform in platforms)platform.SetActive(supplied);ConstructionTop=0;if(!s.Building)return;
  var size=models[s.level].GetComponent<BoxCollider>().size;float tier=progress<.25f?.25f:progress<.5f?.5f:progress<.75f?.75f:1;float height=Mathf.Max(3,size.y*tier);
  for(int i=0;i<4;i++){scaffoldParts[i].transform.localPosition=new Vector3((i%2==0?-1:1)*(size.x*.5f+1),height*.5f,i<2?-1:size.z+1);scaffoldParts[i].transform.localScale=new Vector3(.4f,height,.4f);}
  for(int i=4;i<8;i++){bool across=i<6;scaffoldParts[i].transform.localPosition=across?new Vector3(0,height-.3f,i==4?-1:size.z+1):new Vector3((i==6?-1:1)*(size.x*.5f+1),height-.3f,size.z*.5f);scaffoldParts[i].transform.localScale=across?new Vector3(size.x+2,.3f,.3f):new Vector3(.3f,.3f,size.z+2);}
  for(int i=0;i<2;i++){platforms[i].transform.localPosition=new Vector3((i==0?-1:1)*(size.x*.5f+1),Mathf.Max(1,height-.5f),size.z*.5f);platforms[i].transform.localScale=new Vector3(1.5f,.25f,size.z+2);}
  ConstructionTop=view.position.y+size.y*progress*world.Navigation.Scale;buildProperties.Clear();buildProperties.SetFloat("_BuildTop",ConstructionTop);foreach(var renderer in masonryViews[s.level])renderer.SetPropertyBlock(buildProperties);
 }
 void DisposeConstructionView(){foreach(var material in masonryMaterials)if(material!=null)Destroy(material);masonryMaterials.Clear();}
}
}
