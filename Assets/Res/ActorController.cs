using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject model;
    public PlayerInput pi;    
    
    private Animator anim;

    void Awake()
    {
        pi = GetComponent<PlayerInput>();
        anim = model.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("forward", pi.Dmag);
        if(pi.Dmag > 0.1f){
            model.transform.forward = pi.Dvec;
        }
    }
}
