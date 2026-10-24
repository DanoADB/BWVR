Shader "BWVR/IslandGround" {
 Properties { _Grass("Quaternius grass palette",2D)="white" {} _Rock("Quaternius rock",2D)="white" {} _Sand("Quaternius sand",2D)="white" {} _SeaLevel("Sea level",Float)=0.75 }
 SubShader { Tags {"RenderType"="Opaque"} Pass { Tags {"LightMode"="ForwardBase"}
 CGPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #pragma multi_compile_instancing
 #pragma multi_compile_fog
 #include "UnityCG.cginc"
 #include "Lighting.cginc"
 sampler2D _Grass,_Rock,_Sand;float _SeaLevel;
 struct input {float4 vertex:POSITION;float3 normal:NORMAL;UNITY_VERTEX_INPUT_INSTANCE_ID};
 struct output {float4 pos:SV_POSITION;float3 world:TEXCOORD0;float3 normal:TEXCOORD1;UNITY_FOG_COORDS(2) UNITY_VERTEX_OUTPUT_STEREO};
 output vert(input v){output o;UNITY_SETUP_INSTANCE_ID(v);UNITY_INITIALIZE_OUTPUT(output,o);UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);o.pos=UnityObjectToClipPos(v.vertex);o.world=mul(unity_ObjectToWorld,v.vertex).xyz;o.normal=UnityObjectToWorldNormal(v.normal);UNITY_TRANSFER_FOG(o,o.pos);return o;}
 fixed4 frag(output i):SV_Target{UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);float3 n=normalize(i.normal);float noise=sin(i.world.x*9)*sin(i.world.z*7)*.5+.5;
  // Sample green palette columns, never the white unused part of the grass atlas.
  float3 grass=lerp(tex2D(_Grass,float2(.075,.75)).rgb,tex2D(_Grass,float2(.19,.75)).rgb,noise*.65+.2)*float3(.78,.87,.75);
  float3 weights=pow(abs(n),4);weights/=max(.0001,weights.x+weights.y+weights.z);
  float3 rock=tex2D(_Rock,i.world.yz*9).rgb*weights.x+tex2D(_Rock,i.world.xz*9).rgb*weights.y+tex2D(_Rock,i.world.xy*9).rgb*weights.z;
  float3 sand=tex2D(_Sand,i.world.xz*7).rgb*float3(1,.95,.83);
  float beach=1-smoothstep(.001,.016,i.world.y-_SeaLevel);float steep=smoothstep(.12,.48,1-n.y);float3 color=lerp(lerp(grass,rock,steep),sand,beach);
  color*=max(.3,ShadeSH9(float4(n,1)))+_LightColor0.rgb*(.6*max(0,dot(n,normalize(_WorldSpaceLightPos0.xyz))));fixed4 result=fixed4(color,1);UNITY_APPLY_FOG(i.fogCoord,result);return result;
 }
 ENDCG
 } }
}
