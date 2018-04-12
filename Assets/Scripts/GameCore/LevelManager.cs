using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 游戏核心模块
 *        管理游戏主要工作
 */

public class LevelManager : MonoBehaviour
{

    //关卡基本的几个元件
    [System.NonSerialized]
    public GameObject psLevelStart,
        peLevelEnd,
        prResetPoints0;
    [System.NonSerialized]
    public List<GameObject> pcCheckPoints = new List<GameObject>();
    [System.NonSerialized]
    public List<GameObject> prResetPoints = new List<GameObject>();
    [System.NonSerialized]
    public LevelLoader currentLevelLaoder;

    void Start()
    {
        currentLevelLaoder = GameObject.Find("LevelLaoder").GetComponent<LevelLoader>();
        if (currentLevelLaoder != null)
        {
            currentLevelLaoder.levelManager = this;
        }
        else throw new System.Exception("[LevelManager] Crash!");       
    }
    void Update()
    {

    }
}
