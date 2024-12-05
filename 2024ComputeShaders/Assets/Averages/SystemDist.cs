using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SystemDist : MonoBehaviour
{
    [SerializeField] private Transform prefab; 
    [SerializeField] private Vector2 count; 
    [SerializeField] Transform [] units; 

    public bool neighborsSet = false; 

    void Start()
    {
        int index = 0; 
        units = new Transform[(int)(count.x * count.y)]; 
        for (int i = 0; i < count.x; i++)
        {
            for (int j = 0; j < count.y; j++)
            {
                Vector3 pos = new Vector3(i,Random.value * 10,j); 

                Transform obj = Instantiate(prefab, pos, Quaternion.identity, transform ); 
                obj.GetComponent<AveragesScript>().systemDist = this; 
                obj.name = $"node : {index}"; 

                units[index] = obj; 
                index ++; 
            }
        }

        SetNeighbors(); 
    }

    void SetNeighbors()
    {
        for (int i = 0; i <= (count.x * count.y - 1); i++)
        {
            if (i == 0)
            {
                units[i].GetComponent<AveragesScript>().leftGO = units[i];
                units[i].GetComponent<AveragesScript>().rightGO = units[i + 1];
            }
            else if (i == (count.x * count.y - 1))
            {
                units[i].GetComponent<AveragesScript>().leftGO = units[i - 1];
                units[i].GetComponent<AveragesScript>().rightGO = units[i];
            }
            else
            {
                units[i].GetComponent<AveragesScript>().leftGO = units[i - 1];
                units[i].GetComponent<AveragesScript>().rightGO = units[i + 1];
            }
        }  
    }
}
