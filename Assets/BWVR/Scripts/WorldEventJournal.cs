namespace BWVR {
public readonly struct WorldEvent {
 public readonly float time;public readonly string kind,target,reason;public readonly CreatureAction action;
 public WorldEvent(float at,string type,CreatureAction activity,string who,string why){time=at;kind=type;action=activity;target=who;reason=why;}
}
// Diagnostic receipts stay transient and bounded; save only the current intention/lesson.
public sealed class WorldEventJournal {
 readonly WorldEvent[] entries=new WorldEvent[48];int next;public int Count{get;private set;}
 public WorldEvent Latest=>Count==0?default:entries[(next+entries.Length-1)%entries.Length];
 public void Add(float time,string kind,CreatureAction action,string target,string reason){entries[next]=new WorldEvent(time,kind,action,target,reason);next=(next+1)%entries.Length;Count=System.Math.Min(entries.Length,Count+1);}
 public WorldEvent[] Capture(){var copy=new WorldEvent[Count];for(int i=0;i<Count;i++)copy[i]=entries[(next-Count+i+entries.Length)%entries.Length];return copy;}
}
}
