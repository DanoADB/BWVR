using System;
using UnityEngine;
namespace BWVR {
[Serializable] public sealed class SliceState {
 public int version=1; public float seconds=180, water=52, crops=66, trust=20, care=0.25f; public int gifts; public bool paused;
 public float hunger=.15f,playPreference=.5f,socialPreference=.5f;
 [NonSerialized]public bool developmentFocus;
 [NonSerialized]public bool observeWithoutFoodWork;
 public ConstructionRecord[] construction;
 public SocialMemory[] relationships;
 public DiscoveryRecord[] discoveries;public int creatureDiscoveries;
 public HybridState hybrid;public HybridState Hybrid=>hybrid??(hybrid=new HybridState());
 public BuildingMemory building;public BuildingMemory Building=>building??(building=new BuildingMemory());
 public WorldPlayMemory worldPlay;public WorldPlayMemory WorldPlay=>worldPlay??(worldPlay=new WorldPlayMemory());
 public FamilyState families;public FamilyState Families{get{var value=families??(families=new FamilyState());value.EnsurePopulation();return value;}}
 public ResourceState resources;public ResourceState Resources{get{var value=resources??(resources=new ResourceState{wheat=VillagePopulation.Adults*2});value.EnsurePopulation();return value;}}
 public ToolTechnologyState tools;public ToolTechnologyState Tools{get{var value=tools??(tools=new ToolTechnologyState());value.EnsurePopulation();return value;}}
 public WeatherState weather;public WeatherState Weather=>weather??(weather=new WeatherState());
 public ReligionState religion;public ReligionState Religion=>religion??(religion=new ReligionState());
 public TributeMemory tribute;public TributeMemory Tribute=>tribute??(tribute=new TributeMemory());
 public bool outcomeRecorded,outcomeWon;public void RememberOutcome(){if(Finished&&!outcomeRecorded){outcomeWon=water>=25&&crops>=40;outcomeRecorded=true;}}
 public SocialMemory Social(int index){if(relationships==null||relationships.Length==0)relationships=VillagePopulation.Create(VillagePopulation.Social);VillagePopulation.Expand(ref relationships,VillagePopulation.Social);return relationships[index];}
 bool SocialValid(){if(relationships==null||relationships.Length==0)return true;if(!VillagePopulation.Supported(relationships.Length))return false;foreach(var m in relationships)if(m==null||!m.Valid())return false;return true;}
 public bool Finished=>seconds<=0; public bool Won=>Finished&&(outcomeRecorded?outcomeWon:water>=25&&crops>=40);
 public void Step(float dt){if(developmentFocus||paused||Finished||float.IsNaN(dt)||float.IsInfinity(dt)||dt<=0)return;dt=Mathf.Min(dt,seconds);seconds-=dt;water=Mathf.Clamp(water-dt*.27f,0,100);crops=Mathf.Clamp(crops-dt*(water<25?.2f:.04f),0,100);RememberOutcome();}
 public void Deliver(){if(Finished)return;water=Mathf.Min(100,water+8);gifts++;trust=Mathf.Min(100,trust+4);}
 public void Praise(){if(Finished)return;care=Mathf.Min(1,care+.12f);trust=Mathf.Min(100,trust+3);}
 public bool Valid()=>version==1&&(religion==null||religion.Valid())&&(tools==null||tools.Valid())&&(families==null||families.Valid())&&(resources==null||resources.Valid())&&(weather==null||weather.Valid())&&Finite(seconds,0,180)&&Finite(water,0,100)&&Finite(crops,0,100)&&Finite(trust,0,100)&&Finite(care,0,1)&&Finite(hunger,0,1)&&Finite(playPreference,0,1)&&Finite(socialPreference,0,1)&&ConstructionSite.Valid(construction)&&SocialValid()&&IslandDiscoveries.Valid(discoveries)&&(hybrid==null||hybrid.Valid())&&(building==null||building.Valid())&&(worldPlay==null||worldPlay.Valid())&&(tribute==null||tribute.Valid())&&creatureDiscoveries>=0&&creatureDiscoveries<=6&&gifts>=0&&gifts<=10000;
 static bool Finite(float n,float min,float max)=>!float.IsNaN(n)&&!float.IsInfinity(n)&&n>=min&&n<=max;
}
}
