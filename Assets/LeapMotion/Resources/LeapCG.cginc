#include "UnityCG.cginc"

/////////////// Constants for Dragonfly Color Correction ///////////////
#define CAMERA_WIDTH  608.0
#define CAMERA_HEIGHT 540.0
#define CAMERA_DELTA  float2(1.0 / CAMERA_WIDTH, 1.0 / CAMERA_HEIGHT)

#define RGB_SCALE     1.5 * float3(1.5, 1.0, 0.5)

#define R_OFFSET      CAMERA_DELTA * float2(-0.5, 0.0)
#define G_OFFSET      CAMERA_DELTA * float2(-0.5, 0.5)
#define B_OFFSET      CAMERA_DELTA * float2( 0.0, 0.5)

#define TRANSFORMATION  transpose(float4x4(5.0670, -1.2312, 0.8625, -0.0507, -1.5210, 3.1104, -2.0194, 0.0017, -0.8310, -0.3000, 13.1744, -0.1052, -2.4540, -1.3848, -10.9618, 1.0000))
#define CONSERVATIVE    transpose(float4x4(5.0670, 0.0000, 0.8625, 0.0000, 0.0000, 3.1104, 0.0000, 0.0017, 0.0000, 0.0000, 13.1744, 0.0000, 0.0000, 0.0000, 0.0000, 1.0000))

#define FUDGE_THRESHOLD 0.5
#define FUDGE_CONSTANT  (1 / (1 - FUDGE_THRESHOLD))
////////////////////////////////////////////////////////////////////////                                       

sampler2D _LeapTexture;
sampler2D _LeapDistortion;

float4 _LeapProjection;
float _LeapGammaCorrectionExponent;

float2 LeapGetUndistortedUV(float4 screenPos){
	float2 uv = (screenPos.xy / screenPos.w) * 2 - float2(1,1);
	float2 tangent = (uv + _LeapProjection.xy) / _LeapProjection.zw;
	float2 distortionUV = 0.125 * tangent + float2(0.5, 0.5);

	float4 distortionAmount = tex2D(_LeapDistortion, distortionUV);
	return float2(DecodeFloatRG(distortionAmount.xy), DecodeFloatRG(distortionAmount.zw)) * 2.3 - float2(0.6, 0.6);
}

float LeapRawBrightnessUV(float2 uv){
	#if LEAP_FORMAT_IR
		return tex2D(_LeapTexture, uv).a;
	#else
		return pow(dot(tex2D(_LeapTexture, uv), float4(1, -0.105, -0.05, -0.001)), 0.5);
	#endif
}

float3 LeapRawColorUV(float2 uv){
	#if LEAP_FORMAT_IR
		float brightness = LeapRawBrightnessUV(uv);
		return float3(brightness, brightness, brightness);
	#else
		float4 input_lf;

		input_lf.a = tex2D(_LeapTexture, uv).a;
		input_lf.r = tex2D(_LeapTexture, uv + R_OFFSET).b;
		input_lf.g = tex2D(_LeapTexture, uv + G_OFFSET).r;
		input_lf.b = tex2D(_LeapTexture, uv + B_OFFSET).g;

		float4 output_lf       = mul(TRANSFORMATION, input_lf);
		float4 output_lf_fudge = mul(CONSERVATIVE,   input_lf);

		float3 fudgeMult = input_lf.rgb * FUDGE_CONSTANT - FUDGE_CONSTANT * FUDGE_THRESHOLD;
		float3 fudge = step(FUDGE_THRESHOLD, input_lf.rgb) * fudgeMult;

		float3 color = (output_lf_fudge.rgb - output_lf.rgb) * fudge * fudge + output_lf.rgb;

		return saturate(color * RGB_SCALE);
	#endif
}

float4 LeapRawColorBrightnessUV(float2 uv){
	#if LEAP_FORMAT_IR
		float brightness = LeapRawBrightnessUV(uv);
		return float4(brightness, brightness, brightness, brightness);
	#else
		float4 input_lf;

		input_lf.a = tex2D(_LeapTexture, uv).a;
		input_lf.r = tex2D(_LeapTexture, uv + R_OFFSET).b;
		input_lf.g = tex2D(_LeapTexture, uv + G_OFFSET).r;
		input_lf.b = tex2D(_LeapTexture, uv + B_OFFSET).g;

		float4 output_lf       = mul(TRANSFORMATION, input_lf);
		float4 output_lf_fudge = mul(CONSERVATIVE,   input_lf);

		float3 fudgeMult = input_lf.rgb * FUDGE_CONSTANT - FUDGE_CONSTANT * FUDGE_THRESHOLD;
		float3 fudge = step(FUDGE_THRESHOLD, input_lf.rgb) * fudgeMult;

		float3 color = (output_lf_fudge.rgb - output_lf.rgb) * fudge * fudge + output_lf.rgb;

		return saturate(float4(color * RGB_SCALE, pow(dot(input_lf, float4(-0.051, -0.001, -0.105, 1)), 0.5)));
	#endif
}

float LeapRawBrightness(float4 screenPos){
	return LeapRawBrightnessUV(LeapGetUndistortedUV(screenPos));
}

float3 LeapRawColor(float4 screenPos){
	return LeapRawColorUV(LeapGetUndistortedUV(screenPos));
}

float4 LeapRawColorBrightness(float4 screenPos){
	return LeapRawColorBrightnessUV(LeapGetUndistortedUV(screenPos));
}

float LeapBrightnessUV(float2 uv){
	return pow(LeapRawBrightnessUV(uv), _LeapGammaCorrectionExponent);
}

float3 LeapColorUV(float2 uv){
	return pow(LeapRawColorUV(uv), _LeapGammaCorrectionExponent);
}

float4 LeapColorBrightnessUV(float2 uv){
	return pow(LeapRawColorBrightnessUV(uv), _LeapGammaCorrectionExponent);
}

float LeapBrightness(float4 screenPos){
	return pow(LeapRawBrightness(screenPos), _LeapGammaCorrectionExponent);
}

float3 LeapColor(float4 screenPos){
	return pow(LeapRawColor(screenPos), _LeapGammaCorrectionExponent);
}

float4 LeapColorBrightness(float4 screenPos){
	return pow(LeapRawColorBrightness(screenPos), _LeapGammaCorrectionExponent);
}