using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwtichScene : MonoBehaviour
{
    public string sceneName;
    public void DoSwitch()
    {
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}
