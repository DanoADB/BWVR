Shader "BWVR/VillagePath" {
 SubShader { Tags {"Queue"="AlphaTest+20" "RenderType"="Transparent"} Blend SrcAlpha OneMinusSrcAlpha ZWrite Off Cull Off
 Pass { Tags {"LightMode"="ForwardBase"}
 CGPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #pragma multi_compile_fog
 #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
 #include "UnityCG.cginc"
 #include "Lighting.cginc"
 #include "AutoLight.cginc"
 struct appdata {float4 vertex:POSITION;float3 normal:NORMAL;float2 uv:TEXCOORD0;UNITY_VERTEX_INPUT_INSTANCE_ID};
 struct v2f {float4 pos:SV_POSITION;float2 uv:TEXCOORD0;float3 normal:TEXCOORD1;float3 world:TEXCOORD2;UNITY_FOG_COORDS(3) SHADOW_COORDS(4) UNITY_VERTEX_OUTPUT_STEREO};
 v2f vert(appdata v){v2f o;UNITY_SETUP_INSTANCE_ID(v);UNITY_INITIALIZE_OUTPUT(v2f,o);UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);o.pos=UnityObjectToClipPos(v.vertex);o.uv=v.uv;o.normal=UnityObjectToWorldNormal(v.normal);o.world=mul(unity_ObjectToWorld,v.vertex).xyz;UNITY_TRANSFER_FOG(o,o.pos);TRANSFER_SHADOW(o);return o;}
 fixed4 frag(v2f i):SV_Target{UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
  float grain=frac(sin(dot(floor(i.world.xz*12500),float2(12.9898,78.233)))*43758.5453);
  float edge=min(i.uv.x,1-i.uv.x);float alpha=smoothstep(.015,.24,edge+sin(i.uv.y*2.1)*.035+grain*.035);
  float worn=smoothstep(.05,.4,edge);float3 color=lerp(float3(.12,.075,.035),float3(.24,.16,.08),worn)*(.85+grain*.3);
  float3 n=normalize(i.normal);color*=max(.3,ShadeSH9(float4(n,1)))+_LightColor0.rgb*SHADOW_ATTENUATION(i)*(.6*max(0,dot(n,normalize(_WorldSpaceLightPos0.xyz))));
  fixed4 result=fixed4(color,alpha);UNITY_APPLY_FOG(i.fogCoord,result);return result;
 }
 ENDCG
 } }
}
