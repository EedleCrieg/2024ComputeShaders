using UnityEngine;

[ExecuteInEditMode]
public class UpdateShaderWithLights : MonoBehaviour
{
    public Material particleMaterial; // Assign your shader material here
    public int maxLights = 4;         // Maximum number of lights to pass to shader
    
    private readonly Vector4[] lightColors = new Vector4[4];
    private readonly Vector4[] lightPositions = new Vector4[4];
    
    void Update()
    {
        if (particleMaterial == null)
        {
            Debug.LogWarning("Particle Material is not assigned.");
            return;
        }

        Light[] lights = FindObjectsOfType<Light>(); // Get all active lights in the scene
        int count = Mathf.Min(lights.Length, maxLights); // Limit to maxLights

        for (int i = 0; i < count; i++)
        {
            Light light = lights[i];
            Vector3 lightPos = light.transform.position;

            // Populate light color and position arrays
            lightColors[i] = light.color * light.intensity;
            lightPositions[i] = new Vector4(lightPos.x, lightPos.y, lightPos.z, 1.0f);
        }

        // If fewer than maxLights lights are found, zero out the remaining light slots
        for (int i = count; i < maxLights; i++)
        {
            lightColors[i] = Vector4.zero;
            lightPositions[i] = Vector4.zero;
        }

        // Send light properties to the shader
        particleMaterial.SetVectorArray("_LightColor", lightColors);
        particleMaterial.SetVectorArray("_LightPosition", lightPositions);
    }
}