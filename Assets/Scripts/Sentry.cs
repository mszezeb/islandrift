using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

public class Sentry : MonoBehaviour
{
    public float radius = 5;
    public float countDown = 8;
    public GameObject target;
    public UnityEvent fireTrigger;
    void Start()
    {
        lineRenderer = GetComponentInChildren<LineRenderer>();
    }
    LineRenderer lineRenderer;

    float aiming_time = 0;

    void Update()
    {
        lineRenderer.SetPosition(1, Vector3.zero);
        var distance = Vector3.Distance(target.transform.position, transform.position);
        if (distance < radius)
        {
            RaycastHit res;
            if (Physics.Raycast(transform.position, target.transform.position - transform.position, out res))
            {
                if (res.transform.gameObject == target)
                {
                    lineRenderer.SetPosition(1, new Vector3(0, 0, distance));
                    transform.LookAt(target.transform);
                    aiming_time += Time.deltaTime;
                    if (aiming_time >= countDown)
                    {
                        aiming_time = 0;
                        fireTrigger.Invoke();
                    }
                    return;
                }
            }
        }
        aiming_time = 0;
        return;
    }

    void OnDrawGizmos()
    {
        var rd = Color.red;
        rd.a = 0.5f;
        Gizmos.color = rd;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}