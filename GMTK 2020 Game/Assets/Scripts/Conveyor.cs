using UnityEngine;

public class Conveyor : MonoBehaviour
{
    public float force = 10000;
    public Rigidbody playerRB;
    private bool on = false;
    public bool shouldBeOn = false;

    void OnCollisionEnter(Collision other)
    {
        if (other.collider.gameObject.CompareTag("Player"))
        {
            on = true;
        }
    }

    void OnCollisionExit(Collision other)
    {
        if (other.collider.gameObject.CompareTag("Player"))
        {
            on = false;
        }
    }

    void FixedUpdate()
    {
        if (on && shouldBeOn)
            playerRB.AddForce(transform.forward * force * Time.deltaTime, ForceMode.Force);
    }
}
