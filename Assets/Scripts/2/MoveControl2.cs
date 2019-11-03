using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveControl2 : MonoBehaviour
{
    public float gravity = 9.8f;
    [Header("On Ground")]
    public float groundHorizontalAccelFactor = 1;
    public float groundHorizontalDragFactor = 1;
    [Header("On Air")]
    public float airVerticalAccelFactor = 1;
    public float airVerticalDragFactor = 1;
    public float airHorizontalAccelFactor = 1;
    public float airHorizontalDragFactor = 1;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    Rigidbody rb;
    Camera cm;

    // Update is called once per frame
    void Update()
    {
        Vector3 vel = transform.InverseTransformDirection(rb.velocity);
        float forwardFactor;
        float rightwardFactor;
        float forwardDrag;
        float rightwardDrag;
        float upwardFactor = airVerticalAccelFactor;
        if (onGround)
        {
            forwardDrag = vel.z > 0 ?
                -vel.z * vel.z * groundHorizontalDragFactor :
                vel.z * vel.z * groundHorizontalDragFactor;
            rightwardDrag = vel.x > 0 ?
                -vel.x * vel.x * groundHorizontalDragFactor :
                vel.x * vel.x * groundHorizontalDragFactor;
            forwardFactor = groundHorizontalAccelFactor;
            rightwardFactor = groundHorizontalAccelFactor;
        }
        else
        {
            forwardDrag = vel.z > 0 ?
                -vel.z * vel.z * airHorizontalDragFactor :
                vel.z * vel.z * airHorizontalDragFactor;
            rightwardDrag = vel.x > 0 ?
                -vel.x * vel.x * airHorizontalDragFactor :
                vel.x * vel.x * airHorizontalDragFactor;
            forwardFactor = airHorizontalAccelFactor;
            rightwardFactor = airHorizontalAccelFactor;
        }
        float accelForward = (float)(Input.GetAxisRaw("Vertical") * forwardFactor);
        float accelRightward = (float)(Input.GetAxisRaw("Horizontal") * rightwardFactor);
        Vector2 horizontalAccel = new Vector2(accelRightward, accelForward).normalized;
        horizontalAccel.x *= accelRightward;
        horizontalAccel.y *= accelForward;
        accelForward *= horizontalAccel.y;
        accelRightward *= horizontalAccel.x;
        accelForward += forwardDrag;
        accelRightward += rightwardDrag;

        float upwardDrag = vel.y > 0 ?
            -vel.y * vel.y * airVerticalDragFactor :
            vel.y * vel.y * airVerticalDragFactor;

        float accelUpward = (float)(Input.GetAxisRaw("Jump") * upwardFactor);
        accelUpward += upwardDrag;
        rb.AddRelativeForce(accelRightward, accelUpward - gravity, accelForward, ForceMode.Acceleration);
    }

    void LateUpdate()
    {
    }

    bool onGround;
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            /// TODO: contact point position
            onGround = true;
        }

    }
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            onGround = false;
        }
    }
}
