Shader "Mandragora/SocleColorWireframe"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_BackColor ("Back Color", Color) = (1,1,1,1)
		_WireColor ("Wireframe Color", Color) = (1,1,1,1)
		_EmissiveColor ("Emissive Color", Color) = (1,1,1,1)
		_Emissive("Emissive", float) = 1
		_WireframeWidth ("Wireframe Width", float) = 0.05
		_AlphaCutoff ("Alpha Cutoff", Range(0,1)) = 0.5
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

			#pragma target 4.0
			#pragma vertex vert
			#pragma fragment frag
			#pragma geometry geo
			
			#include "UnityCG.cginc"
			#include "Lighting.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 color : COLOR;
			};

			struct InterpolatorsVertex {
                float4 vertex : SV_POSITION;
				float3 worldVertex : TEXCOORD0;
				float3 normal : NORMAL;
				float4 color : COLOR;
				float4 screenPos : TEXCOORD1;
             };

			// Uses Geometry Shader
			struct InterpolatorsGeometry {
				InterpolatorsVertex data;
				float2 barycentricCoordinates : TEXCOORD3;
			};

			float4 _WireColor, _Color, _BackColor, _EmissiveColor;
			float _WireframeWidth, _AlphaCutoff, _Emissive;
			float _ReflexionIntensity, _ReflexionPower;
			float _ManagerUnlitFactor;                                           //// FACTOR UNLIT
			
			InterpolatorsVertex vert (appdata v)
			{
				InterpolatorsVertex o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldVertex = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.color = v.color;
				o.normal = UnityObjectToWorldNormal(v.normal);
				o.screenPos = ComputeScreenPos(o.vertex);
				return o;
			}

			//////////// GEOMETRY SHADER /////////////////////
			[maxvertexcount(3)]
			void geo (triangle InterpolatorsVertex i[3], inout TriangleStream<InterpolatorsGeometry> stream)
			{

				InterpolatorsGeometry g0, g1, g2;
				g0.data = i[0];
				g1.data = i[1];
				g2.data = i[2];

				// Stores barycentric coordinates used to make wireframe
				g0.barycentricCoordinates = float2(1, 0);
				g1.barycentricCoordinates = float2(0, 1);
				g2.barycentricCoordinates = float2(0, 0);

				stream.Append(g0);
				stream.Append(g1);
				stream.Append(g2);
			}

			void InitializeFragmentNormal(inout InterpolatorsGeometry v) {
				float3 dpdx = ddx(v.data.worldVertex);
				float3 dpdy = ddy(v.data.worldVertex);
				v.data.normal = normalize(cross(dpdy, dpdx));
	
			}
			
			
			fixed4 frag (InterpolatorsGeometry i) : SV_Target
			{

				// Flatten Normals
				InitializeFragmentNormal(i);



				//////// LIGHTING /////////////////

				// Get some variables
				float3 toCam = normalize(_WorldSpaceCameraPos.xyz - i.data.worldVertex);

				// Process light direction
				int lightID = _WorldSpaceLightPos0.w;
				float3 directionalLightDir = normalize(_WorldSpaceLightPos0.xyz);
				float3 pointLightDir = normalize(i.data.worldVertex - _WorldSpaceLightPos0.xyz);
				float3 lightDir = lerp(directionalLightDir, pointLightDir, lightID);

				// Process diffuse lighting
				float NdotL = saturate(dot(-lightDir, i.data.normal));
				//float3 frontColor = NdotL * _Color.rgb;
				//float3 backColor = (1 - NdotL) * _BackColor.rgb;
				float3 diffuseColor = lerp(_BackColor.rgb, _Color.rgb, NdotL);

				// Process Reflection with this Light
				float3 H = normalize(-lightDir + toCam);
				float NdotH = dot(i.data.normal, H);
				NdotH = saturate(NdotH);
				NdotH = pow(NdotH, _ReflexionPower);
				float3 lightReflexion = NdotH * _LightColor0.rgb * _ReflexionIntensity;
				float alphaLightReflexion = (lightReflexion.r + lightReflexion.g + lightReflexion.b)/3;





				//////// WIREFRAME /////////////////

				float2 screenWPos = i.data.screenPos.xy / i.data.screenPos.w;

				// Wireframe Process using Barycentric Coords
				float3 barys;
				barys.xy = i.barycentricCoordinates;
				barys.z = 1 - barys.x - barys.y; // Deduce z using x and y
				float minBary = min(barys.x, min(barys.y, barys.z)); // get the closer to an edge value
				float delta = fwidth(minBary) * _WireframeWidth / i.data.screenPos.z;
				minBary = smoothstep(0, delta, minBary);
				fixed3 wires = 1 - minBary;



				///////////// APPLY COLOR /////////////

				// Wireframe Color
				float3 wireframeColor = wires * _WireColor.rgb;
				wireframeColor += wires * _Emissive * _EmissiveColor.rgb;
				float wireAlpha = step(_AlphaCutoff, wires);

				// Apply Lighting
				float3 litColor = float3(0,0,0);
				litColor = diffuseColor;
				litColor += lightReflexion;


				// Mixing Wireframe and opaque  &  Apply

				float mixFactor = 1 - i.data.color.r;
				mixFactor = saturate(pow(mixFactor, 1.5));

				fixed4 col;

				wireframeColor = lerp(wireframeColor, litColor, mixFactor);

				col.rgb = lerp(wireframeColor, litColor, mixFactor);
				col.a = saturate(mixFactor + wireAlpha);

				col *= _ManagerUnlitFactor;                    ////////// FACTOR UNLIT


				return col;

			}
			ENDCG
		}
	}
}
