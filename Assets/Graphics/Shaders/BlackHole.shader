// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Mandragora/FX/BlackHole"
{
	Properties
	{
		_Color ("Texture", Color) = (0,0,0,0)
		_Intensity ("Intensity", Range(0, 50)) = 0
	}
	SubShader
	{
		GrabPass { "_GrabTexture" }
 
        Pass {
            Tags { "Queue"="Transparent" }
       
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
                half4 pos : SV_POSITION;
                half4 grabPos : TEXCOORD0;
				float2 uv : TEXCOORD1;
            };
 
            sampler2D _GrabTexture;
            half _Intensity;
 
            v2f vert(appdata v)
			{
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.grabPos = ComputeGrabScreenPos(o.pos);
                return o;
            }
 
            half4 frag(v2f i) : COLOR
			{
                float distFromCenter = sqrt((i.uv.y - .5) * (i.uv.y - .5) + (i.uv.x - .5) * (i.uv.x - .5));

                i.grabPos.y = i.grabPos.y + (i.uv.y - .5) * _Intensity * distFromCenter;
				i.grabPos.x = i.grabPos.x + (i.uv.x - .5) * _Intensity * distFromCenter;

                fixed4 color = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(i.grabPos));
                // fixed4 color = float4(i.grabPos.x, 0, 0, 0);
                return color;
            }
            ENDCG
        }
	}
}
