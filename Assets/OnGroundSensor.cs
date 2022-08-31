using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGroundSensor : MonoBehaviour
{
    // Start is called before the first frame update
    public CapsuleCollider capcol;

    private Vector3 point1;
    private Vector3 point2;
    private float radius;
    void Awake()
    {
        radius = capcol.radius;    
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        point1 = transform.position + transform.up * radius;
        point2 = transform.position + transform.up * capcol.height - transform.up * radius;

        Collider[] outputCols = Physics.OverlapCapsule(point1, point2, radius, LayerMask.GetMask("Ground"));
        //Collider[] outputCols = Physics.OverlapCapsule(point1, point2, radius);
        
        if (outputCols.Length != 0)
        {
            SendMessageUpwards("IsGround");
        }
        else
        {
            SendMessageUpwards("IsNotGround");
        }
    }
}
