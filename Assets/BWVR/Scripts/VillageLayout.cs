using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace BWVR {
// Modern village plots are planned around real building footprints and shared routes.
public sealed class VillageLayout {
 public sealed class Plot {public Vector3 position;public float yaw,radius,scale;public string model;}
 public readonly Dictionary<int,Plot> plots=new Dictionary<int,Plot>();
 readonly IslandTerrainData terrain;
 public Vector3 centre,well;
 public VillageLayout(IslandTerrainData data,EnvironmentArt buildings){
  terrain=data;
  var storage=data.placements.Where(p=>p.kind=="building"&&p.type.Contains("STORAGE_PIT")).OrderBy(p=>data.PlacementPoint(p).sqrMagnitude).FirstOrDefault();
  centre=storage!=null?data.PlacementPoint(storage):data.SafePoint(Vector3.zero);
  var drink=data.placements.Where(p=>p.kind=="drink").OrderBy(p=>(data.PlacementPoint(p)-centre).sqrMagnitude).FirstOrDefault();
  well=drink!=null?data.PlacementPoint(drink):centre+Vector3.right*.18f;
  var routes=new List<Vector3>{well};
  foreach(string kind in new[]{"field","tree"}){
   var target=data.placements.Where(p=>p.kind==kind).OrderBy(p=>(data.PlacementPoint(p)-(kind=="tree"?new Vector3(-.064f,.82f,-.012f):centre)).sqrMagnitude).FirstOrDefault();
   if(target!=null)routes.Add(data.PlacementPoint(target));
  }
  var towns=new Dictionary<int,int>();
  foreach(var p in data.placements.Where(p=>p.kind=="building"&&!p.type.Contains("STORAGE_PIT")&&!p.type.Contains("GRAVEYARD"))){
   towns.TryGetValue(p.town,out int number);towns[p.town]=number+1;
   string key=number==4?"Inn":number==7?"Blacksmith":new[]{"House_A","House_B","House_C"}[number%3];
   var prefab=buildings.Find(key);if(prefab==null)throw new Exception("Missing medieval building "+key);
   float scale=3.66f*data.scale*p.size/.195f*QuestSlice.BuildingScale;
   var box=prefab.GetComponent<BoxCollider>();float radius=new Vector2(box.size.x,box.size.z).magnitude*.5f*scale;
   var preferred=data.PlacementPoint(p);Vector3 selected=default;bool found=false;
   // Staggered rings give organic spacing while leaving service corridors open.
   for(int ring=0;ring<45&&!found;ring++)for(int sample=0;sample<(ring==0?1:32);sample++){
    float angle=(sample*137.507f+p.line*29)*Mathf.Deg2Rad;
    var candidate=preferred+new Vector3(Mathf.Cos(angle),0,Mathf.Sin(angle))*ring*.018f;
    if(!Fits(candidate,radius,routes))continue;selected=data.Project(candidate);found=true;break;
   }
   if(!found)throw new Exception("No safe plot found for "+key+" at source line "+p.line);
   var facing=centre-selected;facing.y=0;
   plots[p.line]=new Plot{model=key,position=selected,yaw=Quaternion.LookRotation(facing).eulerAngles.y,radius=radius,scale=scale};
  }
 }
 bool Fits(Vector3 at,float radius,List<Vector3> routes){
  if(!terrain.Dry(at))return false;
  if(FlatDistance(at,centre)<radius+.035f)return false;
  foreach(var end in routes)if(SegmentDistance(at,centre,end)<radius+.022f)return false;
  foreach(var p in plots.Values)if(FlatDistance(at,p.position)<radius+p.radius+.018f)return false;
  foreach(var site in terrain.placements)if((site.kind=="field"||site.type.Contains("STORAGE_PIT"))&&FlatDistance(at,terrain.PlacementPoint(site))<radius+.02f)return false;
  float y=terrain.Project(at).y;
  for(int i=0;i<8;i++){float a=i*Mathf.PI/4;var edge=at+new Vector3(Mathf.Cos(a),0,Mathf.Sin(a))*radius;if(!terrain.Dry(edge)||Mathf.Abs(terrain.Project(edge).y-y)>terrain.scale*1.2f)return false;}
  return true;
 }
 public bool Cleared(Vector3 at){foreach(var p in plots.Values)if(FlatDistance(at,p.position)<p.radius+.012f)return true;return false;}
 static float FlatDistance(Vector3 a,Vector3 b)=>new Vector2(a.x-b.x,a.z-b.z).magnitude;
 public static float SegmentDistance(Vector3 at,Vector3 start,Vector3 end){
  var p=new Vector2(at.x,at.z);var a=new Vector2(start.x,start.z);var delta=new Vector2(end.x-start.x,end.z-start.z);float t=delta.sqrMagnitude>0?Mathf.Clamp01(Vector2.Dot(p-a,delta)/delta.sqrMagnitude):0;return Vector2.Distance(p,a+delta*t);
 }
}
}
