using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class StartScene : MonoBehaviour
{
    static public StartScene scene;
    public class Singleton
    {
        public LevelDesc[] levels;
        private static Singleton singleton;
        Singleton()
        {
            levels = new LevelDesc[3];
            levels[0].name = "MoveControl";
            levels[1].name = "level2";
            levels[2].name = "level3";
            levels[0].lv = 1;
            levels[1].lv = 2;
            levels[2].lv = 3;
            levels[0].locked = false;
            levels[1].locked = true;
            levels[2].locked = true;
        }
        static public Singleton GetSingleton()
        {
            if (null == singleton)
                singleton = new Singleton();
            return singleton;
        }
    }
    public GameObject canvas;
    public GameObject chooseLevel;

    public struct LevelDesc
    {
        public int lv;
        public int collect;
        public string name;
        public bool locked;
    }


    void Start()
    {
        scene = this;
    }

    // static public void CompleteLevel(string name)
    // {
    //     var ls = Singleton.GetSingleton().levels;
    //     for (int i = 0; i < ls.Length; ++i)
    //     {
    //         if (ls[i].name == name)
    //         {
    //             ls[i].locked = false;
    //             break;
    //         }
    //     }
    // }
    static public void CompleteLevel(int lv)
    {
        var ls = Singleton.GetSingleton().levels;
        if (lv >= ls.Length)
            return;
        ls[lv].locked = false;
    }
    public void Quit()
    {
        Application.Quit();
    }

}
