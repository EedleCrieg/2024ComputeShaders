using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCam : MonoBehaviour
{
    public Vector3 up; 
    public float speed; 


    // Update is called once per frame
    void Update()
    {
        transform.Rotate(up, Time.deltaTime * speed); 
    }
}
