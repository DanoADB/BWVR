using System;
using UnityEngine;
namespace BWVR {
public enum BowlHandoffPhase {None,YourShot,Settling,Returning,Rebuild,CreatureInvited,CreatureActing}
// Private light only. No colliders, target registration, timers, ownership or simulation events.
public sealed class BowlHandoffCues:IDisposable {
 readonly GameObject root;readonly Material material;readonly LineRenderer halo,stance,target,arrow;
 public int VisibleLines=>(halo.enabled?1:0)+(stance.enabled?1:0)+(target.enabled?1:0)+(arrow.enabled?1:0);
 public BowlHandoffCues(Transform parent){root=new GameObject("Private course turn guides");root.transform.SetParent(parent,false);material=new Material(Resources.Load<Material>("ConstructionPreview"));halo=Line("Ball handoff ring",32,true);stance=Line("Creature approach / waiting ring",24,true);target=Line("Rebuilt target ring",24,true);arrow=Line("Roll direction at approach",3,false);Hide();}
 LineRenderer Line(string name,int count,bool loop){var go=new GameObject(name);go.transform.SetParent(root.transform,false);var line=go.AddComponent<LineRenderer>();line.useWorldSpace=true;line.loop=loop;line.positionCount=count;line.sharedMaterial=material;line.shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off;line.receiveShadows=false;line.numCornerVertices=2;return line;}
 public void Hide(){halo.enabled=stance.enabled=target.enabled=arrow.enabled=false;}
 void Ring(LineRenderer line,Vector3 centre,float radius,Color color,float width){line.startColor=line.endColor=color;line.widthMultiplier=width;for(int i=0;i<line.positionCount;i++){float angle=i*Mathf.PI*2/line.positionCount;line.SetPosition(i,centre+new Vector3(Mathf.Cos(angle),0,Mathf.Sin(angle))*radius);}line.enabled=true;}
 public void Show(BowlHandoffPhase phase,BeachBall ball,IslandNavigation nav,ConstructionSite site,ConstructionBlock piece,Vector3 approach){
  Hide();if(phase==BowlHandoffPhase.None)return;float u=nav.Scale;var gold=new Color(1,.78f,.25f,.8f);var blue=new Color(.25f,.72f,1,.8f);var amber=new Color(1,.48f,.12f,.75f);bool own=phase==BowlHandoffPhase.YourShot||phase==BowlHandoffPhase.Rebuild;var color=phase==BowlHandoffPhase.Settling?amber:own?gold:blue;
  Vector3 Floor(Vector3 point){var foot=nav.ProjectFoot(point,nav.creature);if(site.Contains(site.Local(foot)))foot.y=Mathf.Max(foot.y,site.transform.position.y);return foot+Vector3.up*u*.25f;}
  if(!ball.GetComponent<CarryItem>().held)Ring(halo,Floor(ball.transform.position),u*(ball.Radius+1),color,u*.17f);
  if(phase==BowlHandoffPhase.Settling){Ring(stance,Floor(ball.transform.position),u*(ball.Radius+2),amber,u*.1f);return;}
  if((phase!=BowlHandoffPhase.CreatureInvited&&phase!=BowlHandoffPhase.CreatureActing)||piece==null)return;
  var from=Floor(approach);var to=Floor(piece.transform.position);Ring(stance,from,u*3,blue,u*.15f);Ring(target,to,u*(Mathf.Max(piece.size.x,piece.size.z)*.5f+1),blue,u*.15f);var forward=to-from;forward.y=0;if(forward.sqrMagnitude<1e-10f)return;forward.Normalize();var right=Vector3.Cross(Vector3.up,forward);arrow.startColor=arrow.endColor=blue;arrow.widthMultiplier=u*.17f;arrow.SetPosition(0,from-forward*u+right*u*1.5f);arrow.SetPosition(1,from+forward*u*2);arrow.SetPosition(2,from-forward*u-right*u*1.5f);arrow.enabled=true;
 }
 public void Dispose(){Hide();UnityEngine.Object.Destroy(root);UnityEngine.Object.Destroy(material);}
}
public sealed partial class BeachBall {
 BowlHandoffCues handoffCues;public BowlHandoffPhase HandoffPhase{get;private set;}
 BowlHandoffPhase ReadBowlHandoff(bool paused){
  if(paused||creature==null||creature.Season.paused||!isActiveAndEnabled||creature.GetComponent<CarryItem>().held)return BowlHandoffPhase.None;var p=creature.Season.buildBowl;if(p==null||p.phase==GatewayPhase.NotStarted||p.phase==GatewayPhase.Ended)return BowlHandoffPhase.None;var mind=creature.GetComponent<CreatureCuriosity>();if(item.held&&mind.Holding!=item)return BowlHandoffPhase.None;if(PeerGame||Player!=null||Goals?.Active==true||Goals?.Turns.Active==true||Puzzle?.Active==true||Bowling?.Active==true)return BowlHandoffPhase.None;
  if(BuildBowlPending&&bowlBound==p&&p.phase==GatewayPhase.Active||BowlCreaturePending&&turnFlightProject==p&&p.attempts==turnFlightAttempt&&p.shots==turnFlightShot)return BowlHandoffPhase.Settling;
  if(BuildBowlReturnRequested)return BowlHandoffPhase.Returning;if(p.phase!=GatewayPhase.Active)return BowlHandoffPhase.None;
  if(BowlTurnOffered)return mind.TakingBowlTurn?BowlHandoffPhase.CreatureActing:BowlHandoffPhase.CreatureInvited;
  if(item.held||InFlight||mind.HandlingBeachBall)return BowlHandoffPhase.None;
  return creature.BowlRebuildPending?BowlHandoffPhase.Rebuild:BowlHandoffPhase.YourShot;
 }
 public void RefreshBowlHandoff(bool paused=false){HandoffPhase=ReadBowlHandoff(paused);if(HandoffPhase==BowlHandoffPhase.None){handoffCues?.Hide();return;}if(handoffCues==null)handoffCues=new BowlHandoffCues(transform.parent);handoffCues.Show(HandoffPhase,this,nav,creature.GetComponent<CreatureConstructionLearning>().sites[0],turnTarget,turnStance);}
}
}
