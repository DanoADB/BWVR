using UnityEngine;
namespace BWVR {
[System.Serializable] public sealed class IslandPlacement {
 public int line,town,variant;public string command,kind,type;public float x,z,yaw,size=1,vertical,pitch,roll;
}
public sealed class IslandTerrainData:ScriptableObject {
 public int width;public int[] heights,flags,bounds;public float cellSize=10,heightUnit=.67f,scale;public Vector2 origin=new Vector2(1865.61f,2641.24f);public float anchorHeight;public Mesh mesh;public string sourceHash;public string placementHash;public IslandPlacement[] placements=System.Array.Empty<IslandPlacement>();
 public bool HasPlacements=>placements!=null&&placements.Length>0;
 public Vector3 PlacementPoint(IslandPlacement item){var point=Project(World(item.x/cellSize,item.z/cellSize,0));point.y+=item.vertical*scale;return point;}
 public float SeaLevel=>.82f-anchorHeight*scale;
 public Vector3 World(float x,float z,float altitude)=>new Vector3((x*cellSize-origin.x)*scale,SeaLevel+altitude*heightUnit*scale,(z*cellSize-origin.y)*scale);
 public float OriginalHeight(float x,float z){float gx=x/cellSize,gz=z/cellSize;int ix=Mathf.FloorToInt(gx),iz=Mathf.FloorToInt(gz);if(ix<0||iz<0||ix>=width-1||iz>=width-1)return 0;
  float u=gx-ix,v=gz-iz;float a=Altitude(ix,iz),b=Altitude(ix+1,iz),c=Altitude(ix,iz+1),d=Altitude(ix+1,iz+1);bool split=(flags[iz*width+ix]&128)!=0;
  float height=split?(u+v<=1?a+(b-a)*u+(c-a)*v:d+(c-d)*(1-u)+(b-d)*(1-v)):(u>=v?a+(b-a)*u+(d-b)*v:a+(d-c)*u+(c-a)*v);return height*heightUnit;
 }
 public int Altitude(int x,int z)=>Mathf.Max(0,heights[z*width+x]);
 public Vector3 Project(Vector3 at){at.y=SeaLevel+OriginalHeight(at.x/scale+origin.x,at.z/scale+origin.y)*scale;return at;}
 public Vector3 SafePoint(Vector3 preferred){if(Dry(preferred))return Project(preferred);for(int ring=1;ring<=25;ring++)for(int i=0;i<24;i++){float angle=i*Mathf.PI/12;var candidate=preferred+new Vector3(Mathf.Cos(angle),0,Mathf.Sin(angle))*ring*.02f;if(Dry(candidate))return Project(candidate);}return Project(preferred);}
 public bool Dry(Vector3 at,bool limitSlope=true){float x=at.x/scale+origin.x,z=at.z/scale+origin.y;int gx=Mathf.FloorToInt(x/cellSize),gz=Mathf.FloorToInt(z/cellSize);
  if(gx<0||gz<0||gx>=width-1||gz>=width-1||heights[gz*width+gx]<3||(flags[gz*width+gx]&64)!=0)return false;
  if(!limitSlope)return true;float h=OriginalHeight(x,z);float dx=(OriginalHeight(x+cellSize,z)-h)/cellSize,dz=(OriginalHeight(x,z+cellSize)-h)/cellSize;return dx*dx+dz*dz<.49f;
 }
}
}
