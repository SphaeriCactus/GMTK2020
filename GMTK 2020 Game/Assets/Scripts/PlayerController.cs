using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 10f;

    private Rigidbody rb;
    private Vector3 moveDirection;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalMovement = Input.GetAxisRaw("Horizontal");
        float verticalMovement = Input.GetAxisRaw("Vertical");

        moveDirection = (horizontalMovement * transform.right + verticalMovement * transform.forward).normalized;
    }

    void FixedUpdate() {
        Vector3 yVelocity = new Vector3(0, rb.velocity.y, 0);
        rb.velocity = moveDirection * walkSpeed;
        rb.velocity += yVelocity;
    }
}
