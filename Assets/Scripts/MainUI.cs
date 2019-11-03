using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    public Image energyImage;
    public Text energyText;
    public int maxEnergy = 100;

    public GameObject leapOn;
    public GameObject leapOff;
    public Text leapCDText;
    public GameObject vectoryOverlay;
    public GameObject levelLayer;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetEnergyValue(float value)
    {
        energyImage.fillAmount = value / maxEnergy;
        energyText.text = ((int)value).ToString();
    }

    public void SetBoostStatus(bool enable, float countDown)
    {
        if (enable)
        {
            if (countDown > 0)
            {
                leapOn.SetActive(false);
                leapOff.SetActive(true);
                leapCDText.text = Mathf.CeilToInt(countDown).ToString();
            }
            else
            {
                leapOn.SetActive(true);
                leapOff.SetActive(false);
            }
        }
        else
        {
            leapOn.SetActive(false);
            leapOff.SetActive(false);
        }
    }

    public void SetVectory(bool b)
    {
        vectoryOverlay.SetActive(b);
        levelLayer.SetActive(!b);
    }


}
