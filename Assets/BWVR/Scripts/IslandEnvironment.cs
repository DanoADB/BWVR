using UnityEngine;
namespace BWVR {
public static class IslandEnvironment {
 const float Floor=.82f;
 static GameObject Place(EnvironmentArt art,string key,Transform parent,Vector3 position,Vector3 size,float yaw=0){
  var prefab=art.Find(key);if(prefab==null)return null;var g=Object.Instantiate(prefab,parent);g.transform.localPosition=position;g.transform.localScale=size;g.transform.localRotation=Quaternion.Euler(0,yaw,0);return g;
 }
 public static void Build(EnvironmentArt art,Transform parent,Renderer ground,IslandTerrainData terrain=null){
  var root=new GameObject("Quaternius CC0 island environment").transform;root.SetParent(parent,false);
  for(int i=0;i<4;i++){
   var house=new GameObject("Modular village home "+(i+1)).transform;house.SetParent(root,false);house.localPosition=new Vector3(-.5f+i*.28f,Floor,.48f);house.localRotation=Quaternion.Euler(0,(i%2==0?8:-8),0);
   Place(art,"Wall_Plaster_Door_Round",house,new Vector3(0,0,-.075f),new Vector3(.17f,.12f,.018f));
   Place(art,"Door_1_Round",house,new Vector3(0,0,-.086f),new Vector3(.05f,.09f,.008f));
   Place(art,"Wall_Plaster_Straight",house,new Vector3(0,0,.075f),new Vector3(.17f,.12f,.018f),180);
   for(int side=-1;side<=1;side+=2)Place(art,"Wall_Plaster_Window_Thin_Round",house,new Vector3(side*.085f,0,0),new Vector3(.15f,.12f,.018f),side*90);
   Place(art,"Roof_RoundTiles_4x4",house,new Vector3(0,.12f,0),new Vector3(.21f,.075f,.20f));
   Place(art,"Roof_Front_Brick4",house,new Vector3(0,.12f,-.075f),new Vector3(.17f,.065f,.014f));
   var box=house.gameObject.AddComponent<BoxCollider>();box.center=new Vector3(0,.09f,0);box.size=new Vector3(.18f,.18f,.17f);
  }
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
  var cloudMaterial=new Material(Shader.Find("BWVR/GameSurface"));cloudMaterial.color=new Color(.85f,.90f,.87f);
  for(int i=0;i<6;i++)for(int puff=0;puff<3;puff++){
   var cloud=GameObject.CreatePrimitive(PrimitiveType.Sphere);cloud.name="Soft horizon cloud";cloud.transform.SetParent(root,false);float a=i*1.05f;cloud.transform.localPosition=new Vector3(Mathf.Cos(a)*4+puff*.3f,2.6f+Mathf.Sin(i)*.25f,Mathf.Sin(a)*4);cloud.transform.localScale=new Vector3(.8f,.18f+puff*.03f,.4f);Object.Destroy(cloud.GetComponent<Collider>());cloud.GetComponent<Renderer>().sharedMaterial=cloudMaterial;
  }
 }
}
}
