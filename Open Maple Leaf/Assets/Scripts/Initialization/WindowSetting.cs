using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Initialization
{
    public class WindowSetting : MonoBehaviour
    {
        private void Awake()
        {
            // 设置目标帧率为120帧每秒
            Application.targetFrameRate = 120;
            // 禁用垂直同步
            QualitySettings.vSyncCount = 0;
        }
        
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("dwmapi.dll", PreserveSig = false)]
        private static extern void DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

        void Start()
        {
            IntPtr hwnd = GetForegroundWindow();
            if (hwnd != IntPtr.Zero)
            {
                int useImmersiveDarkMode = 1; // 1 to enable, 0 to disable
                DwmSetWindowAttribute(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE, ref useImmersiveDarkMode, sizeof(int));
            }
        }
    }
}
