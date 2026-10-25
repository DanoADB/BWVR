using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
namespace BWVR {
// One-Hz thermal/fuel model; existing private PhysX scenes retain projectile ownership.
public sealed class IslandWeather:MonoBehaviour {
 public bool automatic=true;
 public WeatherState State=>season().Weather;
 public int BurningCount=>State.materials.Count(m=>m.burning);
 public Vector3 Wind=>new Vector3(.8f,0,.6f)*State.wind;
 public IReadOnlyList<PlayImpactTarget> Targets=>targets;
 Func<SliceState> season;IslandNavigation nav;IslandOcean ocean;IslandAtmosphere atmosphere;Vector3 cistern;Bounds island;
 Transform rainFocus;PlayImpactTarget[] targets;BeachBall[] balls;TreePlayWorld trees;Light sun;float originalSun,boltRemaining;
 readonly Dictionary<string,PlayImpactTarget> byId=new Dictionary<string,PlayImpactTarget>();
 readonly Dictionary<Vector2Int,List<PlayImpactTarget>> cells=new Dictionary<Vector2Int,List<PlayImpactTarget>>();
 readonly Dictionary<PlayImpactTarget,Bounds> footprints=new Dictionary<PlayImpactTarget,Bounds>();
 readonly List<PlayImpactTarget> neighbors=new List<PlayImpactTarget>();readonly HashSet<PlayImpactTarget> seen=new HashSet<PlayImpactTarget>();
 readonly List<WeatherMaterial> spreading=new List<WeatherMaterial>();
 ParticleSystem rain;readonly List<ParticleSystem> flames=new List<ParticleSystem>();Material rainMaterial,fireMaterial,boltMaterial;LineRenderer bolt;
 public static IslandWeather Build(Transform parent,IslandNavigation navigation,Vector3 reservoir,Func<SliceState> current){var go=new GameObject("Island weather and material interactions");go.transform.SetParent(parent,false);var world=go.AddComponent<IslandWeather>();world.Initialize(navigation,reservoir,current);return world;}
 void Initialize(IslandNavigation navigation,Vector3 reservoir,Func<SliceState> current){
  nav=navigation;cistern=reservoir;season=current;targets=nav.GetComponentsInChildren<PlayImpactTarget>().OrderBy(t=>t.id,StringComparer.Ordinal).ToArray();foreach(var t in targets)byId[t.id]=t;
  rainFocus=nav.GetComponentInChildren<CreatureHybrid>().transform;balls=nav.GetComponentsInChildren<BeachBall>();trees=nav.GetComponentInChildren<TreePlayWorld>();ocean=nav.GetComponentInChildren<IslandOcean>();atmosphere=nav.GetComponentInChildren<IslandAtmosphere>();
  island=nav.Terrain!=null?nav.Terrain.mesh.bounds:new Bounds(new Vector3(0,.82f,0),new Vector3(2,.2f,2));sun=UnityEngine.Object.FindObjectsByType<Light>().FirstOrDefault(l=>l.type==LightType.Directional);if(sun!=null)originalSun=sun.intensity;
  var art=Resources.Load<IslandAtmosphereArt>("IslandAtmosphere");rainMaterial=new Material(Shader.Find("BWVR/LocalMist"));fireMaterial=new Material(rainMaterial);boltMaterial=new Material(rainMaterial);
  if(art!=null&&art.fogMaterial!=null){rainMaterial.mainTexture=art.fogMaterial.mainTexture;fireMaterial.mainTexture=art.fogMaterial.mainTexture;boltMaterial.mainTexture=art.fogMaterial.mainTexture;}
  rainMaterial.color=new Color(.65f,.81f,.95f,.5f);fireMaterial.color=new Color(1,.36f,.04f,.9f);boltMaterial.color=new Color(.8f,.87f,1,1);
  rain=Particles("Bounded island rain",192,rainMaterial,true);rain.transform.position=island.center+Vector3.up*(island.extents.y+nav.Scale*30);
  var shape=rain.shape;shape.shapeType=ParticleSystemShapeType.Box;shape.scale=new Vector3(nav.Scale*140,nav.Scale,nav.Scale*140);var main=rain.main;main.startLifetime=.9f;main.startSize=nav.Scale*.25f;var rainColor=rain.colorOverLifetime;rainColor.enabled=false;var velocity=rain.velocityOverLifetime;velocity.enabled=true;velocity.space=ParticleSystemSimulationSpace.World;velocity.y=-nav.Scale*45;
  for(int i=0;i<WeatherState.FireLimit;i++)flames.Add(Particles("Pooled fire "+i,10,fireMaterial,false));
  var line=new GameObject("Localized lightning - no fullscreen flash");line.transform.SetParent(transform,false);bolt=line.AddComponent<LineRenderer>();bolt.sharedMaterial=boltMaterial;bolt.positionCount=9;bolt.widthMultiplier=nav.Scale*.35f;bolt.shadowCastingMode=ShadowCastingMode.Off;bolt.receiveShadows=false;bolt.enabled=false;
  ResetWorld();
 }
 ParticleSystem Particles(string title,int budget,Material material,bool streak){var go=new GameObject(title);go.transform.SetParent(transform,false);var ps=go.AddComponent<ParticleSystem>();ps.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);var main=ps.main;main.loop=true;main.startSpeed=0;main.startLifetime=.8f;main.startSize=nav.Scale*2;main.maxParticles=budget;main.simulationSpace=ParticleSystemSimulationSpace.World;main.useUnscaledTime=false;var emission=ps.emission;emission.rateOverTime=streak?0:10;var shape=ps.shape;shape.shapeType=ParticleSystemShapeType.Cone;shape.radius=nav.Scale;shape.angle=10;var velocity=ps.velocityOverLifetime;velocity.enabled=true;velocity.y=nav.Scale*3;var color=ps.colorOverLifetime;color.enabled=true;var gradient=new Gradient();gradient.SetKeys(new[]{new GradientColorKey(new Color(1,.8f,.12f),0),new GradientColorKey(new Color(.28f,.22f,.18f),1)},new[]{new GradientAlphaKey(0,0),new GradientAlphaKey(1,.15f),new GradientAlphaKey(0,1)});color.color=gradient;var renderer=ps.GetComponent<ParticleSystemRenderer>();renderer.sharedMaterial=material;renderer.renderMode=streak?ParticleSystemRenderMode.Stretch:ParticleSystemRenderMode.Billboard;renderer.lengthScale=streak?4:1;renderer.velocityScale=0;renderer.shadowCastingMode=ShadowCastingMode.Off;renderer.receiveShadows=false;return ps;}
 public bool Strike(string id){if(!byId.TryGetValue(id,out var target)||season().paused||target.GetComponent<CarryItem>()?.held==true)return false;var center=target.Bounds(nav.Scale).center;for(int i=0;i<9;i++){float p=i/8f;bolt.SetPosition(i,center+Vector3.up*(1-p)*nav.Scale*80+new Vector3(Mathf.Sin(i*5.3f),0,Mathf.Cos(i*3.7f))*nav.Scale*(i==8?0:2));}bolt.enabled=true;boltRemaining=.18f;
  if(target.villager!=null){target.villager.Memory.injury=Mathf.Clamp01(target.villager.Memory.injury+.4f);target.villager.ReactToBall(transform,false);return true;}
  if(!Combustible(target)||BurningCount>=WeatherState.FireLimit)return true;var material=State.Touch(id);if(material==null)return true;material.heat=Mathf.Clamp(material.heat+2*(1-material.wetness*.7f),0,2);if(material.heat>=WeatherMaterialProperties.For(target.kind).ignition&&material.wetness<.65f&&material.fuel>0)material.burning=true;return true;
 }
 static bool Combustible(PlayImpactTarget target)=>target.kind=="tree"||target.kind=="building";
 public void SetWeather(IslandWeatherKind kind,float duration=60)=>State.Set(kind,duration);
 float WaveWidth=>Mathf.Max(nav.Scale*14,IslandOcean.GridSpacing*1.25f);
 public float WaveHeight(Vector3 at){if(State.kind!=IslandWeatherKind.TidalWave)return 0;float front=Mathf.Lerp(island.min.x-nav.Scale*40,island.max.x+nav.Scale*40,Mathf.Clamp01(State.phaseTime/24));float distance=(at.x-front)/WaveWidth;return Mathf.Exp(-distance*distance)*nav.Scale*8;}
 public void Advance(float dt,bool paused){if(float.IsNaN(dt)||float.IsInfinity(dt)||dt<=0)return;
  if(!paused){float left=dt;while(left>0){float step=Mathf.Min(left,1-State.stepRemainder);State.Advance(step,automatic);State.stepRemainder+=step;left-=step;if(State.stepRemainder>=.99999f){State.stepRemainder=0;MaterialsStep();}}
   boltRemaining=Mathf.Max(0,boltRemaining-dt);if(boltRemaining<=0)bolt.enabled=false;
   foreach(var ball in balls)if(ball!=null&&ball.Body!=null&&!ball.Body.isKinematic&&ball.GetComponent<CarryItem>()?.held!=true)ball.Body.AddForce(Wind*2.5f*Mathf.Min(dt,.1f),ForceMode.VelocityChange);
   if(trees!=null&&trees.Active!=null&&!trees.Body.isKinematic)trees.Body.AddForce(Wind*.6f*Mathf.Min(dt,.1f),ForceMode.VelocityChange);
  }
  Visuals(paused);
 }
 void RefreshSpatial(){foreach(var cell in cells.Values)cell.Clear();foreach(var target in targets){var bounds=target.Bounds(nav.Scale);footprints[target]=bounds;if(!Combustible(target))continue;var min=Cell(bounds.min);var max=Cell(bounds.max);for(int x=min.x;x<=max.x;x++)for(int y=min.y;y<=max.y;y++){var key=new Vector2Int(x,y);if(!cells.TryGetValue(key,out var list)){list=new List<PlayImpactTarget>();cells.Add(key,list);}list.Add(target);}}}
 Vector2Int Cell(Vector3 at)=>new Vector2Int(Mathf.FloorToInt(at.x/(nav.Scale*12)),Mathf.FloorToInt(at.z/(nav.Scale*12)));
 void Nearby(Vector3 from){neighbors.Clear();seen.Clear();var min=Cell(from-Vector3.one*nav.Scale*12);var max=Cell(from+Vector3.one*nav.Scale*12);for(int x=min.x;x<=max.x;x++)for(int y=min.y;y<=max.y;y++)if(cells.TryGetValue(new Vector2Int(x,y),out var list))foreach(var target in list)if(seen.Add(target))neighbors.Add(target);}
 void MaterialsStep(){RefreshSpatial();var state=State;spreading.Clear();int fires=BurningCount;
  if(state.kind==IslandWeatherKind.Thunderstorm){state.lightningIn-=1;if(state.lightningIn<=0){state.lightningIn=10+state.Random()*12;var candidate=targets[(int)(state.Random()*targets.Length)%targets.Length];Strike(candidate.id);fires=BurningCount;}}
  // Snapshot sources so spread never cascades across the island within a single tick.
  foreach(var m in state.materials)if(m.burning)spreading.Add(m);
  foreach(var m in state.materials){if(!byId.TryGetValue(m.id,out var target))continue;float localRain=state.rain;if(season().Hybrid.rainRemaining>0&&Vector3.Distance(target.transform.position,cistern)<nav.Scale*24)localRain=1;
   if(!m.burning&&fires>=WeatherState.FireLimit)m.heat=Mathf.Min(m.heat,.99f);bool wasBurning=m.burning;m.Step(localRain,state.wind,1,WeatherMaterialProperties.For(target.kind));if(wasBurning&&!m.burning)fires--;else if(!wasBurning&&m.burning)fires++;if(m.burning){float amount=target.kind=="building"?.0035f:.008f;Damage(target,amount*(1-m.wetness));}
  }
  foreach(var source in spreading){if(!source.burning||!byId.TryGetValue(source.id,out var origin))continue;var from=footprints[origin].center;Nearby(from);
   foreach(var target in neighbors){if(target.id==source.id||!Combustible(target)||season().WorldPlay.Damage(target.id)>=1||target.GetComponent<CarryItem>()?.held==true)continue;var bounds=footprints[target];var delta=bounds.ClosestPoint(from)-from;float distance=delta.magnitude/nav.Scale;if(distance>12)continue;var m=state.Find(target.id);if(m==null&&fires>=WeatherState.FireLimit)continue;m=m??state.Touch(target.id);if(m==null||m.burning)continue;float downwind=Mathf.Max(0,Vector3.Dot(delta.normalized,Wind.normalized));m.heat=Mathf.Clamp(m.heat+WeatherMaterial.HeatReceived(distance,downwind,state.wind,m.wetness,WeatherMaterialProperties.For(target.kind).heatCapacity),0,2);if(m.heat>=WeatherMaterialProperties.For(target.kind).ignition&&m.wetness<.65f&&m.fuel>0&&fires<WeatherState.FireLimit){m.burning=true;fires++;}
   }
  }
  float sea=nav.Terrain!=null?nav.Terrain.SeaLevel:.79f;
  foreach(var target in targets){var bounds=footprints[target];float wave=WaveHeight(bounds.center);if(wave<=nav.Scale||bounds.min.y>=sea+wave||bounds.max.y<sea-nav.Scale)continue;var m=state.Touch(target.id);if(m==null)continue;m.wetness=1;m.heat=0;m.burning=false;if(m.wave==state.wave)continue;m.wave=state.wave;Damage(target,target.villager!=null?.12f:.08f);}
  foreach(var ball in balls){if(ball==null||ball.GetComponent<CarryItem>()?.held==true||WaveHeight(ball.transform.position)<=nav.Scale||ball.transform.position.y-ball.Radius*nav.Scale>sea+WaveHeight(ball.transform.position))continue;var memory=state.Touch(ball.StableId);if(memory==null||memory.wave==state.wave)continue;memory.wave=state.wave;memory.wetness=1;ball.EnvironmentalImpulse(new Vector3(8,4,0));}
  foreach(var resident in targets){if(resident.villager==null||resident.GetComponent<CarryItem>()?.held==true)continue;var nearby=spreading.FirstOrDefault(m=>m.burning&&byId.TryGetValue(m.id,out var hot)&&Vector3.Distance(hot.transform.position,resident.transform.position)<nav.Scale*10);if(nearby==null)continue;resident.villager.Memory.injury=Mathf.Clamp01(resident.villager.Memory.injury+.015f);if(Mathf.FloorToInt(state.time)%8==0)resident.villager.ReactToBall(byId[nearby.id].transform,false);}
  // Rain replenishes a little water; the existing miracle owns its larger, paid resource delta.
  state.materials=Array.FindAll(state.materials,m=>m.fuel<1||m.heat>.001f||m.burning||Mathf.Abs(m.wetness-state.wetness)>.05f||(state.kind==IslandWeatherKind.TidalWave&&m.wave==state.wave));
  season().water=Mathf.Min(100,season().water+state.rain*.08f);season().crops=Mathf.Min(100,season().crops+state.rain*.02f);
 }
 void Damage(PlayImpactTarget target,float amount){if(target.GetComponent<CarryItem>()?.held==true)return;if(target.villager!=null){target.villager.Memory.injury=Mathf.Clamp01(target.villager.Memory.injury+amount);target.villager.ReactToBall(transform,false);return;}target.Apply(season().WorldPlay.Hurt(target.id,amount));}
 void Visuals(bool paused){var s=State;RenderSettings.fogDensity=Mathf.Lerp(.065f,.3f,s.fog);if(sun!=null)sun.intensity=originalSun*Mathf.Lerp(1,.55f,s.rain);
  atmosphere?.SetWeather(s.time,Wind,paused);ocean?.SetWeather(s.time,s.wind,s.kind==IslandWeatherKind.TidalWave?Mathf.Lerp(island.min.x-nav.Scale*40,island.max.x+nav.Scale*40,Mathf.Clamp01(s.phaseTime/24)):0,s.kind==IslandWeatherKind.TidalWave?nav.Scale*8:0,WaveWidth);
  if(rainFocus!=null)rain.transform.position=rainFocus.position+Vector3.up*nav.Scale*35;
  var emission=rain.emission;emission.rateOverTime=s.rain*90;var velocity=rain.velocityOverLifetime;velocity.x=Wind.x*nav.Scale*12;velocity.z=Wind.z*nav.Scale*12;Play(rain,!paused&&s.rain>.01f,paused);
  int i=0;foreach(var m in s.materials){if(!m.burning||!byId.TryGetValue(m.id,out var target)||i>=flames.Count)continue;var fire=flames[i++];var bounds=target.Bounds(nav.Scale);fire.transform.position=bounds.center+Vector3.up*bounds.extents.y;var main=fire.main;main.startSize=Mathf.Clamp(bounds.size.y*.4f,nav.Scale*3,nav.Scale*6);var shape=fire.shape;shape.radius=Mathf.Max(nav.Scale,bounds.extents.x*.6f);Play(fire,!paused,paused);}for(;i<flames.Count;i++)Play(flames[i],false,false);
 }
 static void Play(ParticleSystem ps,bool active,bool paused){if(paused){if(ps.isPlaying)ps.Pause();return;}if(active){if(!ps.isPlaying)ps.Play();}else if(!ps.isStopped)ps.Stop(true,ParticleSystemStopBehavior.StopEmitting);}
 public void ResetWorld(){boltRemaining=0;if(bolt!=null)bolt.enabled=false;foreach(var ps in flames)ps.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);if(rain!=null)rain.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);Visuals(season().paused);}
 void OnDestroy(){if(rainMaterial!=null)Destroy(rainMaterial);if(fireMaterial!=null)Destroy(fireMaterial);if(boltMaterial!=null)Destroy(boltMaterial);}
}
}
