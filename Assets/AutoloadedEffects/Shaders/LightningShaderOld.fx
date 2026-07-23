float2 left;
float2 leftTop;
float2 leftBottom;
float2 right;
float2 rightTop;
float2 rightBottom;
float widthLeft;
float widthRight;

float2 Rotate(float2 target, float radians)
{
    float num = cos(radians);
    float num2 = sin(radians);
    float2 result = float2(0, 0);
    result.x = target.x * num - target.y * num2;
    result.y = target.x * num2 + target.y * num;
    return result;
}
float Rotation(float2 target)
{
    float rotation = acos(target.x);
    if (target.y < 0)
    {
        rotation *= -1;
    }
    return rotation;
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 uv : TEXCOORD0) : COLOR0
{
        
    //if (length(uv - left) < 0.05)
    //{
    //    return float4(0, 0, 0, 1);
    //}
    //if (length(uv - leftTop) < 0.05)
    //{
    //    return float4(1, 1, 1, 1);
    //}
    //if (length(uv - leftBottom) < 0.05)
    //{
    //    return float4(0, 0, 0, 1);
    //}
    //if (length(uv - right) < 0.05)
    //{
    //    return float4(1, 1, 1, 1);
    //}
    //if (length(uv - rightTop) < 0.05)
    //{
    //    return float4(1, 1, 1, 1);
    //}
    //if (length(uv - rightBottom) < 0.05)
    //{
    //    return float4(1, 1, 1, 1);
    //}
    if (uv.y < 0.5)
    {
        float rotation1 = Rotation((leftTop - left) / length(leftTop - left));
        float2 pos = Rotate(uv - left, -rotation1);
        if (pos.y < 0)
        {
            return float4(0, 0, 0, 0);
        }
        float rotation3 = Rotation((rightTop - right) / length(rightTop - right));
        pos = Rotate(uv - right, -rotation3);
        if (pos.y > 0 && widthRight != 0)
        {
            return float4(0, 0, 0, 0);
        }
    }
    else
    {
        float rotation2 = Rotation((leftBottom - left) / length(leftBottom - left));
        float2 pos = Rotate(uv - left, -rotation2);
        if (pos.y > 0)
        {
            return float4(0, 0, 0, 0);
        }
        float rotation4 = Rotation((rightBottom - right) / length(rightBottom - right));
        pos = Rotate(uv - right, -rotation4);
        if (pos.y < 0 && widthRight != 0)
        {
            return float4(0, 0, 0, 0);
        }
    }
    uv.y = abs(1 - 2 * uv.y);
    uv.x = (uv.x - left.x) / (right.x - left.x);
    uv.y /= widthLeft - (widthLeft - widthRight) * uv.x;
    uv.y *= widthLeft;
    if (uv.y > 1)
    {
        return float4(0, 0, 0, 0);
    }
    return sampleColor * (1 - uv.y * uv.y * uv.y);
}

technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}