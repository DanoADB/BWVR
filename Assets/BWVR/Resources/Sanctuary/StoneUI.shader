Shader "BWVR/SanctuaryStoneUI" {
 Properties { [PerRendererData] _MainTex("UI texture",2D)="white"{} _Color("Tint",Color)=(1,1,1,1) }
 SubShader { Tags {"Queue"="Overlay" "RenderType"="Transparent" "IgnoreProjector"="True"} Cull Off ZWrite Off ZTest Always Blend SrcAlpha OneMinusSrcAlpha
 Pass { CGPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #pragma multi_compile_instancing
 #include "UnityCG.cginc"
 struct appdata {float4 vertex:POSITION;float2 uv:TEXCOORD0;fixed4 color:COLOR;UNITY_VERTEX_INPUT_INSTANCE_ID};
 struct v2f {float4 pos:SV_POSITION;float2 uv:TEXCOORD0;fixed4 color:COLOR;UNITY_VERTEX_OUTPUT_STEREO};
 v2f vert(appdata v){v2f o;UNITY_SETUP_INSTANCE_ID(v);UNITY_INITIALIZE_OUTPUT(v2f,o);UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);o.pos=UnityObjectToClipPos(v.vertex);o.uv=v.uv;o.color=v.color;return o;}
 float hash(float2 p){return frac(sin(dot(p,float2(127.1,311.7)))*43758.5453);}
 float grain(float2 p){float2 i=floor(p),f=frac(p);f=f*f*(3-2*f);return lerp(lerp(hash(i),hash(i+float2(1,0)),f.x),lerp(hash(i+float2(0,1)),hash(i+1),f.x),f.y);}
 fixed4 frag(v2f i):SV_Target {UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);float2 p=(i.uv-.5)*2;float r=length(p);float aa=max(fwidth(r),.002);float edge=1-smoothstep(.97-aa,.97+aa,r);float bevel=smoothstep(.80,.96,r);float light=.94+bevel*(dot(normalize(p+float2(.001,.001)),float2(-.6,.8))*.23-.26);float noise=grain(i.uv*14)*.06+grain(i.uv*37)*.035;fixed3 stone=(fixed3(.18,.205,.19)+noise)*light;float ring=1-smoothstep(.003,.009,abs(r-.81));stone=lerp(stone,fixed3(.69,.73,.59),ring*.7);float shadow=(1-smoothstep(.95,1.02,length(p-float2(.025,-.035))))*.23;return fixed4(lerp(fixed3(.09,.065,.04),stone,edge)*i.color.rgb,max(edge,shadow)*i.color.a);}
 ENDCG }
 }
}
