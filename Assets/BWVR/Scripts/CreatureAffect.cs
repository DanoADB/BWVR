using UnityEngine;
namespace BWVR {
// One non-interactive world-space icon, shared atlas, no text or per-frame textures.
public sealed class CreatureAffect:MonoBehaviour {
 public enum Mood {Calm,Curious,Happy,Sad,Hungry,Bored,Excited}
 public Mood Displayed{get;private set;}
 CreatureCuriosity mind;UnityEngine.UI.Image image;Canvas canvas;Texture2D atlas;Sprite[] sprites;float height,size,changed=-10;
 public void Initialize(CreatureCuriosity creature,float unit){
  mind=creature;height=unit*18;size=unit*5;
  var go=new GameObject("Creature feeling icon",typeof(RectTransform),typeof(Canvas));go.transform.SetParent(transform,false);canvas=go.GetComponent<Canvas>();canvas.renderMode=RenderMode.WorldSpace;canvas.sortingOrder=2;
  var rect=go.GetComponent<RectTransform>();rect.sizeDelta=new Vector2(64,64);rect.localScale=Vector3.one*(size/64);
  var symbol=new GameObject("Nonverbal affect",typeof(RectTransform),typeof(UnityEngine.UI.Image));symbol.transform.SetParent(rect,false);image=symbol.GetComponent<UnityEngine.UI.Image>();image.raycastTarget=false;image.rectTransform.sizeDelta=new Vector2(64,64);
  atlas=new Texture2D(256,128,TextureFormat.RGBA32,false){name="Creature feelings atlas",filterMode=FilterMode.Bilinear,wrapMode=TextureWrapMode.Clamp};sprites=new Sprite[7];var pixels=new Color[256*128];
  for(int mood=1;mood<=6;mood++){
   int ox=((mood-1)%4)*64,oy=((mood-1)/4)*64;var tint=new[]{Color.white,new Color(.45f,.8f,1),new Color(1,.48f,.5f),new Color(.47f,.65f,1),new Color(1,.65f,.22f),new Color(.76f,.69f,.91f),new Color(1,.87f,.3f)}[mood];
   void Dot(int x,int y,int radius,Color color){for(int py=Mathf.Max(0,y-radius);py<Mathf.Min(64,y+radius+1);py++)for(int px=Mathf.Max(0,x-radius);px<Mathf.Min(64,x+radius+1);px++)if((px-x)*(px-x)+(py-y)*(py-y)<=radius*radius)pixels[(oy+py)*256+ox+px]=color;}
   void Line(int x,int y,int ex,int ey,int width=2){int steps=Mathf.Max(Mathf.Abs(ex-x),Mathf.Abs(ey-y));for(int n=0;n<=steps;n++)Dot(Mathf.RoundToInt(Mathf.Lerp(x,ex,n/(float)Mathf.Max(1,steps))),Mathf.RoundToInt(Mathf.Lerp(y,ey,n/(float)Mathf.Max(1,steps))),width,tint);}
   Dot(32,32,30,new Color(.025f,.055f,.065f,.92f));
   if(mood==1){Line(22,44,26,49);Line(26,49,39,49);Line(39,49,44,44);Line(44,44,43,36);Line(43,36,32,30);Line(32,30,32,25);Dot(32,16,3,tint);}
   if(mood==2){for(int y=8;y<56;y++)for(int x=8;x<57;x++){float a=(x-32)/17f,b=(y-29)/17f;float f=a*a+b*b-1;if(f*f*f-a*a*b*b*b<=0)Dot(x,y,0,tint);}}
   if(mood==3){Dot(22,39,3,tint);Dot(42,39,3,tint);Line(23,20,28,24);Line(28,24,36,24);Line(36,24,41,20);Line(44,32,48,26);Dot(48,24,3,tint);}
   if(mood==4){Dot(25,30,12,tint);Dot(40,30,12,tint);Line(19,26,29,15,5);Line(45,26,35,15,5);Line(32,43,35,51);Line(36,49,43,50,3);}
   if(mood==5){Line(16,34,27,34);Line(37,34,48,34);Line(24,20,40,20);Line(38,48,49,48,1);Line(49,48,38,40,1);Line(38,40,49,40,1);}
   if(mood==6){Line(32,13,32,51,3);Line(13,32,51,32,3);Line(20,20,44,44,2);Line(20,44,44,20,2);Dot(32,32,9,tint);}
   sprites[mood]=Sprite.Create(atlas,new Rect(ox,oy,64,64),Vector2.one*.5f,64);
  }
  atlas.SetPixels(pixels);atlas.Apply(false,true);canvas.enabled=false;
 }
 void LateUpdate(){if(mind==null||canvas==null)return;var mood=mind.Affect;if(mood!=Displayed&&(Time.unscaledTime-changed>1||Displayed==Mood.Calm)){Displayed=mood;changed=Time.unscaledTime;image.sprite=sprites[(int)mood];}
  canvas.enabled=Displayed!=Mood.Calm;canvas.transform.localPosition=new Vector3(0,height+Mathf.Sin(Time.time*2)*size*.04f,0);var camera=Camera.main;if(camera!=null)canvas.transform.rotation=Quaternion.LookRotation(canvas.transform.position-camera.transform.position);
 }
 void OnDestroy(){if(sprites!=null)foreach(var sprite in sprites)if(sprite!=null)Destroy(sprite);if(atlas!=null)Destroy(atlas);}
}
}
