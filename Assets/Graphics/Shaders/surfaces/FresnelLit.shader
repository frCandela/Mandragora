Shader "Mandragora/FresnelLit"
{
	Properties
	{
		_Color ("Color", Color) = (0.1438679, 0.1438679, 0.5754716, 0.0)
		_Luminosity ("Luminosity", float) = 1
		_Opacity ("Opacity", Range(0,1)) = 0.8
		_FresnelPow ("Fresnel Power", float) = 2
		_FresnelIntensity ("Fresnel Intensity", float) = 1
		_FresnelOpacity ("Fresnel Opacity", Range(0,1)) = 0.5
		_ReflexionIntensity ("Reflexion Intensity", Range(0,1)) = 0.04
		_ReflexionPower ("Reflexion Power", float) = 20
		_SunLightWorldPos ("Sun Light World Position", Vector) = (0, 1.8, 0, 0)
		_SunLightColor ("Sun Light Color", Color) = (0.4811321, 0.2182329, 0.01588645, 1)
	}
	SubShader
	{
		//Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" "LightMode" = "ForwardAdd" }
		Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
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
				float3 worldVertex : TEXCOORD2;
			};

			float _FresnelPow, _FresnelIntensity, _FresnelOpacity, _Luminosity, _Opacity;
			float _ReflexionIntensity, _ReflexionPower;
			fixed4 _Color;
			float4 _SunLightWorldPos, _SunLightColor;
			float _ManagerUnlitFactor;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldVertex = mul(unity_ObjectToWorld, v.vertex).xyz;
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

				// Inverse Normal for VFACE
				//float3 invertedNormal = -i.normal;
				//facing = step(1, facing);
				//i.normal = lerp(invertedNormal, i.normal, facing);

				// Process light direction
				float3 pointLightDir = normalize(i.worldVertex - _SunLightWorldPos.xyz);
				float3 lightDir = pointLightDir;

				// Process Reflection with this Light
				float3 H = normalize(-lightDir + toCam);
				float NdotH = dot(i.normal, H);
				NdotH = saturate(NdotH);
				NdotH = pow(NdotH, _ReflexionPower);
				float3 lightReflexion = NdotH * _SunLightColor.rgb * _ReflexionIntensity;
				float alphaLightReflexion = (lightReflexion.r + lightReflexion.g + lightReflexion.b)/3;
			//	lightReflexion *= facing;


				// Calculate Fresnel
				float3 viewDir = normalize(-toCam);
				float fresnel = dot(viewDir, -i.normal) * 0.5 + 0.5;
				fresnel = 1 - fresnel;
				fresnel = pow(saturate(fresnel), _FresnelPow);
				fresnel *= _FresnelIntensity;
				fresnel = fresnel * _FresnelOpacity;


				// Apply
				fixed4 col = fixed4(0,0,0,0);
				col.rgb = _Color.rgb * fresnel;
				col.rgb += lightReflexion;
				col.rgb *= _Luminosity;

				col.a = saturate(fresnel * _Color.a + alphaLightReflexion) * _Opacity;

				col *= _ManagerUnlitFactor;

				// DEBUG
				//col.rgb = _LightColor0.rgb;
				//col.a = 1;
				
				return col;
			}
			ENDCG
		}
	}
}