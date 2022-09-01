using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControll : MonoBehaviour
{
    public PlayerInput pi;

    private GameObject Player;
    private GameObject CameraHandle;

    // Start is called before the first frame update
    void Awake()
    {
        CameraHandle = transform.parent.gameObject;
        Player = CameraHandle.transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
