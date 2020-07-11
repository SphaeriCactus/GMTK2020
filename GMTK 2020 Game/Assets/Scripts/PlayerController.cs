using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 10f;

    private Rigidbody rb;
    private Vector3 moveDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float horizontalMovement = Input.GetAxis("Horizontal");
        float verticalMovement = Input.GetAxis("Vertical");

        moveDirection = horizontalMovement * transform.right + verticalMovement * transform.forward;
    }

    void FixedUpdate()
    {
        Vector3 yVelocity = new Vector3(0, rb.velocity.y, 0);
        rb.velocity = moveDirection * walkSpeed;
        rb.velocity += yVelocity;
    }
}
