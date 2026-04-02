float lightningAngle;
float2 focus;
float2 lightSource;
float2 lensBottom;
float2 lensTop;
float2 screenPos;
float2 screenSize;
float lightningAngle2;

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
float2 RayColision(float2 s1, float2 v1, float2 s2, float2 v2)
{
	float y = (v1.y * v2.y * (s1.x - s2.x) - v2.y * v1.x * s1.y + v1.y * v2.x * s2.y) / (-(v2.y * v1.x) + v1.y * v2.x);
	float x = ((y - s1.y) * v1.x) / v1.y - s1.x;
	return float2(x, y);
}

float4 PixelShaderFunction (float4 sampleColor : COLOR0, float2 uv : TEXCOORD0) : COLOR0
{
    float2 worldUV = screenPos + screenSize * uv;
	
    float2 vec = Rotate(worldUV, lightningAngle);
    float rot = Rotation(vec / length(vec));
    //float2 vecToCenter = Rotate(lensCenter - lightSource, lightningAngle);
	//float rotToCenter = Rotation(vecToCenter / length(vecToCenter));
    //float2 vecToTop = Rotate(lensTop - lightSource, lightningAngle);
    //float2 vecToBottom = Rotate(lensBottom - lightSource, lightningAngle);
    float4 color = float4(0, 0, 0, 0);
    if (rot < lightningAngle2 && -rot < lightningAngle2)
    {
        color = float4(1, 1, 1, 1);
        float rotToTop = Rotation(lensTop / length(lensTop));
        float rotToBottom = Rotation(lensBottom / length(lensBottom));
        color /= length(vec) / 100;
        color *= sqrt(1 - (rot / lightningAngle2) * (rot / lightningAngle2)) * 2;
        if (rot < max(rotToBottom, rotToTop) && rot > min(rotToBottom, rotToTop))
        {
            float2 vect = RayColision(float2(0, 0), vec, lensBottom, lensTop - lensBottom);
            if (vect.x > 0)
            {
                float _length = length(vect);
                float num = 1 - min(length(vect - lensTop), length(vect - lensBottom)) / length(lensTop - lensBottom);
                float width = length(lensTop - lensBottom) * sqrt(1 - num * num);
                if (length(vec) > _length)
                    color *= 1 - width / _length / 2;
            }
        }
    }
    float2 refractionSource = RayColision(float2(0, 0), vec - focus, lensBottom - focus, lensTop - lensBottom) + focus;
    float4 sourceColor = float4(0, 0, 0, 0);
    //if (length(focus - vec) < 30)
    //{
    //    return float4(0, 0, 1, 1);
    //}
    //if (length (refractionSource - vec) < 200)
    //{
    //    return float4(length(refractionSource - vec) / 200, 0, length(refractionSource - vec) / 200, length(refractionSource - vec) / 200);
    //}
    if (refractionSource.y > min(lensBottom.y, lensTop.y) && refractionSource.y < max(lensBottom.y, lensTop.y) && (vec.x > refractionSource.x))
    {
        float newRot = Rotation(refractionSource / length(refractionSource));
        if (newRot < lightningAngle2 && -newRot < lightningAngle2)
        {
            //return float4(1, 0, 0, 1);
            sourceColor = float4(1, 1, 1, 1);
            float rotToTop = Rotation(lensTop / length(lensTop));
            float rotToBottom = Rotation(lensBottom / length(lensBottom));
            sourceColor /= length(refractionSource) / 100;
            sourceColor *= sqrt(1 - (newRot / lightningAngle2) * (newRot / lightningAngle2)) * 2;
            float Width = 0;
            float _length = length(refractionSource);
            float num = 1 - min(length(refractionSource - lensTop), length(refractionSource - lensBottom)) / length(lensTop - lensBottom);
            Width = sqrt(1 - num * num);
            float width = length(lensTop - lensBottom) * Width;
            if (length(vec) > _length && vec.x > 0)
            {
                sourceColor *= width / _length / 2;
                sourceColor *= length(refractionSource - focus) / length(vec - focus);
            }
            else
            {
                sourceColor = float4(0, 0, 0, 0);
            }
            //if (Width >= 0.5)
            //{
            //    return float4(1, 1, 1, 1);

            //}
        }
    }
	return color + sourceColor;
}

technique Technique1
{ 
    pass AutoloadPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}