Shader "Unlit/sunShader"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Main Texture", 2D) = "white" {}
		_FresnelIntensity ("Fresnel Intensity", Range(0,1)) = 0
		_SFresnelPow ("Smooth Fresnel Power", float) = 1
		_FFresnelPow ("Flat Fresnel Power", float) = 1
		_TriplanarOffset ("Triplanar Offset (XY)", Vector) = (1,1,1,1)
		_TriplanarScale ("Triplanar Scale", float) = 1
		_TriplanarPow ("Triplanar Power", float) = 1
		_FlowMap ("Flow Map", 2D) = "white" {}
		_FlowFreq ("Flow Frequency", float) = 1
		_FlowFactor ("Flow Factor", float) = 1
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
		LOD 100

		Blend SrcAlpha OneMinusSrcAlpha
		//Zwrite Off
		//Cull off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma geometry geo
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float3 wVertex : TEXCOORD1;
				fixed4 color : COLOR;
			};

			struct v2g {
				float3 normal : NORMAL;
				float4 vertex : SV_POSITION;
				float3 wVertex : TEXCOORD1;
				fixed4 color : COLOR;
				float3 flatNormal : TEXCOORD2;
			};

			struct g2f
			{
				v2g data;
				float2 barycentricCoordinates : TEXCOORD3;
			};

			sampler2D _MainTex, _FlowMap;
			float4 _MainTex_ST;
			float4 _Color;
			float _FresnelIntensity, _SFresnelPow, _FFresnelPow;
			float2 _TriplanarOffset;
			float _TriplanarScale, _TriplanarPow;
			float _FlowFreq, _FlowFactor;
			
			v2g vert (appdata v)
			{
				v2g o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.normal = UnityObjectToWorldNormal(v.normal);
				o.wVertex = mul (unity_ObjectToWorld, v.vertex);
				o.color = v.color;
				o.flatNormal = 0;
				return o;
			}

			[maxvertexcount(3)]
			void geo (triangle v2g i[3], inout TriangleStream<g2f> stream)
			{
				float3 p0 = i[0].wVertex.xyz;
				float3 p1 = i[1].wVertex.xyz;
				float3 p2 = i[2].wVertex.xyz;

				float3 triangleNormal = normalize(cross(p1 - p0, p2 - p0));

				i[0].flatNormal = triangleNormal;
				i[1].flatNormal = triangleNormal;
				i[2].flatNormal = triangleNormal;

				g2f g0, g1, g2;
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

			float2 flowProcess(float2 flow, float freq) {
				flow = flow * 2 - 1; // Get flow between -1 / 1
				float fracEven = frac(_Time.y * _FlowFreq);
				float fracOdd = frac(_Time.y * _FlowFreq + 0.5);
				float2 newFlow = flow * fracEven * _FlowFactor;
				float2 newFlowOffseted = flow * fracOdd * _FlowFactor;

				return newFlow;
			}
			
			fixed4 frag (g2f i, fixed facing : VFACE) : SV_Target
			{
				
				// Inverting normal for VFACE
				float3 invertedNormal = - i.data.normal;
				facing = step(1, facing);
				i.data.normal = lerp(invertedNormal, i.data.normal, facing);
				// Inverting flatNormal for VFACE
				float3 invertedFlatNormal = - i.data.flatNormal;
				facing = step(1, facing);
				i.data.flatNormal = lerp(invertedFlatNormal, i.data.flatNormal, facing);
				
				// To Camera Vector
				float3 toCam = _WorldSpaceCameraPos - i.data.wVertex;
				toCam = normalize(toCam);



				// Flat Fresnel
				float flatFresnel = dot(toCam, i.data.flatNormal);
				flatFresnel = (flatFresnel - _FresnelIntensity) / (1 - _FresnelIntensity);
				flatFresnel = 1 - flatFresnel;
				flatFresnel = pow(flatFresnel, _FFresnelPow);
				flatFresnel = saturate(flatFresnel);

				// Smooth Fresnel
				float smoothFresnel = dot(toCam, i.data.normal);
				smoothFresnel = (smoothFresnel - _FresnelIntensity) / (1 - _FresnelIntensity);
				smoothFresnel = pow(smoothFresnel, _SFresnelPow);
				smoothFresnel = saturate(smoothFresnel);

				// Combine Fresnels
				float fresnel = saturate(smoothFresnel + flatFresnel);

				//Triplanar Z
				float2 triplanarUVz = float2(
										(i.data.wVertex.x * _TriplanarScale) + _TriplanarOffset.x, 
										(i.data.wVertex.y * _TriplanarScale) + _TriplanarOffset.y);
				float2 flowZ = tex2D(_FlowMap, triplanarUVz).rg; // flowMap
				flowZ = flowProcess(flowZ, _FlowFreq);
				float3 triplanarTexZ = tex2D(_MainTex, triplanarUVz + flowZ).rgb; // MainTex

				//Triplanar Y
				float2 triplanarUVy = float2(
										(i.data.wVertex.z * _TriplanarScale) + _TriplanarOffset.x, 
										(i.data.wVertex.x * _TriplanarScale) + _TriplanarOffset.y);
				float2 flowY = tex2D(_FlowMap, triplanarUVy).rg; // flowMap
				flowZ = flowProcess(flowY, _FlowFreq);
				float3 triplanarTexY = tex2D(_MainTex, triplanarUVy + flowY).rgb; // MainTex

				//Triplanar X
				float2 triplanarUVx = float2(
										(i.data.wVertex.z * _TriplanarScale) + _TriplanarOffset.x, 
										(i.data.wVertex.y * _TriplanarScale) + _TriplanarOffset.y);
				float2 flowX = tex2D(_FlowMap, triplanarUVx).rg; // flowMap
				flowZ = flowProcess(flowX, _FlowFreq);
				float3 triplanarTexX = tex2D(_MainTex, triplanarUVx + flowX).rgb; // MainTex

				// triplanar MERGE
				float3 facingAxis;
					facingAxis.x = abs(dot(i.data.normal, float3(1,0,0)));
				facingAxis.x = pow(facingAxis.x, _TriplanarPow);
					facingAxis.y = abs(dot(i.data.normal, float3(0,1,0)));
				facingAxis.y = pow(facingAxis.y, _TriplanarPow);
					facingAxis.z = abs(dot(i.data.normal, float3(0,0,1)));
				facingAxis.z = pow(facingAxis.z, _TriplanarPow);
				// normalize
				facingAxis = normalize(facingAxis);

				
				// Sample FlowMap w/ Triplanar UVs
				//float2 flowUvTex = (facingAxis.x * flowTexX) + (facingAxis.y * flowTexY) + (facingAxis.z * flowTexZ);

				// Sample Texture w/ Triplanar UVs and FlowMap offset
				//float3 triplanarTexZ = tex2D(_MainTex, triplanarUVz).rgb; // MainTex
				//float3 triplanarTexY = tex2D(_MainTex, triplanarUVy).rgb; // MainTex
				//float3 triplanarTexX = tex2D(_MainTex, triplanarUVx).rgb; // MainTex
				float3 triplanarTexture = (facingAxis.x * triplanarTexX) + (facingAxis.y * triplanarTexY) + (facingAxis.z * triplanarTexZ);



				fixed4 col = _Color;
				col.rgb *= triplanarTexture;
				col.rgb *= i.data.color.rgb;
				col = fixed4(col.rgb * fresnel, col.a * fresnel);
				//col = fixed4(triplanarTexture,1);
				return col;
			}
			ENDCG
		}
	}
}
