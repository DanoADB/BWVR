using System;
using UnityEngine;
using UnityEngine.Rendering;
namespace BWVR {
// Presentation only: no colliders, actor components, audio, network or world commands.
public sealed class SpiritCompanion:MonoBehaviour {
 public bool Visible{get;private set;}=true;
 public bool Details{get;private set;}
 public bool Suspended{get;set;}
 public bool HandTracked{get;set;}=true;
 public bool Connected=>false;
 public SpiritObservation Observation{get;private set;}
 public string Readout=>caption==null?"":caption.text;
 public Transform Perch=>transform;
 Transform anchor,head,rig,figure,finLeft,finRight;GameObject visualRoot;
 Func<SliceState> season;Func<CreatureAffect.Mood> feeling;
 UnityEngine.UI.Text caption;Canvas card;Mesh sphere;Material[] materials;
 float phase,refreshIn;
 public static SpiritCompanion Build(Transform owner,Transform hand,Camera camera,Transform origin,Func<SliceState> current,Func<CreatureAffect.Mood> mood,Font font){
  var go=new GameObject("Private spirit companion");go.transform.SetParent(owner,false);
  var spirit=go.AddComponent<SpiritCompanion>();spirit.Initialize(hand,camera.transform,origin,current,mood,font);return spirit;
 }
 void Initialize(Transform hand,Transform camera,Transform origin,Func<SliceState> current,Func<CreatureAffect.Mood> mood,Font font){
  anchor=hand;head=camera;rig=origin;season=current;feeling=mood;
  // Ignore Raycast is additional protection; absence from simulation registries is primary.
  gameObject.layer=2;sphere=MakeSphere();
  var shader=Shader.Find("Universal Render Pipeline/Unlit")??Shader.Find("Sprites/Default");
  if(shader==null)throw new InvalidOperationException("Spirit unlit shader unavailable");
  materials=new[]{Tint(shader,new Color(.12f,.56f,.51f)),Tint(shader,new Color(1,.91f,.68f)),Tint(shader,new Color(.025f,.09f,.105f))};
  visualRoot=new GameObject("Player-only spirit presentation");visualRoot.layer=2;visualRoot.transform.SetParent(transform,false);
  figure=new GameObject("Floating seed").transform;figure.gameObject.layer=2;figure.SetParent(visualRoot.transform,false);
  Part("Seed body",Vector3.zero,new Vector3(.74f,1,.55f),0,figure);
  Part("Ivory face",new Vector3(0,.12f,-.46f),new Vector3(.56f,.6f,.16f),1,figure);
  Part("Left eye",new Vector3(-.18f,.19f,-.61f),new Vector3(.055f,.09f,.035f),2,figure);
  Part("Right eye",new Vector3(.18f,.19f,-.61f),new Vector3(.055f,.09f,.035f),2,figure);
  Part("Smile",new Vector3(0,-.08f,-.615f),new Vector3(.09f,.025f,.025f),2,figure);
  finLeft=Part("Left leaf",new Vector3(-.85f,.03f,.05f),new Vector3(.48f,.15f,.18f),0,figure);
  finRight=Part("Right leaf",new Vector3(.85f,.03f,.05f),new Vector3(.48f,.15f,.18f),0,figure);
  Part("Warm crown",new Vector3(0,1.02f,0),new Vector3(.17f,.25f,.16f),1,figure);
  var page=new GameObject("Local spirit observations",typeof(RectTransform),typeof(Canvas));page.layer=2;page.transform.SetParent(visualRoot.transform,false);
  card=page.GetComponent<Canvas>();card.renderMode=RenderMode.WorldSpace;
  var rect=page.GetComponent<RectTransform>();rect.sizeDelta=new Vector2(560,210);rect.localScale=Vector3.one*.008f;rect.localPosition=new Vector3(0,-2.5f,0);
  var background=new GameObject("Readout background",typeof(RectTransform),typeof(UnityEngine.UI.Image));background.layer=2;background.transform.SetParent(rect,false);
  background.GetComponent<RectTransform>().sizeDelta=rect.sizeDelta;var image=background.GetComponent<UnityEngine.UI.Image>();image.color=new Color(.025f,.075f,.085f,.96f);image.raycastTarget=false;
  var text=new GameObject("Measured world state",typeof(RectTransform),typeof(UnityEngine.UI.Text));text.layer=2;text.transform.SetParent(rect,false);
  caption=text.GetComponent<UnityEngine.UI.Text>();caption.font=font;caption.fontSize=22;caption.color=new Color(1,.94f,.78f);caption.alignment=TextAnchor.MiddleLeft;caption.raycastTarget=false;caption.supportRichText=false;
  caption.rectTransform.sizeDelta=new Vector2(520,190);card.gameObject.SetActive(false);Place();
 }
 Material Tint(Shader shader,Color color){var m=new Material(shader){name="Private spirit palette",color=color};return m;}
 Transform Part(string title,Vector3 position,Vector3 scale,int palette,Transform parent){
  var go=new GameObject(title,typeof(MeshFilter),typeof(MeshRenderer));go.layer=2;go.transform.SetParent(parent,false);go.transform.localPosition=position;go.transform.localScale=scale;
  go.GetComponent<MeshFilter>().sharedMesh=sphere;var renderer=go.GetComponent<MeshRenderer>();renderer.sharedMaterial=materials[palette];renderer.shadowCastingMode=ShadowCastingMode.Off;renderer.receiveShadows=false;return go.transform;
 }
 static Mesh MakeSphere(){
  const int columns=16,rows=10;var vertices=new Vector3[(columns+1)*(rows+1)];var triangles=new int[columns*rows*6];int cursor=0;
  for(int y=0;y<=rows;y++)for(int x=0;x<=columns;x++){float latitude=Mathf.PI*y/rows,longitude=2*Mathf.PI*x/columns;vertices[y*(columns+1)+x]=new Vector3(Mathf.Sin(latitude)*Mathf.Cos(longitude),Mathf.Cos(latitude),Mathf.Sin(latitude)*Mathf.Sin(longitude));}
  for(int y=0;y<rows;y++)for(int x=0;x<columns;x++){int a=y*(columns+1)+x,b=a+columns+1;triangles[cursor++]=a;triangles[cursor++]=a+1;triangles[cursor++]=b;triangles[cursor++]=a+1;triangles[cursor++]=b+1;triangles[cursor++]=b;}
  var mesh=new Mesh{name="Shared spirit ellipsoid"};mesh.vertices=vertices;mesh.triangles=triangles;mesh.RecalculateNormals();mesh.RecalculateBounds();return mesh;
 }
 public SpiritObservation Inspect()=>SpiritObservation.Capture(season?.Invoke(),feeling==null?CreatureAffect.Mood.Calm:feeling());
 public void SetVisible(bool visible){Visible=visible;if(visualRoot!=null)visualRoot.SetActive(visible);refreshIn=0;}
 public void SetDetails(bool details){Details=details;if(card!=null)card.gameObject.SetActive(details);refreshIn=0;}
 void Place(){
  if(head==null||rig==null)return;float scale=rig.lossyScale.x;transform.localScale=Vector3.one*(.035f*scale);
  // A left-hand perch, positioned in world space without changing the tracking rig.
  transform.position=anchor!=null&&HandTracked?anchor.position+(Vector3.up*.16f-head.right*.1f+head.forward*.04f)*scale:head.position+(head.forward*.65f-head.right*.3f-Vector3.up*.18f)*scale;
  var direction=transform.position-head.position;if(direction.sqrMagnitude>.000001f)transform.rotation=Quaternion.LookRotation(direction,head.up);
 }
 void LateUpdate(){
  if(!Visible||head==null)return;Place();var state=season?.Invoke();bool stopped=Suspended||state==null||state.paused;
  if(!stopped)phase=(phase+Time.deltaTime*2)% (Mathf.PI*2);
  figure.localPosition=Vector3.up*(Mathf.Sin(phase)*.08f);finLeft.localRotation=Quaternion.Euler(0,0,-20+Mathf.Sin(phase)*8);finRight.localRotation=Quaternion.Euler(0,0,20-Mathf.Sin(phase)*8);
  if(!Details)return;refreshIn-=Time.unscaledDeltaTime;if(refreshIn>0)return;refreshIn=.5f;Observation=Inspect();caption.text=Observation==null?"SPIRIT · AI DISCONNECTED\nNo world observed":Observation.Summary();
 }
 void OnDestroy(){if(sphere!=null)Destroy(sphere);if(materials!=null)foreach(var material in materials)if(material!=null)Destroy(material);}
}
}
