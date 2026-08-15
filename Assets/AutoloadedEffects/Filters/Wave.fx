sampler2D inputTexture : register(s0);

int w;
int h;

float2 center;
float radius1;
float radius2;
float power;
float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 uv : TEXCOORD0) : COLOR0
{
    float2 screen = uv;
    screen.x *= w;
    screen.y *= h;
    float dist = distance(screen, center);
    if (dist > radius1)
    {
        return tex2D(inputTexture, uv);
    }
    if (dist < radius2)
    {
        return tex2D(inputTexture, uv);
    }
    float p = min((dist - radius2) / (radius1 - radius2), 1.0);
    float2 offset = normalize(center - screen);
    offset = offset * sin(p * 6.28318530718) * power;
    offset.x /= w; 
    offset.y /= h;

    return tex2D(inputTexture, uv + offset);
}
technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}