using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SabreAnimations : MonoBehaviour
{

    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            anim.SetBool("Ready", true);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            anim.SetBool("Ready", false);
            anim.SetTrigger("Attack");
        }

        if (Input.GetMouseButton(1))
        {
            anim.SetBool("Block", true);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            anim.SetBool("Block", false);
        }
    }

}
