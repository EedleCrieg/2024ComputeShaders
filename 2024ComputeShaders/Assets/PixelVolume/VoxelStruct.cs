using UnityEngine;

[System.Serializable]
public struct VoxelData{
    public Vector3 position; 
    public float r; 
    public float g; 
    public float b; 
    public float timer; 
    /*    public int neighbor0; 
    public int neighbor1; 
    public int neighbor2; 
    public int neighbor3; 
    public int neighbor4; 
    public int neighbor5; 
    */

    public VoxelData(Vector3 position, float r, float g, float b, float timer)
                {
                    this.position = position; 
                    this.r = r; 
                    this.g = g; 
                    this.b = b; 
                    this.timer = timer; 
                    /*
                    this.neighbor0 = neightbor0; 
                    this.neighbor1 = neightbor1; 
                    this.neighbor2 = neightbor2; 
                    this.neighbor3 = neightbor3; 
                    this.neighbor4 = neightbor4; 
                    this.neighbor5 = neightbor5; 
                    */
                }
}

// int neightbor0, int neightbor1, int neightbor2,
// int neightbor3, int neightbor4, int neightbor5
