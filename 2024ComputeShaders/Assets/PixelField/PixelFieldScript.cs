using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class PixelFieldScript : MonoBehaviour
{
    public Shader blitWithAlphaShader;
    public string kernelName; 
    public int kernelHandle; 
    //public int drawCircleHandle; 
    public Color penColor; 
    public Color penEdgeColor; 
    public int penRadius; 

    public Vector2 randomNeighborDistance; 
     int screenWidth; 
     int screenHeight; 
     uint threadGroupSizeX; 
     uint threadGroupsizeY; 
     int groupSizeX; 
     public bool randomNeighbors = false; 
     public bool average = false; 
     public bool weighted = false; 
     public bool seperateRGB = false; 
    public bool interpolate = false; 
    public bool pingPong = false; 
    public bool selectColor = false; 
    public bool randomCursorColor = false; 
    public Vector2 timeRange; 
     ComputeBuffer resultBuffer; 
     PixelData[] output; 

    public ComputeShader computeShader; 
    public RenderTexture renderTexture; // dimensions defined through texture

    bool pressed = false; 

    //public ComputeShader circlesComputeShader; 
    //public RenderTexture circlesRenderTexture; 
    private RenderTexture finalRenderTexture; 

    public Vector2 mousePosition; 

    public Color color1; 
    public Color color2; 



    void Start()
    {
        Cursor.visible = false;
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

                if(!randomNeighbors)
                {
                    // Calculate indices of adjacent pixels
                    neighbors[0] = (x - 1) + (y - 1) * screenWidth;
                    neighbors[1] = x + (y - 1) * screenWidth;
                    neighbors[2] = (x + 1) + (y - 1) * screenWidth;
                    neighbors[3] = (x - 1) + y * screenWidth;
                    neighbors[4] = (x + 1) + y * screenWidth;
                    neighbors[5] = (x - 1) + (y + 1) * screenWidth;
                    neighbors[6] = x + (y + 1) * screenWidth;
                    neighbors[7] = (x + 1) + (y + 1) * screenWidth;
                }
                if(randomNeighbors)
                {
                    // completly random neighbors
                    /*
                    neighbors[0] = Random.Range(0, count); 
                    neighbors[1] = Random.Range(0, count); 
                    neighbors[2] = Random.Range(0, count); 
                    neighbors[3] = Random.Range(0, count); 
                    neighbors[4] = Random.Range(0, count); 
                    neighbors[5] = Random.Range(0, count); 
                    neighbors[6] = Random.Range(0, count); 
                    neighbors[7] = Random.Range(0, count); 
                    */

                    // local random neighbors
                    neighbors[0] = x - Random.Range((int)randomNeighborDistance.x,(int)randomNeighborDistance.y) - 1 + (y - Random.Range((int)randomNeighborDistance.x,(int)randomNeighborDistance.y)) * screenWidth;
                    neighbors[1] = x + (y - Random.Range((int)randomNeighborDistance.x,(int)randomNeighborDistance.y)) * screenWidth;
                    neighbors[2] = (x + Random.Range((int)randomNeighborDistance.x,(int)randomNeighborDistance.y)) + (y - Random.Range((int)randomNeighborDistance.x,(int)randomNeighborDistance.y)) * screenWidth;
                    neighbors[3] = (x - Random.Range((int)randomNeighborDistance.x,(int)randomNeighborDistance.y)) + y * screenWidth;
                    neighbors[4] = (x + Random.Range((int)randomNeighborDistance.x,(int)randomNeighborDistance.y)) + y * screenWidth;
                    neighbors[5] = (x - Random.Range((int)randomNeighborDistance.x,(int)randomNeighborDistance.y)) + (y + Random.Range((int)randomNeighborDistance.x,(int)randomNeighborDistance.y)) * screenWidth;
                    neighbors[6] = x + (y + Random.Range((int)randomNeighborDistance.x,(int)randomNeighborDistance.y)) * screenWidth;
                    neighbors[7] = (x + Random.Range((int)randomNeighborDistance.x,(int)randomNeighborDistance.y)) + (y + Random.Range((int)randomNeighborDistance.x,(int)randomNeighborDistance.y)) * screenWidth;
                }
                

                // Ensure neighbors are within bounds
                for (int i = 0; i < 8; i++)
                {
                    if (neighbors[i] < 0 || neighbors[i] >= count)
                    {
                        if(randomNeighbors) neighbors[i] = Random.Range(0,count); 
                        else neighbors[i] = index; // Assign to self if out of bounds
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
        // Background 
        renderTexture = new RenderTexture(screenWidth, screenHeight, 0); 
        renderTexture.enableRandomWrite = true; 
        renderTexture.Create(); 
        // Circles
        /*
        circlesRenderTexture = new RenderTexture(screenWidth, screenHeight, 0, RenderTextureFormat.ARGB32); 
        circlesRenderTexture.enableRandomWrite = true; 
        circlesRenderTexture.Create(); 
        */
        // Final
        finalRenderTexture = new RenderTexture(screenWidth, screenHeight, 0); 
        finalRenderTexture.enableRandomWrite = true; 
        finalRenderTexture.Create(); 

        // Compute shader
        kernelHandle = computeShader.FindKernel(kernelName);
        //drawCircleHandle = circlesComputeShader.FindKernel("DrawCirlce"); 
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
        computeShader.SetBool("Weighted", weighted); 
        computeShader.SetBool("Interpolate", interpolate); 
        computeShader.SetBool("PingPong", pingPong); 
        computeShader.SetBool("SeperateRGB", seperateRGB); 
        computeShader.SetBool("RandomCursorColor", randomCursorColor); 
        computeShader.SetInt("resolutionX", screenWidth); 
        computeShader.SetInt("resolutionY", screenHeight); 
        
        computeShader.SetBuffer(kernelHandle, "PixelBuffer", resultBuffer); 

        computeShader.SetTexture(kernelHandle, "Result", renderTexture); 
        //circlesComputeShader.SetTexture(drawCircleHandle, "Result", circlesRenderTexture); 


        output = new PixelData[count]; 
    }

    // Update is called once per frame
    void Update()
    {       
        
        if(Input.GetMouseButton(0)) pressed = true; 
        else pressed = false; 
 
        computeShader.SetFloat("Time", Time.time);  
        computeShader.SetVector("penColor", penColor); 
        computeShader.SetVector("penEdgeColor", penEdgeColor); 
        computeShader.SetInt("penRadius", penRadius);
        computeShader.SetBool("Pressed", pressed); 
        computeShader.SetVector("mousePosition", Input.mousePosition); 

    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        resultBuffer.GetData(output); 

        computeShader.Dispatch(kernelHandle, screenWidth / 8, screenHeight / 8 , 1); 
        //circlesComputeShader.Dispatch(drawCircleHandle, screenWidth / 8, screenHeight / 8 , 1);  


        Graphics.Blit(renderTexture, dest); 
        //Graphics.Blit(circlesRenderTexture,finalRenderTexture, new Material(blitWithAlphaShader)); 

        //Graphics.Blit(finalRenderTexture, dest); 
    }

     void OnDestroy()
    {
        if(resultBuffer != null) resultBuffer.Release();
        if(renderTexture != null) renderTexture.Release(); 
        //if(circlesRenderTexture != null) circlesRenderTexture.Release(); 
    }
}
