using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/*
 * 小节 管理器
 * 
 */

namespace Assets.Scripts.GameCore
{
    /// <summary>
    /// 小节 管理器
    /// </summary>
    public class SectorManager : MonoBehaviour
    {
#if UNITY_STANDALONE
        public const int MAX_SECTOR = 16;
#elif UNITY_ANDROID || UNITY_IOS
        public const int MAX_SECTOR = 8;
#endif

        public SectorManager()
        {
            GlobalMediator.SetSystemServices(GameServices.SectorManager, this);
        }

        //这三个方法由LevelLoader调用。初始化所有小节。
        public bool AddSector(GameObject PrResetPoint, GameObject PcCheckPoint, string[] SectorObjDif)
        {
            if (PrResetPoint != null)
            {
                if (sectorObjects[allSector] == null)
                    sectorObjects[allSector] = new Setor();
                sectorObjects[allSector].PcCheckPoint = PcCheckPoint;
                sectorObjects[allSector].PrResetPoint = PrResetPoint;
                sectorObjects[allSector].SectorObjDif = SectorObjDif;
                allSector++;
                return true; 
            }
            return false;
        }
        public int ObjInSector(string objOrginalName)
        {
            for (int i = 1; i <= allSector; i++)
            {
                if (sectorObjects[i].SectorObjDif.Contains(objOrginalName))
                    return i;
            }
            return -1;
        }
        public bool AddObjToSector(GameObject g, int sector)
        {
            if(sector<1|| sector>allSector) return false;
            Modul m = g.GetComponent<Modul>();
            if (m != null)
            {
                if (!sectorObjects[sector].objects.Contains(m))
                {
                    sectorObjects[sector].objects.Add(m);
                    return true;
                }
            }
            return false;
        }

        private class Setor
        {
            public Setor()
            {
                objects = new List<Modul>();
            }

            public string[] SectorObjDif { get; set; }
            public GameObject PrResetPoint { get; set; }
            public GameObject PcCheckPoint { get; set; }

            public List<Modul> objects { get; private set; }

            public void Add(Modul g)
            {
                objects.Add(g);
            }
            public void Remove(Modul g)
            {
                objects.Remove(g);
            }
            public void Clear() { objects.Clear(); }
        }

        private int allSector = 1;
        private int currentSector = 1;
        private Setor[] sectorObjects = new Setor[MAX_SECTOR];

        /// <summary>
        /// 当前关卡所有小节数。
        /// </summary>
        public int AllSector { get { return allSector; } }
        /// <summary>
        /// 关卡当前小节。
        /// </summary>
        public int CurrentSector { get { return currentSector; } }


    }
}
