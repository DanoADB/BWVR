using System;
using System.Linq;
using UnityEngine;
namespace BWVR {
[Serializable] public sealed class VillagerTraits {
 public float boldness=.5f,sociability=.5f,diligence=.5f,stature=.5f;
 public bool Valid()=>HybridState.Number(boldness,0,1)&&HybridState.Number(sociability,0,1)&&HybridState.Number(diligence,0,1)&&HybridState.Number(stature,0,1);
 public static VillagerTraits Inherit(VillagerTraits a,VillagerTraits b,int seed){var random=new System.Random(seed);float Mix(float x,float y)=>Mathf.Clamp01((x+y)*.5f+((float)random.NextDouble()-.5f)*.16f);return new VillagerTraits{boldness=Mix(a.boldness,b.boldness),sociability=Mix(a.sociability,b.sociability),diligence=Mix(a.diligence,b.diligence),stature=Mix(a.stature,b.stature)};}
}
[Serializable] public sealed class FamilyChild {
 public string id;public int parentA,parentB,home;public float age;public VillagerTraits traits;
}
[Serializable] public sealed class FamilyState {
 public VillagerTraits[] adults={new VillagerTraits{boldness=.25f,sociability=.7f,diligence=.4f,stature=.35f},new VillagerTraits{boldness=.55f,sociability=.5f,diligence=.8f,stature=.65f},new VillagerTraits{boldness=.85f,sociability=.4f,diligence=.6f,stature=.5f}};
 public FamilyChild[] children=Array.Empty<FamilyChild>();public int courtA=-1,courtB=-1,parentA=-1,parentB=-1,home=-1;public float bond=.35f,together,clock,nextBirth,visitAt=30;
 public bool Partnered=>parentA>=0&&parentB>=0;
 public bool FormHousehold(int a,int b,int house){if(Partnered||a<0||b<0||a>=3||b>=3||a==b||house<0||house>1000000)return false;courtA=parentA=a;courtB=parentB=b;home=house;nextBirth=clock+300;return true;}
 public bool TryBirth(float food,int seed){if(!Partnered||children.Length>=3||bond<.75f||food<8||clock<nextBirth)return false;var child=new FamilyChild{id="child-"+children.Length,parentA=parentA,parentB=parentB,home=home,traits=VillagerTraits.Inherit(adults[parentA],adults[parentB],seed+children.Length*7919)};Array.Resize(ref children,children.Length+1);children[children.Length-1]=child;nextBirth=clock+600;return true;}
 public bool Valid(){if(adults==null||adults.Length!=3||adults.Any(a=>a==null||!a.Valid())||children==null||children.Length>3||courtA < -1||courtA>2||courtB < -1||courtB>2||(courtA==-1)!=(courtB==-1)||(courtA>=0&&courtA==courtB)||(Partnered&&(courtA!=parentA||courtB!=parentB))||parentA < -1||parentA>2||parentB < -1||parentB>2||(parentA==-1)!=(parentB==-1)||(Partnered&&(parentA==parentB||home<0))||home < -1||home>1000000||!HybridState.Number(bond,0,1)||!HybridState.Number(together,0,100000000)||!HybridState.Number(clock,0,100000000)||!HybridState.Number(nextBirth,0,100001000)||!HybridState.Number(visitAt,0,100001000))return false;var ids=new System.Collections.Generic.HashSet<string>();foreach(var c in children)if(c==null||!ids.Add(c.id)||c.id!="child-"+(ids.Count-1)||!Partnered||c.parentA!=parentA||c.parentB!=parentB||c.home!=home||!HybridState.Number(c.age,0,100000000)||c.traits==null||!c.traits.Valid())return false;return true;}
}
}
