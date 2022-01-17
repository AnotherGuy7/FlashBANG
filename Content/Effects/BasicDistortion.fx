sampler inputTexture;

float4 MainPS(float2 UV : TEXCOORD0) : COLOR0
{
    float2 uv = UV;
    uv.x += sin(UV.y);
    uv.y += cos(UV.x);
    
    return tex2D(inputTexture, uv);
}

technique Techninque1
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 MainPS();
    }
};