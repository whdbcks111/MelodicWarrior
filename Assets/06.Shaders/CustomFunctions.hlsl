float4 GetColor(UnityTexture2D tex, float2 uv, float2 offset)
{
    float4 color = SAMPLE_TEXTURE2D(
	    tex.tex,
	    tex.samplerstate,
	    tex.GetTransformedUV(uv + offset)
    );
    return color;
}

float GetDistantAlpha(UnityTexture2D tex, float2 uv, float angle, float distance)
{
    float rad = angle / 180.0 * 3.1415926536;
    float2 offset = float2(cos(rad), sin(rad)) * distance;
    offset.x *= tex.texelSize.x;
    offset.y *= tex.texelSize.y;
    return GetColor(tex, uv, offset).a;
}

void HasVisiblePixelAround_float(UnityTexture2D tex, float2 uv, float distance, out bool Out)
{
    float4 curColor = GetColor(tex, uv, float2(0, 0));
    float result = 0.0;
    
    
    int angleCount = 32;
    for (int i = 0; i < angleCount; i++)
    {
        int splitCount = 2;
        for (int j = 1; j < splitCount + 1; j++)
        {
            result += GetDistantAlpha(tex, uv, 360.0 / angleCount * i, distance / splitCount * j);
        }
    }
    
    Out = curColor.a < 0.1f && result > 0;
}