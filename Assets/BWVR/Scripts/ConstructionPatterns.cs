using System;
using UnityEngine;
namespace BWVR {
public enum BuildRecipe{Arch,Bench,Tower,Bridge}
public readonly struct BrickPlacement {
 public readonly Vector3 position;public readonly bool beam;
 public BrickPlacement(float x,float y,float z,bool longPiece=false){position=new Vector3(x,y,z);beam=longPiece;}
}
public static class ConstructionPatterns {
 public static BrickPlacement[] Plan(BuildRecipe recipe,float layer=1.2f){
  if(recipe==BuildRecipe.Bench||recipe==BuildRecipe.Bridge)return new[]{new BrickPlacement(-2,layer*.5f,0),new BrickPlacement(2,layer*.5f,0),new BrickPlacement(0,layer*1.5f,0,true)};
  var result=new BrickPlacement[8];if(recipe==BuildRecipe.Tower){result[0]=new BrickPlacement(0,layer*.5f,0,true);for(int i=1;i<8;i++)result[i]=new BrickPlacement(0,layer*(i+.5f),0);return result;}
  for(int i=0;i<6;i++)result[i]=new BrickPlacement(i%2==0?-2:2,layer*(.5f+i/2),0);result[6]=new BrickPlacement(0,layer*3.5f,0,true);result[7]=new BrickPlacement(0,layer*4.5f,0);return result;
 }
}
[Serializable] public sealed class BuildingMemory {
 public int observed=1,mainRecipe,lastSelected;public int[] cacheRecipes={0,0};public int[] completed=new int[4];public float[] preference={.5f,.5f,.5f,.5f};public ConstructionRecord[] bridge;
 public bool Knows(BuildRecipe recipe)=>(observed&(1<<(int)recipe))!=0;
 public bool Valid(){if(observed<0||observed>15||mainRecipe<0||mainRecipe>2||lastSelected<0||lastSelected>3||cacheRecipes==null||cacheRecipes.Length!=2||completed==null||completed.Length!=4||preference==null||preference.Length!=4||!ConstructionSite.Valid(bridge))return false;foreach(int r in cacheRecipes)if(r<0||r>2)return false;for(int i=0;i<4;i++)if(completed[i]<0||completed[i]>1000000||!HybridState.Number(preference[i],0,1))return false;return true;}
}
}
