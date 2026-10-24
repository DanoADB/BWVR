using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace BWVR.Editor {
public static class IslandTerrainImporter {
 [Serializable] class Export {public int version,width;public int[] heights,flags,bounds;public float cellSize,heightUnit;public string source,sha256;}
 const string Local="Assets/LocalTerrain/Resources";
 [MenuItem("BWVR/Import original first island terrain")]
 public static void Import(){
  var input=JsonUtility.FromJson<Export>(File.ReadAllText(".local/land1.json"));if(input.version!=1||input.width!=513||input.heights.Length!=513*513||input.flags.Length!=input.heights.Length||input.bounds.Length!=4)throw new Exception("Invalid first-island export");
  Directory.CreateDirectory(Local);AssetDatabase.Refresh();var path=Local+"/FirstIsland.asset";var data=AssetDatabase.LoadAssetAtPath<IslandTerrainData>(path);if(data==null){data=ScriptableObject.CreateInstance<IslandTerrainData>();AssetDatabase.CreateAsset(data,path);}
  data.width=input.width;data.heights=input.heights;data.flags=input.flags;data.bounds=input.bounds;data.cellSize=input.cellSize;data.heightUnit=input.heightUnit;data.sourceHash=input.sha256;data.scale=4.8f/((input.bounds[2]-input.bounds[0])*input.cellSize);data.anchorHeight=data.OriginalHeight(data.origin.x,data.origin.y);
  int minX=input.bounds[0]-1,minZ=input.bounds[1]-1,maxX=input.bounds[2]+1,maxZ=input.bounds[3]+1;int w=maxX-minX+1,h=maxZ-minZ+1;var vertices=new Vector3[w*h];var uv=new Vector2[vertices.Length];var triangles=new List<int>();
  for(int z=0;z<h;z++)for(int x=0;x<w;x++){int gx=minX+x,gz=minZ+z,i=z*w+x;vertices[i]=data.World(gx,gz,data.Altitude(gx,gz));uv[i]=new Vector2(vertices[i].x,vertices[i].z);}
  for(int z=0;z<h-1;z++)for(int x=0;x<w-1;x++){
   int a=z*w+x,b=a+1,c=a+w,d=c+1,gx=minX+x,gz=minZ+z;
   if(data.Altitude(gx,gz)==0&&data.Altitude(gx+1,gz)==0&&data.Altitude(gx,gz+1)==0&&data.Altitude(gx+1,gz+1)==0)continue;
   if((data.flags[gz*data.width+gx]&128)==0)triangles.AddRange(new[]{a,d,b,a,c,d});else triangles.AddRange(new[]{a,c,b,b,c,d});
  }
  var mesh=new Mesh{name="Land 1 original height geometry"};mesh.indexFormat=UnityEngine.Rendering.IndexFormat.UInt32;mesh.vertices=vertices;mesh.uv=uv;mesh.triangles=triangles.ToArray();mesh.RecalculateNormals();mesh.RecalculateBounds();
  var meshPath=Local+"/FirstIslandMesh.asset";var existing=AssetDatabase.LoadAssetAtPath<Mesh>(meshPath);if(existing==null)AssetDatabase.CreateAsset(mesh,meshPath);else{EditorUtility.CopySerialized(mesh,existing);UnityEngine.Object.DestroyImmediate(mesh);mesh=existing;}data.mesh=mesh;EditorUtility.SetDirty(data);
  const string matPath="Assets/BWVR/Environment/Resources/QuaterniusGround.mat";var material=AssetDatabase.LoadAssetAtPath<Material>(matPath);if(material==null){material=new Material(Shader.Find("BWVR/IslandGround"));AssetDatabase.CreateAsset(material,matPath);}material.shader=Shader.Find("BWVR/IslandGround");
  const string textures="Assets/ThirdParty/Quaternius/Nature/Textures/";
  material.SetTexture("_Grass",AssetDatabase.LoadAssetAtPath<Texture2D>(textures+"Grass.png"));material.SetTexture("_Rock",AssetDatabase.LoadAssetAtPath<Texture2D>(textures+"Rocks_Diffuse.png"));material.SetTexture("_Sand",AssetDatabase.LoadAssetAtPath<Texture2D>(textures+"Rocks_Desert_Diffuse.png"));EditorUtility.SetDirty(material);AssetDatabase.SaveAssets();
  Debug.Log("BWVR_LAND1_IMPORTED: "+vertices.Length+" vertices, "+triangles.Count/3+" triangles; 4.8m wide; source="+data.sourceHash);
 }
}
}
