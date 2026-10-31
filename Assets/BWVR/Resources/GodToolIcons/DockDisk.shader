Shader "BWVR/DockDisk" {
 Properties { [PerRendererData] _MainTex("UI texture",2D)="white"{} _Color("Tint",Color)=(1,1,1,1) }
 SubShader {Tags {"Queue"="Overlay" "RenderType"="Transparent"} Cull Off ZWrite Off ZTest Always Blend SrcAlpha OneMinusSrcAlpha
 Pass {CGPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #pragma multi_compile_instancing
 #include "UnityCG.cginc"
 struct appdata {float4 vertex:POSITION;float2 uv:TEXCOORD0;fixed4 color:COLOR;UNITY_VERTEX_INPUT_INSTANCE_ID};
 struct v2f {float4 vertex:SV_POSITION;float2 uv:TEXCOORD0;fixed4 color:COLOR;UNITY_VERTEX_OUTPUT_STEREO};
 v2f vert(appdata v){v2f o;UNITY_SETUP_INSTANCE_ID(v);UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);o.vertex=UnityObjectToClipPos(v.vertex);o.uv=v.uv;o.color=v.color;return o;}
 fixed4 frag(v2f i):SV_Target {UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);float2 p=(i.uv-.5)*2;float r=length(p);float aa=max(fwidth(r),.003);float edge=1-smoothstep(.96-aa,.96+aa,r);float bevel=smoothstep(.73,.94,r);float grain=frac(sin(dot(floor(i.uv*105),float2(127.1,311.7)))*43758.5453)*.035;fixed3 stone=(fixed3(.19,.215,.20)+grain)*(1-bevel*.26+bevel*p.y*.23);float rim=1-smoothstep(.012,.028,abs(r-.83));stone=lerp(stone,fixed3(.69,.65,.49),rim*.55);return fixed4(stone*i.color.rgb,edge*i.color.a);}
 ENDCG}
 }
}
