using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Photon : MonoBehaviour
{
    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.tag == "Player")
        {
            //Do shit
        }
        else
        {
            GetComponent<Rigidbody>().AddForce(Vector3.up * 500);
        }
    }
}
