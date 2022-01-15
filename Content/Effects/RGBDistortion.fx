sampler inputTexture;

static const float3 Black = float3(0.0f, 0.0f, 0.0f);

float4 MainPS(float2 UV : TEXCOORD0) : COLOR0
{
    float2 uv = UV;
    float4 gamePixelColor = tex2D(inputTexture, uv);
    
    uv.x += sin(UV.y);
    //float displacedR = tex2D(inputTexture, uv + float2(0.02 + uv.y, 0)).r;
    //float displacedG = tex2D(inputTexture, uv + float2(uv.y, 0)).g;
    //float displacedB = tex2D(inputTexture, uv + float2(-0.02 + uv.y, 0)).b;
    
    //displacedR = gamePixelColor.r + 0.1 + sin(UV.y);
    //displacedG = gamePixelColor.g + cos(UV.y);
    //displacedB = gamePixelColor.b + -0.1 + sin(UV.y);
    
    return tex2D(inputTexture, uv);
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