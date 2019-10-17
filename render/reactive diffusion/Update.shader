Shader "Reactive Diffusion/Update"
{
	Properties
	{
		_Du("Diffusion (u)", Range(0, 1)) = 1
		_Dv("Diffusion (v)", Range(0, 1)) = 0.4
		
		[Toggle(DRAW_PARAMETER_SPACE)] _DrawParameterSpace("Draw Parameter Space", Float) = 0
		[Toggle(DRAW_WARP_CURVES)] _DrawWarpCurves("Draw Warp Curves", Float) = 0
		[Toggle(APPLY_WARP_CURVES)] _ApplyWarpCurves("Apply Warp Curves", Float) = 0
		
		_Feed("Feed", Range(0, 1)) = 0.5
		_Kill("Kill", Range(0, 1)) = 0.5

		_WarpCurves("Warp Curves", 2D) = "defaulttexture" {}
	}

	CGINCLUDE

	#include "UnityCustomRenderTexture.cginc"
	
	#pragma shader_feature DRAW_PARAMETER_SPACE
	#pragma shader_feature DRAW_WARP_CURVES
	#pragma shader_feature APPLY_WARP_CURVES


	half _Du, _Dv;
	half _Feed, _Kill;

	sampler2D _WarpCurves;
	float4 _WarpCurves_TexelSize;

	// nine point stencil
	half2 laplacian(float2 position) {

		float tw = 1 / _CustomRenderTextureWidth;
		float th = 1 / _CustomRenderTextureHeight;
		float4 duv = float4(tw, th, -tw, 0);

		return
			tex2D(_SelfTexture2D, position - duv.xy).xy * 0.05 +
			tex2D(_SelfTexture2D, position - duv.wy).xy * 0.20 +
			tex2D(_SelfTexture2D, position - duv.zy).xy * 0.05 +
			tex2D(_SelfTexture2D, position + duv.zw).xy * 0.20 -
			tex2D(_SelfTexture2D, position + duv.ww).xy * 1.00 +
			tex2D(_SelfTexture2D, position + duv.xw).xy * 0.20 +
			tex2D(_SelfTexture2D, position + duv.zy).xy * 0.05 +
			tex2D(_SelfTexture2D, position + duv.wy).xy * 0.20 +
			tex2D(_SelfTexture2D, position + duv.xy).xy * 0.05;
	}

	half4 frag(v2f_customrendertexture i) : SV_Target
	{

		half2 q = tex2D(_SelfTexture2D, i.globalTexcoord).xy;

		half2 Duv = laplacian(i.globalTexcoord) * float2(_Du, _Dv);



		float warp0 = tex2D(_WarpCurves, float2(i.globalTexcoord.y, _WarpCurves_TexelSize.y * 0.5)).r;
		float warp1 = tex2D(_WarpCurves, float2(i.globalTexcoord.y, _WarpCurves_TexelSize.y * 1.5)).r;

		float kill = _Kill;
		float feed = _Feed;
		
		#ifdef DRAW_PARAMETER_SPACE

			kill = i.globalTexcoord.x;
			feed = i.globalTexcoord.y;

		#endif

		#ifdef APPLY_WARP_CURVES

			//kill = lerp(warp0, warp1, kill);

		#endif

		kill = lerp(0.045 , 0.07, kill);
		feed = lerp(0.01, 0.07, feed);

		half u = q.x;
		half v = q.y;
		half uvv = u * v * v;

		q += float2(Duv.x - uvv + feed * (1 - u),
					Duv.y + uvv - (kill + feed) * v);

		float b = 0;

		#ifdef DRAW_WARP_CURVES

			b = min(step(warp0, i.globalTexcoord.x), 1 - step(warp1, i.globalTexcoord.x));

		#endif

		return half4(saturate(q), b, 0);
	}

	ENDCG
		

	SubShader
	{
		Cull Off ZWrite Off ZTest Always
		Pass
		{
			Name "Update"
			CGPROGRAM
			#pragma vertex CustomRenderTextureVertexShader
			#pragma fragment frag
			ENDCG
		}
	}
}
