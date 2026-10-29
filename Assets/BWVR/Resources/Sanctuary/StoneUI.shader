Shader "BWVR/SanctuaryStoneUI" {
 Properties { [PerRendererData] _MainTex("UI texture",2D)="white"{} _Color("Tint",Color)=(1,1,1,1) }
 SubShader { Tags {"Queue"="Overlay" "RenderType"="Transparent" "IgnoreProjector"="True"} Cull Off ZWrite Off ZTest Always Blend SrcAlpha OneMinusSrcAlpha
 Pass { CGPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #pragma multi_compile_instancing
 #include "UnityCG.cginc"
 struct appdata {float4 vertex:POSITION;float2 uv:TEXCOORD0;fixed4 color:COLOR;UNITY_VERTEX_INPUT_INSTANCE_ID};
 struct v2f {float4 pos:SV_POSITION;float2 uv:TEXCOORD0;fixed4 color:COLOR;float3 world:TEXCOORD1;UNITY_VERTEX_OUTPUT_STEREO};
 v2f vert(appdata v){v2f o;UNITY_SETUP_INSTANCE_ID(v);UNITY_INITIALIZE_OUTPUT(v2f,o);UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);o.pos=UnityObjectToClipPos(v.vertex);o.uv=v.uv;o.color=v.color;o.world=mul(unity_ObjectToWorld,v.vertex).xyz;return o;}
 float hash(float2 p){return frac(sin(dot(p,float2(127.1,311.7)))*43758.5453);}
 float grain(float2 p){float2 i=floor(p),f=frac(p);f=f*f*(3-2*f);return lerp(lerp(hash(i),hash(i+float2(1,0)),f.x),lerp(hash(i+float2(0,1)),hash(i+1),f.x),f.y);}
 float roundedBox(float2 p,float2 halfSize,float radius){float2 q=abs(p)-(halfSize-radius);return length(max(q,0))+min(max(q.x,q.y),0)-radius;}
 fixed4 frag(v2f i):SV_Target {
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
  // Recover the rectangle's world aspect from UV derivatives, including tilted VR panels.
  float2 ux=ddx(i.uv),uy=ddy(i.uv);float3 wx=ddx(i.world),wy=ddy(i.world);
  float aspect=clamp(length(wx*uy.y-wy*ux.y)/max(length(wy*ux.x-wx*uy.x),1e-20),.3,12);
  float2 p=(i.uv-.5)*2*float2(aspect,1),halfSize=float2(aspect*.97,.93);
  float d=roundedBox(p,halfSize,.16);float aa=max(fwidth(d),.001);float edge=1-smoothstep(-aa,aa,d);
  float bevel=smoothstep(-.13,-.015,d);float light=.98-bevel*.17+bevel*(p.y>0?.09:-.07);
  float noise=grain(i.uv*14)*.045+grain(i.uv*37)*.025;fixed3 stone=(fixed3(.18,.205,.19)+noise)*light;
  float border=1-smoothstep(.006,.018,abs(d+.065));stone=lerp(stone,fixed3(.53,.58,.49),border*.45);
  float shadow=(1-smoothstep(-aa,aa,roundedBox(p-float2(.025,-.035),halfSize,.16)))*.2;
  return fixed4(lerp(fixed3(.09,.065,.04),stone,edge)*i.color.rgb,max(edge,shadow)*i.color.a);
 }
 ENDCG }
 }
}
