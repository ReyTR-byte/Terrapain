sampler2D inputTexture : register(s0);

float2 screenPosition;
int w;
int h;
float2 center;
float radius1;
float radius2;
float power;
float time;
float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 uv : TEXCOORD0) : COLOR0
{
    if (time == 0)
    {
        return float4(0, 0, 0, 1);
    }
    float2 screen = uv;
    screen.x *= w;
    screen.y *= h;
    screen += screenPosition;
    float dist = distance(screen, center);
    if (dist < radius1)
    {
        return tex2D(inputTexture, uv);
    }
    float p = min((dist - radius1) / (radius2 - radius1), 1.0) * power;
    float2 offset = float2(0, 0);
    screen.x += time * 3;
    screen.y += time * 2;
    offset.x = sin((screen.y + screen.x) * 0.1) * 10;
    offset.y = cos(screen.x * 0.1) * 10;
    offset.x /= w;
    offset.y /= h;
    offset *= p;
    return tex2D(inputTexture, uv + offset);
}
technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}