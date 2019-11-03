using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class BasicPath : MonoBehaviour
{
    public enum PathMode
    {
        Straight,
        Ellipse,
    }
    public PathMode mode;
    public Transform pointA;
    public Transform pointB;
    public Transform pointO;
    public float velocity = 1;

#if UNITY_EDITOR
    public Color gizmosLineColor = Color.green;
#endif
    [Header("Straight Only")]
    public float interval = 5;

    Transform underControl;
    Matrix4x4 tm_ellipse;

    void Start()
    {
        underControl = transform;
        underControl.position = pointA.position;
        if (mode == PathMode.Straight)
            startStraightMode();
        else
            startEllipseMode();
    }
    bool UpdateMatrixForEllipse()
    {
        if (pointA == null || pointB == null || pointO == null)
            return false;
        tm_ellipse = Matrix4x4.identity;
        var up = -Vector3.Cross(pointA.position - pointO.position,
         pointB.position - pointO.position).normalized + pointO.position;
        tm_ellipse.SetColumn(0, pointA.position - pointO.position);
        tm_ellipse.SetColumn(1, up - pointO.position);
        tm_ellipse.SetColumn(2, pointB.position - pointO.position);
        tm_ellipse.SetColumn(3, pointO.position);
        tm_ellipse.m30 = 1;
        tm_ellipse.m31 = 1;
        tm_ellipse.m32 = 1;
        tm_ellipse.m33 = 1;
        return true;
    }
    private Pool straight_pool;
    class Pool
    {
        BasicPath p;
        public Pool(BasicPath p)
        {
            this.p = p;
            queue = new Queue();
        }
        System.Collections.Queue queue;
        public T Get<T>() where T : class
        {
            T res;
            if (queue.Count != 0)
            {
                res = queue.Dequeue() as T;
            }
            else
            {
                var o = GameObject.Instantiate(p.gameObject, p.transform.parent);
                var s = o.GetComponent<BasicPath>();
                o.GetComponent<MeshRenderer>().enabled = true;
                o.GetComponent<MeshCollider>().enabled = true;
                s.underControl = o.transform;
                s.straight_pool = this;
                res = o as T;
            }
            return res;
        }
        public void Put(GameObject o)
        {
            o.SetActive(false);
            queue.Enqueue(o);
        }
    }

    void _startStraightMode()
    {
        IEnumerator move()
        {
            underControl.position = pointA.position;
            while (true)
            {
                var delta_b = pointB.position - underControl.position;
                var delta = delta_b.normalized * velocity * Time.deltaTime;
                if (delta_b.magnitude < delta.magnitude)
                    break;
                underControl.Translate(delta, Space.World);
                yield return null;
            }
            straight_pool.Put(gameObject);
        }
        StartCoroutine(move());
    }
    void startStraightMode()
    {
        if (straight_pool == null)
            straight_pool = new Pool(this);
        else
            return;
        IEnumerator master()
        {
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<Collider>().enabled = false;
            while (true)
            {
                GameObject o = straight_pool.Get<GameObject>();
                o.SetActive(true);
                o.GetComponent<BasicPath>()?._startStraightMode();
                yield return new WaitForSeconds(interval);
            }
        }
        StartCoroutine(master());
    }

    void startEllipseMode()
    {
        IEnumerator move()
        {
            float acc_d = 0;
            while (true)
            {
                var delta = velocity * Time.deltaTime * (Mathf.PI / 180);
                acc_d += delta;
                Vector3 v = Vector3.zero;
                v.x = Mathf.Cos(acc_d);
                v.z = Mathf.Sin(acc_d);
                v = tm_ellipse.MultiplyPoint3x4(v);
                underControl.transform.position = v;
                yield return null;
            }
        }
        if (UpdateMatrixForEllipse())
            StartCoroutine(move());
    }

#if UNITY_EDITOR
    Vector3[] ellipse_point = new Vector3[8];
    void OnDrawGizmos()
    {
        if (mode == PathMode.Straight)
        {
            Gizmos.color = gizmosLineColor;
            Gizmos.DrawLine(pointA.position, pointB.position);
        }
        else if (UpdateMatrixForEllipse())
        {
            Gizmos.color = gizmosLineColor;

            int pts = ellipse_point.Length;
            for (int i = 0; i < pts; ++i)
            {
                Vector3 v = Vector3.zero;
                v.x = Mathf.Cos((2 * Mathf.PI) / pts * i);
                v.z = Mathf.Sin((2 * Mathf.PI) / pts * i);
                v = tm_ellipse.MultiplyPoint3x4(v);
                ellipse_point[i] = v;
            }
            Gizmos.DrawLine(pointO.position, pointA.position);
            Gizmos.DrawLine(pointO.position, pointB.position);
            for (int i = 0; i < pts; ++i)
            {
                Gizmos.DrawLine(ellipse_point[i], ellipse_point[(i + 1) % pts]);
            }
        }
    }
#endif
}