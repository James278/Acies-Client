using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusketFire : MonoBehaviour
{

    [SerializeField] GameObject smokePrefab;

    [SerializeField] int timeBetweenShots; // Still to be decided

    [SerializeField] float rayCastRange; // Still to be decided

    [SerializeField] bool canShoot;

    [SerializeField] float smokeOffest; // Set to 1
                    
    [SerializeField] Vector3 smokeRotOffset; // Set Y to 90

    Animator anim;
    Camera FPCam;
    Transform musketChildTransform;

    private void Start()
    {
        canShoot = true;

        anim = GetComponent<Animator>();
        FPCam = gameObject.GetComponentInParent<Camera>();
        musketChildTransform = transform.Find("Musket"); 
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (canShoot)
            {
                ActivateSmoke();
                Recoil();
                ProcessRayCast();

                canShoot = false;
                StartCoroutine(WaitToShoot());
            }
        }
        
    }

    private void ActivateSmoke()
    {
        GameObject smoke = Instantiate(smokePrefab, transform.position + transform.right * smokeOffest, 
                                       transform.rotation * Quaternion.Euler(smokeRotOffset)) as GameObject;
        smoke.transform.parent = gameObject.transform;
    }

    private void Recoil()
    {
        anim.SetTrigger("Shoot");
    }

    IEnumerator WaitToShoot()
    {
        yield return new WaitForSeconds(timeBetweenShots);
        canShoot = true;
    }

    private void ProcessRayCast()
    {
        RaycastHit target;
        Ray ray = new Ray(musketChildTransform.position, FPCam.transform.forward);

        if (Physics.Raycast(ray, out target, rayCastRange))
        {
            print("Ray hit");
            Debug.DrawRay(musketChildTransform.position, FPCam.transform.forward * target.distance, Color.red);
        }
    }

}
