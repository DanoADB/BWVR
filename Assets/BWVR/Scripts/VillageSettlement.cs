using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
namespace BWVR {
// Decorative settlement geometry follows the same walkable surface as its inhabitants.
public sealed class VillageSettlement:MonoBehaviour {
 public readonly List<Vector3[]> routes=new List<Vector3[]>();
 public int sites,connectedSites,detailGroups;
 readonly List<Mesh> ownedMeshes=new List<Mesh>();readonly List<Material> ownedMaterials=new List<Material>();
 IslandTerrainData terrain;IslandNavigation nav;float unit;
 public static void Build(Transform parent,IslandTerrainData terrain,IslandNavigation navigation,Vector3 cistern,Vector3 well){
  if(terrain==null||!terrain.HasPlacements)return;
  var root=new GameObject("Village lanes and everyday details");root.transform.SetParent(parent,false);
  root.AddComponent<VillageSettlement>().Initialize(terrain,navigation,cistern,well);
 }
 void Initialize(IslandTerrainData data,IslandNavigation navigation,Vector3 cistern,Vector3 well){
  terrain=data;nav=navigation;unit=data.scale;
  var homes=transform.parent.GetComponentsInChildren<IslandPlacementView>().Where(v=>v.redesigned).OrderBy(v=>v.sourceLine).ToArray();
  var points=new List<Vector3>();
  void Add(Vector3 at,float range){if(nav.Sample(at,nav.villager,range,out var safe))points.Add(safe);else Debug.LogWarning("BWVR_PATH_SITE_UNREACHABLE: "+at);}
  Add(cistern,12);Add(well,12);
  foreach(var home in homes){var box=home.GetComponent<BoxCollider>();Add(home.transform.position,box.bounds.extents.magnitude/unit+6);}
  foreach(var field in data.placements.Where(p=>p.kind=="field"))Add(data.PlacementPoint(field),8);
  sites=homes.Length+2+data.placements.Count(p=>p.kind=="field");
  // A spanning network shares lanes rather than drawing one spoke per house.
  var joined=new HashSet<int>();if(points.Count>0)joined.Add(0);
  var rejected=new HashSet<long>();var filter=new NavMeshQueryFilter{agentTypeID=nav.villager.id,areaMask=1};
  while(joined.Count<points.Count){
   int from=-1,to=-1;float distance=float.MaxValue;
   foreach(int a in joined)for(int b=0;b<points.Count;b++)if(!joined.Contains(b)&&!rejected.Contains((long)a*points.Count+b)){
    float d=(points[a]-points[b]).sqrMagnitude;if(d<distance){distance=d;from=a;to=b;}
   }
   if(to<0)break;
   var path=new NavMeshPath();
   if(!NavMesh.CalculatePath(nav.ToNav(points[from]),nav.ToNav(points[to]),filter,path)||path.status!=NavMeshPathStatus.PathComplete){rejected.Add((long)from*points.Count+to);continue;}
   routes.Add(path.corners.Select(nav.ToWorld).ToArray());joined.Add(to);
  }
  connectedSites=joined.Count;
  var vertices=new List<Vector3>();var triangles=new List<int>();var uv=new List<Vector2>();
  foreach(var route in routes){
   var samples=new List<Vector3>();
   for(int segment=1;segment<route.Length;segment++){
    var a=route[segment-1];var b=route[segment];int steps=Mathf.Max(1,Mathf.CeilToInt(Vector3.Distance(a,b)/(unit*.65f)));
    for(int i=0;i<steps;i++)samples.Add(Vector3.Lerp(a,b,i/(float)steps));
   }
   if(samples.Count==0)continue;samples.Add(route[route.Length-1]);float length=0;
   for(int i=0;i<samples.Count;i++){
    if(i>0)length+=Vector3.Distance(samples[i-1],samples[i]);
    var incoming=(samples[i]-samples[Mathf.Max(0,i-1)]).normalized;var outgoing=(samples[Mathf.Min(samples.Count-1,i+1)]-samples[i]).normalized;
    var across=Vector3.Cross(Vector3.up,(incoming+outgoing).normalized)*unit*1.5f;int n=vertices.Count;
    vertices.Add(terrain.Project(samples[i]-across)+Vector3.up*unit*.15f);vertices.Add(terrain.Project(samples[i]+across)+Vector3.up*unit*.15f);
    uv.Add(new Vector2(0,length/unit));uv.Add(new Vector2(1,length/unit));
    if(i>0)triangles.AddRange(new[]{n-2,n,n-1,n-1,n,n+1});
   }
  }
  var pathMaterial=new Material(Resources.Load<Material>("VillagePath")){name="Soft edged worn village lanes"};ownedMaterials.Add(pathMaterial);
  MeshObject("Connected dirt footpaths",vertices,triangles,uv,pathMaterial,false);
  // Low, non-colliding dressing stays outside lanes; no invisible obstacles for actors.
  var wood=Material("Weathered village timber",new Color(.34f,.20f,.095f));
  var pieces=new List<CombineInstance>();var cube=Resources.GetBuiltinResource<Mesh>("Cube.fbx");
  void Box(Vector3 centre,Vector3 size,float yaw){pieces.Add(new CombineInstance{mesh=cube,transform=Matrix4x4.TRS(centre,Quaternion.Euler(0,yaw,0),size*unit)});}
  for(int i=0;i<homes.Length;i++){
   var home=homes[i];var bounds=home.GetComponent<BoxCollider>().bounds;float radius=new Vector2(bounds.extents.x,bounds.extents.z).magnitude;
   Vector3 centre=default;bool found=false;float yaw=0;
   for(int side=0;side<8;side++){
    yaw=home.transform.eulerAngles.y+side*45;var direction=Quaternion.Euler(0,yaw,0)*Vector3.forward;
    var at=home.transform.position+direction*(radius+unit*1.7f);
    if(!terrain.Dry(at)||routes.Any(r=>Enumerable.Range(1,r.Length-1).Any(s=>VillageLayout.SegmentDistance(at,r[s-1],r[s])<unit*3)))continue;
    if(homes.Any(h=>h!=home&&h.GetComponent<BoxCollider>().bounds.SqrDistance(terrain.Project(at))<unit*unit*9))continue;
    centre=terrain.Project(at);found=true;break;
   }
   if(!found)continue;detailGroups++;
   Vector3 P(float x,float y,float z)=>centre+Quaternion.Euler(0,yaw,0)*new Vector3(x,y,z)*unit;
   if(i%3==0){ // A bench for the shared green.
    ConstructionInspiration.At(transform,P(0,0,0),BuildRecipe.Bench,"bench-"+i,unit);
    Box(P(0,.8f,0),new Vector3(2.4f,.22f,.8f),yaw);Box(P(-.85f,.4f,0),new Vector3(.2f,.8f,.6f),yaw);Box(P(.85f,.4f,0),new Vector3(.2f,.8f,.6f),yaw);Box(P(0,1.25f,.35f),new Vector3(2.4f,.6f,.15f),yaw);
   }else if(i%3==1){ // Stacked cut timber beside the home.
    for(int row=0;row<2;row++)for(int log=0;log<3-row;log++)Box(P((log-(2-row)*.5f)*.55f,.22f+row*.43f,0),new Vector3(.48f,.4f,1.8f),yaw);
   }else{ // A short garden boundary, open at both ends.
    for(int post=-1;post<=1;post++)Box(P(post*1.5f,.7f,0),new Vector3(.18f,1.4f,.18f),yaw);
    Box(P(0,.5f,0),new Vector3(3,.14f,.12f),yaw);Box(P(0,1.05f,0),new Vector3(3,.14f,.12f),yaw);
   }
  }
  var detailMesh=new Mesh{name="Combined benches timber and garden fences"};detailMesh.CombineMeshes(pieces.ToArray(),true,true);ownedMeshes.Add(detailMesh);
  var details=new GameObject("Benches, firewood and open garden fences");details.transform.SetParent(transform,false);details.AddComponent<MeshFilter>().sharedMesh=detailMesh;details.AddComponent<MeshRenderer>().sharedMaterial=wood;
  Debug.Log("BWVR_SETTLEMENT_READY: "+connectedSites+"/"+sites+" connected sites, "+routes.Count+" lanes, "+detailGroups+" detail groups");
 }
 Material Material(string label,Color color){var m=new Material(Shader.Find("BWVR/GameSurface")){name=label,color=color};m.SetFloat("_Cutoff",0);ownedMaterials.Add(m);return m;}
 void MeshObject(string label,List<Vector3> vertices,List<int> triangles,List<Vector2> uv,Material material,bool shadows){
  var mesh=new Mesh{name=label,indexFormat=UnityEngine.Rendering.IndexFormat.UInt32};mesh.SetVertices(vertices);mesh.SetTriangles(triangles,0);mesh.SetUVs(0,uv);mesh.RecalculateNormals();mesh.RecalculateBounds();ownedMeshes.Add(mesh);
  var go=new GameObject(label);go.transform.SetParent(transform,false);go.AddComponent<MeshFilter>().sharedMesh=mesh;var renderer=go.AddComponent<MeshRenderer>();renderer.sharedMaterial=material;renderer.shadowCastingMode=shadows?UnityEngine.Rendering.ShadowCastingMode.On:UnityEngine.Rendering.ShadowCastingMode.Off;
 }
 void OnDestroy(){foreach(var mesh in ownedMeshes)if(mesh!=null)Destroy(mesh);foreach(var material in ownedMaterials)if(material!=null)Destroy(material);}
}
}
