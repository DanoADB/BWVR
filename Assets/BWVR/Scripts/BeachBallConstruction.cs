using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace BWVR {
// Technical colliders only: rendered pieces keep their sourced artwork and own physics scene.
public sealed class BallBlockProxy:MonoBehaviour {
 public ConstructionBlock Block{get;private set;}public Rigidbody Body{get;private set;}
 readonly BoxCollider[] boxes=new BoxCollider[3];MeshCollider mesh;
 public void Initialize(){Body=gameObject.AddComponent<Rigidbody>();Body.isKinematic=true;for(int i=0;i<boxes.Length;i++){boxes[i]=gameObject.AddComponent<BoxCollider>();boxes[i].contactOffset=.005f;boxes[i].enabled=false;}mesh=gameObject.AddComponent<MeshCollider>();mesh.convex=true;mesh.enabled=false;}
 public void Bind(ConstructionBlock block){if(Block==block)return;Block=block;foreach(var box in boxes)box.enabled=false;mesh.enabled=false;mesh.sharedMesh=null;if(block==null)return;
  var source=block.Body.GetComponents<BoxCollider>();for(int i=0;i<source.Length&&i<boxes.Length;i++){boxes[i].center=source[i].center;boxes[i].size=source[i].size;boxes[i].enabled=true;}
  var shape=block.Body.GetComponent<MeshCollider>();if(shape!=null){mesh.sharedMesh=shape.sharedMesh;mesh.enabled=true;}
 }
}
public sealed partial class BeachBall {
 const int BlockContactLimit=16;
 ConstructionBlock[] constructionBlocks;readonly BallBlockProxy[] blockContacts=new BallBlockProxy[BlockContactLimit];readonly ConstructionBlock[] nearestBlocks=new ConstructionBlock[BlockContactLimit];readonly float[] blockDistances=new float[BlockContactLimit];float blockRefresh;
 void InitializeBlockContacts(){
  constructionBlocks=nav.GetComponentsInChildren<ConstructionSite>().Where(s=>!s.IsBridge).SelectMany(s=>s.blocks).Take(144).ToArray();
  for(int i=0;i<blockContacts.Length;i++){var go=new GameObject("Pooled ball/block contact "+i);SceneManager.MoveGameObjectToScene(go,scene);var proxy=go.AddComponent<BallBlockProxy>();proxy.Initialize();blockContacts[i]=proxy;go.SetActive(false);}
 }
 bool BlockContactAvailable(ConstructionBlock b)=>b!=null&&b.isActiveAndEnabled&&b.Site!=null&&b.Site.gameObject.activeInHierarchy&&!b.Site.Paused&&!b.Site.Locked&&!b.Item.held&&b.VillagerOwner==null&&!b.Body.isKinematic;
 void SyncBlockContacts(float dt){
  if(item.held){foreach(var proxy in blockContacts)if(proxy!=null)proxy.gameObject.SetActive(false);blockRefresh=0;return;}
  blockRefresh-=dt;if(blockRefresh<=0){blockRefresh=.25f;
   for(int i=0;i<BlockContactLimit;i++){nearestBlocks[i]=null;blockDistances[i]=80*80;}
   foreach(var block in constructionBlocks){if(!BlockContactAvailable(block))continue;float distance=(nav.ToNav(block.Site.World(block.Body.position))-Body.position).sqrMagnitude;for(int i=0;i<BlockContactLimit;i++)if(distance<blockDistances[i]){for(int n=BlockContactLimit-1;n>i;n--){blockDistances[n]=blockDistances[n-1];nearestBlocks[n]=nearestBlocks[n-1];}blockDistances[i]=distance;nearestBlocks[i]=block;break;}}
   for(int i=0;i<BlockContactLimit;i++)blockContacts[i].Bind(nearestBlocks[i]);
  }
  foreach(var proxy in blockContacts){var b=proxy.Block;bool active=BlockContactAvailable(b);proxy.gameObject.SetActive(active);if(active){proxy.Body.position=nav.ToNav(b.Site.World(b.Body.position));proxy.Body.rotation=b.Site.transform.rotation*b.Body.rotation;}}
 }
 public void ConstructionContact(Collision collision){
  if(creature==null||creature.Season.paused||item.held||collision.contactCount==0)return;var proxy=collision.collider.GetComponent<BallBlockProxy>();var block=proxy!=null?proxy.Block:null;if(!BlockContactAvailable(block))return;
  float speed=collision.relativeVelocity.magnitude;if(speed<.7f||collision.impulse.sqrMagnitude<.0001f)return;
  // Apply the opposite of the impulse on the ball, converted to the yard's coordinates.
  var impulse=Quaternion.Inverse(block.Site.transform.rotation)*-collision.impulse;
  if(speed>=9&&block.Attached){BuildBowlContact(block);block.Site.Detach(block);if(creatureThrow&&thrown)knockedThisThrow=true;}
  impulse=Vector3.ClampMagnitude(impulse,speed>=9?1.5f:.35f);var point=block.Site.Local(nav.ToWorld(collision.GetContact(0).point));block.Body.AddForceAtPosition(impulse,point,ForceMode.Impulse);block.Body.WakeUp();
 }
}
}
