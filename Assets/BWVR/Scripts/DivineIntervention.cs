using System;
using System.Collections.Generic;
using UnityEngine;
namespace BWVR {
[Serializable] public sealed class DivineTuning {
 [Range(1,100)]public float maximum=100;[Range(0,100)]public float initial=60;public float rainCost=35,healingCost=25,recovery=.1f;
 [Tooltip("Testing: tools consume no spiritual energy. Disable when tuning the economy.")] public bool unlimitedEnergy=true;
 public float rainWater=22,rainCrops=6;public bool initialMinorInjury=true;
}
public sealed class DivineIntervention:MonoBehaviour {
 public DivineTuning tuning=new DivineTuning();public bool Suspended;public string LastReason{get;private set;}="";
 public readonly List<DivineTool> tools=new List<DivineTool>();CreatureHybrid creature;IslandNavigation nav;Vector3 cistern;Func<SliceState> season;
 ParticleSystem rain,healing;Material[] materials;Material particleMaterial;readonly List<Renderer> wraps=new List<Renderer>();VillagerRoutine[] villagers;readonly Dictionary<DivineTool,Renderer[]> views=new Dictionary<DivineTool,Renderer[]>();MaterialPropertyBlock tint;
 SliceState Season=>season();HybridState State=>Season.Hybrid;
 public bool UnlimitedEnergy=>tuning.unlimitedEnergy;
 public bool CanAfford(float cost)=>UnlimitedEnergy||State.energy>=cost;
 public float EnergyCost(float cost)=>UnlimitedEnergy?0:cost;
 public void SpendEnergy(float cost){State.energy-=EnergyCost(cost);}
 public static DivineIntervention Build(Transform parent,CreatureHybrid god,IslandNavigation navigation,Vector3 reservoir,VillagerRoutine[] residents,Func<SliceState> current){var root=new GameObject("Quiet intention and miracle tools");root.transform.SetParent(parent,false);var system=root.AddComponent<DivineIntervention>();system.Initialize(god,navigation,reservoir,residents,current);return system;}
 void Initialize(CreatureHybrid god,IslandNavigation navigation,Vector3 reservoir,VillagerRoutine[] residents,Func<SliceState> current){
  creature=god;nav=navigation;cistern=reservoir;villagers=residents;season=current;tint=new MaterialPropertyBlock();InitializeState();
  var colors=new[]{new Color(.25f,.62f,.40f),new Color(.25f,.5f,.85f),new Color(.92f,.75f,.28f),new Color(.82f,.84f,.78f),new Color(.65f,.18f,.19f)};materials=new Material[colors.Length];for(int i=0;i<colors.Length;i++){materials[i]=new Material(Shader.Find("BWVR/GameSurface"));materials[i].color=colors[i];}
  for(int i=0;i<5;i++){
   // Palette glyph templates only. Guidance is introduced by the player, never pre-placed.
   var root=new GameObject(((SpiritualTool)i)+" private tool template");root.SetActive(false);root.transform.SetParent(transform,false);root.transform.localScale=Vector3.one*nav.Scale;
   Part(root.transform,PrimitiveType.Cylinder,new Vector3(0,-.6f,0),new Vector3(.4f,.8f,.4f),2);
   if(i==0){Part(root.transform,PrimitiveType.Sphere,new Vector3(-.6f,.4f,0),new Vector3(.8f,1.1f,.5f),0);Part(root.transform,PrimitiveType.Sphere,new Vector3(.6f,.4f,0),new Vector3(.8f,1.1f,.5f),0);Part(root.transform,PrimitiveType.Sphere,new Vector3(0,.8f,0),Vector3.one*.7f,1);}
   else if(i==1){var shield=Part(root.transform,PrimitiveType.Cube,new Vector3(0,.6f,0),new Vector3(1.5f,1.6f,.35f),1);shield.transform.localRotation=Quaternion.Euler(0,0,20);}
   else if(i==2){Part(root.transform,PrimitiveType.Cylinder,new Vector3(0,.6f,0),new Vector3(1.7f,.15f,1.7f),2).transform.localRotation=Quaternion.Euler(90,0,0);Part(root.transform,PrimitiveType.Sphere,new Vector3(0,.6f,-.2f),new Vector3(1.1f,1.1f,.25f),1);}
   else if(i==3){for(int n=-1;n<=1;n++)Part(root.transform,PrimitiveType.Sphere,new Vector3(n*.7f,.7f,0),new Vector3(1.2f,.8f,.8f),3);for(int n=-1;n<=1;n++)Part(root.transform,PrimitiveType.Sphere,new Vector3(n*.6f,-.1f,0),new Vector3(.2f,.45f,.2f),1);}
   else {Part(root.transform,PrimitiveType.Cube,new Vector3(0,.5f,0),new Vector3(.4f,1.7f,.4f),0);Part(root.transform,PrimitiveType.Cube,new Vector3(0,.5f,0),new Vector3(1.7f,.4f,.4f),0);Part(root.transform,PrimitiveType.Sphere,new Vector3(0,.6f,-.3f),Vector3.one*.65f,3);}
   var box=root.AddComponent<BoxCollider>();box.size=new Vector3(3,3,1.5f);box.contactOffset=nav.Scale*.02f;var item=root.AddComponent<CarryItem>();item.kind=Offering.Toy;item.home=root.transform.position;var tool=root.AddComponent<DivineTool>();tool.kind=(SpiritualTool)i;tool.owner=this;tools.Add(tool);views.Add(tool,root.GetComponentsInChildren<Renderer>());
  }
  foreach(var villager in villagers){var wrap=Part(villager.transform,PrimitiveType.Cylinder,new Vector3(0,.13f,0),new Vector3(.32f,.09f,.32f),4);wrap.name="Minor injury ankle wrap";wraps.Add(wrap.GetComponent<Renderer>());}
  particleMaterial=new Material(Resources.Load<Material>("ConstructionPreview"));rain=Particles("Localized rain",cistern+Vector3.up*nav.Scale*24,new Color(.42f,.7f,1),true);healing=Particles("Healing acknowledgement",cistern,new Color(.48f,1,.65f),false);RefreshConditions();
 }
 GameObject Part(Transform parent,PrimitiveType shape,Vector3 position,Vector3 size,int color){var go=GameObject.CreatePrimitive(shape);go.transform.SetParent(parent,false);go.transform.localPosition=position;go.transform.localScale=size;go.GetComponent<Renderer>().sharedMaterial=materials[color];var collider=go.GetComponent<Collider>();collider.enabled=false;Destroy(collider);return go;}
 ParticleSystem Particles(string label,Vector3 position,Color color,bool isRain){var go=new GameObject(label);go.transform.SetParent(transform,false);go.transform.position=position;var ps=go.AddComponent<ParticleSystem>();ps.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);var main=ps.main;main.playOnAwake=false;main.loop=isRain;main.duration=6;main.simulationSpace=ParticleSystemSimulationSpace.World;main.startLifetime=isRain?1:.7f;main.startSpeed=0;main.startSize=nav.Scale*(isRain?.16f:.4f);main.startColor=color;main.maxParticles=120;var emission=ps.emission;emission.rateOverTime=isRain?60:0;var shape=ps.shape;shape.shapeType=ParticleSystemShapeType.Box;shape.scale=new Vector3(isRain?20:3,1,isRain?20:3)*nav.Scale;var velocity=ps.velocityOverLifetime;velocity.enabled=true;velocity.space=ParticleSystemSimulationSpace.World;velocity.y=new ParticleSystem.MinMaxCurve(nav.Scale*(isRain?-25:3));var renderer=go.GetComponent<ParticleSystemRenderer>();renderer.sharedMaterial=particleMaterial;if(isRain){renderer.renderMode=ParticleSystemRenderMode.Stretch;renderer.lengthScale=4;renderer.velocityScale=0;}return ps;}
 void InitializeState(){if(!State.energyInitialized){if(State.time==0)State.energy=Mathf.Clamp(tuning.initial,0,Mathf.Clamp(tuning.maximum,1,100));State.energyInitialized=true;}if(!State.injuryIntroduced){if(tuning.initialMinorInjury&&villagers.Length>2)villagers[2].GetComponent<VillagerSocial>().Memory.injury=.35f;State.injuryIntroduced=true;}Season.RememberOutcome();}
 public bool CanUse(SpiritualTool kind,HybridTarget target,out string reason){
  reason="";if(Season.paused||Suspended){reason="Paused";return false;}
  if(!Enum.IsDefined(typeof(SpiritualTool),kind)){reason="Unknown tool";return false;}
  if(kind>SpiritualTool.Heal){reason="Use the selected physical tool";return false;}
  if(kind<=SpiritualTool.Investigate)return creature.CanGuide((GuidanceIntent)kind,target,out reason);
  if(State.time<State.nextMiracleAt){reason="Wait for the previous gesture";return false;}
  float cost=Mathf.Clamp(kind==SpiritualTool.Rain?tuning.rainCost:tuning.healingCost,1,100);
  if(float.IsNaN(cost)){reason="Invalid miracle tuning";return false;}if(!CanAfford(cost)){reason="Not enough spiritual energy";return false;}
  if(kind==SpiritualTool.Rain&&(target==null||(target.kind!=HybridTargetKind.Cistern&&target.kind!=HybridTargetKind.Water))){reason="Aim rain at the cistern or water offering";return false;}
  if(kind==SpiritualTool.Heal&&(target?.villager==null||target.villager.Memory.injury<=0||target.GetComponent<CarryItem>()?.held==true)){reason="Aim at an injured villager";return false;}
  return true;
 }
 public bool TryUse(SpiritualTool kind,HybridTarget target){
  if(Season.paused||Suspended){LastReason="Paused";return false;}
  if(!Enum.IsDefined(typeof(SpiritualTool),kind)){LastReason="Unknown tool";return false;}if(kind>SpiritualTool.Heal){LastReason="Use the selected physical tool";return false;}if((int)kind<=(int)SpiritualTool.Investigate){bool accepted=creature.Guide((GuidanceIntent)kind,target);LastReason=accepted?"":State.guidance.reason;return accepted;}
  if(State.time<State.nextMiracleAt){LastReason="Wait for the previous gesture";return false;}
  float cost=Mathf.Clamp(kind==SpiritualTool.Rain?tuning.rainCost:tuning.healingCost,1,100);if(float.IsNaN(cost)){LastReason="Invalid miracle tuning";return false;}
  if(!CanAfford(cost)){LastReason="Not enough spiritual energy";return false;}
  if(kind==SpiritualTool.Rain&&(target==null||(target.kind!=HybridTargetKind.Cistern&&target.kind!=HybridTargetKind.Water))){LastReason="Rain belongs over the village water system";return false;}
  if(kind==SpiritualTool.Heal&&(target?.villager==null||target.villager.Memory.injury<=0||target.GetComponent<CarryItem>()?.held==true)){LastReason="No available injured villager";return false;}
  IslandAudio.Cue(IslandSound.Miracle,target.transform.position,GetEntityId().GetHashCode());SpendEnergy(cost);State.nextMiracleAt=State.time+.5f;creature.InvalidateLesson("Player miracle is not creature work");
  if(kind==SpiritualTool.Rain){Season.RememberOutcome();Season.water=Mathf.Clamp(Season.water+tuning.rainWater,0,100);Season.crops=Mathf.Clamp(Season.crops+tuning.rainCrops,0,100);State.rainRemaining=6;rain.Play();foreach(var v in villagers)v.Celebrate(creature.transform);}
  else {target.villager.Memory.injury=0;target.GetComponent<HumanoidMotion>()?.React();healing.transform.position=target.transform.position+Vector3.up*nav.Scale;healing.Play();healing.Emit(16);}
  if(creature.CanSeeCue(target))creature.GetComponent<CreatureCuriosity>().NoticeGreeting(target.transform);creature.Journal.Add(State.time,"miracle",CreatureAction.IdleOrObserve,target.id,kind+" cost "+EnergyCost(cost));State.Evidence.Add(State.time,EvidenceKind.Miracle,EvidenceSource.PlayerMiracle,CreatureAction.IdleOrObserve,target.id,kind+" cost "+EnergyCost(cost));LastReason="";RefreshConditions();return true;
 }
 public void Tick(float dt,bool paused){
  bool frozen=paused||Suspended;if(frozen){if(rain.isPlaying)rain.Pause();if(healing.isPlaying)healing.Pause();return;}
  State.energy=Mathf.Clamp(State.energy+Mathf.Clamp(tuning.recovery,0,.5f)*dt,0,Mathf.Clamp(tuning.maximum,1,100));State.rainRemaining=Mathf.Max(0,State.rainRemaining-dt);
  if(State.rainRemaining>0){if(!rain.isPlaying)rain.Play();}else if(rain.isPlaying||rain.isPaused)rain.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
  if(healing.isPaused)healing.Play();RefreshConditions();
 }
 void RefreshConditions(){for(int i=0;i<wraps.Count;i++)wraps[i].enabled=villagers[i].GetComponent<VillagerSocial>().Memory.injury>0;foreach(var tool in tools){float cost=tool.kind==SpiritualTool.Rain?tuning.rainCost:tool.kind==SpiritualTool.Heal?tuning.healingCost:0;float light=cost<=0||CanAfford(cost)?1:.28f;foreach(var r in views[tool]){var c=r.sharedMaterial.color;c.r*=light;c.g*=light;c.b*=light;tint.SetColor("_Color",c);r.SetPropertyBlock(tint);}}}
 public void ResetWorld(){InitializeState();foreach(var tool in tools)tool.ReturnHome();rain.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);healing.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);RefreshConditions();}
 void OnDestroy(){if(materials!=null)foreach(var m in materials)Destroy(m);if(particleMaterial!=null)Destroy(particleMaterial);}
}
}
