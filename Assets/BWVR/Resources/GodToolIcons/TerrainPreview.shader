Shader "BWVR/TerrainPreview" {
 Properties { _Color("Preview tint",Color)=(.3,.9,1,.3) }
 SubShader { Tags {"Queue"="Transparent+20" "RenderType"="Transparent"}
  Pass { Blend SrcAlpha OneMinusSrcAlpha ZWrite Off ZTest Always Cull Off
   CGPROGRAM
   #pragma vertex vert
   #pragma fragment frag
   #pragma multi_compile_instancing
   #include "UnityCG.cginc"
   fixed4 _Color;
   struct appdata {float4 vertex:POSITION;float2 uv:TEXCOORD0;float3 normal:NORMAL; UNITY_VERTEX_INPUT_INSTANCE_ID};
   struct v2f {float4 vertex:SV_POSITION;float2 uv:TEXCOORD0;float3 normal:TEXCOORD1; UNITY_VERTEX_OUTPUT_STEREO};
   v2f vert(appdata v){v2f o;UNITY_SETUP_INSTANCE_ID(v);UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);o.vertex=UnityObjectToClipPos(v.vertex);o.uv=v.uv;o.normal=UnityObjectToWorldNormal(v.normal);return o;}
   fixed4 frag(v2f i):SV_Target{float radial=dot(i.uv,i.uv);clip(1-radial);fixed4 color=_Color;color.a*=saturate((1-radial)*3);color.rgb*=.5+.5*saturate(dot(normalize(i.normal),normalize(float3(.4,1,.3))));return color;}
   ENDCG
  }
 }
}
