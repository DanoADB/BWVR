using System.Linq;
using UnityEngine;
namespace BWVR {
public sealed partial class VillageResources {
 public int FoodPopulation=>Gatherers.Length+Season.Families.children.Length;
 public float FoodReserveTarget=>ReserveTarget(FoodPopulation,State.Limit(VillageResource.Wheat));
 public float MealGrain=>Mathf.Max(0,State.wheat-ReservedSeed);
 public float FoodSeconds=>MealGrain*90/Mathf.Max(1,FoodPopulation);
 public bool NeedsFoodReserve=>MealGrain<FoodReserveTarget;
 public bool FoodEmergency=>FoodSeconds<135;
 // Six minutes of meals plus three future sowings; bounded by real covered space.
 public static float ReserveTarget(int people,float capacity)=>Mathf.Min(Mathf.Max(0,capacity),Mathf.Clamp(people,0,6)*4+3);
 Transform foodReserveFill;Renderer foodReserveRenderer;MaterialPropertyBlock foodReserveTint;
 void BuildFoodReserveMarker(){
  float u=Navigation.Scale;Part("Food reserve target post",PrimitiveType.Cube,Store+new Vector3(6,2.5f,-4)*u,new Vector3(.8f,5,.5f),roof);
  foodReserveFill=Part("Food reserve readiness",PrimitiveType.Cube,Store,new Vector3(.5f,.1f,.7f),grain).transform;foodReserveRenderer=foodReserveFill.GetComponent<Renderer>();
 }
 void RefreshFoodReserve(){
  if(foodReserveFill==null)return;float ratio=Mathf.Clamp01(MealGrain/Mathf.Max(1,FoodReserveTarget));float height=Mathf.Max(.15f,4.5f*ratio);
  foodReserveFill.position=Store+new Vector3(6,height*.5f,-4.4f)*Navigation.Scale;foodReserveFill.localScale=new Vector3(.55f,height,.7f)*Navigation.Scale;
  if(foodReserveTint==null)foodReserveTint=new MaterialPropertyBlock();foodReserveTint.SetColor("_Color",FoodEmergency?new Color(.85f,.16f,.08f):NeedsFoodReserve?new Color(.95f,.6f,.12f):new Color(.24f,.7f,.26f));foodReserveRenderer.SetPropertyBlock(foodReserveTint);
 }
}
public sealed partial class VillagerResourceGathering {
 // Reserve work competes for idle adults, never interrupts an owned activity.
 public bool TryFoodReserve(){
  if(world==null||world.Season.paused||Active||Busy)return false;
  int workers=world.Gatherers.Count(a=>a.Active&&(a.Source?.kind==VillageResource.Wheat));
  if(workers>=(world.FoodEmergency?2:1))return false;
  if(TryFoodProduction())return true;if(!world.NeedsFoodReserve)return false;
  float incoming=world.Gatherers.Where(a=>a.Active&&a.Source?.kind==VillageResource.Wheat&&!a.Sowing).Sum(a=>a.Reserved);
  if(world.MealGrain+incoming>=world.FoodReserveTarget)return false;
  var field=world.sources.Where(s=>s.kind==VillageResource.Wheat&&world.Available(s)).OrderBy(s=>(s.approach-transform.position).sqrMagnitude).FirstOrDefault();
  if(field!=null)return TryBegin(field);
  return TrySow();
 }
}
}
