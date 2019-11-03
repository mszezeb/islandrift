using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyPack : MonoBehaviour
{
    public float capacity = 20;
    public float timeSpan = 1;

    public void Use()
    {
        gameObject.SetActive(false);
        Scene.scene.character.StartEnergyChange(capacity, timeSpan);
    }
}
