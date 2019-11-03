using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    // Start is called before the first frame update
    public void DoCollide()
    {
        Scene.scene.BackToLastCheckPoint();
    }
}
