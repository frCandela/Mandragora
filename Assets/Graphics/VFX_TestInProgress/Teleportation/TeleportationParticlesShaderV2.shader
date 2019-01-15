Shader "Mandragora/TpParticlesShaderV2"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_ColorOverride ("Override Color", Color) = (0,0,0,1)
		_OverrideFactor ("Override Color Factor", Range(0,1)) = 0
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
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
			};

			struct v2g {
                float4 vertex : SV_POSITION;
				float4 screenPos : TEXCOORD2;
				float4 color : COLOR;
				float3 vertexGrabPass : TEXCOORD4;
             };

			 struct g2f {
                v2g data;
				float3 mergedColor : TEXCOORD5;
             };

			sampler2D _GrabPass;
			float4 _Color, _ColorOverride;
			float _OverrideFactor;
			
			v2g vert (appdata v)
			{
				v2g o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.screenPos = ComputeScreenPos(o.vertex);
				o.color = v.color;
				o.vertexGrabPass = tex2Dlod(_GrabPass, float4(o.screenPos.xy/o.screenPos.w, 0, 0));
				return o;
			}


			[maxvertexcount(3)]
			void geo (triangle v2g i[3], inout TriangleStream<g2f> stream)
			{
				float3 p0 = i[0].vertexGrabPass;
				float3 p1 = i[1].vertexGrabPass;
				float3 p2 = i[2].vertexGrabPass;

				float3 mergeColor = (p0 + p1 + p2)/3;

				g2f g0, g1, g2;
				g0.data = i[0];
				g1.data = i[1];
				g2.data = i[2];

				g0.mergedColor = mergeColor;
				g1.mergedColor = mergeColor;
				g2.mergedColor = mergeColor;

				stream.Append(g0);
				stream.Append(g1);
				stream.Append(g2);
			}
			
			
			fixed4 frag (g2f i, fixed facing : VFACE) : SV_Target
			{

				//float2 screenWPos = i.screenPos.xy / i.screenPos.w;

				fixed4 col = float4(0,0,0,0);
				col.rgb = i.mergedColor.rgb * i.data.color.rgb * _Color.rgb;
				col.a = i.data.color.a;

				col = lerp(col, _ColorOverride, _OverrideFactor);

				return col;

			}
			ENDCG
		}
	}
}
