using System;
namespace BWVR {
public enum PlayOutingPhase {NotStarted,Active,Complete,Ended}
[Serializable]public sealed class PlayOutingState {
 public PlayOutingPhase phase;public int attempts,completed,unlocked,rounds;public bool echo,puzzle;public int echoAt,roundsAt,puzzleAt;
 public int Progress=>(echo?1:0)|(rounds>=3?2:0)|(puzzle?4:0);
 public int Count=>(echo?1:0)+(rounds>=3?1:0)+(puzzle?1:0);
 public int Next=>!echo?0:rounds<3?1:!puzzle?2:0;
 static int Echo(WorldPlayMemory w)=>w?.echo?.games??0;
 static int Rounds(WorldPlayMemory w)=>w?.goals?.sharedRounds??0;
 static int Puzzle(WorldPlayMemory w)=>w?.puzzle?.solved??0;
 public bool Begin(WorldPlayMemory w){if(phase==PlayOutingPhase.Active||attempts>=1000000)return false;attempts++;phase=PlayOutingPhase.Active;echo=puzzle=false;rounds=0;echoAt=Echo(w);roundsAt=Rounds(w);puzzleAt=Puzzle(w);return true;}
 public bool End(){if(phase!=PlayOutingPhase.Active)return false;phase=PlayOutingPhase.Ended;return true;}
 // Only confirmed, player-led activity counters count. Historical successes are baselined.
 public int Observe(WorldPlayMemory w,bool paused){if(paused||phase!=PlayOutingPhase.Active)return 0;int before=Progress;echo|=Echo(w)>echoAt;puzzle|=Puzzle(w)>puzzleAt;rounds=Math.Max(rounds,Math.Min(3,Math.Max(0,Rounds(w)-roundsAt)));unlocked|=Progress;if(Progress==7){phase=PlayOutingPhase.Complete;completed=Math.Min(1000000,completed+1);}return Progress&~before;}
 public bool Valid()=>Enum.IsDefined(typeof(PlayOutingPhase),phase)&&attempts>=0&&attempts<=1000000&&completed>=0&&completed<=attempts&&unlocked>=0&&unlocked<=7&&(unlocked&Progress)==Progress&&rounds>=0&&rounds<=3&&echoAt>=0&&echoAt<=1000000&&roundsAt>=0&&roundsAt<=1000000&&puzzleAt>=0&&puzzleAt<=1000000&&(phase==PlayOutingPhase.NotStarted?attempts==0&&completed==0&&unlocked==0&&Progress==0&&echoAt==0&&roundsAt==0&&puzzleAt==0:attempts>0)&&(phase!=PlayOutingPhase.Active||Progress!=7)&&(phase!=PlayOutingPhase.Complete||Progress==7&&completed>0);
}
public static class PlayOutingText {
 public static CreationBookPage Read(SliceState s,int index){index=Math.Max(0,Math.Min(index,6));var p=s.playOuting;bool active=p?.phase==PlayOutingPhase.Active;int progress=p?.Progress??0;string Done(int bit)=>(progress&bit)!=0?"Done":"Open";
  if(index==0)return new CreationBookPage("Play Day · "+(p?.Count??0)+" / 3",(p?.phase==PlayOutingPhase.Complete?"A day worth remembering.":active?"Your outing is underway.":p?.phase==PlayOutingPhase.Ended?"Outing ended. Memories remain.":"An invitation to spend time together.")+"\n\n"+Done(1)+" · Echo a four-note phrase\n"+Done(2)+" · Trade three goal rounds ("+(p?.rounds??0)+"/3)\n"+Done(4)+" · Solve the Together gate\n\nAny order. No timer. Turn pages for help.\nThe creature may choose another activity.\nCompleted outings: "+(p?.completed??0),index,7);
  int activity=(index-1)%3,bit=1<<activity;bool learned=((p?.unlocked??0)&bit)!=0;string title=new[]{"A rhythm between you","Taking turns","Stronger together"}[activity];
  string[] instructions={"Mark the chime and approach it.\nLet the creature see you tap four notes\non its blue and amber bars.\n\nWait nearby: it may choose to echo you.\nPraise its completed reply with A (desktop 1).\nA complete echo records this experiment.","First roll through a goal while watched.\nSelect the jewel above that goal to invite turns.\n\nTake your shot, then let the creature retrieve\nand play. Wait for the ball to come back.\nThree completed shared rounds count;\nmisses still count as taking turns.","Carry the blue key to its matching socket.\nThe creature can choose to hold the paw pad.\n\nWhen the shutter opens and the launch cue\nis ready, send the beachball through.\nBoth of you must help; a key alone is not enough.\nYou can leave and try again later."};
  string[] notes={"FIELD NOTE · A reply, not a command\n\nA witnessed phrase received a complete echo.\nThis is learned participation, not obedience.\n\nTry changing the rhythm next time.\nNotice who comes to listen.","FIELD NOTE · The return matters\n\nThree rounds included both turns and returns.\nA missed shot can still be good company.\n\nTry moving a goal to change the approach.\nKeep space clear for your partner.","FIELD NOTE · A shared possibility\n\nThe key, the held pad and a passing ball\ncreated an outcome neither partner did alone.\n\nTry stepping back after an invitation.\nWatch how the creature chooses to help."};
  if(index>=4)return new CreationBookPage("Field note · "+title,learned?notes[activity]:"No outing observation recorded yet.\n\nComplete this experiment during Play Day\nto add a field note. Earlier play is not\nretroactively attributed to an outing.",index,7);
  bool current=(progress&bit)!=0;return new CreationBookPage(title,(current?"Recorded this outing":active?"Not yet recorded this outing":"Ready when you are")+"\n\n"+instructions[activity],index,7);
 }
}
}
