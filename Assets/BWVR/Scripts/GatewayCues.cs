using System;
using System.Linq;
using UnityEngine;
namespace BWVR {
// Read-only light geometry. It owns no collider, item, task, receipt or resource.
public sealed class GatewayCues:IDisposable {
 readonly GameObject root;readonly Material material;readonly LineRenderer[] milestones=new LineRenderer[4];readonly LineRenderer outline,piece;
 public ConstructionBlock SuggestedPiece{get;private set;}
 public bool ShowingPlacement=>outline!=null&&outline.enabled;public bool ShowingEntrance{get;private set;}
 public int VisibleMilestones=>milestones.Count(l=>l!=null&&l.enabled);
 public static int Progress(GatewayProject p)=>p==null?0:(p.supplied>=3?1:0)|(p.playerPieces!=0?2:0)|(p.creaturePieces!=0?4:0)|(p.phase==GatewayPhase.Complete?8:0);
 public GatewayCues(Transform parent){root=new GameObject("Private gateway project light cues");root.transform.SetParent(parent,false);material=new Material(Resources.Load<Material>("ConstructionPreview"));for(int i=0;i<4;i++)milestones[i]=Line("Project progress "+i,24,true);outline=Line("Next arch placement light",16,false);piece=Line("Available piece / entrance light",32,true);Hide();}
 LineRenderer Line(string name,int count,bool loop){var go=new GameObject(name);go.transform.SetParent(root.transform,false);var l=go.AddComponent<LineRenderer>();l.useWorldSpace=true;l.loop=loop;l.positionCount=count;l.sharedMaterial=material;l.shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off;l.receiveShadows=false;l.numCornerVertices=2;l.numCapVertices=2;return l;}
 static void Tint(LineRenderer l,Color c,float width){l.startColor=l.endColor=c;l.widthMultiplier=width;}
 static void Ring(LineRenderer l,Vector3 centre,float radius){for(int i=0;i<l.positionCount;i++){float a=i*Mathf.PI*2/l.positionCount;l.SetPosition(i,centre+new Vector3(Mathf.Cos(a),0,Mathf.Sin(a))*radius);}l.enabled=true;}
 public void Hide(){foreach(var l in milestones)if(l!=null)l.enabled=false;if(outline!=null)outline.enabled=false;if(piece!=null)piece.enabled=false;SuggestedPiece=null;ShowingEntrance=false;}
 public void Refresh(SliceState state,ConstructionSite site,bool busy){
  Hide();var p=state.gatewayProject;if(state.paused||site==null||site.Paused||site.Locked||site.Recipe!=BuildRecipe.Arch||p==null||p.phase!=GatewayPhase.Active&&(p.phase!=GatewayPhase.Complete||(state.hybrid?.time??0)>p.finished+8))return;
  int progress=Progress(p);float u=site.unit;for(int i=0;i<4;i++){Tint(milestones[i],(progress&(1<<i))!=0?new Color(.35f,1,.75f,.85f):new Color(.68f,.57f,.32f,.35f),u*.14f);Ring(milestones[i],site.World(new Vector3(-6+i*4,.22f,-site.HalfSize.y+3)),u*.85f);}
  if(site.RecipeComplete){if(p.supplied>=3&&p.Shared){Tint(piece,new Color(.35f,1,.75f,.8f),u*.18f);Ring(piece,site.World(new Vector3(0,.2f,-5)),u*2);ShowingEntrance=true;}return;}
  if(p.phase!=GatewayPhase.Active||busy||site.Repairer!=null||site.blocks.Any(b=>b.Item.held||b.VillagerOwner!=null))return;
  foreach(var b in site.blocks){if(b.Attached||!b.RecipePiece||!b.CanBuild||!site.TryRecipePlan(b,out var at,out var facing))continue;SuggestedPiece=b;Tint(piece,new Color(.35f,1,.75f,.75f),u*.14f);Ring(piece,new Vector3(b.transform.position.x,site.transform.position.y+u*.2f,b.transform.position.z),u*2);var half=b.size*.5f*u;var corners=new[]{new Vector3(-1,-1,-1),new Vector3(1,-1,-1),new Vector3(1,-1,1),new Vector3(-1,-1,1),new Vector3(-1,1,-1),new Vector3(1,1,-1),new Vector3(1,1,1),new Vector3(-1,1,1)};var order=new[]{0,1,2,3,0,4,5,1,5,6,2,6,7,3,7,4};for(int i=0;i<order.Length;i++)outline.SetPosition(i,at+facing*Vector3.Scale(corners[order[i]],half));Tint(outline,new Color(1,.8f,.35f,.5f),u*.1f);outline.enabled=true;break;}
 }
 public void Dispose(){Hide();UnityEngine.Object.Destroy(root);UnityEngine.Object.Destroy(material);}
}
public sealed partial class QuestSlice {GatewayCues gatewayCues;void DisposeGatewayCues(){gatewayCues?.Dispose();gatewayCues=null;}}
public sealed partial class CreatureHybrid {
 bool GatewayNeedsPartner(ConstructionSite site)=>Season.gatewayProject?.phase==GatewayPhase.Active&&Season.gatewayProject.playerPieces!=0&&Season.gatewayProject.creaturePieces==0&&site==GetComponent<CreatureConstructionLearning>()?.sites[0]&&site.Recipe==BuildRecipe.Arch;
}
public sealed partial class VillagerExploration {
 bool ChooseGatewayVisit(){if(construction==null||!CanStartStructure)return false;var site=construction.sites[0];if(!GatewayVisitWanted(site)||!site.RecipeComplete)return false;if(ChooseShelter())return true;var resources=nav.world.GetComponentInChildren<VillageResources>();if(resources==null||!resources.ProvisionVisible(social,site.transform.position+Vector3.up*site.unit*3,site.transform))return false;return TryStructure(site);}
}
}
