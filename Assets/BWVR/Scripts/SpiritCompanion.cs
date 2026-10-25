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
 Transform anchor,head,rig,figure,orb;GameObject visualRoot;
 Func<SliceState> season;Func<CreatureAffect.Mood> feeling;
 UnityEngine.UI.Text caption;Canvas card;Mesh sphere;Material orbMaterial;SpiritEffects effects;
 public int ParticleCount=>effects?.Count??0;
 public float EffectTime=>effects?.Time??0;
 public Color OrbColor=>orbMaterial==null?Color.white:orbMaterial.color;
 public Vector3 OrbScale=>orb==null?Vector3.one:orb.localScale;
 float phase,refreshIn;
 public static SpiritCompanion Build(Transform owner,Transform hand,Camera camera,Transform origin,Func<SliceState> current,Func<CreatureAffect.Mood> mood,Font font){
  var go=new GameObject("Private spirit companion");go.transform.SetParent(owner,false);
  var spirit=go.AddComponent<SpiritCompanion>();spirit.Initialize(hand,camera.transform,origin,current,mood,font);return spirit;
 }
 void Initialize(Transform hand,Transform camera,Transform origin,Func<SliceState> current,Func<CreatureAffect.Mood> mood,Font font){
  anchor=hand;head=camera;rig=origin;season=current;feeling=mood;
  // Ignore Raycast is additional protection; absence from simulation registries is primary.
  gameObject.layer=2;sphere=MakeSphere();
  var shader=Resources.Load<Shader>("SpiritOrb");
  if(shader==null)throw new InvalidOperationException("Spirit unlit shader unavailable");
  orbMaterial=new Material(shader){name="Private spirit living glow",color=new Color(.2f,.85f,.75f)};
  visualRoot=new GameObject("Player-only spirit presentation");visualRoot.layer=2;visualRoot.transform.SetParent(transform,false);
  figure=new GameObject("Floating seed").transform;figure.gameObject.layer=2;figure.SetParent(visualRoot.transform,false);
  orb=Part("Living light orb",figure);effects=new SpiritEffects(figure);
  var page=new GameObject("Local spirit observations",typeof(RectTransform),typeof(Canvas));page.layer=2;page.transform.SetParent(visualRoot.transform,false);
  card=page.GetComponent<Canvas>();card.renderMode=RenderMode.WorldSpace;
  var rect=page.GetComponent<RectTransform>();rect.sizeDelta=new Vector2(560,210);rect.localScale=Vector3.one*.008f;rect.localPosition=new Vector3(0,-2.5f,0);
  var background=new GameObject("Readout background",typeof(RectTransform),typeof(UnityEngine.UI.Image));background.layer=2;background.transform.SetParent(rect,false);
  background.GetComponent<RectTransform>().sizeDelta=rect.sizeDelta;var image=background.GetComponent<UnityEngine.UI.Image>();image.color=new Color(.025f,.075f,.085f,.96f);image.raycastTarget=false;
  var text=new GameObject("Measured world state",typeof(RectTransform),typeof(UnityEngine.UI.Text));text.layer=2;text.transform.SetParent(rect,false);
  caption=text.GetComponent<UnityEngine.UI.Text>();caption.font=font;caption.fontSize=22;caption.color=new Color(1,.94f,.78f);caption.alignment=TextAnchor.MiddleLeft;caption.raycastTarget=false;caption.supportRichText=false;
  caption.rectTransform.sizeDelta=new Vector2(520,190);card.gameObject.SetActive(false);Place();
 }
 Transform Part(string title,Transform parent){
  var go=new GameObject(title,typeof(MeshFilter),typeof(MeshRenderer));go.layer=2;go.transform.SetParent(parent,false);
  go.GetComponent<MeshFilter>().sharedMesh=sphere;var renderer=go.GetComponent<MeshRenderer>();renderer.sharedMaterial=orbMaterial;renderer.shadowCastingMode=ShadowCastingMode.Off;renderer.receiveShadows=false;return go.transform;
 }
 static Mesh MakeSphere(){
  const int columns=32,rows=20;var vertices=new Vector3[(columns+1)*(rows+1)];var triangles=new int[columns*rows*6];int cursor=0;
  for(int y=0;y<=rows;y++)for(int x=0;x<=columns;x++){float latitude=Mathf.PI*y/rows,longitude=2*Mathf.PI*x/columns;vertices[y*(columns+1)+x]=new Vector3(Mathf.Sin(latitude)*Mathf.Cos(longitude),Mathf.Cos(latitude),Mathf.Sin(latitude)*Mathf.Sin(longitude));}
  for(int y=0;y<rows;y++)for(int x=0;x<columns;x++){int a=y*(columns+1)+x,b=a+columns+1;triangles[cursor++]=a;triangles[cursor++]=a+1;triangles[cursor++]=b;triangles[cursor++]=a+1;triangles[cursor++]=b+1;triangles[cursor++]=b;}
  var mesh=new Mesh{name="Shared spirit ellipsoid"};mesh.vertices=vertices;mesh.triangles=triangles;mesh.RecalculateNormals();mesh.bounds=new Bounds(Vector3.zero,Vector3.one*2.25f);return mesh;
 }
 public SpiritObservation Inspect()=>SpiritObservation.Capture(season?.Invoke(),feeling==null?CreatureAffect.Mood.Calm:feeling());
 public void SetVisible(bool visible){if(Visible!=visible)effects?.Reset();Visible=visible;if(visualRoot!=null)visualRoot.SetActive(visible);refreshIn=0;}
 public void SetDetails(bool details){Details=details;if(card!=null)card.gameObject.SetActive(details);refreshIn=0;}
 void Place(){
  if(head==null||rig==null)return;float scale=rig.lossyScale.x;transform.localScale=Vector3.one*(.035f*scale);
  // A left-hand perch, positioned in world space without changing the tracking rig.
  transform.position=anchor!=null&&HandTracked?anchor.position+(Vector3.up*.16f-head.right*.1f+head.forward*.04f)*scale:head.position+(head.forward*.65f-head.right*.3f-Vector3.up*.18f)*scale;
  var direction=transform.position-head.position;if(direction.sqrMagnitude>.000001f)transform.rotation=Quaternion.LookRotation(direction,head.up);
 }
 void LateUpdate(){
  if(!Visible||head==null)return;Place();var state=season?.Invoke();bool stopped=Suspended||state==null||state.paused;
  Animate(Time.deltaTime,stopped);
  if(!Details)return;refreshIn-=Time.unscaledDeltaTime;if(refreshIn>0)return;refreshIn=.5f;Observation=Inspect();caption.text=Observation==null?"SPIRIT · AI DISCONNECTED\nNo world observed":Observation.Summary();
 }
 // Local affect is reflected as ambience, not a fabricated connected-agent response.
 public void Animate(float dt,bool stopped){
  if(!Visible||effects==null||stopped||dt<=0||float.IsNaN(dt)||float.IsInfinity(dt))return;dt=Mathf.Min(dt,.1f);phase=(phase+dt*.85f)%(Mathf.PI*10);
  var mood=feeling==null?CreatureAffect.Mood.Calm:feeling();
  Color tint=mood==CreatureAffect.Mood.Happy?new Color(.55f,1,.4f):mood==CreatureAffect.Mood.Sad?new Color(.3f,.48f,1):mood==CreatureAffect.Mood.Hungry?new Color(1,.65f,.2f):mood==CreatureAffect.Mood.Bored?new Color(.65f,.5f,.95f):mood==CreatureAffect.Mood.Excited?new Color(1,.4f,.7f):mood==CreatureAffect.Mood.Curious?new Color(.25f,.75f,1):new Color(.2f,.85f,.75f);
  tint=Color.Lerp(tint,new Color(.65f,1,.85f),.15f+.12f*Mathf.Sin(phase*.7f));orbMaterial.color=Color.Lerp(orbMaterial.color,tint,1-Mathf.Exp(-dt*2));orbMaterial.SetFloat("_Phase",phase);orbMaterial.SetFloat("_Morph",.5f+.25f*Mathf.Sin(phase*.6f));
  float pulse=1+Mathf.Sin(phase*2)*.055f,stretch=Mathf.Sin(phase)*.11f;orb.localScale=new Vector3(1-stretch*.5f,1+stretch,1-stretch*.5f)*pulse;
  figure.localPosition=Vector3.up*(Mathf.Sin(phase)*.1f);effects.Tick(dt,false,orbMaterial.color);
 }
 void OnDestroy(){if(sphere!=null)Destroy(sphere);if(orbMaterial!=null)Destroy(orbMaterial);effects?.Dispose();}
}
}
