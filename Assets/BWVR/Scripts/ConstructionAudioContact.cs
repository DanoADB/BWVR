using UnityEngine;
namespace BWVR {
// Native game-unit physics proxy reports a real contact at the visual block's world position.
public sealed class ConstructionAudioContact:MonoBehaviour {
 public ConstructionBlock block;
 void OnCollisionEnter(Collision collision){if(block==null||block.Item.held||block.Site.Paused||block.Site.Clock<2)return;float speed=collision.relativeVelocity.magnitude;if(speed<1.4f)return;IslandAudio.Cue(IslandSound.Place,block.Site.World(block.Body.position),block.GetEntityId().GetHashCode(),Mathf.Clamp01(speed/12),.85f);}
}
}
