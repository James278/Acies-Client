using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusketFire : MonoBehaviour
{

    [SerializeField] GameObject smokePrefab;

    [SerializeField] int timeBetweenShots;

    [SerializeField] bool canShoot;

    private void Start()
    {
        canShoot = true;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            if (canShoot)
            {
                GameObject smoke = Instantiate(smokePrefab, transform.position, Quaternion.identity) as GameObject;
                smoke.transform.parent = gameObject.transform;
                canShoot = false;

                StartCoroutine(WaitToShoot());
            }
        }
        
    }

    IEnumerator WaitToShoot()
    {
        yield return new WaitForSeconds(timeBetweenShots);
        canShoot = true;
    }

}
