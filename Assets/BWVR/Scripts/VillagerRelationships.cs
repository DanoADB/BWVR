using System;
using UnityEngine;
namespace BWVR {
[Serializable]public sealed class VillagerBond {
 public string villager;public float affinity=.5f,confidence=.5f;public int good,bad;public float lastGood=-100,lastBad=-100;
}
public sealed partial class SocialMemory {
 public VillagerBond[] peers=Array.Empty<VillagerBond>();public float nextPeerPlayAt;public int peerGames;
 public VillagerBond Peer(string id,bool create=false){if(string.IsNullOrEmpty(id))return null;foreach(var p in peers)if(p.villager==id)return p;if(!create||peers.Length>=16)return null;var added=new VillagerBond{villager=id};Array.Resize(ref peers,peers.Length+1);peers[peers.Length-1]=added;return added;}
 public bool RememberPeer(string id,bool safe,float time){var p=Peer(id,true);if(p==null)return false;if(safe){if(p.good>0&&time-p.lastGood<15)return false;p.good=Math.Min(1000000,p.good+1);p.lastGood=time;p.affinity=Mathf.Clamp01(p.affinity+.06f);p.confidence=Mathf.Clamp01(p.confidence+.035f);}else{if(p.bad>0&&time-p.lastBad<1)return false;p.bad=Math.Min(1000000,p.bad+1);p.lastBad=time;p.affinity=Mathf.Clamp01(p.affinity-.08f);p.confidence=Mathf.Clamp01(p.confidence-.15f);}return true;}
 public bool PeerWilling(string id,float time){var p=Peer(id);return p==null||p.bad==0||time-p.lastBad>=90;}
 public float PeerAppeal(string id){var p=Peer(id);return p==null?.5f:p.affinity*.75f+p.confidence*.25f;}
 bool PeersValid(){if(peers==null||peers.Length>16||!HybridState.Number(nextPeerPlayAt,0,100000300)||peerGames<0||peerGames>1000000)return false;var ids=new System.Collections.Generic.HashSet<string>();foreach(var p in peers)if(p==null||string.IsNullOrEmpty(p.villager)||p.villager.Length>160||!ids.Add(p.villager)||!Unit(p.affinity)||!Unit(p.confidence)||!Count(p.good)||!Count(p.bad)||!HybridState.Number(p.lastGood,-100,100000000)||!HybridState.Number(p.lastBad,-100,100000000))return false;return true;}
}
}
