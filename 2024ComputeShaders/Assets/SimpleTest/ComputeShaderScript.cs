using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using Debug = UnityEngine.Debug;

public class ComputeShaderScript : MonoBehaviour
{
    public string kernelName; 
    public ComputeShader computeShader; 
    public RenderTexture renderTexture; 
    public int radius = 200; 

    [Range(0,10)]
    public int lineThickness = 1; 

    public Color clearColor; 
    public Color circleColor; 

    float timeEllapsed = 0.0f; 
    Stopwatch stopwatch; 
    Vector2 textureSize; 
    int kernelHandle; 
    int clearHandle; 

// struct of objects    
    struct Circle{
        public Vector2 origin; 
        public Vector2 velocity; 
        public float radius; 
    }
// data array
Circle[] data; 
ComputeBuffer buffer; 

    void Start()
    { 
        // render texture
        textureSize = new Vector2(renderTexture.width, renderTexture.height); 

        renderTexture = new RenderTexture((int)textureSize.x, (int)textureSize.y, 0); 
        renderTexture.enableRandomWrite = true; 
        renderTexture.Create(); 

        // compute shader
        kernelHandle = computeShader.FindKernel(kernelName); 
        clearHandle = computeShader.FindKernel("Clear");

        computeShader.SetTexture(kernelHandle, "Result", renderTexture); 
        computeShader.SetTexture(clearHandle, "Result", renderTexture);
        
        computeShader.SetInt("TextureResolution", (int)textureSize.x); 

        // buffer 
        uint threadGroupSizeX; 
        // compute threadgroupsize
        computeShader.GetKernelThreadGroupSizes(kernelHandle, out threadGroupSizeX, out _, out _); 
        Debug.Log(threadGroupSizeX); 
        int total = (int)threadGroupSizeX * (int)textureSize.x; 
        data = new Circle[total]; 

        float speed = 100; 
        float halfspeed = speed * 0.5f; 
        float minRadius = 10.0f; 
        float maxRadius = 30.0f; 
        float radiusRange = maxRadius - minRadius; 

        for (int i = 0; i < total; i++)
        {
            Circle circle = data[i]; 
            circle.origin.x = Random.value * textureSize.x; 
            circle.origin.y = Random.value * textureSize.y; 
            circle.velocity.x = (Random.value * speed) - halfspeed; 
            circle.velocity.y = (Random.value * speed) - halfspeed; 
            circle.radius = Random.value * radiusRange + minRadius; 
            data[i] = circle; 
        }

        // set compute buffer data
        int stride = (2+2+1) * sizeof(float); 
        buffer = new ComputeBuffer(data.Length, stride); 
        buffer.SetData(data); 
        computeShader.SetBuffer(kernelHandle,"circlesBuffer", buffer); 

        // stopwatch
        stopwatch = new Stopwatch(); 
        stopwatch.Start(); 
        
    }

    private void Update() {
        //timeEllapsed = stopwatch.ElapsedMilliseconds; 
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (renderTexture == null)
        {
            renderTexture = new RenderTexture((int)textureSize.x, (int)textureSize.y, 1); 
            renderTexture.enableRandomWrite = true; 
            renderTexture.Create(); 
        }

        computeShader.SetInt("Radius", radius); 
        computeShader.SetFloat("TimeEllapsed", Time.time); 
        computeShader.SetInt("LineThickness", lineThickness); 
        computeShader.SetVector("ClearColor", clearColor);
        computeShader.SetVector("CircleColor", circleColor);

        // Dispatch the compute shader, with thread groups set to handle the entire texture
        computeShader.Dispatch(clearHandle, (int)textureSize.x / 8 , (int)textureSize.y / 8 , 1);
        computeShader.Dispatch(kernelHandle, (int)textureSize.x / 128 , (int)textureSize.y / 1 , 1);

        // Apply the updated data from the pixel buffer to the texture
        Graphics.Blit(renderTexture, dest);
    }
}
