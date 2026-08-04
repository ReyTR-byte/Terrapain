float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 uv : TEXCOORD0) : COLOR0
{
    uv.y *= 2;
    float dist = distance(uv, float2(0, 1));
    if (dist > 1)
    {
        return float4(0, 0, 0, 0);
    }
    return sampleColor * (1 - dist * dist);
}
technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_3_sw PixelShaderFunction();
    }
}