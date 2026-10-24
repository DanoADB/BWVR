using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
namespace BWVR {
public sealed class IslandAtmosphere:MonoBehaviour {
 readonly List<Transform> clouds=new List<Transform>();
 readonly List<Vector3> homes=new List<Vector3>();
 public int CloudCount=>clouds.Count;
 public int FogCount {get;private set;}
 public const int CloudLimit=6,FogLimit=3,ParticlesPerPocket=4;
 float elapsed;
 public static void Build(Transform parent,IslandTerrainData terrain){
  var art=Resources.Load<IslandAtmosphereArt>("IslandAtmosphere");
  if(art==null){Debug.LogWarning("BWVR_ATMOSPHERE_MISSING: import CC0 atmosphere artwork");return;}
  var root=new GameObject("Island weather - CC0 clouds and local mist");
  root.transform.SetParent(parent,false);root.AddComponent<IslandAtmosphere>().Initialize(art,terrain);
 }
 void Initialize(IslandAtmosphereArt art,IslandTerrainData terrain){
  var bounds=terrain!=null?terrain.mesh.bounds:new Bounds(new Vector3(0,.82f,0),new Vector3(1.8f,.1f,1.8f));
  float radius=terrain!=null?1.35f:.8f;
  for(int i=0;i<CloudLimit;i++){
   float a=(i*60+20)*Mathf.Deg2Rad;
   var at=bounds.center+new Vector3(Mathf.Cos(a)*radius,0,Mathf.Sin(a)*radius);
   at.y=bounds.max.y+.22f+(i%3)*.09f;
   var g=new GameObject("Quaternius low cloud "+(i+1));g.transform.SetParent(transform,false);g.transform.localPosition=at;
   g.transform.localRotation=Quaternion.Euler(0,i*71,0);float size=.45f+(i%3)*.13f;
   g.transform.localScale=new Vector3(size,size*.55f,size*.8f);g.AddComponent<MeshFilter>().sharedMesh=art.cloud;
   var r=g.AddComponent<MeshRenderer>();r.sharedMaterial=art.cloudMaterial;r.shadowCastingMode=ShadowCastingMode.On;r.receiveShadows=false;
   clouds.Add(g.transform);homes.Add(at);
  }
  var sites=new List<Vector3>();
  if(terrain!=null&&terrain.HasPlacements){
   foreach(var p in terrain.placements){
    if(p.kind!="tree")continue;var at=terrain.PlacementPoint(p);
    if(new Vector2(at.x,at.z).magnitude<.4f||!terrain.Dry(at))continue;
    bool blocked=false;
    foreach(var existing in sites)if(Vector3.Distance(existing,at)<.5f){blocked=true;break;}
    if(blocked)continue;
    foreach(var other in terrain.placements)if((other.kind=="building"||other.kind=="field"||other.kind=="drink")&&Vector3.Distance(terrain.PlacementPoint(other),at)<.14f){blocked=true;break;}
    if(blocked)continue;sites.Add(at);if(sites.Count==FogLimit)break;
   }
  }else for(int i=0;i<FogLimit;i++)sites.Add(new Vector3(-.6f+i*.6f,.82f,.55f));
  for(int i=0;i<sites.Count;i++)Fog(sites[i],i,art.fogMaterial);
  // Only the directional sun owns a shadow map. Cloud meshes have 480 triangles each.
  QualitySettings.shadows=Application.isMobilePlatform?ShadowQuality.HardOnly:ShadowQuality.All;
  QualitySettings.shadowResolution=ShadowResolution.Medium;QualitySettings.shadowCascades=0;QualitySettings.shadowDistance=8;
  foreach(var light in FindObjectsByType<Light>(FindObjectsSortMode.None))if(light.type==LightType.Directional){
   light.shadows=Application.isMobilePlatform?LightShadows.Hard:LightShadows.Soft;light.shadowStrength=.45f;light.shadowBias=.02f;light.shadowNormalBias=.02f;
   light.shadowCustomResolution=1024;break;
  }
  Debug.Log("BWVR_ATMOSPHERE_READY: "+CloudCount+" shadow-casting clouds, "+FogCount+" fog pockets, max "+(FogCount*ParticlesPerPocket)+" fog quads");
 }
 void Fog(Vector3 at,int index,Material material){
  var go=new GameObject("Kenney woodland mist "+(index+1));go.transform.SetParent(transform,false);go.transform.localPosition=at+Vector3.up*.025f;
  var ps=go.AddComponent<ParticleSystem>();ps.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
  ps.useAutoRandomSeed=false;ps.randomSeed=(uint)(42+index);
  var main=ps.main;main.loop=true;main.prewarm=true;main.duration=24;main.startLifetime=24;main.startSpeed=0;main.maxParticles=ParticlesPerPocket;
  main.simulationSpace=ParticleSystemSimulationSpace.Local;main.startSize3D=true;main.startSizeX=.23f;main.startSizeY=.055f;main.startSizeZ=.1f;
  var emission=ps.emission;emission.rateOverTime=ParticlesPerPocket/24f;
  var shape=ps.shape;shape.shapeType=ParticleSystemShapeType.Box;shape.scale=new Vector3(.18f,.018f,.12f);
  var color=ps.colorOverLifetime;color.enabled=true;
  var gradient=new Gradient();gradient.SetKeys(new[]{new GradientColorKey(Color.white,0),new GradientColorKey(Color.white,1)},new[]{new GradientAlphaKey(0,0),new GradientAlphaKey(1,.2f),new GradientAlphaKey(1,.8f),new GradientAlphaKey(0,1)});color.color=gradient;
  var r=ps.GetComponent<ParticleSystemRenderer>();r.sharedMaterial=material;r.renderMode=ParticleSystemRenderMode.Billboard;r.shadowCastingMode=ShadowCastingMode.Off;r.receiveShadows=false;r.maxParticleSize=.12f;
  ps.Play();FogCount++;
 }
 void Update(){
  elapsed+=Time.deltaTime;
  for(int i=0;i<clouds.Count;i++)clouds[i].localPosition=homes[i]+new Vector3(Mathf.Sin(elapsed*.018f+i)*.07f,Mathf.Sin(elapsed*.012f+i)*.008f,Mathf.Cos(elapsed*.015f+i)*.035f);
 }
}
}
