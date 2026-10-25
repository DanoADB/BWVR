Shader "BWVR/SpiritOrb" {
 Properties {_Color("Spirit tint",Color)=(.2,.85,.75,1) _Phase("Animation phase",Float)=0 _Morph("Shape softness",Range(0,1))=.5}
 SubShader {Tags {"RenderType"="Opaque"} Pass {
 CGPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #pragma multi_compile_instancing
 #include "UnityCG.cginc"
 fixed4 _Color;float _Phase,_Morph;
 struct appdata {float4 vertex:POSITION;float3 normal:NORMAL;UNITY_VERTEX_INPUT_INSTANCE_ID};
 struct v2f {float4 pos:SV_POSITION;float3 normal:TEXCOORD0;float3 world:TEXCOORD1;UNITY_VERTEX_OUTPUT_STEREO};
 v2f vert(appdata v){v2f o;UNITY_SETUP_INSTANCE_ID(v);UNITY_INITIALIZE_OUTPUT(v2f,o);UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  float wave=sin(v.vertex.y*3+_Phase)*cos(v.vertex.x*2-_Phase*.6);v.vertex.xyz*=1+wave*.10*_Morph;
  o.pos=UnityObjectToClipPos(v.vertex);o.world=mul(unity_ObjectToWorld,v.vertex).xyz;o.normal=UnityObjectToWorldNormal(v.normal);return o;}
 fixed4 frag(v2f i):SV_Target{UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
  float facing=saturate(dot(normalize(i.normal),normalize(_WorldSpaceCameraPos-i.world)));
  float rim=pow(1-facing,2);float core=pow(facing,5);
  return fixed4(_Color.rgb*(.65+.45*rim)+fixed3(.55,.65,.7)*core,1);}
 ENDCG
 }}
}
