using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDistribution : MonoBehaviour
{
    public Vector3 [] basis; 
    [SerializeField]
    public int [] resolution; 
    // Start is called before the first frame update
    void Start()
    {
        int count = 1; 
        // define the count based off of the resolution 
        foreach (int num in resolution)
        {
            count *= num; 
        }

        Debug.Log(count); 

        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube); 

        for (int x = 0; x < resolution[0]; x++)
        {
            for (int y = 0; y < resolution[1]; y++)
            {
                for (int z = 0; z < resolution[2]; z++)
                {
                    for (int w = 0; w < resolution[3]; w++)
                    {
                        int index = x + y * resolution[0] + z * (resolution[0] * resolution[1]) + w * (resolution[0] * resolution[1] * resolution[2]); 
                        Vector3 pos = x * basis[0] + y * basis[1] + z * basis[2] + w * basis[3]; 

                        Instantiate(obj, pos, Quaternion.identity); 
                        obj.name = $"index : {index} : Position {x} , {y} , {z} , {w}"; 
                    }
                }
            }
        }

/*
        for (int i = 1; i < count; i++)
        {
            // coefficients for R4
            int w = i / (resolution[1] * resolution[2] * resolution[3]); 
            int z = (i % (resolution[1] * resolution[2] * resolution[3])) / (resolution[1] * resolution[2]); 
            int y = i % resolution[1] * resolution[2] / resolution[1]; 
            int x = i % resolution[1]; 

            Debug.Log($"{w}, {x}, {y}, {z}"); 

            // vectors in R4
            Vector3 wVect = w * basis[0]; 
            Vector3 xVect = x * basis[1]; 
            Vector3 yVect = y * basis[2]; 
            Vector3 zVect = z * basis[3]; 

            // coefficients mapped to R3
            float xPos = wVect.x + xVect.x + yVect.x + zVect.x; 
            float yPos = wVect.y + xVect.y + yVect.y + zVect.y; 
            float zPos = wVect.z + xVect.z + yVect.z + zVect.z; 

            // vector3 in R3
            Vector3 pos = wVect + xVect + yVect + zVect; //new Vector3(xPos, yPos, zPos); 

            Instantiate(obj, pos, Quaternion.identity); 

            obj.name = $"index : {i} : Position {w} , {x} , {y} , {z}"; 
 
        }
        */

    }

}
