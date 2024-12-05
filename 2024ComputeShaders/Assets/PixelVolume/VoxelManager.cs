using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class VoxelManager : MonoBehaviour
{
    // Cells
    public Vector3 voxelResolution; 
    public Vector2 timerMinMax; 
    public float boundMult; 
    public Transform volumeCell; 
    public Mesh mesh; 
    public Material material; 
    public GameObject[,,] allVoxels; 
    [SerializeField]
    public VoxelData[] voxelData; 

    //MaterialPropertyBlock[] propertyBlocks; 
    int count; 

    // Shader
    public int kernelHandle; 
    ComputeBuffer resultBuffer; 
    public ComputeShader computeShader; 
    uint threadGroupSizeX; 
    uint threadGroupSizeY; 
    uint threadGroupSizeZ; 

    int index = 0; 


    // Start is called before the first frame update
    void Awake()
    {
        count = (int)(voxelResolution.x * voxelResolution.y * voxelResolution.z); 
        voxelData = new VoxelData[count]; 
        allVoxels = new GameObject[(int)voxelResolution.x, (int)voxelResolution.y, (int)voxelResolution.z]; 
        /*
        propertyBlocks = new MaterialPropertyBlock[count]; 
        for (int i = 0; i < count; i++)
        {
            propertyBlocks[i] = new MaterialPropertyBlock(); 
        }
        */
        
        InitilizeSystem((int)voxelResolution.x, (int)voxelResolution.y, (int)voxelResolution.z); 
        InitializeShader(); 
    }

    private int[] GetNeighbors(int x, int y, int z)
    {
        List<int> neighbors = new List<int>(); 

        // Add neighbors by checking within bounds
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                for (int dz = -1; dz <= 1; dz++)
                {
                    if (dx == 0 && dy == 0 && dz == 0) continue;  // Skip self

                    int nx = x + dx, ny = y + dy, nz = z + dz;
                    if (nx >= 0 && ny >= 0 && nz >= 0 && nx < x && ny < y && nz < z)
                    {
                        int neighborIndex = nx * y * z + ny * z + nz;
                        neighbors.Add(neighborIndex);
                    }
                }
            }
        }

        return neighbors.ToArray(); 
    }

    public void InitilizeSystem(int width, int height, int depth)
    {

        int index = 0; 
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    // Cell Position
                    Vector3 pos = new Vector3(x,y,z) * boundMult; 
                    // Instantiate Cell
                    GameObject cell = new GameObject($"({x} , {y} , {z}) : Voxel : {index}", typeof(MeshFilter), typeof(MeshRenderer)); 
                    cell.GetComponent<MeshFilter>().sharedMesh = mesh; 
                    cell.GetComponent<MeshRenderer>().material = new Material(material); 
                    cell.transform.position = pos; 
                    cell.transform.parent = transform; 
                    
                    Color color = Random.ColorHSV(); 
                    float r = color.r; 
                    float g = color.g; 
                    float b = color.b; 
                    cell.GetComponent<MeshRenderer>().material.color = color; 

                    allVoxels[x,y,z] = cell;  //= Instantiate(volumeCell, pos,Quaternion.identity, transform); 
                    //allVoxels[x,y,z].name = $"({x} , {y} , {z}) : Voxel : {index}"; 

                    voxelData[index] = new VoxelData
                    {
                        position = pos,
                        r = r, 
                        g = g,
                        b = b, 
                        timer = Random.Range(timerMinMax.x, timerMinMax.y) 
                    };

                    index++; 
                    
                                        /*
                    // Neighbors
                    int [] neighbors = new int[6]; 
                    // bottom (x , y - 1, z) 
                    neighbors[0] = x + ((y-1) * height) + (z * height * depth); 
                    // middle
                    neighbors[1] = (x - 1) + (y * height) + (z * height * depth); 
                    neighbors[2] = (x + 1) + (y * height) + (z * height * depth);
                    neighbors[3] = x + (y * height) + ((z + 1) * height * depth); 
                    neighbors[4] = x + (y * height) + ((z - 1) * height * depth);
                    // top
                    neighbors[5] = 0;//x + ((y+1) * height) + (z * height * depth);

                    // Check if neighbors are within bounds
                    for (int i = 0; i < 6; i++)
                    {
                        if(neighbors[i] < 0 || neighbors[i] >= count)
                        {
                            neighbors[i] = index; // Assign to self if out of bounds
                        }
                    }

                    
                    */
/*

                    // Assign color
                    float r = Random.value; 
                    float g = Random.value; 
                    float b = Random.value; 

                    Color cellColor = new Color(r,g,b); 

                    // Assign Time
                    float t = Random.Range(1.0f,2.0f); // Turn this into a Variable

                    // Create voxel
                    voxelData[index] = new VoxelData(pos,r,g,b,t,
                                                    neighbors[0], neighbors[1], 
                                                    neighbors[2], neighbors[3], 
                                                    neighbors[4], neighbors[5]); 
                    
                    // Update info
                    obj.transform.position = voxelData[index].position; 
                    obj.GetComponent<MeshRenderer>().material.SetColor("_Color", cellColor); 
                    obj.name = $"voxel : {index} "; 
                
*/
                    
                }
            }
        }
    }

    private void InitializeShader(){
        // Initizize Shader
        kernelHandle = computeShader.FindKernel("CSMain"); 
        
        computeShader.GetKernelThreadGroupSizes(kernelHandle, 
                                                out threadGroupSizeX, 
                                                out threadGroupSizeY, 
                                                out threadGroupSizeZ); 
                                                

        computeShader.SetVector("VoxelResolution", voxelResolution); 
        computeShader.SetVector("TimerMinMax", timerMinMax); 

        //int intSize = sizeof(int); 
        //int floatSize = sizeof(float); 

        resultBuffer = new ComputeBuffer(count, System.Runtime.InteropServices.Marshal.SizeOf(typeof(VoxelData)));//new ComputeBuffer(count, floatSize * 7 ); 
        resultBuffer.SetData(voxelData);
        
        computeShader.SetBuffer(kernelHandle, "VoxelBuffer", resultBuffer); 
    }

    private void Update() {
        computeShader.SetFloat("Time", Time.time); 
        computeShader.SetFloat("DeltaTime", Time.deltaTime); 

        //threadGroupSizeX = (uint)Mathf.CeilToInt(voxelResolution.x / 64.0f);
        //threadGroupSizeY = (uint)Mathf.CeilToInt(voxelResolution.y / 1.0f);
        //threadGroupSizeZ = (uint)Mathf.CeilToInt(voxelResolution.z / 1.0f);

        computeShader.Dispatch(kernelHandle, (int)threadGroupSizeX, (int)threadGroupSizeY, (int)threadGroupSizeZ); 

        resultBuffer.GetData(voxelData); 
        
        UpdateVoxels(); 
        

/*
        for (int i = 0; i < count; i++)
        {
            var vMesh = allVoxels[i]; // Mesh information of the voxel
            var vData = voxelData[i]; // properties of the voxel

            float r = vData.r;  
            float g = vData.g; 
            float b = vData.b;  

            Color cellColor = new Color(r,g,b); 

            vMesh.GetComponent<Renderer>().material.color = new Color(r,g,b); 
        }
        */
        
    }

    void UpdateVoxels()
    {
        /*
        if(index > count)
        {
            index = 0; 
        }
        */
        for (int i = 0; i < count; i++)
        {
            Color color = new Color(voxelData[i].r, voxelData[i].g, voxelData[i].b);
            //propertyBlocks[i].SetColor("_Color", color); 

            int z = i / (int)( voxelResolution.x * voxelResolution.y); 
            int remainder = i % (int)( voxelResolution.x * voxelResolution.y);
            int y = remainder / (int)voxelResolution.x; 
            int x = remainder % (int)voxelResolution.x; 

            allVoxels[x,y,z].GetComponent<MeshRenderer>().material.color = color; 
            //allVoxels[x,y,z].GetComponent<Renderer>().SetPropertyBlock(propertyBlocks[i]); 
        }

        //index++; 

        /*
        Debug.Log("updating voxel");
        if(index >= count)
        {
            Debug.Log("index is reset"); 
            index = 0; 
        }
        if(index < count)
        {
            Debug.Log("incex is less than count"); 
            if(voxelData[index].timer <= 0.1f)
            {
                Debug.Log("updating color"); 

                int z = index / (int)( voxelResolution.x * voxelResolution.y); 
                int remainder = index % (int)( voxelResolution.x * voxelResolution.y);
                int y = remainder / (int)voxelResolution.x; 
                int x = remainder % (int)voxelResolution.x; 

                float r = voxelData[index].r; 
                float g = voxelData[index].g;
                float b = voxelData[index].b;
                Color color =  new Color(r,g,b, Random.value); 
                allVoxels[x,y,z].GetComponent<MeshRenderer>().material.color = color; 
            }
        }
        index ++; 
        /*
        else if(voxelData[index].timer <= 0)
        {
            for (int x = 0; x < voxelResolution.x; x++)
            {
                for (int y = 0; y < voxelResolution.y; y++)
                {
                    for (int z = 0; z < voxelResolution.z; z++)
                    {
                        Debug.Log("updating color"); 
                        float r = voxelData[index].r; 
                        float g = voxelData[index].g;
                        float b = voxelData[index].b;
                        Color color =  new Color(r,g,b, Random.value); 
                        allVoxels[x,y,z].GetComponent<MeshRenderer>().material.color = color; 
                        //voxelData[index].r = r; 
                        //voxelData[index].g = g; 
                        //voxelData[index].b = b; 

                        index++; 
                    }
                }
            }
        }
        */
        
        
    }

    private void OnDestroy() {
        if(resultBuffer != null) resultBuffer.Release(); 
    }
}
