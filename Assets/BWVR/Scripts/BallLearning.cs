using System;
using UnityEngine;
namespace BWVR {
[Serializable]public sealed class BallLearning {
 public float confidence=.25f,lastObserved=-100,lastPracticed=-100;public int seenGood,seenBad,practicedGames;public string demonstrator="";
 public bool Observe(bool safe,string who,float time){if(time-lastObserved<12)return false;lastObserved=time;demonstrator=who??"";confidence=Mathf.Clamp01(confidence+(safe?.12f:-.16f));if(safe)seenGood=Math.Min(1000000,seenGood+1);else seenBad=Math.Min(1000000,seenBad+1);return true;}
 public void Practice(float time){if(time-lastPracticed<12)return;lastPracticed=time;confidence=Mathf.Clamp01(confidence+.08f);practicedGames=Math.Min(1000000,practicedGames+1);}
 public bool Valid()=>HybridState.Number(confidence,0,1)&&HybridState.Number(lastObserved,-100,100000000)&&HybridState.Number(lastPracticed,-100,100000000)&&seenGood>=0&&seenGood<=1000000&&seenBad>=0&&seenBad<=1000000&&practicedGames>=0&&practicedGames<=1000000&&demonstrator!=null&&demonstrator.Length<=160;
}
public sealed partial class SocialMemory {
 public BallLearning ballLearning;public BallLearning BallLesson=>ballLearning??(ballLearning=new BallLearning());
 public float PeerPlayDrive=>Mathf.Clamp01(.2f+boldness*.15f+BallLesson.confidence*.6f);
 public bool WantsPeerPlay(int seed,int serial,string identity)=>(CreatureDecisionModel.Variation(seed,serial,CreatureAction.PlayWithToy,identity+"-peer-play")+1)*.5f<PeerPlayDrive;
}
}
