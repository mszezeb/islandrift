using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CheckPoint : MonoBehaviour
{
    // public CheckPointTrigger triggers;
    public UnityEvent triggers;
    public bool oneShot = true;
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Character"))
        {
            triggers.Invoke();
            if (oneShot)
            {
                this.gameObject.SetActive(false);
            }
        }
    }
}
