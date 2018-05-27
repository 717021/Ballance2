using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 游戏核心模块
 *        管理游戏主要工作
 */
namespace Assets.Scripts.GameCore
{
    public class LevelManager : MonoBehaviour
    {
        public LevelManager()
        {
            GlobalMediator.SetSystemServices(GameServices.LevelManager, this);
        }

        //关卡基本的几个元件
        [System.NonSerialized]
        public GameObject psLevelStart,
            peLevelEnd,
            prResetPoints0;

        private GameObject LevelInitMgr;
        private LevelLoader currentLevelLaoder;

        public IEnumerator LoadGameCore()
        {
            LevelInitMgr.SetActive(true);
            yield return new WaitUntil(InitializationMgr.IsInitializationMgrInitializefinished);
        }

        void Start()
        {
            currentLevelLaoder = GlobalMediator.GetSystemServices(GameServices.LevelLoader) as LevelLoader;
            if (currentLevelLaoder != null)
                currentLevelLaoder.levelManager = this;
            else throw new System.Exception("[LevelManager] Crash!");
            LevelInitMgr = transform.Find("LevelInitMgr").gameObject;
        }
        void Update()
        {

        }
    }
}
