using System;
using UnityEngine;
namespace BWVR {
// Bounded needs create variety without overriding feasibility, teaching or action ownership.
[Serializable] public sealed class CreatureMotivation {
 public float boredom=.2f,curiosity=.35f,companionship=.25f;
 public float[] satiation=new float[8];
 public void Advance(float dt){if(dt<=0||float.IsNaN(dt)||float.IsInfinity(dt))return;
  boredom=Mathf.Clamp01(boredom+dt*.004f);curiosity=Mathf.Clamp01(curiosity+dt*.0025f);companionship=Mathf.Clamp01(companionship+dt*.002f);
  for(int i=0;i<satiation.Length;i++)satiation[i]=Mathf.Max(0,satiation[i]-dt*.0025f);
 }
 public float Bias(CreatureAction action,float novelty,float hunger,bool shared){
  float value=action==CreatureAction.EatFood?Mathf.Lerp(-.85f,1.1f,hunger):0;
  // A small baseline preference for building; satiation and learned interests still create variety.
  if(action==CreatureAction.ConstructionPlay)value+=.08f;
  if(action==CreatureAction.PlayWithToy||action==CreatureAction.ConstructionPlay)value+=boredom*.55f;
  if(action==CreatureAction.InvestigateObject||action==CreatureAction.PlayWithToy||action==CreatureAction.ConstructionPlay)value+=curiosity*Mathf.Clamp01(novelty)*.8f;
  if(action==CreatureAction.SocialInteraction||shared)value+=companionship*.95f;
  // An urgent need should not be suppressed by having addressed it recently.
  if(action!=CreatureAction.WaterDelivery&&!(action==CreatureAction.EatFood&&hunger>.8f))value-=satiation[(int)action]*.95f;
  return Mathf.Clamp(value,-1,1.5f);
 }
 public void Complete(CreatureAction action,bool shared){
  satiation[(int)action]=Mathf.Clamp01(satiation[(int)action]+.38f);
  if(action==CreatureAction.PlayWithToy||action==CreatureAction.ConstructionPlay)boredom=Mathf.Max(0,boredom-.4f);
  if(action==CreatureAction.InvestigateObject||action==CreatureAction.PlayWithToy||action==CreatureAction.ConstructionPlay)curiosity=Mathf.Max(0,curiosity-.25f);
  if(action==CreatureAction.SocialInteraction||shared)companionship=Mathf.Max(0,companionship-.6f);
 }
 public string Reason(CreatureAction action,bool shared,float hunger)=>action==CreatureAction.EatFood&&hunger>.65f?"hunger":(action==CreatureAction.SocialInteraction||shared)&&companionship>.45f?"seeking company":boredom>.55f?"wants a change":curiosity>.5f?"curiosity":"learned preference";
 public bool Valid(){if(!HybridState.Number(boredom,0,1)||!HybridState.Number(curiosity,0,1)||!HybridState.Number(companionship,0,1)||satiation==null||satiation.Length!=8)return false;foreach(float value in satiation)if(!HybridState.Number(value,0,1))return false;return true;}
}
}
