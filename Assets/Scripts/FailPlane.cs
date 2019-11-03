using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

public class FailPlane : MonoBehaviour
{
    public GameObject watchingObject;
    public UnityEvent triggers;
    // Update is called once per frame
    void Update()
    {
        if (watchingObject != null && watchingObject.transform.position.y < gameObject.transform.position.y)
        {
            triggers.Invoke();
        }
    }
    void OnDrawGizmos()
    {
        Mesh ms = GetComponent<MeshFilter>().sharedMesh;
        Color c = Color.red;
        c.a = 0.4f;
        Gizmos.color = c;
        // Gizmos.DrawWireMesh(ms);
        Gizmos.DrawMesh(ms, transform.position, transform.rotation, new Vector3(100000, 0, 100000));
    }
}
