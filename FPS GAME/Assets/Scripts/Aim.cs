using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aim : MonoBehaviour
{

    Animator anim;

    Camera FPSCamera;

    [SerializeField] float zoomedInValue;

    float defaultZoomValue;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();

        FPSCamera = GetComponentInParent<Camera>();

        defaultZoomValue = FPSCamera.fieldOfView;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            anim.SetBool("Aim", true);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            anim.SetBool("Aim", false);
            FPSCamera.fieldOfView = defaultZoomValue;
        }
    }

    public void CameraZoom()
    {
        FPSCamera.fieldOfView = zoomedInValue;
    }

}
