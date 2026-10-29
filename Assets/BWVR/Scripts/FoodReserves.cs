using System.Linq;
using UnityEngine;
namespace BWVR {
public sealed partial class VillageResources {
 public int FoodPopulation=>Gatherers.Length+Season.Families.children.Length;
 public float FoodReserveTarget=>ReserveTarget(FoodPopulation,State.Limit(VillageResource.Wheat));
 public float MealGrain=>State.freshFood+Mathf.Max(0,State.wheat-ReservedSeed);
 public float FoodSeconds=>MealGrain*90/Mathf.Max(1,FoodPopulation);
 public bool NeedsFoodReserve=>!Season.observeWithoutFoodWork&&MealGrain<FoodReserveTarget;
 public bool FoodEmergency=>!Season.observeWithoutFoodWork&&FoodSeconds<135;
 // Six minutes of meals plus three future sowings; bounded by real covered space.
 public static float ReserveTarget(int people,float capacity)=>Mathf.Min(Mathf.Max(0,capacity),Mathf.Clamp(people,0,VillagePopulation.Maximum)*4+3);
 GameObject[] reserveMeals;
 public int VisibleReserveMeals {get;private set;}
 void BuildFoodReserveMarker(){
  float u=Navigation.Scale;Prop("box-open","Meal reserve crate",Store+new Vector3(3,.3f,-1)*u,1.3f);
  reserveMeals=new GameObject[8];for(int i=0;i<reserveMeals.Length;i++)reserveMeals[i]=StorePart("food-sack","Reserved grain portion "+i,Store+new Vector3(2.5f+(i%2)*.55f,1+(i/4)*.5f,-1.5f+(i%4/2)*.65f)*u,new Vector3(.5f,.6f,.6f));
 }
 void RefreshFoodReserve(){
  if(reserveMeals==null)return;VisibleReserveMeals=Mathf.CeilToInt(Mathf.Clamp01(MealGrain/Mathf.Max(1,FoodReserveTarget))*reserveMeals.Length);
  for(int i=0;i<reserveMeals.Length;i++)reserveMeals[i].SetActive(i<VisibleReserveMeals);
 }
}
public sealed partial class VillagerResourceGathering {
 // Reserve work competes for idle adults, never interrupts an owned activity.
 public bool TryFoodReserve(){
  if(world==null||world.Season.observeWithoutFoodWork||world.Season.paused||Active||Busy)return false;
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
