Shader "Unlit/handShader"
{
	Properties
	{
		_LimitBloom ("Limitation Bloom", Range(0,1)) = 0
		_StarsTex ("Stars Texture", 2D) = "white" {}
		_FlowTex ("Flow Textures", 2D) = "white" {}
		_Tiling ("Textures Tiling", float) = 1
		_FlowIntensity ("Flow Intensity", float) = 1
		_StarsColor1 ("(1) Stars Color", Color) = (1,1,1,1)
		_StarsColor2 ("(2) Stars Color", Color) = (1,1,1,1)
		_StarsColor3 ("(3) Stars Color", Color) = (1,1,1,1)
		_VertexColorMultiply ("Vertex Color Multiplicator", float) = 1
		_VertexColorPower ("Vertex Color Power", float) = 1
		_FadeColor ("FadeColor", Color) = (1,1,1,1)
		_FadeOpacity ("Fade Opacity", Range(0,1)) = 1
		_FresnelFlowIntensity ("Fresnel Flow Intensity", float) = 1
		_ReflexionIntensity ("Reflexion Intensity", Range(0,1)) = 0.1
		_ReflexionPower ("Reflexion Power", float) = 2
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
		LOD 100

		ZWrite Off
		Cull Off

		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma geometry geo
			
			#include "UnityCG.cginc"
			#include "Lighting.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 color : COLOR;
			};

			struct v2g
			{
				float4 vertex : SV_POSITION;
				float3 flatNormal : TEXCOORD3;
				float3 color : COLOR;
				float3 worldVertex : TEXCOORD2;
				float4 screenPos : TEXCOORD1;
			};

			struct g2f
			{
				v2g data;
			};

			sampler2D _StarsTex, _FlowTex;
			float _Tiling, _FlowIntensity, _VertexColorMultiply, _VertexColorPower, _FresnelFlowIntensity, _FadeOpacity;
			float _ReflexionIntensity, _ReflexionPower;
			float _LimitBloom;
			fixed4 _StarsColor1, _StarsColor2, _StarsColor3, _FadeColor;
			
			v2g vert (appdata v)
			{
				v2g o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldVertex = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.screenPos = ComputeScreenPos(o.vertex);
				o.color = v.color;
				o.flatNormal = float3(0,0,0);
				return o;
			}


			[maxvertexcount(3)]
			void geo (triangle v2g i[3], inout TriangleStream<g2f> stream)
			{
				float3 p0 = i[0].worldVertex.xyz;
				float3 p1 = i[1].worldVertex.xyz;
				float3 p2 = i[2].worldVertex.xyz;

				float3 triangleNormal = normalize(cross(p1 - p0, p2 - p0));

				i[0].flatNormal = triangleNormal;
				i[1].flatNormal = triangleNormal;
				i[2].flatNormal = triangleNormal;

				g2f g0, g1, g2;
				g0.data = i[0];
				g1.data = i[1];
				g2.data = i[2];

				stream.Append(g0);
				stream.Append(g1);
				stream.Append(g2);
			}
			

			fixed4 frag (g2f i, fixed facing : VFACE) : SV_Target
			{
				// Get some variables
				float redVertexColor = pow(saturate(i.data.color.r), _VertexColorPower);
				float3 flatNormals = i.data.flatNormal;
				float3 worldPosition = i.data.worldVertex;
				float3 toCam = normalize(_WorldSpaceCameraPos - worldPosition);

				// Inverse Normal for VFACE
				float3 invertedNormal = -flatNormals;
				facing = step(1, facing);
				flatNormals = lerp(invertedNormal, flatNormals, facing);

				// Process light direction
				int lightID = _WorldSpaceLightPos0.w;
				float3 directionalLightDir = normalize(_WorldSpaceLightPos0.xyz);
				float3 pointLightDir = normalize(worldPosition - _WorldSpaceLightPos0.xyz);
				float3 lightDir = lerp(directionalLightDir, pointLightDir, lightID);
				// Process Reflection with this particular Light
				float3 H = normalize(lightDir + toCam);
				float NdotH = 1 - saturate(dot(flatNormals, H));
				NdotH = pow(NdotH, _ReflexionPower);
				float3 lightReflexion = NdotH * _LightColor0.rgb * _ReflexionIntensity * redVertexColor;
				lightReflexion *= facing;


				// Calculate Fresnel that multiplw with Flow Texture
				float fresnel = dot(toCam, flatNormals);
				fresnel = saturate(1 - fresnel);
				fresnel *= _FresnelFlowIntensity;

				// Get Screen UVs
				float2 screenUv = (i.data.screenPos.xy/i.data.screenPos.w) * _Tiling;

				// Get FlowTex with fresnel Multiply
				float3 flowTex = tex2D(_FlowTex, screenUv * (1 + fresnel)).rgb;

				// Get Stars Texture with flow Offset
				float2 starsUvOffset = flowTex.rg * _FlowIntensity;
				float3 starsTex = tex2D(_StarsTex, screenUv + starsUvOffset).rgb;

				// Process FadeColor
				float3 fadeColor = float3(_FadeColor.r * flowTex.b,
											_FadeColor.g * redVertexColor,
											_FadeColor.b);
				float3 finalFade = redVertexColor * _FadeColor.rgb;

				// Noise 3D
				float3 Noise = float3(0,0,0);

				// Apply All colors
				float3 starsCol = float3(0,0,0);
				starsCol += _StarsColor1.rgb * starsTex.r;
				starsCol += _StarsColor2.rgb * starsTex.g;
				starsCol += _StarsColor3.rgb * starsTex.b;
				// saturate and add the fade color that will glow
				starsCol = saturate(starsCol);
				starsCol += finalFade * _FadeOpacity;
				// add Vertex Color Multiply
				starsCol *= _VertexColorMultiply * redVertexColor;


				fixed4 col = fixed4(0,0,0,1);
				col.rgb = starsCol;
				col.rgb += lightReflexion;
				col.rgb = lerp(saturate(col.rgb), col.rgb, _LimitBloom);

				col.a = redVertexColor;
				
				return col;
			}
			ENDCG
		}
	}
}