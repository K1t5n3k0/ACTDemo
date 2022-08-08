using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public bool SWITCH = true;

    public string KeyUp = "w";    
    public string KeyDown = "s";   
    public string KeyLeft = "a";
    public string KeyRight = "d";

    public float Dup;
    public float Dright;

    public float targetDup;
    public float targetDright;
    public float velocityDup;
    public float velocityDright;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        targetDup = (Input.GetKey(KeyUp)?1.0f:0) - (Input.GetKey(KeyDown)?1.0f:0);
        targetDright = (Input.GetKey(KeyRight)?1.0f:0) - (Input.GetKey(KeyLeft)?1.0f:0);
        
        if(SWITCH == false){
            targetDup = 0;
            targetDright = 0;
        }

        Dup = Mathf.SmoothDamp (Dup, targetDup, ref velocityDup, 0.1f);
        Dright = Mathf.SmoothDamp (Dright, targetDright, ref velocityDright, 0.1f);
    }
}
