using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

/*
 * 调试性能显示
 */

public class DisplayInfoUi : MonoBehaviour
{
    /// <summary>
    /// FPS计算时间
    /// </summary>
    public float fpsMeasuringDelta = 1.0f;
    public float profileMeasuringDelta = 5.0f;
    public Text textDisplayFps;
    public Text textDisplay;
    public Text textDisplayStatic;
    public RectTransform pos;

    private float timePassed2;
    private float timePassed;
    private int m_FrameCount = 0;
    private float m_FPS = 0.0f;
    private long TotalAllocatedMem;
    private long TotalReservedMem;
    private long TotalUnusedReserved;
    private long usedHeapSize;
    private bool profilerCanuse = false;


    private void Start()
    {
        Debug.developerConsoleVisible = false;
        profilerCanuse = Profiler.supported;
        GetDeviceInfos();
        timePassed = 0.0f;
    }
    private void Update()
    {
        m_FrameCount = m_FrameCount + 1;
        timePassed = timePassed + Time.deltaTime;
        timePassed2 = timePassed2 + Time.deltaTime;
        if (timePassed > fpsMeasuringDelta)
        {
            m_FPS = m_FrameCount / timePassed;

            if (timePassed2 > profileMeasuringDelta)
            {
                timePassed = 0.0f;
                if (profilerCanuse)
                {
                    TotalAllocatedMem = Profiler.GetTotalAllocatedMemoryLong();
                    TotalReservedMem = Profiler.GetTotalReservedMemoryLong();
                    TotalUnusedReserved = Profiler.GetTotalUnusedReservedMemoryLong();
                    usedHeapSize = Profiler.usedHeapSizeLong;

                    UpdateInfos();
                }
            }

            timePassed = 0.0f;
            m_FrameCount = 0;
            UpdateFps(); 
        }
    }
    private void GetDeviceInfos()
    {
        textDisplayStatic.text =
             "<color=#cccccc>Ballance : </color><color=#66aaff>" + StaticValues.GameVersion + "</color> BulidVer :  " + StaticValues.GameBulidVersion +
             "\n<color=#cccccc>Unity : </color><color=#66aaff>" + Application.unityVersion + "</color> " + SystemInfo.deviceModel +
            "\n<color=#cccccc>Device : </color><color=#66aaff>" + SystemInfo.deviceName + "</color> " + Application.platform +
            "\n<color=#cccccc>Mem size : </color>" + SystemInfo.systemMemorySize + " MB" +
            "\n<color=#cccccc>GraphicsDevice : </color><color=#66aaff>" + SystemInfo.graphicsDeviceName + "</color> " + SystemInfo.graphicsDeviceType +
            "\n<color=#cccccc>GraphicsDeviceVendor : </color>" + SystemInfo.graphicsDeviceVendor + " (Ver : " + SystemInfo.graphicsDeviceVersion + ")" +
             "\n<color=#cccccc>GraphicsDevice : </color>" + SystemInfo.graphicsMemorySize + " MB";
    }
    private void UpdateFps()
    {
        string smfps = "";
        if (m_FPS < 10)
            smfps = "<color=#FF4500>" + m_FPS.ToString("0.00") + "</color>";
        else if (m_FPS < 20)
            smfps = "<color=#FF8C00>" + m_FPS.ToString("0.00") + "</color>";
        else if (m_FPS < 30)
            smfps = "<color=#FFC125>" + m_FPS.ToString("0.00") + "</color>";
        else if (m_FPS < 40)
            smfps = "<color=#FFFF00>" + m_FPS.ToString("0.00") + "</color>";
        else if (m_FPS < 50)
            smfps = "<color=#C0FF3E>" + m_FPS.ToString("0.00") + "</color>";
        else if (m_FPS < 55)
            smfps = "<color=#00FF00>" + m_FPS.ToString("0.00") + "</color>";
        else if (m_FPS < 65)
            smfps = "<color=#54FF9F>" + m_FPS.ToString("0.00") + "</color>";
        else if (m_FPS >= 65)
            smfps = "<color=#76EEC6>" + m_FPS.ToString("0.00") + "</color>";
        else smfps = m_FPS.ToString("0.00");

        textDisplayFps.text = "<color=#cccccc>FPS : </color>" + smfps;
    }
    private void UpdateInfos()
    {       
        if (profilerCanuse)
        {
            textDisplay.text =
                "<color=#cccccc>MemAllocated : </color>" +
                (TotalAllocatedMem / 1000000d).ToString("0.00") + "M\n <color=#cccccc>MemReserved : </color>" +
                (TotalReservedMem / 1000000d).ToString("0.00") + "M\n<color=#cccccc>MemUnusedReserved</color> : " +
                (TotalUnusedReserved / 1000000d).ToString("0.00") + "M\n<color=#cccccc>Mem used : </color><color=#66eeeeff>" +
                ((TotalAllocatedMem) / (double)TotalReservedMem * 100).ToString("0.0") + "%</color>\n<color=#cccccc>Used Heap : </color>" +
                (usedHeapSize / 1000000d).ToString("0.00") + "M";
        }
        else textDisplay.text = "\n<color=#F4A460>性能工具现在不可用</color>";
    }
    public void SetPos(bool b)
    {
        if(b)
        {
            pos.anchoredPosition = new Vector2(pos.anchoredPosition.x, -265f);
        }
        else
        {
            pos.anchoredPosition = new Vector2(pos.anchoredPosition.x, -221.9f);
        }
    }
}
