sampler inputTexture;
sampler lightMask;
sampler blockerMask;
bool applyLighting;

static const float3 Black = float3(0.0f, 0.0f, 0.0f);

float4 MainPS(float2 UV : TEXCOORD0) : COLOR0
{
    float4 gamePixelColor = tex2D(inputTexture, UV);
    if (!applyLighting)
        return gamePixelColor;
    
    float blockerPixelAlpha = tex2D(blockerMask, UV);
    if (blockerPixelAlpha < 0.1f)
        return float4(Black, 1.0f);
    
    //return gamePixelColor * lightStrength * fadeStrength;
    
    float lightingPixelAlpha = clamp(tex2D(lightMask, UV), 0.35f, 1.0f);
    float4 lightingPixelColor = tex2D(lightMask, UV);
    float fadeStrength = 1.2 - (distance(float2(0.5, 0.5), UV) * 1.8f);
    
    float4 newColor = lerp(gamePixelColor, lightingPixelColor, 1.0f - lightingPixelAlpha);
    return newColor * fadeStrength;
}

float rand(float2 co)
{
    return frac(sin(dot(co, float2(12.9898, 78.233))) * 43758.5453);
}

technique Techninque1
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 MainPS();
    }
};