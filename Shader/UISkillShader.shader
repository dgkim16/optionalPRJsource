Shader "Unlit/UISkillShader"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" {}
        _NoiseTex ("NoiseTex", 2D) = "white" {}
        _Energy ("Energy", Range(0,110)) = 0
        [HDR]_ColorFull ("ColorFull", Color) = (1,1,1,1)
        [HDR]_ColorCharged ("ColorCharged", Color) = (1,1,1,1)
        _ColorUncharged ("ColorUncharged", Color) = (1,1,1,1)
        _Brightness ("Brightness", Range(0,1)) = 0.5
        [MaterialToggle] _isFull ("isFull", Float) = 0
        
    }
    SubShader
    {
        Tags { "RenderType"="CutOut" "Queue"="AlphaTest" }
        LOD 100
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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _NoiseTex;
            float4 _MainTex_ST;
            float _Energy;
            float4 _ColorFull;
            float4 _ColorCharged;
            float4 _ColorUncharged;
            float _Brightness;
            float _isFull;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            // https://stackoverflow.com/questions/13501081/efficient-bicubic-filtering-code-in-glsl
            float4 cubic(float v){
                float4 n = float4(1.0, 2.0, 3.0, 4.0) - v;
                float4 s = n * n * n;
                float x = s.x;
                float y = s.y - 4.0 * s.x;
                float z = s.z - 4.0 * s.y + 6.0 * s.x;
                float w = 6.0 - x - y - z;
                return float4(x, y, z, w) * (1.0/6.0);
            }

            // https://www.youtube.com/watch?v=7fd331zsie0
            float2 randomVector(float2 p) {
                p = p + 0.01;
                float x = dot(p, float2(123.4, 234.5));
                float y = dot(p, float2(234.5, 345.6));
                float2 gradient = float2(x,y);
                gradient = sin(gradient) * 43758.5453123;

                gradient = sin(gradient + _Time/2);
                return gradient;
            }

            float4 perlinNoiseGen(float2 uv) {
                //create grid
                uv = uv * 3;
                float2 i = floor(uv);
                float2 f = frac(uv);
                float3 color = float3(i, 0);
                color = float3(f, 0);

                // find corner coords
                float2 bl = i + float2(0,0);
                float2 br = i + float2(1,0);
                float2 tl = i + float2(0,1);
                float2 tr = i + float2(1,1);

                // find random gradients for each grid corner
                float2 gradBl = randomVector(bl);
                float2 gradBr = randomVector(br);
                float2 gradTl = randomVector(tl);
                float2 gradTr = randomVector(tr);

                // find distance from current pixel to each corner
                float2 distFromPixelToBl = f - float2(0,0);
                float2 distFromPixelToBr = f - float2(1,0);
                float2 distFromPixelToTl = f - float2(0,1);
                float2 distFromPixelToTr = f - float2(1,1);

                // get dot product of gradients + distances
                float dotBl = dot(gradBl, distFromPixelToBl);
                float dotBr = dot(gradBr, distFromPixelToBr);
                float dotTl = dot(gradTl, distFromPixelToTl);
                float dotTr = dot(gradTr, distFromPixelToTr);

                // smooth out f
                f = smoothstep(0,1,f);
                //f = cubic(f);
                //f = quintic(f);

                // linear interpolation between 4 dot products
                float b = lerp(dotBl, dotBr, f.x);
                float t = lerp(dotTl, dotTr, f.x);
                float perlin = lerp(b, t, f.y);


                // return perlin noise
                perlin = clamp(0,1,((perlin + 0.5) / 2));
                color = lerp(_ColorCharged.rgb, _ColorUncharged.rgb, perlin);
                return float4(color, clamp(0.3, 1, perlin));
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                float glow = _Brightness + (_SinTime.w * 0.5);
                float4 col;
                if(distance(i.uv, float2(0.5,0.5)) > 0.5) discard;
                float4 noise = perlinNoiseGen(i.uv);
                if(_Energy/100 >= 1)
                    _isFull = 1;
                if(_isFull > 0.5) {
                    if(distance(i.uv, float2(0.5,0.5)) > 0.5) discard;
                    col = tex2D(_MainTex, i.uv*1.05 - 0.025);
                    if(distance(i.uv, float2(0.5,0.5)) > 0.45 || col.a < 0.5) {
                        col = _ColorCharged * noise * 3;
                        col.a = 1;
                        return col;
                    }       
                    col.rgb = _ColorFull.rgb * glow + 0.1 + noise;         
                    return col;
                }
                col =  tex2D(_MainTex, i.uv);
                
                if(i.uv.y  < _Energy/100) {
                    if(col.a < 0.5) {
                        col.rgba = _ColorFull * noise * 0.6;
                        col.a = noise.r * 0.6;
                        return col;
                    }
                    col.rgb = _ColorCharged.rgb * glow + noise;
                    return col;
                }
                if(col.a < 0.5) {               
                    col.a = lerp(0.7,1, noise.a);
                    col.rgb = _ColorUncharged.rgb * noise.rgb; 
                    return col;
                }
                // 나중에 border 쪽에서 출러인는거 넣기
                col.rgb = _ColorUncharged * glow / 5* noise;
                return col;
            }
            ENDCG
        }
    }
}
