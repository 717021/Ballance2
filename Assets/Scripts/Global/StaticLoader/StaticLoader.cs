using Assets.Scripts.Global;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * 静态加载包组件
 */

/// <summary>
/// 静态包加载器
/// </summary>
public class StaticLoader : MonoBehaviour
{
    /// <summary>
    /// 加载器是否正在加载
    /// </summary>
    public static bool IsLoading
    {
        get;private set;
    }
    /// <summary>
    /// 加载器是否加载完成
    /// </summary>
    public static bool IsStaticLoaderLoadfinished()
    {
        return !IsLoading;
    }

    [System.Serializable]
    public class StaticMod
    {
        public string Name = "";
        public TextAsset DescriptionFile = null;
        public List<GameObject> Prefabs = new List<GameObject>();
    }

    public List<StaticMod> StaticLoadMods = new List<StaticMod>();

    void Start()
    {
        if (StaticLoadMods.Count > 0) IsLoading = true;
        StartCoroutine(StartLoad());
    }
    IEnumerator StartLoad()
    {
         //等待 UI 加载完成
         yield return new WaitUntil(GlobalMediator.IsUILoadFinished);

        //加载静态自加载包
        foreach (StaticMod s in StaticLoadMods)
        {
            yield return StartCoroutine(GlobalModLoader.LoadPackInStatic(s, this));
        }

        IsLoading = false;
    }
}
