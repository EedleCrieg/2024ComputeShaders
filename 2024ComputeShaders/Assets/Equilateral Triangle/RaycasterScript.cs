using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycasterScript : MonoBehaviour
{
    public LayerMask hitLayer; 
    public float rayDistance = 10f; 
    private Vector3 hitPosition; 

    public Transform sin; 
    public Transform cos; 

    // Update is called once per frame
    void Update()
    {
        // cast ray
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, rayDistance, hitLayer) )
        {
            hitPosition = hit.point; 
        }

        // Draw Rays
        Debug.DrawRay(transform.position, hitPosition, Color.blue); 
        Vector3 xValue = new Vector3(hitPosition.x, 0,0); 
        Vector3 yValue = new Vector3(0, hitPosition.y,0); 
        Debug.DrawRay(xValue, yValue, Color.green); 
        Debug.DrawRay(yValue, xValue, Color.red);

        // position objects
        sin.transform.position = new Vector3(0,hitPosition.y,0); 
        cos.transform.position = new Vector3(0,hitPosition.x,0); 
    }
}
