sampler2D sample : register(s0);

float width;
float height;
int time;
float speed;
float4 color;
float rastyajenie = 1;

float4 PixelShaderFunction(float2 UV : TEXCOORD0) : COLOR0
{
    float k = height / width;
    float2 uv = float2(UV.y, UV.x);
    uv.x *= k;
    if (uv.x < 0.5)
    {
        //return float4(0, 0, 0, 1);
        float2 dir = normalize(uv - float2(0.5, 0.5));
        float value = 1.57079632 - abs(asin(dir.y));
        uv.y = 0.5 - distance(uv, float2(0.5, 0.5));
        uv.x = 0.5 - value * 0.5;
    }
    else if (uv.x > k - 0.5)
    {
        float2 dir = normalize(uv - float2(k - 0.5, 0.5));
        float value = 1.57079632 - abs(asin(dir.y));
        uv.y = 0.5 - distance(uv, float2(k - 0.5, 0.5));
        uv.x = k - 0.5 + value * 0.5;
    }
    uv.y = 1 - abs(uv.y - 0.5) * 2;
    uv.x = (((uv.x + 1.57079632) / rastyajenie + time * speed) % 1);
    
    float4 sampleColor = tex2D(sample, uv);
    return sampleColor * (uv.y + sampleColor.a - 1) * color;
}
technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}