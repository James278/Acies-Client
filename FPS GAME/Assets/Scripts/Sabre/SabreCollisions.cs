using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SabreCollisions : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Sabre"))
        {
            print("Sabre hit");
        }
    }
}
