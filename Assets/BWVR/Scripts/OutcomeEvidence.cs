using System;
namespace BWVR {
public enum EvidenceKind {GuidanceOffered,Noticed,Busy,ActionStarted,ActionCompleted,ActionInterrupted,GuidanceEnded,Miracle}
public enum EvidenceSource {Autonomous,PlayerPlacement,PlayerGuidance,PlayerMiracle}
[Serializable] public sealed class OutcomeEvidence {
 public int id,relatedId;public float time;public EvidenceKind kind;public EvidenceSource source;public CreatureAction action;public CreatureSituation situation;
 public string actor="",target="",partner="",outcome="";
}
// Presentation/history only: writing evidence never awards resources or reinforcement.
[Serializable] public sealed class OutcomeLedger {
 public const int Capacity=96;
 public int version=1,nextId=1,completedCreatureActions,playerMiracles;
 public OutcomeEvidence[] events=Array.Empty<OutcomeEvidence>();
 public int Add(float time,EvidenceKind kind,EvidenceSource source,CreatureAction action,string target,string outcome,int relatedId=0,string partner="",CreatureSituation situation=CreatureSituation.Calm){
  if(nextId>=1000000000)return 0;
  var record=new OutcomeEvidence{id=nextId++,relatedId=relatedId,time=time,kind=kind,source=source,action=action,situation=situation,
   actor=Actor(kind),target=target??"",partner=partner??"",outcome=outcome??""};
  int length=events.Length;if(length<Capacity){Array.Resize(ref events,length+1);events[length]=record;}
  else {Array.Copy(events,1,events,0,Capacity-1);events[Capacity-1]=record;}
  if(kind==EvidenceKind.ActionCompleted&&record.actor=="creature")completedCreatureActions=Math.Min(1000000000,completedCreatureActions+1);
  if(kind==EvidenceKind.Miracle)playerMiracles=Math.Min(1000000000,playerMiracles+1);
  return record.id;
 }
 public bool Valid(float now){
  if(version!=1||nextId<1||nextId>1000000000||events==null||events.Length>Capacity||completedCreatureActions<0||completedCreatureActions>1000000000||playerMiracles<0||playerMiracles>1000000000)return false;
  int previous=0;float at=0;foreach(var e in events){
   if(e==null||e.id<=previous||e.id>=nextId||e.relatedId<0||e.relatedId>=e.id||!HybridState.Number(e.time,at,now)||!Enum.IsDefined(typeof(EvidenceKind),e.kind)||!Enum.IsDefined(typeof(EvidenceSource),e.source)||!Enum.IsDefined(typeof(CreatureAction),e.action)||!Enum.IsDefined(typeof(CreatureSituation),e.situation)||!Text(e.actor)||!Text(e.target)||!Text(e.partner)||!Text(e.outcome))return false;
   if((e.kind==EvidenceKind.Miracle)!=(e.source==EvidenceSource.PlayerMiracle)||(e.kind==EvidenceKind.GuidanceOffered||e.kind==EvidenceKind.GuidanceEnded)&&e.source!=EvidenceSource.PlayerGuidance)return false;
   if(e.actor!=Actor(e.kind))return false;
   previous=e.id;at=e.time;
  }return true;
 }
 static bool Text(string value)=>value!=null&&value.Length<=160;
 static string Actor(EvidenceKind kind)=>kind==EvidenceKind.Miracle||kind==EvidenceKind.GuidanceOffered?"player":kind==EvidenceKind.GuidanceEnded?"system":"creature";
}
public sealed partial class HybridState {
 public OutcomeLedger evidence;
 public OutcomeLedger Evidence=>evidence??(evidence=new OutcomeLedger());
}
}
