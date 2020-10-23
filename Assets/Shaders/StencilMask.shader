Shader "Stencils/StencilMask"
{
    Properties
    {
    }
    SubShader
    {
        Tags { "Queue"="Geometry-1" }
        ColorMask 0
        ZWrite off
        Stencil
        {
            Ref 1
            Comp always
            Pass replace
        }

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            o.Alpha = fixed4(1, 1, 1, 1);
        }
        ENDCG
    }
    FallBack "Diffuse"
}
