using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/*
 * IC 管理器
 *     不建议太多子物体的物体备份ic
 */

namespace Assets.Scripts.GameCore
{
    /// <summary>
    /// IC 备份方式
    /// </summary>
    public enum ICBackType
    {
        /// <summary>
        /// 无备份
        /// </summary>
        NoBackup,
        /// <summary>
        /// 备份本体
        /// </summary>
        BackupThisObject,
        /// <summary>
        /// 备份本体和所有子
        /// </summary>
        BackupThisAndChild,
        /// <summary>
        /// 自定义备份，由 ICBackupCustom 属性指定
        /// </summary>
        Custom,
    }
    /// <summary>
    /// IC 恢复方式
    /// </summary>
    public enum ICResetType
    {
        /// <summary>
        /// 由 SectorManager 重置
        /// </summary>
        ResetBySectorMgr,
        /// <summary>
        /// 不重置
        /// </summary>
        NoReset,
        /// <summary>
        /// 发送 重置消息 到机关
        /// </summary>
        SendResetMsg,
    }
    /// <summary>
    /// IC 管理器
    /// </summary>
    public class ICManager : MonoBehaviour
    {
        public ICManager()
        {
            GlobalMediator.SetSystemServices(GameServices.ICManager, this);
        }

        private struct ICInfo
        {
            public GameObject obj;
            public Vector3 pos;
            public Vector3 rote;
        }
        private List<ICInfo> allICs = new List<ICInfo>();

        /// <summary>
        /// 备份物体和其所有子物体的IC
        /// </summary>
        /// <param name="g">需要的物体</param>
        public void BackupObjectAndChildIC(GameObject g)
        {
            BackupObjectIC(g);
            foreach (Transform t in g.transform)
            {
                BackupObjectAndChildIC(t.gameObject);
            }
        }
        /// <summary>
        /// 取消备份物体和其所有子物体的IC
        /// </summary>
        /// <param name="g">需要的物体</param>
        public void RemoveObjectAndChildIC(GameObject g)
        {
            RemoveObjectIC(g);
            foreach (Transform t in g.transform)
            {
                RemoveObjectAndChildIC(t.gameObject);
            }
        }
        /// <summary>
        /// 恢复物体和其所有子物体的IC
        /// </summary>
        /// <param name="g">需要的物体</param>
        public void ObjectAndChildRecoverIC(GameObject g)
        {
            ObjectRecoverIC(g);
            foreach (Transform t in g.transform)
            {
                ObjectAndChildRecoverIC(t.gameObject);
            }
        }
        /// <summary>
        /// 备份物体的IC
        /// </summary>
        /// <param name="g">需要的物体</param>
        public void BackupObjectIC(GameObject g)
        {
            ICInfo i = new ICInfo();
            i.obj = g;
            i.pos = g.transform.position;
            i.rote = g.transform.eulerAngles;
            allICs.Add(i);
        }
        /// <summary>
        /// 取消备份物体的IC
        /// </summary>
        /// <param name="g">需要的物体</param>
        public void RemoveObjectIC(GameObject g)
        {
            ICInfo i = FindIC(g);
            if (i.obj == g)
            {
                allICs.Remove(i);
            }
        }
        /// <summary>
        /// 恢复物体的IC
        /// </summary>
        /// <param name="g">需要的物体</param>
        public void ObjectRecoverIC(GameObject g)
        {
            ICInfo i = FindIC(g);
            if (i.obj == g)
            {
                g.SetActive(false);
                g.transform.position = i.pos;
                g.transform.eulerAngles = i.rote;
                g.SetActive(true);
            }
        }

        private ICInfo FindIC(GameObject g)
        {
            ICManager.ICInfo result = default(ICManager.ICInfo);
            foreach (ICManager.ICInfo current in this.allICs)
            {
                if (current.obj == g)
                {
                    result = current;
                    break;
                }
            }
            return result;
        }
        private void OnDestroy()
        {
            allICs.Clear(); allICs = null;
        }
    }
}
