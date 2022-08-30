using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject model;
    public PlayerInput pi;

    public float RunSpeed = 2.0f;
    [SerializeField]
    private Vector3 movingVec;
    private Animator anim;
    private Rigidbody rigid;

    void Awake()
    {
        pi = GetComponent<PlayerInput>();
        anim = model.GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("forward", pi.Dmag);
        if(pi.Dmag > 0.1f){
            model.transform.forward = pi.Dvec;
        }

        movingVec = pi.Dmag * model.transform.forward;
    }

    private void FixedUpdate()
    {
        //rigid.position += new Vector3( 0,0,1.0f) * Time.fixedDeltaTime;
        //print(rigid.position);
        rigid.position = rigid.position + RunSpeed * movingVec * Time.fixedDeltaTime;
    }
}
