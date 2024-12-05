Shader "Custom/ParticleShader"
{
    Properties     
    {         
        _PointSize("Point size", Float) = 5.0     
    }  

    SubShader{
        Pass{
        Tags { "RenderType"="Opaque" }
        LOD 200
        // Blend SrcAlpha Zero // Pretty nice normal look
        // Blend OneMinusDstColor Zero // I like this one
        Blend OneMinusSrcAlpha SrcAlpha

        CGPROGRAM
// Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct v2f members oritinalPosition)
#pragma exclude_renderers d3d11
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma vertex vert
		#pragma fragment frag

		uniform float _PointSize;
        static const float PI = 3.14159265359;

		#include "UnityCG.cginc"

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 5.0

        struct Particle{
            float3 oritinalPosition; 
            float3 position; 
            float3 velocity; 
            float3 color; 
            float life; 
        }; 

        struct v2f{
            float4 position : SV_POSITION;
            float4 color : COLOR;
            float life : LIFE; 
            float size : PSIZE; 
        };

        StructuredBuffer<Particle> particleBuffer; 

        v2f vert(uint vertex_id : SV_VertexID, uint instance_id : SV_InstanceID)
        {
            v2f o = (v2f)0; 

            // Color
            float life = particleBuffer[instance_id].life; 
            float l = life;  

            float r = particleBuffer[instance_id].color.x; //sin(life * 0.5); 
            float g = particleBuffer[instance_id].color.y;//0.2; //cos(life * 0.5); 
            float b = particleBuffer[instance_id].color.z;//(1 + r) * 0.5; 
            float a = 1; //sin(life * 2 * PI);//1.0f - l; //1.0f; ////sin(life * 0.5); 

            o.color = fixed4(r,g,b,l);//
            //o.color = fixed4(1.0f - l+0.1, l+0.1, 1.0f, 1);

            // Position
            o.position = UnityObjectToClipPos(float4(particleBuffer[instance_id].position, 1.0f)); 
            o.size = _PointSize; 

            return o; 
        }

        float4 frag(v2f i) : COLOR
        {
            return i.color; 
        }
        ENDCG
        }
    }
    FallBack Off

}
