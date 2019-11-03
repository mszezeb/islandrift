using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfManageButton : MonoBehaviour
{
    void OnEnable()
    {
        var ls = StartScene.Singleton.GetSingleton().levels;
        for (int i = 0; i < ls.Length; ++i)
        {
            if ("level" + ls[i].lv == transform.name)
            {
                GetComponent<UnityEngine.UI.Button>().enabled = !ls[i].locked;
                transform.Find("lock").gameObject.SetActive(ls[i].locked);
                transform.Find("Text").GetComponent<UnityEngine.UI.Text>().text = ls[i].collect + "/1";
                break;
            }
        }
    }
}
