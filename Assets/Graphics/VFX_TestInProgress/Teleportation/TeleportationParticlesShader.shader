Shader "Unlit/TpParticlesShader"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_AlphaCutoff ("Alpha Cutoff", Range(0,1)) = 0
	}
	SubShader
	{
		Tags { "RenderQueue"="Transparent" "RenderType"="TransparentCutout" "IgnoreProjector"="True" }
		LOD 100

		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha

		GrabPass {
			"_GrabPass"
		}

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
				float4 color : COLOR;
			};

			struct InterpolatorsVertex {
                float4 vertex : SV_POSITION;
				float3 normal : NORMAL;
				float4 screenPos : TEXCOORD2;
                float3 worldVertex : TEXCOORD1;
				float4 color : COLOR;
				float3 vertexGrabPass : TEXCOORD4;
             };

			struct InterpolatorsGeometry {
				InterpolatorsVertex data;
				float3 faceScreenUV : TEXCOORD3;
			};

			sampler2D _GrabPass;
			float4 _Color;
			float _AlphaCutoff;
			
			InterpolatorsVertex vert (appdata v)
			{
				InterpolatorsVertex o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldVertex = mul(unity_ObjectToWorld, v.vertex);
				o.screenPos = ComputeScreenPos(o.vertex);
				o.normal = UnityObjectToWorldNormal(v.normal);
				o.color = v.color;
				o.vertexGrabPass = tex2Dlod(_GrabPass, float4(o.screenPos.xy/o.screenPos.w, 0, 0));
				return o;
			}

			[maxvertexcount(3)]
			void geo (triangle InterpolatorsVertex i[3], inout TriangleStream<InterpolatorsGeometry> stream)
			{
				float4 p0 = i[0].screenPos;
				float4 p1 = i[1].screenPos;
				float4 p2 = i[2].screenPos;

				float2 screenUV = ((p0.xy/p0.w) + (p1.xy/p1.w) + (p2.xy/p2.w)) / 3;
				float screenZ = (p0 + p1 + p2) / 3;

				float3 center = float3(screenUV, screenZ);

			/*	float3 vertexGrabPassColor0 = tex2Dlod(_Tex, float4(p0.xy, 0,0));
				float3 vertexGrabPassColor1 = tex2Dlod(_Tex, float4(p1.xy, 0,0));
				float3 vertexGrabPassColor2 = tex2Dlod(_Tex, float4(p2.xy, 0,0));*/

				InterpolatorsGeometry g0, g1, g2;
				g0.data = i[0];
				g1.data = i[1];
				g2.data = i[2];

				g0.faceScreenUV = center;
				g1.faceScreenUV = center;
				g2.faceScreenUV = center;

				stream.Append(g0);
				stream.Append(g1);
				stream.Append(g2);
			}
			
			
			fixed4 frag (InterpolatorsGeometry i, fixed facing : VFACE) : SV_Target
			{

				float2 screenWPos = i.data.screenPos.xy / i.data.screenPos.w;

				fixed4 col;

				float3 grabPassColor = tex2D(_GrabPass, screenWPos).rgb;
				float3 grabPassFaceCenterColor = tex2D(_GrabPass, i.faceScreenUV).rgb;

				//col.rgb =/* i.data.color.rgb * _Color **/ grabPassFaceCenterColor;
				col = float4(saturate(i.data.vertexGrabPass), 1) * i.data.color * _Color;

				//clip(col.a - _AlphaCutoff);

				return col;

			}
			ENDCG
		}
	}
}
