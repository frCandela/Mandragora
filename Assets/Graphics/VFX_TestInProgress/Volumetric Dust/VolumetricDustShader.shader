Shader "Unlit/VolumetricDustShader"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_FresnelIntensity ("Fresnel Intensity", Range(0,1)) = 0
		_LightDistFactor ("Light Distance Attenuation", float) = 1
		_DepthAttenFactor ("Depth Attenuation Factor", float) = 1
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

			float4 _Color;
			float _FresnelIntensity, _LightDistFactor, _DepthAttenFactor;
			
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
				float depthAtten = 1 / length(toCam);
				depthAtten *= _DepthAttenFactor;
				depthAtten = saturate(depthAtten);

				// Fresnel
				float fresnel = dot(toCamNorm, i.normal);
				fresnel = (fresnel - _FresnelIntensity) / (1 - _FresnelIntensity);
				fresnel = saturate(fresnel);

				// Lighting
				float3 lightColor = i.lighting;

				// Apply
				fixed4 col = fixed4(i.color.rgb * lightColor * fresnel, i.color.a * fresnel);
				float3 depthColor = (col.r + col.g + col.b)/3;
				col.rgb = lerp(depthColor, col.rgb, depthAtten);
				col.a *= depthAtten;
				return col;
			}
			ENDCG
		}

		
	}
	Fallback "VertexLit"
}
