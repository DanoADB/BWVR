Shader "BWVR/LocalMist" {
 Properties {_MainTex("Kenney white puff",2D)="white"{} _Color("Tint",Color)=(.72,.84,.81,.14)}
 SubShader {Tags {"Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True"}
 Blend SrcAlpha OneMinusSrcAlpha ZWrite Off Cull Off
 Pass {
 CGPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #pragma multi_compile_instancing
 #include "UnityCG.cginc"
 sampler2D _MainTex;fixed4 _Color;
 struct appdata {float4 vertex:POSITION;float2 uv:TEXCOORD0;fixed4 color:COLOR;UNITY_VERTEX_INPUT_INSTANCE_ID};
 struct v2f {float4 pos:SV_POSITION;float2 uv:TEXCOORD0;float3 world:TEXCOORD1;fixed4 color:COLOR;UNITY_VERTEX_OUTPUT_STEREO};
 v2f vert(appdata v){v2f o;UNITY_SETUP_INSTANCE_ID(v);UNITY_INITIALIZE_OUTPUT(v2f,o);UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);o.pos=UnityObjectToClipPos(v.vertex);o.uv=v.uv;o.world=mul(unity_ObjectToWorld,v.vertex).xyz;o.color=v.color;return o;}
 fixed4 frag(v2f i):SV_Target{UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);fixed4 tex=tex2D(_MainTex,i.uv);float fade=saturate(distance(_WorldSpaceCameraPos,i.world)/.06);return fixed4(_Color.rgb*i.color.rgb,tex.a*_Color.a*i.color.a*fade);}
 ENDCG
 }}
}
