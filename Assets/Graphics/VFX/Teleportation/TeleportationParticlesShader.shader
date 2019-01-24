Shader "Unlit/TpParticlesShader"
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

			struct v2f {
                float4 vertex : SV_POSITION;
				float4 screenPos : TEXCOORD2;
				float4 color : COLOR;
				float3 vertexGrabPass : TEXCOORD4;
             };

			sampler2D _GrabPass;
			float4 _Color, _ColorOverride;
			float _OverrideFactor;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.screenPos = ComputeScreenPos(o.vertex);
				o.color = v.color;
				o.vertexGrabPass = tex2Dlod(_GrabPass, float4(o.screenPos.xy/o.screenPos.w, 0, 0));
				return o;
			}
			
			
			fixed4 frag (v2f i, fixed facing : VFACE) : SV_Target
			{

				float2 screenWPos = i.screenPos.xy / i.screenPos.w;

				fixed4 col = float4(saturate(i.vertexGrabPass), 1) * i.color * _Color;

				col = lerp(col, _ColorOverride, _OverrideFactor);

				return col;

			}
			ENDCG
		}
	}
}
