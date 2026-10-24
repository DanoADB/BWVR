using System.Collections.Generic;
using UnityEngine;
namespace BWVR {
public sealed class DiscoveryPoint:MonoBehaviour {
 public readonly List<DiscoveryObject> objects=new List<DiscoveryObject>();public ConstructionSite construction;public int id{get;private set;}
 public DiscoveryRecord Record=>world.Memory(id);public IslandDiscoveries world{get;private set;}AudioSource sound;
 public void Initialize(IslandDiscoveries owner,int identity,float unit,AudioClip clip){world=owner;id=identity;sound=gameObject.AddComponent<AudioSource>();sound.clip=clip;sound.playOnAwake=false;sound.spatialBlend=1;sound.minDistance=unit*12;sound.maxDistance=unit*100;sound.volume=.3f;}
 public bool Unseen(int actor)=>actor>=0&&actor<4&&(Record.visitors&(1<<actor))==0;
 public bool Discover(int actor){if(!Unseen(actor))return false;Record.visitors|=1<<actor;sound.Play();return true;}
 public void Advance(float dt){Record.restock=Mathf.Max(0,Record.restock-dt);foreach(var o in objects)o.Refresh();}
 public void ResetPoint(){foreach(var o in objects){o.Owner=null;o.GetComponent<CarryItem>().held=false;o.transform.position=o.GetComponent<CarryItem>().home;o.Refresh();}}
}
}
