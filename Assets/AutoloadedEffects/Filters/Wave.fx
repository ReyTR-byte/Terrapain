sampler2D inputTexture : register(s0);
sampler2D offsets : register(s1);

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 uv : TEXCOORD0) : COLOR0
{
    float2 offset = tex2D(offsets, uv).xy;
    //return tex2D(offsets, uv) * 1000; 
    return tex2D(inputTexture, uv + offset);
}
technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}