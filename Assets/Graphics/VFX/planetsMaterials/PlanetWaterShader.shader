Shader "Mandragora/PlanetWaterShader"
{
	Properties
	{
		_FresnelColor ("Fresnel Color", Color) = (0.1438679, 0.1438679, 0.5754716, 0.0)
		_Luminosity ("Luminosity", float) = 1
		_Opacity ("Opacity", Range(0,1)) = 0.8
		_FresnelPow ("Fresnel Power", float) = 2
		_FresnelIntensity ("Fresnel Intensity", float) = 1
		_FresnelOpacity ("Fresnel Opacity", Range(0,1)) = 0.5
		_ReflexionIntensity ("Reflexion Intensity", Range(0,1)) = 0.04
		_ReflexionPower ("Reflexion Power", float) = 20
		_BaseColor ("Base Color", Color) = (1,1,1,1)
		_BaseColorOpacity ("Base Color Opacity", Range(0,1)) = 1
		_NoiseSettings ("Noise Amplitude(X) Frequency(Y) Speed(Z)", Vector) = (1,1,1,0)
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" "LightMode" = "ForwardAdd" }
		//Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "Lighting.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float3 normal : NORMAL;
				float3 flatNormal : TEXCOORD3;
				float4 worldVertex : TEXCOORD2;
			};

			float _FresnelPow, _FresnelIntensity, _FresnelOpacity, _Luminosity, _Opacity, _BaseColorOpacity;
			float _ReflexionIntensity, _ReflexionPower;
			fixed4 _FresnelColor, _BaseColor;
			float3 _NoiseSettings;

			float4 ApplyNoise(float4 pos, float amp, float freq, float speed) {
				float3 newPos;
				newPos.x = cos(pos.y*freq + _Time.y*speed)*amp + sin(pos.z*freq + _Time.y*speed)*amp;
				newPos.y = cos(pos.z*freq + _Time.y*speed)*amp + sin(pos.x*freq + _Time.y*speed)*amp;
				newPos.z = cos(pos.x*freq + _Time.y*speed)*amp + sin(pos.y*freq + _Time.y*speed)*amp;
				return float4(newPos + pos.xyz, pos.w);
			}
			
			v2f vert (appdata v)
			{
				v2f o;
				float3 offsetedLocalPos = ApplyNoise(v.vertex, _NoiseSettings.x, _NoiseSettings.y, _NoiseSettings.z);
				o.worldVertex = mul(unity_ObjectToWorld, float4(offsetedLocalPos, v.vertex.w));
				o.vertex = mul(UNITY_MATRIX_VP, o.worldVertex);
				o.normal = UnityObjectToWorldNormal(v.normal);
				o.flatNormal = float3(0,0,0);
				return o;
			}

			void InitializeFragmentNormal(inout v2f v) {
				float3 dpdx = ddx(v.worldVertex);
				float3 dpdy = ddy(v.worldVertex);
				v.normal = normalize(cross(dpdy, dpdx));
	
			}
			

			fixed4 frag (v2f i) : SV_Target
			{
				// Flatten Normals
				InitializeFragmentNormal(i);

				// Get some variables
				float3 toCam = normalize(_WorldSpaceCameraPos.xyz - i.worldVertex);

				// Process light direction
				int lightID = _WorldSpaceLightPos0.w;
				float3 directionalLightDir = normalize(_WorldSpaceLightPos0.xyz);
				float3 pointLightDir = normalize(i.worldVertex - _WorldSpaceLightPos0.xyz);
				float3 lightDir = lerp(directionalLightDir, pointLightDir, lightID);

				// Process Reflection with this Light
				float3 H = normalize(-lightDir + toCam);
				float NdotH = dot(i.normal, H);
				NdotH = saturate(NdotH);
				NdotH = pow(NdotH, _ReflexionPower);
				float3 lightReflexion = NdotH * _LightColor0.rgb * _ReflexionIntensity;
				float alphaLightReflexion = (lightReflexion.r + lightReflexion.g + lightReflexion.b)/3;


				// Calculate Fresnel
				float3 viewDir = toCam;
				float fresnel = dot(viewDir, i.normal) * 0.5 + 0.5;
				fresnel = 1 - fresnel;
				fresnel = pow(saturate(fresnel), _FresnelPow);
				fresnel *= _FresnelIntensity;
				fresnel = fresnel * _FresnelOpacity;


				// Apply
				fixed4 col = fixed4(0,0,0,0);
				col.rgb = _FresnelColor.rgb * fresnel;
				col.rgb += lightReflexion;
				col.rgb *= _Luminosity;

				col.a = saturate(fresnel + alphaLightReflexion) * _Opacity;

				// Base Color Calculation - Apply
				float baseColAlpha = (1 - col.a) * _BaseColorOpacity;
				col.rgb *= lerp(float3(1,1,1), _BaseColor.rgb, baseColAlpha);
				col.a = saturate(col.a + baseColAlpha);
				
				return col;
			}
			ENDCG
		}
	}
}