using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    private bool moving;
    public float speed;
    void Start()
    {
        speed = 0.1f;
        moving = false;
    }

    void Update()
    {
        if (moving)
        {
            transform.Translate(Vector3.up * speed * Time.deltaTime, Space.World);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            moving = true;
        }
    }
}
