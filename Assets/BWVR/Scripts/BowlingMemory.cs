using System;
using UnityEngine;
namespace BWVR {
[Serializable] public sealed class BowlingMemory {
 public int playerRolls,creatureRolls,playerPins,creaturePins,observedRolls,bestPlayer,bestCreature;public float preference=.5f,practice;
 public bool Valid()=>Count(playerRolls)&&Count(creatureRolls)&&Count(playerPins)&&Count(creaturePins)&&Count(observedRolls)&&observedRolls<=playerRolls&&observedRolls<=playerPins&&playerPins<=(long)playerRolls*6&&creaturePins<=(long)creatureRolls*6&&bestPlayer<=playerPins&&bestCreature<=creaturePins&&bestPlayer>=0&&bestPlayer<=6&&bestCreature>=0&&bestCreature<=6&&HybridState.Number(preference,0,1)&&HybridState.Number(practice,0,1);
 static bool Count(int n)=>n>=0&&n<=1000000;
 public void Complete(bool creature,int pins,bool witnessed){pins=Mathf.Clamp(pins,0,6);if(creature){creatureRolls=Math.Min(1000000,creatureRolls+1);creaturePins=Math.Min(1000000,creaturePins+pins);bestCreature=Math.Max(bestCreature,pins);if(pins>0)practice=Mathf.Clamp01(practice+.04f);}else{playerRolls=Math.Min(1000000,playerRolls+1);playerPins=Math.Min(1000000,playerPins+pins);bestPlayer=Math.Max(bestPlayer,pins);if(witnessed&&pins>0)observedRolls=Math.Min(playerRolls,observedRolls+1);}}
}
}
