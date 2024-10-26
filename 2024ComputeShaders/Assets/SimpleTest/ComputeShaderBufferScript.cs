using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public Transform prefab; 
    public int count; 
    public ComputeShader computeShader; 
    //public RenderTexture renderTexture; 
    public string kernelName; 
    int kernelHandle; 
    public string clearName; 
    int clearHandle; 
    uint threadGroupSizeX; 
    int groupSizeX; 
    public Color clearColor; 

    Transform [] cells; 
    ComputeBuffer resultBuffer; 
    Vector3[] output; 
    
    // Start is called before the first frame update
    void Start()
    {
        kernelHandle = computeShader.FindKernel(kernelName); 
        computeShader.GetKernelThreadGroupSizes(kernelHandle, out threadGroupSizeX, out _, out _); 
        groupSizeX = (int)((count + threadGroupSizeX - 1)/threadGroupSizeX); 

        resultBuffer = new ComputeBuffer(count, sizeof(float) * 3); 
        computeShader.SetBuffer(kernelHandle, "Result", resultBuffer); 
        output = new Vector3[count]; 

        cells = new Transform[count]; 
        for (int i = 0; i < count; i++)
        {
            cells[i] = Instantiate(prefab, transform).transform; 
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
