#warning Upgrade NOTE: unity_Scale shader variable was removed; replaced 'unity_Scale.w' with '1.0'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "FX/Water (Basic)" {
Properties {
	_horizonColor ("Horizon color", COLOR)  = ( .172 , .463 , .435 , 0)
	_WaveScale ("Wave scale", Range (0.02,0.15)) = .07
	_ColorControl ("Reflective color (RGB) fresnel (A) ", 2D) = "" { }
	_ColorControlCube ("Reflective color cube (RGB) fresnel (A) ", Cube) = "" { TexGen CubeReflect }
	_BumpMap ("Waves Normalmap ", 2D) = "" { }
	WaveSpeed ("Wave speed (map1 x,y; map2 x,y)", Vector) = (19,9,-16,-7)
	_MainTex ("Fallback texture", 2D) = "" { }
}

CGINCLUDE
// -----------------------------------------------------------
// This section is included in all program sections below

#include "UnityCG.cginc"

uniform float4 _horizonColor;

uniform float4 WaveSpeed;
uniform float _WaveScale;
uniform float4 _WaveOffset;

struct appdata {
	float4 vertex : POSITION;
	float3 normal : NORMAL;
};

struct v2f {
	float4 pos : SV_POSITION;
	float2 bumpuv[2] : TEXCOORD0;
	float3 viewDir : TEXCOORD2;
};

v2f vert(appdata v)
{
	v2f o;
	float4 s;

	o.pos = UnityObjectToClipPos (v.vertex);

	// scroll bump waves
	float4 temp;
	temp.xyzw = v.vertex.xzxz * _WaveScale / 1.0 + _WaveOffset;
	o.bumpuv[0] = temp.xy * float2(.4, .45);
	o.bumpuv[1] = temp.wz;

	// object space view direction
	o.viewDir.xzy = normalize( ObjSpaceViewDir(v.vertex) );

	return o;
}

ENDCG
	
// -----------------------------------------------------------
// Fragment program

Subshader {
	Tags { "RenderType"="Opaque" }
	Pass {

CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest 

sampler2D _BumpMap;
sampler2D _ColorControl;

half4 frag( v2f i ) : COLOR
{
	half3 bump1 = UnpackNormal(tex2D( _BumpMap, i.bumpuv[0] )).rgb;
	half3 bump2 = UnpackNormal(tex2D( _BumpMap, i.bumpuv[1] )).rgb;
	half3 bump = (bump1 + bump2) * 0.5;
	
	half fresnel = dot( i.viewDir, bump );
	half4 water = tex2D( _ColorControl, float2(fresnel,fresnel) );
	
	half4 col;
	col.rgb = lerp( water.rgb, _horizonColor.rgb, water.a );
	col.a = _horizonColor.a;
	return col;
}
ENDCG
	}
}

// -----------------------------------------------------------
//  Old cards

// three texture, cubemaps
Subshader {
	Tags { "RenderType"="Opaque" }
	Pass {
		Color (0.5,0.5,0.5,0.5)
		SetTexture [_MainTex] {
			Matrix [_WaveMatrix]
			combine texture * primary
		}
		SetTexture [_MainTex] {
			Matrix [_WaveMatrix2]
			combine texture * primary + previous
		}
		SetTexture [_ColorControlCube] {
			combine texture +- previous, primary
			Matrix [_Reflection]
		}
	}
}

// dual texture, cubemaps
Subshader {
	Tags { "RenderType"="Opaque" }
	Pass {
		Color (0.5,0.5,0.5,0.5)
		SetTexture [_MainTex] {
			Matrix [_WaveMatrix]
			combine texture
		}
		SetTexture [_ColorControlCube] {
			combine texture +- previous, primary
			Matrix [_Reflection]
		}
	}
}

// single texture
Subshader {
	Tags { "RenderType"="Opaque" }
	Pass {
		Color (0.5,0.5,0.5,0)
		SetTexture [_MainTex] {
			Matrix [_WaveMatrix]
			combine texture, primary
		}
	}
}

}
