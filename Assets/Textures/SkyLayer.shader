Shader "Custom/SkyLayer" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
	    _ScrollXSpeed("X Scrll Speed", Range(0, 10)) = 2
		_ScrollYSpeed("Y Scrll Speed", Range(0, 10)) = 2
	}
		SubShader{
		Tags {"Queue"="Transparent"}
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
#pragma surface surf Lambert alpha

		// Use shader model 3.0 target, to get nicer looking lighting
#pragma target 3.0

		fixed _ScrollXSpeed;
	fixed _ScrollYSpeed;
	sampler2D _MainTex;

	struct Input {
		float2 uv_MainTex;
	};



	void surf(Input IN, inout SurfaceOutput o) {
		fixed2 scrolledUV = IN.uv_MainTex;

		fixed xScrollValue = _ScrollXSpeed * _Time;
		fixed yScrollValue = _ScrollYSpeed * _Time;

		scrolledUV += fixed2(xScrollValue, yScrollValue);

		half4 c = tex2D(_MainTex, scrolledUV);
		o.Albedo = c.rgb;
		o.Alpha = c.a;
	}
	ENDCG
	}
		FallBack "Diffuse"

}
