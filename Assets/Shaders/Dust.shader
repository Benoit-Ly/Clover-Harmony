Shader "Custom/SpaceDustShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "gray" {}
		_Mask("Mask", 2D) = "white" {}
		valueX("X", float) = 0
		valueY("Y", float) = 0
		_Color("Color", Color) = (1,0,1,1)
		Intensity("Intensity", float) = 0
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"}
		Cull off
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off
		Lighting Off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _Mask;
			float valueX;
			float valueY;
			float4 _Color;
			float Intensity;

			float4 frag(v2f_img i) : COLOR
			{
				float2 uv = i.uv;
				float2 uv2 = i.uv;

				uv.x += valueX * _Time.x;
				uv2.x += valueY *_Time.x;

				float4 finalColor = tex2D(_MainTex, uv) * _Color;
				float finaleMask = tex2D(_Mask, uv2);

				float4 final = finalColor * finaleMask *Intensity;

				return final;
			}

			ENDCG
		}
	}
}