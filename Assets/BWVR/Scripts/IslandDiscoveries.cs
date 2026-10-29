using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
namespace BWVR {
[Serializable] public sealed class DiscoveryRecord {
 public int visitors;public float restock;public ConstructionRecord[] blocks;
}
// A small fixed set of reachable clearings, not a per-frame world spawning system.
public sealed class IslandDiscoveries:MonoBehaviour {
 public readonly List<DiscoveryPoint> points=new List<DiscoveryPoint>();
 Func<SliceState> season;AudioClip reward;IslandNavigation nav;
 public DiscoveryRecord Memory(int id){var s=season();if(s.discoveries==null||s.discoveries.Length==0){s.discoveries=new DiscoveryRecord[6];for(int i=0;i<6;i++)s.discoveries[i]=new DiscoveryRecord();}return s.discoveries[id];}
 public static bool Valid(DiscoveryRecord[] records){if(records==null||records.Length==0)return true;if(records.Length!=6)return false;for(int i=0;i<6;i++){var r=records[i];if(r==null||r.visitors<0||r.visitors>((1<<(VillagePopulation.Adults+1))-1)||float.IsNaN(r.restock)||float.IsInfinity(r.restock)||r.restock<0||r.restock>90||!ConstructionSite.Valid(r.blocks)||(r.blocks!=null&&r.blocks.Length!=0&&r.blocks.Length!=8))return false;if(i>=2&&r.blocks!=null&&r.blocks.Length!=0)return false;}return true;}
 public static IslandDiscoveries Build(Transform parent,IslandNavigation navigation,Vector3 village,ConstructionSite original,Func<SliceState> current){
  var root=new GameObject("Island discovery clearings");root.transform.SetParent(parent,false);var world=root.AddComponent<IslandDiscoveries>();world.Initialize(navigation,village,original,current);return world;
 }
 void Initialize(IslandNavigation navigation,Vector3 village,ConstructionSite original,Func<SliceState> current){
  nav=navigation;season=current;
  for(int cache=0;cache<2;cache++){original.ImportLegacyCache(cache,Memory(cache).blocks);Memory(cache).blocks=null;}
  if(season().relationships!=null)foreach(var memory in season().relationships){if(memory==null)continue;memory.usefulStructures=(memory.usefulStructures&0xF000)|((memory.usefulStructures|(memory.usefulStructures>>4)|(memory.usefulStructures>>8))&15);if(memory.gatheringSite==1||memory.gatheringSite==2)memory.gatheringSite=0;}
  const int rate=22050;var samples=new float[rate/2];for(int i=0;i<samples.Length;i++){float t=i/(float)rate;samples[i]=Mathf.Sin(t*(t<.18f?660:880)*Mathf.PI*2)*Mathf.Exp(-t*7)*.16f;}reward=AudioClip.Create("Warm discovery notes",samples.Length,1,rate,false);reward.SetData(samples,0);
  for(int id=0;id<6;id++){
   bool placed=false;Vector3 at=default;
   for(int attempt=0;attempt<120;attempt++){
    float angle=(id*60+attempt*137.5f)*Mathf.Deg2Rad;float radius=55+id*23+(attempt/12)*9;
    var wanted=village+new Vector3(Mathf.Cos(angle),0,Mathf.Sin(angle))*radius*nav.Scale;
    if(!nav.Sample(wanted,nav.creature,5,out at)||!nav.Sample(at,nav.villager,1,out _)||!Reachable(village,at,nav.creature)||!Reachable(village,at,nav.villager))continue;
    if(Vector3.Distance(at,original.transform.position)<nav.Scale*34||points.Any(p=>Vector3.Distance(at,p.transform.position)<nav.Scale*45))continue;
    if(nav.Terrain!=null&&!nav.Terrain.Dry(at))continue;placed=true;break;
   }
   if(!placed){Debug.LogWarning("BWVR_DISCOVERY_SITE_SKIPPED: no reachable clearing for "+id);continue;}
   var root=new GameObject("Discovery clearing "+(id+1));root.transform.SetParent(transform,false);root.transform.position=nav.Terrain!=null?nav.Terrain.Project(at):at;
   var point=root.AddComponent<DiscoveryPoint>();point.Initialize(this,id,nav.Scale,reward);
   points.Add(point);
   for(int i=0;i<2;i++){
    Vector3 wanted=root.transform.position+new Vector3(i==0?-14:14,0,0)*nav.Scale;
    if(!nav.Sample(wanted,nav.creature,8,out var safe)||!nav.Sample(safe,nav.villager,2,out _)){Debug.LogWarning("BWVR_DISCOVERY_PROP_SKIPPED: "+id+"/"+i);continue;}
    if(nav.Terrain!=null)safe=nav.Terrain.Project(safe);
    CreateObject(point,safe,i==1,id%2==0);
   }
   // Small, collider-free stones distinguish clearings without adding navigation obstacles.
   for(int n=0;n<5;n++){float a=n*Mathf.PI*2/5;var position=root.transform.position+new Vector3(Mathf.Cos(a),0,Mathf.Sin(a))*(id<2?13:5)*nav.Scale;if(nav.Terrain!=null)position=nav.Terrain.Project(position);var stone=SourcedItemArt.Fit("SettlementProps/rock-"+(char)('a'+n%3),root.transform,"Clearing stone",Vector3.zero,Vector3.one);stone.transform.position=position;stone.transform.localScale=new Vector3(1.3f,.5f,1)*nav.Scale;}
  }
 }
 void CreateObject(DiscoveryPoint point,Vector3 at,bool treat,bool mallet){
  float u=nav.Scale;var go=new GameObject(treat?"Strawberry treat":mallet?"Play hammer":"Seed shaker");go.transform.SetParent(point.transform,false);go.transform.position=at+Vector3.up*u*(treat?.55f:1.1f);go.transform.localScale=Vector3.one*u*.55f;
  SourcedItemArt.Fit(treat?"FoodProps/strawberry":mallet?"SettlementProps/tool-hammer":"SettlementProps/bottle",go.transform,"CC0 discovery artwork",treat||mallet?Vector3.zero:Vector3.down*.3f,treat?Vector3.one*2:mallet?new Vector3(3,4,1.5f):new Vector3(2,3.4f,1.4f));
  var box=go.AddComponent<BoxCollider>();box.size=treat?Vector3.one*2:new Vector3(3,4,1.5f);box.contactOffset=u*.02f;
  var item=go.AddComponent<CarryItem>();item.kind=treat?Offering.Food:Offering.Toy;item.home=go.transform.position;
  var discovered=go.AddComponent<DiscoveryObject>();discovered.point=point;discovered.treat=treat;
  go.AddComponent<CreatureAffordance>().kind=treat?CreatureAffordance.Kind.Food:mallet?CreatureAffordance.Kind.Turn:CreatureAffordance.Kind.Chime;
  if(!treat&&!mallet){var audio=go.AddComponent<AudioSource>();audio.clip=reward;audio.playOnAwake=false;audio.spatialBlend=1;audio.minDistance=u*10;audio.maxDistance=u*100;audio.volume=.25f;}
  HybridTarget.Bind(go,"object-"+(point.id==0?22+point.objects.Count:point.id==1?32+point.objects.Count:34+(point.id-2)*2+point.objects.Count),HybridTargetKind.Object);
  point.objects.Add(discovered);
 }
 public bool Reachable(Vector3 from,Vector3 to,IslandNavigation.Profile profile){if(!nav.Sample(from,profile,12,out var start))return false;var path=new NavMeshPath();return NavMesh.CalculatePath(nav.ToNav(start),nav.ToNav(to),new NavMeshQueryFilter{agentTypeID=profile.id,areaMask=1},path)&&path.status==NavMeshPathStatus.PathComplete;}
 public void Advance(float dt,bool paused){foreach(var p in points){if(p.construction!=null)p.construction.Paused=paused;if(!paused&&dt>0)p.Advance(dt);}}
 public void Capture(){foreach(var p in points)if(p.construction!=null)p.Record.blocks=p.construction.Capture();}
 public void ResetWorld(){foreach(var p in points){p.ResetPoint();p.construction?.ResetConstruction();}}
 void OnDestroy(){if(reward!=null)Destroy(reward);}
}
}
