using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //public Edward edward;
    public float walkSpeed = 10f;

    private Rigidbody rb;
    private Vector3 moveDirection;
    //private bool hasMoved;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //hasMoved = false;
    }

    void Update()
    {
        float horizontalMovement = Input.GetAxis("Horizontal");
        float verticalMovement = Input.GetAxis("Vertical");

        // if ((horizontalMovement != 0f || verticalMovement != 0f) && !hasMoved)
        // {
        //     hasMoved = true;
        //     edward.Speak(2);
        // }

        moveDirection = horizontalMovement * transform.right + verticalMovement * transform.forward;
    }

    void FixedUpdate()
    {
        Vector3 yVelocity = new Vector3(0, rb.velocity.y, 0);
        rb.velocity = moveDirection * walkSpeed;
        rb.velocity += yVelocity;
    }
}
