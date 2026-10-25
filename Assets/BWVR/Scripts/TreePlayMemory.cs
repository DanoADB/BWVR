using System;
using UnityEngine;
namespace BWVR {
[Serializable] public sealed class MovedTree {
 public int line;public Vector3 position;public Quaternion rotation=Quaternion.identity;
}
[Serializable] public sealed class TreePlayMemory {
 public MovedTree[] moved=Array.Empty<MovedTree>();public int throws,safeThrows,harmfulThrows;public float enjoyment=.25f,caution=.2f,preference=.5f;
 public MovedTree Find(int line){foreach(var tree in moved)if(tree.line==line)return tree;return null;}
 public bool Remember(int line,Vector3 position,Quaternion rotation){var entry=Find(line);if(entry==null){if(moved.Length>=32)return false;Array.Resize(ref moved,moved.Length+1);entry=new MovedTree{line=line};moved[moved.Length-1]=entry;}entry.position=position;entry.rotation=rotation;return true;}
 public bool Valid(){if(moved==null||moved.Length>32||throws<0||throws>1000000||safeThrows<0||safeThrows>throws||harmfulThrows<0||harmfulThrows>throws||safeThrows+harmfulThrows>throws||!HybridState.Number(enjoyment,0,1)||!HybridState.Number(caution,0,1)||!HybridState.Number(preference,0,1))return false;
  var ids=new System.Collections.Generic.HashSet<int>();foreach(var t in moved){if(t==null||t.line<0||!ids.Add(t.line)||!HybridState.Number(t.position.x,-100000,100000)||!HybridState.Number(t.position.y,-100000,100000)||!HybridState.Number(t.position.z,-100000,100000))return false;var q=t.rotation;float norm=q.x*q.x+q.y*q.y+q.z*q.z+q.w*q.w;if(!HybridState.Number(norm,.95f,1.05f))return false;}return true;
 }
}
}
