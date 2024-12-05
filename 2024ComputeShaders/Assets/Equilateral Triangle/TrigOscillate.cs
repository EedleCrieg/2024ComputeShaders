using System;
using UnityEngine;

public class TrigOscillate : MonoBehaviour
{
    public float wavelength; 
    float PI = 3.14f; 
    public Transform parentTransform; 
    // Start is called before the first frame update
    void Start()
    {
        //wavelength = Mathf.PI * 0.66667f; 
    }

    // Update is called once per frame
    void Update()
    {
        //float wave = Mathf.Sin((2 * Mathf.PI /wavelength) - Time.time) * 0.25f + 0.75f; 
        //float wave = (4 / wavelength) * (Time.time * wavelength * Mathf.Floor(Time.time / wavelength + 0.5f)) * 0.25f + 0.75f; 
        float wave = (4 / wavelength * Mathf.Abs(Time.time - wavelength * (float)Math.Floor((Time.time / wavelength) + 0.5f)) - 1.0f)  * 0.25f + 0.75f; 
        Vector3 pos = parentTransform.position * wave;  

        transform.position = pos; 
    }
}
