Shader "Unlit/SunShaderV1"
{
	Properties
	{
		_Color1 ("Color 1", Color) = (1,1,1,1)
		_Color2 ("Color 2", Color) = (1,1,1,1)
		_Color3 ("Color 3", Color) = (1,1,1,1)
		_MainTex ("Main Texture", 2D) = "white" {}
		_SmoothFresnelIntensity ("Smooth Fresnel Intensity", Range(0,1)) = 0
		_FlatFresnelIntensity ("Flat Fresnel Intensity", Range(0,1)) = 0
		_SFresnelPow ("Smooth Fresnel Power", float) = 1
		_FFresnelPow ("Flat Fresnel Power", float) = 1
		_TriplanarOffset ("Triplanar Offset (XY)", Vector) = (1,1,1,1)
		_TriplanarScale ("Triplanar Scale", float) = 1
		_TriplanarPow ("Triplanar Power", float) = 1
		_FlowMap ("Flow Map", 2D) = "white" {}
		_FlowFreq ("Flow Frequency", float) = 1
		_FlowFactor ("Flow Factor", float) = 1
		_FlowTimeOffset ("Flow Time Offset", float) = 1
		_DistortionStrength ("Distortion Strength", float) = 0
		_NoiseFreq ("Noise Frequency (XYZ)", Vector) = (1,1,1,0)
		_NoiseAmplitude ("Noise Amplitude (XYZ)", Vector) = (1,1,1,0)
	}
	SubShader
	{
		Tags { "Queue"="Transparent+1" "RenderType"="Transparent" "IgnoreProjector"="True" }
		LOD 100

		Blend SrcAlpha OneMinusSrcAlpha
		//Zwrite Off
		//Cull off

		// Grab the screen behind the object into _BackgroundTexture
        GrabPass
        {
            "_BackgroundTexture"
        }

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
				float4 grabPos : TEXCOORD4;
				float3 normal : NORMAL;
				float3 wVertex : TEXCOORD1;
				fixed4 color : COLOR;
			};

			struct v2g {
				float3 normal : NORMAL;
				float4 vertex : SV_POSITION;
				float4 grabPos : TEXCOORD4;
				float3 wVertex : TEXCOORD1;
				fixed4 color : COLOR;
				float3 flatNormal : TEXCOORD2;
			};

			struct g2f
			{
				v2g data;
				float2 barycentricCoordinates : TEXCOORD3;
			};

			sampler2D _MainTex, _FlowMap, _BackgroundTexture;
			float4 _MainTex_ST;
			float4 _Color1, _Color2, _Color3;
			float _SmoothFresnelIntensity, _FlatFresnelIntensity, _SFresnelPow, _FFresnelPow;
			float2 _TriplanarOffset;
			float _TriplanarScale, _TriplanarPow;
			float _FlowFreq, _FlowFactor, _FlowTimeOffset;
			float _DistortionStrength;
			float3 _NoiseFreq, _NoiseAmplitude;
			
			v2g vert (appdata v)
			{
				v2g o;

				// Noise
				float3 noise = float3(0,0,0);
				noise.x = (cos((v.vertex.y + _Time.y) * _NoiseFreq.x) + sin((v.vertex.x + _Time.y) * _NoiseFreq.z)) * _NoiseAmplitude.x;
				noise.y = (cos((v.vertex.z + _Time.y) * _NoiseFreq.y) + sin((v.vertex.y + _Time.y) * _NoiseFreq.x)) * _NoiseAmplitude.y;
				noise.z = (cos((v.vertex.x + _Time.y) * _NoiseFreq.z) + sin((v.vertex.z + _Time.y) * _NoiseFreq.y)) * _NoiseAmplitude.z;
				v.vertex.xyz += noise; // Apply

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.normal = UnityObjectToWorldNormal(v.normal);
				o.wVertex = mul (unity_ObjectToWorld, v.vertex);
				o.grabPos = ComputeGrabScreenPos(o.vertex);
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

			float2 flowProcess(float2 flow, float freq, float timeOffset) {
				flow = flow * 2 - 1; // Get flow between -1 / 1
				float time = _Time.y * _FlowFreq;
				float fracEven = frac(time + timeOffset);
				float fracOdd = frac(time + 0.5 + timeOffset);
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
				flatFresnel = (flatFresnel - _FlatFresnelIntensity) / (1 - _FlatFresnelIntensity);
				flatFresnel = 1 - flatFresnel;
				flatFresnel = pow(flatFresnel, _FFresnelPow);
				flatFresnel = saturate(flatFresnel);

				// Smooth Fresnel
				float smoothFresnel = dot(toCam, i.data.normal);
				smoothFresnel = (smoothFresnel - _SmoothFresnelIntensity) / (1 - _SmoothFresnelIntensity);
				smoothFresnel = pow(smoothFresnel, _SFresnelPow);
				smoothFresnel = saturate(smoothFresnel);

				// Combine Fresnels
				float fresnel = saturate(smoothFresnel + flatFresnel);

				//Triplanar Z
				float2 triplanarUVz = float2(
										(i.data.wVertex.x * _TriplanarScale) + _TriplanarOffset.x, 
										(i.data.wVertex.y * _TriplanarScale) + _TriplanarOffset.y);
				float3 flowZ = tex2D(_FlowMap, triplanarUVz).rga; // flowMap
				flowZ.rg = flowProcess(flowZ.rg, _FlowFreq, flowZ.b * _FlowTimeOffset);
				float3 triplanarTexZ = tex2D(_MainTex, triplanarUVz + flowZ.xy).rgb; // MainTex

				//Triplanar Y
				float2 triplanarUVy = float2(
										(i.data.wVertex.z * _TriplanarScale) + _TriplanarOffset.x, 
										(i.data.wVertex.x * _TriplanarScale) + _TriplanarOffset.y);
				float3 flowY = tex2D(_FlowMap, triplanarUVy).rga; // flowMap
				flowY.xy = flowProcess(flowY.xy, _FlowFreq, flowY.z * _FlowTimeOffset);
				float3 triplanarTexY = tex2D(_MainTex, triplanarUVy + flowY.xy).rgb; // MainTex

				//Triplanar X
				float2 triplanarUVx = float2(
										(i.data.wVertex.z * _TriplanarScale) + _TriplanarOffset.x, 
										(i.data.wVertex.y * _TriplanarScale) + _TriplanarOffset.y);
				float3 flowX = tex2D(_FlowMap, triplanarUVx).rga; // flowMap
				flowX.xy = flowProcess(flowX.xy, _FlowFreq, flowX.z * _FlowTimeOffset);
				float3 triplanarTexX = tex2D(_MainTex, triplanarUVx + flowX.xy).rgb; // MainTex

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



				// Sample Texture w/ Triplanar UVs and FlowMap offset
				float3 triplanarTexture = (facingAxis.x * triplanarTexX) + (facingAxis.y * triplanarTexY) + (facingAxis.z * triplanarTexZ);


				// From Grab Pass
				float4 distortion = float4(0,0,0,0);
				distortion.x += _DistortionStrength * (1 - smoothFresnel) * triplanarTexture.r;
				distortion.y += _DistortionStrength * (1 - smoothFresnel) * triplanarTexture.g;
				//distortion *= (1 - smoothFresnel);
				float3 grabTex = tex2Dproj(_BackgroundTexture, i.data.grabPos + distortion).rgb;



				fixed4 col = _Color1 * (smoothFresnel + (triplanarTexture.r * 0.5 - 0.5));
				col += _Color1 * triplanarTexture.r * smoothFresnel;
				col += _Color2 * (1 - triplanarTexture.r) * smoothFresnel;
				col += _Color3 * flatFresnel;
				col = max(_Color2 * 0.2 * (1 - smoothFresnel) * (1 - triplanarTexture.r), col);
				


				float3 color = (col.rgb * col.a) + ((1 - col.a) * grabTex);
				color.rgb = max(_Color1.rgb * 0.1, color.rgb);
				float alpha = 1;
				col = fixed4(color, alpha);
				//col.rgb = 1 - grabTex;
				return col;
			}
			ENDCG
		}
	}
}
