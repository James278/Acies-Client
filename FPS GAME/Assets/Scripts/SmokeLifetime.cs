using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeLifetime : MonoBehaviour
{

    float lifeTimeCounter;

    // Update is called once per frame
    void Update()
    {
        lifeTimeCounter += Time.deltaTime;
        if (lifeTimeCounter >= 10f)
        {
            Destroy(gameObject);
        }
    }
}
