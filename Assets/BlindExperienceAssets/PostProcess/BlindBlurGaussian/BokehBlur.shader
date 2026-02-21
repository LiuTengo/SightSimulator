Shader "BlurShader/BokehBlur"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZWrite Off Cull Off
        Pass
        {
            Name "BokehBlurPass"

            ZWrite Off
            Cull Off
            ZTest Always

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            half4 _GoldenRot;
			float _Iteration;
			float _Radius;
			half2 _PixelSize;
            
            half4 Frag (Varyings input) : SV_Target
            {
                half2x2 rot = half2x2(_GoldenRot);
				half4 accumulator = 0.0;
				half4 divisor = 0.0;
		
				half r = 1.0;
				half2 angle = half2(0.0, _Radius);
		
				for (int j = 0; j < _Iteration; j++)
				{
					r += 1.0 / r;
					angle = mul(rot, angle);
					half4 bokeh = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, float2(input.texcoord + _PixelSize * (r - 1.0) * angle));
					accumulator += bokeh * bokeh;
					divisor += bokeh;
				}
				return accumulator / divisor;
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/Shader Graph/FallbackError"
}