int w;
int h;

float2 center;
float radius1;
float radius2;
float power;
float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 uv : TEXCOORD0) : COLOR0
{
    //return float4(1,1,1,1);
    float2 screen = uv;
    screen.x *= w;
    screen.y *= h;
    float dist = distance(screen, center);
    if (dist > radius1)
    {
        return float4(0,0,0,0);
    }
    if (dist < radius2)
    {
        return float4(0,0,0,0);
    }
    float p = min((dist - radius2) / (radius1 - radius2), 1.0);
    float4 offset = float4(normalize(center - screen), 0, 0);
    offset = offset * sin(p * 6.28318530718) * power;
    offset.x /= w; 
    offset.y /= h;
    return offset;// * 1000;
}
technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}