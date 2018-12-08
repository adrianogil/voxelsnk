
// Interesting Material:
// - http://www.iquilezles.org/www/articles/distfunctions/distfunctions.htm
Shader "Voxel/Viz"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _VizSize ("Voxel Viz Size", Vector) = (20, 20, 20, 1) // (R, G, B, A)
        _VoxelSize ("Voxel Size", Vector) = (1, 1, 1, 1) // (R, G, B, A)
        _MinMarchingDistance("Min Marching Distance", Float) = 0.2
        _MainTex("Voxel Data Image", 2D) = "white"
    }
    Subshader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent" }

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "sdf.cginc"

            #define _Steps 80

            float3 _VoxelSize;
            float3 _VizSize;
            float _MinMarchingDistance;

            sampler _MainTex;

            uniform float4 _Color;

            fixed4 simpleLambert (fixed3 normal) {
                fixed3 lightDir = _WorldSpaceLightPos0.xyz; // Light direction
                fixed3 lightCol = _LightColor0.rgb;     // Light color

                fixed NdotL = max(dot(normal, lightDir),0);
                fixed4 c;
                c.rgb = tex2D(_MainTex, float2(0.0, 0.0)) * _Color * lightCol * NdotL;
                c.a = 1;
                return c;
            }

            float map (float3 p)
            {
                float3 q = op_repeat(p, float3(5,5,5));
                // float d = sdf_boxcheap(q, float3(0,0,0), float3(1,1,1));
                float d = sd_torus(q, float2(0.5,0.5));

                return d;
            }

            float3 normal (float3 p)
            {
                const float eps = 0.01;

                return normalize
                (   float3
                    (   map(p + float3(eps, 0, 0)   ) - map(p - float3(eps, 0, 0)),
                        map(p + float3(0, eps, 0)   ) - map(p - float3(0, eps, 0)),
                        map(p + float3(0, 0, eps)   ) - map(p - float3(0, 0, eps))
                    )
                );
            }

            fixed4 renderSurface(float3 p)
            {
                float3 n = normal(p);
                return simpleLambert(n);
            }

            float4 raymarch (float3 position, float3 direction)
            {
                float2 lastuv = float2(-1,-1);

                for (int i = 0; i < _Steps; i++)
                {
                    float3 voxelIndex = floor(position/_VoxelSize);

                    if (voxelIndex.x < 0 ||  voxelIndex.y < 0 || voxelIndex.z < 0 ||
                        voxelIndex.x >= _VizSize.x || voxelIndex.y >= _VizSize.y || voxelIndex.z >= _VizSize.z) {
                        position += _MinMarchingDistance * direction;
                        continue;
                    }

                    // else return float4(1,0,0,1);

                    float2 uv = float2(voxelIndex.x + voxelIndex.z * _VizSize.x, voxelIndex.y);
                    uv = uv / float2(_VizSize.x * _VizSize.z - 1, _VizSize.y - 1);

                    // uv.y = 1 - uv.y;

                    if (length(uv - lastuv) > 0.01)
                    {
                        float4 voxelColor = tex2D(_MainTex, uv);
                        if (voxelColor.a > 0.9) {
                            float3 voxelRelativePos = abs(position - voxelIndex*_VoxelSize - 0.5*_VoxelSize);
                            float darkFactor = 2*length(voxelRelativePos);
                            // float darkFactor = 2*max(voxelRelativePos.x, max(voxelRelativePos.y, voxelRelativePos.z));
                            // float darkFactor = 2*max(0, voxelRelativePos.x+voxelRelativePos.y+voxelRelativePos.z-1);
                            return lerp(voxelColor, float4(0,0,0,1), darkFactor);
                            // return voxelColor;
                        }
                    }

                    lastuv = uv;

                    // float distance = map(position);
                    // if (distance < _MinMarchingDistance)
                    //     return renderSurface(position);

                    position += _MinMarchingDistance * direction;
                }
                return float4(0,0,0,0);
            }

            struct v2f {
                float4 pos : SV_POSITION;   // Clip space
                float3 wPos : TEXCOORD1;    // World position
                float3 lPos : TEXCOORD2;    // World position
            };

            // Vertex function
            v2f vert (appdata_full v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.wPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.lPos = v.vertex.xyz;
                return o;
            }

            // Fragment function
            float4 frag (v2f i) : SV_Target
            {
                float3 worldPosition = i.wPos;
                float3 viewDirection = normalize(i.wPos - _WorldSpaceCameraPos);
                return raymarch (worldPosition, viewDirection);
            }

            ENDCG
        }
    }
}