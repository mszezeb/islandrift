using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float vHorizon = 1.0f;
    public float vVertical = 1.0f;
    public Transform tracing;
    public bool editTracingDir = false;
    public float distance = 5;
    Camera cm;
    // Start is called before the first frame update
    void Start()
    {
        cm = GetComponent<Camera>();
        Vector3 rot = Scene.scene.spawnPoint.rotation.eulerAngles;
        r_v = -rot.x;
        r_h = rot.y + 180;
    }

    float r_v = 0;
    float r_h = 0;

    // Update is called once per frame
    void LateUpdate()
    {
        if (tracing != null && Cursor.lockState == CursorLockMode.Locked)
        {
            tracing.rotation = Quaternion.Euler(0, 0, 0);
            Vector3 rel = tracing.position;
            r_h += Input.GetAxisRaw("Mouse X") * vHorizon;
            r_v += Input.GetAxisRaw("Mouse Y") * vHorizon;
            float thr_v = 80;
            // thr_v *= Mathf.PI / 2;
            if (r_v > thr_v)
                r_v = thr_v;
            else if (r_v < -thr_v)
                r_v = -thr_v;


            var sh = Mathf.Sin(r_h * (Mathf.PI / 180));
            var ch = Mathf.Cos(r_h * (Mathf.PI / 180));
            var sv = Mathf.Sin(r_v * (Mathf.PI / 180));
            var cv = Mathf.Cos(r_v * (Mathf.PI / 180));

            var a = sh * cv;
            var b = ch * cv;
            var c = -sv;
            var pos = transform.localPosition;

            pos.x = a;
            pos.z = b;
            pos.y = c;
            pos *= distance;
            pos += rel;
            transform.localPosition = pos;
            transform.LookAt(tracing);

            RaycastHit[] raycastHit = Physics.RaycastAll(tracing.position, transform.position - tracing.position, distance);
            if (raycastHit.Length > 0)
            {
                foreach (var v in raycastHit)
                {
                    if (!v.collider.isTrigger && !v.collider.CompareTag("Character"))
                    {
                        transform.position = v.point;
                        break;
                    }
                }
            }

            if (editTracingDir)
            {
                var r = tracing.rotation;
                r.y = r_h;
                if (r.y < -180)
                    r.y += 360;
                else if (r.y > 180)
                    r.y -= 360;
                r.y *= Mathf.PI / 180;
                // r.y = transform.rotation.y;
                var e = tracing.rotation.eulerAngles;
                e.y = r_h + 180;
                tracing.rotation = Quaternion.Euler(e);
                // tracing.Rotate(0, 0.1f, 0);
                // tracing.rotation = r;
                // var l = tracing.transform.position - transform.position;
                // l.y = tracing.transform.position.y;
                // tracing.LookAt(l, Vector3.up);
            }
        }
        // this.transform.position.
        // transform.Rotate(0, r_h, 0, Space.Self);
    }
}
