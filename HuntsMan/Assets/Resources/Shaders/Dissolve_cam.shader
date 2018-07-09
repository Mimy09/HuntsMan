Shader "Custom/Dissolve_Cam"
{
	Properties{
		_MainTex("Albedo", 2D) = "white" {}
		_Metallic("Metallic", 2D) = "white" {}
		_MetallicFactor("Metallic Strength", Range(0,2)) = 1
		_Roughness("Roughness", 2D) = "white" {}
		_RoughnessFactor("Roughness Strength", Range(0,2)) = 1
		//_NormalMap("NormalMap", 2D) = "bump" {}
		//_NormFactor("Normal Strength", Range(0,2)) = 1

		_SliceGuide("Slice Guide (RGB)", 2D) = "white" {}
		_BurnSize("Burn Size", Range(0.0, 1.0)) = 0.1
		_BurnRamp("Burn Ramp (RGB)", 2D) = "white" {}
		_Scale("Scale", float) = 0.25
		_DissolveRate("Disolve Rate", float) = 10.0
		_DissolveDistance("Disolve Distance", Range(0.0, 10.0)) = 0.2
	}
	SubShader{
		Tags{ "RenderType" = "Opaque" }
		Cull Off
		CGPROGRAM
		#include "UnityPBSLighting.cginc"
		#pragma surface surf Standard addshadow
	
		struct Input {
			float2 uv_MainTex;
			float2 uv_Metallic;
			float2 uv_Roughness;
			//float2 uv_NormalMap;

			float2 uv_SliceGuide;
			float3 worldPos;
			float3 worldNormal; INTERNAL_DATA
		};


		sampler2D _MainTex;
		sampler2D _Metallic;
		//sampler2D _NormalMap;
		sampler2D _Roughness;
		//half _NormFactor;
		half _MetallicFactor;
		half _RoughnessFactor;

		sampler2D _SliceGuide;
		sampler2D _BurnRamp;
		float _BurnSize;
		half _Scale;

		float _DissolveRate;
		float _DissolveDistance;

		void surf(Input IN, inout SurfaceOutputStandard o) {
			float2 UV;
			fixed4 c;

			if(abs(IN.worldNormal.x)>0.5) {
				UV = IN.worldPos.zy; // side
				c = tex2D(_MainTex,UV);
			} else if(abs(IN.worldNormal.z)>0.5) {
				UV = IN.worldPos.xy; // front
				c = tex2D(_MainTex,UV);
			} else {
				UV = IN.worldPos.xz; // top
				c = tex2D(_MainTex,UV);
			}
			UV.x *= _Scale;
			UV.y *= _Scale;

			float d = distance(IN.worldPos,_WorldSpaceCameraPos) / _DissolveRate - _DissolveDistance;
			d = 1 - clamp(d, 0.0, 1.0);

			clip(tex2D(_SliceGuide,UV).rgb - d);
			o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
			o.Metallic = tex2D(_Metallic,IN.uv_Metallic).rgb * _MetallicFactor;
			o.Smoothness = tex2D(_Roughness,IN.uv_Roughness).rgb * _RoughnessFactor;

			//o.Normal = UnpackScaleNormal(tex2D(_NormalMap,IN.uv_NormalMap),_NormFactor);

			half test = tex2D(_SliceGuide, UV).rgb - d;
			if(test < _BurnSize && d > 0 && d < 1) {
				o.Emission = tex2D(_BurnRamp, float2(test *(1 / _BurnSize), 0));
				o.Albedo *= o.Emission;
			}
		}
		ENDCG
	}
	Fallback "Diffuse"
}