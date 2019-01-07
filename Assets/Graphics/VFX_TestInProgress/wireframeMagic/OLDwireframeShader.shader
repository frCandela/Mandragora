Shader "Mandragora/OLDwireframeShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_TextureInfluence ("Texture Influence", Range(0,1)) = 1
		_Color ("Color", Color) = (1,1,1,1)
		_EmissiveColor ("Emissive Color", Color) = (1,1,1,1)
		_Emissive("Emissive", float) = 0
		_WireframeWidth ("_WireframeWidth", float) = 0
		_AlphaCutoff ("Alpha Cutoff", Range(0,1)) = 0
	}
	SubShader
	{
		Tags { "Queue"="AlphaTest" "RenderType"="TransparentCutout" "IgnoreProjector"="True" }
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

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float3 worldVertex : TEXCOORD1;
				float2 uv : TEXCOORD0;
			};

			struct InterpolatorsVertex {
                float4 vertex : SV_POSITION;
				float3 normal : NORMAL;
				float4 screenPos : TEXCOORD2;
                float2 uv : TEXCOORD0;
                float3 worldVertex : TEXCOORD1;
				float3 flatNormals : TEXCOORD4;
             };

			struct InterpolatorsGeometry {
				InterpolatorsVertex data;
				float2 barycentricCoordinates : TEXCOORD3;
			};

			sampler2D _MainTex;
			float4 _Color, _EmissiveColor;
			float _WireframeWidth, _AlphaCutoff, _Emissive, _TextureInfluence;
			
			InterpolatorsVertex vert (appdata v)
			{
				InterpolatorsVertex o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldVertex = mul(unity_ObjectToWorld, v.vertex);
				o.screenPos = ComputeScreenPos(o.vertex);
				o.normal = UnityObjectToWorldNormal(v.normal);
				o.flatNormals = float3(0,0,0);
				o.uv = v.uv;
				return o;
			}

			[maxvertexcount(3)]
			void geo (triangle InterpolatorsVertex i[3], inout TriangleStream<InterpolatorsGeometry> stream)
			{
				float3 p0 = i[0].worldVertex.xyz;
				float3 p1 = i[1].worldVertex.xyz;
				float3 p2 = i[2].worldVertex.xyz;

				float3 triangleNormal = normalize(cross(p1 - p0, p2 - p0));

				i[0].flatNormals = triangleNormal;
				i[1].flatNormals = triangleNormal;
				i[2].flatNormals = triangleNormal;

				InterpolatorsGeometry g0, g1, g2;
				g0.data = i[0];
				g1.data = i[1];
				g2.data = i[2];

				g0.barycentricCoordinates = float2(1, 0);
				g1.barycentricCoordinates = float2(0, 1);
				g2.barycentricCoordinates = float2(0, 0);

				stream.Append(g0);
				stream.Append(g1);
				stream.Append(g2);
			}
			
			
			fixed4 frag (InterpolatorsGeometry i) : SV_Target
			{
				// sample the texture
				float3 toCam = _WorldSpaceCameraPos.xyz - i.data.worldVertex;
				float3 toCamNorm = normalize(toCam);
				float toLight = normalize(_WorldSpaceLightPos0.xyz - i.data.worldVertex);
				float NdptL = dot(i.data.normal, _WorldSpaceLightPos0.xyz) * 0.5 + 0.5;

				float2 screenWPos = i.data.screenPos.xy / i.data.screenPos.w;

				fixed4 col;

				float3 textureColor = tex2D(_MainTex, i.data.uv).rgb;
				float3 albedo = lerp(_Color.rgb, textureColor.rgb, _TextureInfluence);

				float3 barys;
				barys.xy = i.barycentricCoordinates;
				barys.z = 1 - barys.x - barys.y;
				float minBary = min(barys.x, min(barys.y, barys.z));
				float delta = fwidth(minBary) * _WireframeWidth / length(toCam);
				minBary = smoothstep(0, delta, minBary);
				fixed3 wires = 1 - minBary;

				col.rgb = wires * albedo;
				col.rgb += _Emissive * _EmissiveColor.rgb;
				
				clip(wires - _AlphaCutoff);
				col.a = wires;
				return col;


			}
			ENDCG
		}
	}
}
