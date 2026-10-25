using UnityEngine;
using UnityEngine.Rendering;
namespace BWVR {
// Small world-space hand, with no colliders or input ownership.
public sealed class PickupHand:MonoBehaviour {
 Transform[] fingers=new Transform[4];Material material;
 public static PickupHand Create(Transform parent,int side){var root=new GameObject(side==0?"Left pickup hand":"Right pickup hand");root.transform.SetParent(parent,false);var hand=root.AddComponent<PickupHand>();hand.Build(side);root.SetActive(false);return hand;}
 void Build(int side){
  material=new Material(Shader.Find("BWVR/GameSurface")){color=new Color(.95f,.83f,.54f)};
  Part("Palm",new Vector3(0,.08f,0),new Vector3(.7f,.22f,.8f));
  for(int i=0;i<4;i++)fingers[i]=Part("Finger "+i,new Vector3((i-1.5f)*.19f,.09f,.55f),new Vector3(.16f,.18f,i==0||i==3?.65f:.8f));
  Part("Thumb",new Vector3(side==0?.45f:-.45f,.08f,.08f),new Vector3(.36f,.20f,.22f));
 }
 Transform Part(string label,Vector3 at,Vector3 size){var g=GameObject.CreatePrimitive(PrimitiveType.Sphere);g.name=label;g.transform.SetParent(transform,false);g.transform.localPosition=at;g.transform.localScale=size;var collider=g.GetComponent<Collider>();collider.enabled=false;Destroy(collider);var renderer=g.GetComponent<Renderer>();renderer.sharedMaterial=material;renderer.shadowCastingMode=ShadowCastingMode.Off;renderer.receiveShadows=false;return g.transform;}
 public void Show(bool visible,Vector3 point,Vector3 normal,Vector3 forward,float viewScale,bool carrying,bool valid,bool grabbable){
  gameObject.SetActive(visible);if(!visible)return;
  var tangent=Vector3.ProjectOnPlane(forward,normal);if(tangent.sqrMagnitude<.001f)tangent=Vector3.ProjectOnPlane(Vector3.forward,normal);if(tangent.sqrMagnitude<.001f)tangent=Vector3.ProjectOnPlane(Vector3.right,normal);
  transform.SetPositionAndRotation(point+normal*.012f*viewScale,Quaternion.LookRotation(tangent,normal));transform.localScale=Vector3.one*.035f*viewScale;
  material.color=!valid?new Color(1,.32f,.22f):carrying?new Color(.45f,.9f,1):grabbable?new Color(1,.92f,.5f):new Color(.86f,.8f,.65f);
  foreach(var finger in fingers){var p=finger.localPosition;p.y=carrying?.24f:.09f;p.z=carrying?.28f:.55f;finger.localPosition=p;finger.localRotation=Quaternion.Euler(carrying?65:0,0,0);}
 }
 void OnDestroy(){if(material!=null)Destroy(material);}
}
}
