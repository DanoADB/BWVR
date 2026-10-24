Shader "BWVR/GameSurface" {
 Properties { _MainTex("Game texture",2D)="white" {} _Color("Tint",Color)=(1,1,1,1) _Cutoff("Alpha cutout",Range(0,1))=0.25 }
 SubShader { Tags {"RenderType"="TransparentCutout" "Queue"="AlphaTest"} Cull Off
 Pass { Tags {"LightMode"="ForwardBase"}
 CGPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #pragma multi_compile_instancing
 #pragma multi_compile_fog
 #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
 #include "UnityCG.cginc"
 #include "Lighting.cginc"
 #include "AutoLight.cginc"
 sampler2D _MainTex; float4 _MainTex_ST; fixed4 _Color; float _Cutoff;
 struct appdata { float4 vertex:POSITION; float3 normal:NORMAL; float2 uv:TEXCOORD0; UNITY_VERTEX_INPUT_INSTANCE_ID };
 struct v2f { float4 pos:SV_POSITION; float2 uv:TEXCOORD0; float3 normal:TEXCOORD1; UNITY_FOG_COORDS(2) SHADOW_COORDS(3) UNITY_VERTEX_OUTPUT_STEREO };
 v2f vert(appdata v){v2f o; UNITY_SETUP_INSTANCE_ID(v); UNITY_INITIALIZE_OUTPUT(v2f,o); UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);o.pos=UnityObjectToClipPos(v.vertex);o.uv=TRANSFORM_TEX(v.uv,_MainTex);o.normal=UnityObjectToWorldNormal(v.normal);UNITY_TRANSFER_FOG(o,o.pos);TRANSFER_SHADOW(o);return o;}
 fixed4 frag(v2f i):SV_Target {UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);fixed4 c=tex2D(_MainTex,i.uv)*_Color;clip(c.a-_Cutoff);float3 n=normalize(i.normal);c.rgb*=max(.3,ShadeSH9(float4(n,1)))+_LightColor0.rgb*SHADOW_ATTENUATION(i)*(.6*abs(dot(n,normalize(_WorldSpaceLightPos0.xyz))));c.a=1;UNITY_APPLY_FOG(i.fogCoord,c);return c;}
 ENDCG
 }
 }
}
