// This defines the path for the shader in Unity's shader selection menu.
// You'll find this shader under "GeminiShaders/Basic/Unlit Color".
Shader "Custom/Basic/WorldShader"
{
     // Properties are simplified again, as animation data is now per-vertex.
    Properties
    {
        _BaseMap("Base Map (Albedo)", 2D) = "white" {}
        _BaseColor("Base Color", Color) = (1,1,1,1)
        _TilesPerRow("Tiles Per Row", Float) = 16
    }

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "RenderType"="Opaque" }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            // Attributes struct now includes a second UV channel (TEXCOORD1) for animation data.
            struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                float2 uv           : TEXCOORD0; // Base quad UVs (0,0 to 1,1)
                float4 uv2          : TEXCOORD1; // Animation data: xy=startTile, z=frameCount, w=frameDuration
            };

            // Varyings struct is updated to pass the animation data to the fragment shader.
            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float2 uv           : TEXCOORD0;
                float3 normalWS     : NORMAL;
                float4 animationData: TEXCOORD1;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float _TilesPerRow;
            CBUFFER_END
            
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            // --- Vertex Shader ---
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                VertexPositionInputs positionInputs = GetVertexPositionInputs(IN.positionOS.xyz);
                VertexNormalInputs normalInputs = GetVertexNormalInputs(IN.normalOS);

                OUT.positionHCS = positionInputs.positionCS;
                OUT.normalWS = normalInputs.normalWS;
                OUT.uv = IN.uv;
                // Pass the animation data directly to the fragment shader.
                OUT.animationData = IN.uv2;

                return OUT;
            }

            // --- Fragment (Pixel) Shader ---
            half4 frag(Varyings IN) : SV_Target
            {
                // --- GPU Atlas & Animation Calculation ---
                float2 startTile = IN.animationData.xy;
                float frameCount = IN.animationData.z;
                float frameDuration = IN.animationData.w;

                // Calculate the current animation frame index on the GPU.
                // _Time.y contains the game time in seconds.
                // This will be 0 for static tiles since (frameDuration > 0) will be false.
                
                float2 finalTile = startTile;
                if (frameDuration > 0 && frameCount > 0)
                {
                    float frame = fmod(floor(_Time.y / frameDuration), frameCount);
                    finalTile = startTile + float2(frame, 0);
                }

                
                float tileSize = 1.0 / _TilesPerRow;
                // Invert the Y coordinate for correct mapping from top-left.
                float2 tileOffset = float2(finalTile.x * tileSize, ((_TilesPerRow - 1) - finalTile.y) * tileSize);
                float epsilon = 0.001;
                float2 atlasUV = (IN.uv * tileSize * (1 - epsilon)) + tileOffset + (tileSize * epsilon * 0.5);

                // --- Final Color Calculation ---
                half4 baseMapColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, atlasUV);
                half4 albedo = baseMapColor * _BaseColor;
                
                Light mainLight = GetMainLight();
                half diffuseTerm = saturate(dot(IN.normalWS, mainLight.direction));
                half3 finalColor = albedo.rgb * mainLight.color * diffuseTerm;

                return half4(finalColor, albedo.a);
            }
            ENDHLSL
        }
    }
}
