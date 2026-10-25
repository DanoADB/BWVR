using UnityEngine;
namespace BWVR {
public static class IslandEnvironment {
 const float Floor=.82f;
 static GameObject Place(EnvironmentArt art,string key,Transform parent,Vector3 position,Vector3 size,float yaw=0){
  var prefab=art.Find(key);if(prefab==null)return null;var g=Object.Instantiate(prefab,parent);g.transform.localPosition=position;g.transform.localScale=size;g.transform.localRotation=Quaternion.Euler(0,yaw,0);return g;
 }
 public static Transform Home(EnvironmentArt art,Transform parent,Vector3 at,float yaw,float scale,string name,int variant=0){
  int style=Mathf.Abs(variant)%3;
  float wallHeight=style==1?.105f:style==2?.13f:.12f;
  float roofHeight=.195f-wallHeight;
  var house=new GameObject(name).transform;house.SetParent(parent,false);house.localPosition=at;house.localRotation=Quaternion.Euler(0,yaw,0);house.localScale=Vector3.one*(scale*QuestSlice.BuildingScale);
  var wallTint=new[]{new Color(1,.94f,.82f),new Color(.9f,.96f,1),new Color(.76f,.90f,.78f)}[style];
  var roofTint=new[]{new Color(1.4f,.7f,.4f),new Color(.45f,1.05f,1.25f),new Color(.85f,.65f,.42f)}[style];
  void Tint(GameObject g,Color color){if(g==null)return;foreach(var r in g.GetComponentsInChildren<Renderer>())for(int slot=0;slot<r.sharedMaterials.Length;slot++){var material=r.sharedMaterials[slot];if(!material.name.Contains("Plaster")&&!material.name.Contains("RoundTiles"))continue;var block=new MaterialPropertyBlock();block.SetColor("_Color",color);if(material.name.Contains("RoundTiles"))block.SetFloat("_Recolor",1);r.SetPropertyBlock(block,slot);}}
  var front=Place(art,"Wall_Plaster_Door_Round",house,new Vector3(0,0,-.075f),new Vector3(.17f,wallHeight,.018f));Tint(front,wallTint);
  // Apply the SAME authored wall-axis conversion to its matching door, rather than
  // fitting the leaf independently. Small overlap compensates for hinge/handle bounds
  // on the door mesh, so the wooden leaf fills the entire arched aperture.
  var door=Place(art,"Door_1_Round",house,new Vector3(0,wallHeight*.0096f,-.086f),new Vector3(.17f*1.12f/2*1.15f,wallHeight*2.32f/3.12f*1.1f,.0054f));
  door.name="Fitted round door - style "+style;
  Tint(Place(art,"Wall_Plaster_Straight",house,new Vector3(0,0,.075f),new Vector3(.17f,wallHeight,.018f),180),wallTint);
  for(int side=-1;side<=1;side+=2){
   Tint(Place(art,"Wall_Plaster_Window_Thin_Round",house,new Vector3(side*.085f,0,0),new Vector3(.15f,wallHeight,.018f),side*90),wallTint);
   Place(art,"Window_Thin_Round1",house,new Vector3(side*.096f,wallHeight*1.02f/3.12f,0),new Vector3(.15f*.88f/2,wallHeight*1.6f/3.12f,.015f),side*90);
  }
  Tint(Place(art,"Roof_RoundTiles_4x4",house,new Vector3(0,wallHeight,0),new Vector3(style==2?.22f:.21f,roofHeight,.20f)),roofTint);
  Place(art,"Roof_Front_Brick4",house,new Vector3(0,wallHeight,-.075f),new Vector3(.17f,roofHeight,.014f));
  if(style!=0){
   var chimney=Place(art,"Rock_Medium_1",house,new Vector3(style==1?-.052f:.052f,.11f,.025f),new Vector3(.025f,.08f,.025f));chimney.name="Stone chimney";
   Place(art,"Rock_Medium_1",house,new Vector3(style==1?-.052f:.052f,.18f,.025f),new Vector3(.033f,.012f,.033f));
  }
  if(style==1){
   Place(art,"Window_Thin_Round1",house,new Vector3(0,wallHeight+.012f,-.095f),new Vector3(.025f,.032f,.009f));
  }
  var box=house.gameObject.AddComponent<BoxCollider>();box.center=new Vector3(0,.0975f,0);box.size=new Vector3(.18f,.195f,.17f);
  return house;
 }
 public static void Build(EnvironmentArt art,Transform parent,Renderer ground,IslandTerrainData terrain=null){
  var root=new GameObject("Quaternius CC0 island environment").transform;root.SetParent(parent,false);
  if(terrain!=null&&terrain.HasPlacements){BuildOriginal(art,root,terrain);Understory(art,root,terrain);Surface(ground,terrain,root);IslandAtmosphere.Build(root,terrain);return;}
  for(int i=0;i<4;i++)Home(art,root,new Vector3(-.5f+i*.28f,Floor,.48f),i%2==0?8:-8,1,"Modular village home "+(i+1),i);
  for(int i=0;i<11;i++){
   float angle=.12f+i*.53f;var at=new Vector3(Mathf.Cos(angle)*.75f,Floor,Mathf.Sin(angle)*.75f);
   if(at.z<-.45f&&Mathf.Abs(at.x)<.48f)continue;
   var tree=Place(art,new[]{"CommonTree_1","CommonTree_2","Pine_1"}[i%3],root,at,new Vector3(.19f,.30f+(i%3)*.025f,.19f),i*71);
   var box=tree.AddComponent<BoxCollider>();box.center=new Vector3(0,.25f,0);box.size=new Vector3(.12f,.5f,.12f);
   Place(art,"Bush_Common",root,at+new Vector3(.07f,0,-.04f),new Vector3(.1f,.06f,.08f),i*43);
  }
  for(int i=0;i<32;i++){
   float a=i*2.399963f;float radius=.58f+Mathf.Sin(i*7)*.12f;var at=new Vector3(Mathf.Cos(a)*radius,Floor+.001f,Mathf.Sin(a)*radius);
   if(at.z>.36f||Vector3.Distance(at,new Vector3(-.32f,Floor,.25f))<.17f)continue;
   Place(art,i%4==0?"Bush_Common_Flowers":i%3==0?"Fern_1":"Grass_Common_Short",root,at,new Vector3(.055f,.035f,.055f),i*47);
  }
  for(int i=0;i<15;i++){float a=i*Mathf.PI*2/15;Place(art,"Rock_Medium_"+(i%2+1),root,new Vector3(Mathf.Cos(a)*.86f,Floor-.035f,Mathf.Sin(a)*.86f),new Vector3(.13f,.075f,.11f),i*63);}
  for(int i=0;i<12;i++)Place(art,"RockPath_Round_Small_1",root,new Vector3(-.37f+i*.065f,Floor+.002f,.32f-i*.04f),new Vector3(.045f,.004f,.04f),i*29);
  if(terrain!=null){
   // Match original mesh heights to the same game-unit conversion as Land 1.
   foreach(Transform child in root){
    var at=child.localPosition;child.localPosition=new Vector3(at.x*.1f,at.y,at.z*.1f);
    float factor=child.name.StartsWith("Modular village home")?3.66f*terrain.scale/.195f:
     child.name.Contains("Tree")||child.name.Contains("Pine")?12.723f*terrain.scale/.325f:.1f;
    child.localScale*=factor;
   }
   // Scatter only outside the active village so scenery does not cover its work sites.
   for(int i=0;i<140;i++){
    float u=Mathf.Repeat(i*.61803399f,1),v=Mathf.Repeat(i*.41421356f,1);var at=terrain.World(Mathf.Lerp(terrain.bounds[0],terrain.bounds[2],u),Mathf.Lerp(terrain.bounds[1],terrain.bounds[3],v),0);
    if(new Vector2(at.x,at.z).magnitude<.095f||!terrain.Dry(at))continue;at=terrain.Project(at);
    bool tree=i%3!=0;Place(art,tree?(i%2==0?"CommonTree_1":"Pine_1"):(i%2==0?"Bush_Common":"Rock_Medium_1"),root,at,tree?new Vector3(10,12.723f,10)*terrain.scale:new Vector3(.010f,.0065f,.010f),i*137.5f);
   }
   foreach(Transform child in root){if(child.localPosition.y>2)continue;if(child.name.StartsWith("Modular village home")){child.localPosition=terrain.SafePoint(child.localPosition);continue;}if(!terrain.Dry(child.localPosition)){child.gameObject.SetActive(false);continue;}float offset=(child.localPosition.y-Floor)*.1f;
    // Scattered instances already have their sampled heights.
    if(new Vector2(child.localPosition.x,child.localPosition.z).magnitude<.095f){var at=terrain.Project(child.localPosition);at.y+=offset;child.localPosition=at;}
   }
   var surface=Resources.Load<Material>("QuaterniusGround");if(surface!=null){var material=new Material(surface);material.SetFloat("_SeaLevel",terrain.SeaLevel);ground.sharedMaterial=material;}
  }else{
  // Original procedural fallback for clones without locally imported game terrain.
  var texture=new Texture2D(128,128);texture.name="BWVR painted meadow";texture.wrapMode=TextureWrapMode.Repeat;
  for(int y=0;y<128;y++)for(int x=0;x<128;x++){float noise=Mathf.PerlinNoise(x*.12f,y*.12f);texture.SetPixel(x,y,Color.Lerp(new Color(.31f,.40f,.15f),new Color(.56f,.62f,.29f),noise));}texture.Apply();
  var material=new Material(Shader.Find("BWVR/GameSurface"));material.mainTexture=texture;material.mainTextureScale=Vector2.one*5;ground.sharedMaterial=material;
  }
  IslandAtmosphere.Build(root,terrain);
 }
 static void Surface(Renderer ground,IslandTerrainData terrain,Transform layout){var surface=Resources.Load<Material>("QuaterniusGround");if(surface!=null){var material=new Material(surface);material.SetFloat("_SeaLevel",terrain.SeaLevel);PaintBiomes(material,terrain,layout);ground.sharedMaterial=material;}}
 static void PaintBiomes(Material material,IslandTerrainData terrain,Transform layout){
  const int size=512;var pixels=new Color[size*size];
  var min=terrain.World(terrain.bounds[0],terrain.bounds[1],0);var max=terrain.World(terrain.bounds[2],terrain.bounds[3],0);var span=max-min;
  void Dab(Vector3 at,float radius,int channel,float strength){
   float cx=(at.x-min.x)/span.x*(size-1),cy=(at.z-min.z)/span.z*(size-1),rx=radius/span.x*(size-1),ry=radius/span.z*(size-1);
   for(int y=Mathf.Max(0,Mathf.FloorToInt(cy-ry));y<=Mathf.Min(size-1,Mathf.CeilToInt(cy+ry));y++)for(int x=Mathf.Max(0,Mathf.FloorToInt(cx-rx));x<=Mathf.Min(size-1,Mathf.CeilToInt(cx+rx));x++){
    float d=new Vector2((x-cx)/rx,(y-cy)/ry).magnitude;var color=pixels[y*size+x];color[channel]=Mathf.Max(color[channel],(1-Mathf.SmoothStep(.25f,1,d))*strength);pixels[y*size+x]=color;
   }
  }
  var moved=new System.Collections.Generic.Dictionary<int,Vector3>();
  foreach(var view in layout.GetComponentsInChildren<IslandPlacementView>())if(view.redesigned)moved[view.sourceLine]=view.transform.position;
  Vector3 Position(IslandPlacement p)=>moved.TryGetValue(p.line,out var point)?point:terrain.PlacementPoint(p);
  if(terrain.HasPlacements){
   foreach(var p in terrain.placements){var at=Position(p);
    if(p.kind=="tree")Dab(at,terrain.scale*18,1,.8f);
    if(p.kind=="field")Dab(at,terrain.scale*8,2,1);
    if(p.kind!="building")continue;Dab(at,terrain.scale*8,0,.8f);
    IslandPlacement centre=null;float closest=float.MaxValue;
    foreach(var candidate in terrain.placements)if(candidate.kind=="building"&&candidate.town==p.town&&candidate.type.Contains("TOWN_CENTRE")){float d=Vector3.Distance(terrain.PlacementPoint(candidate),at);if(d<closest){closest=d;centre=candidate;}}
    if(centre==null)continue;var end=Position(centre);int steps=Mathf.CeilToInt(Vector3.Distance(at,end)/(terrain.scale*2));
    for(int i=0;i<=steps;i++){var point=Vector3.Lerp(at,end,i/(float)Mathf.Max(1,steps));if(terrain.Dry(point))Dab(point,terrain.scale*3.5f,0,.85f);}
   }
  }
  var texture=new Texture2D(size,size,TextureFormat.RGBA32,false,true){name="Island forest and village ground",wrapMode=TextureWrapMode.Clamp};texture.SetPixels(pixels);texture.Apply(false,true);material.SetTexture("_Biome",texture);material.SetVector("_BiomeRect",new Vector4(min.x,min.z,1/span.x,1/span.z));
 }
 static void Understory(EnvironmentArt art,Transform root,IslandTerrainData terrain){
  var plants=new GameObject("Meadow and forest understory").transform;plants.SetParent(root,false);int count=0;
  var houses=root.GetComponentsInChildren<BoxCollider>();
  foreach(var p in terrain.placements){if(p.kind!="tree"||p.line%3!=0)continue;var centre=terrain.PlacementPoint(p);
   for(int i=0;i<3;i++){float a=(p.line*137.5f+i*119)*Mathf.Deg2Rad;var at=centre+new Vector3(Mathf.Cos(a),0,Mathf.Sin(a))*terrain.scale*(5+i*3);if(!terrain.Dry(at))continue;
    bool blocked=false;foreach(var box in houses)if(box.bounds.SqrDistance(terrain.Project(at))<terrain.scale*terrain.scale*9){blocked=true;break;}if(blocked)continue;
    at=terrain.Project(at);string key=i==0?"Fern_1":p.line%5==0?"Bush_Common_Flowers":"Grass_Common_Short";var g=Place(art,key,plants,at,new Vector3(4,1.5f,4)*terrain.scale,a*Mathf.Rad2Deg);if(g!=null){AddVegetationLOD(g,true);count++;}
   }
  }
  Debug.Log("BWVR_UNDERSTORY: "+count+" ground-cover clusters; original placements preserved");
 }
 static void BuildOriginal(EnvironmentArt art,Transform root,IslandTerrainData terrain){
  var buildings=Resources.Load<EnvironmentArt>("VillageBuildings");
  var layout=buildings!=null?new VillageLayout(terrain,buildings):null;
  int count=0;
  foreach(var p in terrain.placements){
   if(p.kind is "drink" or "villager")continue;
   var at=terrain.PlacementPoint(p);var name="Original "+p.kind+" L"+p.line+" - "+p.type;Transform marker;
   if(p.kind=="building"&&!p.type.Contains("STORAGE_PIT")&&!p.type.Contains("GRAVEYARD")){
    if(layout!=null){
     var plot=layout.plots[p.line];marker=Object.Instantiate(buildings.Find(plot.model),root).transform;
     marker.name="Medieval "+plot.model+" L"+p.line;marker.localPosition=plot.position;marker.localRotation=Quaternion.Euler(0,plot.yaw,0);marker.localScale=Vector3.one*plot.scale;
    }else marker=Home(art,root,at,p.yaw,3.66f*terrain.scale*p.size/.195f,name,p.line+p.town);
   }else{
    marker=new GameObject(name).transform;marker.SetParent(root,false);marker.localPosition=at;marker.localRotation=Quaternion.Euler(p.pitch,p.yaw,p.roll);
    float unit=terrain.scale*p.size;
    if(p.kind=="tree"){
     bool bush=p.variant>=13&&p.variant<=15;string key=bush?"Bush_Common":p.variant is 3 or 4 or 12?"Pine_1":p.variant%2==0?"CommonTree_1":"CommonTree_2";
     var tree=Place(art,key,marker,Vector3.zero,(bush?new Vector3(3,2.5f,3):new Vector3(10,12.723f,10))*unit);
     if(tree==null)throw new System.Exception("Missing replacement: "+key);
     AddVegetationLOD(tree,bush);
     // The catalog has no dead/palm meshes yet; use the matching broadleaf silhouette.
     if(p.variant==22)tree.transform.localScale=new Vector3(4,8,4)*unit;
    }else if(p.kind=="rock"||p.kind=="feature"){
     var size=p.kind=="rock"?new Vector3(8,6,7):p.type.Contains("Pilar")?new Vector3(5,22,5):new Vector3(4,8,4);
     Place(art,"Rock_Medium_"+(p.line%2+1),marker,Vector3.zero,size*unit);
    }else if(p.kind=="field"){
     for(int z=-1;z<=1;z++)for(int x=-1;x<=1;x++)Place(art,"Grass_Common_Short",marker,new Vector3(x*3,0,z*3)*unit,new Vector3(3,1.2f,3)*unit);
    }else if(p.kind=="well"||p.type.Contains("STORAGE_PIT")){
     StoneRing(art,marker,unit,p.kind=="well"?2:6);
    }else if(p.type.Contains("GRAVEYARD")){
     for(int i=0;i<6;i++)Place(art,"Rock_Medium_1",marker,new Vector3((i%3-1)*2,0,(i/3-.5f)*3)*unit,new Vector3(.8f,1.4f,.5f)*unit);
    }
   }
   var view=marker.gameObject.AddComponent<IslandPlacementView>();view.sourceLine=p.line;view.kind=p.kind;
   if(layout!=null&&layout.plots.TryGetValue(p.line,out var plotInfo)){view.redesigned=true;view.modelKey=plotInfo.model;}
   else if(layout!=null&&(p.kind=="tree"||p.kind=="rock"||p.kind=="feature")&&layout.Cleared(at))marker.gameObject.SetActive(false);
   count++;
  }
  // Repeated Quaternius meshes share instanced materials; avoid one material per tree.
  foreach(var renderer in root.GetComponentsInChildren<MeshRenderer>())foreach(var material in renderer.sharedMaterials)if(material!=null)material.enableInstancing=true;
  Debug.Log("BWVR_ORIGINAL_LAYOUT: "+count+" placed replacements; script="+terrain.placementHash);
 }
 static Mesh distantVegetation;
 static Material distantLeaves;
 static void AddVegetationLOD(GameObject tree,bool bush){
  if(distantVegetation==null){
   distantVegetation=new Mesh{name="Distant vegetation silhouette"};var vertices=new System.Collections.Generic.List<Vector3>();var triangles=new System.Collections.Generic.List<int>();
   // Small shared silhouette for overview distance; the full Quaternius model appears nearby.
   for(int i=0;i<8;i++){float a=i*Mathf.PI/4,b=(i+1)*Mathf.PI/4;int n=vertices.Count;vertices.Add(new Vector3(0,1,0));vertices.Add(new Vector3(Mathf.Cos(a)*.5f,.3f,Mathf.Sin(a)*.5f));vertices.Add(new Vector3(Mathf.Cos(b)*.5f,.3f,Mathf.Sin(b)*.5f));triangles.AddRange(new[]{n,n+1,n+2});
    n=vertices.Count;vertices.Add(new Vector3(0,.2f,0));vertices.Add(new Vector3(Mathf.Cos(b)*.5f,.3f,Mathf.Sin(b)*.5f));vertices.Add(new Vector3(Mathf.Cos(a)*.5f,.3f,Mathf.Sin(a)*.5f));triangles.AddRange(new[]{n,n+1,n+2});}
   vertices.AddRange(new[]{new Vector3(-.04f,0,0),new Vector3(.04f,0,0),new Vector3(.04f,.4f,0),new Vector3(-.04f,.4f,0)});int t=vertices.Count-4;triangles.AddRange(new[]{t,t+2,t+1,t,t+3,t+2});distantVegetation.SetVertices(vertices);distantVegetation.SetTriangles(triangles,0);distantVegetation.RecalculateNormals();distantVegetation.RecalculateBounds();
   distantLeaves=new Material(Shader.Find("BWVR/GameSurface")){name="Distant meadow foliage",color=new Color(.24f,.38f,.12f),enableInstancing=true};
  }
  var detailed=tree.GetComponentsInChildren<Renderer>();var proxy=new GameObject("Distant silhouette");proxy.transform.SetParent(tree.transform,false);proxy.AddComponent<MeshFilter>().sharedMesh=distantVegetation;var renderer=proxy.AddComponent<MeshRenderer>();renderer.sharedMaterial=distantLeaves;
  var lod=tree.AddComponent<LODGroup>();lod.SetLODs(new[]{new LOD(.04f,detailed),new LOD(.001f,new Renderer[]{renderer})});lod.RecalculateBounds();
 }
 public static void WaterSource(EnvironmentArt art,Transform parent,Vector3 at,float unit){
  var well=new GameObject("Modern well at original drinking waypoint").transform;well.SetParent(parent,false);well.localPosition=at;StoneRing(art,well,unit,2);
  for(int side=-1;side<=1;side+=2)Place(art,"Wall_Plaster_Straight",well,new Vector3(side*2.1f,0,0)*unit,new Vector3(.4f,4,.4f)*unit);
  Place(art,"Roof_RoundTiles_4x4",well,new Vector3(0,4,0)*unit,new Vector3(5,1.8f,4)*unit);
 }
 static void StoneRing(EnvironmentArt art,Transform parent,float unit,float radius){for(int i=0;i<12;i++){float a=i*Mathf.PI/6;Place(art,"RockPath_Round_Small_1",parent,new Vector3(Mathf.Cos(a)*radius,0,Mathf.Sin(a)*radius)*unit,new Vector3(radius*.65f,1,radius*.5f)*unit,i*30);}}
}
}
