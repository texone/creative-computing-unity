Shader "Sky/TextureSky"
{
	HLSLINCLUDE

	#pragma vertex Vert

	#pragma target 4.5
	#pragma only_renderers d3d11 ps4 xboxone vulkan metal switch

	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonLighting.hlsl"
	#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
	#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Sky/SkyUtils.hlsl"


	float4 _Color;
	sampler2D _NoiseMap;

	struct Attributes
	{
		uint vertexID : SV_VertexID;
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	struct Varyings
	{
		float4 positionCS : SV_POSITION;
		UNITY_VERTEX_OUTPUT_STEREO
	};

	Varyings Vert(Attributes input)
	{
		Varyings output;
		UNITY_SETUP_INSTANCE_ID(input);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
		output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID, UNITY_RAW_FAR_CLIP_VALUE);
		//output.screenPosition = ComputeScreenPos(output.positionCS);
		return output;
	}

	float3 ScreenSpaceDither(float2 vScreenPos)
	{
		// Iestyn's RGB dither (7 asm instructions) from Portal 2 X360, slightly modified for VR
		//float3 vDither = float3( dot( float2( 171.0, 231.0 ), vScreenPos.xy + iTime ) );
		float d = dot(float2(171.0, 231.0), vScreenPos);
		float3 vDither = float3(d,d,d);
		vDither.rgb = frac(vDither.rgb / float3(103.0, 71.0, 97.0));
		return vDither.rgb / 255.0; //note: looks better without 0.375...

		//note: not sure why the 0.5-offset is there...
		//vDither.rgb = fract( vDither.rgb / float3( 103.0, 71.0, 97.0 ) ) - float3( 0.5, 0.5, 0.5 );
		//return (vDither.rgb / 255.0) * 0.375;
	}

	float _Dither;

	float4 RenderSky(Varyings input)
	{
		float3 viewDirWS = GetSkyViewDirWS(input.positionCS.xy);
		
		float phi = (atan2(viewDirWS.z, viewDirWS.x) / (PI) + 1) / 2;
		float delta = (asin(viewDirWS.y) / (PI * 0.5) + 1) / 2;

		return tex2D(_NoiseMap, float2(phi,delta) + ScreenSpaceDither(input.positionCS.xy).xy * _Dither);
	}

	float4 FragBaking(Varyings input) : SV_Target
	{
		return RenderSky(input);
	}

	float4 FragRender(Varyings input) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
		float4 color = RenderSky(input);
		//color.rgb *= GetCurrentExposureMultiplier();
		return color;
	}

	ENDHLSL

	SubShader
	{
		Pass
		{
			ZWrite Off
			ZTest Always
			Blend Off
			Cull Off

			HLSLPROGRAM
				#pragma fragment FragBaking
			ENDHLSL

		}

			Pass
		{
			ZWrite Off
			ZTest LEqual
			Blend Off
			Cull Off

			HLSLPROGRAM
				#pragma fragment FragRender
			ENDHLSL
		}

	}
	Fallback Off
}