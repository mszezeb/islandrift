using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Rotating : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float flippingInterval = 8;
    public float flippingTime = 2;

    void Start()
    {
        StartCoroutine(RotateInterval());
    }

    IEnumerator RotateInterval()
    {
        while (true)
        {
            yield return new WaitForSeconds(flippingInterval);
            yield return DoRotate();
        }
    }

    IEnumerator DoRotate()
    {
        float acc = 0;
        while (acc < 180)
        {
            float rot = 180 / flippingTime * Time.deltaTime;
            if (rot + acc > 180)
                rot = 180 - acc;
            transform.RotateAround(pointA.position, pointB.position - pointA.position, rot);
            acc += rot;
            yield return null;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawRay(pointA.position, pointB.position - pointA.position);
    }
}