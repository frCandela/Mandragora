Shader "Mandragora/WorkshopCustom"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_WireColor ("Wireframe Color", Color) = (1,1,1,1)
		_WireframeWidth ("Wireframe Width", float) = 0.05
		_ReflexionIntensity ("Reflexion Intensity", Range(0,1)) = 0.1
		_ReflexionPower ("Reflexion Power", float) = 2
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Cull Off

		Pass
		{

			CGPROGRAM

			#pragma target 4.0
			#pragma vertex vert
			#pragma fragment frag
			#pragma geometry geo
			
			#include "UnityCG.cginc"
			#include "Lighting.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
			};

			struct InterpolatorsVertex {
                float4 vertex : SV_POSITION;
				float4 color : COLOR;
				float4 screenPos : TEXCOORD2;
				float3 worldPos : TEXCOORD0;
             };

			// Uses Geometry Shader
			struct InterpolatorsGeometry {
				InterpolatorsVertex data;
				float2 barycentricCoordinates : TEXCOORD3;
				float3 flatNormal : TEXCOORD1;
			};

			float4 _Color, _WireColor;
			float _WireframeWidth;
			float _ReflexionIntensity, _ReflexionPower;
			
			InterpolatorsVertex vert (appdata v)
			{
				InterpolatorsVertex o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.screenPos = ComputeScreenPos(o.vertex);
				return o;
			}

			//////////// GEOMETRY SHADER /////////////////////
			[maxvertexcount(3)]
			void geo (triangle InterpolatorsVertex i[3], inout TriangleStream<InterpolatorsGeometry> stream)
			{
				float3 p0 = i[0].worldPos;
				float3 p1 = i[1].worldPos;
				float3 p2 = i[2].worldPos;

				float3 triangleNormal = normalize(cross(p1 - p0, p2 - p0)); // get the normal of the face


				InterpolatorsGeometry g0, g1, g2;
				g0.data = i[0];
				g1.data = i[1];
				g2.data = i[2];

				// Stores flatNormal
				g0.flatNormal = triangleNormal;
				g1.flatNormal = triangleNormal;
				g2.flatNormal = triangleNormal;


				// Stores barycentric coordinates used to make wireframe
				g0.barycentricCoordinates = float2(1, 0);
				g1.barycentricCoordinates = float2(0, 1);
				g2.barycentricCoordinates = float2(0, 0);

				stream.Append(g0);
				stream.Append(g1);
				stream.Append(g2);
			}
			
			
			fixed4 frag (InterpolatorsGeometry i, fixed facing : VFACE) : SV_Target
			{

				// Get some variables
				//float redVertexColor = pow(saturate(i.data.color.r), _VertexColorPower);
				float3 worldPosition = i.data.worldPos;
				float3 toCam = normalize(_WorldSpaceCameraPos - worldPosition);

				// ScreenSpace
				float2 screenWPos = i.data.screenPos.xy / i.data.screenPos.w;

				// Wireframe Process using Barycentric Coords
				float3 barys;
				barys.xy = i.barycentricCoordinates;
				barys.z = 1 - barys.x - barys.y; // Deduce z using x and y
				float minBary = min(barys.x, min(barys.y, barys.z)); // get the closer to an edge value
				float delta = fwidth(minBary) * _WireframeWidth / i.data.screenPos.z;
				minBary = smoothstep(0, delta, minBary);
				fixed3 wires = saturate(1 - minBary);

				// Process light direction
				int lightID = _WorldSpaceLightPos0.w;
				float3 directionalLightDir = normalize(_WorldSpaceLightPos0.xyz);
				float3 pointLightDir = normalize(worldPosition - _WorldSpaceLightPos0.xyz);
				float3 lightDir = lerp(directionalLightDir, pointLightDir, lightID);

				// Diffuse lighting
				float diffuse = dot(i.flatNormal, lightDir) * 0.5 + 0.5;
				diffuse = saturate(diffuse);
				float diffuseLight = diffuse * _LightColor0.rgb;
				diffuseLight *= facing;

				// Process Reflection with this Light
				float3 H = normalize(lightDir + toCam);
				float NdotH = 1 - saturate(dot(i.flatNormal, H));
				NdotH = pow(NdotH, _ReflexionPower);
				float3 lightReflexion = NdotH * _LightColor0.rgb * _ReflexionIntensity;
				lightReflexion *= facing;

				// Apply color
				fixed4 col;
				col.rgb = _Color.rgb;
				col.rgb += _WireColor.rgb * wires;
				col.a = 1.0;

				return col;

			}
			ENDCG
		}
	}
}
