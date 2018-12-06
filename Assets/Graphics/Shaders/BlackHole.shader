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
 
            struct v2f
			{
                half4 pos : SV_POSITION;
                half4 grabPos : TEXCOORD0;
				half4 localPos : TEXCOORD1;
            };
 
            sampler2D _GrabTexture;
            half _Intensity;
 
            v2f vert(appdata_base v)
			{
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.localPos = v.vertex;
                o.grabPos = ComputeGrabScreenPos(o.pos);
                return o;
            }
 
            half4 frag(v2f i) : COLOR
			{
				i.grabPos.x += (i.localPos.x - .5) * _Intensity;
				i.grabPos.y += (i.localPos.y - .5) * _Intensity;

                fixed4 color = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(i.grabPos));
                return color;
            }
            ENDCG
        }
	}
}
