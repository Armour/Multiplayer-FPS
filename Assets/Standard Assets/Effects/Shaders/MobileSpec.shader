// Simplified Specular shader. Differences from regular Specular one:
// - no Main Color nor Specular Color
// - specular lighting directions are approximated per vertex
// - writes zero to alpha channel
// - no Deferred Lighting support
// - no Lightmap support
// - fully supports only 1 directional light. Other lights can affect it, but it will be per-vertex/SH.

Shader "Mobile/Specular" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1) 
	_SpecColor ("Specular Color", Color) = (0.5,0.5,0.5,1)
	_Shininess ("Shininess", Range (0.03, 1)) = 0.078125
	_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
}
SubShader { 
	Tags { "RenderType"="Opaque" }
	LOD 250
	
CGPROGRAM
#pragma surface surf MobileBlinnPhong exclude_path:prepass nolightmap noforwardadd halfasview

inline fixed4 LightingMobileBlinnPhong (SurfaceOutput s, fixed3 lightDir, fixed3 halfDir, fixed atten)
{
	fixed diff = max (0, dot (s.Normal, lightDir));
	fixed nh = max (0, dot (s.Normal, halfDir));
	fixed spec = pow (nh, s.Specular*128) * s.Gloss;
	
	fixed4 c;
	c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * spec * _SpecColor) * (atten*2);
	c.a = 0.0;
	return c;
}

sampler2D _MainTex;
half _Shininess;
half4 _Color;

struct Input {
	float2 uv_MainTex;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	o.Albedo = tex.rgb * _Color;
	o.Gloss = tex.a;
	o.Alpha = tex.a;
	o.Specular = _Shininess;
}
ENDCG
}

FallBack "Mobile/VertexLit"
}
