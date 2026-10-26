using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
namespace BWVR {
public sealed partial class GodToolPalette {
 VillageResources planting;GameObject seedTargets;float seedPreviewClock;
 readonly List<GatheringSource> seedPlots=new List<GatheringSource>();readonly List<LineRenderer> seedOutlines=new List<LineRenderer>();MaterialPropertyBlock seedTint;
 void BuildSeedTargets(){planting=nav.GetComponentInChildren<VillageResources>();seedTargets=new GameObject("Private planting plot previews");seedTargets.transform.SetParent(transform,false);seedTint=new MaterialPropertyBlock();
  foreach(var node in planting.sources.Where(s=>s.kind==VillageResource.Wheat).Take(12)){seedPlots.Add(node);var go=new GameObject("Seed plot preview "+node.id);go.transform.SetParent(seedTargets.transform,false);var line=go.AddComponent<LineRenderer>();line.sharedMaterial=previewMaterial;line.useWorldSpace=true;line.widthMultiplier=nav.Scale*.12f;line.shadowCastingMode=ShadowCastingMode.Off;line.receiveShadows=false;seedOutlines.Add(line);}seedTargets.SetActive(false);
 }
 void SeedVisibility(bool visible){if(seedTargets==null)return;seedTargets.SetActive(visible);seedPreviewClock=0;}
 void RefreshSeedTargets(float dt){seedPreviewClock-=dt;if(seedPreviewClock>0)return;seedPreviewClock=.25f;
  for(int i=0;i<seedPlots.Count;i++){var node=seedPlots[i];var record=planting.State.Find(node.id);var line=seedOutlines[i];bool empty=record!=null&&!record.growing&&record.remaining<=.001f;line.gameObject.SetActive(empty);if(!empty)continue;
   bool available=!miracles.Suspended&&planting.CanPlayerPlant(node.at,out _);line.loop=available;line.positionCount=4;var corners=available?new[]{new Vector3(-2,0,-2),new Vector3(-2,0,2),new Vector3(2,0,2),new Vector3(2,0,-2)}:new[]{new Vector3(-2,0,-2),new Vector3(2,0,2),new Vector3(-2,0,2),new Vector3(2,0,-2)};
   for(int n=0;n<4;n++){var p=node.at+corners[n]*nav.Scale;line.SetPosition(n,(nav.Terrain!=null?nav.Terrain.Project(p):p)+Vector3.up*nav.Scale*.16f);}seedTint.SetColor("_Color",available?new Color(.55f,1,.55f):new Color(.85f,.58f,.28f));line.SetPropertyBlock(seedTint);
  }
 }
 void AimPlanting(Ray ray){bool hit=ground.Raycast(ray,out var contact,8*rig.localScale.x);PreviewPoint=hit?contact.point:ray.GetPoint(2*rig.localScale.x);string reason="Aim at an empty field outline";PreviewValid=hit&&!miracles.Suspended&&planting.CanPlayerPlant(PreviewPoint,out reason);Reason=reason;
  var node=hit?planting.PlayerPlotAt(PreviewPoint):null;preview.SetActive(hit);preview.transform.position=node!=null?node.at:PreviewPoint;PreviewShape(PreviewValid,PreviewValid?3:1);previewMaterial.color=PreviewValid?new Color(.55f,1,.55f):new Color(1,.25f,.18f);
 }
 string PlantCostHint(){var node=planting.PlayerPlotAt(PreviewPoint);var record=node==null?null:planting.State.Find(node.id);return record?.expansionPlot==true&&!record.farmPrepared?"Hold · 1 seed + 2 timber":"Hold · 1 seed";}
 bool PlantAt(Vector3 point){if(Season.paused||miracles.Suspended){Reason="Paused";return false;}if(!planting.TryPlayerPlant(point,out var reason)){Reason=reason;return false;}Uses++;Reason="";creature.InvalidateLesson("Player planting is not creature work");creature.Journal.Add(Season.Hybrid.time,"player planting",CreatureAction.IdleOrObserve,planting.PlayerPlotAt(point).id,"One paid seed; saved crop growth");feedback?.Invoke(true,"Seeds planted");seedPreviewClock=0;return true;}
}
}
