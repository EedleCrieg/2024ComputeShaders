
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    // Parameters
    public bool RandomPosition = false;
    public bool FourDimensional = false;  

    public Vector3 [] fourBasis; 
    Vector2 cursorPos; 

    const int SIZE_PARTICLE = 13 * sizeof(float); 

    [Header("Wave Properties")]
    public float RadiusShift; // how much the center oscillates 
    public float Amplitude; 
    public float Frequency; 
    public float Speed; 
    public float Ratio; 

    public int particleCount = 1000; 
    public Vector2 lifeSpanRange; 
    public Vector3 resolution; 
    public Vector4 Resolution4D; 
    public float mult; 
    public Material material; 
    public ComputeShader computeShader; 
    [Range(1,10)]
    public int pointSize = 2; 

    int kernelID; 
    ComputeBuffer particleBuffer; 

    int groupSizeX; 

    // Start is called before the first frame update
    void Start()
    {
        if (FourDimensional) particleCount = (int)(Resolution4D.x * Resolution4D.y * Resolution4D.z * Resolution4D.w); 
        else particleCount = (int)(resolution.x * resolution.y * resolution.z); 

         // set up our four dimensional basis
        fourBasis = new Vector3[4];     
        fourBasis[0] = new Vector3(1  , -1  , -1);    // x
        fourBasis[1] = new Vector3(-1 ,  1  , -1);    // y
        fourBasis[2] = new Vector3(-1 , -1  ,  1);    // z
        fourBasis[3] = new Vector3(1  ,  1  ,  1);      // w
       
       
        Init(); 
    }

    void Init()
    {
        
        // initialize the particles
        Particle[] particleArray = new Particle[particleCount]; 

        // Doing a single for loop with an index doesn't give me the result that I want
        if(FourDimensional)
        {
            for (int x = 0; x < Resolution4D.x; x++)
            {
                for (int y = 0; y < Resolution4D.y; y++)
                {
                    for (int z = 0; z < Resolution4D.z; z++)
                    {
                        for (int w = 0; w < Resolution4D.w; w++)
                        {
                            int index = (int)(x + y * Resolution4D.x + z * (Resolution4D.x * Resolution4D.y) + w * (Resolution4D.x * Resolution4D.y * Resolution4D.z)); 
                            Vector3 pos = x * fourBasis[0] + y * fourBasis[1] + z * fourBasis[2] + w * fourBasis[3]; 

                            float xPos = RandomPosition? pos.x * mult + Random.value : pos.x * mult ;
                            float yPos = RandomPosition? pos.y * mult + Random.value : pos.y * mult ;
                            float zPos = RandomPosition? pos.z * mult + Random.value : pos.z * mult ;

                            particleArray[index].position.x = xPos; 
                            particleArray[index].position.y = yPos; //+ Random.value; //xyz.y;
                            particleArray[index].position.z = zPos; //+ Random.value; //xyz.z + 3;

                            particleArray[index].originalPosition.x = xPos; 
                            particleArray[index].originalPosition.y = yPos; 
                            particleArray[index].originalPosition.z = zPos; 
                            // colors
                            particleArray[index].color.x = Random.value; 
                            particleArray[index].color.y = Random.value; 
                            particleArray[index].color.z = Random.value; 

                // Initial life value
                particleArray[index].life = Random.Range(lifeSpanRange.x, lifeSpanRange.y); //Random.value * 3.0f + 1.0f;
                             
                        }
                    }
                }
            }
        }
            // this is not distributing the particles like how I want and I'm not sure way
            /*
            for (int i = 0; i < particleCount; i++)
            {
                 // extract coordinates coefficients from index
                int w = Mathf.FloorToInt(i / (Resolution4D.x * Resolution4D.y * Resolution4D.z)); 
                int z = Mathf.FloorToInt((i % (Resolution4D.x * Resolution4D.y * Resolution4D.z)) / (Resolution4D.x * Resolution4D.y)); 
                int y = Mathf.FloorToInt(i % Resolution4D.x * Resolution4D.y / Resolution4D.x); 
                int x = Mathf.FloorToInt(i % Resolution4D.x); 

                // multiply basis vectors with coefficients
                Vector3 wVect = w * fourBasis[0]; 
                Vector3 zVect = x * fourBasis[3]; 
                Vector3 yVect = y * fourBasis[2]; 
                Vector3 xVect = z * fourBasis[1]; 

                // convert to xyz coordinates
                float xPos = wVect.x + zVect.x + yVect.x + xVect.x; 
                float yPos = wVect.y + zVect.y + yVect.y + xVect.x; 
                float zPos = wVect.z + zVect.z + yVect.z + xVect.z; 

                particleArray[i].position.x = xPos; 
                particleArray[i].position.y = yPos; //+ Random.value; //xyz.y;
                particleArray[i].position.z = zPos; //+ Random.value; //xyz.z + 3;

                particleArray[i].originalPosition.x = xPos; 
                particleArray[i].originalPosition.y = yPos; 
                particleArray[i].originalPosition.z = zPos; 
                // colors
                particleArray[i].color.x = Random.value; 
                particleArray[i].color.y = Random.value; 
                particleArray[i].color.z = Random.value; 

                // Initial life value
                particleArray[i].life = Random.Range(lifeSpanRange.x, lifeSpanRange.y); //Random.value * 3.0f + 1.0f;
            }
            */

        else
        {
            
            for (int i = 0; i < particleCount; i++)
            {
                // position
                int x = Mathf.FloorToInt(i / (resolution.x * resolution.y)); 
                int y = Mathf.FloorToInt((i % (resolution.x * resolution.y)) / resolution.x); 
                int z = Mathf.FloorToInt(i % resolution.x); 

                float xPos = RandomPosition? x * mult + Random.value : x * mult ;
                float yPos = RandomPosition? y * mult + Random.value : y * mult ;
                float zPos = RandomPosition? z * mult + Random.value : z * mult ;

                particleArray[i].position.x = xPos; 
                particleArray[i].position.y = yPos; //+ Random.value; //xyz.y;
                particleArray[i].position.z = zPos; //+ Random.value; //xyz.z + 3;

                particleArray[i].originalPosition.x = xPos; 
                particleArray[i].originalPosition.y = yPos; 
                particleArray[i].originalPosition.z = zPos; 
                // colors
                particleArray[i].color.x = Random.value; 
                particleArray[i].color.y = Random.value; 
                particleArray[i].color.z = Random.value; 

                // Initial life value
                particleArray[i].life = Random.Range(lifeSpanRange.x, lifeSpanRange.y); //Random.value * 3.0f + 1.0f;
            }
        }

        // create compute buffer
        particleBuffer = new ComputeBuffer(particleCount, SIZE_PARTICLE); 

        particleBuffer.SetData(particleArray); 

        // set kernel
        kernelID = computeShader.FindKernel("CSMain"); 

        uint threadsX; 
        computeShader.GetKernelThreadGroupSizes(kernelID, out threadsX, out _, out _); 
        groupSizeX = Mathf.CeilToInt((float)particleCount / (float)threadsX); 

        // bind the compute buffer to the shader and compute shader
        computeShader.SetBuffer(kernelID, "particleBuffer", particleBuffer); 
        material.SetBuffer("particleBuffer", particleBuffer); 

        material.SetInt("_PointSize", pointSize); 

        // set variables in compute shader
        computeShader.SetFloat("lifeSpanMin", lifeSpanRange.x); 
        computeShader.SetFloat("lifeSpanMax", lifeSpanRange.y); 
        computeShader.SetVector("Resolution", resolution); 
        
    }


    void OnGUI() 
    {
        Vector3 p = new Vector3(); 
        Camera c = Camera.main; 
        Event e = Event.current; 
        Vector2 mousePos = new Vector2(); 

        // Get the position from Event. 
        // Note that they position from Event is inverted.
        mousePos.x = e.mousePosition.x; 
        mousePos.y = c.pixelHeight - e.mousePosition.y; 

        p = c.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, c.nearClipPlane + 14)); 

        cursorPos.x = p.x; 
        cursorPos.y = p.y; 
    }
    

    private void OnRenderObject() {
        material.SetPass(0); 
        Graphics.DrawProceduralNow(MeshTopology.Points, 1, particleCount); 
    }

    void OnDestroy()
    {
        if (particleBuffer != null) particleBuffer.Release(); 
    }

    // Update is called once per frame
    void Update()
    {
        float[] mousePosition2D = {cursorPos.x, cursorPos.y}; 

        // Send data to compute shader
        computeShader.SetFloat("deltaTime", Time.deltaTime); 
        computeShader.SetFloat("Time", Time.time); 
        computeShader.SetFloats("mousePosition", mousePosition2D); 
        computeShader.SetFloat("Amplitude", Amplitude); 
        computeShader.SetFloat("Frequency", Frequency); 
        computeShader.SetFloat("Speed", Speed); 
        computeShader.SetFloat("Ratio", Ratio); 
        computeShader.SetFloat("RadiusShift", RadiusShift); 
        

        // Update the particles
        computeShader.Dispatch(kernelID, groupSizeX, 1, 1); 
    }


}
