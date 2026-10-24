using UnityEngine;
namespace BWVR {public sealed class BeachBallCollision:MonoBehaviour {public BeachBall world;void OnCollisionEnter(Collision collision){world?.Impact(collision.collider.GetComponent<BallCollisionProxy>()?.target,collision.relativeVelocity.magnitude);}}}
