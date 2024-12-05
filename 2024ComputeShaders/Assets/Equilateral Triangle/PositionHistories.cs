using UnityEngine;

public class PositionHistories : MonoBehaviour
{
    public int historyLength = 10;  // Number of positions to store
    public float factor = 0.1f; 
    private Vector3[] positions;    // Array to store the last n positions

    public Color startColor; 
    public Color endColor; 

    void Start()
    {
        // Initialize the array with the desired length
        positions = new Vector3[historyLength];
    }

    void Update()
    {
        // Shift each position down in the array
        for (int i = historyLength - 1; i > 0; i--)
        {
            positions[i] = new Vector3((positions[i - 1].x + Time.deltaTime) * factor, positions[i - 1].y, 0);
        }

        // Store the current position at the start of the array
        positions[0] = transform.position;
    }

    // Optional: For visualization purposes
    void OnDrawGizmos()
    {
        // Draw lines between stored positions for a visual trail
        if (positions == null) return;

        for (int i = 0; i < historyLength - 1; i++)
        {
            Gizmos.color = Color.Lerp(startColor, endColor, (float)i / historyLength);
            Gizmos.DrawLine(positions[i], positions[i + 1]);
        }
    }
}