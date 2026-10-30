using UnityEngine;
namespace BWVR {
public sealed partial class VillagerExploration {
 StormChapter fireChapter;WaterVessel fireBucket;float fireElapsed,fireClock,firePulse;int fireStage;
 public bool FireResponding=>Current==Phase.FireResponse;
 public bool TryFireResponse(StormChapter chapter){
  if(!CanStartStructure||chapter==null||!chapter.FirePending||social.Memory.injury>.05f||social.RecentFright||Vector3.Distance(transform.position,chapter.FireWater)>nav.world.Scale*120||!world.Reachable(transform.position,chapter.FireWater,nav.profile)||!world.Reachable(chapter.FireWater,chapter.FireWork,nav.profile)||!chapter.ClaimFireResponse(this))return false;
  fireChapter=chapter;fireElapsed=fireClock=firePulse=0;fireStage=0;routine.BeginHandsFreeActivity();nav.ResetRoute();
  if(fireBucket==null)fireBucket=WaterVessel.Create(transform,"Fire response bucket",transform.lossyScale.y*.24f,transform.lossyScale.y*.10f);fireBucket.SetFill(0);fireBucket.gameObject.SetActive(true);motion?.Carry(fireBucket.transform);Current=Phase.FireResponse;return true;
 }
 bool TickFireResponse(float dt){
  fireElapsed+=dt;fireClock+=dt;var chapter=fireChapter;if(chapter==null||!chapter.FirePending||social.Memory.injury>.05f||social.RecentFright||fireElapsed>100){ResetTask();return false;}
  float speed=transform.lossyScale.y*1.6f;
  if(fireStage==0){if(nav.MoveTo(chapter.FireWater,speed,dt)){fireStage=1;fireClock=0;motion?.Activity(1);}}
  else if(fireStage==1){fireBucket.SetFill(Mathf.Clamp01(fireClock/1.5f));if(fireClock>=1.5f){if(!chapter.DrawFireWater(this)){ResetTask();return false;}fireBucket.SetFill(1);motion?.Activity(0);fireStage=2;fireClock=0;nav.ResetRoute();}}
  else if(fireStage==2){motion?.LookAt(chapter.Creature);if(nav.MoveTo(chapter.FireWork,speed,dt)){fireStage=3;fireClock=firePulse=0;motion?.Activity(5);motion?.LookAt(null);}}
  else {if(Vector3.Distance(transform.position,chapter.FireWork)>nav.world.Scale*4){ResetTask();return false;}motion?.Signal(false,true,chapter.FirePoint);firePulse+=dt;if(firePulse>=.5f){firePulse-=.5f;if(chapter.PourFireWater(this,.5f)){fireBucket.SetFill(fireBucket.Fill-.125f);IslandAudio.Cue(IslandSound.Pour,transform.position,GetEntityId().GetHashCode(),.45f);}if(!chapter.FirePending){motion?.React();ResetTask();Current=Phase.Celebrate;return true;}if(fireBucket.Fill<=.01f){fireStage=0;fireClock=0;motion?.Signal(false);motion?.Activity(0);nav.ResetRoute();}}}
  if(nav.Blocked&&fireElapsed>5){ResetTask();return false;}return Active;
 }
 void ReleaseFireResponse(){if(fireChapter!=null){fireChapter.ReleaseFireResponse(this);motion?.Signal(false);motion?.Carry(null);}fireChapter=null;if(fireBucket!=null){fireBucket.SetFill(0);fireBucket.gameObject.SetActive(false);}fireElapsed=fireClock=firePulse=0;fireStage=0;}
}
}
