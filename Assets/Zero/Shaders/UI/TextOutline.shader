Shader "Zero/UI/TextOutline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" { }
        //外边框颜色
        _OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float3 vertex : POSITION;                
                fixed4 color : COLOR;                
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            //边框颜色
            fixed4 _OutlineColor;            

            v2f vert(appdata v)
            {
                v2f o;                
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;                
                o.uv = TRANSFORM_TEX(v.uv.xy, _MainTex);
                return o;
            }
            
            #define BIAS 0.5

            static const float2 dirList[9] = {
                float2(-BIAS, -BIAS),
                float2(0, -BIAS),
                float2(BIAS, -BIAS),
                float2(-BIAS, 0),
                float2(0, 0),
                float2(BIAS, 0),
                float2(-BIAS, BIAS),
                float2(0, BIAS),
                float2(BIAS, BIAS)
            };

            //得到每个像素点的alpha值
            half getDirPosAlpha(float index, float2 xy)
            {
                float2 curPos = xy;
                float2 dir = dirList[index];
                float2 dirPos = curPos + dir * _MainTex_TexelSize.xy;
                return tex2D(_MainTex, dirPos).a;
            };

            //
            float mixAlpha(float2 xy)
            {
                float a = 0;
                float index = 0;
                a += getDirPosAlpha(index, xy);
                a += getDirPosAlpha(index++, xy);
                a += getDirPosAlpha(index++, xy);
                a += getDirPosAlpha(index++, xy);
                a += getDirPosAlpha(index++, xy);
                a += getDirPosAlpha(index++, xy);
                a += getDirPosAlpha(index++, xy);
                a += getDirPosAlpha(index++, xy);
                a += getDirPosAlpha(index++, xy);
                a = step(a, 9) * a;
                a = clamp(0, 1, a);
                return a;
            }


            //两个颜色叠加, 参考lerp
            fixed4 blendColor(fixed4 colorBottom, fixed4 colorTop)
            {
                float a = colorTop.a + colorBottom.a * (1 - colorTop.a);
                fixed4 newCol = fixed4(0, 0, 0, a);
                newCol.rgb = (colorTop.rgb * colorTop.a + colorBottom.rgb * colorBottom.a * (1 - colorTop.a)) / a;
                return newCol;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float uvAlpha = tex2D(_MainTex, i.uv).a;
                
                fixed4 col = i.color;
                col.a = uvAlpha * i.color.a;

                float4 outlineCol = _OutlineColor;
                // 计算一个alpha系数，为了让镂空时的内边缘平滑
                outlineCol.a = lerp(1 - uvAlpha, outlineCol.a, smoothstep(0, 1, i.color.a - 0.65));
                // 推算边框的alpha值，为了让描边的外边缘平滑
                outlineCol.a = mixAlpha(i.uv) * _OutlineColor.a * outlineCol.a;
                col = blendColor(outlineCol, col);
                clip(col.a - 0.001);
                return col;
            }
            
            ENDCG
        }
    }
}
