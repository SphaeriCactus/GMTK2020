using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conveyor : MonoBehaviour
{
    public float force = 5000f;

    void OnCollisionStay(Collision other)
    {
        GameObject go = other.collider.gameObject;
        if (go.CompareTag("Player"))
        {
            go.GetComponent<Rigidbody>().AddForce(transform.forward * force * Time.deltaTime, ForceMode.Force);
        }
    }
}
