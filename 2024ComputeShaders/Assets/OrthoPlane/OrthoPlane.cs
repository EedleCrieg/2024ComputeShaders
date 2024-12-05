using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.AI;

public class OrthoPlane : MonoBehaviour
{
    public Transform main; 
    public Transform o1; 
    public Transform o2; 
    public Transform o3; 
    public Transform o4; 
    public Transform o5; 
    public Transform o6; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float x = main.position.x; 
        float y = main.position.y; 
        float z = main.position.z;

        o1.position = new Vector3(-y, x, 0); 
        o2.position = new Vector3( y,-x, 0); 
        o3.position = new Vector3(-z, 0, x);
        o4.position = new Vector3( z, 0,-x);
        o5.position = new Vector3( 0,-z, y);
        o6.position = new Vector3( 0, z,-y);

        Debug.DrawLine(main.position, Vector3.zero, Color.red); 
        Debug.DrawLine(o1.position, Vector3.zero, Color.blue); 
        Debug.DrawLine(o2.position, Vector3.zero, Color.blue); 
        Debug.DrawLine(o3.position, Vector3.zero, Color.blue); 
        Debug.DrawLine(o4.position, Vector3.zero, Color.blue); 
        Debug.DrawLine(o5.position, Vector3.zero, Color.blue); 
        Debug.DrawLine(o6.position, Vector3.zero, Color.blue); 
    }
}
