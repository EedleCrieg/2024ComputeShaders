using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ComputeShaderScript : MonoBehaviour
{
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
    // Start is called before the first frame update
    void Start()
    {
        textureSize = new Vector2(renderTexture.width, renderTexture.height); 
        
        kernelHandle = computeShader.FindKernel("Circumference"); 
        clearHandle = computeShader.FindKernel("Clear"); 

        renderTexture = new RenderTexture((int)textureSize.x, (int)textureSize.y, 0); 
        renderTexture.enableRandomWrite = true; 
        renderTexture.Create(); 

        computeShader.SetTexture(kernelHandle, "Result", renderTexture); 
        computeShader.SetTexture(clearHandle, "Result", renderTexture);
        
        computeShader.SetInt("TextureResolution", (int)textureSize.x); 

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
        computeShader.SetFloat("TimeEllapsed", timeEllapsed); 
        computeShader.SetInt("LineThickness", lineThickness); 
        computeShader.SetVector("ClearColor", clearColor);
        computeShader.SetVector("CircleColor", circleColor);

        // Dispatch the compute shader, with thread groups set to handle the entire texture
        
        computeShader.Dispatch(clearHandle, (int)textureSize.x / 8 , (int)textureSize.y / 8 , 1);
        computeShader.Dispatch(kernelHandle, (int)textureSize.x / 64 , (int)textureSize.y / 1 , 1);

        // Apply the updated data from the pixel buffer to the texture
        Graphics.Blit(renderTexture, dest);
    }
}
