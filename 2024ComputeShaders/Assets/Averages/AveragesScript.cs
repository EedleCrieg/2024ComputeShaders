using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AveragesScript : MonoBehaviour
{
    // Average Types
    // Half of existing
    // x0 | x1 | x2
    // 1/2((x1 - x0) + (x1 - x2))
    // (x0 + x2)1/2

    public Transform leftGO; 
   public  Transform rightGO; 

   public float inc = 0.1f; 

   public SystemDist systemDist; 

   public float reset  =1.0f; 
   public float alpha = 0.1f; 

   float timer = 1.0f; 

    // Update is called once per frame
    void Update()
    {
        if(leftGO != null && rightGO != null)
        {
            // if the timer is 0 then avergae
            if (timer <= 0)
            {
                //float y = alpha * 0.5f * (leftGO.position.y + rightGO.position.y) - transform.position.y; 
                float y = alpha * 0.5f * ((rightGO.position.y - transform.position.y) - (transform.position.y - leftGO.position.y)); 
                Vector3 newPos = new Vector3(transform.position.x,y,transform.position.z); 
                //Vector3 lerpPos = Vector3.Lerp(transform.position, newPos,0.5f); 
                transform.position = newPos; 
                timer = reset; 
            }
            // if the timer is not 0 then reduce timer by inc amount; 
            if (timer > 0)
            {
                timer -= inc; 
            }
        }
        
    }
}
