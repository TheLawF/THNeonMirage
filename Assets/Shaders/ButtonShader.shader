Shader "Custom/ButtonShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _Radius("Radius", Range(0, 0.5)) = 0.1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 100
        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };
            
            sampler2D _MainTex;
            sampler2D _MuskTex;
            fixed4 _Color;
            float _Radius;

            
            float roundedBoxSDF(float2 p, float2 b, float r)
            {
                float2 q = abs(p) - b + r;
                return length(max(q, 0.0)) + min(max(q.x, q.y), 0.0) - r;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv * 2.0 - 1.0; // 把 UV 归一化到 -1~1
                float2 rectSize = float2(0.9, 0.4); // 按钮的大小
                float sdf = roundedBoxSDF(uv, rectSize, _Radius);

                if (sdf > 0.0) discard; // 超出圆角范围则丢弃像素

                fixed4 texColor = tex2D(_MainTex, i.uv);
                return fixed4(texColor.rgb * i.color.rgb, texColor.a);
            }
            
            ENDCG
        }
    }
    FallBack "Diffuse"
}
