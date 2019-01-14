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
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
		LOD 100
		Cull Off

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
			

			fixed4 frag (v2f i, fixed facing : VFACE) : SV_Target
			{
				// Flatten Normals
				InitializeFragmentNormal(i);

				// Get some variables
				float3 toCam = normalize(_WorldSpaceCameraPos.xyz - i.worldVertex);

				// Inverse Normal for VFACE
				float3 invertedNormal = -i.normal;
				facing = step(1, facing);
				i.normal = lerp(invertedNormal, i.normal, facing);

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
				lightReflexion *= facing;


				// Calculate Fresnel that multiply with Flow Texture
				float fresnel = dot(toCam, i.normal);
				fresnel = 1 - fresnel;
				fresnel = pow(saturate(fresnel), _FresnelPow);
				fresnel *= _FresnelIntensity;
				fresnel = saturate(fresnel) * _FresnelOpacity;


				// Apply
				fixed4 col = fixed4(0,0,0,0);
				col.rgb = _Color.rgb;
				col.rgb += lightReflexion;
				col.rgb *= _Luminosity;

				col.a = saturate(fresnel + _Color.a + alphaLightReflexion) * _Opacity;
				
				return col;
			}
			ENDCG
		}
	}
}