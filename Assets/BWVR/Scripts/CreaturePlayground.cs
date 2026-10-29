using System.Linq;
using UnityEngine;
namespace BWVR {
public sealed class CreaturePlayground:MonoBehaviour {
 Material[] materials;AudioClip chime;
 public static CreatureAffordance[] Build(Transform parent,IslandNavigation nav,Vector3 centre,CarryItem[] inhabitants){
  var root=new GameObject("Creature discovery toys");root.transform.SetParent(parent,false);return root.AddComponent<CreaturePlayground>().Initialize(nav,centre,inhabitants);
 }
 CreatureAffordance[] Initialize(IslandNavigation nav,Vector3 centre,CarryItem[] inhabitants){
  materials=new[]{new Color(.13f,.38f,.63f),new Color(.73f,.27f,.08f),new Color(.65f,.48f,.13f)}.Select(c=>{var m=new Material(Shader.Find("BWVR/GameSurface"));m.color=c;return m;}).ToArray();
  float u=nav.Scale;
  ConstructionSite.Build(transform,nav,centre);
  const int rate=22050;var samples=new float[rate/2];for(int i=0;i<samples.Length;i++){float t=i/(float)rate;samples[i]=Mathf.Sin(t*880*Mathf.PI*2)*Mathf.Exp(-t*9)*.2f;}
  chime=AudioClip.Create("Soft wooden chime",samples.Length,1,rate,false);chime.SetData(samples,0);
  for(int i=0;i<3;i++){
   if(i==1)continue; // Individual physical bricks replace the decorative block cluster.
   var at=centre+new Vector3((i-1)*10,0,-12)*u;if(!nav.Sample(at,nav.creature,18,out var safe)){Debug.LogWarning("No safe creature toy site "+i);continue;}if(nav.Terrain!=null)safe=nav.Terrain.Project(safe);
   var toy=SourcedPlayArt.Part(i==0?"ball":"chime-body",transform,"Sourced toy",Vector3.zero,Vector3.one);if(i==0)toy.AddComponent<SphereCollider>();else toy.AddComponent<BoxCollider>();toy.name=new[]{"Blue rolling ball","Amber turning blocks","Golden discovery chime"}[i];toy.transform.SetParent(transform,false);toy.transform.position=safe+Vector3.up*u*1.5f;toy.transform.localScale=Vector3.one*u*(i==2?2:3);
   var item=toy.AddComponent<CarryItem>();item.kind=Offering.Toy;item.home=toy.transform.position;
   var affordance=toy.AddComponent<CreatureAffordance>();affordance.kind=(CreatureAffordance.Kind)i;HybridTarget.Bind(toy,i==0?"object-12":"object-13",HybridTargetKind.Object);
   if(i==0){toy.name="Large striped beachball";toy.AddComponent<BeachBall>().Initialize(nav);}
   if(i==2){
    toy.transform.position=safe+Vector3.up*u*.25f;item.home=toy.transform.position;toy.transform.localScale=new Vector3(2.8f,.45f,1.6f)*u;
    for(int note=0;note<2;note++){var bar=SourcedPlayArt.Part(note==0?"chime-low":"chime-high",toy.transform,note==0?"Low chime bar":"High chime bar",new Vector3(note==0?-.23f:.23f,.7f,0),new Vector3(.36f,.35f,note==0?.85f:.65f),materials[2]);var collider=bar.AddComponent<BoxCollider>();collider.isTrigger=true;bar.AddComponent<ChimeKey>().note=note;}
    var audio=toy.AddComponent<AudioSource>();audio.clip=chime;audio.playOnAwake=false;audio.spatialBlend=1;audio.minDistance=u*10;audio.maxDistance=u*100;audio.volume=.35f;toy.AddComponent<SharedRhythm>();toy.AddComponent<PlayerRhythm>();}
  }
  foreach(var item in inhabitants)if(item.kind==Offering.Food||item.kind==Offering.Villager){var affordance=item.gameObject.AddComponent<CreatureAffordance>();affordance.kind=item.kind==Offering.Food?CreatureAffordance.Kind.Food:CreatureAffordance.Kind.Villager;}
  return transform.parent.GetComponentsInChildren<CreatureAffordance>();
 }
 void OnDestroy(){if(materials!=null)foreach(var material in materials)Destroy(material);if(chime!=null)Destroy(chime);}
}
}
