Shader "Unlit/SunCoreShader"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_Luminosity("Luminosity", float) = 1
		_ExplosionFactor ("Explosion Factor", float) = 0
		_SizeFactor ("Size Factor", float) = 1
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
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
			};

			float4 _Color;
			float _Luminosity;
			float _ExplosionFactor, _SizeFactor;
			
			v2f vert (appdata v)
			{
				v2f o;
				v.vertex.xyz += v.normal * _ExplosionFactor;
				v.vertex.xyz *= _SizeFactor;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = _Color * _Luminosity;
				return col;
			}
			ENDCG
		}
	}
}
