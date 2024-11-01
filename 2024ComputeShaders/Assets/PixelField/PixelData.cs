using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices; 
using UnityEngine;

[StructLayout(LayoutKind.Sequential)]
public struct PixelData{
    //public int[] neighbors; // keep getting error that data type is not blittable
    public int neighbor1, neighbor2, neighbor3, neighbor4, neighbor5, neighbor6, neighbor7, neighbor8; 
    public float r;
    public float g; 
    public float b;  
    public float timer;
    public float interpolationFactor; 
    public int interpolating; 
    public int ping; 
    public Vector3 finalColor; 
    public Vector3 startColor;  

    public PixelData ( int neighbor1,int neighbor2, int neighbor3, int neighbor4, int neighbor5, int neighbor6, int neighbor7, int neighbor8, 
                        float timer, float r, float g, float b, 
                        float interpolationFactor, int interpolating, int ping, Vector3 finalColor, Vector3 startColor)
    {
        this.neighbor1 = neighbor1;
        this.neighbor2 = neighbor2;
        this.neighbor3 = neighbor3;
        this.neighbor4 = neighbor4;
        this.neighbor5 = neighbor5;
        this.neighbor6 = neighbor6;
        this.neighbor7 = neighbor7;
        this.neighbor8 = neighbor8; 
        this.timer = timer;  
        this.r = r; 
        this.g = g; 
        this.b = b;
        this.interpolationFactor = interpolationFactor;  
        this.interpolating = interpolating;
        this.ping = ping; 
        this.finalColor = finalColor; 
        this.startColor = startColor;  
    }
}
