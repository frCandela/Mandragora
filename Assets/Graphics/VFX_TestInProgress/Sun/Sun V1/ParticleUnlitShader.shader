Shader "Mandragora/ParticleUnlitShader"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_Opacity ("Opacity", Range(0,1)) = 1
		_Luminosity ("Luminosity", float) = 1
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
		LOD 100

		//Blend SrcAlpha OneMinusSrcAlpha
		Blend One SrcAlpha
		Cull Off

		Pass
		{
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 color : COLOR;
			};

			float4 _Color;
			float _Opacity, _Luminosity;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// Apply
				fixed4 col = fixed4(1,1,1,1);
				col.rgb *= i.color.rgb * _Color.rgb;
				col.rgb = saturate(col.rgb);
				col.rgb *= _Luminosity;
				col.a = saturate(i.color.a * _Opacity);

				return col;
			}
			ENDCG
		}
	}
}
