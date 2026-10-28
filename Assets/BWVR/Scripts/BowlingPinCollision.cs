using UnityEngine;
namespace BWVR {
public sealed class BowlingPinCollision:MonoBehaviour {public BowlingLane lane;public Rigidbody ball;void OnCollisionEnter(Collision hit){if(hit.rigidbody==ball&&hit.relativeVelocity.magnitude>1)lane?.Contact();}}
}
