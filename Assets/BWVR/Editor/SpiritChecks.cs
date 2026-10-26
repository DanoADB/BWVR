using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace BWVR.Editor {
public static class SpiritChecks {
 static void Require(bool ok,string message){if(!ok)throw new Exception(message);}
 [MenuItem("BWVR/Verify spirit read-only observations")]
 public static void VerifyModel(){
  var legacy=new SliceState();string before=JsonUtility.ToJson(legacy);var empty=SpiritObservation.Capture(legacy,CreatureAffect.Mood.Curious);
  Require(JsonUtility.ToJson(legacy)==before&&legacy.resources==null&&legacy.hybrid==null&&legacy.families==null,"Inspection initialized or changed legacy state");
  Require(!empty.resourcesKnown&&empty.adults==VillagePopulation.Adults&&empty.children==0&&empty.learnedAssociations==0&&empty.weather=="Unknown","Unknown observations invented data");
  Require(empty.Summary().Contains("AI DISCONNECTED")&&SpiritObservation.Capture(null,CreatureAffect.Mood.Calm)==null,"Disconnected/missing-state status wrong");
  var state=new SliceState{resources=new ResourceState{wood=9,wheat=11,resin=2,exposedWheat=3},hybrid=new HybridState(),families=new FamilyState(),weather=new WeatherState()};
  state.hybrid.associations=new[]{new LearnedAssociation{samples=0},new LearnedAssociation{samples=2}};before=JsonUtility.ToJson(state);
  var observed=SpiritObservation.Capture(state,CreatureAffect.Mood.Happy);Require(JsonUtility.ToJson(state)==before,"Observation mutated live state");
  Require(observed.wood==9&&observed.wheat==11&&observed.resin==2&&observed.exposedFood==3&&observed.learnedAssociations==1&&observed.mood==CreatureAffect.Mood.Happy,"Wrong inventory or learned-context observations");
  state.resources.wood=20;Require(observed.wood==9&&SpiritObservation.Capture(state,CreatureAffect.Mood.Sad).wood==20,"Snapshot aliases mutable live state");
  Debug.Log("BWVR_SPIRIT_MODEL_PASS: detached observations, legacy/null state, real inventory, sampled learning and honest disconnected status");
 }
 [MenuItem("BWVR/Verify private spirit (Play mode)")]
 public static void VerifyRuntime(){
  Require(EditorApplication.isPlaying,"Enter Play mode");var slice=UnityEngine.Object.FindAnyObjectByType<QuestSlice>();var spirit=slice.GetComponentInChildren<SpiritCompanion>(true);Require(spirit!=null,"Spirit missing from slice");
  var field=typeof(QuestSlice).GetField("state",BindingFlags.Instance|BindingFlags.NonPublic);var state=(SliceState)field.GetValue(slice);var before=JsonUtility.ToJson(state);
  bool visible=spirit.Visible,help=slice.HelpVisible,tracking=spirit.HandTracked,suspended=spirit.Suspended,originalPause=state.paused;
  try{
   Require(!spirit.Connected,"Local presentation falsely reports AI connectivity");
   Require(spirit.GetComponentsInChildren<Collider>(true).Length==0&&spirit.GetComponentsInChildren<Rigidbody>(true).Length==0&&spirit.GetComponentsInChildren<AudioSource>(true).Length==0&&spirit.GetComponentsInChildren<Light>(true).Length==0,"Spirit enters physical/audio/light simulation");
   Require(spirit.GetComponentsInChildren<CarryItem>(true).Length==0&&spirit.GetComponentsInChildren<CreatureAffordance>(true).Length==0&&spirit.GetComponentsInChildren<HybridTarget>(true).Length==0&&spirit.GetComponentsInChildren<PlayImpactTarget>(true).Length==0&&spirit.GetComponentsInChildren<IslandNavigator>(true).Length==0,"Spirit is visible to actor or hazard registries");
   var hybrid=slice.GetComponentInChildren<CreatureHybrid>();Require(!hybrid.Targets.Any(t=>t!=null&&t.transform.IsChildOf(spirit.transform)),"Spirit registered in creature perception");
   foreach(var renderer in spirit.GetComponentsInChildren<MeshRenderer>(true))Require(renderer.sharedMaterial!=null&&!renderer.sharedMaterial.shader.name.Contains("Error")&&renderer.shadowCastingMode==UnityEngine.Rendering.ShadowCastingMode.Off,"Spirit shader/shadow setup invalid");
   var particles=spirit.GetComponentsInChildren<ParticleSystem>(true);Require(particles.Length==3&&particles.Sum(p=>p.main.maxParticles)==SpiritEffects.ParticleLimit,"Spirit particle budget changed");
   foreach(var ps in particles)Require(!ps.collision.enabled&&!ps.trigger.enabled&&!ps.lights.enabled&&!ps.main.playOnAwake&&ps.main.simulationSpace==ParticleSystemSimulationSpace.Local,"Particles participate in the world or start independently");
   spirit.SetVisible(true);var initialColor=spirit.OrbColor;var initialScale=spirit.OrbScale;for(int n=0;n<120;n++)spirit.Animate(.05f,false);
   Require(spirit.ParticleCount>0&&spirit.ParticleCount<=SpiritEffects.ParticleLimit,"Particles absent or unbounded");Require(spirit.OrbColor!=initialColor&&spirit.OrbScale!=initialScale,"Orb does not change color and shape");
   float effectTime=spirit.EffectTime;var frozenColor=spirit.OrbColor;var frozenScale=spirit.OrbScale;spirit.Animate(1,true);spirit.Animate(float.NaN,false);Require(spirit.EffectTime==effectTime&&spirit.OrbColor==frozenColor&&spirit.OrbScale==frozenScale,"Paused or invalid time advanced orb effects");
   spirit.SetVisible(true);slice.SetHelp(true);spirit.SendMessage("LateUpdate");Require(spirit.Observation!=null&&spirit.Readout==spirit.Observation.Summary(),"Help did not expose measured observations");
   Require(spirit.GetComponentInChildren<Canvas>().gameObject.activeInHierarchy,"Readout not visible");
   slice.SetHelp(false);Require(!spirit.GetComponentInChildren<Canvas>(true).gameObject.activeInHierarchy,"Details remained visible after closing help");
   var figure=spirit.transform.Find("Player-only spirit presentation/Floating seed");var position=figure.localPosition;spirit.Suspended=true;spirit.SendMessage("LateUpdate");Require(figure.localPosition==position,"Suspended spirit animation advanced");
   spirit.Suspended=false;bool paused=state.paused;state.paused=true;position=figure.localPosition;spirit.SendMessage("LateUpdate");Require(figure.localPosition==position,"Paused spirit animation advanced");state.paused=paused;
   spirit.HandTracked=false;spirit.SendMessage("LateUpdate");Require(Vector3.Distance(spirit.transform.position,Camera.main.transform.position)<Camera.main.transform.parent.lossyScale.x,"Tracking-loss fallback is out of reach");
   spirit.SetVisible(false);Require(spirit.ParticleCount==0&&!spirit.GetComponentsInChildren<Renderer>(true).Any(r=>r.gameObject.activeInHierarchy),"Dismissed spirit still renders/leaves particles");spirit.SetVisible(true);spirit.Animate(.05f,false);Require(spirit.ParticleCount>0,"Summoning did not restart shimmer");
   for(int n=0;n<800;n++)spirit.Animate(.05f,false);Require(spirit.ParticleCount>0&&spirit.ParticleCount<=SpiritEffects.ParticleLimit,"Emission died after hide/show or multiple particle cycles");
   // The sanctuary belongs to the XR rig, a separate root from the island slice.
   var toggle=UnityEngine.Object.FindObjectsByType<SpatialTarget>(FindObjectsInactive.Include,FindObjectsSortMode.None).FirstOrDefault(t=>t.name=="Spirit: show / hide");Require(toggle!=null,"No sanctuary toggle");toggle.Activate();Require(!spirit.Visible,"Sanctuary toggle did not dismiss");toggle.Activate();Require(spirit.Visible,"Sanctuary toggle did not summon");
   Require(JsonUtility.ToJson(state)==before,"Spirit controls/inspection changed simulation or saved learning");
   Debug.Log("BWVR_SPIRIT_RUNTIME_PASS: private orb, color/shape/pulse, bounded motes/aura/shimmer, pause/suspend/hide/restart, no perception/physics/hazards, measured readout, help/toggle, tracking-loss fallback and unchanged season");
  }finally{state.paused=originalPause;spirit.HandTracked=tracking;spirit.Suspended=suspended;slice.SetHelp(help);spirit.SetVisible(visible);}
 }
}
}
