using System;
namespace BWVR {
public enum VillageTool { BoundAxe, CultivatingHoe }
[Serializable] public sealed class ToolLearner {
 public int woodPractice,farmPractice;public bool axeKnown,hoeKnown,sawAxe,sawHoe,axeOwned,hoeOwned;
 public bool Knows(VillageTool tool)=>tool==VillageTool.BoundAxe?axeKnown:hoeKnown;
 public bool Owns(VillageTool tool)=>tool==VillageTool.BoundAxe?axeOwned:hoeOwned;
 public bool Valid()=>woodPractice>=0&&woodPractice<=1000000&&farmPractice>=0&&farmPractice<=1000000&&(!axeKnown||woodPractice>=3||sawAxe)&&(!hoeKnown||farmPractice>=3||sawHoe)&&(!axeOwned||axeKnown)&&(!hoeOwned||hoeKnown)&&(!sawAxe||axeKnown)&&(!sawHoe||hoeKnown);
}
// Three stable adult identities, two bounded capabilities, no general research tree.
[Serializable] public sealed class ToolTechnologyState {
 public ToolLearner[] learners={new ToolLearner(),new ToolLearner(),new ToolLearner()};public int woodSpent,resinSpent;
 public static int ResinCost(VillageTool tool)=>tool==VillageTool.BoundAxe?2:1;
 public bool Practice(int actor,int work){if(actor<0||actor>=3)return false;var p=learners[actor];if(work==1){p.woodPractice=Math.Min(1000000,p.woodPractice+1);if(p.woodPractice>=3&&!p.axeKnown){p.axeKnown=true;return true;}}if(work==2){p.farmPractice=Math.Min(1000000,p.farmPractice+1);if(p.farmPractice>=3&&!p.hoeKnown){p.hoeKnown=true;return true;}}return false;}
 public bool Observe(int student,int maker,VillageTool tool){if(student<0||student>=3||maker<0||maker>=3||student==maker||!Enum.IsDefined(typeof(VillageTool),tool)||!learners[maker].Owns(tool)||learners[student].Knows(tool))return false;var p=learners[student];if(tool==VillageTool.BoundAxe){p.axeKnown=p.sawAxe=true;}else{p.hoeKnown=p.sawHoe=true;}return true;}
 public bool CanCraft(ResourceState resources,int actor,VillageTool tool)=>actor>=0&&actor<3&&Enum.IsDefined(typeof(VillageTool),tool)&&learners[actor].Knows(tool)&&!learners[actor].Owns(tool)&&resources.wood>=2&&resources.resin>=ResinCost(tool);
 public bool Craft(ResourceState resources,int actor,VillageTool tool){if(!CanCraft(resources,actor,tool))return false;resources.wood-=2;resources.resin-=ResinCost(tool);woodSpent+=2;resinSpent+=ResinCost(tool);if(tool==VillageTool.BoundAxe)learners[actor].axeOwned=true;else learners[actor].hoeOwned=true;return true;}
 public bool Valid(){if(learners==null||learners.Length!=3)return false;int wood=0,resin=0;foreach(var p in learners){if(p==null||!p.Valid())return false;if(p.axeOwned){wood+=2;resin+=2;}if(p.hoeOwned){wood+=2;resin++;}}return woodSpent==wood&&resinSpent==resin;}
}
}
