using UnityEngine;

/*
 * 资源统一寻找器
 */

namespace Assets.Scripts.Global
{
    /// <summary>
    /// 资源统一寻找器
    /// </summary>
    public static class GlobalAssetPool
    {
        public static Object GetAsset(string assetToken)
        {
            if (assetToken.Contains(":"))
            {
                string[] s = assetToken.Split(':');
                if (s.Length == 2)
                    return GetResource(s[0], s[1]);
                goto LOADKERNEL;
            }
            else goto LOADKERNEL;
            LOADKERNEL:
            return GetGameResource(assetToken);
        }
        public static T GetAsset<T>(string assetToken)where T : Object
        {
            if (assetToken.Contains(":"))
            {
                string[] s = assetToken.Split(':');
                if (s.Length == 2)
                    return GetResource<T>(s[0], s[1]);
                goto LOADKERNEL;
            }
            else goto LOADKERNEL;
            LOADKERNEL:
            return GetGameResource<T>(assetToken);
        }

        /// <summary>
        /// 获取游戏自带的资源。
        /// </summary>
        /// <param name="resourcePath">需要的资源路径。</param>
        /// <returns></returns>
        public static Object GetGameResource(string resourcePath)
        {
            return Resources.Load(resourcePath);
        }
        /// <summary>
        /// 获取游戏自带的资源。
        /// </summary>
        /// <typeparam name="T">需要的资源类型。</typeparam>
        /// <param name="resourcePath">需要的资源路径。</param>
        /// <returns></returns>
        public static T GetGameResource<T>(string resourcePath) where T : Object
        {
            return Resources.Load<T>(resourcePath);
        }


        /// <summary>
        /// 获取包中的资源。
        /// </summary>
        /// <param name="packName">包名。</param>
        /// <param name="resourceName">需要的资源名字。</param>
        /// <returns></returns>
        public static Object GetResource(string packName, string resourceName)
        {
            GlobalPack p;
            if (packName.Contains("*TYPE*")) packName = packName.Replace("*TYPE*", GlobalModLoader.GameTypeString);
            if (GlobalModLoader.IsPackLoaded(packName, out p))
            {
                if (p.AssetsPool.ContainsKey(resourceName))
                {
                    GameObject rs = null;
                    if (p.AssetsPool.TryGetValue(resourceName, out rs))
                        return rs;
                }
                if (p.Base != null)
                    return p.Base.LoadAsset(resourceName);
            }
            return null;
        }
        /// <summary>
        /// 获取包中的资源。
        /// </summary>
        /// <typeparam name="T">需要的资源类型。</typeparam>
        /// <param name="packName">包名。</param>
        /// <param name="resourceName">需要的资源名字。</param>
        /// <returns></returns>
        public static T GetResource<T>(string packName, string resourceName) where T : Object
        {
            GlobalPack p;
            if (packName.Contains("*TYPE*")) packName = packName.Replace("*TYPE*", GlobalModLoader.GameTypeString);
            if (GlobalModLoader.IsPackLoaded(packName, out p))
            {
                if (p.AssetsPool.ContainsKey(resourceName))
                {
                    GameObject rs = null;
                    if (p.AssetsPool.TryGetValue(resourceName, out rs))
                        return rs as T;
                }
                if (p.Base != null)
                    return p.Base.LoadAsset(resourceName, typeof(T)) as T;
            }
            return default(T);
        }
    }
}
