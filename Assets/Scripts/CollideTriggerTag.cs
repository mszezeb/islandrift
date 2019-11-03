using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollideTriggerTag : MonoBehaviour
{
    public string colliderTag;
    public UnityEvent trigger;
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag(colliderTag))
        {
            trigger.Invoke();
        }
    }
}
