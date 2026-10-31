using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
namespace BWVR {
// Scheduler-owned building work. Transit timber is saved per stable villager identity.
public sealed partial class VillageCathedral:MonoBehaviour {
 public static readonly string[] Stages={"Shrine","Chapel","VillageChurch","GothicChurch","GrandCathedral"};
 public CathedralState State=>world.Season.Cathedral;
 public Vector3 Entrance{get;private set;}public Vector3 PlotSize{get;private set;}
 public int Workers=>phase.Count(p=>p>0);public string Description=>State.level==5?"Grand cathedral":(State.level==0?"No shrine":Stages[State.level-1])+(State.Building?" · building "+Mathf.RoundToInt(100*(State.timber/CathedralState.Timber[State.level]+State.work/CathedralState.Labor[State.level])*.5f)+"%":"");
 VillageResources world;VillageFamilies homes;VillagerRoutine[] jobs;SliceState known;GameObject[] models,loads,piles;GameObject obstacle,plot;Transform view;Material stone;int shown=-1;int[] phase;float[] elapsed,retry;Vector3[] workAt;Vector3 fallback;
 public static VillageCathedral Build(Transform parent,VillageResources resources,VillageFamilies families,VillagerRoutine[] actors,Vector3 village){var go=new GameObject("Creature cathedral");go.transform.SetParent(parent,false);var system=go.AddComponent<VillageCathedral>();system.Initialize(resources,families,actors,village);return system;}
 void Initialize(VillageResources resources,VillageFamilies families,VillagerRoutine[] actors,Vector3 village){world=resources;homes=families;jobs=actors;phase=new int[jobs.Length];elapsed=new float[jobs.Length];retry=new float[jobs.Length];workAt=new Vector3[jobs.Length];models=new GameObject[5];view=new GameObject("Five completed building stages").transform;view.SetParent(transform,false);
  for(int i=0;i<5;i++){var prefab=Resources.Load<GameObject>("Cathedrals/"+Stages[i]);if(prefab==null)throw new InvalidOperationException("Missing CC0 cathedral stage "+Stages[i]);models[i]=Instantiate(prefab,view);models[i].SetActive(false);var size=prefab.GetComponent<BoxCollider>().size;PlotSize=new Vector3(Mathf.Max(PlotSize.x,size.x+4),Mathf.Max(PlotSize.y,size.y),Mathf.Max(PlotSize.z,size.z+4));}
  if(!FindSite(village,out fallback))throw new InvalidOperationException("No clear, reachable cathedral plot; refusing to overlap the settlement");
  loads=new GameObject[jobs.Length];for(int i=0;i<jobs.Length;i++){loads[i]=world.CreateLoad(VillageResource.Wood);loads[i].name="Cathedral builder timber";loads[i].SetActive(false);}
  piles=new GameObject[12];for(int i=0;i<piles.Length;i++){piles[i]=world.CreateLoad(VillageResource.Wood);piles[i].name="Delivered cathedral timber";piles[i].SetActive(false);}
  obstacle=new GameObject("Reserved cathedral native footprint");obstacle.transform.SetParent(transform,false);var carve=obstacle.AddComponent<NavMeshObstacle>();carve.shape=NavMeshObstacleShape.Box;carve.size=new Vector3(PlotSize.x,100,PlotSize.z);carve.center=Vector3.up*40;carve.carving=true;carve.carveOnlyStationary=true;
  stone=new Material(Shader.Find("BWVR/GameSurface")){color=new Color(.53f,.49f,.4f),enableInstancing=true};stone.SetFloat("_Cutoff",0);plot=SourcedItemArt.Fit("SettlementProps/structure-floor",transform,"Cathedral foundation",Vector3.zero,Vector3.one,stone);BuildConstructionView();BuildBell();ResetWorld();
 }
 bool FindSite(Vector3 village,out Vector3 site){var nav=world.Navigation;float u=nav.Scale;var discoveries=nav.GetComponentInChildren<IslandDiscoveries>();
  for(int i=0;i<320;i++){float angle=i*137.5f*Mathf.Deg2Rad;var center=village+new Vector3(Mathf.Cos(angle),0,Mathf.Sin(angle))*(40+i*.4f)*u;float low=float.MaxValue,high=float.MinValue;bool clear=true;
   // Grid and segment checks reject trunks, existing buildings, water and steep ground.
   for(int z=0;z<=6&&clear;z++)for(int x=0;x<=4;x++){var wanted=center+new Vector3((x/4f-.5f)*PlotSize.x,0,(z/6f-.5f)*PlotSize.z)*u;if(!nav.Sample(wanted,nav.villager,.3f,out var at)){clear=false;break;}low=Mathf.Min(low,at.y);high=Mathf.Max(high,at.y);if(x>0&&!nav.SegmentClear(at-new Vector3(PlotSize.x/4*u,0,0),at,nav.villager)){clear=false;break;}}
   if(!clear||high-low>u*2)continue;if(world.sources.Any(s=>Mathf.Abs(s.at.x-center.x)<(PlotSize.x/2+12)*u&&Mathf.Abs(s.at.z-center.z)<(PlotSize.z/2+12)*u))continue;var yard=nav.GetComponentsInChildren<ConstructionSite>().FirstOrDefault(s=>!s.IsBridge);if(yard!=null&&yard.Contains(yard.Local(center),Mathf.Max(PlotSize.x,PlotSize.z)))continue;
   var start=center+Vector3.back*(PlotSize.z/2+10)*u;if(!nav.Sample(start,nav.villager,1,out var approach)||!discoveries.Reachable(world.StoreApproach,approach,nav.villager))continue;
   if(Physics.OverlapBox(new Vector3(center.x,high+10*u,center.z),new Vector3(PlotSize.x/2,8,PlotSize.z/2)*u,Quaternion.identity,~0,QueryTriggerInteraction.Ignore).Any(c=>c.GetComponentInParent<IslandPlacementView>()!=null||c.GetComponentInParent<VillageResources>()!=null||c.GetComponentInParent<ConstructionSite>()!=null))continue;
   site=new Vector3(center.x,high,center.z);return true;
  }site=default;return false;
 }
 public void Advance(float dt){if(world.Season.paused||!HybridState.Number(dt,0,1)||dt<=0)return;EnsureSeason();State.Advance(dt,world.VillageFaith);Refresh();}
 void EnsureSeason(){if(known==world.Season)return;for(int i=0;i<jobs.Length;i++){Cancel(jobs[i]);retry[i]=0;}known=world.Season;shown=-1;ResetBellPresentation();if(!State.sited){State.site=world.Navigation.ToNav(fallback);State.sited=true;}var center=world.Navigation.ToWorld(State.site);float u=world.Navigation.Scale;view.position=center+Vector3.back*(PlotSize.z/2-2)*u;view.localScale=Vector3.one*u;Entrance=view.position+Vector3.back*8*u;
  obstacle.transform.position=world.Navigation.ToNav(center);plot.transform.position=center-Vector3.up*u*.5f;plot.transform.localScale=new Vector3(PlotSize.x,1,PlotSize.z)*u;world.Navigation.TreeObstacleChanged();Refresh();}
 public void ResetWorld(){known=null;EnsureSeason();}
 public bool Active(VillagerRoutine job){int i=Array.IndexOf(jobs,job);return i>=0&&phase[i]>0;}
 public bool Tick(VillagerRoutine job,float dt){if(world.Season.paused||!HybridState.Number(dt,0,1)||dt<=0)return false;EnsureSeason();int i=Array.IndexOf(jobs,job);if(i<0)return false;var social=job.GetComponent<VillagerSocial>();var nav=job.GetComponent<IslandNavigator>();var motion=job.GetComponent<HumanoidMotion>();bool unsafeActor=job.GetComponent<CarryItem>().held||social.RecentFright||social.Memory.injury>.05f||world.Season.Religion.fanatic||job.Worshipping;
  if(!State.Building||unsafeActor||phase[i]>0&&(!job.BuildingCathedral||social.Active||job.GetComponent<VillagerResourceGathering>().Active||job.GetComponent<VillagerExploration>().Active||job.GetComponent<VillagerBlockPlay>().Active)){Cancel(job);return false;}
  if(phase[i]==0){if(world.Season.Hybrid.time<retry[i]||Workers>=2||social.Active||job.Signalling||job.GetComponent<VillagerResourceGathering>().Active||job.GetComponent<VillagerExploration>().Active||job.GetComponent<VillagerBlockPlay>().Active||homes.Owns(job))return false;
   if(State.cargo[i]<=0&&State.Remaining>0&&world.FreeWood<.01f)return false;if(State.cargo[i]<=0&&State.Remaining==0&&State.timber+.001f<CathedralState.Timber[State.level])return false;
   if(!world.Navigation.Sample(Entrance+Vector3.right*(i%2==0?-7:7)*world.Navigation.Scale,nav.profile,2,out var point))return false;workAt[i]=world.Navigation.ProjectFoot(point,nav.profile);job.BeginCathedral();elapsed[i]=0;phase[i]=State.cargo[i]>0?2:State.timber+.001f>=CathedralState.Timber[State.level]?3:1;
  }
  elapsed[i]+=dt;var target=phase[i]==1?world.StoreApproach:workAt[i];if(phase[i]==2){loads[i].SetActive(true);motion.Carry(loads[i].transform);}bool arrived=nav.MoveTo(target,job.transform.lossyScale.y*1.6f,dt);
  if(!arrived){motion.Activity(0);if(nav.Blocked&&elapsed[i]>8||elapsed[i]>90){Cancel(job);retry[i]=world.Season.Hybrid.time+20;}return Active(job);}
  elapsed[i]=0;if(phase[i]==1){if(State.Collect(world.State,i,world.FreeWood)<=0){Cancel(job);retry[i]=world.Season.Hybrid.time+10;return false;}phase[i]=2;loads[i].transform.position=job.transform.position;loads[i].SetActive(true);motion.Carry(loads[i].transform);world.Refresh();}
  else if(phase[i]==2){State.Deliver(i);loads[i].SetActive(false);motion.Carry(null);phase[i]=3;Refresh();}
  else{var toward=view.position-job.transform.position;toward.y=0;if(toward.sqrMagnitude>1e-10f)job.transform.rotation=Quaternion.LookRotation(toward);motion.Activity(4);if(State.Build(dt)){foreach(var actor in jobs)Cancel(actor);var god=world.GetComponentInParent<QuestSlice>().GetComponentInChildren<CreatureHybrid>();god.Journal.Add(world.Season.Hybrid.time,"cathedral completed",CreatureAction.IdleOrObserve,"village",Stages[State.level-1]);Refresh();return false;}if(State.timber+.001f<CathedralState.Timber[State.level]){Cancel(job);retry[i]=world.Season.Hybrid.time+2;return false;}}
  return true;
 }
 public void Cancel(VillagerRoutine job){int i=Array.IndexOf(jobs,job);if(i<0||phase==null||phase[i]==0)return;phase[i]=0;elapsed[i]=0;if(loads!=null)loads[i].SetActive(false);if(job.BuildingCathedral){job.GetComponent<HumanoidMotion>().Carry(null);job.ResetTask();}}
 void Refresh(){if(models==null)return;var s=State;if(shown!=s.level){for(int i=0;i<5;i++)models[i].SetActive(s.level==i+1);shown=s.level;}bool occupied=s.level>0||s.Building;if(obstacle.activeSelf!=occupied){obstacle.SetActive(occupied);world.Navigation.TreeObstacleChanged();}plot.SetActive(occupied);float u=world.Navigation.Scale;int foundationStage=Mathf.Clamp((s.Building?s.authorized:s.level)-1,0,4);var foundationSize=models[foundationStage].GetComponent<BoxCollider>().size;plot.transform.localScale=new Vector3(foundationSize.x+2,1,foundationSize.z+2)*u;plot.transform.position=view.position+Vector3.forward*foundationSize.z*.5f*u-Vector3.up*.5f*u;for(int i=0;i<piles.Length;i++){piles[i].SetActive(s.Building&&s.timber>i*3);piles[i].transform.position=Entrance+new Vector3(-3+(i%3)*2,.6f+(i/6)*.8f,-3+(i%6/3)*2)*u;}RefreshConstructionView();RefreshBell();}
 void OnDestroy(){DisposeConstructionView();DisposeBell();if(stone!=null)Destroy(stone);if(loads!=null)foreach(var load in loads)if(load!=null)Destroy(load);if(piles!=null)foreach(var pile in piles)if(pile!=null)Destroy(pile);}
}
}
