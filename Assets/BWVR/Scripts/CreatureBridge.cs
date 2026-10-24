using System;
using UnityEngine;
using UnityEngine.AI;
namespace BWVR {
// One authored construction opportunity on a real shoreline gap. Passage exists
// only while the three supporting physical pieces form the learned span.
public sealed class CreatureBridge:MonoBehaviour {
 public ConstructionSite Site{get;private set;}public bool Walkable{get;private set;}public Mesh NavigationMesh{get;private set;}
 public Vector3 BankA{get;private set;}public Vector3 BankB{get;private set;}public float PreviousDistance{get;private set;}
 public ConstructionInspiration Example{get;private set;}IslandNavigation nav;GameObject ramps;Material wood;bool initializing;float leftY,rightY;
 public static CreatureBridge Create(Transform parent,IslandNavigation navigation,Vector3 village){
  if(navigation.Terrain==null)return null;
  Vector3 best=default;Quaternion facing=Quaternion.identity;float scale=0,bestDistance=float.MaxValue,detour=0;var terrain=navigation.Terrain;var path=new NavMeshPath();
  for(int x=-450;x<=450;x+=4)for(int z=-450;z<=450;z+=4){
   var centre=village+new Vector3(x,0,z)*navigation.Scale;if(terrain.Dry(centre,false))continue;
   for(int direction=0;direction<8;direction++)for(int span=6;span<=16;span+=2){
    var axis=Quaternion.Euler(0,direction*22.5f,0)*Vector3.right;float u=span*navigation.Scale/4;var a=centre-axis*u*5;var b=centre+axis*u*5;
    if(!terrain.Dry(centre-axis*u*2,false)||!terrain.Dry(centre+axis*u*2,false)||terrain.Dry(centre-axis*u*.5f,false)||terrain.Dry(centre+axis*u*.5f,false))continue;
    float proximity=(centre-village).sqrMagnitude;if(proximity>=bestDistance)continue;
    var rotation=Quaternion.LookRotation(Vector3.Cross(axis,Vector3.up));bool dry=true;float highest=float.NegativeInfinity,lowest=float.PositiveInfinity;
    foreach(int side in new[]{-1,1})for(int sx=4;sx<=6;sx+=2)for(int sz=-1;sz<=1;sz++){var at=centre+rotation*new Vector3(side*sx,0,sz)*u;if(!terrain.Dry(at,false)){dry=false;break;}float y=terrain.Project(at).y;highest=Mathf.Max(highest,y);lowest=Mathf.Min(lowest,y);}
    if(!dry||highest-lowest>u*.5f||!navigation.Sample(a,navigation.villager,1,out a)||!navigation.Sample(b,navigation.villager,1,out b)||!navigation.Sample(a,navigation.creature,2,out _)||!navigation.Sample(b,navigation.creature,2,out _))continue;
    if(!NavMesh.CalculatePath(navigation.ToNav(a),navigation.ToNav(b),new NavMeshQueryFilter{agentTypeID=navigation.villager.id,areaMask=1},path)||path.status!=NavMeshPathStatus.PathComplete)continue;
    float length=0;for(int i=1;i<path.corners.Length;i++)length+=Vector3.Distance(path.corners[i-1],path.corners[i]);
    if(length< span*2.5f*1.6f)continue; // Actual ramp-to-ramp shortcut must be useful.
    centre.y=highest;best=centre;facing=rotation;scale=u;bestDistance=proximity;detour=length;
   }
  }
  if(scale==0){Debug.LogWarning("BWVR_BRIDGE_SKIPPED: no safe useful shoreline gap");return null;}
  var go=new GameObject("Learnable creek crossing");go.transform.SetParent(parent,false);var bridge=go.AddComponent<CreatureBridge>();bridge.nav=navigation;bridge.PreviousDistance=detour;
  bridge.Site=ConstructionSite.BuildBridge(go.transform,best,facing,scale);bridge.BankA=terrain.Project(bridge.Site.World(new Vector3(-5,0,0)));bridge.BankB=terrain.Project(bridge.Site.World(new Vector3(5,0,0)));bridge.leftY=bridge.BankA.y;bridge.rightY=bridge.BankB.y;
  bridge.Example=ConstructionInspiration.At(go.transform,bridge.BankA,BuildRecipe.Bridge,"creek-gap",navigation.Scale);
  bridge.CreateRamps();bridge.Site.Changed+=bridge.Refresh;Debug.Log("BWVR_BRIDGE_OPPORTUNITY: previous land route "+detour.ToString("F1")+" game units; physical span "+(scale*4/navigation.Scale).ToString("F1"));return bridge;
 }
 void CreateRamps(){
  var vertices=new Vector3[8];float top=Site.transform.position.y+Site.unit*.6f;float[] x={-5,-3,3,5};
  for(int i=0;i<4;i++)for(int side=0;side<2;side++){var at=Site.World(new Vector3(x[i],0,side==0?-1:1));at.y=i==0?leftY:i==3?rightY:top;vertices[i*2+side]=at;}
  // Upward winding, continuous ramp-deck-ramp geometry in native game units.
  int[] triangles={0,1,2,1,3,2,2,3,4,3,5,4,4,5,6,5,7,6};NavigationMesh=new Mesh{name="Completed brick bridge navigation"};var native=new Vector3[8];for(int i=0;i<8;i++)native[i]=nav.ToNav(vertices[i]);NavigationMesh.vertices=native;NavigationMesh.triangles=triangles;NavigationMesh.RecalculateBounds();
  ramps=new GameObject("Bridge approach ramps");ramps.transform.SetParent(transform,false);var mesh=new Mesh{name="Visible bank ramps"};mesh.vertices=vertices;mesh.triangles=new[]{0,1,2,1,3,2,4,5,6,5,7,6};mesh.RecalculateNormals();ramps.AddComponent<MeshFilter>().sharedMesh=mesh;wood=new Material(Shader.Find("BWVR/GameSurface")){color=new Color(.37f,.23f,.12f)};ramps.AddComponent<MeshRenderer>().sharedMaterial=wood;ramps.AddComponent<MeshCollider>().sharedMesh=mesh;ramps.SetActive(false);
 }
 public void Refresh(){if(initializing)return;bool intact=Site.RecipeComplete;if(intact==Walkable)return;Walkable=intact;ramps.SetActive(intact);nav.BridgeChanged(this);}
 public void Restore(ConstructionRecord[] records,bool known){initializing=true;try{Site.Locked=!known;Site.Restore(records);}finally{initializing=false;}Refresh();}
 public bool Project(Vector3 point,out Vector3 result){var p=Site.Local(point);result=point;if(Mathf.Abs(p.z)>1.05f||Mathf.Abs(p.x)>5)return false;float top=Site.transform.position.y+Site.unit*.6f;result.y=p.x< -3?Mathf.Lerp(leftY,top,(p.x+5)/2):p.x>3?Mathf.Lerp(top,rightY,(p.x-3)/2):top;return true;}
 public bool OverWater(Vector3 point){var p=Site.Local(point);return Mathf.Abs(p.x)<5&&Mathf.Abs(p.z)<1.8f&&!nav.Terrain.Dry(point,false);}
 public Vector3 NearestBank(Vector3 point)=>Vector3.SqrMagnitude(point-BankA)<Vector3.SqrMagnitude(point-BankB)?BankA:BankB;
 void OnDestroy(){if(Site!=null)Site.Changed-=Refresh;if(NavigationMesh!=null)Destroy(NavigationMesh);if(ramps!=null&&ramps.GetComponent<MeshFilter>().sharedMesh!=null)Destroy(ramps.GetComponent<MeshFilter>().sharedMesh);if(wood!=null)Destroy(wood);}
}
}
