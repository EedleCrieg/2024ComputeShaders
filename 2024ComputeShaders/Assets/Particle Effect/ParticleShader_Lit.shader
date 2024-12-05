Shader "Custom/ParticleShader_Lit"
{
    Properties     
    {         
        _PointSize("Point size", Float) = 5.0     
        _Color("Color", Color) = (1, 1, 1, 1) // Base color for particles
    }  

    SubShader
    {
        Tags { "Queue"="Geometry" "RenderType"="Opaque" }
        LOD 200
        Blend Off // Opaque particles with lighting

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            uniform float _PointSize;
            uniform float4 _Color;

            #include "UnityCG.cginc"
            #include "Lighting.cginc" // Include Unity lighting functions

            #pragma target 3.0

            struct Particle
            {
                float3 originalPosition; 
                float3 position; 
                float3 velocity; 
                float3 color; 
                float life; 
            }; 

            struct v2f
            {
                float4 position : SV_POSITION;
                float3 worldNormal : NORMAL;
                float3 worldPos : TEXCOORD0;
                float4 color : COLOR;
                float size : PSIZE; 
            };

            StructuredBuffer<Particle> particleBuffer; 

            v2f vert(uint vertex_id : SV_VertexID, uint instance_id : SV_InstanceID)
            {
                v2f o = (v2f)0;

                // Assign color based on particle's data
                float3 particleColor = particleBuffer[instance_id].color;
                o.color = float4(particleColor, 1); 

                // Calculate position
                float4 worldPos = float4(particleBuffer[instance_id].position, 1.0f);
                o.position = UnityObjectToClipPos(worldPos);
                o.size = _PointSize; 

                // World-space normal calculation
                o.worldNormal = normalize(mul((float3x3)unity_WorldToObject, particleBuffer[instance_id].position));
                o.worldPos = worldPos.xyz;

                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                // Sample ambient lighting
                float3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;

                // Compute diffuse lighting from the main directional light
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz); 
                float3 normal = normalize(i.worldNormal);
                float NdotL = max(dot(normal, lightDir), 0.0);
                float3 diffuse = _LightColor0.rgb * NdotL;

                // Apply both ambient and diffuse lighting to the base color
                float4 finalColor = i.color * _Color; // Base color
                finalColor.rgb *= ambient + diffuse; // Apply lighting

                return finalColor;
            }
            ENDCG
        }
    }
    FallBack Off
}