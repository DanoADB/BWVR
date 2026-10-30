using System;
namespace BWVR {
public enum VillageTool { BoundAxe, CultivatingHoe }
[Serializable] public sealed partial class ToolLearner {
 public int woodPractice,farmPractice;public bool axeKnown,hoeKnown,sawAxe,sawHoe,axeOwned,hoeOwned;
 public bool Knows(VillageTool tool)=>tool==VillageTool.BoundAxe?axeKnown:hoeKnown;
 public bool Owns(VillageTool tool)=>tool==VillageTool.BoundAxe?axeOwned:hoeOwned;
 public bool Valid()=>UsesValid()&&woodPractice>=0&&woodPractice<=1000000&&farmPractice>=0&&farmPractice<=1000000&&(!axeKnown||woodPractice>=3||sawAxe)&&(!hoeKnown||farmPractice>=3||sawHoe)&&(!axeOwned||axeKnown)&&(!hoeOwned||hoeKnown)&&(!sawAxe||axeKnown)&&(!sawHoe||hoeKnown);
}
// Stable adult identities, two bounded capabilities, no general research tree.
[Serializable] public sealed partial class ToolTechnologyState {
 public ToolLearner[] learners=VillagePopulation.Create(i=>new ToolLearner());public int woodSpent,resinSpent;
 public void EnsurePopulation()=>VillagePopulation.Expand(ref learners,i=>new ToolLearner());
 public static int ResinCost(VillageTool tool)=>tool==VillageTool.BoundAxe?2:1;
 public bool Practice(int actor,int work){if(actor<0||actor>=learners.Length)return false;var p=learners[actor];if(work==1){p.woodPractice=Math.Min(1000000,p.woodPractice+1);if(p.woodPractice>=3&&!p.axeKnown){p.axeKnown=true;return true;}}if(work==2){p.farmPractice=Math.Min(1000000,p.farmPractice+1);if(p.farmPractice>=3&&!p.hoeKnown){p.hoeKnown=true;return true;}}return false;}
 public bool Observe(int student,int maker,VillageTool tool){if(student<0||student>=learners.Length||maker<0||maker>=learners.Length||student==maker||!Enum.IsDefined(typeof(VillageTool),tool)||!learners[maker].Owns(tool)||learners[student].Knows(tool))return false;var p=learners[student];if(tool==VillageTool.BoundAxe){p.axeKnown=p.sawAxe=true;}else{p.hoeKnown=p.sawHoe=true;}return true;}
 public bool CanCraft(ResourceState resources,int actor,VillageTool tool)=>actor>=0&&actor<learners.Length&&Enum.IsDefined(typeof(VillageTool),tool)&&learners[actor].Knows(tool)&&!learners[actor].Owns(tool)&&resources.wood>=2&&resources.resin>=ResinCost(tool);
 public bool Craft(ResourceState resources,int actor,VillageTool tool){if(!CanCraft(resources,actor,tool))return false;resources.wood-=2;resources.resin-=ResinCost(tool);woodSpent+=2;resinSpent+=ResinCost(tool);if(tool==VillageTool.BoundAxe)learners[actor].axeOwned=true;else learners[actor].hoeOwned=true;return true;}
 public bool Valid(){if(!DeliveryValid()||learners==null||!VillagePopulation.Supported(learners.Length))return false;int wood=0,resin=0;foreach(var p in learners){if(p==null||!p.Valid())return false;if(p.axeOwned){wood+=2;resin+=2;}if(p.hoeOwned){wood+=2;resin++;}}return woodSpent==wood&&resinSpent==resin;}
}
}
