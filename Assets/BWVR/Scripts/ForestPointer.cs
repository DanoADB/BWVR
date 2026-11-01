using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
namespace BWVR {
public sealed partial class GodToolPalette {
 GameObject forestTargets;float forestPreviewClock;
 readonly List<GatheringSource> forestSites=new List<GatheringSource>();readonly List<LineRenderer> forestRings=new List<LineRenderer>();
 void BuildForestTargets(){
  forestTargets=new GameObject("Private harvested forest previews");forestTargets.transform.SetParent(transform,false);
  foreach(var node in planting.sources){if(node.kind!=VillageResource.Wood||node.tree==null||forestSites.Count>=16)continue;forestSites.Add(node);var go=new GameObject("Regrowth site preview "+node.id);go.transform.SetParent(forestTargets.transform,false);var line=go.AddComponent<LineRenderer>();line.sharedMaterial=previewMaterial;line.useWorldSpace=true;line.loop=true;line.positionCount=16;line.widthMultiplier=nav.Scale*.12f;line.shadowCastingMode=ShadowCastingMode.Off;line.receiveShadows=false;
   for(int i=0;i<16;i++){float a=i*Mathf.PI/8;var p=node.at+new Vector3(Mathf.Cos(a),0,Mathf.Sin(a))*nav.Scale*3;line.SetPosition(i,(nav.Terrain!=null?nav.Terrain.Project(p):p)+Vector3.up*nav.Scale*.18f);}forestRings.Add(line);
  }forestTargets.SetActive(false);
 }
 void ForestVisibility(bool visible){if(forestTargets==null||forestTargets.activeSelf==visible)return;forestTargets.SetActive(visible);forestPreviewClock=0;}
 void RefreshForestTargets(float dt){forestPreviewClock-=dt;if(forestPreviewClock>0)return;forestPreviewClock=.25f;
  for(int i=0;i<forestSites.Count;i++){var node=forestSites[i];var record=planting.State.Find(node.id);bool candidate=record!=null&&(record.growing||planting.State.DepletedTree(node.id));var line=forestRings[i];line.gameObject.SetActive(candidate);if(candidate){bool free=node.owner==null&&!node.tree.ResourceReserved&&planting.Safe(node);seedTint.SetColor("_Color",free?new Color(.55f,1,.55f):new Color(.85f,.58f,.28f));line.SetPropertyBlock(seedTint);}}
 }
 void AimRegrowth(Ray ray){bool hit=ground.Raycast(ray,out var contact,8*rig.localScale.x);PreviewPoint=hit?contact.point:ray.GetPoint(2*rig.localScale.x);string why="Aim at a harvested tree stump or sapling";PreviewValid=hit&&Brush.CanRegrow(PreviewPoint,out why);Reason=why;var node=hit?planting.PlayerTreeAt(PreviewPoint):null;preview.SetActive(hit);preview.transform.position=node!=null?node.at:PreviewPoint;PreviewShape(PreviewValid,3);previewMaterial.color=PreviewValid?new Color(.55f,1,.55f):new Color(1,.25f,.18f);}
 bool RegrowPointer(Ray ray,bool pressed,float dt){RefreshForestTargets(dt);if(pressed&&!releaseGate&&Brush.PulseReady)AimRegrowth(ray);bool applied=Brush.ProcessRegrow(PreviewPoint,pressed&&!releaseGate,dt,PreviewValid);if(applied){Uses++;forestPreviewClock=0;}if(pressed&&!releaseGate)Reason=Brush.Reason;hint.text="Regrow trees · "+(Brush.Active?"Nurturing woodland":PreviewValid?"Hold over a stump or sapling":Reason);return true;}
}
}
