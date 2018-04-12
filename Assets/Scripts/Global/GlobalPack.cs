using Assets.Scripts.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/*
 * 代码说明：全局mod包申明类
 */

namespace Assets.Scripts.Global
{
    /// <summary>
    /// 元件类型
    /// </summary>
    public enum GlobalGamePartType
    {
        None,
        /// <summary>
        /// 在MenuLevel场景加载
        /// </summary>
        MenuLevelPart,
        /// <summary>
        /// 在Level场景加载
        /// </summary>
        LevelPart,
        /// <summary>
        /// 在LevelLoader场景加载
        /// </summary>
        LevelLoaderPart,
        /// <summary>
        /// 整体模块，所有场景都加载
        /// </summary>
        GamePart,
        /// <summary>
        /// 静态模块
        /// </summary>
        StaticPart,
    }
    /// <summary>
    /// 全局mod包类型
    /// </summary>
    public enum GlobalPackType
    {
        None,
        /// <summary>
        /// 资源
        /// </summary>
        Resource,
        /// <summary>
        /// 关卡
        /// </summary>
        Level,
        /// <summary>
        /// 元件
        /// </summary>
        Mod
    }
    /// <summary>
    /// 全局mod包加载状态
    /// </summary>
    public enum GlobalPackLoadState
    {
        /// <summary>
        /// 未加载
        /// </summary>
        NotLoad,
        /// <summary>
        /// 加载中
        /// </summary>
        Loading,
        /// <summary>
        /// 加载失败
        /// </summary>
        LoadFailed,
        /// <summary>
        /// 加载成功
        /// </summary>
        Loaded,
    }
    /// <summary>
    /// 全局mod包
    /// </summary>
    public class GlobalPack : IDisposable
    {
        private string name = "";
        private string path = "";
        private BFSReader describeFile = null;

        public GlobalPack(string path)
        {
            AssetsPool = new Dictionary<string, GameObject>();
            this.path = path;
        }

        /// <summary>
        /// 包 描述文件
        /// </summary>
        public BFSReader DescribeFile
        {
            get { return describeFile; }
            set
            {
                if (describeFile == null)
                    describeFile = value;
            }
        }
        /// <summary>
        /// 作者名称
        /// </summary>
        public string AuthorName { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(name))
                    return Path;
                else return name;
            }
            set { name = value; }
        }
        /// <summary>
        /// 路径
        /// </summary>
        public string Path { get { return path; } }
        /// <summary>
        /// 基本 Assets
        /// </summary>
        public AssetBundle Base = null;
        /// <summary>
        /// 依赖 包 :分隔
        /// </summary>
        public string DependsPack { get; set; }
        /// <summary>
        /// 依赖 入口函数 格式：className:methodName 必须是静态类，静态方法
        /// </summary>
        public string EntryFunction { get; set; }
        /// <summary>
        /// 是否需要初始化
        /// </summary>
        public bool NeedInitialize { get; set; }
        /// <summary>
        /// 加载状态
        /// </summary>
        public GlobalPackLoadState LoadState { get; set; }
        /// <summary>
        /// 包类型
        /// </summary>
        public GlobalPackType Type { get; set; }
        /// <summary>
        /// 暂时储存资源中的perfab
        /// </summary>
        public Dictionary<string, GameObject> AssetsPool { get; private set; }
        /// <summary>
        /// 获取暂存perfab
        /// </summary>
        /// <param name="perfabName">名字</param>
        /// <returns></returns>
        public GameObject GetPerfab(string perfabName)
        {
            if (string.IsNullOrEmpty(perfabName)) return null;
            GameObject perfab;
            if (AssetsPool.TryGetValue(perfabName, out perfab))
                return perfab;
            else if(Base!=null)
            {
                perfab = Base.LoadAsset<GameObject>(perfabName);
                if (perfab != null)
                    AssetsPool.Add(perfabName, perfab);
                return perfab;
            }
            return null;
        }

        public string GetLoadStateStr()
        {
            switch (LoadState)
            {
                case GlobalPackLoadState.NotLoad:
                    return "<color=#0077eeff></color>";
                case GlobalPackLoadState.Loading:
                    return "<color=#66eeeeff></color>";
                case GlobalPackLoadState.LoadFailed:
                    return "<color=#FFB90F></color>";
            }
            return LoadState.ToString();
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            describeFile.Dispose();
            describeFile = null;
            AssetsPool.Clear();
            AssetsPool = null;
            if (Base != null)
            {
                Base.Unload(false);
                Base = null;
            }
        }
    }
    /// <summary>
    /// 游戏部件
    /// </summary>
    public class GlobalGamePart
    {
        public GlobalGamePart(GlobalPack pack) { Pack = pack; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
    /// 部件类型
    /// </summary>
        public GlobalGamePartType Type { get; set; }
        /// <summary>
        /// 自动加载Perfab名字 (Perfab资源名称 / Perfab资源名称>克隆的GameObject名字 )
        /// </summary>
        public string AutoInitObject { get; set; }
        /// <summary>
        /// 自动附加脚本名字( modname:classname>objname)
        /// </summary>
        public string AutoAttachScript { get; set; }

        public GlobalPack Pack { get; private set; }
    }
}
