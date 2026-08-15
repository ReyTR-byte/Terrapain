sampler2D sample : register(s0);

int w;
int h;

float radius1;
float radius2;
float time;
float2 center;
float4 color2;

float Rotation(float2 target)
{
    float rotation = acos(target.x);
    if (target.y < 0)
    {
        rotation *= -1;
    }
    return rotation;
}

float4 PixelShaderFunction(float2 uv : TEXCOORD0, float4 color : COLOR0) : COLOR0
{
    float2 screen = uv;
    screen.x *= w;
    screen.y *= h;
    screen -= center;
    float len = length(screen);
    if (len < radius1)
    {
        return float4(0, 0, 0, 0);
    }
    float2 newUV;
    newUV.x = (Rotation(normalize(screen)) / 6.2831853 + 0.5 + time * 0.7) % 1;
    newUV.y = (time * 1.5) % 1;
    float4 sampleColor = tex2D(sample, newUV);
    float height = 1 - max((len - radius2) / (radius1 - radius2) * sampleColor.a * sampleColor.a, 0);
    screen.y += cos(time * 30) * 20;
    screen.x += sin(time * 15) * 30;
    float value = (sin(screen.x * 0.008 + screen.y * 0.018 + time * 20) + 1) / 4;
    value += (cos(screen.x * 0.0175 - screen.y * 0.015 + time * 10) + 1) / 4;
    value = value * 0.5 + sampleColor.a * 0.5;
    color = color * value + color2 * (1 - value);
    return sampleColor * (height - 1) + sampleColor.a * color;
}

technique Technique1
{
    pass AutoloadPass
    {
        //VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}