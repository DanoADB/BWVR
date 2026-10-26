using UnityEngine;
namespace BWVR {
// Read-only nonverbal feedback: one shared atlas, no raycaster or input ownership.
public sealed class VillagerAffect:MonoBehaviour {
 public enum Mood {None,Learning,Love,Hungry,Angry,Afraid,Hurt}
 public Mood Displayed{get;private set;}
 static Texture2D atlas;static Sprite[] sprites;static int users;bool initialized;
 VillageResources world;VillagerSocial social;Canvas canvas;UnityEngine.UI.Image image;float height,size,learningUntil,angerUntil,loveUntil;int disputes,kindness;
 public void Initialize(VillageResources resources){world=resources;social=GetComponent<VillagerSocial>();disputes=social.Memory.disputes;kindness=Kindness();
  float body=transform.lossyScale.y;var animator=GetComponentInChildren<Animator>();if(animator!=null){var renderers=animator.GetComponentsInChildren<Renderer>();if(renderers.Length>0){var bounds=renderers[0].bounds;foreach(var r in renderers)bounds.Encapsulate(r.bounds);body=Mathf.Max(body,bounds.max.y-transform.position.y);}}
  height=body+world.Navigation.Scale*1.2f;size=body*.48f;
  Acquire();initialized=true;var go=new GameObject("Villager state icon",typeof(RectTransform),typeof(Canvas));go.transform.SetParent(transform,false);canvas=go.GetComponent<Canvas>();canvas.renderMode=RenderMode.WorldSpace;canvas.sortingOrder=2;var rect=go.GetComponent<RectTransform>();rect.sizeDelta=new Vector2(64,64);rect.localScale=Vector3.one*(size/64/transform.lossyScale.x);
  var symbol=new GameObject("Nonverbal villager state",typeof(RectTransform),typeof(UnityEngine.UI.Image));symbol.transform.SetParent(rect,false);image=symbol.GetComponent<UnityEngine.UI.Image>();image.raycastTarget=false;image.rectTransform.sizeDelta=new Vector2(64,64);canvas.enabled=false;
 }
 int Kindness()=>social.Memory.greetings+social.Memory.sharedPlay+social.Memory.rides+social.Memory.comfortReceived;
 public void Learned(){if(world!=null&&!world.Season.paused)learningUntil=world.Season.Hybrid.time+4;}
 public static Mood Select(bool hurt,bool afraid,bool angry,bool hungry,bool learning,bool love)=>hurt?Mood.Hurt:afraid?Mood.Afraid:angry?Mood.Angry:hungry?Mood.Hungry:learning?Mood.Learning:love?Mood.Love:Mood.None;
 void LateUpdate(){if(world==null||canvas==null)return;float now=world.Season.Hybrid.time;if(social.Memory.disputes>disputes)angerUntil=now+4;disputes=social.Memory.disputes;int k=Kindness();if(k>kindness)loveUntil=now+4;kindness=k;
  var mood=Select(social.Memory.injury>.05f,social.RecentFright,now<angerUntil,world.FoodSeconds<90,now<learningUntil,now<loveUntil);
  // Needs recur briefly; event acknowledgments remain visible for four simulation seconds.
  bool eventState=mood==Mood.Learning||mood==Mood.Love||mood==Mood.Angry;bool visible=mood!=Mood.None&&(eventState||now%14<4);
  Displayed=visible?mood:Mood.None;canvas.enabled=visible;if(visible)image.sprite=sprites[(int)mood-1];canvas.transform.position=transform.position+Vector3.up*height;var camera=Camera.main;if(camera!=null)canvas.transform.rotation=Quaternion.LookRotation(canvas.transform.position-camera.transform.position);
 }
 static void Acquire(){users++;if(atlas!=null)return;atlas=new Texture2D(384,64,TextureFormat.RGBA32,false){name="Shared villager state symbols",filterMode=FilterMode.Bilinear,wrapMode=TextureWrapMode.Clamp};sprites=new Sprite[6];var pixels=new Color[384*64];
  for(int m=0;m<6;m++){int offset=m*64;var tint=new[]{new Color(1,.86f,.3f),new Color(1,.45f,.6f),new Color(1,.7f,.2f),new Color(1,.3f,.2f),new Color(.65f,.7f,1),new Color(1,.55f,.65f)}[m];
   void Dot(int x,int y,int radius,Color color){for(int py=Mathf.Max(0,y-radius);py<Mathf.Min(64,y+radius+1);py++)for(int px=Mathf.Max(0,x-radius);px<Mathf.Min(64,x+radius+1);px++)if((px-x)*(px-x)+(py-y)*(py-y)<=radius*radius)pixels[py*384+offset+px]=color;}
   void Line(int x,int y,int ex,int ey,int width=2){int n=Mathf.Max(Mathf.Abs(ex-x),Mathf.Abs(ey-y));for(int i=0;i<=n;i++)Dot(Mathf.RoundToInt(Mathf.Lerp(x,ex,i/(float)Mathf.Max(1,n))),Mathf.RoundToInt(Mathf.Lerp(y,ey,i/(float)Mathf.Max(1,n))),width,tint);}
   Dot(32,32,30,new Color(.025f,.055f,.065f,.94f));
   if(m==0){Dot(32,39,13,tint);Line(26,23,38,23,3);Line(28,16,36,16);Line(32,57,32,61,1);Line(10,40,14,40,1);Line(50,40,54,40,1);Line(15,55,18,52,1);Line(46,52,49,55,1);}
   if(m==1){for(int y=8;y<56;y++)for(int x=8;x<57;x++){float a=(x-32)/17f,b=(y-29)/17f,f=a*a+b*b-1;if(f*f*f-a*a*b*b*b<=0)Dot(x,y,0,tint);}}
   if(m==2){Dot(26,31,12,tint);Dot(39,31,12,tint);Line(18,27,29,16,4);Line(46,27,35,16,4);Line(32,44,36,52);Line(36,51,43,51);}
   if(m==3){Line(17,45,27,39);Line(37,39,47,45);Dot(23,33,3,tint);Dot(41,33,3,tint);Line(22,18,29,23);Line(29,23,36,23);Line(36,23,43,18);}
   if(m==4){Line(20,44,25,48);Line(39,48,44,44);Dot(23,36,4,tint);Dot(41,36,4,tint);Dot(32,20,6,tint);}
   if(m==5){Line(32,14,32,50,6);Line(14,32,50,32,6);}
   sprites[m]=Sprite.Create(atlas,new Rect(offset,0,64,64),Vector2.one*.5f,64);
  }atlas.SetPixels(pixels);atlas.Apply(false,true);
 }
 void OnDisable(){if(canvas!=null)canvas.enabled=false;}
 void OnDestroy(){if(!initialized)return;if(--users>0)return;if(sprites!=null)foreach(var sprite in sprites)Destroy(sprite);Destroy(atlas);sprites=null;atlas=null;}
}
}
