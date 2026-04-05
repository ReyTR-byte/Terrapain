sampler2D texture1 : register(s1);
sampler2D texture2 : register(s2);

float EyeOfCthulhu;
float2 scale;
float2 player;
float4 shineColor;
float4 borderColor;
float4 mainColor;
float alpha;
int time;
float2 Rotate(float2 target, float radians)
{
    float num = cos(radians);
    float num2 = sin(radians);
    float2 result = float2(0, 0);
    result.x = target.x * num - target.y * num2;
    result.y = target.x * num2 + target.y * num;
    return result;
}
float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 uv : TEXCOORD0) : COLOR0
{
    uv -= float2(0.5f, 0.5f);
    float2 notAbsoluteUV = uv;
    uv.y = abs(uv.y);
    float value = 1 - (max(uv.y * 10 - 3.5, 0));
    float4 border = (1 - value * value) * borderColor;
    notAbsoluteUV *= scale;
    uv *= scale;
    float x = player.x;
    float y = abs(player.y);
    float intensity = clamp((150 - distance(x, uv.x) + uv.y) / 50, 0, 1);
    if (intensity > 0 && uv.y > 25)
    {
        intensity *= clamp((350 - y) / 350, 0.4, 1);
        intensity *= cos(uv.y / 3);
        intensity = max(intensity, 0);
    }
    else
    {
        intensity = 0;
    }
    float4 adjustColor = shineColor * intensity;
    float2 target = notAbsoluteUV / scale.y * min((10000 - clamp(distance(notAbsoluteUV,player), 100, 10000)) / 9900, 2);
    float4 anotherColor = float4(0,0,0,0);
    //if (uv.x > EyeOfCthulhu)
    //{
    //    target.x += time / float(-10);
    //    anotherColor = tex2D(texture1, target);
    //    float a = anotherColor.a;
    //    anotherColor *= a;
    //    anotherColor.a = a;
    //}
    //else
    //{
    //    target.x += time / float(-5);
    //    target.y += 0.5;
    //    target.x *= -1;
    //    target.x %= 1;
    //    if (target.x < 0)
    //    {
    //        target.x += 1;
    //    }
    //    anotherColor = tex2D(texture2, target);
    //    float a = anotherColor.a;
    //    anotherColor *= a;
    //    anotherColor.a = a;
    //}
    //anotherColor *= 1 - border.a;
    return (mainColor + border + adjustColor + anotherColor) * alpha;
}

technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}