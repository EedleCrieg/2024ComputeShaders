using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

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

    public float speed; 

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

    void Update (){
        computeShader.SetFloat("time", Time.time); 
        computeShader.SetFloat("speed", speed); 
        computeShader.Dispatch(kernelHandle, groupSizeX, 1, 1);

        resultBuffer.GetData(output); 

        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].localPosition = output[i]; 
        }
    }

    private void OnDestroy(){
        resultBuffer.Dispose(); 
    }

}
