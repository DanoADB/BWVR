Shader "BWVR/IslandGround" {
 Properties { _Grass("Quaternius grass palette",2D)="white" {} _Rock("Quaternius rock",2D)="white" {} _Sand("Quaternius sand",2D)="white" {} _Detail("Quaternius ground detail",2D)="white" {} _Biome("Forest, paths and fields",2D)="black" {} _BiomeRect("Biome coordinates",Vector)=(0,0,1,1) _SeaLevel("Sea level",Float)=0.75 }
 SubShader { Tags {"RenderType"="Opaque"} Pass { Tags {"LightMode"="ForwardBase"}
 CGPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #pragma multi_compile_instancing
 #pragma multi_compile_fog
 #include "UnityCG.cginc"
 #include "Lighting.cginc"
 sampler2D _Grass,_Rock,_Sand,_Detail,_Biome;float4 _BiomeRect;float _SeaLevel;
 float hash(float2 p){return frac(sin(dot(p,float2(127.1,311.7)))*43758.5453);}
 float noise2(float2 p){float2 a=floor(p),f=frac(p);f=f*f*(3-2*f);return lerp(lerp(hash(a),hash(a+float2(1,0)),f.x),lerp(hash(a+float2(0,1)),hash(a+1),f.x),f.y);}
 struct input {float4 vertex:POSITION;float3 normal:NORMAL;UNITY_VERTEX_INPUT_INSTANCE_ID};
 struct output {float4 pos:SV_POSITION;float3 world:TEXCOORD0;float3 normal:TEXCOORD1;UNITY_FOG_COORDS(2) UNITY_VERTEX_OUTPUT_STEREO};
 output vert(input v){output o;UNITY_SETUP_INSTANCE_ID(v);UNITY_INITIALIZE_OUTPUT(output,o);UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);o.pos=UnityObjectToClipPos(v.vertex);o.world=mul(unity_ObjectToWorld,v.vertex).xyz;o.normal=UnityObjectToWorldNormal(v.normal);UNITY_TRANSFER_FOG(o,o.pos);return o;}
 fixed4 frag(output i):SV_Target{UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);float3 n=normalize(i.normal);float broad=noise2(i.world.xz*3.3),patch=noise2(i.world.xz*19),grain=noise2(i.world.xz*145);
  float4 biome=tex2D(_Biome,(i.world.xz-_BiomeRect.xy)*_BiomeRect.zw);
  float3 palette=lerp(float3(.065,.16,.028),float3(.17,.25,.065),broad);
  float dry=smoothstep(.52,.8,noise2(i.world.xz*7+17))*(1-biome.g*.75);
  float3 grass=lerp(palette,float3(.33,.24,.085),dry*.65)*( .87+patch*.2+grain*.12);
  grass=lerp(grass,grass*float3(.65,.83,.74),biome.g*.65);
  float3 detail=tex2D(_Detail,i.world.xz*85).rgb;
  float3 soil=float3(.16,.09,.035)*(.7+detail.r*.65);
  grass=lerp(grass,soil,saturate(biome.r*.82));
  float furrow=smoothstep(.25,.65,sin(i.world.x*2200)*.5+.5);
  float3 crops=lerp(float3(.10,.075,.02),float3(.26,.21,.065),furrow);
  grass=lerp(grass,crops,biome.b*.8);
  float3 weights=pow(abs(n),4);weights/=max(.0001,weights.x+weights.y+weights.z);
  float3 rock=tex2D(_Rock,i.world.yz*9).rgb*weights.x+tex2D(_Rock,i.world.xz*9).rgb*weights.y+tex2D(_Rock,i.world.xy*9).rgb*weights.z;
  float3 sand=tex2D(_Sand,i.world.xz*7).rgb*float3(1,.95,.83);
  float height=i.world.y-_SeaLevel;
  float beach=1-smoothstep(.001,.010+patch*.005,height);float steep=smoothstep(.12,.48,1-n.y);
  rock=lerp(rock*float3(.8,.86,.82),grass*.72,(1-steep)*patch*.4);
  sand*=lerp(float3(.52,.60,.58),float3(.92,.89,.73),smoothstep(.001,.007,height));
  float3 color=lerp(lerp(grass,rock,steep),sand,beach);
  color*=max(.3,ShadeSH9(float4(n,1)))+_LightColor0.rgb*(.6*max(0,dot(n,normalize(_WorldSpaceLightPos0.xyz))));fixed4 result=fixed4(color,1);UNITY_APPLY_FOG(i.fogCoord,result);return result;
 }
 ENDCG
 } }
}
