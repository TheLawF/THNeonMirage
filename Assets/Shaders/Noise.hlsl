#ifdef NOISE
#define NOISE

#include "HashLib.hlsl"
float voronoi(float2 uv)
{
    float dist = 16;
    float2 intPos = floor(uv);
    float2 fracPos = frac(uv);

    for(int x = -1; x <= 1; x++) //3x3九宫格采样
        {
        for(int y = -1; y <= 1 ; y++)
        {
            //hash22(intPos + float2(x,y)) 相当于offset，定义为在周围9个格子中的某一个特征点
            //float2(x,y) 相当于周围九格子root
            //如没有 offset，那么格子是规整的距离场
            //如果没有 root，相当于在自己的晶格范围内生成特征点，一个格子就有九个“球球”
            float d = distance(hash22(intPos + float2(x,y)) + float2(x,y), fracPos); //fracPos作为采样点，hash22(intPos)作为生成点，来计算dist
            dist = min(dist, d);
        }
        }
    return dist;
}

#endif