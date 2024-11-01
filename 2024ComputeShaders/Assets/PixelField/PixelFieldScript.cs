using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class PixelFieldScript : MonoBehaviour
{
    public string kernelName; 
    public int kernelHandle; 
     int screenWidth; 
     int screenHeight; 
     uint threadGroupSizeX; 
     uint threadGroupsizeY; 
     int groupSizeX; 
     public bool average = false; 
    public bool interpolate = false; 
    public bool pingPong = false; 
    public bool selectColor = false; 
     ComputeBuffer resultBuffer; 
     PixelData[] output; 
    float time; 
    public Vector2 timeRange; 
    public ComputeShader computeShader; 
    public RenderTexture renderTexture; // dimensions defined through texture


    public Color color1; 
    public Color color2; 



    void Start()
    {
         // Screen dimensions
        screenWidth = Screen.width; 
        screenHeight = Screen.height; 

        int count = screenHeight * screenWidth;

        int index = 0;  

        // Define data
        PixelData[] pixelDataArray = new PixelData[count]; 
       for (int y = 0; y < screenHeight; y++)
       {
            for (int x = 0; x < screenWidth; x++)
            {
                //int index = x + y;// * screenWidth;
                int[] neighbors = new int[8];

                // Calculate indices of adjacent pixels
                neighbors[0] = (x - 1) + (y - 1) * screenWidth;
                neighbors[1] = x + (y - 1) * screenWidth;
                neighbors[2] = (x + 1) + (y - 1) * screenWidth;
                neighbors[3] = (x - 1) + y * screenWidth;
                neighbors[4] = (x + 1) + y * screenWidth;
                neighbors[5] = (x - 1) + (y + 1) * screenWidth;
                neighbors[6] = x + (y + 1) * screenWidth;
                neighbors[7] = (x + 1) + (y + 1) * screenWidth;

                // Ensure neighbors are within bounds
                for (int i = 0; i < 8; i++)
                {
                    if (neighbors[i] < 0 || neighbors[i] >= count)
                    {
                        neighbors[i] = index; // Assign to self if out of bounds
                    }
                }

                // Colors
                float r; 
                float g; 
                float b; 
                // Select Color
                if (selectColor)
                {
                    Color selectedColor = Random.value >= 0.5f? color1: color2; 

                     r = selectedColor.r; 
                     g = selectedColor.g; 
                     b = selectedColor.b; 
                }
                else{
                // Randomize colors
                     r = Random.value; 
                     g = Random.value; 
                     b = Random.value;
                }
                 
                float t = Random.Range(timeRange.x, timeRange.y); 
                pixelDataArray[index] = new PixelData( neighbors[0],  neighbors[1],  neighbors[2],  neighbors[3],  neighbors[4],  neighbors[5],  neighbors[6],  neighbors[7], 
                                                        t,  r, g, b, 
                                                        0.0f, 0, 0, Vector3.one, Vector3.one ); 
                index ++; 
            }
       }

        // Render texture
        renderTexture = new RenderTexture(screenWidth, screenHeight, 0); 
        renderTexture.enableRandomWrite = true; 
        renderTexture.Create(); 

        // Compute shader
        kernelHandle = computeShader.FindKernel(kernelName);

        // Buffer
        computeShader.GetKernelThreadGroupSizes(kernelHandle, out threadGroupSizeX, out _, out _); 
        groupSizeX = (int)((count + threadGroupSizeX - 1)/threadGroupSizeX); 

        int intSize = sizeof(int); 
        int floatSize = sizeof(float);  

        resultBuffer = new ComputeBuffer(count, intSize * 10 + floatSize * 11); 
        resultBuffer.SetData(pixelDataArray); 
        float timeRangeMin = timeRange.x; 
        float timeRangeMax = timeRange.y; 
        computeShader.SetFloat("TimeRangeMin", timeRangeMin); 
        computeShader.SetFloat("TimeRangeMax", timeRangeMax); 
        computeShader.SetBool("Average", average); 
        computeShader.SetBool("Interpolate", interpolate); 
        computeShader.SetBool("PingPong", pingPong); 
        computeShader.SetInt("resolutionX", screenWidth); 
        computeShader.SetInt("resolutionY", screenHeight); 
        computeShader.SetBuffer(kernelHandle, "PixelBuffer", resultBuffer); 
        computeShader.SetTexture(kernelHandle, "Result", renderTexture); 
        
        output = new PixelData[count]; 
    }

    // Update is called once per frame
    void Update()
    {       
        computeShader.SetFloat("Time", Time.time);  
        computeShader.Dispatch(kernelHandle, screenWidth / 8, screenHeight / 8 , 1); 
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        resultBuffer.GetData(output); 
        Graphics.Blit(renderTexture, dest); 
    }

     void OnDestroy()
    {
        if (resultBuffer != null)
        {
            resultBuffer.Release();
        }
    }
}
