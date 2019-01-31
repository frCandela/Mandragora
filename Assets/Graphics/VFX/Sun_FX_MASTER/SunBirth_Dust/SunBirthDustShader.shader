Shader "Mandragora/SunBirthDustShader"
{
	Properties
	{
		_Color ("Color", Color) = (0.3, 0.3, 0.3, 1)
		_Luminosity ("Luminosity", float) = 1
		_FresnelIntensity ("Fresnel Intensity", Range(0,1)) = 0.21
		_LightDistFactor ("Light Distance Attenuation", float) = 6.42
		_DepthAttenFactor ("Depth Attenuation Factor", float) = 3.5
		_DepthColor ("Depth Color", Color) = (0,0,0,0)
		_MaxDepthFadeDistance ("Max Fade Distance", float) = 0
		_MinDepthFadeDistance ("Min Fade Distance", float) = 0
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
		LOD 100

		Blend SrcAlpha OneMinusSrcAlpha
		Zwrite Off
		Cull Off

		Pass
		{
			Tags { "LightMode" = "Vertex" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "Lighting.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float3 wVertex : TEXCOORD0;
				fixed4 color : COLOR;
			};

			struct v2f
			{
				float3 normal : NORMAL;
				float4 vertex : SV_POSITION;
				float3 wVertex : TEXCOORD0;
				fixed4 color : COLOR;
				float3 lighting : TEXCOORD1;
			};

			float4 _Color, _DepthColor;
			float _FresnelIntensity, _LightDistFactor, _DepthAttenFactor;
			float _MaxDepthFadeDistance, _MinDepthFadeDistance;
			float _Luminosity;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.normal = UnityObjectToWorldNormal(v.normal);
				o.wVertex = mul (unity_ObjectToWorld, v.vertex);
				o.color = v.color;
				float3 currentLight = float3(0,0,0);
				for(int l=0; l < 3; l++)
				{
					float addLightAtten = length(o.wVertex - unity_LightPosition[l].xyz) / unity_LightAtten[l].w;
					addLightAtten = 1 / (addLightAtten * _LightDistFactor);
					float3 addLightColor = unity_LightColor[l].rgb * addLightAtten;

					currentLight += addLightColor;
				}
				o.lighting = currentLight;
				return o;
			}
			
			fixed4 frag (v2f i, fixed facing : VFACE) : SV_Target
			{

				// Invert Normals on backface
				float3 invertedNormal = - i.normal;
				facing = step(1, facing);
				i.normal = lerp(invertedNormal, i.normal, facing);

				// Get to camera vector
				float3 toCam = _WorldSpaceCameraPos - i.wVertex;
				float3 toCamNorm = normalize(toCam);

				// Depth Attenuation
				float distFromCam = length(toCam);
				float depthAtten = 1 / distFromCam;
				depthAtten *= _DepthAttenFactor;
				depthAtten = saturate(depthAtten);

				// Depth Fade
				float depthFade = saturate(distFromCam - _MinDepthFadeDistance / _MaxDepthFadeDistance - _MinDepthFadeDistance);

				// Fresnel
				float fresnel = dot(toCamNorm, i.normal);
				fresnel = (fresnel - _FresnelIntensity) / (1 - _FresnelIntensity);
				fresnel = saturate(fresnel);

				// Lighting
				float3 lightColor = clamp(i.lighting, 0, 10);

				// Apply
				fixed4 col = fixed4(i.color.rgb * lightColor * fresnel, 1);
				col = lerp(_DepthColor, col, depthAtten);
				col.rgb = _Color.rgb;
				col.rgb *= _Luminosity;
				col.a = i.color.a * fresnel;
				col.a *= depthFade;
				return col;
			}
			ENDCG
		}

		
	}
	Fallback "VertexLit"
}
