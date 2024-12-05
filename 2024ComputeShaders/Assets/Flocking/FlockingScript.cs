
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class FlockingScript : MonoBehaviour
{
    // Compute Shader variables
    public ComputeShader computeShader; 

    // Boid variables
    public float rotationSpeed = 1f; 
    public float boidSpeed = 1f; 
    public float neighbourDist = 1f; 
    public float boidSpeedVariation = 1f; 
    public Transform boidPrefab; 
    public int boidsCount; 
    public float spawnRadius; 
    public Transform target; 
    public Transform repellent; 

    // Shader info
    public string kernelName; 
    int kernelHandle; 

    // Boid info
    ComputeBuffer boidsBuffer; 
    Boid[] boidsArray; 
    Transform [] boids; 
    int groupSizeX; 
    int numOfBoids; 

    void Start()
    {
        uint x; 
        computeShader.GetKernelThreadGroupSizes(kernelHandle, out x, out _, out _); 
        groupSizeX = Mathf.CeilToInt((float)boidsCount / (float)x); 
        numOfBoids = groupSizeX * (int)x; 

        InitBoids(); 
        InitShader(); 
    }

    private void InitBoids()
    {
        boids = new Transform[numOfBoids]; 
        boidsArray = new Boid[numOfBoids]; 

        for (int i = 0; i < numOfBoids; i++)
        {
            Vector3 pos = transform.position + Random.insideUnitSphere * spawnRadius; 
            boidsArray[i] = new Boid(pos); 
            boids[i] = Instantiate(boidPrefab, pos, Quaternion.identity, this.transform) as Transform; 
            boidsArray[i].direction = boids[i].transform.forward;  
        }
    }


    private void InitShader()
    {
        boidsBuffer = new ComputeBuffer(numOfBoids, 6 * sizeof(float)); 
        boidsBuffer.SetData(boidsArray); 

        computeShader.SetBuffer(kernelHandle, "boidsBuffer", boidsBuffer); 
        computeShader.SetFloat("rotationSpeed", rotationSpeed); 
        computeShader.SetFloat("boidSpeed", boidSpeed); 
        computeShader.SetFloat("boidSpeedVariation", boidSpeedVariation); 
        computeShader.SetVector("flockPosition", target.transform.position); 
        computeShader.SetFloat("neighborDist", neighbourDist); 
        computeShader.SetInt("boidsCount", boidsCount); 
    }

    // Update is called once per frame
    void Update()
    {
        computeShader.SetFloat("time", Time.time); 
        computeShader.SetFloat("deltaTime", Time.deltaTime); 
        computeShader.SetVector("target", target.position); 
        computeShader.SetVector("repellent", repellent.position); 

        computeShader.Dispatch(kernelHandle, groupSizeX, 1, 1); 

        boidsBuffer.GetData(boidsArray);

        for (int i = 0; i < boidsArray.Length; i++)
        {
            boids[i].transform.localPosition = boidsArray[i].position; 

            if(!boidsArray[i].direction.Equals(Vector3.zero))
            {
                boids[i].transform.rotation = Quaternion.LookRotation(boidsArray[i].direction); 
            }
        }
    }

    void OnDestroy() {
        if (boidsBuffer != null)
        {
            boidsBuffer.Release(); 
        }
    }
}
