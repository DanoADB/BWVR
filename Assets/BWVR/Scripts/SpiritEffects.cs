using System;
using UnityEngine;
using UnityEngine.Rendering;
namespace BWVR {
// Presentation-only particles, advanced explicitly so pause and suspend freeze exactly.
public sealed class SpiritEffects:IDisposable {
 public const int ParticleLimit=59;
 readonly ParticleSystem aura,motes,shimmer;readonly Material haloMaterial,sparkMaterial;
 bool summon=true;float auraCredit,moteCredit;
 public int Count=>aura.particleCount+motes.particleCount+shimmer.particleCount;
 public float Time=>motes.time;
 public SpiritEffects(Transform parent){
  var shader=Resources.Load<Shader>("SpiritMotes");if(shader==null)throw new InvalidOperationException("Spirit particle shader missing");
  haloMaterial=new Material(shader){name="Spirit soft aura"};haloMaterial.SetFloat("_Star",0);
  sparkMaterial=new Material(shader){name="Spirit stardust"};sparkMaterial.SetFloat("_Star",1);
  aura=Emitter(parent,"Soft spirit aura",3,haloMaterial,2.2f,3.7f,1);
  motes=Emitter(parent,"Orbiting spirit motes",40,sparkMaterial,2.3f,.16f,2);
  shimmer=Emitter(parent,"Summon shimmer",16,sparkMaterial,.8f,.23f,3);
  var shape=motes.shape;shape.radius=1.4f;shape.radiusThickness=.3f;
  var velocity=motes.velocityOverLifetime;velocity.enabled=true;velocity.space=ParticleSystemSimulationSpace.Local;velocity.orbitalY=.8f;velocity.orbitalZ=.3f;velocity.y=.15f;
  shape=shimmer.shape;shape.radius=.75f;var main=shimmer.main;main.startSpeed=new ParticleSystem.MinMaxCurve(.6f,1.5f);
 }
 static ParticleSystem Emitter(Transform parent,string name,int limit,Material material,float lifetime,float size,uint seed){
  var go=new GameObject(name);go.layer=2;go.transform.SetParent(parent,false);var ps=go.AddComponent<ParticleSystem>();ps.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
  ps.useAutoRandomSeed=false;ps.randomSeed=seed;
  var main=ps.main;main.playOnAwake=false;main.loop=true;main.duration=8;main.maxParticles=limit;main.startLifetime=lifetime;main.startSize=new ParticleSystem.MinMaxCurve(size*.6f,size);main.startSpeed=0;main.simulationSpace=ParticleSystemSimulationSpace.Local;main.scalingMode=ParticleSystemScalingMode.Hierarchy;main.cullingMode=ParticleSystemCullingMode.AlwaysSimulate;
  var emission=ps.emission;emission.enabled=false;var shape=ps.shape;shape.enabled=true;shape.shapeType=ParticleSystemShapeType.Sphere;shape.radius=limit==3?0:1;
  var color=ps.colorOverLifetime;color.enabled=true;var gradient=new Gradient();gradient.SetKeys(new[]{new GradientColorKey(new Color(.3f,1,.85f),0),new GradientColorKey(new Color(1,.8f,.35f),.7f),new GradientColorKey(new Color(.6f,.4f,1),1)},new[]{new GradientAlphaKey(0,0),new GradientAlphaKey(limit==3?.45f:1,.18f),new GradientAlphaKey(0,1)});color.color=gradient;
  var sizeLife=ps.sizeOverLifetime;sizeLife.enabled=true;sizeLife.size=new ParticleSystem.MinMaxCurve(1,AnimationCurve.EaseInOut(0,.4f,1,1));
  var renderer=ps.GetComponent<ParticleSystemRenderer>();renderer.sharedMaterial=material;renderer.renderMode=ParticleSystemRenderMode.Billboard;renderer.shadowCastingMode=ShadowCastingMode.Off;renderer.receiveShadows=false;renderer.lightProbeUsage=LightProbeUsage.Off;renderer.reflectionProbeUsage=ReflectionProbeUsage.Off;renderer.maxParticleSize=.3f;
  ps.Simulate(0,false,true,false);return ps;
 }
 public void Tick(float dt,bool paused,Color tint){
  if(paused||dt<=0||float.IsNaN(dt)||float.IsInfinity(dt))return;dt=Mathf.Min(dt,.1f);
  var main=aura.main;main.startColor=tint;
  if(summon){shimmer.Emit(12);aura.Emit(1);summon=false;}
  // Explicit emission also works after hide/show; Simulate pauses native emitters.
  auraCredit+=dt*.7f;moteCredit+=dt*13;int halos=Mathf.FloorToInt(auraCredit),sparks=Mathf.FloorToInt(moteCredit);
  if(halos>0){aura.Emit(halos);auraCredit-=halos;}if(sparks>0){motes.Emit(sparks);moteCredit-=sparks;}
  aura.Simulate(dt,false,false,false);motes.Simulate(dt,false,false,false);shimmer.Simulate(dt,false,false,false);
 }
 public void Reset(){foreach(var ps in new[]{aura,motes,shimmer}){ps.Stop(false,ParticleSystemStopBehavior.StopEmittingAndClear);ps.Simulate(0,false,true,false);}auraCredit=moteCredit=0;summon=true;}
 public void Dispose(){UnityEngine.Object.Destroy(haloMaterial);UnityEngine.Object.Destroy(sparkMaterial);}
}
}
