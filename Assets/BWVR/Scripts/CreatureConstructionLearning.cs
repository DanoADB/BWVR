using System;
using System.Collections.Generic;
using UnityEngine;
namespace BWVR {
// Pattern memory extends the coordinator's observation/completion events.
public sealed class CreatureConstructionLearning:MonoBehaviour {
 public readonly List<ConstructionSite> sites=new List<ConstructionSite>();CreatureHybrid creature;ConstructionSite original;IslandDiscoveries discoveries;Func<SliceState> season;Material cairnMaterial;
 readonly HashSet<ConstructionSite> counted=new HashSet<ConstructionSite>();
 public BuildingMemory Memory=>season().Building;
 public void Initialize(CreatureHybrid god,ConstructionSite main,IslandDiscoveries island,Func<SliceState> current){
  creature=god;original=main;discoveries=island;season=current;sites.Add(main);main.Recipe=(BuildRecipe)Memory.mainRecipe;
  foreach(var point in island.points)if(point.construction!=null){sites.Add(point.construction);point.construction.Recipe=(BuildRecipe)Memory.cacheRecipes[point.id];}
  foreach(var site in sites)if(site.RecipeComplete)counted.Add(site);
  var nav=god.GetComponent<IslandNavigator>().world;var pointOfInterest=island.points.Find(p=>p.id==2);
  if(pointOfInterest!=null){var wanted=pointOfInterest.transform.position;if(nav.Sample(wanted,nav.creature,8,out var at)){if(nav.Terrain!=null)at=nav.Terrain.Project(at);var example=ConstructionInspiration.At(transform.parent,at,BuildRecipe.Tower,"cairn",nav.Scale);cairnMaterial=new Material(Shader.Find("BWVR/GameSurface")){color=new Color(.53f,.52f,.43f)};for(int i=0;i<3;i++){var stone=GameObject.CreatePrimitive(PrimitiveType.Sphere);stone.name="Cairn stone";stone.transform.SetParent(example.transform,false);stone.transform.localPosition=Vector3.up*nav.Scale*(.6f+i);stone.transform.localScale=new Vector3(3-i*.7f,1.1f,2.7f-i*.6f)*nav.Scale;stone.GetComponent<Renderer>().sharedMaterial=cairnMaterial;stone.GetComponent<Collider>().enabled=false;Destroy(stone.GetComponent<Collider>());}}}
  foreach(var example in transform.parent.GetComponentsInChildren<ConstructionInspiration>()){var target=example.GetComponent<HybridTarget>();creature.Register(target);var cue=example.gameObject.AddComponent<SpatialTarget>();cue.invoke=()=>creature.Guide(GuidanceIntent.Investigate,target);}
 }
 public void Observe(ConstructionInspiration example){if(example==null||Memory.Knows(example.recipe))return;Memory.observed|=1<<(int)example.recipe;creature.Journal.Add(creature.State.time,"shape learned",CreatureAction.InvestigateObject,example.GetComponent<HybridTarget>().id,example.recipe.ToString());AssignEmptySites();}
 public void AssignEmptySites(){foreach(var site in sites){if(site.IsBridge)continue;BuildRecipe best=BuildRecipe.Arch;float score=float.NegativeInfinity;for(int i=0;i<3;i++){var recipe=(BuildRecipe)i;if(!Memory.Knows(recipe))continue;float value=Memory.preference[i]+1f/(1+Memory.completed[i])-(Memory.lastSelected==i?.12f:0);if(value>score){score=value;best=recipe;}}if(site.SelectRecipe(best))Memory.lastSelected=(int)best;}}
 public void Placed(ConstructionBlock block){if(block==null)return;var site=block.Site;if(site.RecipeComplete&&counted.Add(site)){int index=(int)site.Recipe;Memory.completed[index]=Math.Min(1000000,Memory.completed[index]+1);creature.Journal.Add(creature.State.time,"structure complete",CreatureAction.ConstructionPlay,site.name,site.Recipe.ToString());}}
 public void Tick(){foreach(var site in sites)if(!site.RecipeComplete)counted.Remove(site);}
 public void Reinforce(BuildRecipe recipe,bool positive){int index=(int)recipe;Memory.preference[index]=Mathf.Clamp01(Memory.preference[index]+(positive?.12f:-.12f));}
 public void Capture(){Memory.mainRecipe=(int)original.Recipe;foreach(var point in discoveries.points)if(point.construction!=null)Memory.cacheRecipes[point.id]=(int)point.construction.Recipe;}
 public void ResetWorld(){counted.Clear();foreach(var site in sites){site.Recipe=BuildRecipe.Arch;}AssignEmptySites();}
 void OnDestroy(){if(cairnMaterial!=null)Destroy(cairnMaterial);}
}
}
