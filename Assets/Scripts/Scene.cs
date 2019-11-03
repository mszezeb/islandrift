using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene : MonoBehaviour
{
    public int lv;
    public static Scene scene;
    public Transform spawnPoint;
    public GameObject player;
    public CharacterMoveControl character;
    public MainUI mainUI;
    public void OnEnable()
    {
        scene = this;
        var v = Physics.gravity;
        v.y = -initGravity;
        Physics.gravity = v;
        Cursor.lockState = CursorLockMode.Locked;

        player.transform.position = spawnPoint.position;
    }
    public Scene()
    {
    }

    CheckPoint[] checkPoints;
    CheckPoint lastPoint;
    class CheckPointData
    {
        public CharacterMoveControl.CharacterCheckPointData character;
        public GameObject[] energyPacks = new GameObject[0];
        public bool[] energyPackSTatus = new bool[0];
        public void UpdateEnergyPacksData()
        {
            if (energyPacks.Length == 0)
            {
                var lst = GameObject.FindGameObjectsWithTag("EnergyPack");
                energyPackSTatus = new bool[lst.Length];
                energyPacks = lst;
            }
            for (int i = 0; i < energyPacks.Length; ++i)
            {
                energyPackSTatus[i] = energyPacks[i].activeSelf;
            }
        }
        public void RestoreEnergyPackData()
        {
            for (int i = 0; i < energyPacks.Length; ++i)
            {
                energyPacks[i].SetActive(energyPackSTatus[i]);
            }
        }
    }
    CheckPointData checkPointData = new CheckPointData();

    void Start()
    {
        checkPoints = GameObject.FindObjectsOfType<CheckPoint>();
        DoCheckPoint(0);
    }
    void Update()
    {
        mainUI.SetEnergyValue(character.currentEnergy);
        mainUI.SetBoostStatus(character.boostEnabled, character.boostCDRemain);
    }
    public void DoCheckPoint(int id)
    {
        Debug.Log("CheckPoint id " + id);
        checkPointData.character = character.GetCheckPointData();
        checkPointData.UpdateEnergyPacksData();
    }
    public void DoVictory()
    {
        Cursor.lockState = CursorLockMode.None;
        mainUI.SetVectory(true);
        StartScene.CompleteLevel(lv);
    }
    public void DoCollect()
    {
        Debug.Log("collect");
        StartScene.Singleton.GetSingleton().levels[lv - 1].collect = 1;
    }
    public void NextLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(StartScene.Singleton.GetSingleton().levels[lv].name);
    }
    public void BackToLastCheckPoint()
    {
        character.ApplyCheckPointData(checkPointData.character);
        checkPointData.RestoreEnergyPackData();
    }
    public float initGravity = 9.8f;
}
