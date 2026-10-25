Shader "BWVR/SpiritMotes" {
 Properties {_Star("Star rays",Range(0,1))=1}
 SubShader {Tags {"Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True"}
 Blend SrcAlpha One ZWrite Off Cull Off
 Pass {
 CGPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #pragma multi_compile_instancing
 #include "UnityCG.cginc"
 float _Star;
 struct appdata {float4 vertex:POSITION;float2 uv:TEXCOORD0;fixed4 color:COLOR;UNITY_VERTEX_INPUT_INSTANCE_ID};
 struct v2f {float4 pos:SV_POSITION;float2 uv:TEXCOORD0;fixed4 color:COLOR;UNITY_VERTEX_OUTPUT_STEREO};
 v2f vert(appdata v){v2f o;UNITY_SETUP_INSTANCE_ID(v);UNITY_INITIALIZE_OUTPUT(v2f,o);UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);o.pos=UnityObjectToClipPos(v.vertex);o.uv=v.uv;o.color=v.color;return o;}
 fixed4 frag(v2f i):SV_Target{UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);float2 p=i.uv*2-1;
  float glow=pow(saturate(1-dot(p,p)),3);
  float rays=pow(saturate(1-abs(p.x)*10),2)*pow(saturate(1-abs(p.y)),2)+pow(saturate(1-abs(p.y)*10),2)*pow(saturate(1-abs(p.x)),2);
  return fixed4(i.color.rgb,saturate(glow*.5+_Star*rays*.65)*i.color.a);}
 ENDCG
 }}
}
