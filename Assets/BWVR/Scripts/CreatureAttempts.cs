using System;
using UnityEngine;
namespace BWVR {
public enum AttemptMode {Ordinary,Chase,Echo,Spin}
[Serializable] public sealed class CreatureAttempt {
 public CreatureAction action;public AttemptMode mode;public string target="",reason="";public int failures,completions,streak;public float retryAt,updatedAt;public Vector3 point;
 public bool Valid()=>Enum.IsDefined(typeof(CreatureAction),action)&&action!=CreatureAction.IdleOrObserve&&Enum.IsDefined(typeof(AttemptMode),mode)&&(mode==AttemptMode.Ordinary||action==CreatureAction.PlayWithToy)&&!string.IsNullOrEmpty(target)&&target.Length<=160&&reason!=null&&reason.Length<=160&&failures>=0&&failures<=1000000&&completions>=0&&completions<=1000000&&streak>=0&&streak<=4&&streak<=failures&&HybridState.Number(retryAt,0,100000180)&&HybridState.Number(updatedAt,0,100000000)&&Point(point);
 static bool Point(Vector3 p)=>HybridState.Number(p.x,-10000,10000)&&HybridState.Number(p.y,-10000,10000)&&HybridState.Number(p.z,-10000,10000);
}
public sealed partial class HybridState {
 public CreatureAttempt[] attempts;
 public static AttemptMode Mode(bool chase,bool echo,bool spin)=>spin?AttemptMode.Spin:echo?AttemptMode.Echo:chase?AttemptMode.Chase:AttemptMode.Ordinary;
 public CreatureAttempt Attempt(CreatureAction action,string target,AttemptMode mode){if(attempts!=null)foreach(var a in attempts)if(a.action==action&&a.target==target&&a.mode==mode)return a;return null;}
 public bool AttemptAllowed(CreatureAction action,string target,AttemptMode mode,Vector3 point){var a=Attempt(action,target,mode);return a==null||time>=a.retryAt||(point-a.point).sqrMagnitude>=64;}
 CreatureAttempt AttemptRecord(CreatureAction action,string target,AttemptMode mode){var a=Attempt(action,target,mode);if(a!=null)return a;a=new CreatureAttempt{action=action,target=target,mode=mode};if(attempts==null)attempts=Array.Empty<CreatureAttempt>();if(attempts.Length<32){Array.Resize(ref attempts,attempts.Length+1);attempts[attempts.Length-1]=a;}else{int oldest=0;for(int i=1;i<attempts.Length;i++)if(attempts[i].updatedAt<attempts[oldest].updatedAt)oldest=i;attempts[oldest]=a;}return a;}
 public bool FailedAttempt(CreatureAction action,string target,AttemptMode mode,Vector3 point,string reason){if(action==CreatureAction.IdleOrObserve||!AttemptAllowed(action,target,mode,point))return false;var a=AttemptRecord(action,target,mode);if((point-a.point).sqrMagnitude>=64)a.streak=0;a.streak=Math.Min(4,a.streak+1);a.failures=Math.Min(1000000,a.failures+1);a.retryAt=time+15*(1<<(a.streak-1));a.updatedAt=time;a.point=point;a.reason=reason;return true;}
 public void CompletedAttempt(CreatureAction action,string target,AttemptMode mode,Vector3 point){if(action==CreatureAction.IdleOrObserve)return;var a=AttemptRecord(action,target,mode);a.completions=Math.Min(1000000,a.completions+1);a.streak=0;a.retryAt=0;a.updatedAt=time;a.point=point;a.reason="";}
 bool AttemptsValid(){if(attempts==null)return true;if(attempts.Length>32)return false;var keys=new System.Collections.Generic.HashSet<string>();foreach(var a in attempts)if(a==null||!a.Valid()||a.updatedAt>time||!keys.Add((int)a.action+"/"+(int)a.mode+"/"+a.target))return false;return true;}
}
public sealed partial class CreatureHybrid {
 void RememberFailure(CreatureAction action,string id,AttemptMode mode,HybridTarget target,string reason){if(Season.paused||suspended||target==null||!target.gameObject.activeInHierarchy||target.GetComponent<CarryItem>()?.held==true)return;if(State.FailedAttempt(action,id,mode,nav.world.ToNav(target.transform.position),reason))Journal.Add(State.time,"attempt deferred",action,id,reason+"; try a different opportunity");}
 void RememberFailure(CreatureCandidate c,HybridTarget target,string reason)=>RememberFailure(c.action,c.target,HybridState.Mode(c.chase,c.echo,c.spin),target,reason);
 void RememberFailure(CreatureIntention intent,HybridTarget target,string reason){if(intent.active&&(reason=="Target unreachable"||reason=="Action timed out"))RememberFailure(intent.action,intent.target,HybridState.Mode(intent.chase,intent.echo,intent.spin),target,reason);}
 void ApplyAttemptGate(CreatureCandidate c,HybridTarget target){if(target!=null&&!State.AttemptAllowed(c.action,c.target,HybridState.Mode(c.chase,c.echo,c.spin),nav.world.ToNav(target.transform.position))){c.feasible=false;c.reason="Trying other opportunities after an unsuccessful attempt";}}
}
}
