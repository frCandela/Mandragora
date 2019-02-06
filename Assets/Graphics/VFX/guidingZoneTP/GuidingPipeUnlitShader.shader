Shader "Mandragora/GuidingPipeUnlitShader"
{
	Properties
	{
		_Color ("Main Color", Color) = (0,0,0,1)
		_Luminosity ("Luminosity", float) = 1
		_Completion ("Pipe Completion", Range(0,1)) = 0.5
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			float4 _Color;
			float _Luminosity;
			float _Direction, _Completion;
			float _ManagerUnlitFactor;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float slider = step(i.uv.y, _Completion);
				fixed4 col = _Color * _Luminosity;
				col *= slider;
				//col *= _ManagerUnlitFactor;
				return col;
			}
			ENDCG
		}
	}
}
