Shader "BWVR/IslandWater" {
 Properties {_Shore("Shore distance",2D)="white"{} _ShoreRect("Shore coordinates",Vector)=(0,0,1,1) _WeatherDriven("Weather clock enabled",Float)=0 _WeatherTime("Simulation time",Float)=0 _Surge("Wave front,height,width",Vector)=(0,0,1,0) _TimeOffset("Wave time offset",Float)=0 _WaveScale("Wave displacement scale",Float)=1}
 SubShader {Tags {"RenderType"="Opaque" "Queue"="Geometry"} Pass {Tags {"LightMode"="ForwardBase"}
 CGPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #pragma multi_compile_instancing
 #pragma multi_compile_fog
 #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
 #include "UnityCG.cginc"
 #include "Lighting.cginc"
 #include "AutoLight.cginc"
 sampler2D _Shore;float4 _ShoreRect;float _TimeOffset,_WaveScale,_WeatherDriven,_WeatherTime;float4 _Surge;
 struct input {float4 vertex:POSITION;UNITY_VERTEX_INPUT_INSTANCE_ID};
 struct output {float4 pos:SV_POSITION;float3 world:TEXCOORD0;float3 normal:TEXCOORD1;UNITY_FOG_COORDS(2) SHADOW_COORDS(3) UNITY_VERTEX_OUTPUT_STEREO};
 output vert(input v){
  output o;UNITY_SETUP_INSTANCE_ID(v);UNITY_INITIALIZE_OUTPUT(output,o);UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  float t=lerp(_Time.y,_WeatherTime,_WeatherDriven)+_TimeOffset;float3 world=mul(unity_ObjectToWorld,v.vertex).xyz;
  float a=dot(world.xz,float2(8,4))-t*.8;float b=dot(world.xz,float2(-5,11))+t*1.1;
  v.vertex.y+=(sin(a)*.0025+sin(b)*.0015)*_WaveScale;
  float waveDistance=(world.x-_Surge.x)/max(.0001,_Surge.z);
  v.vertex.y+=exp(-waveDistance*waveDistance)*_Surge.y;
  o.pos=UnityObjectToClipPos(v.vertex);o.world=mul(unity_ObjectToWorld,v.vertex).xyz;
  float surgeSlope=-2*waveDistance*exp(-waveDistance*waveDistance)*_Surge.y/max(.0001,_Surge.z);
  o.normal=normalize(float3((-cos(a)*.02+cos(b)*.0075)*_WaveScale-surgeSlope,1,(-cos(a)*.01-cos(b)*.0165)*_WaveScale));
  UNITY_TRANSFER_FOG(o,o.pos);TRANSFER_SHADOW(o);return o;
 }
 fixed4 frag(output i):SV_Target{
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);float t=lerp(_Time.y,_WeatherTime,_WeatherDriven)+_TimeOffset;
  float2 uv=(i.world.xz-_ShoreRect.xy)*_ShoreRect.zw;
  float4 shore=tex2D(_Shore,uv);float inside=step(0,uv.x)*step(0,uv.y)*step(uv.x,1)*step(uv.y,1);
  shore=lerp(float4(1,0,1,1),shore,inside);
  float pixelSpan=max(length(ddx(i.world.xz)),length(ddy(i.world.xz)));float detailFade=1-smoothstep(.002,.02,pixelSpan);
  float3 n=normalize(i.normal+detailFade*float3(sin(dot(i.world.xz,float2(73,49))+t*1.4)*.035,0,cos(dot(i.world.xz,float2(-37,91))-t)*.035));
  float3 view=normalize(_WorldSpaceCameraPos-i.world);
  float fresnel=pow(1-saturate(dot(n,view)),4);
  float3 color=lerp(float3(.10,.43,.43),float3(.035,.18,.29),shore.r);
  color=lerp(color,float3(.48,.70,.77),fresnel*.65);
  float shadow=SHADOW_ATTENUATION(i);
  float spec=pow(saturate(dot(n,normalize(view+normalize(_WorldSpaceLightPos0.xyz)))),90);
  color*=.72+.28*shadow;color+=_LightColor0.rgb*spec*.65*shadow*lerp(.25,1,detailFade);
  float foam=shore.g*(.35+.65*smoothstep(.1,.8,sin(dot(i.world.xz,float2(100,64))-t*1.8)*.5+.5));
  float crestDistance=(i.world.x-_Surge.x)/max(.0001,_Surge.z);
  float stormFoam=smoothstep(.55,.95,exp(-crestDistance*crestDistance))*step(.0001,_Surge.y);
  foam=saturate(foam+stormFoam*.85);
  color=lerp(color,float3(.72,.86,.80),foam*.65);
  fixed4 result=fixed4(color,1);UNITY_APPLY_FOG(i.fogCoord,result);return result;
 }
 ENDCG
 }}
}
