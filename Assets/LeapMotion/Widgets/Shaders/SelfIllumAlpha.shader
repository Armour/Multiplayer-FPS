// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "SelfIllumAlpha" {
Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Texture", 2D) = "white" { }
}
SubShader {
	Tags { "Queue" = "Transparent" }
    Pass {
		
		
		Cull Back
        Blend SrcAlpha OneMinusSrcAlpha 
        
		
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		
		#include "UnityCG.cginc"
		
		float4 _Color;
		sampler2D _MainTex;
			
		struct v2f {
		    float4  pos : SV_POSITION;
		    float2  uv : TEXCOORD0;
		};
		
		float4 _MainTex_ST;
		
		v2f vert (appdata_base v)
		{
		    v2f o;
		    o.pos = UnityObjectToClipPos (v.vertex);
		    o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
		    return o;
		}
		
		half4 frag (v2f i) : COLOR
		{
		    half4 texcol = tex2D (_MainTex, i.uv);
		    return texcol * _Color;
		}
		ENDCG
    }
}
Fallback off
} 