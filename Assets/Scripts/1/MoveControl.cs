using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveControl : MonoBehaviour
{
    public float vSpeed = 10;
    public float hSpeed = 10;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    Rigidbody rb;
    Camera cm;
    public CameraControl cc;
    public float airDrag = 0.1f;

    // Update is called once per frame
    void Update()
    {
        float movingVertical = (float)(Input.GetAxisRaw("Vertical") * vSpeed * -1);
        // transform.Translate(0, movingVertical, 0, Space.Self);
        rb.AddForce(0, movingVertical, 0, ForceMode.Acceleration);
        float movingHorizontal = (float)(Input.GetAxisRaw("Horizontal") * hSpeed * -1);
        float movingForward = (float)(Input.GetAxisRaw("Jump") * hSpeed);
        cc.editTracingDir = onGround;
        rb.AddRelativeForce(movingHorizontal, 0, movingForward, ForceMode.Acceleration);
        rb.AddForce((Vector3.zero - rb.velocity) * airDrag);

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
